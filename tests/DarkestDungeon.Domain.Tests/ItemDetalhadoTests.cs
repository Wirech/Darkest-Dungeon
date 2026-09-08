using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Itens;
using FluentAssertions;

namespace DarkestDungeon.Domain.Tests;

public class ArmaTests
{
    private static IEnumerable<NivelDeArma> CincoNiveisValidos() =>
        Enumerable.Range(1, 5).Select(n => new NivelDeArma(n, 5 + n, 8 + n, 4m + n, 3 + n));

    [Fact]
    public void Arma_com_menos_de_cinco_niveis_deve_ser_rejeitada()
    {
        var criar = () => new Arma("X", "X", "Descrição", ClasseDeHeroi.Cruzado, CincoNiveisValidos().Take(4));
        criar.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Arma_com_mais_de_cinco_niveis_deve_ser_rejeitada()
    {
        var niveis = CincoNiveisValidos().Concat(new[] { new NivelDeArma(1, 5, 8, 4, 3) });
        var criar = () => new Arma("X", "X", "Descrição", ClasseDeHeroi.Cruzado, niveis);
        criar.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Arma_com_niveis_duplicados_deve_ser_rejeitada()
    {
        var niveis = Enumerable.Range(1, 5).Select(_ => new NivelDeArma(1, 5, 8, 4, 3));
        var criar = () => new Arma("X", "X", "Descrição", ClasseDeHeroi.Cruzado, niveis);
        criar.Should().Throw<ArgumentException>();
    }
}

public class ArmaduraTests
{
    private static IEnumerable<NivelDeArmadura> CincoNiveisValidos() =>
        Enumerable.Range(1, 5).Select(n => new NivelDeArmadura(n, 10 * n, 5m + n));

    [Fact]
    public void Armadura_valida_deve_ser_criada()
    {
        var armadura = new Armadura("Placa", "Plate", "Descrição", ClasseDeHeroi.Veterano, CincoNiveisValidos());
        armadura.Niveis.Should().HaveCount(5);
    }

    [Fact]
    public void Armadura_sem_cinco_niveis_deve_ser_rejeitada()
    {
        var criar = () => new Armadura("X", "X", "Descrição", ClasseDeHeroi.Veterano, CincoNiveisValidos().Take(3));
        criar.Should().Throw<ArgumentException>();
    }
}

public class AcessorioTests
{
    [Fact]
    public void Acessorio_com_todas_as_sete_raridades_deve_ser_criado()
    {
        foreach (var raridade in Enum.GetValues<RaridadeDeAcessorio>())
        {
            var acessorio = new Acessorio("A", "A", "D", raridade, Array.Empty<EfeitoDeAcessorio>());
            acessorio.Raridade.Should().Be(raridade);
        }
    }

    [Fact]
    public void Acessorio_deve_aceitar_ClasseExclusiva_e_ConjuntoId_opcionais()
    {
        var acessorio = new Acessorio("A", "A", "D", RaridadeDeAcessorio.Rara,
            new[] { new EfeitoDeAcessorio("Bônus", 5m, UnidadeDeEfeitoDeAcessorio.Percentual, SinalDeEfeito.Positivo) },
            classeExclusiva: ClasseDeHeroi.Ocultista,
            conjuntoId: Guid.NewGuid());

        acessorio.ClasseExclusiva.Should().Be(ClasseDeHeroi.Ocultista);
        acessorio.ConjuntoId.Should().NotBeNull();
    }
}
