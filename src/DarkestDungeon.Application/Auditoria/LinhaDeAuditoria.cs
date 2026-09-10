using DarkestDungeon.Domain.Classes;

namespace DarkestDungeon.Application.Auditoria;

/// Entrada do relatório de auditoria por habilidade.
public sealed record LinhaDeAuditoria(
    Guid HabilidadeId,
    string NomeExibicao,
    string NomeOriginal,
    ClasseDeHeroi ClasseDoDono,
    StatusDeAuditoria Status,
    IReadOnlyList<DiffDeCampo> Diffs
);
