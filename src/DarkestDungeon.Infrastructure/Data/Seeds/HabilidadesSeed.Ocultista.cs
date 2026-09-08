using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

public static partial class HabilidadesSeed
{
    private static IEnumerable<HabilidadeDeCombate> OcultistaCombate()
    {
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Punhalada Sacrificial",
            nomeOriginal: "Sacrificial Stab",
            descricao: "Ataque corpo-a-corpo com bônus de dano contra criaturas Anciãs (Eldritch).",
            posicoesValidas: new[] { 1, 2, 3 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 80m,
            modificadorCritico: 9m,
            efeitos: new[]
            {
                Efeito("Bônus de Dano vs Anciãos", AlvoDeEfeito.Self, 15m, UnidadeDeEfeito.Percentual),
            },
            id: IdDeterministico("Sacrificial Stab"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Artilharia do Abismo",
            nomeOriginal: "Abyssal Artillery",
            descricao: "Barragem à distância que atinge as duas últimas posições inimigas em área.",
            posicoesValidas: new[] { 3, 4 },
            posicoesQueAtinge: new[] { 3, 4 },
            alvoEmArea: true,
            modificadorDano: -33m,
            modificadorAcerto: 85m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Bônus de Dano vs Anciãos", AlvoDeEfeito.Self, 15m, UnidadeDeEfeito.Percentual),
            },
            id: IdDeterministico("Abyssal Artillery"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Maldição Enfraquecedora",
            nomeOriginal: "Weakening Curse",
            descricao: "Debuff duradouro que reduz o dano e a proteção do alvo em 10% por 3 rodadas.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: -75m,
            modificadorAcerto: 95m,
            modificadorCritico: 5m,
            efeitos: new[]
            {
                Efeito("Redução de Dano", AlvoDeEfeito.Inimigo, 10m, UnidadeDeEfeito.Percentual, duracao: 3),
                Efeito("Redução de Proteção", AlvoDeEfeito.Inimigo, 10m, UnidadeDeEfeito.Percentual, duracao: 3),
            },
            id: IdDeterministico("Weakening Curse"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Reconstrução Estranha",
            nomeOriginal: "Wyrd Reconstruction",
            descricao: "Cura errática (0-13 HP) que pode aplicar sangramento com 60% de chance.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Cura", AlvoDeEfeito.Aliado, 7m, UnidadeDeEfeito.Pontos),
                Efeito("Sangramento", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Pontos, chance: 60m, duracao: 3),
            },
            id: IdDeterministico("Wyrd Reconstruction"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Maldição de Vulnerabilidade",
            nomeOriginal: "Vulnerability Hex",
            descricao: "Marca o alvo por 3 rodadas e reduz sua esquiva em 15 pontos.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: -90m,
            modificadorAcerto: 95m,
            modificadorCritico: 5m,
            efeitos: new[]
            {
                Efeito("Marcação", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Rodadas, duracao: 3),
                Efeito("Redução de Esquiva", AlvoDeEfeito.Inimigo, 15m, UnidadeDeEfeito.Pontos, duracao: 3),
            },
            id: IdDeterministico("Vulnerability Hex"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Mãos do Abismo",
            nomeOriginal: "Hands from the Abyss",
            descricao: "Ataque à distância que atordoa o alvo com 110% de chance base (capado em 100 pelo modelo) e reduz a tocha em 5 pontos.",
            posicoesValidas: new[] { 1, 2 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: false,
            modificadorDano: -50m,
            modificadorAcerto: 90m,
            modificadorCritico: 9m,
            efeitos: new[]
            {
                Efeito("Redução de Tocha", AlvoDeEfeito.Self, 5m, UnidadeDeEfeito.Pontos),
                Efeito("Atordoamento", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Rodadas, chance: 100m, duracao: 1),
            },
            id: IdDeterministico("Hands from the Abyss"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Chamado do Daemon",
            nomeOriginal: "Daemon's Pull",
            descricao: "Puxa o alvo 2 posições para frente e limpa todos os cadáveres.",
            posicoesValidas: new[] { 2, 3, 4 },
            posicoesQueAtinge: new[] { 3, 4 },
            alvoEmArea: false,
            modificadorDano: -50m,
            modificadorAcerto: 90m,
            modificadorCritico: 5m,
            efeitos: new[]
            {
                Efeito("Puxar", AlvoDeEfeito.Inimigo, 2m, UnidadeDeEfeito.Pontos),
                Efeito("Limpar Cadáveres", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Daemon's Pull"));
    }

    private static IEnumerable<HabilidadeDeAcampamento> OcultistaAcampamento()
    {
        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Abandonai a Esperança",
            nomeOriginal: "Abandon Hope",
            descricao: "Reduz 25 pontos do próprio estresse mas aumenta o estresse dos aliados.",
            custoDeDescanso: 1,
            alvo: AlvoDeAcampamento.PartyInteira,
            efeitos: new[]
            {
                Efeito("Redução de Estresse", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Pontos),
                Efeito("Aumento de Estresse", AlvoDeEfeito.Aliado, 10m, UnidadeDeEfeito.Pontos, chance: 50m),
                Efeito("Aumento de Estresse Alternativo", AlvoDeEfeito.Aliado, 5m, UnidadeDeEfeito.Pontos, chance: 50m),
            },
            id: IdDeterministico("Abandon Hope"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Ritual Sombrio",
            nomeOriginal: "Dark Ritual",
            descricao: "Cura 50% do HP e remove debuffs de mortalidade de um aliado ao custo de 100 tocha e +15 estresse próprio.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.UmAliado,
            efeitos: new[]
            {
                Efeito("Redução de Tocha", AlvoDeEfeito.Self, 100m, UnidadeDeEfeito.Pontos),
                Efeito("Aumento de Estresse", AlvoDeEfeito.Self, 15m, UnidadeDeEfeito.Pontos),
                Efeito("Cura", AlvoDeEfeito.Aliado, 50m, UnidadeDeEfeito.Percentual),
                Efeito("Remove Mortalidade", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Dark Ritual"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Força Sombria",
            nomeOriginal: "Dark Strength",
            descricao: "Concede +20% de dano ao aliado por 4 batalhas ao custo de +15 estresse próprio.",
            custoDeDescanso: 2,
            alvo: AlvoDeAcampamento.UmAliado,
            efeitos: new[]
            {
                Efeito("Aumento de Estresse", AlvoDeEfeito.Self, 15m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Dano", AlvoDeEfeito.Aliado, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Dark Strength"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Comunhão Indizível",
            nomeOriginal: "Unspeakable Commune",
            descricao: "Impede emboscadas noturnas ao custo de +7 estresse em cada aliado.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.PartyInteira,
            efeitos: new[]
            {
                Efeito("Impede Emboscada Noturna", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Aumento de Estresse", AlvoDeEfeito.Aliado, 7m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Unspeakable Commune"));
    }
}
