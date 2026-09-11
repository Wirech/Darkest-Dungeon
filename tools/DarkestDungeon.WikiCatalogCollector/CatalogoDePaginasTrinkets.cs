using System.Text.RegularExpressions;

namespace DarkestDungeon.WikiCatalogCollector;

internal static class CatalogoDePaginasTrinkets
{
    internal static readonly IReadOnlyList<string> Indices = new[]
    {
        "Trinkets",
        "Crimson Court Trinkets",
        "Color of Madness Trinkets",
        "Shieldbreaker Trinkets",
        "The Color of Madness",
        "The Crimson Court",
    };

    private static readonly Regex LinkWiki = new(
        @"\[\[(?!File:|Image:|Category:|Template:|Trinkets)([^\]|#]+)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    internal static IReadOnlyList<string> ExtrairTitulosDeIndice(string wikitext)
    {
        if (string.IsNullOrWhiteSpace(wikitext))
        {
            return [];
        }

        var titulos = new List<string>();
        var vistos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (Match match in LinkWiki.Matches(wikitext))
        {
            var titulo = match.Groups[1].Value.Trim();
            if (titulo.Length is 0 or > 80)
            {
                continue;
            }

            if (titulo.Contains("trinket", StringComparison.OrdinalIgnoreCase)
                && titulo.Contains("page", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (vistos.Add(titulo))
            {
                titulos.Add(titulo);
            }
        }

        return titulos;
    }

    internal static string Slug(string titulo)
    {
        var limpo = titulo.Trim().ToLowerInvariant();
        limpo = Regex.Replace(limpo, @"[^a-z0-9]+", "_");
        return limpo.Trim('_');
    }
}
