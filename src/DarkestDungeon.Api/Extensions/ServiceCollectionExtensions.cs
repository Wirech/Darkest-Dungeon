namespace DarkestDungeon.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDarkestDungeon(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationServices();
        services.AddInfrastructureServices(configuration);

        return services;
    }
}