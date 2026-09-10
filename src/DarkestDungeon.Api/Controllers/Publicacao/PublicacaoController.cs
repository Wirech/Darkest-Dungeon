using DarkestDungeon.Application.Publicacao;
using Microsoft.AspNetCore.Mvc;

namespace DarkestDungeon.Api.Controllers.Publicacao;

/// Controller da Feature 005 — publicação atômica no SQL Server.
[ApiController]
[Route("api/publicacao")]
public sealed class PublicacaoController : ControllerBase
{
    private readonly IPublicadorAtomicoService servico;

    public PublicacaoController(IPublicadorAtomicoService servico)
    {
        this.servico = servico ?? throw new ArgumentNullException(nameof(servico));
    }

    /// POST /api/publicacao
    [HttpPost]
    public async Task<IActionResult> PublicarAsync([FromBody] SolicitacaoDePublicacao solicitacao, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await servico.PublicarAsync(solicitacao, cancellationToken);
            if (resultado.Estado == EstadoDePublicacao.Concluida)
            {
                return Accepted($"/api/publicacao/{resultado.PublicacaoId}/status", resultado);
            }

            // Rollback aplicado ⇒ 500 com detalhes
            return StatusCode(500, new
            {
                titulo = "Publicação falhou; rollback aplicado",
                mensagem = resultado.Mensagem,
                publicacaoId = resultado.PublicacaoId,
                campoQueFalhou = resultado.CampoQueFalhou,
                habilidadeAfetada = resultado.HabilidadeAfetada,
            });
        }
        catch (ConfirmacaoAusenteException ex)
        {
            return BadRequest(new { titulo = "Confirmação obrigatória", mensagem = ex.Message });
        }
        catch (SessoesAtivasException ex)
        {
            return BadRequest(new { titulo = "Publicação bloqueada", mensagem = ex.Message, sessoesAtivas = ex.SessoesAtivas });
        }
        catch (PublicacaoEmCursoException ex)
        {
            return Conflict(new { titulo = "Publicação em andamento", mensagem = ex.Message, publicacaoAtivaId = ex.PublicacaoAtivaId });
        }
    }

    /// GET /api/publicacao/{id}/status
    [HttpGet("{publicacaoId:guid}/status")]
    public async Task<IActionResult> ObterStatusAsync(Guid publicacaoId, CancellationToken cancellationToken)
    {
        var resultado = await servico.ObterStatusAsync(publicacaoId, cancellationToken);
        if (resultado is null)
        {
            return NotFound(new { titulo = "Publicação desconhecida", mensagem = $"Nenhum registro para PublicacaoId={publicacaoId}." });
        }

        return Ok(resultado);
    }

    /// GET /api/publicacao/{id}/logs?nivel=Error&campo=niveis[3].modificadorDano
    [HttpGet("{publicacaoId:guid}/logs")]
    public async Task<IActionResult> ObterLogsAsync(Guid publicacaoId, [FromQuery] NivelDeLog? nivel, [FromQuery] string? campo, CancellationToken cancellationToken)
    {
        var logs = await servico.ObterLogsAsync(publicacaoId, nivel, campo, cancellationToken);
        return Ok(new
        {
            publicacaoId,
            total = logs.Count,
            itens = logs.Select(l => new
            {
                l.Timestamp,
                nivel = l.Nivel.ToString(),
                l.HabilidadeId,
                l.NomeExibicao,
                l.Campo,
                l.Mensagem,
                l.StackTrace,
            }),
        });
    }
}
