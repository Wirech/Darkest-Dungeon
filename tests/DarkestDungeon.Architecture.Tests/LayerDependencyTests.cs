using System.Reflection;
using DarkestDungeon.Application.Seres;
using DarkestDungeon.Domain.Seres;
using FluentAssertions;
using NetArchTest.Rules;

namespace DarkestDungeon.Architecture.Tests;

public class LayerDependencyTests
{
    [Fact]
    public void Domain_NaoDeveDependerDeApiApplicationOuInfrastructure()
    {
        var result = Types.InAssembly(typeof(Ser).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny("DarkestDungeon.Api", "DarkestDungeon.Application", "DarkestDungeon.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_NaoDeveDependerDeApiOuInfrastructure()
    {
        var result = Types.InAssembly(typeof(SerDto).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny("DarkestDungeon.Api", "DarkestDungeon.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Domain_ProjectReferences_NaoDevemApontarParaCamadasExternas()
    {
        var referencedAssemblies = typeof(Ser).Assembly.GetReferencedAssemblies().Select(assembly => assembly.Name);

        referencedAssemblies.Should().NotContain(new[]
        {
            "DarkestDungeon.Api",
            "DarkestDungeon.Application",
            "DarkestDungeon.Infrastructure"
        });
    }
}