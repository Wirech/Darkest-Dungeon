namespace DarkestDungeon.Domain.Itens;

/// Atributos numéricos de um dos cinco níveis de uma Arma.
public sealed record NivelDeArma
{
    private NivelDeArma()
    {
    }

    public NivelDeArma(int nivel, int danoMinimo, int danoMaximo, decimal critico, int velocidade)
    {
        if (nivel is < 1 or > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(nivel), "Nível de Arma deve estar entre 1 e 5.");
        }

        if (danoMinimo < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(danoMinimo), "Dano mínimo deve ser >= 0.");
        }

        if (danoMaximo < danoMinimo)
        {
            throw new ArgumentException("Dano máximo deve ser >= dano mínimo.", nameof(danoMaximo));
        }

        if (critico < 0m || critico > 100m)
        {
            throw new ArgumentOutOfRangeException(nameof(critico), "Crítico deve estar entre 0 e 100.");
        }

        if (velocidade < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(velocidade), "Velocidade deve ser >= 0.");
        }

        Nivel = nivel;
        DanoMinimo = danoMinimo;
        DanoMaximo = danoMaximo;
        Critico = critico;
        Velocidade = velocidade;
    }

    public int Nivel { get; private init; }
    public int DanoMinimo { get; private init; }
    public int DanoMaximo { get; private init; }
    public decimal Critico { get; private init; }
    public int Velocidade { get; private init; }
}
