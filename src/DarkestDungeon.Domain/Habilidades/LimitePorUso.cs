namespace DarkestDungeon.Domain.Habilidades;

/// Limite de uso de uma habilidade (batalha ou acampamento).
public sealed record LimitePorUso
{
    private LimitePorUso()
    {
    }

    public LimitePorUso(EscopoDeLimite escopo, int maximoUsos)
    {
        if (maximoUsos is < 1 or > 10)
        {
            throw new ArgumentOutOfRangeException(nameof(maximoUsos), "Máximo de usos deve estar entre 1 e 10.");
        }

        Escopo = escopo;
        MaximoUsos = maximoUsos;
    }

    public EscopoDeLimite Escopo { get; private init; }
    public int MaximoUsos { get; private init; }
}
