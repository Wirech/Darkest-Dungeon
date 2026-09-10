namespace DarkestDungeon.Application.Auditoria;

/// Relatório completo de auditoria — agregação de linhas + resumo.
public sealed record RelatorioDeAuditoria(
    DateTime GeradoEm,
    ResumoDeAuditoria Resumo,
    IReadOnlyList<LinhaDeAuditoria> Linhas,
    IReadOnlyList<ChanceBaseCapada> ChancesCapadas
);

/// Entrada FR-013 — habilidade cuja chance base publicada na wiki excede 100% (capada na aplicação).
public sealed record ChanceBaseCapada(
    string NomeOriginal,
    string TipoDoEfeito,
    int NumeroDoNivel,
    decimal ChanceOriginal,
    decimal ChanceAplicada
);
