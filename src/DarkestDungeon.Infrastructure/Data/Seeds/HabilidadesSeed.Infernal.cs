using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

public static partial class HabilidadesSeed
{
    private static IEnumerable<HabilidadeDeCombate> InfernalCombate()
    {
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Golpe Perverso",
            nomeOriginal: "Wicked Hack",
            descricao: "Golpe simples da glaive contra as duas primeiras posições inimigas.",
            posicoesValidas: new[] { 1, 2 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 85m,
            modificadorCritico: 4m,
            efeitos: Array.Empty<EfeitoDeHabilidade>(),
            id: IdDeterministico("Wicked Hack"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Cisne de Ferro",
            nomeOriginal: "Iron Swan",
            descricao: "Ataque de alcance longo que atinge a última posição inimiga (backline).",
            posicoesValidas: new[] { 1 },
            posicoesQueAtinge: new[] { 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 85m,
            modificadorCritico: 5m,
            efeitos: Array.Empty<EfeitoDeHabilidade>(),
            id: IdDeterministico("Iron Swan"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Grito Bárbaro",
            nomeOriginal: "Barbaric Yawp",
            descricao: "Grito AOE que atordoa as duas primeiras posições inimigas; -20% dano e -3 velocidade próprios por 3 rodadas. Limite 3 usos por batalha.",
            posicoesValidas: new[] { 1, 2 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: true,
            modificadorDano: -100m,
            modificadorAcerto: 95m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Atordoamento", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Rodadas, duracao: 1),
                Efeito("Redução de Dano", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual, duracao: 3),
                Efeito("Redução de Velocidade", AlvoDeEfeito.Self, 3m, UnidadeDeEfeito.Pontos, duracao: 3),
            },
            limitePorUso: new LimitePorUso(EscopoDeLimite.Batalha, 3),
            id: IdDeterministico("Barbaric Yawp"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Se Sangra",
            nomeOriginal: "If It Bleeds",
            descricao: "AOE nas posições centrais que aplica sangramento (2 pts/rd por 3 rodadas).",
            posicoesValidas: new[] { 1, 2, 3 },
            posicoesQueAtinge: new[] { 2, 3 },
            alvoEmArea: true,
            modificadorDano: -35m,
            modificadorAcerto: 85m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Sangramento", AlvoDeEfeito.Inimigo, 2m, UnidadeDeEfeito.Pontos, duracao: 3),
            },
            id: IdDeterministico("If It Bleeds"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Investida",
            nomeOriginal: "Breakthrough",
            descricao: "Investida AOE que avança 1 posição e aplica -10% dano e -1 velocidade próprios por 3 rodadas.",
            posicoesValidas: new[] { 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: true,
            modificadorDano: -50m,
            modificadorAcerto: 85m,
            modificadorCritico: -1m,
            efeitos: new[]
            {
                Efeito("Avanço", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Dano", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Percentual, duracao: 3),
                Efeito("Redução de Velocidade", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos, duracao: 3),
            },
            id: IdDeterministico("Breakthrough"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Descarga de Adrenalina",
            nomeOriginal: "Adrenaline Rush",
            descricao: "Cura 1 HP, remove sangramento e envenenamento, +5 precisão e +20% dano por 4 rodadas.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Cura", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Remove Sangramento", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Remove Envenenamento", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Precisão", AlvoDeEfeito.Self, 5m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Bônus de Dano", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Adrenaline Rush"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Sangrar Até Morrer",
            nomeOriginal: "Bleed Out",
            descricao: "Golpe de alcance longo com +20% dano contra a última posição; aplica sangramento (3 pts/rd por 3 rodadas) mas -20% dano e -3 velocidade próprios por 3 rodadas.",
            posicoesValidas: new[] { 1 },
            posicoesQueAtinge: new[] { 1 },
            alvoEmArea: false,
            modificadorDano: 20m,
            modificadorAcerto: 85m,
            modificadorCritico: 6m,
            efeitos: new[]
            {
                Efeito("Sangramento", AlvoDeEfeito.Inimigo, 3m, UnidadeDeEfeito.Pontos, duracao: 3),
                Efeito("Redução de Dano", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual, duracao: 3),
                Efeito("Redução de Velocidade", AlvoDeEfeito.Self, 3m, UnidadeDeEfeito.Pontos, duracao: 3),
            },
            id: IdDeterministico("Bleed Out"));
    }

    private static IEnumerable<HabilidadeDeAcampamento> InfernalAcampamento()
    {
        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Transe de Batalha",
            nomeOriginal: "Battle Trance",
            descricao: "+25% dano se estiver na posição 1 (frontline) e -25% dano se não estiver, por 4 batalhas.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Bônus de Dano se em Frontline", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Redução de Dano se Fora do Frontline", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Battle Trance"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Farra",
            nomeOriginal: "Revel",
            descricao: "Farra em grupo: -5 precisão, -2 velocidade, -20 estresse e -10% estresse recebido em toda a party por 4 batalhas.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.Party,
            efeitos: new[]
            {
                Efeito("Redução de Precisão", AlvoDeEfeito.Aliado, 5m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Redução de Velocidade", AlvoDeEfeito.Aliado, 2m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Redução de Estresse", AlvoDeEfeito.Aliado, 20m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Estresse Recebido", AlvoDeEfeito.Aliado, 10m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Revel"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Rejeitar os Deuses",
            nomeOriginal: "Reject the Gods",
            descricao: "-30 estresse próprio, mas +7 estresse em aliados não-religiosos ou +15 estresse em religiosos.",
            custoDeDescanso: 2,
            alvo: AlvoDeAcampamento.PartyInteira,
            efeitos: new[]
            {
                Efeito("Redução de Estresse", AlvoDeEfeito.Self, 30m, UnidadeDeEfeito.Pontos),
                Efeito("Aumento de Estresse Padrão", AlvoDeEfeito.Aliado, 7m, UnidadeDeEfeito.Pontos),
                Efeito("Aumento de Estresse em Religiosos", AlvoDeEfeito.Aliado, 15m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Reject the Gods"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Afiar Lança",
            nomeOriginal: "Sharpen Spear",
            descricao: "+10% crítico por 4 batalhas.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Bônus de Crítico", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Sharpen Spear"));
    }
}
