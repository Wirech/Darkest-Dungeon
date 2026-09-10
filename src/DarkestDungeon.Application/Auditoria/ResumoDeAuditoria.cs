namespace DarkestDungeon.Application.Auditoria;

/// Estatísticas agregadas do relatório de auditoria.
public sealed record ResumoDeAuditoria(
    int TotalHabilidades,
    int Ok,
    int Parcial,
    int Faltando,
    int NiveisPendentes,
    int AssetsColetados,
    int AssetsPendentes
);
