using System.Text.Json;
using DarkestDungeon.Domain.Habilidades;
using DarkestDungeon.Infrastructure.Data.Seeds;

namespace DarkestDungeon.MediaCollector.Inventario;

public static class GeradorDoManifestoDeCamping
{
    public static async Task AtualizarAsync(string diretorioDeSaida, CancellationToken cancellationToken)
    {
        var destino = Path.Combine(diretorioDeSaida, "manifesto-habilidades.json");
        var existentes = new List<AssociacaoDeHabilidade>();
        if (File.Exists(destino))
        {
            var lido = await LeitorDoManifesto.LerAsync(destino, cancellationToken);
            existentes = lido.Associacoes
                .Where(a => !a.Arquivo.StartsWith("camp_skill_", StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var camping = new List<AssociacaoDeHabilidade>();
        foreach (var nome in NomesDeAcampamentoDoCatalogo())
        {
            var id = AliasesDeCamping.IdDoJogo(nome);
            if (AliasesDeCamping.Leftovers.Contains(id))
            {
                continue;
            }

            camping.Add(new AssociacaoDeHabilidade("Acampamento", AliasesDeCamping.NomeDoArquivo(nome), [nome]));
        }

        var manifesto = new ManifestoDeHabilidades(1, existentes.Concat(camping).ToList());
        Directory.CreateDirectory(diretorioDeSaida);
        var json = JsonSerializer.Serialize(manifesto, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(destino, json, cancellationToken);
    }

    public static IReadOnlyList<string> NomesDeAcampamentoDoCatalogo() =>
        HabilidadesSeed.Materializar()
            .OfType<HabilidadeDeAcampamento>()
            .Select(h => h.NomeOriginal)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
            .ToArray();
}
