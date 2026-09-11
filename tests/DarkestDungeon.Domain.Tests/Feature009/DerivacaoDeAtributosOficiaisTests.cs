using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Itens;
using FluentAssertions;

namespace DarkestDungeon.Domain.Tests.Feature009;

public sealed class DerivacaoDeAtributosOficiaisTests
{
    [Fact]
    public void Derivar_deve_usar_armadura_arma_acc_prot_zero_e_bonus_de_resolucao()
    {
        var classe = new Classe(
            ClasseDeHeroi.Cruzado,
            "Cruzado",
            "Crusader",
            new ResistenciasDeClasse(40, 30, 30, 30, 40, 30, 67, 10),
            passosAFrente: 1,
            passosAtras: 1);
        var arma = new NivelDeArma(2, 7, 14, 4m, 1);
        var armadura = new NivelDeArmadura(3, 47, 15m);

        var derivado = DerivacaoDeAtributosOficiais.Derivar(classe, arma, armadura, 3);

        derivado.HpMaximo.Should().Be(47);
        derivado.HpAtual.Should().Be(47);
        derivado.Esquiva.Should().Be(15m);
        derivado.DanoBaseMinimo.Should().Be(7);
        derivado.DanoBaseMaximo.Should().Be(14);
        derivado.Critico.Should().Be(4m);
        derivado.Velocidade.Should().Be(1);
        derivado.Precisao.Should().Be(0);
        derivado.Protecao.Should().Be(0);
        derivado.Stress.Should().Be(0);
        derivado.ChanceDeVirtude.Should().Be(25);
        derivado.BonusDeCritico.Should().Be(0);
        derivado.PassosAFrente.Should().Be(1);
        derivado.PassosAtras.Should().Be(1);
        derivado.Resistencias.Atordoamento.Should().Be(70);
        derivado.Resistencias.Sangramento.Should().Be(60);
        derivado.ResistenciasExtras.Doenca.Should().Be(60);
        derivado.ResistenciasExtras.GolpeMortal.Should().Be(67);
        derivado.ResistenciasExtras.Armadilha.Should().Be(10);
    }

    [Fact]
    public void Bonus_de_resolucao_deve_respeitar_teto_100()
    {
        DerivacaoDeAtributosOficiais.AplicarBonusDeResolucao(95, 6).Should().Be(100);
        DerivacaoDeAtributosOficiais.AplicarBonusDeResolucao(40, 0).Should().Be(40);
    }
}
