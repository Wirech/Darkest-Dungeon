using System.Text.Json;
using DarkestDungeon.Api.Contracts;
using DarkestDungeon.Api.Extensions;
using DarkestDungeon.Application.Extensions;
using DarkestDungeon.Infrastructure.Data;
using DarkestDungeon.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var erros = context.ModelState
                .Where(item => item.Value?.Errors.Count > 0)
                .SelectMany(item => item.Value!.Errors.Select(error => new ErroCampoResponse(
                    JsonNamingPolicy.CamelCase.ConvertName(item.Key),
                    string.IsNullOrWhiteSpace(error.ErrorMessage) ? "Valor inválido." : error.ErrorMessage)))
                .ToArray();

            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(new ErroResponse("Dados inválidos.", erros));
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();
builder.Services.AddApplicationServices();
builder.Services.AdicionarServicosDoCatalogo();
if (builder.Environment.IsEnvironment("Testing"))
{
    var databaseName = builder.Configuration["Testing:DatabaseName"] ?? "DarkestDungeonTesting";
    builder.Services.AddInfrastructureServices(options => options.UseInMemoryDatabase(databaseName));
}
else
{
    builder.Services.AddInfrastructureServices(builder.Configuration);
}
builder.Services.AdicionarRepositoriosDoCatalogo();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new { status = "saudável" }));
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DarkestDungeonDbContext>();
    if (app.Environment.IsEnvironment("Testing"))
    {
        context.Database.EnsureCreated();
    }

    var idsExistentes = context.Classes.Select(c => c.Id).ToHashSet();
    var novas = DarkestDungeon.Infrastructure.Data.Seeds.ClassesSeed.Materializar()
        .Where(c => !idsExistentes.Contains(c.Id))
        .ToArray();
    if (novas.Length > 0)
    {
        context.Classes.AddRange(novas);
        context.SaveChanges();
    }

    var idsClassePorEnum = context.Classes.ToDictionary(c => c.ClasseDeHeroi, c => c.Id);
    var idsHabilidadeExistentes = context.Habilidades.Select(h => h.Id).ToHashSet();
    var todasHabilidadesSeed = DarkestDungeon.Infrastructure.Data.Seeds.HabilidadesSeed.Materializar();
    var resultadoNiveis = DarkestDungeon.Infrastructure.Data.Seeds.AplicadorDeNiveisDoWikiSnapshot.Aplicar(todasHabilidadesSeed);
    Console.WriteLine($"[Feature 005] Níveis Wiki: aplicadas={resultadoNiveis.Aplicadas}, fallback={resultadoNiveis.Fallback}, ignoradas={resultadoNiveis.Ignoradas}");
    var novasHabilidades = todasHabilidadesSeed
        .Where(h => !idsHabilidadeExistentes.Contains(h.Id))
        .ToArray();
    if (novasHabilidades.Length > 0)
    {
        context.Habilidades.AddRange(novasHabilidades);
        context.SaveChanges();
    }

    // Feature 005: persistir/atualizar Niveis+ValoresDeEfeito+Descricao via SQL bulk (bypass do change tracker EF por causa de owned types).
    // Só aplica quando o provider é relacional (SQL Server) — em Testing usa InMemory que não suporta raw SQL.
    // Roda sempre: idempotente — DELETE+INSERT dos níveis reflete alterações nos JSONs; UPDATE Descricao sincroniza com a wiki.
    if (context.Database.IsRelational())
    {
        var inseridos = DarkestDungeon.Infrastructure.Data.Seeds.AplicadorDeNiveisDoWikiSnapshot.PersistirNiveisEmSql(context, todasHabilidadesSeed);
        Console.WriteLine($"[Feature 005] Níveis persistidos em SQL: {inseridos} linhas em NiveisDeHabilidade");

        // FR-003b: marca Level 2..5 como Pendente no MapaDeCobertura para habilidades que caíram em fallback (exceto shared globais single-level).
        var pendencias = DarkestDungeon.Infrastructure.Data.Seeds.AplicadorDeNiveisDoWikiSnapshot.RegistrarPendenciasDeNivelNoMapa(context, resultadoNiveis, todasHabilidadesSeed);
        if (pendencias > 0)
        {
            Console.WriteLine($"[Feature 005] Pendências de nível registradas no MapaDeCobertura: {pendencias}");
        }
    }

    var associacoesExistentes = context.ClassesHabilidades
        .Select(ch => new { ch.ClasseId, ch.HabilidadeId })
        .ToHashSet();
    var novasAssociacoes = DarkestDungeon.Infrastructure.Data.Seeds.HabilidadesSeed
        .MaterializarAssociacoes(idsClassePorEnum)
        .Where(ch => !associacoesExistentes.Contains(new { ch.ClasseId, ch.HabilidadeId }))
        .ToArray();
    if (novasAssociacoes.Length > 0)
    {
        context.ClassesHabilidades.AddRange(novasAssociacoes);
        context.SaveChanges();
    }

    if (!context.MapaDeCobertura.Any())
    {
        context.MapaDeCobertura.AddRange(DarkestDungeon.Infrastructure.Data.Seeds.MapaDeCoberturaSeed.Materializar());
        context.SaveChanges();
    }

    // Feature 005 - vincular assets Classe × Aparência ao inventário Spine da Feature 004.
    var classesParaAssets = context.Classes.Include(c => c.Assets).ToDictionary(c => c.ClasseDeHeroi, c => c);
    if (classesParaAssets.Values.Any(c => c.Assets.Count == 0))
    {
        var resultado = DarkestDungeon.Infrastructure.Data.Seeds.AssetsDeClasseSeed.Aplicar(classesParaAssets);
        context.SaveChanges();
        Console.WriteLine($"[Feature 005] AssetsDeClasse: coletados={resultado.Coletados}, pendentes={resultado.Pendentes}");
    }
}

// Feature 005: CLI standalone `dotnet run -- auditoria [--classe=<slug>|todas]` (T151/T030) — não sobe servidor HTTP.
if (DarkestDungeon.Api.Auditoria.AuditoriaJobRunner.DeveExecutar(args))
{
    return await DarkestDungeon.Api.Auditoria.AuditoriaJobRunner.ExecutarAsync(args, app.Services);
}

app.Run();
return 0;

public partial class Program
{
}
