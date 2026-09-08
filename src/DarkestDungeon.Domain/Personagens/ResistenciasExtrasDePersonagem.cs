namespace DarkestDungeon.Domain.Personagens;

/// Resistências extras próprias de Personagem (Doença, Golpe Mortal, Armadilha).
/// Ser e Inimigo intencionalmente NÃO possuem essas resistências (FR-015).
public sealed record ResistenciasExtrasDePersonagem
{
    private ResistenciasExtrasDePersonagem()
    {
    }

    public ResistenciasExtrasDePersonagem(decimal doenca, decimal golpeMortal, decimal armadilha)
    {
        ValidarPercentual(doenca, nameof(doenca));
        ValidarPercentual(golpeMortal, nameof(golpeMortal));
        ValidarPercentual(armadilha, nameof(armadilha));

        Doenca = doenca;
        GolpeMortal = golpeMortal;
        Armadilha = armadilha;
    }

    public decimal Doenca { get; private init; }
    public decimal GolpeMortal { get; private init; }
    public decimal Armadilha { get; private init; }

    private static void ValidarPercentual(decimal valor, string nome)
    {
        if (valor < 0m || valor > 100m)
        {
            throw new ArgumentOutOfRangeException(nome, $"{nome} deve estar entre 0 e 100.");
        }
    }
}
