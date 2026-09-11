namespace DarkestDungeon.WikiCatalogCollector;

internal sealed class SnapshotDeClasseOficial
{
    public string ClasseDeHeroiEnum { get; set; } = string.Empty;
    public string FonteUrl { get; set; } = string.Empty;
    public string Forma { get; set; } = "humana";
    public int PassosAFrente { get; set; }
    public int PassosAtras { get; set; }
    public bool Religiosa { get; set; }
    public string ProvisaoInicial { get; set; } = string.Empty;
    public string BonusAoCritico { get; set; } = string.Empty;
    public SnapshotDeArma Arma { get; set; } = new();
    public SnapshotDeArmadura Armadura { get; set; } = new();
}

internal sealed class SnapshotDeArma
{
    public List<NivelDeArmaSnapshot> Niveis { get; set; } = new();
}

internal sealed class SnapshotDeArmadura
{
    public List<NivelDeArmaduraSnapshot> Niveis { get; set; } = new();
}

internal sealed class NivelDeArmaSnapshot
{
    public int Nivel { get; set; }
    public int DanoMinimo { get; set; }
    public int DanoMaximo { get; set; }
    public decimal Critico { get; set; }
    public int Velocidade { get; set; }
}

internal sealed class NivelDeArmaduraSnapshot
{
    public int Nivel { get; set; }
    public int HpMaximo { get; set; }
    public decimal Esquiva { get; set; }
}

internal sealed record LacunaDeColeta(string Classe, string Campo, string Motivo);
