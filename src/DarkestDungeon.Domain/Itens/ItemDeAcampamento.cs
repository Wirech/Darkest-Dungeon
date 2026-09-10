namespace DarkestDungeon.Domain.Itens;

/// Item de acampamento/provisão (tipo novo 006). Sem atributos numéricos de combate.
public sealed class ItemDeAcampamento : Item
{
    private ItemDeAcampamento()
    {
        Midia = MidiaDeItem.Pendente();
    }

    public ItemDeAcampamento(
        string nomeExibicao,
        string nomeOriginal,
        string descricao,
        MidiaDeItem? midia = null,
        Guid? id = null)
        : base(nomeExibicao, nomeOriginal, descricao, id)
    {
        Midia = midia ?? MidiaDeItem.Pendente();
    }

    public MidiaDeItem Midia { get; private set; }

    public void DefinirMidia(MidiaDeItem midia)
    {
        ArgumentNullException.ThrowIfNull(midia);
        Midia = midia;
    }
}
