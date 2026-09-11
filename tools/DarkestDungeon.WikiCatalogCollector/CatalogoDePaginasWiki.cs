using System.ComponentModel;
using System.Reflection;

namespace DarkestDungeon.WikiCatalogCollector;

internal enum ClasseDeHeroiWiki
{
    [Description("Abomination")]
    Abominacao,
    [Description("Antiquarian")]
    Antiquario,
    [Description("Arbalest")]
    Besteiro,
    [Description("Bounty Hunter")]
    CacadorDeRecompensas,
    [Description("Crusader")]
    Cruzado,
    [Description("Grave Robber")]
    LadraoDeCova,
    [Description("Jester")]
    BoboDaCorte,
    [Description("Houndmaster")]
    MestreDeCaca,
    [Description("Leper")]
    Leproso,
    [Description("Hellion")]
    Infernal,
    [Description("Highwayman")]
    Bandido,
    [Description("Musketeer")]
    Musqueteiro,
    [Description("Man-at-Arms")]
    Veterano,
    [Description("Occultist")]
    Ocultista,
    [Description("Plague Doctor")]
    MedicoDaPeste,
    [Description("Vestal")]
    Vestal,
    [Description("Flagellant")]
    Flagelante,
    [Description("Shieldbreaker")]
    Rompedor,
    [Description("Duelist")]
    Duelista,
    [Description("Runaway")]
    Fugitivo
}

internal sealed record PaginaDeClasseWiki(
    ClasseDeHeroiWiki Enum,
    string NomeEnum,
    string NomeOriginal,
    string Slug,
    string TituloCanonico);

internal static class CatalogoDePaginasWiki
{
    internal static readonly IReadOnlyDictionary<ClasseDeHeroiWiki, string> SlugPorClasse = new Dictionary<ClasseDeHeroiWiki, string>
    {
        [ClasseDeHeroiWiki.Abominacao] = "abominacao",
        [ClasseDeHeroiWiki.Antiquario] = "antiquario",
        [ClasseDeHeroiWiki.Besteiro] = "besteiro",
        [ClasseDeHeroiWiki.CacadorDeRecompensas] = "cacador-de-recompensas",
        [ClasseDeHeroiWiki.Cruzado] = "cruzado",
        [ClasseDeHeroiWiki.LadraoDeCova] = "ladrao-de-cova",
        [ClasseDeHeroiWiki.BoboDaCorte] = "bobo-da-corte",
        [ClasseDeHeroiWiki.MestreDeCaca] = "mestre-de-caca",
        [ClasseDeHeroiWiki.Leproso] = "leproso",
        [ClasseDeHeroiWiki.Infernal] = "infernal",
        [ClasseDeHeroiWiki.Bandido] = "bandido",
        [ClasseDeHeroiWiki.Musqueteiro] = "musqueteiro",
        [ClasseDeHeroiWiki.Veterano] = "veterano",
        [ClasseDeHeroiWiki.Ocultista] = "ocultista",
        [ClasseDeHeroiWiki.MedicoDaPeste] = "medico-da-peste",
        [ClasseDeHeroiWiki.Vestal] = "vestal",
        [ClasseDeHeroiWiki.Flagelante] = "flagelante",
        [ClasseDeHeroiWiki.Rompedor] = "rompedor",
        [ClasseDeHeroiWiki.Duelista] = "duelista",
        [ClasseDeHeroiWiki.Fugitivo] = "fugitivo"
    };

    public static IReadOnlyList<PaginaDeClasseWiki> Listar()
    {
        return Enum.GetValues<ClasseDeHeroiWiki>()
            .Select(valor =>
            {
                var membro = typeof(ClasseDeHeroiWiki).GetMember(valor.ToString()).Single();
                var nomeOriginal = membro.GetCustomAttribute<DescriptionAttribute>()?.Description
                    ?? throw new InvalidOperationException($"Classe '{valor}' sem Description.");
                return new PaginaDeClasseWiki(
                    valor,
                    valor.ToString(),
                    nomeOriginal,
                    SlugPorClasse[valor],
                    $"{nomeOriginal} (Darkest Dungeon)");
            })
            .ToArray();
    }
}
