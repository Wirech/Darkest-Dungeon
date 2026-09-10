using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Application.Auditoria;
using DarkestDungeon.Application.Midias;
using DarkestDungeon.Application.Publicacao;
using DarkestDungeon.Infrastructure.Auditoria;
using DarkestDungeon.Infrastructure.Data;
using DarkestDungeon.Infrastructure.Midias;
using DarkestDungeon.Infrastructure.Publicacao;
using DarkestDungeon.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DarkestDungeon.Api.Extensions;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DarkestDungeonDb")
            ?? throw new InvalidOperationException("Connection string 'DarkestDungeonDb' não foi configurada.");

        return services.AddInfrastructureServices(options => options.UseSqlServer(connectionString), configuration);
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDbContext) =>
        services.AddInfrastructureServices(configureDbContext, configuration: null);

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureDbContext,
        IConfiguration? configuration)
    {
        services.AddDbContext<DarkestDungeonDbContext>(configureDbContext);
        services.AddDbContextFactory<DarkestDungeonDbContext>(configureDbContext, ServiceLifetime.Scoped);
        services.AddScoped<ISerRepository, SerRepository>();
        services.AddScoped(typeof(IRepositorioIdentificavel<>), typeof(RepositorioIdentificavel<>));

        // Feature 005 - Auditoria providers
        services.AddScoped<IProvedorDeSnapshotsWiki, ProvedorDeSnapshotsWikiEmDisco>();
        services.AddScoped<IProvedorDeHabilidadesSeed, ProvedorDeHabilidadesSeedEfCore>();
        services.AddScoped<IProvedorDeAssetsAuditados, ProvedorDeAssetsAuditadosEfCore>();
        services.AddScoped<IAuditoriaWikiService, AuditoriaWikiService>();

        // Feature 005 - Publicação atômica
        services.AddScoped<IDetectorDeSessoesAtivas, DetectorDeSessoesAtivasSqlServer>();
        services.AddScoped<IPublicadorAtomicoService, PublicadorAtomicoEfCore>();

        // Feature 006 - Vínculos de mídia (não reutiliza o publicador 005)
        if (configuration is not null)
        {
            services.Configure<OpcoesDeInventarioDeMidias>(configuration.GetSection(OpcoesDeInventarioDeMidias.Section));
        }
        else
        {
            services.Configure<OpcoesDeInventarioDeMidias>(_ => { });
        }

        services.AddScoped<ILeitorDeInventarioDeMidias, LeitorDeInventarioDeMidias>();
        services.AddScoped<IPublicadorDeVinculosDeMidia, PublicadorDeVinculosDeMidia>();
        services.AddScoped<ICoberturaDeMidiasService, CoberturaDeMidiasService>();

        return services;
    }
}