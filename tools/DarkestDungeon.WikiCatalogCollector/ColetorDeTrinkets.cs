using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DarkestDungeon.WikiCatalogCollector;

internal static class ColetorDeTrinkets
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static async Task<int> ExecutarAsync(string pastaSaida, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(pastaSaida);
        var lacunas = new List<LacunaDeTrinket>();
        var titulos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        using var cliente = new ClienteWiki();
        foreach (var indice in CatalogoDePaginasTrinkets.Indices)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Console.WriteLine($"Descobrindo trinkets em {indice}...");
            try
            {
                var (_, wikitext) = await cliente.ObterWikitextAsync(indice, cancellationToken).ConfigureAwait(false);
                foreach (var titulo in CatalogoDePaginasTrinkets.ExtrairTitulosDeIndice(wikitext))
                {
                    titulos.Add(titulo);
                }
            }
            catch (Exception ex) when (ex.Message.Contains("429", StringComparison.Ordinal))
            {
                Console.Error.WriteLine($"Falha de infraestrutura (HTTP 429) no índice '{indice}': {ex.Message}");
                return 2;
            }
            catch (Exception ex)
            {
                lacunas.Add(new LacunaDeTrinket
                {
                    TrinketOuPagina = indice,
                    Campo = "indice",
                    Motivo = ex.Message,
                    Momento = DateTimeOffset.UtcNow.ToString("O"),
                });
            }
        }

        var gravados = 0;
        foreach (var titulo in titulos.OrderBy(t => t, StringComparer.OrdinalIgnoreCase))
        {
            cancellationToken.ThrowIfCancellationRequested();
            Console.WriteLine($"Coletando trinket {titulo}...");
            try
            {
                var (urlFinal, wikitext) = await cliente.ObterWikitextAsync(titulo, cancellationToken).ConfigureAwait(false);
                var parse = ParserDeTrinkets.Parsear(wikitext, urlFinal, titulo);
                lacunas.AddRange(parse.Lacunas);
                if (parse.Snapshot is null)
                {
                    continue;
                }

                var slug = CatalogoDePaginasTrinkets.Slug(parse.Snapshot.NomeOriginal);
                var caminho = Path.Combine(pastaSaida, $"{slug}.json");
                await File.WriteAllTextAsync(caminho, JsonSerializer.Serialize(parse.Snapshot, JsonOptions), cancellationToken)
                    .ConfigureAwait(false);
                gravados++;
            }
            catch (Exception ex) when (ex.Message.Contains("429", StringComparison.Ordinal))
            {
                Console.Error.WriteLine($"Falha de infraestrutura (HTTP 429) em '{titulo}': {ex.Message}");
                await GravarRelatorioAsync(pastaSaida, lacunas, cancellationToken).ConfigureAwait(false);
                return 2;
            }
            catch (Exception ex)
            {
                lacunas.Add(new LacunaDeTrinket
                {
                    TrinketOuPagina = titulo,
                    Campo = "download",
                    Motivo = ex.Message,
                    Momento = DateTimeOffset.UtcNow.ToString("O"),
                });
            }
        }

        await GravarRelatorioAsync(pastaSaida, lacunas, cancellationToken).ConfigureAwait(false);
        Console.WriteLine($"Coleta de trinkets concluída. Snapshots={gravados}. Lacunas={lacunas.Count}.");
        return 0;
    }

    private static Task GravarRelatorioAsync(string pastaSaida, IReadOnlyList<LacunaDeTrinket> lacunas, CancellationToken cancellationToken)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Relatório de lacunas — trinkets");
        sb.AppendLine();
        if (lacunas.Count == 0)
        {
            sb.AppendLine("Nenhuma lacuna registrada.");
        }
        else
        {
            foreach (var lacuna in lacunas)
            {
                sb.AppendLine($"- **{lacuna.TrinketOuPagina}** / `{lacuna.Campo}`: {lacuna.Motivo} ({lacuna.Momento})");
            }
        }

        return File.WriteAllTextAsync(Path.Combine(pastaSaida, "lacunas.md"), sb.ToString(), cancellationToken);
    }
}
