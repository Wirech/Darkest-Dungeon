using DarkestDungeon.Domain.Classes;

namespace DarkestDungeon.Domain.Itens;

/// Acessório (trinket) com raridade, efeitos e classe exclusiva opcional (FR-013).
/// `ConjuntoId` é apenas metadata; o cálculo do bônus de conjunto está fora do escopo desta feature.
public sealed class Acessorio : Item
{
    private readonly List<EfeitoDeAcessorio> efeitos = new();

    private Acessorio()
    {
        Midia = MidiaDeItem.Pendente();
    }

    public Acessorio(
        string nomeExibicao,
        string nomeOriginal,
        string descricao,
        RaridadeDeAcessorio raridade,
        IEnumerable<EfeitoDeAcessorio> efeitos,
        ClasseDeHeroi? classeExclusiva = null,
        Guid? conjuntoId = null,
        Guid? id = null)
        : base(nomeExibicao, nomeOriginal, descricao, id)
    {
        ArgumentNullException.ThrowIfNull(efeitos);
        this.efeitos.AddRange(efeitos);
        Raridade = raridade;
        ClasseExclusiva = classeExclusiva;
        ConjuntoId = conjuntoId;
        Midia = MidiaDeItem.Pendente();
    }

    public RaridadeDeAcessorio Raridade { get; private set; }
    public ClasseDeHeroi? ClasseExclusiva { get; private set; }
    public Guid? ConjuntoId { get; private set; }
    public IReadOnlyList<EfeitoDeAcessorio> Efeitos => efeitos;
    public MidiaDeItem Midia { get; private set; }

    public void DefinirMidia(MidiaDeItem midia)
    {
        ArgumentNullException.ThrowIfNull(midia);
        Midia = midia;
    }
}
