using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

public static partial class HabilidadesSeed
{
    private static IEnumerable<HabilidadeDeCombate> VeteranoCombate()
    {
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Esmagar",
            nomeOriginal: "Crush",
            descricao: "Ataque corpo-a-corpo pesado do Veterano contra as três primeiras posições inimigas.",
            posicoesValidas: new[] { 1, 2 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 85m,
            modificadorCritico: 5m,
            efeitos: Array.Empty<EfeitoDeHabilidade>(),
            id: IdDeterministico("Crush"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Muralha",
            nomeOriginal: "Rampart",
            descricao: "Investida com escudo: empurra o alvo, atordoa com 100% de chance e avança o Veterano em 1 posição.",
            posicoesValidas: new[] { 1, 2, 3 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: -60m,
            modificadorAcerto: 90m,
            modificadorCritico: 5m,
            efeitos: new[]
            {
                Efeito("Empurrão", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Atordoamento", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Rodadas, duracao: 1),
                Efeito("Avanço", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Rampart"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Rugido",
            nomeOriginal: "Bellow",
            descricao: "Grito de guerra em área: reduz esquiva e velocidade dos inimigos e aumenta crítico recebido em alvos marcados por 3 rodadas.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: true,
            modificadorDano: -100m,
            modificadorAcerto: 90m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Redução de Esquiva", AlvoDeEfeito.Inimigo, 5m, UnidadeDeEfeito.Pontos, duracao: 3),
                Efeito("Redução de Velocidade", AlvoDeEfeito.Inimigo, 5m, UnidadeDeEfeito.Pontos, duracao: 3),
                Efeito("Aumento de Crítico Recebido se Marcado", AlvoDeEfeito.Inimigo, 5m, UnidadeDeEfeito.Percentual, duracao: 3),
            },
            id: IdDeterministico("Bellow"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Defensor",
            nomeOriginal: "Defender",
            descricao: "Protege um aliado por 3 rodadas e concede +15% proteção ao Veterano por 4 rodadas.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Guarda", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Rodadas, duracao: 3),
                Efeito("Bônus de Proteção", AlvoDeEfeito.Self, 15m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Defender"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Retribuição",
            nomeOriginal: "Retribution",
            descricao: "Marca a si mesmo por 2 rodadas e ativa o contra-ataque (Riposte) por 3 rodadas com -40% de dano.",
            posicoesValidas: new[] { 1, 2, 3 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: false,
            modificadorDano: -75m,
            modificadorAcerto: 85m,
            modificadorCritico: 2.5m,
            efeitos: new[]
            {
                Efeito("Marcação", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Rodadas, duracao: 2),
                Efeito("Ativa Contra-Ataque", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Rodadas, duracao: 3),
                Efeito("Contra-Ataque com Redução de Dano", AlvoDeEfeito.Self, 40m, UnidadeDeEfeito.Percentual, duracao: 3),
            },
            id: IdDeterministico("Retribution"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Comando",
            nomeOriginal: "Command",
            descricao: "Buff em área para três aliados: +5 precisão, +4% crítico e +15% dano quando Guardados por 3 rodadas.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: true,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Bônus de Precisão", AlvoDeEfeito.Aliado, 5m, UnidadeDeEfeito.Pontos, duracao: 3),
                Efeito("Bônus de Crítico", AlvoDeEfeito.Aliado, 4m, UnidadeDeEfeito.Percentual, duracao: 3),
                Efeito("Bônus de Dano se Guardado", AlvoDeEfeito.Aliado, 15m, UnidadeDeEfeito.Percentual, duracao: 3),
            },
            id: IdDeterministico("Command"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Reforçar",
            nomeOriginal: "Bolster",
            descricao: "Buff em área para três aliados: +5 esquiva e -10% estresse por 1 batalha. Limite 1 uso por batalha.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: true,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Bônus de Esquiva", AlvoDeEfeito.Aliado, 5m, UnidadeDeEfeito.Pontos, duracao: 1),
                Efeito("Redução de Estresse Recebido", AlvoDeEfeito.Aliado, 10m, UnidadeDeEfeito.Percentual, duracao: 1),
            },
            limitePorUso: new LimitePorUso(EscopoDeLimite.Batalha, 1),
            id: IdDeterministico("Bolster"));
    }

    private static IEnumerable<HabilidadeDeAcampamento> VeteranoAcampamento()
    {
        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Manter Equipamento",
            nomeOriginal: "Maintain Equipment",
            descricao: "Polir e reforçar o equipamento: +15% proteção e +15% dano por 4 batalhas.",
            custoDeDescanso: 4,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Bônus de Proteção", AlvoDeEfeito.Self, 15m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de Dano", AlvoDeEfeito.Self, 15m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Maintain Equipment"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Táticas",
            nomeOriginal: "Tactics",
            descricao: "Ensina táticas ao grupo: +10 esquiva e +5% crítico a todos os aliados por 4 batalhas.",
            custoDeDescanso: 4,
            alvo: AlvoDeAcampamento.Party,
            efeitos: new[]
            {
                Efeito("Bônus de Esquiva", AlvoDeEfeito.Aliado, 10m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Bônus de Crítico", AlvoDeEfeito.Aliado, 5m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Tactics"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Instrução",
            nomeOriginal: "Instruction",
            descricao: "Treina um aliado: +10 precisão e +3 velocidade por 4 batalhas.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.UmAliado,
            efeitos: new[]
            {
                Efeito("Bônus de Precisão", AlvoDeEfeito.Aliado, 10m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Bônus de Velocidade", AlvoDeEfeito.Aliado, 3m, UnidadeDeEfeito.Pontos, duracao: 4),
            },
            id: IdDeterministico("Instruction"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Prática de Armas",
            nomeOriginal: "Weapons Practice",
            descricao: "Treinamento de combate: +10% dano em todos os aliados e +8% crítico com 75% de chance por 4 batalhas.",
            custoDeDescanso: 4,
            alvo: AlvoDeAcampamento.TodosOsAliados,
            efeitos: new[]
            {
                Efeito("Bônus de Dano", AlvoDeEfeito.Aliado, 10m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de Crítico", AlvoDeEfeito.Aliado, 8m, UnidadeDeEfeito.Percentual, chance: 75m, duracao: 4),
            },
            id: IdDeterministico("Weapons Practice"));
    }
}
