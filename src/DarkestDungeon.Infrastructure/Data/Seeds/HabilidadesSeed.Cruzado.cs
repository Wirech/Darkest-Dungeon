using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

public static partial class HabilidadesSeed
{
    private static IEnumerable<HabilidadeDeCombate> CruzadoCombate()
    {
        yield return new HabilidadeDeCombate(
            nomeExibicao: "Golpe Sagrado",
            nomeOriginal: "Smite",
            descricao: "Ataque corpo-a-corpo básico do Cruzado. Concede bônus de dano contra criaturas Não-Mortas.",
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
            id: IdDeterministico("Smite"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Acusação Zelosa",
            nomeOriginal: "Zealous Accusation",
            descricao: "Ataque à distância em área que atinge as duas primeiras posições inimigas simultaneamente.",
            posicoesValidas: new[] { 1, 2 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: true,
            modificadorDano: -40m,
            modificadorAcerto: 85m,
            modificadorCritico: -4m,
            efeitos: Array.Empty<EfeitoDeHabilidade>(),
            id: IdDeterministico("Zealous Accusation"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Golpe Atordoante",
            nomeOriginal: "Stunning Blow",
            descricao: "Golpe pesado que atordoa o alvo com 100% de chance base.",
            posicoesValidas: new[] { 1, 2 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: -50m,
            modificadorAcerto: 90m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Atordoamento", AlvoDeEfeito.Inimigo, 1m, UnidadeDeEfeito.Rodadas, duracao: 1),
            },
            id: IdDeterministico("Stunning Blow"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Baluarte da Fé",
            nomeOriginal: "Bulwark of Faith",
            descricao: "Aumenta a tocha em 24 pontos, concede +20% de proteção e marca o próprio Cruzado por uma batalha.",
            posicoesValidas: new[] { 1, 2 },
            posicoesQueAtinge: new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Tocha", AlvoDeEfeito.Self, 24m, UnidadeDeEfeito.Pontos),
                Efeito("Proteção", AlvoDeEfeito.Self, 20m, UnidadeDeEfeito.Percentual),
                Efeito("Marcação", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Rodadas),
            },
            limitePorUso: new LimitePorUso(EscopoDeLimite.Batalha, 1),
            id: IdDeterministico("Bulwark of Faith"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Cura de Batalha",
            nomeOriginal: "Battle Heal",
            descricao: "Cura direta em campo, restaura de 2 a 3 pontos de HP de um aliado.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Cura", AlvoDeEfeito.Aliado, 3m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Battle Heal"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Lança Sagrada",
            nomeOriginal: "Holy Lance",
            descricao: "Investida corpo-a-corpo lançada da retaguarda; o Cruzado avança uma posição e ganha bônus vs Não-Mortos.",
            posicoesValidas: new[] { 3, 4 },
            posicoesQueAtinge: new[] { 2, 3, 4 },
            alvoEmArea: false,
            modificadorDano: 0m,
            modificadorAcerto: 85m,
            modificadorCritico: 6.5m,
            efeitos: new[]
            {
                Efeito("Bônus de Dano vs Não-Mortos", AlvoDeEfeito.Self, 15m, UnidadeDeEfeito.Percentual),
                Efeito("Avanço", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Holy Lance"));

        yield return new HabilidadeDeCombate(
            nomeExibicao: "Grito Inspirador",
            nomeOriginal: "Inspiring Cry",
            descricao: "Grito de guerra que restaura um pouco de HP, reduz o estresse dos aliados e aumenta a tocha.",
            posicoesValidas: new[] { 1, 2, 3, 4 },
            posicoesQueAtinge: new[] { 1, 2, 3, 4 },
            alvoEmArea: true,
            modificadorDano: 0m,
            modificadorAcerto: 0m,
            modificadorCritico: 0m,
            efeitos: new[]
            {
                Efeito("Cura", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Estresse", AlvoDeEfeito.Aliado, 5m, UnidadeDeEfeito.Pontos),
                Efeito("Tocha", AlvoDeEfeito.Aliado, 5m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Inspiring Cry"));
    }

    private static IEnumerable<HabilidadeDeAcampamento> CruzadoAcampamento()
    {
        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Líder Inabalável",
            nomeOriginal: "Unshakable Leader",
            descricao: "Reduz o estresse recebido pelo Cruzado em 25% por 4 batalhas.",
            custoDeDescanso: 2,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Redução de Estresse Recebido", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Unshakable Leader"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Manter-se Firme",
            nomeOriginal: "Stand Tall",
            descricao: "Reduz o estresse do alvo em 15 pontos e remove debuffs de mortalidade.",
            custoDeDescanso: 3,
            alvo: AlvoDeAcampamento.UmAliado,
            efeitos: new[]
            {
                Efeito("Redução de Estresse", AlvoDeEfeito.Aliado, 15m, UnidadeDeEfeito.Pontos),
                Efeito("Remove Mortalidade", AlvoDeEfeito.Aliado, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Stand Tall"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Discurso Zeloso",
            nomeOriginal: "Zealous Speech",
            descricao: "Reduz 15 pontos de estresse de toda a party e -15% de estresse recebido por 4 batalhas.",
            custoDeDescanso: 5,
            alvo: AlvoDeAcampamento.PartyInteira,
            efeitos: new[]
            {
                Efeito("Redução de Estresse", AlvoDeEfeito.Aliado, 15m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Estresse Recebido", AlvoDeEfeito.Aliado, 15m, UnidadeDeEfeito.Percentual, duracao: 4),
            },
            id: IdDeterministico("Zealous Speech"));

        yield return new HabilidadeDeAcampamento(
            nomeExibicao: "Vigília Zelosa",
            nomeOriginal: "Zealous Vigil",
            descricao: "Reduz 25 pontos de estresse (35 se estiver afligido) e impede emboscadas noturnas.",
            custoDeDescanso: 4,
            alvo: AlvoDeAcampamento.Self,
            efeitos: new[]
            {
                Efeito("Redução de Estresse", AlvoDeEfeito.Self, 25m, UnidadeDeEfeito.Pontos),
                Efeito("Redução de Estresse Extra se Afligido", AlvoDeEfeito.Self, 15m, UnidadeDeEfeito.Pontos),
                Efeito("Impede Emboscada Noturna", AlvoDeEfeito.Self, 1m, UnidadeDeEfeito.Pontos),
            },
            id: IdDeterministico("Zealous Vigil"));
    }
}
