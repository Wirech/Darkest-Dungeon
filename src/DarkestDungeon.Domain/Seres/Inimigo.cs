namespace DarkestDungeon.Domain.Seres;

/// Oponente controlado pelo sistema — especialização de `Ser`.
/// Possui apenas as 5 resistências de `Ser` (Atordoamento, Sangramento, Envenenamento, Debuff, Movimento);
/// intencionalmente NÃO possui Doença, Golpe Mortal nem Armadilha (FR-015, Q9).
public sealed class Inimigo : Ser
{
    private readonly List<Guid> habilidadesIds = new();

    private Inimigo()
    {
    }

    public Inimigo(
        string nome,
        TipoDeInimigo tipoDeInimigo,
        int hpMaximo,
        int hpAtual,
        int velocidade,
        decimal critico,
        int danoBaseMinimo,
        int danoBaseMaximo,
        int movimento,
        decimal bonusDeCritico,
        int tamanho,
        int acoesPorTurno,
        decimal esquiva,
        decimal precisao,
        decimal protecao,
        int nivel,
        Resistencias resistencias,
        IEnumerable<Guid>? habilidadesIds = null,
        Guid? id = null)
        : base(nome, tipoDeInimigo.ToString(), hpMaximo, hpAtual, velocidade, critico, danoBaseMinimo, danoBaseMaximo, movimento, bonusDeCritico, tamanho, acoesPorTurno, esquiva, precisao, protecao, nivel, resistencias, id)
    {
        TipoDeInimigo = tipoDeInimigo;
        if (habilidadesIds is not null)
        {
            this.habilidadesIds.AddRange(habilidadesIds);
        }
    }

    public TipoDeInimigo TipoDeInimigo { get; private set; }
    public IReadOnlyList<Guid> HabilidadesIds => habilidadesIds;

    public void AdicionarHabilidade(Guid habilidadeId)
    {
        if (habilidadesIds.Contains(habilidadeId))
        {
            throw new ArgumentException("Habilidade já foi atribuída a este Inimigo.", nameof(habilidadeId));
        }

        habilidadesIds.Add(habilidadeId);
    }
}
