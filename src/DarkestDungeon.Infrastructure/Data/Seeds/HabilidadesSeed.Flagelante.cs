using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

// O Flagelante é a única classe sem as 3 skills de acampamento compartilhadas (só usa as 4 únicas).
// Também é a única com Deathblow Resist 73% (contra 67% padrão).
public static partial class HabilidadesSeed
{
    private static IEnumerable<HabilidadeDeCombate> FlagelanteCombate()
    {
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Punir",
            nomeOriginal: "Punish",
            descricao: "Golpe brutal com +100% de dano; aplica sangramento (4 pts/rd por 3 rodadas) e reduz a resistência a sangramento em 20%.",
            posicoesValidas: new[] { 1, 2 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: 100m,
            modificadorAcerto: 95m,
            modificadorCritico: 5m,
            efeitos: new[]
            {
                Efeito("Sangramento", AlvoDeEfeito.Inimigo, 4m, UnidadeDeEfeito.Pontos, duracao: 3),
                Efeito("Redução de Resistência a Sangramento", AlvoDeEfeito.Inimigo, 20m, UnidadeDeEfeito.Percentual, duracao: 3),
            },
            id: IdDeterministico("Punish"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Chuva de Sofrimento",
            nomeOriginal: "Rain of Sorrows",
            descricao: "AOE nas duas últimas posições; sangramento 3 pts/rd por 3 rodadas e -20% resistência a sangramento.",
            posicoesValidas: new[] { 1, 2 },
            posicoesQueAtinge: new[] { 3, 4 },
            alvoEmArea: true,
            modificadorDano: -67m,
            modificadorAcerto: 95m,
            modificadorCritico: 2m,
            efeitos: new[]
            {
                Efeito("Sangramento", AlvoDeEfeito.Inimigo, 3m, UnidadeDeEfeito.Pontos, duracao: 3),
                Efeito("Redução de Resistência a Sangramento", AlvoDeEfeito.Inimigo, 20m, UnidadeDeEfeito.Percentual, duracao: 3),
            },
            id: IdDeterministico("Rain of Sorrows"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Exsanguinação",
            nomeOriginal: "Exsanguinate",
            descricao: "+100% dano e sangramento severo (5 pts/rd por 3 rodadas); cura 35% MAX HP mas -25% cura, -3 velocidade por 3 rodadas. Utilizável abaixo de 40% HP. Limite 3 usos por batalha.",
            posicoesValidas: new[] { 1, 2 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: 100m,
            modificadorAcerto: 90m,
            modificadorCritico: 3m,
            efeitos: new[]
            {
                Efeito("Sangramento", AlvoDeEfeito.Inimigo, 5m, UnidadeDeEfeito.Pontos, duracao: 3),
                Efeito("Cura Percentual", AlvoDeEfeito.Self, 35m, UnidadeDeEfeito.Percentual),
                Efeito("Redução de Cura Aplicada", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual, duracao: 3),
                Efeito("Redução de Cura Recebida", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual, duracao: 3),
                Efeito("Redução de Velocidade", AlvoDeEfeito.Self, 3m, UnidadeDeEfeito.Pontos, duracao: 3),
            },
            limitePorUso: new LimitePorUso(EscopoDeLimite.Batalha, 3),
            id: IdDeterministico("Exsanguinate"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Reclamar",
            nomeOriginal: "Reclaim",
            descricao: "Cura de aliado 2 pts/rd por 2 rodadas; auto-aplica sangramento (chance 100% base, 3 pts/rd por 3 rodadas).",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Cura ao Longo do Tempo", AlvoDeEfeito.Aliado, 2m, UnidadeDeEfeito.Pontos, duracao: 2),
                Efeito("Auto-Sangramento", AlvoDeEfeito.Self, 3m, UnidadeDeEfeito.Pontos, duracao: 3),
            },
            id: IdDeterministico("Reclaim"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Redimir",
            nomeOriginal: "Redeem",
            descricao: "Cura 33% MAX HP do aliado; auto-cura 35% mas -25% cura, -3 velocidade por 3 rodadas. Utilizável abaixo de 40% HP. Limite 2 usos por batalha.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Cura Percentual", AlvoDeEfeito.Aliado, 33m, UnidadeDeEfeito.Percentual),
                Efeito("Cura Percentual", AlvoDeEfeito.Self, 35m, UnidadeDeEfeito.Percentual),
                Efeito("Redução de Cura Aplicada", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual, duracao: 3),
                Efeito("Redução de Cura Recebida", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual, duracao: 3),
                Efeito("Redução de Velocidade", AlvoDeEfeito.Self, 3m, UnidadeDeEfeito.Pontos, duracao: 3),
            },
            limitePorUso: new LimitePorUso(EscopoDeLimite.Batalha, 2),
            id: IdDeterministico("Redeem"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Suportar",
            nomeOriginal: "Endure",
            descricao: "-10 estresse em aliado, +10 estresse próprio e +1 velocidade por 4 rodadas.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Redução de Estresse", AlvoDeEfeito.Aliado, 10m, UnidadeDeEfeito.Pontos),
                Efeito("Aumento de Estresse", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Velocidade", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos, duracao: 4),
            },
            id: IdDeterministico("Endure"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Sofrer",
            nomeOriginal: "Suffer",
            descricao: "Remove marcação e transfere sangramento/envenenamento do aliado para si; +6% resistência a golpe mortal e -20% estresse recebido por 4 rodadas.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Remove Marcação", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Transfere Sangramento e Envenenamento", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Marcação", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Rodadas, duracao: 2),
                Efeito("Recebe Sangramento e Envenenamento", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Estresse Recebido", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de Resistência a Golpe Mortal", AlvoDeEfeito.Self, 6m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Suffer"));
    }

    private static IEnumerable<HabilidadeDeAcampamento> FlagelanteAcampamento()
    {
        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Raiva do Açoite",
            nomeOriginal: "Lash's Anger",
            descricao: "Auto-aumenta o estresse em 40 pontos (para acelerar Rapturous).",
            custoDeDescanso: 1,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Aumento de Estresse", AlvoDeEfeito.Self, 40m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Lash's Anger"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Consolo do Açoite",
            nomeOriginal: "Lash's Solace",
            descricao: "Reduz 50 pontos de estresse do Flagelante.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Redução de Estresse", AlvoDeEfeito.Self, 50m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Lash's Solace"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Beijo do Açoite",
            nomeOriginal: "Lash's Kiss",
            descricao: "Auto-cura 33% do HP, remove sangramento e envenenamento; +3 velocidade por 4 batalhas.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Cura", AlvoDeEfeito.Self, 33m, UnidadeDeEfeito.Percentual),
                Efeito("Remove Sangramento", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Remove Envenenamento", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Velocidade", AlvoDeEfeito.Self, 3m, UnidadeDeEfeito.Pontos, duracao: 4),
            },
            id: IdDeterministico("Lash's Kiss"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Cura do Açoite",
            nomeOriginal: "Lash's Cure",
            descricao: "Remove doença do próprio Flagelante.",
            custoDeDescanso: 2,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Remove Doença", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Lash's Cure"));
    }
}
