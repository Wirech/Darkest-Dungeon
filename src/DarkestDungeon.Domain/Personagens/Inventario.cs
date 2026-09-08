namespace DarkestDungeon.Domain.Personagens;

/// Inventário padrão de um Personagem: exatamente 4 slots (Guid? — Id do Item ou vazio).
public sealed class Inventario
{
    private readonly Guid?[] slots = new Guid?[TamanhoDoInventario];

    /// Tamanho fixo do inventário padrão (4 slots).
    public const int TamanhoDoInventario = 4;

    public Inventario()
    {
    }

    public Inventario(IEnumerable<Guid?> conteudo)
    {
        ArgumentNullException.ThrowIfNull(conteudo);
        var lista = conteudo.ToArray();
        if (lista.Length != TamanhoDoInventario)
        {
            throw new ArgumentException($"Inventário deve conter exatamente {TamanhoDoInventario} slots.", nameof(conteudo));
        }

        Array.Copy(lista, slots, TamanhoDoInventario);
    }

    public IReadOnlyList<Guid?> Slots => slots;

    public void DefinirSlot(int indice, Guid? itemId)
    {
        if (indice < 0 || indice >= TamanhoDoInventario)
        {
            throw new ArgumentOutOfRangeException(nameof(indice), $"Índice deve estar entre 0 e {TamanhoDoInventario - 1}.");
        }

        slots[indice] = itemId;
    }
}
