namespace DarkestDungeon.Domain.Habilidades;

/// Habilidade usada em batalha (FR-005).
public sealed class HabilidadeDeCombate : HabilidadeDeHeroi
{
    private readonly List<int> posicoesValidas = new();
    private readonly List<int> posicoesQueAtinge = new();

    private HabilidadeDeCombate()
    {
    }

    public HabilidadeDeCombate(
        string nomeExibicao,
        string nomeOriginal,
        string descricao,
        IEnumerable<int> posicoesValidas,
        IEnumerable<int> posicoesQueAtinge,
        bool alvoEmArea,
        decimal modificadorDano,
        decimal modificadorAcerto,
        decimal modificadorCritico,
        IEnumerable<EfeitoDeHabilidade> efeitos,
        LimitePorUso? limitePorUso = null,
        Guid? id = null)
        : base(nomeExibicao, nomeOriginal, descricao, efeitos, limitePorUso, id)
    {
        ArgumentNullException.ThrowIfNull(posicoesValidas);
        ArgumentNullException.ThrowIfNull(posicoesQueAtinge);

        var validasArr = posicoesValidas.ToArray();
        var atingeArr = posicoesQueAtinge.ToArray();

        if (validasArr.Length == 0)
        {
            throw new ArgumentException("Deve haver pelo menos uma posição válida.", nameof(posicoesValidas));
        }

        if (atingeArr.Length == 0)
        {
            throw new ArgumentException("Deve haver pelo menos uma posição que atinge.", nameof(posicoesQueAtinge));
        }

        foreach (var pos in validasArr.Concat(atingeArr))
        {
            if (pos is < 1 or > 4)
            {
                throw new ArgumentOutOfRangeException(nameof(posicoesValidas), "Posições devem estar entre 1 e 4.");
            }
        }

        this.posicoesValidas.AddRange(validasArr.Distinct().OrderBy(p => p));
        this.posicoesQueAtinge.AddRange(atingeArr.Distinct().OrderBy(p => p));
        AlvoEmArea = alvoEmArea;
        ModificadorDano = modificadorDano;
        ModificadorAcerto = modificadorAcerto;
        ModificadorCritico = modificadorCritico;
    }

    public IReadOnlyList<int> PosicoesValidas => posicoesValidas;
    public IReadOnlyList<int> PosicoesQueAtinge => posicoesQueAtinge;
    public bool AlvoEmArea { get; private set; }
    public decimal ModificadorDano { get; private set; }
    public decimal ModificadorAcerto { get; private set; }
    public decimal ModificadorCritico { get; private set; }
}
