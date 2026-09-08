namespace DarkestDungeon.Domain.Classes;

/// Associação N:M entre Classe e HabilidadeDeHeroi (FR-007).
/// A mesma HabilidadeDeHeroi pode ser referenciada por várias Classes (compartilhada).
public sealed class ClasseHabilidade
{
    private ClasseHabilidade()
    {
    }

    public ClasseHabilidade(Guid classeId, Guid habilidadeId)
    {
        if (classeId == Guid.Empty)
        {
            throw new ArgumentException("ClasseId não pode ser vazio.", nameof(classeId));
        }

        if (habilidadeId == Guid.Empty)
        {
            throw new ArgumentException("HabilidadeId não pode ser vazio.", nameof(habilidadeId));
        }

        ClasseId = classeId;
        HabilidadeId = habilidadeId;
    }

    public Guid ClasseId { get; private set; }
    public Guid HabilidadeId { get; private set; }
}
