using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

// A Abominação alterna entre Forma Humana e Forma Besta via `Transform` (ação livre, limite 2 usos por batalha).
public static partial class HabilidadesSeed
{
    private static IEnumerable<HabilidadeDeCombate> AbominacaoCombate()
    {
        // Transform (compartilhada entre as duas formas) — muda a forma da Abominação.
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Transformar",
            nomeOriginal: "Transform",
            descricao: "Alterna entre Forma Humana e Forma Besta como ação livre. Ao virar Besta: +1 velocidade, +20% resistência a envenenamento, +10% dano por 3-4 rodadas, cura 5 HP, mas todos os aliados sofrem +8 estresse. Limite 2 usos por batalha.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: true,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Muda para Forma Besta", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Velocidade", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Bônus de Resistência a Envenenamento", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de Dano", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Percentual, duracao: 3),
                Efeito("Cura", AlvoDeEfeito.Self, 5m, UnidadeDeEfeito.Pontos),
                Efeito("Aumento de Estresse em Aliados", AlvoDeEfeito.Aliado, 8m, UnidadeDeEfeito.Pontos),
            },
            limitePorUso: new LimitePorUso(EscopoDeLimite.Batalha, 2),
            id: IdDeterministico("Transform"));

        // Forma Humana — 3 skills exclusivas.
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Algemas",
            nomeOriginal: "Manacles",
            descricao: "[Forma Humana] Arremesso ranged que atordoa o alvo com 90% de chance base.",
            posicoesValidas: new[] { 2, 3 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: false,
            modificadorDano: -60m,
            modificadorAcerto: 95m,
            modificadorCritico: 1m,
            efeitos: new[]
            {
                Efeito("Atordoamento", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Rodadas, chance: 90m, duracao: 1),
            },
            id: IdDeterministico("Manacles"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Bile da Besta",
            nomeOriginal: "Beast's Bile",
            descricao: "[Forma Humana] Cospe bile venenosa AOE em duas posições; envenenamento 2 pts/rd por 3 rodadas e -20% resistência a envenenamento.",
            posicoesValidas: new[] { 2, 3 },
            posicoesQueAtinge: new[] { 2, 3 },
            alvoEmArea: true,
            modificadorDano: -90m,
            modificadorAcerto: 95m,
            modificadorCritico: 2m,
            efeitos: new[]
            {
                Efeito("Envenenamento", AlvoDeEfeito.Inimigo, 2m, UnidadeDeEfeito.Pontos, duracao: 3),
                Efeito("Redução de Resistência a Envenenamento", AlvoDeEfeito.Inimigo, 20m, UnidadeDeEfeito.Percentual, duracao: 3),
            },
            id: IdDeterministico("Beast's Bile"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Absolvição",
            nomeOriginal: "Absolution",
            descricao: "[Forma Humana] Auto-cura de 3 HP e -7 estresse.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Cura", AlvoDeEfeito.Self, 3m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Estresse", AlvoDeEfeito.Self, 7m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Absolution"));

        // Forma Besta — 3 skills exclusivas.
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Rasgar",
            nomeOriginal: "Rake",
            descricao: "[Forma Besta] Golpe AOE de garras que ativa Rasgar por 4 rodadas (+15% dano).",
            posicoesValidas: new[] { 1, 2 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: true,
            modificadorDano: -50m,
            modificadorAcerto: 90m,
            modificadorCritico: -3m,
            efeitos: new[]
            {
                Efeito("Rasgar — Bônus de Dano", AlvoDeEfeito.Self, 15m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Rake"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Fúria",
            nomeOriginal: "Rage",
            descricao: "[Forma Besta] Ataque brutal contra as três primeiras posições.",
            posicoesValidas: new[] { 1, 2 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 85m,
            modificadorCritico: 7.5m,
            efeitos: Array.Empty<EfeitoDeHabilidade>(),
            id: IdDeterministico("Rage"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Bater",
            nomeOriginal: "Slam",
            descricao: "[Forma Besta] Empurra o alvo 2 posições, aplica -10 esquiva e -2 velocidade por 4 rodadas; Besta avança 1 posição.",
            posicoesValidas: new[] { 1, 2, 3 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: -25m,
            modificadorAcerto: 80m,
            modificadorCritico: 1m,
            efeitos: new[]
            {
                Efeito("Empurrão", AlvoDeEfeito.Inimigo, 2m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Esquiva", AlvoDeEfeito.Inimigo, 10m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Redução de Velocidade", AlvoDeEfeito.Inimigo, 2m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Avanço", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Slam"));
    }

    private static IEnumerable<HabilidadeDeAcampamento> AbominacaoAcampamento()
    {
        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Controle da Raiva",
            nomeOriginal: "Anger Management",
            descricao: "+20 estresse próprio para reduzir 10 estresse de todos os aliados.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.PartyInteira,
            efeitos: new[]
            {
                Efeito("Aumento de Estresse", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Estresse", AlvoDeEfeito.Aliado, 10m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Anger Management"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Preparação Psíquica",
            nomeOriginal: "Psych Up",
            descricao: "+25% dano próprio por 4 batalhas, mas aliados ganham +10 estresse (não-religiosos) ou +20 estresse (religiosos).",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.PartyInteira,
            efeitos: new[]
            {
                Efeito("Bônus de Dano", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Aumento de Estresse Padrão", AlvoDeEfeito.Aliado, 10m, UnidadeDeEfeito.Pontos),
                Efeito("Aumento de Estresse em Religiosos", AlvoDeEfeito.Aliado, 20m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Psych Up"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Aceleração",
            nomeOriginal: "The Quickening",
            descricao: "+4 velocidade por 4 batalhas.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Bônus de Velocidade", AlvoDeEfeito.Self, 4m, UnidadeDeEfeito.Pontos, duracao: 4),
            },
            id: IdDeterministico("The Quickening"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Sangue Ancião",
            nomeOriginal: "Eldritch Blood",
            descricao: "+40% resistência a envenenamento, sangramento e doença; +20% estresse recebido por 4 batalhas.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Bônus de Resistência a Envenenamento", AlvoDeEfeito.Self, 40m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de Resistência a Sangramento", AlvoDeEfeito.Self, 40m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de Resistência a Doença", AlvoDeEfeito.Self, 40m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Aumento de Estresse Recebido", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Eldritch Blood"));
    }
}
