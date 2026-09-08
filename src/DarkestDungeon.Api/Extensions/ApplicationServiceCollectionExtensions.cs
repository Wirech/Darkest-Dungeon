using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Application.Seres;

namespace DarkestDungeon.Api.Extensions;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ISerService, SerService>();
        services.AddScoped<IIdentificavelService<SerDto>>(provider => provider.GetRequiredService<ISerService>());

        return services;
    }
}
