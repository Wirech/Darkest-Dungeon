namespace DarkestDungeon.Domain.Itens;

/// Atributos numéricos de um dos cinco níveis de uma Armadura.
public sealed record NivelDeArmadura
{
    private NivelDeArmadura()
    {
    }

    public NivelDeArmadura(int nivel, int hpAdicional, decimal esquiva)
    {
        if (nivel is < 1 or > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(nivel), "Nível de Armadura deve estar entre 1 e 5.");
        }

        if (hpAdicional < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(hpAdicional), "HP adicional deve ser >= 0.");
        }

        if (esquiva < 0m || esquiva > 100m)
        {
            throw new ArgumentOutOfRangeException(nameof(esquiva), "Esquiva deve estar entre 0 e 100.");
        }

        Nivel = nivel;
        HpAdicional = hpAdicional;
        Esquiva = esquiva;
    }

    public int Nivel { get; private init; }
    public int HpAdicional { get; private init; }
    public decimal Esquiva { get; private init; }
}
