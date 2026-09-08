using DarkestDungeon.Application.Classes;
using DarkestDungeon.Application.Habilidades;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Application.Habilidades;

internal static class HabilidadeMapper
{
    public static HabilidadeResumoDto ParaResumo(Habilidade habilidade) =>
        new(habilidade.Id, CategoriaDe(habilidade), habilidade.NomeExibicao, habilidade.NomeOriginal);

    public static HabilidadeDetalheDto ParaDetalhe(Habilidade habilidade, IEnumerable<ClasseHabilidade> associacoes)
    {
        var efeitos = ObterEfeitos(habilidade).Select(e =>
            new EfeitoDeHabilidadeDto(e.NomeDoEfeito, e.Alvo, e.Valor, e.Unidade, e.ChanceBase, e.DuracaoEmRodadas)).ToList();

        var limite = ObterLimite(habilidade) is { } lim ? new LimitePorUsoDto(lim.Escopo, lim.MaximoUsos) : null;

        HabilidadeCombateDto? combate = habilidade is HabilidadeDeCombate hc
            ? new HabilidadeCombateDto(hc.PosicoesValidas, hc.PosicoesQueAtinge, hc.AlvoEmArea, hc.ModificadorDano, hc.ModificadorAcerto, hc.ModificadorCritico)
            : null;

        HabilidadeAcampamentoDto? acampamento = habilidade is HabilidadeDeAcampamento ha
            ? new HabilidadeAcampamentoDto(ha.CustoDeDescanso, ha.Alvo)
            : null;

        HabilidadeInimigoDto? inimigo = habilidade is HabilidadeDeInimigo hi
            ? new HabilidadeInimigoDto(hi.CondicaoDeAparecer, hi.ChanceDeExecucao)
            : null;

        var classesIds = associacoes.Where(a => a.HabilidadeId == habilidade.Id).Select(a => a.ClasseId).ToList();

        return new HabilidadeDetalheDto(
            habilidade.Id,
            CategoriaDe(habilidade),
            habilidade.NomeExibicao,
            habilidade.NomeOriginal,
            habilidade.Descricao,
            efeitos,
            limite,
            combate,
            acampamento,
            inimigo,
            classesIds);
    }

    public static CategoriaDeHabilidadeDto CategoriaDe(Habilidade habilidade) => habilidade switch
    {
        HabilidadeDeCombate => CategoriaDeHabilidadeDto.Combate,
        HabilidadeDeAcampamento => CategoriaDeHabilidadeDto.Acampamento,
        HabilidadeDeInimigo => CategoriaDeHabilidadeDto.Inimigo,
        _ => throw new InvalidOperationException($"Discriminator inesperado: {habilidade.GetType().Name}"),
    };

    private static IReadOnlyList<EfeitoDeHabilidade> ObterEfeitos(Habilidade habilidade) => habilidade switch
    {
        HabilidadeDeHeroi h => h.Efeitos,
        HabilidadeDeInimigo i => i.Efeitos,
        _ => Array.Empty<EfeitoDeHabilidade>(),
    };

    private static LimitePorUso? ObterLimite(Habilidade habilidade) => habilidade switch
    {
        HabilidadeDeHeroi h => h.LimitePorUso,
        _ => null,
    };
}
