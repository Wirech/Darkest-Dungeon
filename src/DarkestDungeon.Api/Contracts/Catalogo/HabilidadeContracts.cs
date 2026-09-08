using DarkestDungeon.Application.Habilidades;
using DarkestDungeon.Application.Habilidades.Commands;

namespace DarkestDungeon.Api.Contracts.Catalogo;

public sealed record EfeitoDeHabilidadeRequest(
    string NomeDoEfeito,
    DarkestDungeon.Domain.Habilidades.AlvoDeEfeito Alvo,
    decimal Valor,
    DarkestDungeon.Domain.Habilidades.UnidadeDeEfeito Unidade,
    decimal ChanceBase,
    int? DuracaoEmRodadas);

public sealed record LimitePorUsoRequest(
    DarkestDungeon.Domain.Habilidades.EscopoDeLimite Escopo,
    int MaximoUsos);

public sealed record CriarHabilidadeDeCombateRequest(
    string NomeExibicao,
    string NomeOriginal,
    string Descricao,
    IReadOnlyCollection<Guid> ClassesIds,
    IReadOnlyCollection<int> PosicoesValidas,
    IReadOnlyCollection<int> PosicoesQueAtinge,
    bool AlvoEmArea,
    decimal ModificadorDano,
    decimal ModificadorAcerto,
    decimal ModificadorCritico,
    IReadOnlyCollection<EfeitoDeHabilidadeRequest> Efeitos,
    LimitePorUsoRequest? LimitePorUso);

public sealed record CriarHabilidadeDeAcampamentoRequest(
    string NomeExibicao,
    string NomeOriginal,
    string Descricao,
    IReadOnlyCollection<Guid> ClassesIds,
    int CustoDeDescanso,
    DarkestDungeon.Domain.Habilidades.AlvoDeAcampamento Alvo,
    IReadOnlyCollection<EfeitoDeHabilidadeRequest> Efeitos,
    LimitePorUsoRequest? LimitePorUso);

public sealed record CriarHabilidadeDeInimigoRequest(
    string NomeExibicao,
    string NomeOriginal,
    string Descricao,
    string? CondicaoDeAparecer,
    decimal? ChanceDeExecucao,
    IReadOnlyCollection<EfeitoDeHabilidadeRequest> Efeitos);

internal static class HabilidadeRequestMapper
{
    public static EfeitoDeHabilidadeCommand ParaCommand(this EfeitoDeHabilidadeRequest request) =>
        new(request.NomeDoEfeito, request.Alvo, request.Valor, request.Unidade, request.ChanceBase, request.DuracaoEmRodadas);

    public static LimitePorUsoCommand? ParaCommand(this LimitePorUsoRequest? request) =>
        request is null ? null : new LimitePorUsoCommand(request.Escopo, request.MaximoUsos);

    public static CriarHabilidadeDeCombateCommand ParaCommand(this CriarHabilidadeDeCombateRequest request) =>
        new(
            request.NomeExibicao ?? string.Empty,
            request.NomeOriginal ?? string.Empty,
            request.Descricao ?? string.Empty,
            request.ClassesIds ?? Array.Empty<Guid>(),
            request.PosicoesValidas ?? Array.Empty<int>(),
            request.PosicoesQueAtinge ?? Array.Empty<int>(),
            request.AlvoEmArea,
            request.ModificadorDano,
            request.ModificadorAcerto,
            request.ModificadorCritico,
            (request.Efeitos ?? Array.Empty<EfeitoDeHabilidadeRequest>()).Select(e => e.ParaCommand()).ToArray(),
            request.LimitePorUso.ParaCommand());

    public static CriarHabilidadeDeAcampamentoCommand ParaCommand(this CriarHabilidadeDeAcampamentoRequest request) =>
        new(
            request.NomeExibicao ?? string.Empty,
            request.NomeOriginal ?? string.Empty,
            request.Descricao ?? string.Empty,
            request.ClassesIds ?? Array.Empty<Guid>(),
            request.CustoDeDescanso,
            request.Alvo,
            (request.Efeitos ?? Array.Empty<EfeitoDeHabilidadeRequest>()).Select(e => e.ParaCommand()).ToArray(),
            request.LimitePorUso.ParaCommand());

    public static CriarHabilidadeDeInimigoCommand ParaCommand(this CriarHabilidadeDeInimigoRequest request) =>
        new(
            request.NomeExibicao ?? string.Empty,
            request.NomeOriginal ?? string.Empty,
            request.Descricao ?? string.Empty,
            request.CondicaoDeAparecer,
            request.ChanceDeExecucao,
            (request.Efeitos ?? Array.Empty<EfeitoDeHabilidadeRequest>()).Select(e => e.ParaCommand()).ToArray());
}
