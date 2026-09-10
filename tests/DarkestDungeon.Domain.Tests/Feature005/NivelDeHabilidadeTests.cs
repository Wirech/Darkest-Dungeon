using DarkestDungeon.Domain.Habilidades;
using FluentAssertions;
using Xunit;

namespace DarkestDungeon.Domain.Tests.Feature005;

public sealed class NivelDeHabilidadeTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void Construtor_rejeita_NumeroDoNivel_fora_de_1_a_5(int numero)
    {
        var acao = () => new NivelDeHabilidade(numero, 0, 0, 0);
        acao.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public void Construtor_aceita_NumeroDoNivel_valido(int numero)
    {
        var nivel = new NivelDeHabilidade(numero, 10, 5, 3);
        nivel.NumeroDoNivel.Should().Be(numero);
        nivel.ValoresDeEfeito.Should().BeEmpty();
    }

    [Fact]
    public void ValoresDeEfeito_sao_preservados_quando_fornecidos()
    {
        var efeito = new ValorDeEfeito("Sangramento", 5, 100);
        var nivel = new NivelDeHabilidade(1, 0, 0, 0, new[] { efeito });
        nivel.ValoresDeEfeito.Should().ContainSingle().Which.TipoDoEfeito.Should().Be("Sangramento");
    }
}
