namespace DarkestDungeon.Domain.Habilidades;

/// Valor de um efeito específico dentro de um `NivelDeHabilidade`.
/// Ex.: "Sangramento" com Valor=5 e Chance=100 no Level 3 de Slice It.
public sealed record ValorDeEfeito
{
    private ValorDeEfeito()
    {
        TipoDoEfeito = string.Empty;
    }

    public ValorDeEfeito(string tipoDoEfeito, decimal valor, decimal? chance = null)
    {
        if (string.IsNullOrWhiteSpace(tipoDoEfeito))
        {
            throw new ArgumentException("Tipo do efeito deve ser informado.", nameof(tipoDoEfeito));
        }

        if (tipoDoEfeito.Length > 60)
        {
            throw new ArgumentException("Tipo do efeito deve ter no máximo 60 caracteres.", nameof(tipoDoEfeito));
        }

        if (chance is < 0m or > 200m)
        {
            throw new ArgumentOutOfRangeException(nameof(chance), "Chance deve estar entre 0 e 200 (permite bônus > 100% capado depois).");
        }

        TipoDoEfeito = tipoDoEfeito.Trim();
        Valor = valor;
        Chance = chance;
    }

    public string TipoDoEfeito { get; private init; }
    public decimal Valor { get; private init; }
    public decimal? Chance { get; private init; }
}
