namespace DarkestDungeon.Domain.Habilidades;

/// Especialização abstrata para habilidades acessíveis a Personagens.
/// Subdividida em `HabilidadeDeCombate` e `HabilidadeDeAcampamento`.
public abstract class HabilidadeDeHeroi : Habilidade
{
    private readonly List<EfeitoDeHabilidade> efeitos = new();

    protected HabilidadeDeHeroi()
    {
    }

    protected HabilidadeDeHeroi(
        string nomeExibicao,
        string nomeOriginal,
        string descricao,
        IEnumerable<EfeitoDeHabilidade> efeitos,
        LimitePorUso? limitePorUso = null,
        Guid? id = null)
        : base(nomeExibicao, nomeOriginal, descricao, id)
    {
        ArgumentNullException.ThrowIfNull(efeitos);
        this.efeitos.AddRange(efeitos);
        LimitePorUso = limitePorUso;
    }

    public IReadOnlyList<EfeitoDeHabilidade> Efeitos => efeitos;
    public LimitePorUso? LimitePorUso { get; private set; }
}
