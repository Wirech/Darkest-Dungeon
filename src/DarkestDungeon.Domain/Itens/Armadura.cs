using DarkestDungeon.Domain.Classes;

namespace DarkestDungeon.Domain.Itens;

/// Armadura (Item) com exatamente uma classe elegível e cinco níveis fixos (FR-012).
public sealed class Armadura : Item
{
    private readonly List<NivelDeArmadura> niveis = new();

    private Armadura()
    {
    }

    public Armadura(
        string nomeExibicao,
        string nomeOriginal,
        string descricao,
        ClasseDeHeroi classeElegivel,
        IEnumerable<NivelDeArmadura> niveis,
        Guid? id = null)
        : base(nomeExibicao, nomeOriginal, descricao, id)
    {
        ArgumentNullException.ThrowIfNull(niveis);
        var arr = niveis.ToArray();

        if (arr.Length != 5)
        {
            throw new ArgumentException("Armadura deve conter exatamente 5 níveis.", nameof(niveis));
        }

        if (arr.Select(n => n.Nivel).Distinct().Count() != 5)
        {
            throw new ArgumentException("Níveis de Armadura devem ser únicos (1..5).", nameof(niveis));
        }

        if (arr.Any(n => n.Nivel is < 1 or > 5))
        {
            throw new ArgumentOutOfRangeException(nameof(niveis), "Cada nível de Armadura deve estar entre 1 e 5.");
        }

        this.niveis.AddRange(arr.OrderBy(n => n.Nivel));
        ClasseElegivel = classeElegivel;
    }

    public ClasseDeHeroi ClasseElegivel { get; private set; }
    public IReadOnlyList<NivelDeArmadura> Niveis => niveis;
}
