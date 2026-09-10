namespace DarkestDungeon.Domain.Personagens;

/// Habilidade atribuída a um Personagem (referência viva a `Habilidade` — sem snapshot).
/// Feature 005: adicionado `NumeroDoNivel` (0..5) — 0 = bloqueada, 1..5 = treinada no nível.
/// Flags legadas (`Habilitada`, `Treinada`, `Equipada`) preservadas para compatibilidade com a Feature 003.
public sealed class HabilidadeDePersonagem
{
    private HabilidadeDePersonagem()
    {
    }

    public HabilidadeDePersonagem(Guid habilidadeId, bool habilitada = true, bool treinada = false, bool equipada = false, int numeroDoNivel = 0)
    {
        if (equipada && !treinada)
        {
            throw new ArgumentException("Uma habilidade só pode ser Equipada se estiver Treinada.", nameof(equipada));
        }

        if (numeroDoNivel is < 0 or > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(numeroDoNivel), "NumeroDoNivel deve estar entre 0 e 5.");
        }

        HabilidadeId = habilidadeId;
        Habilitada = habilitada;
        Treinada = treinada;
        Equipada = equipada;
        NumeroDoNivel = numeroDoNivel > 0 ? numeroDoNivel : (treinada ? 1 : 0);
    }

    public Guid HabilidadeId { get; private set; }
    public bool Habilitada { get; private set; }
    public bool Treinada { get; private set; }
    public bool Equipada { get; private set; }

    /// Feature 005: nível atual desta habilidade para este Personagem.
    /// 0 = bloqueada / não treinada; 1..5 = treinada no nível correspondente da `Habilidade.Niveis`.
    public int NumeroDoNivel { get; private set; }

    /// Feature 005: derivado — habilidade é considerada treinada se `NumeroDoNivel >= 1`.
    /// Mantido em paralelo à flag legada `Treinada` para retrocompat.
    public bool TreinadaPorNivel => NumeroDoNivel >= 1;

    public void DefinirHabilitada(bool habilitada) => Habilitada = habilitada;

    public void Treinar()
    {
        Treinada = true;
        if (NumeroDoNivel == 0)
        {
            NumeroDoNivel = 1;
        }
    }

    /// Feature 005: subir/definir o nível atual (1..5). Ao definir >= 1, marca também `Treinada`.
    public void DefinirNivel(int numeroDoNivel)
    {
        if (numeroDoNivel is < 0 or > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(numeroDoNivel), "NumeroDoNivel deve estar entre 0 e 5.");
        }

        if (numeroDoNivel == 0 && Equipada)
        {
            throw new InvalidOperationException("Não é possível bloquear (Nivel=0) uma habilidade equipada. Desequipe primeiro.");
        }

        NumeroDoNivel = numeroDoNivel;
        if (numeroDoNivel >= 1)
        {
            Treinada = true;
        }
        else
        {
            Treinada = false;
        }
    }

    public void Equipar()
    {
        if (!Treinada && NumeroDoNivel < 1)
        {
            throw new InvalidOperationException("A habilidade precisa ser treinada antes de ser equipada.");
        }

        Equipada = true;
    }

    public void Desequipar() => Equipada = false;
}
