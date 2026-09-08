namespace DarkestDungeon.Domain.Seres;

public sealed class Resistencias
{
    private Resistencias()
    {
    }

    public Resistencias(decimal atordoamento, decimal sangramento, decimal envenenamento, decimal debuff, decimal movimento)
    {
        ValidarPercentual(atordoamento, nameof(Atordoamento), "Resistência de Atordoamento");
        ValidarPercentual(sangramento, nameof(Sangramento), "Resistência de Sangramento");
        ValidarPercentual(envenenamento, nameof(Envenenamento), "Resistência de Envenenamento");
        ValidarPercentual(debuff, nameof(Debuff), "Resistência de Debuff");
        ValidarPercentual(movimento, nameof(Movimento), "Resistência de Movimento");

        Atordoamento = atordoamento;
        Sangramento = sangramento;
        Envenenamento = envenenamento;
        Debuff = debuff;
        Movimento = movimento;
    }

    public decimal Atordoamento { get; private set; }
    public decimal Sangramento { get; private set; }
    public decimal Envenenamento { get; private set; }
    public decimal Debuff { get; private set; }
    public decimal Movimento { get; private set; }

    public static void ValidarPercentual(decimal valor, string campo, string nomeExibicao)
    {
        if (valor is < 0 or > 100)
        {
            throw new ArgumentException($"{nomeExibicao} deve estar entre 0 e 100.", campo);
        }
    }
}