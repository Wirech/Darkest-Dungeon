using System.Text.Json;

namespace DarkestDungeon.MediaCollector.Inventario;

public static class ArmazenamentoDeInventario
{
    private const string NomeArquivoInventario = "inventario.json";

    public static async Task SalvarAtomicamenteAsync(string diretorioDeSaida, ResultadoDaImportacao resultado, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(diretorioDeSaida);
        var temporario = Path.Combine(diretorioDeSaida, $"{NomeArquivoInventario}.tmp");
        var destino = Path.Combine(diretorioDeSaida, NomeArquivoInventario);
        await File.WriteAllTextAsync(temporario, JsonSerializer.Serialize(resultado, new JsonSerializerOptions { WriteIndented = true }), cancellationToken);
        File.Move(temporario, destino, overwrite: true);
    }

    public static async Task<Dictionary<string, (string Sha256, long Tamanho)>> CarregarAnteriorAsync(string diretorioDeSaida, CancellationToken cancellationToken)
    {
        var mapa = new Dictionary<string, (string, long)>(StringComparer.OrdinalIgnoreCase);
        var caminho = Path.Combine(diretorioDeSaida, NomeArquivoInventario);
        if (!File.Exists(caminho)) return mapa;
        try
        {
            await using var fluxo = File.OpenRead(caminho);
            var anterior = await JsonSerializer.DeserializeAsync<ResultadoDaImportacao>(fluxo, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);
            if (anterior is null) return mapa;
            foreach (var arquivo in anterior.Arquivos)
            {
                mapa[arquivo.CaminhoOrigem] = (arquivo.Sha256, arquivo.TamanhoBytes);
            }
        }
        catch (JsonException) { }
        return mapa;
    }
}
