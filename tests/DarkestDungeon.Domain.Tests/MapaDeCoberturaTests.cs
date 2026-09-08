using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Cobertura;
using FluentAssertions;

namespace DarkestDungeon.Domain.Tests;

public class MapaDeCoberturaTests
{
    [Fact]
    public void Entrada_valida_deve_ser_criada()
    {
        var entrada = new EntradaDoMapaDeCobertura(
            ClasseDeHeroi.Cruzado,
            CategoriaDeCobertura.ResistenciaBase,
            "Atordoamento",
            EstadoDeAtributo.Pendente);

        entrada.Classe.Should().Be(ClasseDeHeroi.Cruzado);
        entrada.Estado.Should().Be(EstadoDeAtributo.Pendente);
    }

    [Fact]
    public void ChaveDoAtributo_vazia_deve_ser_rejeitada()
    {
        var criar = () => new EntradaDoMapaDeCobertura(
            ClasseDeHeroi.Cruzado, CategoriaDeCobertura.ResistenciaBase, "  ", EstadoDeAtributo.Pendente);
        criar.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AtualizarEstado_deve_permitir_transicao()
    {
        var entrada = new EntradaDoMapaDeCobertura(
            ClasseDeHeroi.Cruzado, CategoriaDeCobertura.HabilidadeCombate, "Golpe", EstadoDeAtributo.Pendente);

        entrada.AtualizarEstado(EstadoDeAtributo.Coletado, "coletado da wiki");

        entrada.Estado.Should().Be(EstadoDeAtributo.Coletado);
        entrada.Notas.Should().Be("coletado da wiki");
    }
}
