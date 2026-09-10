using DarkestDungeon.Domain.Personagens;
using FluentAssertions;
using Xunit;

namespace DarkestDungeon.Domain.Tests.Feature005;

/// Testes da Feature 005 — value objects `NivelDeResolucao` e `TabelaDeExperiencia`.
public sealed class NivelDeResolucaoTests
{
    [Theory]
    [InlineData(0, "Curioso", "Seeker", 0)]
    [InlineData(1, "Aprendiz", "Apprentice", 10)]
    [InlineData(2, "Aventureiro", "Adventurer", 20)]
    [InlineData(3, "Veterano", "Veteran", 30)]
    [InlineData(4, "Mestre", "Master", 40)]
    [InlineData(5, "Campeão", "Champion", 50)]
    [InlineData(6, "Lenda", "Legend", 60)]
    public void De_valor_valido_retorna_nomes_PtBr_e_originais_e_bonus_correto(int valor, string nomePtBr, string nomeIngles, int bonus)
    {
        var nivel = NivelDeResolucao.De(valor);

        nivel.Valor.Should().Be(valor);
        nivel.Nome.Should().Be(nomePtBr);
        nivel.NomeOriginal.Should().Be(nomeIngles);
        nivel.BonusResistenciaPercentual.Should().Be(bonus);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(7)]
    public void De_valor_fora_de_0_a_6_lanca(int valor)
    {
        var acao = () => NivelDeResolucao.De(valor);
        acao.Should().Throw<ArgumentOutOfRangeException>();
    }
}

public sealed class TabelaDeExperienciaTests
{
    [Fact]
    public void Limiares_devem_ser_2_8_14_24_36_48_no_modo_unico()
    {
        // Q5 da 3ª clarify — apenas modo mais difícil (equivalente a Darkest/Stygian oficial)
        TabelaDeExperiencia.Limiares.Should().Equal(new[] { 2, 8, 14, 24, 36, 48 });
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(2, 1)]
    [InlineData(7, 1)]
    [InlineData(8, 2)]
    [InlineData(23, 3)]
    [InlineData(24, 4)]
    [InlineData(48, 6)]
    [InlineData(999, 6)]
    public void Resolver_retorna_nivel_correto_para_XP_dado(int xp, int nivelEsperado)
    {
        var nivel = TabelaDeExperiencia.Resolver(xp);
        nivel.Valor.Should().Be(nivelEsperado);
    }

    [Fact]
    public void Resolver_XP_negativo_lanca()
    {
        var acao = () => TabelaDeExperiencia.Resolver(-1);
        acao.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void XpFaltando_para_proximo_nivel_calcula_delta_correto()
    {
        TabelaDeExperiencia.XpFaltandoParaProximoNivel(0).Should().Be(2);
        TabelaDeExperiencia.XpFaltandoParaProximoNivel(2).Should().Be(6); // 8 - 2
        TabelaDeExperiencia.XpFaltandoParaProximoNivel(48).Should().Be(0); // já nível 6
    }
}
