using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Infrastructure.Data;
using DarkestDungeon.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DarkestDungeon.Api.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DarkestDungeonDb")
            ?? throw new InvalidOperationException("Connection string 'DarkestDungeonDb' não foi configurada.");

        return services.AddInfrastructureServices(options => options.UseSqlServer(connectionString));
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDbContext)
    {
        services.AddDbContext<DarkestDungeonDbContext>(configureDbContext);
        services.AddScoped<ISerRepository, SerRepository>();
        services.AddScoped(typeof(IRepositorioIdentificavel<>), typeof(RepositorioIdentificavel<>));

        return services;
    }
}