using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

// A Duelista pode equipar TODAS as 7 skills simultaneamente e alterna entre modo Defensivo e Agressivo.
// A skill Anticipation existe em ambas as formas com efeitos distintos, mas é 1 registro compartilhado.
public static partial class HabilidadesSeed
{
    private static IEnumerable<HabilidadeDeCombate> DuelistaCombate()
    {
        // Anticipation — alterna entre modos Defensivo e Agressivo, ativando Riposte.
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Antecipação",
            nomeOriginal: "Anticipation",
            descricao: "Ação livre que ativa Contra-Ataque por 2 rodadas e alterna entre modo Defensivo/Agressivo. No Defensivo dá +3% crítico enquanto Riposte ativo; no Agressivo dá +3 esquiva enquanto Riposte ativo (4 rodadas).",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Ativa Contra-Ataque", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Rodadas, duracao: 2),
                Efeito("Alternar Modo", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Crítico ou Esquiva enquanto Riposte", AlvoDeEfeito.Self, 3m, UnidadeDeEfeito.Pontos, duracao: 4),
            },
            id: IdDeterministico("Anticipation"));

        // Defensive Stance (3 skills)
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Touché",
            nomeOriginal: "Touché",
            descricao: "[Defensivo] Golpe rápido que recua 1 e concede +25 esquiva até a próxima esquiva (4 rodadas).",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 90m,
            modificadorCritico: 1m,
            efeitos: new[]
            {
                Efeito("Recuo", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Esquiva", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Pontos, duracao: 4),
            },
            id: IdDeterministico("Touché"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Finta",
            nomeOriginal: "Feint",
            descricao: "[Defensivo] Marca a si mesma, ativa Contra-Ataque com +25% dano por 4 rodadas; reduz precisão do alvo (10 pts vs marcados) e proteção em 20%.",
            posicoesValidas: new[] { 1, 2, 3 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: -50m,
            modificadorAcerto: 95m,
            modificadorCritico: 3m,
            efeitos: new[]
            {
                Efeito("Redução de Precisão vs Marcado", AlvoDeEfeito.Inimigo, 10m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Proteção", AlvoDeEfeito.Inimigo, 20m, UnidadeDeEfeito.Percentual),
                Efeito("Marcação", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Rodadas, duracao: 2),
                Efeito("Contra-Ataque com Bônus de Dano", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Feint"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Desengajar",
            nomeOriginal: "Disengage",
            descricao: "[Defensivo] Recua 3 posições, +4 velocidade e +10 precisão por 4 rodadas; ataques utilizáveis de qualquer posição por 2 rodadas.",
            posicoesValidas: new[] { 1, 2 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: -50m,
            modificadorAcerto: 95m,
            modificadorCritico: -1m,
            efeitos: new[]
            {
                Efeito("Recuo", AlvoDeEfeito.Self, 3m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Velocidade", AlvoDeEfeito.Self, 4m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Bônus de Precisão", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Ataques de Qualquer Posição", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Rodadas, duracao: 2),
            },
            id: IdDeterministico("Disengage"));

        // Aggressive Stance (3 skills)
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Flèche",
            nomeOriginal: "Flèche",
            descricao: "[Agressivo] Investida com +20% dano; avança 2 posições e se auto-marca por 2 rodadas.",
            posicoesValidas: new[] { 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3 },
            alvoEmArea: false,
            modificadorDano: 20m,
            modificadorAcerto: 90m,
            modificadorCritico: 9m,
            efeitos: new[]
            {
                Efeito("Avanço", AlvoDeEfeito.Self, 2m, UnidadeDeEfeito.Pontos),
                Efeito("Marcação", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Rodadas, duracao: 2),
            },
            id: IdDeterministico("Flèche"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Golpe de Misericórdia",
            nomeOriginal: "Coup de Grâce",
            descricao: "[Agressivo] Golpe ranged com penetração de armadura; +100% crítico contra atordoados; conceder +1 ação ao matar.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: -60m,
            modificadorAcerto: 95m,
            modificadorCritico: 7m,
            efeitos: new[]
            {
                Efeito("Penetração de Armadura", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Crítico vs Atordoado", AlvoDeEfeito.Self, 100m, UnidadeDeEfeito.Percentual),
                Efeito("Ação Extra ao Matar", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Coup de Grâce"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Chute",
            nomeOriginal: "The Boot",
            descricao: "[Agressivo] Chute que atordoa (100% base) e empurra o alvo (110% base capado em 100); Duelista avança 1.",
            posicoesValidas: new[] { 1, 2, 3 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: -40m,
            modificadorAcerto: 95m,
            modificadorCritico: 1m,
            efeitos: new[]
            {
                Efeito("Atordoamento", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Rodadas, duracao: 1),
                Efeito("Empurrão", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Avanço", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("The Boot"));
    }

    private static IEnumerable<HabilidadeDeAcampamento> DuelistaAcampamento()
    {
        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Meditação",
            nomeOriginal: "Meditation",
            descricao: "+10 precisão e +10 esquiva enquanto Contra-Ataque ativo por 4 batalhas.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Bônus de Precisão com Riposte", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Bônus de Esquiva com Riposte", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Pontos, duracao: 4),
            },
            id: IdDeterministico("Meditation"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Preparação",
            nomeOriginal: "Preparation",
            descricao: "+25% dano, +20 esquiva e +2 velocidade na primeira rodada por 4 batalhas.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Bônus de Dano na 1ª Rodada", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de Esquiva na 1ª Rodada", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Bônus de Velocidade na 1ª Rodada", AlvoDeEfeito.Self, 2m, UnidadeDeEfeito.Pontos, duracao: 4),
            },
            id: IdDeterministico("Preparation"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Instrução Impiedosa",
            nomeOriginal: "Ruthless Instruction",
            descricao: "+7% crítico e +2 velocidade ao aliado por 4 batalhas, ao custo de +10 estresse (50% chance) e +10 estresse próprio.",
            custoDeDescanso: 2,
            alvo: AlvoDeAcampamento.UmAliado,
            efeitos: new[]
            {
                Efeito("Bônus de Crítico", AlvoDeEfeito.Aliado, 7m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de Velocidade", AlvoDeEfeito.Aliado, 2m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Aumento de Estresse", AlvoDeEfeito.Aliado, 10m, UnidadeDeEfeito.Pontos, chance: 50m),
                Efeito("Aumento de Estresse", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Ruthless Instruction"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "De Novo!",
            nomeOriginal: "Again!",
            descricao: "Renova os usos de habilidades de acampamento do aliado, ao custo de +15 estresse próprio.",
            custoDeDescanso: 1,
            alvo: AlvoDeAcampamento.UmAliado,
            efeitos: new[]
            {
                Efeito("Renova Usos de Acampamento", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Aumento de Estresse", AlvoDeEfeito.Self, 15m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Again!"));
    }
}
