using DarkestDungeon.Application.Auditoria;
using DarkestDungeon.Domain.Classes;
using Microsoft.AspNetCore.Mvc;

namespace DarkestDungeon.Api.Controllers.Auditoria;

/// Controller da Feature 005 — endpoints de leitura do relatório de auditoria.
/// A execução (mineração + geração de Markdown) roda como job local via `AuditoriaJobRunner` fora da API pública.
[ApiController]
[Route("api/auditoria")]
public sealed class AuditoriaController : ControllerBase
{
    private readonly IAuditoriaWikiService servico;

    public AuditoriaController(IAuditoriaWikiService servico)
    {
        this.servico = servico ?? throw new ArgumentNullException(nameof(servico));
    }

    /// GET /api/auditoria/relatorio
    [HttpGet("relatorio")]
    public async Task<ActionResult<RelatorioDeAuditoria>> ObterRelatorioAsync(CancellationToken cancellationToken)
    {
        var relatorio = await servico.GerarRelatorioAsync(filtroClasse: null, cancellationToken);
        return Ok(relatorio);
    }

    /// GET /api/auditoria/relatorio/classes/{classe}
    [HttpGet("relatorio/classes/{classe}")]
    public async Task<ActionResult<RelatorioDeAuditoria>> ObterRelatorioPorClasseAsync(string classe, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<ClasseDeHeroi>(classe, ignoreCase: true, out var classeParsed))
        {
            return NotFound(new { titulo = "Classe desconhecida", mensagem = $"Classe '{classe}' não faz parte do catálogo de 20 classes." });
        }

        var relatorio = await servico.GerarRelatorioAsync(filtroClasse: classeParsed, cancellationToken);
        return Ok(relatorio);
    }

    /// GET /api/auditoria/cobertura — atalho para o resumo (contagens de OK/Parcial/Faltando + Assets).
    [HttpGet("cobertura")]
    public async Task<ActionResult<ResumoDeAuditoria>> ObterCoberturaAsync(CancellationToken cancellationToken)
    {
        var relatorio = await servico.GerarRelatorioAsync(filtroClasse: null, cancellationToken);
        return Ok(relatorio.Resumo);
    }
}
