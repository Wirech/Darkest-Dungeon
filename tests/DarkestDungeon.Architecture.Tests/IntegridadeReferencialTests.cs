using DarkestDungeon.Api.Extensions;
using DarkestDungeon.Application.Extensions;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Habilidades;
using DarkestDungeon.Domain.Itens;
using DarkestDungeon.Infrastructure.Data;
using DarkestDungeon.Infrastructure.Extensions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace DarkestDungeon.Architecture.Tests;

/// Valida via inspeção do modelo EF Core que FKs de Classe/Habilidade/Item não permitem cascade.
/// Cumpre FR-023 sem necessidade de SQL Server real.
public class IntegridadeReferencialTests
{
    private static DarkestDungeonDbContext CriarContexto()
    {
        var services = new ServiceCollection();
        services.AddApplicationServices();
        services.AdicionarServicosDoCatalogo();
        services.AddInfrastructureServices(options => options.UseInMemoryDatabase("integ-ref-" + Guid.NewGuid()));
        services.AdicionarRepositoriosDoCatalogo();
        var provider = services.BuildServiceProvider();
        return provider.CreateScope().ServiceProvider.GetRequiredService<DarkestDungeonDbContext>();
    }

    [Fact]
    public void FKs_para_Classe_e_Habilidade_devem_usar_OnDelete_Restrict()
    {
        using var contexto = CriarContexto();
        var model = contexto.Model;

        var relacionamentos = model.GetEntityTypes()
            .Where(et => !et.IsOwned())
            .SelectMany(et => et.GetForeignKeys())
            .Where(fk => !fk.DeclaringEntityType.IsOwned())
            .Where(fk => fk.PrincipalEntityType.ClrType == typeof(Classe) || fk.PrincipalEntityType.ClrType == typeof(Habilidade))
            .ToArray();

        relacionamentos.Should().NotBeEmpty("deve haver FKs para Classe ou Habilidade");
        foreach (var fk in relacionamentos)
        {
            fk.DeleteBehavior.Should().Be(DeleteBehavior.Restrict, $"FK {fk.DeclaringEntityType.Name}→{fk.PrincipalEntityType.Name} deve bloquear cascade");
        }
    }
}
