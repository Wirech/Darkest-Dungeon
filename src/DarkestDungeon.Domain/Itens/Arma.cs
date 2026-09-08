using DarkestDungeon.Domain.Classes;

namespace DarkestDungeon.Domain.Itens;

/// Arma (Item) com exatamente uma classe elegível e cinco níveis fixos (FR-011).
public sealed class Arma : Item
{
    private readonly List<NivelDeArma> niveis = new();

    private Arma()
    {
    }

    public Arma(
        string nomeExibicao,
        string nomeOriginal,
        string descricao,
        ClasseDeHeroi classeElegivel,
        IEnumerable<NivelDeArma> niveis,
        Guid? id = null)
        : base(nomeExibicao, nomeOriginal, descricao, id)
    {
        ArgumentNullException.ThrowIfNull(niveis);
        var arr = niveis.ToArray();

        if (arr.Length != 5)
        {
            throw new ArgumentException("Arma deve conter exatamente 5 níveis.", nameof(niveis));
        }

        if (arr.Select(n => n.Nivel).Distinct().Count() != 5)
        {
            throw new ArgumentException("Níveis de Arma devem ser únicos (1..5).", nameof(niveis));
        }

        if (arr.Any(n => n.Nivel is < 1 or > 5))
        {
            throw new ArgumentOutOfRangeException(nameof(niveis), "Cada nível de Arma deve estar entre 1 e 5.");
        }

        this.niveis.AddRange(arr.OrderBy(n => n.Nivel));
        ClasseElegivel = classeElegivel;
    }

    public ClasseDeHeroi ClasseElegivel { get; private set; }
    public IReadOnlyList<NivelDeArma> Niveis => niveis;
}
