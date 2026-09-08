using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Application.Classes;
using DarkestDungeon.Application.Cobertura;
using DarkestDungeon.Application.Habilidades;
using DarkestDungeon.Application.Itens;
using DarkestDungeon.Application.Personagens;
using Microsoft.Extensions.DependencyInjection;

namespace DarkestDungeon.Application.Extensions;

/// Módulo de DI da feature 003.
public static class CatalogoServiceCollectionExtensions
{
    public static IServiceCollection AdicionarServicosDoCatalogo(this IServiceCollection services)
    {
        services.AddScoped<IClasseService, ClasseService>();
        services.AddScoped<IHabilidadeService, HabilidadeService>();
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<IPersonagemService, PersonagemService>();
        services.AddScoped<IInimigoService, InimigoService>();
        services.AddScoped<IMapaDeCoberturaService, MapaDeCoberturaService>();
        return services;
    }
}
