using System.Security.Cryptography;
using System.Text.Json;
using DarkestDungeon.Application.Midias;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DarkestDungeon.Api.Tests.Feature006;

internal static class InventarioDeMidiasTestHelper
{
    internal static readonly byte[] PngMinimo = [137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 0];

    internal static string HashPng => Convert.ToHexString(SHA256.HashData(PngMinimo)).ToLowerInvariant();

    internal static string FixturesEquipamentos => Path.GetFullPath(Path.Combine(
        AppContext.BaseDirectory, "ColetaMidias", "Fixtures", "Equipamentos"));

    internal static string MapeamentoPastas => Path.GetFullPath(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "..",
        "tools", "DarkestDungeon.MediaCollector", "Configuracao", "mapeamento-pastas.json"));

    internal static string GravarInventario(string diretorio, IEnumerable<object> arquivos, IEnumerable<object>? lacunas = null)
    {
        Directory.CreateDirectory(diretorio);
        var caminho = Path.Combine(diretorio, "inventario.json");
        var documento = new
        {
            instalacaoOrigem = "fixture",
            declaracaoDeUso = "Assets importados de instalação local licenciada; redistribuição não autorizada.",
            arquivos,
            lacunas = lacunas ?? [],
        };
        File.WriteAllText(caminho, JsonSerializer.Serialize(documento, new JsonSerializerOptions { WriteIndented = true }));
        return caminho;
    }

    internal static object Arquivo(
        string categoria,
        string origem,
        string destino,
        string? sha = null,
        string? classe = null) => new
    {
        categoria,
        sha256 = sha ?? HashPng,
        caminhoOrigem = origem,
        caminhoDestino = destino,
        tamanhoBytes = PngMinimo.LongLength,
        reutilizado = false,
        classe,
    };

    internal static HttpClient ClienteComInventario(WebApplicationFactory<Program> factory, string caminhoInventario)
    {
        var databaseName = $"DarkestDungeonTesting-{Guid.NewGuid():N}";
        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Testing:DatabaseName"] = databaseName,
                    ["Midias:CaminhoDoInventario"] = caminhoInventario,
                });
            });
        }).CreateClient();
    }

    internal static HttpClient ClienteComLeitor(WebApplicationFactory<Program> factory, ILeitorDeInventarioDeMidias leitor)
    {
        var databaseName = $"DarkestDungeonTesting-{Guid.NewGuid():N}";
        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Testing:DatabaseName"] = databaseName,
                });
            });
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<ILeitorDeInventarioDeMidias>();
                services.AddSingleton(leitor);
            });
        }).CreateClient();
    }
}

internal sealed class LeitorDeInventarioAtrasado : ILeitorDeInventarioDeMidias
{
    private readonly InventarioDeMidiasDeEquipamento inventario;
    private readonly TimeSpan atraso;

    public LeitorDeInventarioAtrasado(InventarioDeMidiasDeEquipamento inventario, TimeSpan atraso)
    {
        this.inventario = inventario;
        this.atraso = atraso;
    }

    public async Task<InventarioDeMidiasDeEquipamento?> LerAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(atraso, cancellationToken);
        return inventario;
    }
}

internal sealed class LeitorDeInventarioQueFalha : ILeitorDeInventarioDeMidias
{
    public Task<InventarioDeMidiasDeEquipamento?> LerAsync(CancellationToken cancellationToken) =>
        throw new InvalidOperationException("falha simulada de publicação");
}

internal sealed class LeitorDeInventarioFixo : ILeitorDeInventarioDeMidias
{
    private readonly InventarioDeMidiasDeEquipamento inventario;

    public LeitorDeInventarioFixo(InventarioDeMidiasDeEquipamento inventario)
    {
        this.inventario = inventario;
    }

    public Task<InventarioDeMidiasDeEquipamento?> LerAsync(CancellationToken cancellationToken) =>
        Task.FromResult<InventarioDeMidiasDeEquipamento?>(inventario);
}
