using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

public static partial class HabilidadesSeed
{
    private static IEnumerable<HabilidadeDeCombate> LeprosoCombate()
    {
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Talhar",
            nomeOriginal: "Chop",
            descricao: "Ataque melee simples do Leproso com sua espada gigante.",
            posicoesValidas: new[] { 1, 2 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 75m,
            modificadorCritico: 3m,
            efeitos: Array.Empty<EfeitoDeHabilidade>(),
            id: IdDeterministico("Chop"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Cortar",
            nomeOriginal: "Hew",
            descricao: "Golpe em área que atinge as duas primeiras posições inimigas simultaneamente.",
            posicoesValidas: new[] { 1, 2 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: true,
            modificadorDano: -50m,
            modificadorAcerto: 75m,
            modificadorCritico: -4m,
            efeitos: Array.Empty<EfeitoDeHabilidade>(),
            id: IdDeterministico("Hew"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Expurgar",
            nomeOriginal: "Purge",
            descricao: "Empurrão brutal que joga o alvo 3 posições para trás, limpa cadáveres e concede +5 precisão por 4 rodadas.",
            posicoesValidas: new[] { 1 },
            posicoesQueAtinge: new[] { 1 },
            alvoEmArea: false,
            modificadorDano: -40m,
            modificadorAcerto: 85m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Empurrão", AlvoDeEfeito.Inimigo, 3m, UnidadeDeEfeito.Pontos),
                Efeito("Limpar Cadáveres", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Precisão", AlvoDeEfeito.Self, 5m, UnidadeDeEfeito.Pontos, duracao: 4),
            },
            id: IdDeterministico("Purge"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Vingança",
            nomeOriginal: "Revenge",
            descricao: "Buff berserker: +10 precisão, +25% dano e +7% crítico mas -10 esquiva e +25% dano recebido por 1 batalha. Limite 1 uso por batalha.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Bônus de Precisão", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Pontos, duracao: 1),
                Efeito("Bônus de Dano", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual, duracao: 1),
                Efeito("Bônus de Crítico", AlvoDeEfeito.Self, 7m, UnidadeDeEfeito.Percentual, duracao: 1),
                Efeito("Redução de Esquiva", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Pontos, duracao: 1),
                Efeito("Aumento de Dano Recebido", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual, duracao: 1),
            },
            limitePorUso: new LimitePorUso(EscopoDeLimite.Batalha, 1),
            id: IdDeterministico("Revenge"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Resistir",
            nomeOriginal: "Withstand",
            descricao: "Marca a si mesmo, concede +20% proteção e +30% resistência a envenenamento/sangramento/debuff/movimento por 1 batalha. Limite 1 uso por batalha.",
            posicoesValidas: new[] { 1, 2, 3 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Marcação", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Rodadas, duracao: 1),
                Efeito("Bônus de Proteção", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual, duracao: 1),
                Efeito("Bônus de Resistências", AlvoDeEfeito.Self, 30m, UnidadeDeEfeito.Percentual, duracao: 1),
            },
            limitePorUso: new LimitePorUso(EscopoDeLimite.Batalha, 1),
            id: IdDeterministico("Withstand"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Solenidade",
            nomeOriginal: "Solemnity",
            descricao: "Auto-cura de 6 pontos e -5 estresse.",
            posicoesValidas: new[] { 1, 2 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Cura", AlvoDeEfeito.Self, 6m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Estresse", AlvoDeEfeito.Self, 5m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Solemnity"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Intimidar",
            nomeOriginal: "Intimidate",
            descricao: "Remove furtividade, reduz dano do alvo em 20% e velocidade em 3 por 3 rodadas; auto-marca com +2 velocidade por 4 rodadas.",
            posicoesValidas: new[] { 1 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: -85m,
            modificadorAcerto: 95m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Remove Furtividade", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Dano", AlvoDeEfeito.Inimigo, 20m, UnidadeDeEfeito.Percentual, duracao: 3),
                Efeito("Redução de Velocidade", AlvoDeEfeito.Inimigo, 3m, UnidadeDeEfeito.Pontos, duracao: 3),
                Efeito("Marcação", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Rodadas),
                Efeito("Bônus de Velocidade", AlvoDeEfeito.Self, 2m, UnidadeDeEfeito.Pontos, duracao: 4),
            },
            id: IdDeterministico("Intimidate"));
    }

    private static IEnumerable<HabilidadeDeAcampamento> LeprosoAcampamento()
    {
        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Remover a Máscara",
            nomeOriginal: "Let the Mask Down",
            descricao: "-25 estresse do Leproso, mas +5 estresse em cada aliado.",
            custoDeDescanso: 1,
            alvo: AlvoDeAcampamento.PartyInteira,
            efeitos: new[]
            {
                Efeito("Redução de Estresse", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Pontos),
                Efeito("Aumento de Estresse", AlvoDeEfeito.Aliado, 5m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Let the Mask Down"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Mortalha Sangrenta",
            nomeOriginal: "Bloody Shroud",
            descricao: "+25% resistência a sangramento, envenenamento, movimento e debuff por 4 batalhas.",
            custoDeDescanso: 2,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Bônus de Resistência a Sangramento", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de Resistência a Envenenamento", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de Resistência a Movimento", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de Resistência a Debuff", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Bloody Shroud"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Reflexão",
            nomeOriginal: "Reflection",
            descricao: "-20 estresse, +10 precisão e +8% crítico por 4 batalhas.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Redução de Estresse", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Precisão", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Bônus de Crítico", AlvoDeEfeito.Self, 8m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Reflection"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Quarentena",
            nomeOriginal: "Quarantine",
            descricao: "O Leproso sofre 20% de dano em HP para reduzir 15-20 estresse dos aliados.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.PartyInteira,
            efeitos: new[]
            {
                Efeito("Auto-Dano", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual),
                Efeito("Redução de Estresse Alto", AlvoDeEfeito.Aliado, 20m, UnidadeDeEfeito.Pontos, chance: 50m),
                Efeito("Redução de Estresse Baixo", AlvoDeEfeito.Aliado, 15m, UnidadeDeEfeito.Pontos, chance: 50m),
            },
            id: IdDeterministico("Quarantine"));
    }
}
