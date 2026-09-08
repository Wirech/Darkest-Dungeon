namespace DarkestDungeon.Domain.Itens;

/// Efeito estruturado de um Acessório (positivo ou negativo).
public sealed record EfeitoDeAcessorio
{
    private EfeitoDeAcessorio()
    {
        Nome = string.Empty;
    }

    public EfeitoDeAcessorio(string nome, decimal valor, UnidadeDeEfeitoDeAcessorio unidade, SinalDeEfeito sinal)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("Nome do efeito deve ser informado.", nameof(nome));
        }

        if (nome.Length > 80)
        {
            throw new ArgumentException("Nome do efeito deve ter no máximo 80 caracteres.", nameof(nome));
        }

        Nome = nome.Trim();
        Valor = valor;
        Unidade = unidade;
        Sinal = sinal;
    }

    public string Nome { get; private init; }
    public decimal Valor { get; private init; }
    public UnidadeDeEfeitoDeAcessorio Unidade { get; private init; }
    public SinalDeEfeito Sinal { get; private init; }
}

/// Unidade numérica do valor de um Efeito de Acessório.
public enum UnidadeDeEfeitoDeAcessorio
{
    Percentual,
    Pontos,
}
