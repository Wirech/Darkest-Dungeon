using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

public static partial class HabilidadesSeed
{
    private static IEnumerable<HabilidadeDeCombate> RompedorCombate()
    {
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Perfurar",
            nomeOriginal: "Pierce",
            descricao: "Ataque com penetração de armadura que atinge qualquer posição inimiga; Rompedora avança 1.",
            posicoesValidas: new[] { 1, 2, 3 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: -10m,
            modificadorAcerto: 90m,
            modificadorCritico: 5m,
            efeitos: new[]
            {
                Efeito("Penetração de Armadura", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Avanço", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Pierce"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Puncionar",
            nomeOriginal: "Puncture",
            descricao: "Ignora e quebra guarda; alvo não pode ser guardado por 2 rodadas, puxado 2 posições e recebe -1 velocidade por 4 rodadas; Rompedora avança 1.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: -50m,
            modificadorAcerto: 90m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Ignora e Quebra Guarda", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Não Pode Ser Guardado", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Rodadas, duracao: 2),
                Efeito("Puxar", AlvoDeEfeito.Inimigo, 2m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Velocidade", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Avanço", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Puncture"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Beijo da Víbora",
            nomeOriginal: "Adder's Kiss",
            descricao: "Aplica envenenamento (3 pts/rd por 3 rodadas); Rompedora recua 1.",
            posicoesValidas: new[] { 1 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 90m,
            modificadorCritico: 5m,
            efeitos: new[]
            {
                Efeito("Envenenamento", AlvoDeEfeito.Inimigo, 3m, UnidadeDeEfeito.Pontos, duracao: 3),
                Efeito("Recuo", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Adder's Kiss"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Empalar",
            nomeOriginal: "Impale",
            descricao: "AOE total que atinge todas as posições inimigas; Rompedora recua 1.",
            posicoesValidas: new[] { 1 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: true,
            modificadorDano: -60m,
            modificadorAcerto: 90m,
            modificadorCritico: -6m,
            efeitos: new[]
            {
                Efeito("Recuo", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Impale"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Expor",
            nomeOriginal: "Expose",
            descricao: "Remove furtividade, aplica +8% de crítico recebido por 3 rodadas e -4 velocidade por 4 rodadas; Rompedora recua 1.",
            posicoesValidas: new[] { 1, 2, 3 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: false,
            modificadorDano: -40m,
            modificadorAcerto: 85m,
            modificadorCritico: 2.5m,
            efeitos: new[]
            {
                Efeito("Remove Furtividade", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Aumento de Crítico Recebido", AlvoDeEfeito.Inimigo, 8m, UnidadeDeEfeito.Percentual, duracao: 3),
                Efeito("Redução de Velocidade", AlvoDeEfeito.Inimigo, 4m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Recuo", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Expose"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Cativar",
            nomeOriginal: "Captivate",
            descricao: "+40% dano contra marcados; aplica envenenamento (3 pts/rd por 3 rodadas).",
            posicoesValidas: new[] { 2, 3 },
            posicoesQueAtinge: new[] { 2, 3 },
            alvoEmArea: false,
            modificadorDano: -25m,
            modificadorAcerto: 85m,
            modificadorCritico: 4m,
            efeitos: new[]
            {
                Efeito("Bônus de Dano vs Marcado", AlvoDeEfeito.Self, 40m, UnidadeDeEfeito.Percentual),
                Efeito("Envenenamento", AlvoDeEfeito.Inimigo, 3m, UnidadeDeEfeito.Pontos, duracao: 3),
            },
            id: IdDeterministico("Captivate"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Balanço da Serpente",
            nomeOriginal: "Serpent Sway",
            descricao: "Avança 1 posição, ativa 2 bloqueios e +1 velocidade por 4 rodadas. Limite 2 usos por batalha.",
            posicoesValidas: new[] { 1, 2, 3 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Avanço", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Bloqueios", AlvoDeEfeito.Self, 2m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Velocidade", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos, duracao: 4),
            },
            limitePorUso: new LimitePorUso(EscopoDeLimite.Batalha, 2),
            id: IdDeterministico("Serpent Sway"));
    }

    private static IEnumerable<HabilidadeDeAcampamento> RompedorAcampamento()
    {
        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Olhos da Serpente",
            nomeOriginal: "Snake Eyes",
            descricao: "Concede +15% de penetração de armadura a todos os aliados por 4 batalhas.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.TodosOsAliados,
            efeitos: new[]
            {
                Efeito("Bônus de Penetração de Armadura", AlvoDeEfeito.Aliado, 15m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Snake Eyes"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Pele de Serpente",
            nomeOriginal: "Snake Skin",
            descricao: "+15% proteção e +15% HP máximo por 4 batalhas.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Bônus de Proteção", AlvoDeEfeito.Self, 15m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de HP Máximo", AlvoDeEfeito.Self, 15m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Snake Skin"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Tempestade de Areia",
            nomeOriginal: "Sandstorm",
            descricao: "O aliado não pode ser marcado por 4 batalhas.",
            custoDeDescanso: 2,
            alvo: AlvoDeAcampamento.UmAliado,
            efeitos: new[]
            {
                Efeito("Não Pode Ser Marcado", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Rodadas, duracao: 4),
            },
            id: IdDeterministico("Sandstorm"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Abraço da Víbora",
            nomeOriginal: "Adder's Embrace",
            descricao: "+20% chance de envenenar e +20% resistência a envenenamento por 4 batalhas.",
            custoDeDescanso: 2,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Bônus de Chance de Envenenamento", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de Resistência a Envenenamento", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Adder's Embrace"));
    }
}
