using DarkestDungeon.Domain.Seres;
using FluentAssertions;

namespace DarkestDungeon.Domain.Tests;

public class SerTests
{
    [Fact]
    public void CriarSer_ComDadosValidos_DeveNormalizarEPersistirAtributos()
    {
        var ser = CriarSer(nome: "  Cruzado  ", tipo: "  Herói  ");

        ser.Id.Should().NotBe(Guid.Empty);
        ser.Nome.Should().Be("Cruzado");
        ser.Tipo.Should().Be("Herói");
        ser.DanoBaseMinimo.Should().Be(7);
        ser.DanoBaseMaximo.Should().Be(13);
        ser.Resistencias.Atordoamento.Should().Be(40);
    }

    [Theory]
    [InlineData(101)]
    [InlineData(-1)]
    public void CriarSer_ComCriticoForaDaFaixa_DeveRejeitar(decimal critico)
    {
        var act = () => CriarSer(critico: critico);

        act.Should().Throw<ArgumentException>().WithMessage("Crítico deve estar entre 0 e 100.*");
    }

    [Fact]
    public void CriarSer_ComHpAtualMaiorQueMaximo_DeveRejeitar()
    {
        var act = () => CriarSer(hpMaximo: 10, hpAtual: 11);

        act.Should().Throw<ArgumentException>().WithMessage("HP Atual deve ser menor ou igual ao HP máximo.*");
    }

    [Fact]
    public void CriarSer_ComRangeDeDanoInvalido_DeveRejeitar()
    {
        var act = () => CriarSer(danoBaseMinimo: 14, danoBaseMaximo: 13);

        act.Should().Throw<ArgumentException>().WithMessage("Dano base mínimo deve ser menor ou igual ao Dano base máximo.*");
    }

    [Fact]
    public void CriarResistencias_ComValorForaDaFaixa_DeveRejeitar()
    {
        var act = () => new Resistencias(101, 0, 0, 0, 0);

        act.Should().Throw<ArgumentException>().WithMessage("Resistência de Atordoamento deve estar entre 0 e 100.*");
    }

    private static Ser CriarSer(
        string nome = "Cruzado",
        string tipo = "Herói",
        int hpMaximo = 33,
        int hpAtual = 33,
        decimal critico = 5,
        int danoBaseMinimo = 7,
        int danoBaseMaximo = 13)
    {
        return new Ser(
            nome,
            tipo,
            hpMaximo,
            hpAtual,
            velocidade: 1,
            critico,
            danoBaseMinimo,
            danoBaseMaximo,
            movimento: 2,
            bonusDeCritico: 0,
            tamanho: 1,
            acoesPorTurno: 1,
            esquiva: 5,
            precisao: 85,
            protecao: 0,
            nivel: 0,
            new Resistencias(40, 30, 20, 25, 35));
    }
}