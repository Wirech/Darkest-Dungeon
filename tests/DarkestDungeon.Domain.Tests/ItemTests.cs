using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Itens;
using FluentAssertions;

namespace DarkestDungeon.Domain.Tests;

public class ItemTests
{
    private static IEnumerable<NivelDeArma> CincoNiveisDeArma() =>
        Enumerable.Range(1, 5).Select(n => new NivelDeArma(n, 5 + n, 8 + n, 4 + n, 3 + n));

    private static IEnumerable<NivelDeArmadura> CincoNiveisDeArmadura() =>
        Enumerable.Range(1, 5).Select(n => new NivelDeArmadura(n, 10 * n, 5m + n));

    [Fact]
    public void Arma_valida_deve_ser_criada_com_cinco_niveis()
    {
        var arma = new Arma("Espada", "Sword", "Descrição", ClasseDeHeroi.Cruzado, CincoNiveisDeArma());

        arma.Niveis.Should().HaveCount(5);
        arma.ClasseElegivel.Should().Be(ClasseDeHeroi.Cruzado);
    }

    [Fact]
    public void Armadura_valida_deve_ser_criada_com_cinco_niveis()
    {
        var armadura = new Armadura("Placa", "Plate", "Descrição", ClasseDeHeroi.Cruzado, CincoNiveisDeArmadura());

        armadura.Niveis.Should().HaveCount(5);
        armadura.ClasseElegivel.Should().Be(ClasseDeHeroi.Cruzado);
    }

    [Fact]
    public void Acessorio_deve_aceitar_ConjuntoId_e_ClasseExclusiva_opcionais()
    {
        var acessorio = new Acessorio(
            "Amuleto",
            "Amulet",
            "Descrição",
            RaridadeDeAcessorio.Rara,
            new[] { new EfeitoDeAcessorio("Precisão", 5m, UnidadeDeEfeitoDeAcessorio.Percentual, SinalDeEfeito.Positivo) },
            classeExclusiva: ClasseDeHeroi.Ocultista,
            conjuntoId: Guid.NewGuid());

        acessorio.ClasseExclusiva.Should().Be(ClasseDeHeroi.Ocultista);
        acessorio.Raridade.Should().Be(RaridadeDeAcessorio.Rara);
        acessorio.ConjuntoId.Should().NotBeNull();
    }
}
