using DarkestDungeon.Domain.Habilidades;
using FluentAssertions;

namespace DarkestDungeon.Domain.Tests;

public class EfeitoDeHabilidadeTests
{
    [Fact]
    public void ChanceBase_menor_que_zero_deve_ser_rejeitada()
    {
        var criar = () => new EfeitoDeHabilidade("Sangramento", AlvoDeEfeito.Inimigo, 3, UnidadeDeEfeito.Pontos, chanceBase: -1);

        criar.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ChanceBase_maior_que_cem_deve_ser_rejeitada()
    {
        var criar = () => new EfeitoDeHabilidade("Sangramento", AlvoDeEfeito.Inimigo, 3, UnidadeDeEfeito.Pontos, chanceBase: 101);

        criar.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void DuracaoEmRodadas_negativa_deve_ser_rejeitada()
    {
        var criar = () => new EfeitoDeHabilidade("Buff", AlvoDeEfeito.Self, 5, UnidadeDeEfeito.Percentual, 100, duracaoEmRodadas: -1);

        criar.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Efeito_valido_deve_manter_atributos()
    {
        var efeito = new EfeitoDeHabilidade("Sangramento", AlvoDeEfeito.Inimigo, 3, UnidadeDeEfeito.Pontos, 100, 3);

        efeito.NomeDoEfeito.Should().Be("Sangramento");
        efeito.Alvo.Should().Be(AlvoDeEfeito.Inimigo);
        efeito.Valor.Should().Be(3);
        efeito.ChanceBase.Should().Be(100);
        efeito.DuracaoEmRodadas.Should().Be(3);
    }
}
