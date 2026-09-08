using DarkestDungeon.Domain.Habilidades;
using FluentAssertions;

namespace DarkestDungeon.Domain.Tests;

public class HabilidadeTests
{
    private static IEnumerable<EfeitoDeHabilidade> EfeitosDeExemplo()
    {
        yield return new EfeitoDeHabilidade("Sangramento", AlvoDeEfeito.Inimigo, 3, UnidadeDeEfeito.Pontos, 100, 3);
    }

    [Fact]
    public void HabilidadeDeCombate_deve_ser_criada_com_atributos_validos()
    {
        var hab = new HabilidadeDeCombate(
            "Corte de Sabre",
            "Sabre Slash",
            "Golpe rápido em posição avançada.",
            new[] { 1, 2 },
            new[] { 1, 2 },
            alvoEmArea: false,
            modificadorDano: 10,
            modificadorAcerto: 5,
            modificadorCritico: 3,
            EfeitosDeExemplo());

        hab.NomeExibicao.Should().Be("Corte de Sabre");
        hab.NomeOriginal.Should().Be("Sabre Slash");
        hab.PosicoesValidas.Should().BeEquivalentTo(new[] { 1, 2 });
        hab.Efeitos.Should().HaveCount(1);
    }

    [Fact]
    public void HabilidadeDeCombate_deve_rejeitar_posicao_fora_de_1a4()
    {
        var criar = () => new HabilidadeDeCombate(
            "Golpe Invalido",
            "Invalid Strike",
            "Descrição",
            new[] { 5 },
            new[] { 1 },
            false,
            0,
            0,
            0,
            Array.Empty<EfeitoDeHabilidade>());

        criar.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void HabilidadeDeAcampamento_deve_validar_custo_de_descanso()
    {
        var criar = () => new HabilidadeDeAcampamento(
            "Descanso Ruim",
            "Bad Rest",
            "Descrição",
            custoDeDescanso: 25,
            AlvoDeAcampamento.Self,
            Array.Empty<EfeitoDeHabilidade>());

        criar.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void HabilidadeDeInimigo_deve_aceitar_condicao_e_chance_opcionais()
    {
        var hab = new HabilidadeDeInimigo(
            "Marca Sombria",
            "Weakening Curse",
            "Descrição",
            EfeitosDeExemplo(),
            condicaoDeAparecer: "Turno 1",
            chanceDeExecucao: 40);

        hab.CondicaoDeAparecer.Should().Be("Turno 1");
        hab.ChanceDeExecucao.Should().Be(40);
    }

    [Fact]
    public void HabilidadeDeHeroi_deve_rejeitar_nome_vazio()
    {
        var criar = () => new HabilidadeDeAcampamento(
            "  ",
            "Nome",
            "Descrição",
            0,
            AlvoDeAcampamento.Self,
            Array.Empty<EfeitoDeHabilidade>());

        criar.Should().Throw<ArgumentException>();
    }
}
