using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

public static partial class HabilidadesSeed
{
    private static IEnumerable<HabilidadeDeCombate> MedicoDaPesteCombate()
    {
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Explosão Nociva",
            nomeOriginal: "Noxious Blast",
            descricao: "Aplica envenenamento (5 pts/rd por 3 rodadas) e reduz a precisão do alvo em 5.",
            posicoesValidas: new[] { 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: -80m,
            modificadorAcerto: 95m,
            modificadorCritico: 5m,
            efeitos: new[]
            {
                Efeito("Envenenamento", AlvoDeEfeito.Inimigo, 5m, UnidadeDeEfeito.Pontos, duracao: 3),
                Efeito("Redução de Precisão", AlvoDeEfeito.Inimigo, 5m, UnidadeDeEfeito.Pontos, duracao: 3),
            },
            id: IdDeterministico("Noxious Blast"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Granada da Peste",
            nomeOriginal: "Plague Grenade",
            descricao: "Granada em área nas duas últimas posições inimigas; envenenamento 4 pts/rd por 3 rodadas.",
            posicoesValidas: new[] { 3, 4 },
            posicoesQueAtinge: new[] { 3, 4 },
            alvoEmArea: true,
            modificadorDano: -90m,
            modificadorAcerto: 95m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Envenenamento", AlvoDeEfeito.Inimigo, 4m, UnidadeDeEfeito.Pontos, duracao: 3),
            },
            id: IdDeterministico("Plague Grenade"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Gás Cegante",
            nomeOriginal: "Blinding Gas",
            descricao: "Gás em área que atordoa as duas últimas posições inimigas com 100% de chance. Limite 3 usos por batalha.",
            posicoesValidas: new[] { 3, 4 },
            posicoesQueAtinge: new[] { 3, 4 },
            alvoEmArea: true,
            modificadorDano: -100m,
            modificadorAcerto: 95m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Atordoamento", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Rodadas, duracao: 1),
            },
            limitePorUso: new LimitePorUso(EscopoDeLimite.Batalha, 3),
            id: IdDeterministico("Blinding Gas"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Incisão",
            nomeOriginal: "Incision",
            descricao: "Ataque corpo-a-corpo que aplica sangramento (2 pts/rd por 3 rodadas).",
            posicoesValidas: new[] { 1, 2, 3 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 85m,
            modificadorCritico: 5m,
            efeitos: new[]
            {
                Efeito("Sangramento", AlvoDeEfeito.Inimigo, 2m, UnidadeDeEfeito.Pontos, duracao: 3),
            },
            id: IdDeterministico("Incision"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Medicina de Campo",
            nomeOriginal: "Battlefield Medicine",
            descricao: "Cura leve (1 HP) e remove sangramento e envenenamento do aliado.",
            posicoesValidas: new[] { 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Cura", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Remove Sangramento", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Remove Envenenamento", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Battlefield Medicine"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Vapores Encorajadores",
            nomeOriginal: "Emboldening Vapours",
            descricao: "Concede +20% dano e +3 velocidade ao aliado por 1 batalha. Limite 2 usos por batalha.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Bônus de Dano", AlvoDeEfeito.Aliado, 20m, UnidadeDeEfeito.Percentual, duracao: 1),
                Efeito("Bônus de Velocidade", AlvoDeEfeito.Aliado, 3m, UnidadeDeEfeito.Pontos, duracao: 1),
            },
            limitePorUso: new LimitePorUso(EscopoDeLimite.Batalha, 2),
            id: IdDeterministico("Emboldening Vapours"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Explosão Desorientadora",
            nomeOriginal: "Disorienting Blast",
            descricao: "Embaralha uma posição inimiga, atordoa o alvo e limpa todos os cadáveres.",
            posicoesValidas: new[] { 2, 3, 4 },
            posicoesQueAtinge: new[] { 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: -100m,
            modificadorAcerto: 95m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Embaralhar Alvo", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Atordoamento", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Rodadas, duracao: 1),
                Efeito("Limpar Cadáveres", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Disorienting Blast"));
    }

    private static IEnumerable<HabilidadeDeAcampamento> MedicoDaPesteAcampamento()
    {
        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Vapores Experimentais",
            nomeOriginal: "Experimental Vapours",
            descricao: "Cura 50% do HP do aliado e concede +33% cura recebida por 4 batalhas.",
            custoDeDescanso: 4,
            alvo: AlvoDeAcampamento.UmAliado,
            efeitos: new[]
            {
                Efeito("Cura", AlvoDeEfeito.Aliado, 50m, UnidadeDeEfeito.Percentual),
                Efeito("Bônus de Cura Recebida", AlvoDeEfeito.Aliado, 33m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Experimental Vapours"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Sanguessugas",
            nomeOriginal: "Leeches",
            descricao: "Cura 15% do HP do aliado e remove envenenamento e doenças.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.UmAliado,
            efeitos: new[]
            {
                Efeito("Cura", AlvoDeEfeito.Aliado, 15m, UnidadeDeEfeito.Percentual),
                Efeito("Remove Envenenamento", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Remove Doença", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Leeches"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "A Cura",
            nomeOriginal: "The Cure",
            descricao: "Remove doenças da própria Médica e concede +20% resistência a doença por 4 batalhas.",
            custoDeDescanso: 1,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Remove Doença", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Resistência a Doença", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("The Cure"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Auto-Medicação",
            nomeOriginal: "Self-Medicate",
            descricao: "Cura 20% do HP, remove sangramento e envenenamento, reduz estresse e concede +10 precisão por 4 batalhas.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Redução de Estresse", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Pontos),
                Efeito("Cura", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual),
                Efeito("Remove Sangramento", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Remove Envenenamento", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Precisão", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Pontos, duracao: 4),
            },
            id: IdDeterministico("Self-Medicate"));
    }
}
