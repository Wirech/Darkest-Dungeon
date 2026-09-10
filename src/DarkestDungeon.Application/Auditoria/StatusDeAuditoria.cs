namespace DarkestDungeon.Application.Auditoria;

/// Status de auditoria de uma habilidade contra a wiki oficial.
public enum StatusDeAuditoria
{
    /// Todos os campos batem com a wiki.
    OK = 0,

    /// Alguns campos divergem ou estão simplificados.
    Parcial = 1,

    /// Campos obrigatórios ausentes ou não coletados.
    Faltando = 2,
}
