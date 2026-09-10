namespace DarkestDungeon.Domain.Itens;

/// Consumível (tipo novo 006). Sem atributos numéricos de combate.
public sealed class Consumivel : Item
{
    private Consumivel()
    {
        Midia = MidiaDeItem.Pendente();
    }

    public Consumivel(
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
