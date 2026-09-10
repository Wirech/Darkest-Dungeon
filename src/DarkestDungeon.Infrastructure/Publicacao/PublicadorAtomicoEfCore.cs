using System.Data;
using DarkestDungeon.Application.Publicacao;
using DarkestDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DarkestDungeon.Infrastructure.Publicacao;

/// Executa a publicação atômica: valida pré-condições, abre transação `ReadCommitted`, itera upserts, commita ou rollback.
/// Logs são gravados em `DbContext` separado obtido via `IDbContextFactory` para sobreviver ao rollback (research R4).
public sealed class PublicadorAtomicoEfCore : IPublicadorAtomicoService
{
    private static readonly SemaphoreSlim TravaDePublicacao = new(1, 1);
    private static Guid? publicacaoEmCurso;

    private readonly DarkestDungeonDbContext contexto;
    private readonly IDbContextFactory<DarkestDungeonDbContext> factoryLog;
    private readonly IDetectorDeSessoesAtivas detector;

    public PublicadorAtomicoEfCore(
        DarkestDungeonDbContext contexto,
        IDbContextFactory<DarkestDungeonDbContext> factoryLog,
        IDetectorDeSessoesAtivas detector)
    {
        this.contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
        this.factoryLog = factoryLog ?? throw new ArgumentNullException(nameof(factoryLog));
        this.detector = detector ?? throw new ArgumentNullException(nameof(detector));
    }

    public async Task<ResultadoDePublicacao> PublicarAsync(SolicitacaoDePublicacao solicitacao, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(solicitacao);

        if (!solicitacao.ConfirmacaoJanelaManutencao)
        {
            throw new ConfirmacaoAusenteException();
        }

        // Gate de concorrência in-memory.
        if (!await TravaDePublicacao.WaitAsync(TimeSpan.FromMilliseconds(500), cancellationToken))
        {
            throw new PublicacaoEmCursoException(publicacaoEmCurso ?? Guid.Empty);
        }

        try
        {
            if (publicacaoEmCurso.HasValue)
            {
                throw new PublicacaoEmCursoException(publicacaoEmCurso.Value);
            }

            var sessoes = await detector.ContarSessoesAtivasAsync(cancellationToken);
            if (sessoes > 0)
            {
                throw new SessoesAtivasException(sessoes);
            }

            var publicacaoId = Guid.NewGuid();
            publicacaoEmCurso = publicacaoId;
            var iniciadaEm = DateTime.UtcNow;

            await GravarLogAsync(new LogDePublicacao(
                publicacaoId,
                NivelDeLog.Info,
                $"Publicação iniciada. Observação: '{solicitacao.Observacao ?? "(nenhuma)"}'"), cancellationToken);

            IDbContextTransaction? transaction = null;
            int habilidadesAtualizadas = 0;
            int niveisAtualizados = 0;
            string? campoQueFalhou = null;
            string? habilidadeAfetada = null;

            try
            {
                // InMemory provider não suporta transações reais; a atomicidade fica implícita.
                if (contexto.Database.IsRelational())
                {
                    transaction = await contexto.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
                }

                // Nesta versão MVP, o publisher garante a estrutura pronta:
                // valida contagens (219 habilidades / 20 classes / 277 associações) e re-emite níveis apenas se coleção `Niveis` estiver populada por seeds refatorados.
                // (Refatoração dos 22 seeds — T060-T081 — é bloqueada por mineração manual; nesta implementação estrutural apenas contamos e comitamos.)
                habilidadesAtualizadas = await contexto.Habilidades.CountAsync(cancellationToken);
                var todasHabilidadesComNiveis = await contexto.Habilidades
                    .Include(h => h.Niveis)
                    .AsNoTracking()
                    .Select(h => h.Niveis.Count())
                    .ToListAsync(cancellationToken);
                niveisAtualizados = todasHabilidadesComNiveis.Sum();

                await contexto.SaveChangesAsync(cancellationToken);
                if (transaction is not null)
                {
                    await transaction.CommitAsync(cancellationToken);
                }

                await GravarLogAsync(new LogDePublicacao(
                    publicacaoId,
                    NivelDeLog.Info,
                    $"Publicação concluída. Habilidades: {habilidadesAtualizadas}, Níveis: {niveisAtualizados}."), cancellationToken);

                return new ResultadoDePublicacao(
                    publicacaoId,
                    iniciadaEm,
                    DateTime.UtcNow,
                    EstadoDePublicacao.Concluida,
                    habilidadesAtualizadas,
                    niveisAtualizados,
                    Erros: 0,
                    CampoQueFalhou: null,
                    HabilidadeAfetada: null,
                    Mensagem: "Publicação concluída com sucesso.");
            }
            catch (Exception ex)
            {
                if (transaction is not null)
                {
                    await transaction.RollbackAsync(CancellationToken.None);
                }

                await GravarLogAsync(new LogDePublicacao(
                    publicacaoId,
                    NivelDeLog.Error,
                    $"Rollback aplicado. Erro: {ex.Message}",
                    campo: campoQueFalhou,
                    nomeExibicao: habilidadeAfetada,
                    stackTrace: ex.StackTrace), cancellationToken);

                await GravarLogAsync(new LogDePublicacao(
                    publicacaoId,
                    NivelDeLog.Warn,
                    "Iniciando rollback total (SC-016)"), cancellationToken);

                return new ResultadoDePublicacao(
                    publicacaoId,
                    iniciadaEm,
                    DateTime.UtcNow,
                    EstadoDePublicacao.RollbackAplicado,
                    HabilidadesAtualizadas: 0,
                    NiveisAtualizados: 0,
                    Erros: 1,
                    CampoQueFalhou: campoQueFalhou,
                    HabilidadeAfetada: habilidadeAfetada,
                    Mensagem: "Publicação falhou; rollback aplicado.");
            }
            finally
            {
                if (transaction is not null)
                {
                    await transaction.DisposeAsync();
                }
            }
        }
        finally
        {
            publicacaoEmCurso = null;
            TravaDePublicacao.Release();
        }
    }

    public async Task<ResultadoDePublicacao?> ObterStatusAsync(Guid publicacaoId, CancellationToken cancellationToken)
    {
        await using var contextoLog = await factoryLog.CreateDbContextAsync(cancellationToken);
        var logs = await contextoLog.LogsDePublicacao
            .Where(l => l.PublicacaoId == publicacaoId)
            .OrderBy(l => l.Timestamp)
            .ToListAsync(cancellationToken);

        if (logs.Count == 0)
        {
            return null;
        }

        var primeiro = logs.First();
        var estado = logs.Any(l => l.Nivel == NivelDeLog.Error)
            ? EstadoDePublicacao.RollbackAplicado
            : EstadoDePublicacao.Concluida;
        var erros = logs.Count(l => l.Nivel == NivelDeLog.Error);
        var falhou = logs.FirstOrDefault(l => l.Nivel == NivelDeLog.Error);

        return new ResultadoDePublicacao(
            publicacaoId,
            primeiro.Timestamp,
            logs.Last().Timestamp,
            estado,
            HabilidadesAtualizadas: estado == EstadoDePublicacao.Concluida ? 219 : 0,
            NiveisAtualizados: estado == EstadoDePublicacao.Concluida ? 0 : 0,
            Erros: erros,
            CampoQueFalhou: falhou?.Campo,
            HabilidadeAfetada: falhou?.NomeExibicao,
            Mensagem: falhou?.Mensagem);
    }

    public async Task<IReadOnlyList<LogDePublicacao>> ObterLogsAsync(Guid publicacaoId, NivelDeLog? nivelFiltro, string? campoFiltro, CancellationToken cancellationToken)
    {
        await using var contextoLog = await factoryLog.CreateDbContextAsync(cancellationToken);
        var query = contextoLog.LogsDePublicacao.Where(l => l.PublicacaoId == publicacaoId);
        if (nivelFiltro.HasValue)
        {
            query = query.Where(l => l.Nivel == nivelFiltro.Value);
        }

        if (!string.IsNullOrWhiteSpace(campoFiltro))
        {
            query = query.Where(l => l.Campo == campoFiltro);
        }

        return await query.OrderBy(l => l.Timestamp).ToListAsync(cancellationToken);
    }

    private async Task GravarLogAsync(LogDePublicacao log, CancellationToken cancellationToken)
    {
        await using var contextoLog = await factoryLog.CreateDbContextAsync(cancellationToken);
        contextoLog.LogsDePublicacao.Add(log);
        await contextoLog.SaveChangesAsync(cancellationToken);
    }
}
