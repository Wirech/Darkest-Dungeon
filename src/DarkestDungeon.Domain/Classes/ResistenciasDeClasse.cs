namespace DarkestDungeon.Domain.Classes;

/// Resistências base fornecidas pela Classe ao Personagem (8 percentuais 0..100).
public sealed record ResistenciasDeClasse
{
    private ResistenciasDeClasse()
    {
    }

    public ResistenciasDeClasse(
        decimal atordoamento,
        decimal sangramento,
        decimal envenenamento,
        decimal debuff,
        decimal movimento,
        decimal doenca,
        decimal golpeMortal,
        decimal armadilha)
    {
        ValidarPercentual(atordoamento, nameof(atordoamento));
        ValidarPercentual(sangramento, nameof(sangramento));
        ValidarPercentual(envenenamento, nameof(envenenamento));
        ValidarPercentual(debuff, nameof(debuff));
        ValidarPercentual(movimento, nameof(movimento));
        ValidarPercentual(doenca, nameof(doenca));
        ValidarPercentual(golpeMortal, nameof(golpeMortal));
        ValidarPercentual(armadilha, nameof(armadilha));

        Atordoamento = atordoamento;
        Sangramento = sangramento;
        Envenenamento = envenenamento;
        Debuff = debuff;
        Movimento = movimento;
        Doenca = doenca;
        GolpeMortal = golpeMortal;
        Armadilha = armadilha;
    }

    public decimal Atordoamento { get; private init; }
    public decimal Sangramento { get; private init; }
    public decimal Envenenamento { get; private init; }
    public decimal Debuff { get; private init; }
    public decimal Movimento { get; private init; }
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
