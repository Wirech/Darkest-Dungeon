namespace DarkestDungeon.Application.Auditoria;

/// Uma linha de divergência entre o valor esperado (wiki) e o valor atual (seed).
public sealed record DiffDeCampo(
    string Campo,
    string ValorEsperado,
    string ValorAtual,
    string Observacao
);
