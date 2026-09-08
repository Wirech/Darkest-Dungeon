using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

public static partial class HabilidadesSeed
{
    private static IEnumerable<HabilidadeDeCombate> CacadorDeRecompensasCombate()
    {
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Coletar Recompensa",
            nomeOriginal: "Collect Bounty",
            descricao: "Executa o alvo com +90% de dano contra marcados e +15% contra humanos.",
            posicoesValidas: new[] { 1, 2, 3 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 85m,
            modificadorCritico: 7m,
            efeitos: new[]
            {
                Efeito("Bônus de Dano vs Marcado", AlvoDeEfeito.Self, 90m, UnidadeDeEfeito.Percentual),
                Efeito("Bônus de Dano vs Humano", AlvoDeEfeito.Self, 15m, UnidadeDeEfeito.Percentual),
            },
            id: IdDeterministico("Collect Bounty"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Marca de Morte",
            nomeOriginal: "Mark for Death",
            descricao: "Marca o alvo por 3 rodadas, reduz sua proteção em 10% e concede +3 velocidade ao Caçador por 2 rodadas.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: -100m,
            modificadorAcerto: 100m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Marcação", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Rodadas, duracao: 3),
                Efeito("Redução de Proteção", AlvoDeEfeito.Inimigo, 10m, UnidadeDeEfeito.Percentual, duracao: 3),
                Efeito("Bônus de Velocidade", AlvoDeEfeito.Self, 3m, UnidadeDeEfeito.Pontos, duracao: 2),
            },
            id: IdDeterministico("Mark for Death"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Venha Cá",
            nomeOriginal: "Come Hither",
            descricao: "Puxa o alvo 2 posições para frente e o marca por 2 rodadas.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 3, 4 },
            alvoEmArea: false,
            modificadorDano: -80m,
            modificadorAcerto: 90m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Marcação", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Rodadas, duracao: 2),
                Efeito("Puxar", AlvoDeEfeito.Inimigo, 2m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Come Hither"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Soco Cruzado",
            nomeOriginal: "Uppercut",
            descricao: "Empurra o alvo 2 posições para trás e o atordoa com 100% de chance base.",
            posicoesValidas: new[] { 1, 2 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: -67m,
            modificadorAcerto: 90m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Empurrão", AlvoDeEfeito.Inimigo, 2m, UnidadeDeEfeito.Pontos),
                Efeito("Atordoamento", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Rodadas, duracao: 1),
            },
            id: IdDeterministico("Uppercut"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Bomba de Fumaça",
            nomeOriginal: "Flashbang",
            descricao: "Atordoa o alvo (chance 110% base capada em 100) e embaralha sua posição.",
            posicoesValidas: new[] { 2, 3, 4 },
            posicoesQueAtinge: new[] { 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: -100m,
            modificadorAcerto: 95m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Atordoamento", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Rodadas, duracao: 1),
                Efeito("Embaralhar Alvo", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Flashbang"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Acabe com Ele",
            nomeOriginal: "Finish Him",
            descricao: "Ataque de execução com +25% de dano contra alvos atordoados.",
            posicoesValidas: new[] { 1, 2, 3 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 85m,
            modificadorCritico: 5m,
            efeitos: new[]
            {
                Efeito("Bônus de Dano vs Atordoado", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual),
            },
            id: IdDeterministico("Finish Him"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Estrepes",
            nomeOriginal: "Caltrops",
            descricao: "Aplica sangramento (2 pts/rd por 3 rodadas), +10% dano recebido e -4 velocidade no alvo por 3 rodadas.",
            posicoesValidas: new[] { 2, 3, 4 },
            posicoesQueAtinge: new[] { 3, 4 },
            alvoEmArea: false,
            modificadorDano: -95m,
            modificadorAcerto: 90m,
            modificadorCritico: 5m,
            efeitos: new[]
            {
                Efeito("Sangramento", AlvoDeEfeito.Inimigo, 2m, UnidadeDeEfeito.Pontos, duracao: 3),
                Efeito("Aumento de Dano Recebido", AlvoDeEfeito.Inimigo, 10m, UnidadeDeEfeito.Percentual, duracao: 3),
                Efeito("Redução de Velocidade", AlvoDeEfeito.Inimigo, 4m, UnidadeDeEfeito.Pontos, duracao: 3),
            },
            id: IdDeterministico("Caltrops"));
    }

    private static IEnumerable<HabilidadeDeAcampamento> CacadorDeRecompensasAcampamento()
    {
        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "É Assim que a Gente Faz",
            nomeOriginal: "This Is How We Do It",
            descricao: "+10 precisão e +8% crítico por 4 batalhas.",
            custoDeDescanso: 2,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Bônus de Precisão", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Bônus de Crítico", AlvoDeEfeito.Self, 8m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("This Is How We Do It"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Rastreamento",
            nomeOriginal: "Tracking",
            descricao: "-15% chance da party ser surpreendida e +10% chance de surpreender monstros por 4 batalhas.",
            custoDeDescanso: 2,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Redução de Chance de Ser Surpreendido", AlvoDeEfeito.Self, 15m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Aumento de Chance de Surpreender", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Tracking"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Abate Planejado",
            nomeOriginal: "Planned Takedown",
            descricao: "+25% dano e +15 precisão contra inimigos grandes (tamanho 2+) por 4 batalhas.",
            custoDeDescanso: 4,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Bônus de Dano vs Grande", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de Precisão vs Grande", AlvoDeEfeito.Self, 15m, UnidadeDeEfeito.Pontos, duracao: 4),
            },
            id: IdDeterministico("Planned Takedown"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Explorar à Frente",
            nomeOriginal: "Scout Ahead",
            descricao: "+25% chance de exploração por 4 batalhas.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Bônus de Exploração", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Scout Ahead"));
    }
}
