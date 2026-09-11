using System.Text.RegularExpressions;

namespace DarkestDungeon.MediaCollector.Inventario;

public static class AliasesDeCamping
{
    private static readonly IReadOnlyDictionary<string, string> WikiParaId = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["Wound Care"] = "first_aid",
        ["This Is How We Do It"] = "how_its_done",
        ["Unshakable Leader"] = "unshakeable_leader",
        ["Night Moves"] = "night_steps",
        ["Unparalleled Finesse"] = "uncatchable",
        ["The Cure"] = "preventative_medicine",
        ["Self-Medicate"] = "self_medicate",
        ["Every Rose Has Its Thorn"] = "every_rose",
        ["Man's Best Friend"] = "pet_the_hound",
        ["Resupply"] = "supply",
        ["Snuff Box"] = "forage",
        ["Snake Eyes"] = "way_of_serpent",
        ["Snake Skin"] = "way_of_scales",
        ["Sandstorm"] = "way_of_sway",
        ["Adder's Embrace"] = "way_of_adder",
        ["Lash's Anger"] = "lash_anger",
        ["Lash's Solace"] = "lash_solace",
        ["Lash's Kiss"] = "lash_kiss",
        ["Lash's Cure"] = "lash_cure",
        ["Pick Pocket"] = "pickpocket",
        ["Again!"] = "again",
    };

    public static readonly HashSet<string> Leftovers = new(StringComparer.OrdinalIgnoreCase)
    {
        "bandage", "bear_traps", "hobby", "perimeter_alarms", "wrap",
    };

    public static string IdDoJogo(string nomeOriginal)
    {
        if (WikiParaId.TryGetValue(nomeOriginal.Trim(), out var id))
        {
            return id;
        }

        var slug = nomeOriginal.ToLowerInvariant()
            .Replace("'", string.Empty, StringComparison.Ordinal)
            .Replace("!", string.Empty, StringComparison.Ordinal)
            .Replace("-", "_", StringComparison.Ordinal);
        slug = Regex.Replace(slug, "[^a-z0-9]+", "_");
        return slug.Trim('_');
    }

    public static string NomeDoArquivo(string nomeOriginal) => $"camp_skill_{IdDoJogo(nomeOriginal)}.png";
}
