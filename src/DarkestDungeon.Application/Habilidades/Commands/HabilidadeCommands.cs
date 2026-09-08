using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Application.Habilidades.Commands;

public sealed record EfeitoDeHabilidadeCommand(
    string NomeDoEfeito,
    AlvoDeEfeito Alvo,
    decimal Valor,
    UnidadeDeEfeito Unidade,
    decimal ChanceBase,
    int? DuracaoEmRodadas);

public sealed record LimitePorUsoCommand(EscopoDeLimite Escopo, int MaximoUsos);

public sealed record CriarHabilidadeDeCombateCommand(
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
    IReadOnlyCollection<EfeitoDeHabilidadeCommand> Efeitos,
    LimitePorUsoCommand? LimitePorUso);

public sealed record CriarHabilidadeDeAcampamentoCommand(
    string NomeExibicao,
    string NomeOriginal,
    string Descricao,
    IReadOnlyCollection<Guid> ClassesIds,
    int CustoDeDescanso,
    AlvoDeAcampamento Alvo,
    IReadOnlyCollection<EfeitoDeHabilidadeCommand> Efeitos,
    LimitePorUsoCommand? LimitePorUso);

public sealed record CriarHabilidadeDeInimigoCommand(
    string NomeExibicao,
    string NomeOriginal,
    string Descricao,
    string? CondicaoDeAparecer,
    decimal? ChanceDeExecucao,
    IReadOnlyCollection<EfeitoDeHabilidadeCommand> Efeitos);
