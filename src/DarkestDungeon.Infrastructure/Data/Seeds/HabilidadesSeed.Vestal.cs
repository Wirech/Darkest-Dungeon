using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

public static partial class HabilidadesSeed
{
    private static IEnumerable<HabilidadeDeCombate> VestalCombate()
    {
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Golpe de Maça",
            nomeOriginal: "Mace Bash",
            descricao: "Ataque corpo-a-corpo com maça; concede bônus de dano contra criaturas Não-Mortas.",
            posicoesValidas: new[] { 1, 2 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 85m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Bônus de Dano vs Não-Mortos", AlvoDeEfeito.Self, 15m, UnidadeDeEfeito.Percentual),
            },
            id: IdDeterministico("Mace Bash"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Julgamento",
            nomeOriginal: "Judgement",
            descricao: "Ataque à distância que também cura 3 pontos da Vestal (auto-cura).",
            posicoesValidas: new[] { 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: -25m,
            modificadorAcerto: 85m,
            modificadorCritico: 5m,
            efeitos: new[]
            {
                Efeito("Cura", AlvoDeEfeito.Self, 3m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Judgement"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Luz Deslumbrante",
            nomeOriginal: "Dazzling Light",
            descricao: "Explosão de luz que aumenta a tocha em 6 pontos e atordoa o alvo.",
            posicoesValidas: new[] { 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: false,
            modificadorDano: -75m,
            modificadorAcerto: 90m,
            modificadorCritico: 5m,
            efeitos: new[]
            {
                Efeito("Tocha", AlvoDeEfeito.Self, 6m, UnidadeDeEfeito.Pontos),
                Efeito("Atordoamento", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Rodadas, duracao: 1),
            },
            id: IdDeterministico("Dazzling Light"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Graça Divina",
            nomeOriginal: "Divine Grace",
            descricao: "Cura direta de 4 a 5 pontos de HP em um único aliado.",
            posicoesValidas: new[] { 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Cura", AlvoDeEfeito.Aliado, 5m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Divine Grace"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Conforto Divino",
            nomeOriginal: "Divine Comfort",
            descricao: "Cura em área de 1 a 3 pontos de HP para até três aliados adjacentes.",
            posicoesValidas: new[] { 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: true,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Cura", AlvoDeEfeito.Aliado, 3m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Divine Comfort"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Iluminação",
            nomeOriginal: "Illumination",
            descricao: "Feixe de luz que remove furtividade, aumenta tocha e reduz a esquiva do alvo por 4 rodadas.",
            posicoesValidas: new[] { 1, 2, 3 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: -75m,
            modificadorAcerto: 90m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Remove Furtividade", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Tocha", AlvoDeEfeito.Self, 5m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Esquiva", AlvoDeEfeito.Inimigo, 20m, UnidadeDeEfeito.Pontos, duracao: 4),
            },
            id: IdDeterministico("Illumination"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Mão da Luz",
            nomeOriginal: "Hand of Light",
            descricao: "Ataque à distância que buffa a próxima ação da Vestal com +6 precisão e +25% dano por 4 rodadas.",
            posicoesValidas: new[] { 1, 2 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: false,
            modificadorDano: -50m,
            modificadorAcerto: 85m,
            modificadorCritico: 1m,
            efeitos: new[]
            {
                Efeito("Bônus de Dano vs Não-Mortos", AlvoDeEfeito.Self, 15m, UnidadeDeEfeito.Percentual),
                Efeito("Bônus de Precisão", AlvoDeEfeito.Self, 6m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Bônus de Dano", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Hand of Light"));
    }

    private static IEnumerable<HabilidadeDeAcampamento> VestalAcampamento()
    {
        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Benção",
            nomeOriginal: "Bless",
            descricao: "Concede +10 precisão e +10 esquiva a um aliado por 4 batalhas.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.UmAliado,
            efeitos: new[]
            {
                Efeito("Bônus de Precisão", AlvoDeEfeito.Aliado, 10m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Bônus de Esquiva", AlvoDeEfeito.Aliado, 10m, UnidadeDeEfeito.Pontos, duracao: 4),
            },
            id: IdDeterministico("Bless"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Canto Sacro",
            nomeOriginal: "Chant",
            descricao: "Reduz o estresse do aliado (15 se religioso, 5 se não) e o estresse recebido por 4 batalhas.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.UmAliado,
            efeitos: new[]
            {
                Efeito("Redução de Estresse", AlvoDeEfeito.Aliado, 15m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Estresse Recebido", AlvoDeEfeito.Aliado, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Chant"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Oração",
            nomeOriginal: "Pray",
            descricao: "Reduz o estresse de todos os aliados (15 se religiosos, 5 se não) e concede bônus de proteção por 4 batalhas.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.Party,
            efeitos: new[]
            {
                Efeito("Redução de Estresse", AlvoDeEfeito.Aliado, 15m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Proteção", AlvoDeEfeito.Aliado, 15m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Pray"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Santuário",
            nomeOriginal: "Sanctuary",
            descricao: "Impede emboscadas noturnas (se religiosa) e cura aliados com debuffs de mortalidade.",
            custoDeDescanso: 4,
            alvo: AlvoDeAcampamento.PartyInteira,
            efeitos: new[]
            {
                Efeito("Impede Emboscada Noturna", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Cura Mortalidade", AlvoDeEfeito.Aliado, 50m, UnidadeDeEfeito.Percentual),
                Efeito("Redução de Estresse", AlvoDeEfeito.Aliado, 25m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Sanctuary"));
    }
}
