namespace DarkestDungeon.Domain.Habilidades;

/// Uma das 5 linhas obrigatórias por Habilidade (Level 1..5 da wiki oficial).
/// Owned type dentro de `Habilidade`.
public sealed class NivelDeHabilidade
{
    private readonly List<ValorDeEfeito> valoresDeEfeito = new();

    private NivelDeHabilidade()
    {
    }

    public NivelDeHabilidade(
        int numeroDoNivel,
        decimal modificadorDano,
        decimal modificadorAcerto,
        decimal modificadorCritico,
        IEnumerable<ValorDeEfeito>? valoresDeEfeito = null,
        int? custoDeDescanso = null)
    {
        if (numeroDoNivel is < 1 or > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(numeroDoNivel), "Nível de habilidade deve estar entre 1 e 5.");
        }

        if (custoDeDescanso is < 0 or > 20)
        {
            throw new ArgumentOutOfRangeException(nameof(custoDeDescanso), "Custo de descanso deve estar entre 0 e 20.");
        }

        NumeroDoNivel = numeroDoNivel;
        ModificadorDano = modificadorDano;
        ModificadorAcerto = modificadorAcerto;
        ModificadorCritico = modificadorCritico;
        CustoDeDescanso = custoDeDescanso;

        if (valoresDeEfeito is not null)
        {
            this.valoresDeEfeito.AddRange(valoresDeEfeito);
        }
    }

    public int NumeroDoNivel { get; private set; }
    public decimal ModificadorDano { get; private set; }
    public decimal ModificadorAcerto { get; private set; }
    public decimal ModificadorCritico { get; private set; }
    public int? CustoDeDescanso { get; private set; }
    public IReadOnlyList<ValorDeEfeito> ValoresDeEfeito => valoresDeEfeito;
}
