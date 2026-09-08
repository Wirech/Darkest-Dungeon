using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

public static partial class HabilidadesSeed
{
    private static IEnumerable<HabilidadeDeCombate> FugitivoCombate()
    {
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Golpe Escaldante",
            nomeOriginal: "Searing Strike",
            descricao: "Ataque melee que aplica Queimadura 3 pts e concede +15% de dano contra Queimados.",
            posicoesValidas: new[] { 1, 2, 3 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 85m,
            modificadorCritico: 3m,
            efeitos: new[]
            {
                Efeito("Queimadura", AlvoDeEfeito.Inimigo, 3m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Dano vs Queimado", AlvoDeEfeito.Self, 15m, UnidadeDeEfeito.Percentual),
            },
            id: IdDeterministico("Searing Strike"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Vaga-lume",
            nomeOriginal: "Firefly",
            descricao: "Projétil ranged que aplica Queimadura 7 pts, -50% de decaimento de queimadura por 2 rodadas, e +4 tocha.",
            posicoesValidas: new[] { 3, 4 },
            posicoesQueAtinge: new[] { 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: -75m,
            modificadorAcerto: 85m,
            modificadorCritico: 6m,
            efeitos: new[]
            {
                Efeito("Queimadura", AlvoDeEfeito.Inimigo, 7m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Decaimento de Queimadura", AlvoDeEfeito.Inimigo, 50m, UnidadeDeEfeito.Percentual, duracao: 2),
                Efeito("Tocha", AlvoDeEfeito.Self, 4m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Firefly"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Corra e Esconda",
            nomeOriginal: "Run and Hide",
            descricao: "Recua 2 posições e entra em furtividade por 4 rodadas (não utilizável em pos 1/2); enquanto Furtiva, cura 2 pts/rd por 2 rodadas e +50% valor de Queimadura por 4 rodadas.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Recuo", AlvoDeEfeito.Self, 2m, UnidadeDeEfeito.Pontos),
                Efeito("Furtividade", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Rodadas, duracao: 4),
                Efeito("Cura ao Longo do Tempo", AlvoDeEfeito.Self, 2m, UnidadeDeEfeito.Pontos, duracao: 2),
                Efeito("Bônus de Valor de Queimadura", AlvoDeEfeito.Self, 50m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Run and Hide"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Saquear",
            nomeOriginal: "Ransack",
            descricao: "Ataque melee que puxa o alvo 1 posição, +10 precisão enquanto Furtiva, limpa todos os cadáveres; Fugitiva avança 1.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: -10m,
            modificadorAcerto: 85m,
            modificadorCritico: 3m,
            efeitos: new[]
            {
                Efeito("Puxar", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Precisão enquanto Furtiva", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Pontos),
                Efeito("Limpar Cadáveres", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Avanço", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Ransack"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Luz da Fogueira",
            nomeOriginal: "Hearthlight",
            descricao: "AOE total que remove furtividade dos inimigos, +6 tocha e concede a todos os heróis +5 precisão e +8% crítico contra Queimados.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: true,
            modificadorDano: -100m,
            modificadorAcerto: 100m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Remove Furtividade", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Tocha", AlvoDeEfeito.Self, 6m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Precisão", AlvoDeEfeito.Aliado, 5m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Crítico vs Queimado", AlvoDeEfeito.Aliado, 8m, UnidadeDeEfeito.Percentual),
            },
            id: IdDeterministico("Hearthlight"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Queima Controlada",
            nomeOriginal: "Controlled Burn",
            descricao: "Aplica Queimadura 4 pts, +6 tocha e um debuff que gera queimaduras contínuas na posição inimiga por 3 rodadas. Limite 2 usos por batalha.",
            posicoesValidas: new[] { 1, 2, 3 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: false,
            modificadorDano: -100m,
            modificadorAcerto: 95m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Queimadura", AlvoDeEfeito.Inimigo, 4m, UnidadeDeEfeito.Pontos),
                Efeito("Tocha", AlvoDeEfeito.Self, 6m, UnidadeDeEfeito.Pontos),
                Efeito("Debuff Queima Controlada", AlvoDeEfeito.Inimigo, 3m, UnidadeDeEfeito.Rodadas, duracao: 3),
            },
            limitePorUso: new LimitePorUso(EscopoDeLimite.Batalha, 2),
            id: IdDeterministico("Controlled Burn"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Refluxo",
            nomeOriginal: "Backdraft",
            descricao: "Requer alvo Queimado; ignora guarda, aplica dano à posição atrás do alvo, +25% dano por pilha de Queimadura, copia Queimadura do 1º alvo para o 2º e cura Queimadura do 1º.",
            posicoesValidas: new[] { 1, 2, 3 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: -50m,
            modificadorAcerto: 95m,
            modificadorCritico: 7m,
            efeitos: new[]
            {
                Efeito("Ignora Guarda (Requer Queimado)", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Dano à Posição Atrás", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Dano por Pilha de Queimadura", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual),
                Efeito("Copiar Queimadura para Alvo Trás", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Curar Queimadura do Alvo Frente", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Backdraft"));
    }

    private static IEnumerable<HabilidadeDeAcampamento> FugitivoAcampamento()
    {
        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Atear Fogo",
            nomeOriginal: "Kindle",
            descricao: "Produz três Pilhas de Cinzas e impede emboscadas noturnas.",
            custoDeDescanso: 4,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Produz Pilhas de Cinzas", AlvoDeEfeito.Self, 3m, UnidadeDeEfeito.Pontos),
                Efeito("Impede Emboscada Noturna", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Kindle"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Cauterizar",
            nomeOriginal: "Cauterize",
            descricao: "Remove doença, sangramento e envenenamento do aliado; -20% resistência a debuff por 4 batalhas.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.UmAliado,
            efeitos: new[]
            {
                Efeito("Remove Doença", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Remove Sangramento", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Remove Envenenamento", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Resistência a Debuff", AlvoDeEfeito.Aliado, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Cauterize"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Brincar com Fogo",
            nomeOriginal: "Play with Fire",
            descricao: "Ao atingir: aplica Queimadura 3 no aliado por 4 batalhas; +10% dano recebido por 4 batalhas.",
            custoDeDescanso: 2,
            alvo: AlvoDeAcampamento.UmAliado,
            efeitos: new[]
            {
                Efeito("Queimadura no Ataque", AlvoDeEfeito.Aliado, 3m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Aumento de Dano Recebido", AlvoDeEfeito.Aliado, 10m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Play with Fire"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Bater Carteira",
            nomeOriginal: "Pick Pocket",
            descricao: "Produz uma Chave-Mestra (3 usos).",
            custoDeDescanso: 1,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Produz Chave-Mestra", AlvoDeEfeito.Self, 3m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Pick Pocket"));
    }
}
