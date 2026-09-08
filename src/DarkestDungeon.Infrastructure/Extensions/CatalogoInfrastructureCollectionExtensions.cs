using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DarkestDungeon.Infrastructure.Extensions;

/// Módulo de DI da feature 003.
public static class CatalogoInfrastructureCollectionExtensions
{
    public static IServiceCollection AdicionarRepositoriosDoCatalogo(this IServiceCollection services)
    {
        services.AddScoped<IClasseRepository, ClasseRepository>();
        services.AddScoped<IHabilidadeRepository, HabilidadeRepository>();
        services.AddScoped<IItemRepository, ItemRepository>();
        services.AddScoped<IPersonagemRepository, PersonagemRepository>();
        services.AddScoped<IInimigoRepository, InimigoRepository>();
        services.AddScoped<IMapaDeCoberturaRepository, MapaDeCoberturaRepository>();
        return services;
    }
}
