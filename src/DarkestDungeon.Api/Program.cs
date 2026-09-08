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
    var novasHabilidades = DarkestDungeon.Infrastructure.Data.Seeds.HabilidadesSeed.Materializar()
        .Where(h => !idsHabilidadeExistentes.Contains(h.Id))
        .ToArray();
    if (novasHabilidades.Length > 0)
    {
        context.Habilidades.AddRange(novasHabilidades);
        context.SaveChanges();
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
}

app.Run();

public partial class Program
{
}
