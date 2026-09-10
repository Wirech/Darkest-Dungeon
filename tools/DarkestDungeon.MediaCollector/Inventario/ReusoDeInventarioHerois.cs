using System.Text.Json;

namespace DarkestDungeon.MediaCollector.Inventario;

public static class ReusoDeInventarioHerois
{
    public static async Task<Dictionary<string, (string CaminhoDestino, long Tamanho)>> CarregarPorHashAsync(
        string? caminhoInventarioHerois,
        CancellationToken cancellationToken)
    {
        var mapa = new Dictionary<string, (string, long)>(StringComparer.OrdinalIgnoreCase);
        var caminho = caminhoInventarioHerois;
        if (string.IsNullOrWhiteSpace(caminho))
        {
            var padrao = Path.Combine("assets", "herois", "inventario.json");
            caminho = File.Exists(padrao) ? padrao : null;
        }

        if (string.IsNullOrWhiteSpace(caminho) || !File.Exists(caminho))
        {
            return mapa;
        }

        try
        {
            await using var fluxo = File.OpenRead(caminho);
            var anterior = await JsonSerializer.DeserializeAsync<ResultadoDaImportacao>(
                fluxo,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cancellationToken);
            if (anterior is null)
            {
                return mapa;
            }

            foreach (var arquivo in anterior.Arquivos)
            {
                if (!string.IsNullOrWhiteSpace(arquivo.Sha256))
                {
                    mapa[arquivo.Sha256] = (arquivo.CaminhoDestino, arquivo.TamanhoBytes);
                }
            }
        }
        catch (JsonException)
        {
        }

        return mapa;
    }
}
