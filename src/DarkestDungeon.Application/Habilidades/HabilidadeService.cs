using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Application.Habilidades.Commands;
using DarkestDungeon.Application.Validation;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Application.Habilidades;

public sealed class HabilidadeService : IHabilidadeService
{
    private readonly IHabilidadeRepository habilidades;
    private readonly IClasseRepository classes;

    public HabilidadeService(IHabilidadeRepository habilidades, IClasseRepository classes)
    {
        this.habilidades = habilidades;
        this.classes = classes;
    }

    public async Task<ResultadoOperacao<HabilidadeDetalheDto>> ObterAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var habilidade = await habilidades.ObterPorIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (habilidade is null)
        {
            return ResultadoOperacao<HabilidadeDetalheDto>.NaoEncontrado($"Habilidade '{id}' não encontrada.");
        }

        var associacoes = await ObterAssociacoesAsync(id, cancellationToken).ConfigureAwait(false);
        return ResultadoOperacao<HabilidadeDetalheDto>.Ok(HabilidadeMapper.ParaDetalhe(habilidade, associacoes));
    }

    public async Task<ResultadoOperacao<IReadOnlyCollection<HabilidadeResumoDto>>> ListarAsync(CategoriaDeHabilidadeDto? categoria, Guid? classeId, CancellationToken cancellationToken = default)
    {
        var lista = await habilidades.ListarAsync(categoria, classeId, cancellationToken).ConfigureAwait(false);
        var resumo = lista.Select(HabilidadeMapper.ParaResumo).ToArray();
        return ResultadoOperacao<IReadOnlyCollection<HabilidadeResumoDto>>.Ok(resumo);
    }

    public async Task<ResultadoOperacao<HabilidadeDetalheDto>> CriarDeCombateAsync(CriarHabilidadeDeCombateCommand command, CancellationToken cancellationToken = default)
    {
        var validacao = await ValidarNomeUnicoAsync(command.NomeExibicao, cancellationToken).ConfigureAwait(false);
        if (validacao is { } erro)
        {
            return erro;
        }

        var associacoes = await ValidarClassesEMontarAssociacoesAsync(command.ClassesIds, cancellationToken).ConfigureAwait(false);
        if (associacoes.Erro is { } falha)
        {
            return falha;
        }

        try
        {
            var habilidade = new HabilidadeDeCombate(
                command.NomeExibicao,
                command.NomeOriginal,
                command.Descricao,
                command.PosicoesValidas,
                command.PosicoesQueAtinge,
                command.AlvoEmArea,
                command.ModificadorDano,
                command.ModificadorAcerto,
                command.ModificadorCritico,
                command.Efeitos.Select(MapearEfeito),
                MapearLimite(command.LimitePorUso));

            return await PersistirAsync(habilidade, associacoes.ClassesIds!, cancellationToken).ConfigureAwait(false);
        }
        catch (ArgumentException ex)
        {
            return ResultadoOperacao<HabilidadeDetalheDto>.Invalido(ex.Message, new ErroOperacao(ex.ParamName ?? "requisicao", ex.Message));
        }
    }

    public async Task<ResultadoOperacao<HabilidadeDetalheDto>> CriarDeAcampamentoAsync(CriarHabilidadeDeAcampamentoCommand command, CancellationToken cancellationToken = default)
    {
        var validacao = await ValidarNomeUnicoAsync(command.NomeExibicao, cancellationToken).ConfigureAwait(false);
        if (validacao is { } erro)
        {
            return erro;
        }

        var associacoes = await ValidarClassesEMontarAssociacoesAsync(command.ClassesIds, cancellationToken).ConfigureAwait(false);
        if (associacoes.Erro is { } falha)
        {
            return falha;
        }

        try
        {
            var habilidade = new HabilidadeDeAcampamento(
                command.NomeExibicao,
                command.NomeOriginal,
                command.Descricao,
                command.CustoDeDescanso,
                command.Alvo,
                command.Efeitos.Select(MapearEfeito),
                MapearLimite(command.LimitePorUso));

            return await PersistirAsync(habilidade, associacoes.ClassesIds!, cancellationToken).ConfigureAwait(false);
        }
        catch (ArgumentException ex)
        {
            return ResultadoOperacao<HabilidadeDetalheDto>.Invalido(ex.Message, new ErroOperacao(ex.ParamName ?? "requisicao", ex.Message));
        }
    }

    public async Task<ResultadoOperacao<HabilidadeDetalheDto>> CriarDeInimigoAsync(CriarHabilidadeDeInimigoCommand command, CancellationToken cancellationToken = default)
    {
        var validacao = await ValidarNomeUnicoAsync(command.NomeExibicao, cancellationToken).ConfigureAwait(false);
        if (validacao is { } erro)
        {
            return erro;
        }

        try
        {
            var habilidade = new HabilidadeDeInimigo(
                command.NomeExibicao,
                command.NomeOriginal,
                command.Descricao,
                command.Efeitos.Select(MapearEfeito),
                command.CondicaoDeAparecer,
                command.ChanceDeExecucao);

            return await PersistirAsync(habilidade, Array.Empty<Guid>(), cancellationToken).ConfigureAwait(false);
        }
        catch (ArgumentException ex)
        {
            return ResultadoOperacao<HabilidadeDetalheDto>.Invalido(ex.Message, new ErroOperacao(ex.ParamName ?? "requisicao", ex.Message));
        }
    }

    private async Task<ResultadoOperacao<HabilidadeDetalheDto>?> ValidarNomeUnicoAsync(string nomeExibicao, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(nomeExibicao))
        {
            return ResultadoOperacao<HabilidadeDetalheDto>.Invalido(
                "Nome de exibição é obrigatório.",
                new ErroOperacao("nomeExibicao", "Nome de exibição é obrigatório."));
        }

        var existente = await habilidades.ObterPorNomeExibicaoAsync(nomeExibicao.Trim(), cancellationToken).ConfigureAwait(false);
        if (existente is not null)
        {
            return ResultadoOperacao<HabilidadeDetalheDto>.Invalido(
                $"Já existe uma habilidade com o nome '{nomeExibicao}'.",
                new ErroOperacao("nomeExibicao", "Nome de habilidade deve ser único no catálogo global."));
        }

        return null;
    }

    private async Task<(IReadOnlyList<Guid>? ClassesIds, ResultadoOperacao<HabilidadeDetalheDto>? Erro)> ValidarClassesEMontarAssociacoesAsync(
        IReadOnlyCollection<Guid> classesIds,
        CancellationToken cancellationToken)
    {
        if (classesIds is null || classesIds.Count == 0)
        {
            return (null, ResultadoOperacao<HabilidadeDetalheDto>.Invalido(
                "Habilidade de herói deve ser associada a pelo menos uma classe.",
                new ErroOperacao("classesIds", "Informe ao menos uma classe elegível.")));
        }

        var validas = new List<Guid>();
        foreach (var classeId in classesIds.Distinct())
        {
            if (!await classes.ExisteAsync(classeId, cancellationToken).ConfigureAwait(false))
            {
                return (null, ResultadoOperacao<HabilidadeDetalheDto>.Invalido(
                    $"Classe '{classeId}' não encontrada.",
                    new ErroOperacao("classesIds", $"Classe '{classeId}' não encontrada.")));
            }

            validas.Add(classeId);
        }

        return (validas, null);
    }

    private async Task<ResultadoOperacao<HabilidadeDetalheDto>> PersistirAsync(
        Habilidade habilidade,
        IReadOnlyList<Guid> classesIds,
        CancellationToken cancellationToken)
    {
        var associacoes = classesIds
            .Select(classeId => new ClasseHabilidade(classeId, habilidade.Id))
            .ToArray();

        await habilidades.AdicionarAsync(habilidade, associacoes, cancellationToken).ConfigureAwait(false);
        return ResultadoOperacao<HabilidadeDetalheDto>.Ok(HabilidadeMapper.ParaDetalhe(habilidade, associacoes));
    }

    private async Task<IReadOnlyList<ClasseHabilidade>> ObterAssociacoesAsync(Guid habilidadeId, CancellationToken cancellationToken)
    {
        return await habilidades.ListarAssociacoesPorHabilidadeAsync(habilidadeId, cancellationToken).ConfigureAwait(false);
    }

    private static EfeitoDeHabilidade MapearEfeito(EfeitoDeHabilidadeCommand efeito) =>
        new(efeito.NomeDoEfeito, efeito.Alvo, efeito.Valor, efeito.Unidade, efeito.ChanceBase, efeito.DuracaoEmRodadas);

    private static LimitePorUso? MapearLimite(LimitePorUsoCommand? limite) =>
        limite is null ? null : new LimitePorUso(limite.Escopo, limite.MaximoUsos);
}
