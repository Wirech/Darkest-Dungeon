using System.Text.Json;
using System.Text.Json.Nodes;
using DarkestDungeon.Api.Tests.Feature010;
using DarkestDungeon.Api.Tests.Fixtures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DarkestDungeon.Api.Tests.Feature012;

public abstract class AcervoFiltradoFactory : ApiTestFactory
{
    private readonly string pastaTemp;

    protected AcervoFiltradoFactory(Func<string, bool> manterArquivo)
    {
        pastaTemp = Path.Combine(Path.GetTempPath(), $"dd-acervo-012-{Guid.NewGuid():N}");
        Directory.CreateDirectory(pastaTemp);

        var origem = AcervoDoCardTestHelper.Herois;
        var inventarioOrigem = Path.Combine(origem, "inventario.json");
        using var fluxo = File.OpenRead(inventarioOrigem);
        var documento = JsonNode.Parse(fluxo)!.AsObject();
        var arquivos = documento["Arquivos"]!.AsArray();
        for (var i = arquivos.Count - 1; i >= 0; i--)
        {
            var caminho = arquivos[i]?["CaminhoDestino"]?.GetValue<string>() ?? string.Empty;
            if (!manterArquivo(caminho))
            {
                arquivos.RemoveAt(i);
            }
        }

        File.WriteAllText(
            Path.Combine(pastaTemp, "inventario.json"),
            documento.ToJsonString(new JsonSerializerOptions { WriteIndented = false }));

        var manifesto = Path.Combine(origem, "manifesto-habilidades.json");
        if (File.Exists(manifesto))
        {
            File.Copy(manifesto, Path.Combine(pastaTemp, "manifesto-habilidades.json"), overwrite: true);
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Acervo:Herois"] = pastaTemp
            });
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        try
        {
            if (Directory.Exists(pastaTemp))
            {
                Directory.Delete(pastaTemp, recursive: true);
            }
        }
        catch (IOException)
        {
            // pasta temporária de teste
        }
    }
}

public sealed class AcervoSemIdleDoCruzadoFactory : AcervoFiltradoFactory
{
    public AcervoSemIdleDoCruzadoFactory()
        : base(caminho => !caminho.Contains("crusader.sprite.idle", StringComparison.OrdinalIgnoreCase))
    {
    }
}

public sealed class AcervoSemWalkDoCruzadoFactory : AcervoFiltradoFactory
{
    public AcervoSemWalkDoCruzadoFactory()
        : base(caminho => !caminho.Contains("crusader.sprite.walk", StringComparison.OrdinalIgnoreCase))
    {
    }
}
