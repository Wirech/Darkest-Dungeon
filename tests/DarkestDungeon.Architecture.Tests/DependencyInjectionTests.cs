using DarkestDungeon.Api.Extensions;
using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Application.Extensions;
using DarkestDungeon.Application.Seres;
using DarkestDungeon.Domain.Seres;
using DarkestDungeon.Infrastructure.Data;
using DarkestDungeon.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;

namespace DarkestDungeon.Architecture.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void Container_DeveResolverServicosPrincipais()
    {
        using var scope = ConstruirContainer();

        scope.ServiceProvider.GetRequiredService<IIdentificavelService<SerDto>>().Should().NotBeNull();
        scope.ServiceProvider.GetRequiredService<ISerService>().Should().NotBeNull();
        scope.ServiceProvider.GetRequiredService<IRepositorioIdentificavel<Ser>>().Should().NotBeNull();
        scope.ServiceProvider.GetRequiredService<ISerRepository>().Should().NotBeNull();
        scope.ServiceProvider.GetRequiredService<DarkestDungeonDbContext>().Should().NotBeNull();
    }

    [Theory]
    [InlineData(typeof(IClasseService))]
    [InlineData(typeof(IHabilidadeService))]
    [InlineData(typeof(IItemService))]
    [InlineData(typeof(IPersonagemService))]
    [InlineData(typeof(IInimigoService))]
    [InlineData(typeof(IMapaDeCoberturaService))]
    public void Servicos_do_Catalogo_devem_ser_registrados(Type contrato)
    {
        using var scope = ConstruirContainer();
        scope.ServiceProvider.GetService(contrato).Should().NotBeNull();
    }

    [Theory]
    [InlineData(typeof(IClasseRepository))]
    [InlineData(typeof(IHabilidadeRepository))]
    [InlineData(typeof(IItemRepository))]
    [InlineData(typeof(IPersonagemRepository))]
    [InlineData(typeof(IInimigoRepository))]
    [InlineData(typeof(IMapaDeCoberturaRepository))]
    public void Repositorios_do_Catalogo_devem_ser_registrados(Type contrato)
    {
        using var scope = ConstruirContainer();
        scope.ServiceProvider.GetService(contrato).Should().NotBeNull();
    }

    private static IServiceScope ConstruirContainer()
    {
        var services = new ServiceCollection();
        services.AddApplicationServices();
        services.AdicionarServicosDoCatalogo();
        services.AddInfrastructureServices(options => options.UseInMemoryDatabase("architecture-di-" + Guid.NewGuid()));
        services.AdicionarRepositoriosDoCatalogo();

        var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        });
        return provider.CreateScope();
    }
}