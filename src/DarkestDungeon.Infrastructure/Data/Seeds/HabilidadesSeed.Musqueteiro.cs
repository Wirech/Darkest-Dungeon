using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

// O Musqueteiro é reskin funcional do Besteiro (mesma tabela de valores oficial).
// Compartilha `Field Dressing`, `Marching Plan` e `Triage` como registros globais criados em BesteiroAcampamento();
// diverge apenas em `Clean Musket` (equivalente ao Restring Crossbow).
public static partial class HabilidadesSeed
{
    private static IEnumerable<HabilidadeDeCombate> MusqueteiroCombate()
    {
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Tiro Certeiro",
            nomeOriginal: "Aimed Shot",
            descricao: "Tiro preciso à distância; +50% de dano e +9% de crítico contra alvos marcados.",
            posicoesValidas: new[] { 3, 4 },
            posicoesQueAtinge: new[] { 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 95m,
            modificadorCritico: 5m,
            efeitos: new[]
            {
                Efeito("Bônus de Dano vs Marcado", AlvoDeEfeito.Self, 50m, UnidadeDeEfeito.Percentual),
                Efeito("Bônus de Crítico vs Marcado", AlvoDeEfeito.Self, 9m, UnidadeDeEfeito.Percentual),
            },
            id: IdDeterministico("Aimed Shot"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Cortina de Fumaça",
            nomeOriginal: "Smokescreen",
            descricao: "Rajada AOE que reduz precisão e crítico dos alvos por 2 rodadas.",
            posicoesValidas: new[] { 3, 4 },
            posicoesQueAtinge: new[] { 3, 4 },
            alvoEmArea: true,
            modificadorDano: -80m,
            modificadorAcerto: 95m,
            modificadorCritico: -10m,
            efeitos: new[]
            {
                Efeito("Redução de Precisão", AlvoDeEfeito.Inimigo, 15m, UnidadeDeEfeito.Pontos, duracao: 2),
                Efeito("Redução de Crítico", AlvoDeEfeito.Inimigo, 15m, UnidadeDeEfeito.Percentual, duracao: 2),
            },
            id: IdDeterministico("Smokescreen"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Anunciar o Alvo",
            nomeOriginal: "Call the Shot",
            descricao: "Marca o alvo por 3 rodadas e reduz sua esquiva em 20 por 2 rodadas.",
            posicoesValidas: new[] { 3, 4 },
            posicoesQueAtinge: new[] { 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: -100m,
            modificadorAcerto: 100m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Marcação", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Rodadas, duracao: 3),
                Efeito("Redução de Esquiva", AlvoDeEfeito.Inimigo, 20m, UnidadeDeEfeito.Pontos, duracao: 2),
            },
            id: IdDeterministico("Call the Shot"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Chumbinho",
            nomeOriginal: "Buckshot",
            descricao: "Rajada em área que empurra os alvos das duas primeiras posições para trás com 75% de chance.",
            posicoesValidas: new[] { 3, 4 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: true,
            modificadorDano: -50m,
            modificadorAcerto: 95m,
            modificadorCritico: 2m,
            efeitos: new[]
            {
                Efeito("Empurrão", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos, chance: 75m),
            },
            id: IdDeterministico("Buckshot"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Pistola",
            nomeOriginal: "Sidearm",
            descricao: "Tiro rápido em alvo aleatório; concede +3 velocidade à Musqueteira por 4 rodadas.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: -10m,
            modificadorAcerto: 75m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Alvo Aleatório", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Velocidade", AlvoDeEfeito.Self, 3m, UnidadeDeEfeito.Pontos, duracao: 4),
            },
            id: IdDeterministico("Sidearm"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Reparo Rápido",
            nomeOriginal: "Patch Up",
            descricao: "Cura direta de 2 a 3 pontos de HP e concede +20% cura recebida por 3 rodadas.",
            posicoesValidas: new[] { 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Cura", AlvoDeEfeito.Aliado, 3m, UnidadeDeEfeito.Pontos),
                Efeito("Bônus de Cura Recebida", AlvoDeEfeito.Aliado, 20m, UnidadeDeEfeito.Percentual, duracao: 3),
            },
            id: IdDeterministico("Patch Up"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Tiro de Skeet",
            nomeOriginal: "Skeet Shot",
            descricao: "Tiro sinalizador que remove furtividade, aumenta tocha e limpa atordoamento/marcação dos aliados; -1 estresse com 60% de chance.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: true,
            modificadorDano: -100m,
            modificadorAcerto: 95m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Remove Furtividade", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Tocha", AlvoDeEfeito.Self, 3m, UnidadeDeEfeito.Pontos),
                Efeito("Limpa Atordoamento", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Limpa Marcação", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Estresse", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Pontos, chance: 60m),
            },
            id: IdDeterministico("Skeet Shot"));
    }

    /// Apenas Clean Musket — as demais 3 acampamento são compartilhadas com o Besteiro.
    private static IEnumerable<HabilidadeDeAcampamento> MusqueteiroAcampamento()
    {
        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Limpar Mosquete",
            nomeOriginal: "Clean Musket",
            descricao: "Higieniza o mosquete: +10 precisão, +20% dano e +8% crítico em habilidades ranged por 4 batalhas ao custo de -2 velocidade.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Bônus de Precisão Ranged", AlvoDeEfeito.Self, 10m, UnidadeDeEfeito.Pontos, duracao: 4),
                Efeito("Bônus de Dano Ranged", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Bônus de Crítico Ranged", AlvoDeEfeito.Self, 8m, UnidadeDeEfeito.Percentual, duracao: 4),
                Efeito("Redução de Velocidade", AlvoDeEfeito.Self, 2m, UnidadeDeEfeito.Pontos, duracao: 4),
            },
            id: IdDeterministico("Clean Musket"));
    }
}
