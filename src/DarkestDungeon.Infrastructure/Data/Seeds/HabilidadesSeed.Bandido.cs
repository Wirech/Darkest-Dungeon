using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

public static partial class HabilidadesSeed
{
    private static IEnumerable<HabilidadeDeCombate> BandidoCombate()
    {
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Corte Perverso",
            nomeOriginal: "Wicked Slice",
            descricao: "Golpe corpo-a-corpo consistente com +15% de dano contra as duas primeiras posições inimigas.",
            posicoesValidas: new[] { 1, 2, 3 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: 15m,
            modificadorAcerto: 85m,
            modificadorCritico: 5m,
            efeitos: Array.Empty<EfeitoDeHabilidade>(),
            id: IdDeterministico("Wicked Slice"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Tiro de Pistola",
            nomeOriginal: "Pistol Shot",
            descricao: "Tiro à distância com +25% de dano contra alvos marcados.",
            posicoesValidas: new[] { 2, 3, 4 },
            posicoesQueAtinge: new[] { 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: -15m,
            modificadorAcerto: 85m,
            modificadorCritico: 7.5m,
            efeitos: new[]
            {
                Efeito("Bônus de Dano vs Marcado", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual),
            },
            id: IdDeterministico("Pistol Shot"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Tiro à Queima-Roupa",
            nomeOriginal: "Point Blank Shot",
            descricao: "Tiro devastador da linha de frente que empurra o alvo e recua o Bandido em 1 posição.",
            posicoesValidas: new[] { 1 },
            posicoesQueAtinge: new[] { 1 },
            alvoEmArea: false,
            modificadorDano: 50m,
            modificadorAcerto: 95m,
            modificadorCritico: 5m,
            efeitos: new[]
            {
                Efeito("Empurrão", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Recuo", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Point Blank Shot"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Disparo de Chumbo",
            nomeOriginal: "Grapeshot Blast",
            descricao: "Rajada AOE nas três primeiras posições inimigas; aplica +4% crítico recebido por 3 rodadas.",
            posicoesValidas: new[] { 2, 3 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: true,
            modificadorDano: -50m,
            modificadorAcerto: 75m,
            modificadorCritico: -9m,
            efeitos: new[]
            {
                Efeito("Aumento de Crítico Recebido", AlvoDeEfeito.Inimigo, 4m, UnidadeDeEfeito.Percentual, duracao: 3),
            },
            id: IdDeterministico("Grapeshot Blast"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Tiro Rastreador",
            nomeOriginal: "Tracking Shot",
            descricao: "Tiro que remove furtividade e concede ao Bandido +6 precisão, +4% crítico e +12% dano por 1 batalha. Limite 1 uso por batalha.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: -80m,
            modificadorAcerto: 95m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Remove Furtividade", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Precisão", AlvoDeEfeito.Self, 6m, UnidadeDeEfeito.Pontos, duracao: 1),
                Efeito("Bônus de Crítico", AlvoDeEfeito.Self, 4m, UnidadeDeEfeito.Percentual, duracao: 1),
                Efeito("Bônus de Dano", AlvoDeEfeito.Self, 12m, UnidadeDeEfeito.Percentual, duracao: 1),
            },
            limitePorUso: new LimitePorUso(EscopoDeLimite.Batalha, 1),
            id: IdDeterministico("Tracking Shot"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Avanço do Duelista",
            nomeOriginal: "Duelist's Advance",
            descricao: "Avanço melee que ativa o contra-ataque (Riposte) por 3 rodadas com -40% de dano.",
            posicoesValidas: new[] { 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: false,
            modificadorDano: -20m,
            modificadorAcerto: 90m,
            modificadorCritico: 5m,
            efeitos: new[]
            {
                Efeito("Avanço", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Ativa Contra-Ataque", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Rodadas, duracao: 3),
                Efeito("Contra-Ataque com Redução de Dano", AlvoDeEfeito.Self, 40m, UnidadeDeEfeito.Percentual, duracao: 3),
            },
            id: IdDeterministico("Duelist's Advance"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Cortar Veia",
            nomeOriginal: "Open Vein",
            descricao: "Golpe que sangra o alvo (2 pts/rd por 3 rodadas) e reduz sua resistência a sangramento e velocidade.",
            posicoesValidas: new[] { 1, 2, 3 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: -15m,
            modificadorAcerto: 95m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Sangramento", AlvoDeEfeito.Inimigo, 2m, UnidadeDeEfeito.Pontos, duracao: 3),
                Efeito("Redução de Resistência a Sangramento", AlvoDeEfeito.Inimigo, 20m, UnidadeDeEfeito.Percentual, duracao: 3),
                Efeito("Redução de Velocidade", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos, duracao: 3),
            },
            id: IdDeterministico("Open Vein"));
    }

    /// Apenas 3 skills únicas — `Gallows Humor` é criada em LadraoDeCovaAcampamento e compartilhada com o Bandido.
    private static IEnumerable<HabilidadeDeAcampamento> BandidoAcampamento()
    {
        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Destreza Sem Igual",
            nomeOriginal: "Unparalleled Finesse",
            descricao: "Auto-buff completo: +10 esquiva, +2 velocidade, +20% dano melee e +10 precisão melee por 4 batalhas.",
            custoDeDescanso: 4,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Bônus de Esquiva", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Bônus de Velocidade", AlvoDeEfeito.Self, 2m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Bônus de Dano Melee", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de Precisão Melee", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Pontos, duracao: 4),
            },
            id: IdDeterministico("Unparalleled Finesse"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Limpar Armas",
            nomeOriginal: "Clean Guns",
            descricao: "Auto-buff ranged: +10 precisão, +20% dano e +6% crítico em habilidades ranged por 4 batalhas.",
            custoDeDescanso: 4,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Bônus de Precisão Ranged", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Bônus de Dano Ranged", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de Crítico Ranged", AlvoDeEfeito.Self, 6m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Clean Guns"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Instinto de Bandido",
            nomeOriginal: "Bandit's Sense",
            descricao: "Impede emboscadas noturnas, -20% chance da party ser surpreendida e +20% chance de surpreender monstros por 4 batalhas.",
            custoDeDescanso: 4,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Impede Emboscada Noturna", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Chance de Ser Surpreendido", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Aumento de Chance de Surpreender", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Bandit's Sense"));
    }
}
