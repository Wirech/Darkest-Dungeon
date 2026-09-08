using DarkestDungeon.Domain.Seres;
using FluentAssertions;

namespace DarkestDungeon.Architecture.Tests;

public class SerExtensibilityTests
{
    [Fact]
    public void TipoDerivado_DeveReutilizarValidacoesComunsDeSer()
    {
        var criarDerivadoInvalido = () => new SerDeTeste(
            "Monstro de Teste",
            "Monstro",
            hpMaximo: 10,
            hpAtual: 12,
            velocidade: 1,
            critico: 5,
            danoBaseMinimo: 2,
            danoBaseMaximo: 4,
            movimento: 1,
            bonusDeCritico: 0,
            tamanho: 1,
            acoesPorTurno: 1,
            esquiva: 0,
            precisao: 80,
            protecao: 0,
            nivel: 1,
            new Resistencias(10, 10, 10, 10, 10));

        criarDerivadoInvalido.Should().Throw<ArgumentException>()
            .WithMessage("HP Atual deve ser menor ou igual ao HP máximo.*");
    }

    private sealed class SerDeTeste : Ser
    {
        public SerDeTeste(
            string nome,
            string tipo,
            int hpMaximo,
            int hpAtual,
            int velocidade,
            decimal critico,
            int danoBaseMinimo,
            int danoBaseMaximo,
            int movimento,
            decimal bonusDeCritico,
            int tamanho,
            int acoesPorTurno,
            decimal esquiva,
            decimal precisao,
            decimal protecao,
            int nivel,
            Resistencias resistencias)
            : base(nome, tipo, hpMaximo, hpAtual, velocidade, critico, danoBaseMinimo, danoBaseMaximo, movimento, bonusDeCritico, tamanho, acoesPorTurno, esquiva, precisao, protecao, nivel, resistencias)
        {
        }
    }
}