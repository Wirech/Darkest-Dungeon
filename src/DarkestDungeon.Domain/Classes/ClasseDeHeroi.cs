using System.ComponentModel;

namespace DarkestDungeon.Domain.Classes;

/// Catálogo controlado das 20 classes oficiais de heróis (jogo base + DLCs).
public enum ClasseDeHeroi
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
    Fugitivo,
}
