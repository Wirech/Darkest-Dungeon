using DarkestDungeon.Application.Midias;
using FluentAssertions;

namespace DarkestDungeon.Architecture.Tests;

public sealed class Feature010ArchitectureTests
{
    [Fact]
    public void Application_nao_referencia_System_IO_nos_contratos_do_card()
    {
        var assembly = typeof(IResolvedorDeMidiasDoCard).Assembly;
        var nomes = assembly.GetReferencedAssemblies().Select(a => a.Name);

        nomes.Should().NotContain("System.IO.FileSystem");
        nomes.Should().NotContain("DarkestDungeon.Infrastructure");

        var tipos = new[]
        {
            typeof(IResolvedorDeMidiasDoCard),
            typeof(SlotDeMidiaDoCardDto),
            typeof(MidiasDoPersonagemDto),
            typeof(ConjuntoDeCorpoDto),
            typeof(VersaoDoCorpoDto),
            typeof(OpcoesDeAcervoDoCard),
        };

        foreach (var tipo in tipos)
        {
            tipo.Assembly.Should().BeSameAs(assembly);
            tipo.GetMethods().SelectMany(m => m.GetParameters())
                .Select(p => p.ParameterType.Namespace)
                .Where(ns => ns is not null)
                .Should()
                .NotContain(ns => ns!.StartsWith("System.IO", StringComparison.Ordinal));
        }
    }
}
