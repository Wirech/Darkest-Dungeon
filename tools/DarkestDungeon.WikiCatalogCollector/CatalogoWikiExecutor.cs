using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DarkestDungeon.WikiCatalogCollector;

internal static class CatalogoWikiExecutor
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static async Task<int> ExecutarColetaAsync(string pastaSaida, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(pastaSaida);
        var paginas = CatalogoDePaginasWiki.Listar();
        var snapshots = new List<(PaginaDeClasseWiki Pagina, SnapshotDeClasseOficial Snapshot)>();
        var lacunas = new List<LacunaDeColeta>();

        using var cliente = new ClienteWiki();
        foreach (var pagina in paginas)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Console.WriteLine($"Coletando {pagina.NomeOriginal} ({pagina.NomeEnum})...");

            string urlFinal;
            string wikitext;
            try
            {
                (urlFinal, wikitext) = await cliente.ObterWikitextAsync(pagina.TituloCanonico, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                lacunas.Add(new LacunaDeColeta(pagina.NomeEnum, "download", ex.Message));
                continue;
            }

            var parse = ParserDeWikitext.Parsear(wikitext, pagina);
            lacunas.AddRange(parse.Lacunas);
            if (parse.Snapshot is null)
            {
                continue;
            }

            parse.Snapshot.FonteUrl = urlFinal;
            lacunas.AddRange(ValidadorDeSnapshot.Validar(parse.Snapshot));
            if (parse.Snapshot.Forma is not "humana")
            {
                lacunas.Add(new LacunaDeColeta(pagina.NomeEnum, "forma", "Apenas a forma humana é permitida."));
                continue;
            }

            snapshots.Add((pagina, parse.Snapshot));
            await Task.Delay(250, cancellationToken).ConfigureAwait(false);
        }

        if (lacunas.Count > 0 || snapshots.Count != paginas.Count)
        {
            Console.Error.WriteLine("Coleta falhou. Lacunas (nenhum valor foi inventado nem gravado):");
            foreach (var lacuna in lacunas)
            {
                Console.Error.WriteLine($"- [{lacuna.Classe}] {lacuna.Campo}: {lacuna.Motivo}");
            }

            if (snapshots.Count != paginas.Count)
            {
                var obtidas = snapshots.Select(s => s.Pagina.NomeEnum).ToHashSet();
                foreach (var pagina in paginas.Where(p => !obtidas.Contains(p.NomeEnum)))
                {
                    Console.Error.WriteLine($"- [{pagina.NomeEnum}] snapshot: classe sem JSON válido.");
                }
            }

            return 2;
        }

        foreach (var (pagina, snapshot) in snapshots)
        {
            var caminho = Path.Combine(pastaSaida, $"{pagina.Slug}.json");
            await File.WriteAllTextAsync(
                    caminho,
                    JsonSerializer.Serialize(snapshot, JsonOptions),
                    cancellationToken)
                .ConfigureAwait(false);
            Console.WriteLine($"Gravado {caminho}");
        }

        Console.WriteLine($"Coleta concluída: {snapshots.Count} classes em {pastaSaida}");
        return 0;
    }
}
