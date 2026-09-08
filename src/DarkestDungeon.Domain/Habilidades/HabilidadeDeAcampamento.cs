namespace DarkestDungeon.Domain.Habilidades;

/// Habilidade usada em acampamentos (FR-006).
public sealed class HabilidadeDeAcampamento : HabilidadeDeHeroi
{
    private HabilidadeDeAcampamento()
    {
    }

    public HabilidadeDeAcampamento(
        string nomeExibicao,
        string nomeOriginal,
        string descricao,
        int custoDeDescanso,
        AlvoDeAcampamento alvo,
        IEnumerable<EfeitoDeHabilidade> efeitos,
        LimitePorUso? limitePorUso = null,
        Guid? id = null)
        : base(nomeExibicao, nomeOriginal, descricao, efeitos, limitePorUso, id)
    {
        if (custoDeDescanso is < 0 or > 20)
        {
            throw new ArgumentOutOfRangeException(nameof(custoDeDescanso), "Custo de descanso deve estar entre 0 e 20.");
        }

        CustoDeDescanso = custoDeDescanso;
        Alvo = alvo;
    }

    public int CustoDeDescanso { get; private set; }
    public AlvoDeAcampamento Alvo { get; private set; }
}
