namespace DarkestDungeon.Domain.Personagens;

/// Habilidade atribuída a um Personagem (referência viva a `Habilidade` — sem snapshot).
public sealed class HabilidadeDePersonagem
{
    private HabilidadeDePersonagem()
    {
    }

    public HabilidadeDePersonagem(Guid habilidadeId, bool habilitada = true, bool treinada = false, bool equipada = false)
    {
        if (equipada && !treinada)
        {
            throw new ArgumentException("Uma habilidade só pode ser Equipada se estiver Treinada.", nameof(equipada));
        }

        HabilidadeId = habilidadeId;
        Habilitada = habilitada;
        Treinada = treinada;
        Equipada = equipada;
    }

    public Guid HabilidadeId { get; private set; }
    public bool Habilitada { get; private set; }
    public bool Treinada { get; private set; }
    public bool Equipada { get; private set; }

    public void DefinirHabilitada(bool habilitada) => Habilitada = habilitada;

    public void Treinar() => Treinada = true;

    public void Equipar()
    {
        if (!Treinada)
        {
            throw new InvalidOperationException("A habilidade precisa ser treinada antes de ser equipada.");
        }

        Equipada = true;
    }

    public void Desequipar() => Equipada = false;
}
