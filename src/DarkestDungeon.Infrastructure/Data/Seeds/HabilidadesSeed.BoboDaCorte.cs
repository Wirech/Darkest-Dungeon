using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

public static partial class HabilidadesSeed
{
    private static IEnumerable<HabilidadeDeCombate> BoboDaCorteCombate()
    {
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Adaga de Punho",
            nomeOriginal: "Dirk Stab",
            descricao: "Ataque melee versátil que avança 1 posição, ignora guarda e ativa Finale por 8 rodadas (+30% dano).",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 85m,
            modificadorCritico: 5m,
            efeitos: new[]
            {
                Efeito("Avanço", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Ignora Guarda", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Finale — Bônus de Dano", AlvoDeEfeito.Self, 30m, UnidadeDeEfeito.Percentual, duracao: 8),
            },
            id: IdDeterministico("Dirk Stab"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Colheita",
            nomeOriginal: "Harvest",
            descricao: "AOE em duas posições que aplica sangramento por 3 rodadas e prepara o Finale.",
            posicoesValidas: new[] { 2, 3 },
            posicoesQueAtinge: new[] { 2, 3 },
            alvoEmArea: true,
            modificadorDano: -50m,
            modificadorAcerto: 90m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Sangramento", AlvoDeEfeito.Inimigo, 2m, UnidadeDeEfeito.Pontos, duracao: 3),
                Efeito("Finale — Bônus de Dano", AlvoDeEfeito.Self, 30m, UnidadeDeEfeito.Percentual, duracao: 8),
            },
            id: IdDeterministico("Harvest"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Grande Final",
            nomeOriginal: "Finale",
            descricao: "Golpe final devastador: +50% dano, 140% precisão, mas recua 3 posições e sofre penalidades. Limite 1 uso por batalha.",
            posicoesValidas: new[] { 1, 2 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 50m,
            modificadorAcerto: 100m,
            modificadorCritico: 5m,
            efeitos: new[]
            {
                Efeito("Recuo", AlvoDeEfeito.Self, 3m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Esquiva", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Pontos, duracao: 1),
                Efeito("Redução de Velocidade", AlvoDeEfeito.Self, 3m, UnidadeDeEfeito.Pontos, duracao: 1),
                Efeito("Aumento de Estresse Recebido", AlvoDeEfeito.Self, 100m, UnidadeDeEfeito.Percentual, duracao: 1),
            },
            limitePorUso: new LimitePorUso(EscopoDeLimite.Batalha, 1),
            id: IdDeterministico("Finale"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Solo",
            nomeOriginal: "Solo",
            descricao: "Salta para a linha de frente e se marca por 3 rodadas com +20 esquiva; ativa Finale (+75% dano, +8% crítico). Limite 2 usos por batalha.",
            posicoesValidas: new[] { 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: true,
            modificadorDano: -100m,
            modificadorAcerto: 100m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Avanço", AlvoDeEfeito.Self, 3m, UnidadeDeEfeito.Pontos),
                Efeito("Marcação", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Rodadas, duracao: 3),
                Efeito("Bônus de Esquiva", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Finale — Bônus de Dano", AlvoDeEfeito.Self, 75m, UnidadeDeEfeito.Percentual, duracao: 8),
                Efeito("Finale — Bônus de Crítico", AlvoDeEfeito.Self, 8m, UnidadeDeEfeito.Percentual, duracao: 8),
            },
            limitePorUso: new LimitePorUso(EscopoDeLimite.Batalha, 2),
            id: IdDeterministico("Solo"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Corte Fora",
            nomeOriginal: "Slice Off",
            descricao: "Golpe corpo-a-corpo que aplica sangramento (3 pts/rd por 3 rodadas) e prepara o Finale.",
            posicoesValidas: new[] { 2, 3 },
            posicoesQueAtinge: new[] { 2, 3 },
            alvoEmArea: false,
            modificadorDano: -33m,
            modificadorAcerto: 95m,
            modificadorCritico: 8m,
            efeitos: new[]
            {
                Efeito("Sangramento", AlvoDeEfeito.Inimigo, 3m, UnidadeDeEfeito.Pontos, duracao: 3),
                Efeito("Finale — Bônus de Dano", AlvoDeEfeito.Self, 30m, UnidadeDeEfeito.Percentual, duracao: 8),
            },
            id: IdDeterministico("Slice Off"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Balada de Guerra",
            nomeOriginal: "Battle Ballad",
            descricao: "Buff em área para três aliados: +5 precisão, +2% crítico e +2 velocidade por 4 rodadas; prepara o Finale.",
            posicoesValidas: new[] { 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: true,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Bônus de Precisão", AlvoDeEfeito.Aliado, 5m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Bônus de Crítico", AlvoDeEfeito.Aliado, 2m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de Velocidade", AlvoDeEfeito.Aliado, 2m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Finale — Bônus de Dano", AlvoDeEfeito.Self, 30m, UnidadeDeEfeito.Percentual, duracao: 8),
                Efeito("Finale — Bônus de Crítico", AlvoDeEfeito.Self, 8m, UnidadeDeEfeito.Percentual, duracao: 8),
            },
            id: IdDeterministico("Battle Ballad"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Melodia Inspiradora",
            nomeOriginal: "Inspiring Tune",
            descricao: "Toca uma melodia que reduz o estresse dos aliados em 8 pontos e o estresse recebido em 10%.",
            posicoesValidas: new[] { 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Redução de Estresse", AlvoDeEfeito.Aliado, 8m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Estresse Recebido", AlvoDeEfeito.Aliado, 10m, UnidadeDeEfeito.Percentual),
                Efeito("Finale — Bônus de Dano", AlvoDeEfeito.Self, 30m, UnidadeDeEfeito.Percentual, duracao: 8),
                Efeito("Finale — Bônus de Crítico", AlvoDeEfeito.Self, 8m, UnidadeDeEfeito.Percentual, duracao: 8),
            },
            id: IdDeterministico("Inspiring Tune"));
    }

    private static IEnumerable<HabilidadeDeAcampamento> BoboDaCorteAcampamento()
    {
        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Voltar no Tempo",
            nomeOriginal: "Turn Back Time",
            descricao: "Cura profundamente o estresse do aliado: -30 estresse e mais -15 se afligido.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.UmAliado,
            efeitos: new[]
            {
                Efeito("Redução de Estresse", AlvoDeEfeito.Aliado, 30m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Estresse Extra se Afligido", AlvoDeEfeito.Aliado, 15m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Turn Back Time"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Toda Rosa Tem Seu Espinho",
            nomeOriginal: "Every Rose Has Its Thorn",
            descricao: "-15 estresse em todos os aliados e -15% estresse recebido por 4 batalhas.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.TodosOsAliados,
            efeitos: new[]
            {
                Efeito("Redução de Estresse", AlvoDeEfeito.Aliado, 15m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Estresse Recebido", AlvoDeEfeito.Aliado, 15m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Every Rose Has Its Thorn"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Olho do Tigre",
            nomeOriginal: "Tiger's Eye",
            descricao: "Concede +10 precisão e +8% crítico ao aliado por 4 batalhas.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.UmAliado,
            efeitos: new[]
            {
                Efeito("Bônus de Precisão", AlvoDeEfeito.Aliado, 10m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Bônus de Crítico", AlvoDeEfeito.Aliado, 8m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Tiger's Eye"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Zombaria",
            nomeOriginal: "Mockery",
            descricao: "Aumenta o estresse do alvo em 20 pontos e reduz o estresse dos outros aliados em 20 pontos.",
            custoDeDescanso: 2,
            alvo: AlvoDeAcampamento.PartyInteira,
            efeitos: new[]
            {
                Efeito("Aumento de Estresse", AlvoDeEfeito.Aliado, 20m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Estresse", AlvoDeEfeito.Aliado, 20m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Mockery"));
    }
}
