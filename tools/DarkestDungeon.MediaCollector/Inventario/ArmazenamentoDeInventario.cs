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
        var anterior = await CarregarCompletoAsync(diretorioDeSaida, cancellationToken);
        if (anterior is null) return mapa;
        foreach (var arquivo in anterior.Arquivos)
        {
            mapa[arquivo.CaminhoOrigem] = (arquivo.Sha256, arquivo.TamanhoBytes);
        }
        return mapa;
    }

    public static async Task<ResultadoDaImportacao?> CarregarCompletoAsync(string diretorioDeSaida, CancellationToken cancellationToken)
    {
        var caminho = Path.Combine(diretorioDeSaida, NomeArquivoInventario);
        if (!File.Exists(caminho)) return null;
        try
        {
            await using var fluxo = File.OpenRead(caminho);
            return await JsonSerializer.DeserializeAsync<ResultadoDaImportacao>(fluxo, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public static async Task<ResultadoDaImportacao> MesclarCampingAsync(
        string diretorioDeSaida,
        ResultadoDaImportacao camping,
        CancellationToken cancellationToken)
    {
        var existente = await CarregarCompletoAsync(diretorioDeSaida, cancellationToken);
        if (existente is null)
        {
            return camping;
        }

        var porDestino = existente.Arquivos
            .Where(a => !string.IsNullOrWhiteSpace(a.CaminhoDestino))
            .GroupBy(a => a.CaminhoDestino, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.Last(), StringComparer.OrdinalIgnoreCase);

        foreach (var arquivo in camping.Arquivos)
        {
            porDestino[arquivo.CaminhoDestino] = arquivo;
        }

        var lacunas = existente.Lacunas
            .Where(l => !string.Equals(l.Categoria, "HabilidadeDeAcampamento", StringComparison.OrdinalIgnoreCase))
            .Concat(camping.Lacunas)
            .ToList();

        var resumo = existente.ResumoPorClasse
            .Where(r => !string.Equals(r.Classe, "Acampamento", StringComparison.OrdinalIgnoreCase))
            .Concat(camping.ResumoPorClasse)
            .ToList();

        return new ResultadoDaImportacao(
            string.IsNullOrWhiteSpace(camping.InstalacaoOrigem) ? existente.InstalacaoOrigem : camping.InstalacaoOrigem,
            string.IsNullOrWhiteSpace(existente.DeclaracaoDeUso) ? camping.DeclaracaoDeUso : existente.DeclaracaoDeUso,
            porDestino.Values.ToList(),
            existente.Conjuntos,
            existente.AssociacoesDeHabilidades,
            lacunas,
            resumo);
    }
}
