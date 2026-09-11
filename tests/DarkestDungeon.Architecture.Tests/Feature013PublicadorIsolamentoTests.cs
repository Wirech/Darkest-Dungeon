using DarkestDungeon.Application.Itens;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Application.Publicacao;
using DarkestDungeon.Infrastructure.Data.Seeds;
using FluentAssertions;
using NetArchTest.Rules;

namespace DarkestDungeon.Architecture.Tests;

public sealed class Feature013PublicadorIsolamentoTests
{
    [Fact]
    public void Fluxo_013_nao_referencia_IPublicadorAtomicoService()
    {
        var application = Types.InAssembly(typeof(FichaEfetivaDePersonagem).Assembly)
            .That()
            .HaveName(
                nameof(FichaEfetivaDePersonagem),
                nameof(MapaDeEfeitoDeTrinket),
                nameof(PersonagemService),
                nameof(ItemService))
            .ShouldNot()
            .HaveDependencyOn(typeof(IPublicadorAtomicoService).FullName)
            .GetResult();

        application.IsSuccessful.Should().BeTrue();

        var seed = Types.InAssembly(typeof(AcessoriosOficiaisSeed).Assembly)
            .That()
            .HaveName(nameof(AcessoriosOficiaisSeed))
            .ShouldNot()
            .HaveDependencyOn(typeof(IPublicadorAtomicoService).FullName)
            .GetResult();

        seed.IsSuccessful.Should().BeTrue();
    }
}
