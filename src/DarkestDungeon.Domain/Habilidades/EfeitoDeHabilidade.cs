namespace DarkestDungeon.Domain.Habilidades;

/// Linha estruturada de efeito de uma Habilidade (FR-006a).
public sealed record EfeitoDeHabilidade
{
    private EfeitoDeHabilidade()
    {
        NomeDoEfeito = string.Empty;
    }

    public EfeitoDeHabilidade(
        string nomeDoEfeito,
        AlvoDeEfeito alvo,
        decimal valor,
        UnidadeDeEfeito unidade,
        decimal chanceBase,
        int? duracaoEmRodadas = null)
    {
        if (string.IsNullOrWhiteSpace(nomeDoEfeito))
        {
            throw new ArgumentException("Nome do efeito deve ser informado.", nameof(nomeDoEfeito));
        }

        if (nomeDoEfeito.Length > 60)
        {
            throw new ArgumentException("Nome do efeito deve ter no máximo 60 caracteres.", nameof(nomeDoEfeito));
        }

        if (chanceBase < 0m || chanceBase > 100m)
        {
            throw new ArgumentOutOfRangeException(nameof(chanceBase), "Chance base deve estar entre 0 e 100.");
        }

        if (duracaoEmRodadas is < 0 or > 20)
        {
            throw new ArgumentOutOfRangeException(nameof(duracaoEmRodadas), "Duração deve estar entre 0 e 20 rodadas.");
        }

        NomeDoEfeito = nomeDoEfeito.Trim();
        Alvo = alvo;
        Valor = valor;
        Unidade = unidade;
        ChanceBase = chanceBase;
        DuracaoEmRodadas = duracaoEmRodadas;
    }

    public string NomeDoEfeito { get; private init; }
    public AlvoDeEfeito Alvo { get; private init; }
    public decimal Valor { get; private init; }
    public UnidadeDeEfeito Unidade { get; private init; }
    public decimal ChanceBase { get; private init; }
    public int? DuracaoEmRodadas { get; private init; }
}
