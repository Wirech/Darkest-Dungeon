using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

public static partial class HabilidadesSeed
{
    private static IEnumerable<HabilidadeDeCombate> LadraoDeCovaCombate()
    {
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Picareta na Cara",
            nomeOriginal: "Pick to the Face",
            descricao: "Golpe corpo-a-corpo com penetração de armadura.",
            posicoesValidas: new[] { 1, 2, 3 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: -15m,
            modificadorAcerto: 90m,
            modificadorCritico: 1m,
            efeitos: new[]
            {
                Efeito("Penetração de Armadura", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Pick to the Face"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Estocada",
            nomeOriginal: "Lunge",
            descricao: "Investida da retaguarda que avança 2 posições e concede +20% de dano contra envenenados.",
            posicoesValidas: new[] { 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: false,
            modificadorDano: 40m,
            modificadorAcerto: 95m,
            modificadorCritico: 8m,
            efeitos: new[]
            {
                Efeito("Bônus de Dano vs Envenenado", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual),
                Efeito("Avanço", AlvoDeEfeito.Self, 2m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Lunge"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Adagas Cintilantes",
            nomeOriginal: "Flashing Daggers",
            descricao: "AOE em duas posições que reduz a resistência a sangramento dos alvos em 20% por 3 rodadas.",
            posicoesValidas: new[] { 2, 3, 4 },
            posicoesQueAtinge: new[] { 2, 3 },
            alvoEmArea: true,
            modificadorDano: -33m,
            modificadorAcerto: 90m,
            modificadorCritico: -5m,
            efeitos: new[]
            {
                Efeito("Redução de Resistência a Sangramento", AlvoDeEfeito.Inimigo, 20m, UnidadeDeEfeito.Percentual, duracao: 3),
            },
            id: IdDeterministico("Flashing Daggers"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Fusão nas Sombras",
            nomeOriginal: "Shadow Fade",
            descricao: "Recua 2 posições, entra em furtividade por 2 rodadas com +80% dano, +4% crítico e +10 esquiva por 4 rodadas.",
            posicoesValidas: new[] { 1, 2 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Recuo", AlvoDeEfeito.Self, 2m, UnidadeDeEfeito.Pontos),
                Efeito("Furtividade", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Rodadas, duracao: 2),
                Efeito("Bônus de Dano", AlvoDeEfeito.Self, 80m, UnidadeDeEfeito.Percentual, duracao: 2),
                Efeito("Bônus de Crítico", AlvoDeEfeito.Self, 4m, UnidadeDeEfeito.Percentual, duracao: 2),
                Efeito("Bônus de Esquiva", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Pontos, duracao: 4),
            },
            id: IdDeterministico("Shadow Fade"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Adaga Arremessada",
            nomeOriginal: "Thrown Dagger",
            descricao: "Arremesso à distância com +25% dano contra marcados e +20% dano contra envenenados; +5 precisão à Ladra por 4 rodadas.",
            posicoesValidas: new[] { 2, 3, 4 },
            posicoesQueAtinge: new[] { 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: -10m,
            modificadorAcerto: 90m,
            modificadorCritico: 8m,
            efeitos: new[]
            {
                Efeito("Bônus de Dano vs Marcado", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual),
                Efeito("Bônus de Dano vs Envenenado", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual),
                Efeito("Bônus de Precisão", AlvoDeEfeito.Self, 5m, UnidadeDeEfeito.Pontos, duracao: 4),
            },
            id: IdDeterministico("Thrown Dagger"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Dardo Envenenado",
            nomeOriginal: "Poison Dart",
            descricao: "Dardo à distância que envenena por 4 rodadas (2 pts/rd) e reduz a resistência a envenenamento do alvo em 20% por 3 rodadas.",
            posicoesValidas: new[] { 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: -60m,
            modificadorAcerto: 95m,
            modificadorCritico: 7.5m,
            efeitos: new[]
            {
                Efeito("Envenenamento", AlvoDeEfeito.Inimigo, 2m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Redução de Resistência a Envenenamento", AlvoDeEfeito.Inimigo, 20m, UnidadeDeEfeito.Percentual, duracao: 3),
            },
            id: IdDeterministico("Poison Dart"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Truque Tóxico",
            nomeOriginal: "Toxin Trickery",
            descricao: "Remove sangramento e envenenamento, concede +9 esquiva e +2 velocidade por 1 batalha. Limite 1 uso por batalha.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Remove Envenenamento", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Remove Sangramento", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Esquiva", AlvoDeEfeito.Self, 9m, UnidadeDeEfeito.Pontos, duracao: 1),
                Efeito("Bônus de Velocidade", AlvoDeEfeito.Self, 2m, UnidadeDeEfeito.Pontos, duracao: 1),
            },
            limitePorUso: new LimitePorUso(EscopoDeLimite.Batalha, 1),
            id: IdDeterministico("Toxin Trickery"));
    }

    /// `Gallows Humor` é criada aqui e compartilhada com o Bandido via NomesPorClasse (FR-009).
    private static IEnumerable<HabilidadeDeAcampamento> LadraoDeCovaAcampamento()
    {
        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Caixa de Rapé",
            nomeOriginal: "Snuff Box",
            descricao: "Remove doença da Ladra e de um aliado.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.SelfEUmAliado,
            efeitos: new[]
            {
                Efeito("Remove Doença", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Remove Doença", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Snuff Box"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Humor Negro",
            nomeOriginal: "Gallows Humor",
            descricao: "Piada mórbida: -25 estresse do próprio, -20 estresse dos aliados com 75% de chance ou +10 estresse com 25% de chance.",
            custoDeDescanso: 4,
            alvo: AlvoDeAcampamento.PartyInteira,
            efeitos: new[]
            {
                Efeito("Redução de Estresse", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Estresse", AlvoDeEfeito.Aliado, 20m, UnidadeDeEfeito.Pontos, chance: 75m),
                Efeito("Aumento de Estresse", AlvoDeEfeito.Aliado, 10m, UnidadeDeEfeito.Pontos, chance: 25m),
            },
            id: IdDeterministico("Gallows Humor"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Andanças Noturnas",
            nomeOriginal: "Night Moves",
            descricao: "Concede +20% de chance de exploração por 4 batalhas.",
            custoDeDescanso: 2,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Bônus de Exploração", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Night Moves"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Furto",
            nomeOriginal: "Pilfer",
            descricao: "Produz um item de suprimento aleatório.",
            custoDeDescanso: 1,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Produz Suprimento", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Pilfer"));
    }
}
