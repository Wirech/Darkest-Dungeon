using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

public static partial class HabilidadesSeed
{
    private static IEnumerable<HabilidadeDeCombate> MestreDeCacaCombate()
    {
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Investida do Cão",
            nomeOriginal: "Hound's Rush",
            descricao: "Cão avança e ataca com +15% dano vs Bestas e +60% dano vs Marcados; aplica sangramento 1 pt/rd por 3 rodadas.",
            posicoesValidas: new[] { 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 85m,
            modificadorCritico: 5m,
            efeitos: new[]
            {
                Efeito("Sangramento", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos, duracao: 3),
                Efeito("Bônus de Dano vs Besta", AlvoDeEfeito.Self, 15m, UnidadeDeEfeito.Percentual),
                Efeito("Bônus de Dano vs Marcado", AlvoDeEfeito.Self, 60m, UnidadeDeEfeito.Percentual),
            },
            id: IdDeterministico("Hound's Rush"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Assédio do Cão",
            nomeOriginal: "Hound's Harry",
            descricao: "AOE total que aplica sangramento 1 pt/rd por 3 rodadas em toda a formação inimiga.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: true,
            modificadorDano: -75m,
            modificadorAcerto: 85m,
            modificadorCritico: -5m,
            efeitos: new[]
            {
                Efeito("Sangramento", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos, duracao: 3),
            },
            id: IdDeterministico("Hound's Harry"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Assobio de Alvo",
            nomeOriginal: "Target Whistle",
            descricao: "Marca o alvo por 3 rodadas e reduz sua proteção em 20% por 4 rodadas.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: -100m,
            modificadorAcerto: 100m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Marcação", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Rodadas, duracao: 3),
                Efeito("Redução de Proteção", AlvoDeEfeito.Inimigo, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Target Whistle"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Grito de Guerra",
            nomeOriginal: "Cry Havoc",
            descricao: "Reduz o estresse dos aliados em 2 pontos com 66% de chance base.",
            posicoesValidas: new[] { 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: true,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Redução de Estresse", AlvoDeEfeito.Aliado, 2m, UnidadeDeEfeito.Pontos, chance: 66m),
            },
            id: IdDeterministico("Cry Havoc"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Cão de Guarda",
            nomeOriginal: "Guard Dog",
            descricao: "Cão protege o aliado por 2 rodadas; Mestre de Caça ganha +10 esquiva por 3 rodadas.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Guarda", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Rodadas, duracao: 2),
                Efeito("Bônus de Esquiva", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Pontos, duracao: 3),
            },
            id: IdDeterministico("Guard Dog"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Lamber Feridas",
            nomeOriginal: "Lick Wounds",
            descricao: "Auto-cura de 4 HP.",
            posicoesValidas: new[] { 1, 2, 3 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Cura", AlvoDeEfeito.Self, 4m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Lick Wounds"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Cassetete",
            nomeOriginal: "Blackjack",
            descricao: "Golpe corpo-a-corpo que atordoa o alvo (chance 110% base capada em 100).",
            posicoesValidas: new[] { 1, 2 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: false,
            modificadorDano: -65m,
            modificadorAcerto: 95m,
            modificadorCritico: 5m,
            efeitos: new[]
            {
                Efeito("Atordoamento", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Rodadas, duracao: 1),
            },
            id: IdDeterministico("Blackjack"));
    }

    private static IEnumerable<HabilidadeDeAcampamento> MestreDeCacaAcampamento()
    {
        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Vigília do Cão",
            nomeOriginal: "Hound's Watch",
            descricao: "-20% chance da party ser surpreendida, +20% chance de surpreender monstros e impede emboscadas noturnas por 4 batalhas.",
            custoDeDescanso: 4,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Redução de Chance de Ser Surpreendido", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Aumento de Chance de Surpreender", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Impede Emboscada Noturna", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Hound's Watch"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Cão de Terapia",
            nomeOriginal: "Therapy Dog",
            descricao: "-10 estresse em todos os aliados e -10% estresse recebido por 4 batalhas.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.TodosOsAliados,
            efeitos: new[]
            {
                Efeito("Redução de Estresse", AlvoDeEfeito.Aliado, 10m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Estresse Recebido", AlvoDeEfeito.Aliado, 10m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Therapy Dog"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Melhor Amigo do Homem",
            nomeOriginal: "Man's Best Friend",
            descricao: "Reduz 20 pontos de estresse do próprio Mestre de Caça.",
            custoDeDescanso: 2,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Redução de Estresse", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Man's Best Friend"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Soltar o Cão",
            nomeOriginal: "Release the Hound",
            descricao: "+30% chance de exploração por 4 batalhas.",
            custoDeDescanso: 4,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Bônus de Exploração", AlvoDeEfeito.Self, 30m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Release the Hound"));
    }
}
