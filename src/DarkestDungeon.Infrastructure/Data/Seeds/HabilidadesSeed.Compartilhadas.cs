using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

public static partial class HabilidadesSeed
{
    private static IEnumerable<HabilidadeDeAcampamento> AcampamentoCompartilhadas()
    {
        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Encorajar",
            nomeOriginal: "Encourage",
            descricao: "Reduz 15 pontos de estresse de um aliado.",
            custoDeDescanso: 2,
            alvo: AlvoDeAcampamento.UmAliado,
            efeitos: new[]
            {
                Efeito("Redução de Estresse", AlvoDeEfeito.Aliado, 15m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Encourage"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Cuidados com Ferimentos",
            nomeOriginal: "Wound Care",
            descricao: "Cura 15% do HP do aliado e remove sangramento e envenenamento.",
            custoDeDescanso: 2,
            alvo: AlvoDeAcampamento.UmAliado,
            efeitos: new[]
            {
                Efeito("Cura", AlvoDeEfeito.Aliado, 15m, UnidadeDeEfeito.Percentual),
                Efeito("Remove Sangramento", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Remove Envenenamento", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Wound Care"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Conversa Motivadora",
            nomeOriginal: "Pep Talk",
            descricao: "Reduz o estresse recebido pelo aliado em 15% por 4 batalhas.",
            custoDeDescanso: 2,
            alvo: AlvoDeAcampamento.UmAliado,
            efeitos: new[]
            {
                Efeito("Redução de Estresse Recebido", AlvoDeEfeito.Aliado, 15m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Pep Talk"));
    }
}
