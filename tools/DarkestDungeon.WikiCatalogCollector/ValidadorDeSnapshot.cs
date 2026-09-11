namespace DarkestDungeon.WikiCatalogCollector;

internal static class ValidadorDeSnapshot
{
    public static IReadOnlyList<LacunaDeColeta> Validar(SnapshotDeClasseOficial snapshot)
    {
        var lacunas = new List<LacunaDeColeta>();
        var classe = snapshot.ClasseDeHeroiEnum;

        if (snapshot.PassosAFrente < 0 || snapshot.PassosAtras < 0)
        {
            lacunas.Add(new LacunaDeColeta(classe, "passos", "Passos negativos não são oficiais."));
        }

        if (string.IsNullOrWhiteSpace(snapshot.BonusAoCritico)
            || snapshot.BonusAoCritico.Trim() == "-")
        {
            lacunas.Add(new LacunaDeColeta(classe, "bonusAoCritico", "Texto oficial do Crit Buff Bonus ausente."));
        }

        if (snapshot.Arma.Niveis.Count != 5)
        {
            lacunas.Add(new LacunaDeColeta(classe, "arma", $"Esperados 5 níveis, obtidos {snapshot.Arma.Niveis.Count}."));
        }

        if (snapshot.Armadura.Niveis.Count != 5)
        {
            lacunas.Add(new LacunaDeColeta(classe, "armadura", $"Esperados 5 níveis, obtidos {snapshot.Armadura.Niveis.Count}."));
        }

        foreach (var nivel in snapshot.Arma.Niveis)
        {
            if (nivel.DanoMinimo > nivel.DanoMaximo)
            {
                lacunas.Add(new LacunaDeColeta(classe, $"arma.nivel{nivel.Nivel}", "Dano mínimo maior que máximo."));
            }
        }

        return lacunas;
    }
}
