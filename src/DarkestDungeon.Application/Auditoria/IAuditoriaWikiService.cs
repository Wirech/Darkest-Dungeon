using DarkestDungeon.Domain.Classes;

namespace DarkestDungeon.Application.Auditoria;

/// Serviço da Feature 005 que compara os dados semeados (Feature 003) com os snapshots da wiki oficial e produz o relatório de auditoria.
public interface IAuditoriaWikiService
{
    /// Gera o relatório. Se `filtroClasse` for informado, restringe às habilidades daquela classe.
    Task<RelatorioDeAuditoria> GerarRelatorioAsync(ClasseDeHeroi? filtroClasse = null, CancellationToken cancellationToken = default);

    /// Serializa o relatório como Markdown e grava no caminho informado (usado pelo job runner CLI).
    Task SalvarComoMarkdownAsync(RelatorioDeAuditoria relatorio, string caminhoAbsoluto, CancellationToken cancellationToken = default);
}
