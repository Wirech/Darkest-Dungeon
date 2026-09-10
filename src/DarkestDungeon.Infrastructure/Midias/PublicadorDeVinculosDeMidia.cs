using System.Data;
using DarkestDungeon.Application.Midias;
using DarkestDungeon.Domain.Itens;
using DarkestDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DarkestDungeon.Infrastructure.Midias;

public sealed class PublicadorDeVinculosDeMidia : IPublicadorDeVinculosDeMidia
{
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<CategoriaDeMidiaDeItem, Guid> EmCurso = new();

    private readonly DarkestDungeonDbContext contexto;
    private readonly ILeitorDeInventarioDeMidias leitor;

    public PublicadorDeVinculosDeMidia(DarkestDungeonDbContext contexto, ILeitorDeInventarioDeMidias leitor)
    {
        this.contexto = contexto;
        this.leitor = leitor;
    }

    public async Task<ResultadoDePublicacaoDeVinculos> PublicarAsync(
        SolicitacaoDePublicacaoDeVinculos solicitacao,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(solicitacao);
        if (!CategoriaDeMidiaParser.TentarAnalisar(solicitacao.Categoria, out var categoria))
        {
            throw new CategoriaDeMidiaInvalidaException();
        }

        if (solicitacao.Observacao is { Length: > 500 })
        {
            throw new ArgumentException("Observação deve ter no máximo 500 caracteres.", nameof(solicitacao));
        }

        var publicacao = new PublicacaoDeVinculosDeMidia(categoria, DateTimeOffset.UtcNow);
        if (!EmCurso.TryAdd(categoria, publicacao.Id))
        {
            throw new PublicacaoDeVinculosEmCursoException(EmCurso[categoria], categoria);
        }

        Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? transacao = null;
        try
        {
            var inventario = await leitor.LerAsync(cancellationToken);
            if (inventario is null)
            {
                throw new InventarioDeMidiasAusenteException();
            }

            contexto.PublicacoesDeVinculosDeMidia.Add(publicacao);

            if (contexto.Database.IsRelational())
            {
                transacao = await contexto.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            }

            var totais = await AplicarCategoriaAsync(categoria, inventario, cancellationToken);
            publicacao.Concluir(DateTimeOffset.UtcNow, totais.Itens, totais.Ok, totais.Pendentes, totais.Novos);
            await contexto.SaveChangesAsync(cancellationToken);
            if (transacao is not null)
            {
                await transacao.CommitAsync(cancellationToken);
            }

            return Mapear(publicacao);
        }
        catch (CategoriaDeMidiaInvalidaException)
        {
            throw;
        }
        catch (PublicacaoDeVinculosEmCursoException)
        {
            throw;
        }
        catch (InventarioDeMidiasAusenteException)
        {
            throw;
        }
        catch (Exception)
        {
            if (transacao is not null)
            {
                await transacao.RollbackAsync(cancellationToken);
            }

            publicacao.MarcarRollback(DateTimeOffset.UtcNow);
            contexto.ChangeTracker.Clear();
            contexto.PublicacoesDeVinculosDeMidia.Add(publicacao);
            await contexto.SaveChangesAsync(cancellationToken);
            throw new PublicacaoDeVinculosFalhouException(publicacao.Id, categoria);
        }
        finally
        {
            if (transacao is not null)
            {
                await transacao.DisposeAsync();
            }

            EmCurso.TryRemove(categoria, out _);
        }
    }

    public async Task<ResultadoDePublicacaoDeVinculos?> ObterStatusAsync(Guid publicacaoId, CancellationToken cancellationToken)
    {
        var entidade = await contexto.PublicacoesDeVinculosDeMidia.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == publicacaoId, cancellationToken);
        return entidade is null ? null : Mapear(entidade);
    }

    private async Task<(int Itens, int Ok, int Pendentes, int Novos)> AplicarCategoriaAsync(
        CategoriaDeMidiaDeItem categoria,
        InventarioDeMidiasDeEquipamento inventario,
        CancellationToken cancellationToken)
    {
        var chave = CategoriaDeMidiaParser.Inventario(categoria);
        var daCategoria = inventario.Arquivos
            .Where(a => a.Categoria.Equals(chave, StringComparison.OrdinalIgnoreCase))
            .ToArray();
        var pngs = daCategoria
            .Where(a => a.CaminhoOrigem.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return categoria switch
        {
            CategoriaDeMidiaDeItem.Arma => await PublicarArmasAsync(daCategoria, cancellationToken),
            CategoriaDeMidiaDeItem.Armadura => await PublicarArmadurasAsync(daCategoria, cancellationToken),
            CategoriaDeMidiaDeItem.Acessorio => await PublicarAcessoriosAsync(pngs, cancellationToken),
            CategoriaDeMidiaDeItem.ItemDeAcampamento => await PublicarSimplesAsync<ItemDeAcampamento>(
                pngs, (n, o, d, id) => new ItemDeAcampamento(n, o, d, id: id), cancellationToken),
            CategoriaDeMidiaDeItem.Consumivel => await PublicarSimplesAsync<Consumivel>(
                pngs, (n, o, d, id) => new Consumivel(n, o, d, id: id), cancellationToken),
            _ => (0, 0, 0, 0),
        };
    }

    private async Task<(int Itens, int Ok, int Pendentes, int Novos)> PublicarArmasAsync(
        IReadOnlyList<ArquivoDeInventarioDeMidia> arquivos,
        CancellationToken cancellationToken)
    {
        var armas = await contexto.Itens.OfType<Arma>().ToListAsync(cancellationToken);
        var ok = 0;
        var pendentes = 0;
        foreach (var arma in armas)
        {
            foreach (var nivel in arma.Niveis)
            {
                var midia = ResolvedorDeMidiaDeNivel.Resolver(arquivos, arma.ClasseElegivel, nivel.Nivel, arma: true);
                nivel.DefinirMidia(midia);
                if (midia.Status == StatusDeMidia.OK) ok++; else pendentes++;
            }
        }

        return (armas.Count, ok, pendentes, 0);
    }

    private async Task<(int Itens, int Ok, int Pendentes, int Novos)> PublicarArmadurasAsync(
        IReadOnlyList<ArquivoDeInventarioDeMidia> arquivos,
        CancellationToken cancellationToken)
    {
        var armaduras = await contexto.Itens.OfType<Armadura>().ToListAsync(cancellationToken);
        var ok = 0;
        var pendentes = 0;
        foreach (var armadura in armaduras)
        {
            foreach (var nivel in armadura.Niveis)
            {
                var midia = ResolvedorDeMidiaDeNivel.Resolver(arquivos, armadura.ClasseElegivel, nivel.Nivel, arma: false);
                nivel.DefinirMidia(midia);
                if (midia.Status == StatusDeMidia.OK) ok++; else pendentes++;
            }
        }

        return (armaduras.Count, ok, pendentes, 0);
    }

    private async Task<(int Itens, int Ok, int Pendentes, int Novos)> PublicarAcessoriosAsync(
        IReadOnlyList<ArquivoDeInventarioDeMidia> arquivos,
        CancellationToken cancellationToken)
    {
        var existentes = await contexto.Itens.OfType<Acessorio>().ToListAsync(cancellationToken);
        var ok = 0;
        var pendentes = 0;
        var novos = 0;
        var vistos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var arquivo in arquivos)
        {
            var stem = Path.GetFileNameWithoutExtension(arquivo.Identificador ?? arquivo.CaminhoOrigem);
            if (!vistos.Add(stem))
            {
                continue;
            }

            var (item, novo) = ResolvedorDeAcessorios.ResolverOuCriar(existentes, arquivo);
            if (novo)
            {
                contexto.Itens.Add(item);
                existentes = existentes.Append(item).ToList();
                novos++;
            }

            if (item.Midia.Status == StatusDeMidia.OK) ok++; else pendentes++;
        }

        foreach (var restante in existentes.Where(a => a.Midia.Status != StatusDeMidia.OK))
        {
            pendentes++;
        }

        return (existentes.Count, ok, pendentes, novos);
    }

    private async Task<(int Itens, int Ok, int Pendentes, int Novos)> PublicarSimplesAsync<TItem>(
        IReadOnlyList<ArquivoDeInventarioDeMidia> arquivos,
        Func<string, string, string, Guid, TItem> factory,
        CancellationToken cancellationToken)
        where TItem : Item
    {
        var existentes = await contexto.Itens.OfType<TItem>().ToListAsync(cancellationToken);
        var ok = 0;
        var pendentes = 0;
        var novos = 0;
        var vistos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var arquivo in arquivos)
        {
            var stem = Path.GetFileNameWithoutExtension(arquivo.Identificador ?? arquivo.CaminhoOrigem);
            if (stem.Length > 80) stem = stem[..80];
            if (!vistos.Add(stem)) continue;

            var item = existentes.FirstOrDefault(i => i.NomeOriginal.Equals(stem, StringComparison.OrdinalIgnoreCase));
            var midia = ResolvedorDeAcessorios.MidiaDePng(arquivo);
            if (item is null)
            {
                item = factory(stem, stem, stem, Guid.NewGuid());
                DefinirMidia(item, midia);
                contexto.Itens.Add(item);
                existentes.Add(item);
                novos++;
            }
            else
            {
                DefinirMidia(item, midia);
            }

            if (midia.Status == StatusDeMidia.OK) ok++; else pendentes++;
        }

        return (existentes.Count, ok, pendentes, novos);
    }

    private static void DefinirMidia(Item item, MidiaDeItem midia)
    {
        switch (item)
        {
            case ItemDeAcampamento acampamento: acampamento.DefinirMidia(midia); break;
            case Consumivel consumivel: consumivel.DefinirMidia(midia); break;
            case Acessorio acessorio: acessorio.DefinirMidia(midia); break;
        }
    }

    private static ResultadoDePublicacaoDeVinculos Mapear(PublicacaoDeVinculosDeMidia entidade) => new(
        entidade.Id,
        entidade.Categoria,
        entidade.Estado,
        entidade.IniciadaEm,
        entidade.ConcluidaEm,
        entidade.ItensAtualizados,
        entidade.VinculosOk,
        entidade.VinculosPendentes,
        entidade.AcessoriosNovos);
}
