namespace DarkestDungeon.Domain.Habilidades;

/// Habilidade acessível apenas a Inimigos (FR-004). Nunca de acampamento.
public sealed class HabilidadeDeInimigo : Habilidade
{
    private readonly List<EfeitoDeHabilidade> efeitos = new();

    private HabilidadeDeInimigo()
    {
    }

    public HabilidadeDeInimigo(
        string nomeExibicao,
        string nomeOriginal,
        string descricao,
        IEnumerable<EfeitoDeHabilidade> efeitos,
        string? condicaoDeAparecer = null,
        decimal? chanceDeExecucao = null,
        Guid? id = null)
        : base(nomeExibicao, nomeOriginal, descricao, id)
    {
        ArgumentNullException.ThrowIfNull(efeitos);

        if (condicaoDeAparecer is not null && condicaoDeAparecer.Length > 200)
        {
            throw new ArgumentException("Condição de aparecer deve ter até 200 caracteres.", nameof(condicaoDeAparecer));
        }

        if (chanceDeExecucao is < 0m or > 100m)
        {
            throw new ArgumentOutOfRangeException(nameof(chanceDeExecucao), "Chance de execução deve estar entre 0 e 100.");
        }

        this.efeitos.AddRange(efeitos);
        CondicaoDeAparecer = condicaoDeAparecer?.Trim();
        ChanceDeExecucao = chanceDeExecucao;
    }

    public IReadOnlyList<EfeitoDeHabilidade> Efeitos => efeitos;
    public string? CondicaoDeAparecer { get; private set; }
    public decimal? ChanceDeExecucao { get; private set; }
}
