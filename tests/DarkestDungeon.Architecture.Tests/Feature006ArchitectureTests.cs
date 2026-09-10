using DarkestDungeon.Application.Midias;
using DarkestDungeon.Application.Publicacao;
using DarkestDungeon.Domain.Itens;
using DarkestDungeon.Infrastructure.Midias;
using FluentAssertions;
using NetArchTest.Rules;

namespace DarkestDungeon.Architecture.Tests;

public sealed class Feature006ArchitectureTests
{
    [Fact]
    public void Application_nao_referencia_EntityFrameworkCore()
    {
        var nomes = typeof(IPublicadorDeVinculosDeMidia).Assembly
            .GetReferencedAssemblies()
            .Select(a => a.Name);

        nomes.Should().NotContain("Microsoft.EntityFrameworkCore");
        nomes.Should().NotContain("Microsoft.EntityFrameworkCore.SqlServer");
    }

    [Fact]
    public void PublicadorDeVinculos_nao_depende_do_publicador_005()
    {
        var result = Types.InAssembly(typeof(PublicadorDeVinculosDeMidia).Assembly)
            .That()
            .HaveName(nameof(PublicadorDeVinculosDeMidia))
            .ShouldNot()
            .HaveDependencyOn(typeof(IPublicadorAtomicoService).FullName)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();

        var detector = Types.InAssembly(typeof(PublicadorDeVinculosDeMidia).Assembly)
            .That()
            .HaveName(nameof(PublicadorDeVinculosDeMidia))
            .ShouldNot()
            .HaveDependencyOn(typeof(IDetectorDeSessoesAtivas).FullName)
            .GetResult();

        detector.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void CoberturaDeMidiasService_vive_na_Application_sem_depender_de_Infrastructure()
    {
        typeof(CoberturaDeMidiasService).Assembly.Should().BeSameAs(typeof(ICoberturaDeMidiasService).Assembly);

        var result = Types.InAssembly(typeof(CoberturaDeMidiasService).Assembly)
            .That()
            .HaveName(nameof(CoberturaDeMidiasService))
            .ShouldNot()
            .HaveDependencyOn("DarkestDungeon.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Domain_Itens_nao_referencia_assemblies_de_IO_de_instalacao()
    {
        var nomes = typeof(MidiaDeItem).Assembly.GetReferencedAssemblies().Select(a => a.Name);
        nomes.Should().NotContain("System.IO.FileSystem");
        nomes.Should().NotContain("DarkestDungeon.Infrastructure");
        nomes.Should().NotContain("DarkestDungeon.Api");
    }
}
