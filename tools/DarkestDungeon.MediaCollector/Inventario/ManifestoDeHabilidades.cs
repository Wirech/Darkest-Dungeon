using System.Text.Json;

namespace DarkestDungeon.MediaCollector.Inventario;

public sealed record AssociacaoDeHabilidade(string Classe, string Arquivo, IReadOnlyList<string> Habilidades);
public sealed record ManifestoDeHabilidades(int Versao, List<AssociacaoDeHabilidade> Associacoes);

public static class LeitorDoManifesto
{
    public static async Task<ManifestoDeHabilidades> LerAsync(string caminho, CancellationToken cancellationToken)
    {
        await using var fluxo = File.OpenRead(caminho);
        var manifesto = await JsonSerializer.DeserializeAsync<ManifestoDeHabilidades>(fluxo, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, cancellationToken);
        return manifesto ?? throw new InvalidDataException("Manifesto de habilidades inválido.");
    }
}