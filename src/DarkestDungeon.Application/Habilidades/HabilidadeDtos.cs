using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Application.Habilidades;

public enum CategoriaDeHabilidadeDto
{
    Combate,
    Acampamento,
    Inimigo,
}

public sealed record EfeitoDeHabilidadeDto(
    string NomeDoEfeito,
    AlvoDeEfeito Alvo,
    decimal Valor,
    UnidadeDeEfeito Unidade,
    decimal ChanceBase,
    int? DuracaoEmRodadas);

public sealed record LimitePorUsoDto(EscopoDeLimite Escopo, int MaximoUsos);

public sealed record HabilidadeResumoDto(Guid Id, CategoriaDeHabilidadeDto Categoria, string NomeExibicao, string NomeOriginal);

public sealed record HabilidadeCombateDto(
    IReadOnlyList<int> PosicoesValidas,
    IReadOnlyList<int> PosicoesQueAtinge,
    bool AlvoEmArea,
    decimal ModificadorDano,
    decimal ModificadorAcerto,
    decimal ModificadorCritico);

public sealed record HabilidadeAcampamentoDto(int CustoDeDescanso, AlvoDeAcampamento Alvo);

public sealed record HabilidadeInimigoDto(string? CondicaoDeAparecer, decimal? ChanceDeExecucao);

public sealed record HabilidadeDetalheDto(
    Guid Id,
    CategoriaDeHabilidadeDto Categoria,
    string NomeExibicao,
    string NomeOriginal,
    string Descricao,
    IReadOnlyList<EfeitoDeHabilidadeDto> Efeitos,
    LimitePorUsoDto? LimitePorUso,
    HabilidadeCombateDto? Combate,
    HabilidadeAcampamentoDto? Acampamento,
    HabilidadeInimigoDto? Inimigo,
    IReadOnlyList<Guid> ClassesIds);
