using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

public static partial class HabilidadesSeed
{
    private static IEnumerable<HabilidadeDeCombate> AntiquarioCombate()
    {
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Facada Nervosa",
            nomeOriginal: "Nervous Stab",
            descricao: "Ataque melee fraco mas versátil da Antiquária; utilizável de qualquer posição.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 85m,
            modificadorCritico: 3m,
            efeitos: Array.Empty<EfeitoDeHabilidade>(),
            id: IdDeterministico("Nervous Stab"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Vapores Purulentos",
            nomeOriginal: "Festering Vapours",
            descricao: "Nuvem envenenante que aplica 1 pt/rd de envenenamento por 3 rodadas e reduz a resistência a envenenamento em 20%.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: -75m,
            modificadorAcerto: 95m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Envenenamento", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos, duracao: 3),
                Efeito("Redução de Resistência a Envenenamento", AlvoDeEfeito.Inimigo, 20m, UnidadeDeEfeito.Percentual, duracao: 3),
            },
            id: IdDeterministico("Festering Vapours"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Abaixe-se!",
            nomeOriginal: "Get Down!",
            descricao: "Recua 2 posições e concede +10% chance de aplicar envenenamento, +15 esquiva e +1 velocidade por 4 rodadas.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Recuo", AlvoDeEfeito.Self, 2m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Chance de Envenenamento", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de Esquiva", AlvoDeEfeito.Self, 15m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Bônus de Velocidade", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos, duracao: 4),
            },
            id: IdDeterministico("Get Down!"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Pó de Flash",
            nomeOriginal: "Flashpowder",
            descricao: "Explosão luminosa que reduz a precisão do alvo em 10 por 2 rodadas e remove furtividade.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: -100m,
            modificadorAcerto: 95m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Redução de Precisão", AlvoDeEfeito.Inimigo, 10m, UnidadeDeEfeito.Pontos, duracao: 2),
                Efeito("Remove Furtividade", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Flashpowder"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Vapores Fortificantes",
            nomeOriginal: "Fortifying Vapours",
            descricao: "Cura leve (1 HP) e concede +10% resistência a sangramento e envenenamento por 3 rodadas.",
            posicoesValidas: new[] { 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Cura", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Resistência a Sangramento", AlvoDeEfeito.Aliado, 10m, UnidadeDeEfeito.Percentual, duracao: 3),
                Efeito("Bônus de Resistência a Envenenamento", AlvoDeEfeito.Aliado, 10m, UnidadeDeEfeito.Percentual, duracao: 3),
            },
            id: IdDeterministico("Fortifying Vapours"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Vapores Revigorantes",
            nomeOriginal: "Invigorating Vapours",
            descricao: "Concede +3 esquiva a três aliados adjacentes por 3 rodadas.",
            posicoesValidas: new[] { 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: true,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Bônus de Esquiva", AlvoDeEfeito.Aliado, 3m, UnidadeDeEfeito.Pontos, duracao: 3),
            },
            id: IdDeterministico("Invigorating Vapours"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Proteja-me!",
            nomeOriginal: "Protect Me",
            descricao: "Força um aliado a guardá-la, concede +4 esquiva e +10% proteção por 4 rodadas ao próprio alvo e o marca por 3 rodadas. Limite 3 usos por batalha.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Bônus de Esquiva", AlvoDeEfeito.Aliado, 4m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Bônus de Proteção", AlvoDeEfeito.Aliado, 10m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Marcação", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Rodadas, duracao: 3),
                Efeito("Força Guarda por Aliado", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Rodadas, duracao: 2),
            },
            limitePorUso: new LimitePorUso(EscopoDeLimite.Batalha, 3),
            id: IdDeterministico("Protect Me"));
    }

    private static IEnumerable<HabilidadeDeAcampamento> AntiquarioAcampamento()
    {
        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Reabastecer",
            nomeOriginal: "Resupply",
            descricao: "Produz um item de suprimento aleatório (até 3 usos).",
            custoDeDescanso: 1,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Produz Suprimento", AlvoDeEfeito.Self, 3m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Resupply"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Vasculhar Bugigangas",
            nomeOriginal: "Trinket Scrounge",
            descricao: "Produz uma bugiganga (trinket) aleatória.",
            custoDeDescanso: 2,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Produz Bugiganga Aleatória", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Trinket Scrounge"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Pós Estranhos",
            nomeOriginal: "Strange Powders",
            descricao: "Concede +20% resistência a sangramento, envenenamento, movimento, debuff e doença por 4 batalhas.",
            custoDeDescanso: 2,
            alvo: AlvoDeAcampamento.UmAliado,
            efeitos: new[]
            {
                Efeito("Bônus de Resistência a Sangramento", AlvoDeEfeito.Aliado, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de Resistência a Envenenamento", AlvoDeEfeito.Aliado, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de Resistência a Movimento", AlvoDeEfeito.Aliado, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de Resistência a Debuff", AlvoDeEfeito.Aliado, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de Resistência a Doença", AlvoDeEfeito.Aliado, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Strange Powders"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Encantamento Curioso",
            nomeOriginal: "Curious Incantation",
            descricao: "Reduz o estresse recebido pela Antiquária em 50% por 4 batalhas.",
            custoDeDescanso: 1,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Redução de Estresse Recebido", AlvoDeEfeito.Self, 50m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Curious Incantation"));
    }
}
