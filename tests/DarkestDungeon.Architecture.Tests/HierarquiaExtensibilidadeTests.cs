using DarkestDungeon.Domain.Habilidades;
using DarkestDungeon.Domain.Itens;
using DarkestDungeon.Domain.Seres;
using FluentAssertions;
using NetArchTest.Rules;

namespace DarkestDungeon.Architecture.Tests;

public class HierarquiaExtensibilidadeTests
{
    [Fact]
    public void Personagem_e_Inimigo_devem_herdar_de_Ser()
    {
        typeof(Personagem).BaseType.Should().Be<Ser>();
        typeof(Inimigo).BaseType.Should().Be<Ser>();
    }

    [Fact]
    public void HabilidadeDeHeroi_e_HabilidadeDeInimigo_devem_herdar_de_Habilidade()
    {
        typeof(HabilidadeDeHeroi).BaseType.Should().Be<Habilidade>();
        typeof(HabilidadeDeInimigo).BaseType.Should().Be<Habilidade>();
    }

    [Fact]
    public void HabilidadeDeCombate_e_HabilidadeDeAcampamento_devem_herdar_de_HabilidadeDeHeroi()
    {
        typeof(HabilidadeDeCombate).BaseType.Should().Be<HabilidadeDeHeroi>();
        typeof(HabilidadeDeAcampamento).BaseType.Should().Be<HabilidadeDeHeroi>();
    }

    [Fact]
    public void Arma_Armadura_Acessorio_devem_herdar_de_Item()
    {
        typeof(Arma).BaseType.Should().Be<Item>();
        typeof(Armadura).BaseType.Should().Be<Item>();
        typeof(Acessorio).BaseType.Should().Be<Item>();
    }

    [Fact]
    public void Habilidade_e_Item_devem_ser_abstratas()
    {
        typeof(Habilidade).IsAbstract.Should().BeTrue();
        typeof(Item).IsAbstract.Should().BeTrue();
    }

    [Fact]
    public void Domain_nao_referencia_EntityFrameworkCore()
    {
        var result = Types.InAssembly(typeof(Ser).Assembly)
            .Should()
            .NotHaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_nao_referencia_EntityFrameworkCore()
    {
        var result = Types.InAssembly(typeof(DarkestDungeon.Application.Abstractions.ISerService).Assembly)
            .Should()
            .NotHaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
