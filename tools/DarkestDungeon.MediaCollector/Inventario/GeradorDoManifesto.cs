using System.Text.Json;
using DarkestDungeon.MediaCollector.Catalogo;
using DarkestDungeon.MediaCollector.Configuracao;

namespace DarkestDungeon.MediaCollector.Inventario;

public static class GeradorDoManifesto
{
    // Ordem canônica dos sufixos usados pelo jogo em `<classe>.ability.<sufixo>.png`.
    private static readonly string[] SufixosOrdemCanonica = ["one", "two", "three", "four", "five", "six", "seven"];

    public static async Task<int> GerarAsync(OpcoesDoColetor opcoes, IReadOnlyList<HeroiDoCatalogo> herois, CancellationToken cancellationToken)
    {
        var associacoes = new List<AssociacaoDeHabilidade>();
        foreach (var heroi in herois)
        {
            var diretorioClasse = Directory.EnumerateDirectories(opcoes.DiretorioDeOrigem, "*", SearchOption.AllDirectories)
                .FirstOrDefault(caminho => Path.GetFileName(caminho).Equals(NomeDiretorioDaClasse(heroi.NomeOriginal), StringComparison.OrdinalIgnoreCase)
                    && Path.GetFileName(Path.GetDirectoryName(caminho) ?? string.Empty).Equals("heroes", StringComparison.OrdinalIgnoreCase));
            if (diretorioClasse is null) continue;

            var arquivos = Directory.EnumerateFiles(diretorioClasse, "*.ability.*.png", SearchOption.TopDirectoryOnly)
                .Select(caminho => Path.GetFileName(caminho))
                .OrderBy(nome => Array.IndexOf(SufixosOrdemCanonica, nome.Split('.').Reverse().Skip(1).First()))
                .ToArray();

            for (var indice = 0; indice < arquivos.Length && indice < heroi.Habilidades.Count; indice++)
            {
                associacoes.Add(new(heroi.NomeExibicao, arquivos[indice], [heroi.Habilidades[indice]]));
            }
        }

        var manifesto = new ManifestoDeHabilidades(1, associacoes);
        Directory.CreateDirectory(opcoes.DiretorioDeSaida);
        var destino = Path.Combine(opcoes.DiretorioDeSaida, "manifesto-habilidades.json");
        await File.WriteAllTextAsync(destino, JsonSerializer.Serialize(manifesto, new JsonSerializerOptions { WriteIndented = true }), cancellationToken);
        Console.WriteLine($"Manifesto gerado com {associacoes.Count} associações em {destino}.");
        return 0;
    }

    private static string NomeDiretorioDaClasse(string nomeOriginal) => nomeOriginal switch
    {
        "Bounty Hunter" => "bounty_hunter",
        "Grave Robber" => "grave_robber",
        "Man-at-Arms" => "man_at_arms",
        "Plague Doctor" => "plague_doctor",
        _ => nomeOriginal.ToLowerInvariant().Replace(" ", "_", StringComparison.Ordinal),
    };
}
