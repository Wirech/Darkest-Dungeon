using DarkestDungeon.Application.Midias;
using Microsoft.AspNetCore.Mvc;

namespace DarkestDungeon.Api.Controllers.Midias;

[ApiController]
[Route("api/midias/publicacao")]
public sealed class MidiasPublicacaoController : ControllerBase
{
    private readonly IPublicadorDeVinculosDeMidia publicador;

    public MidiasPublicacaoController(IPublicadorDeVinculosDeMidia publicador)
    {
        this.publicador = publicador;
    }

    [HttpPost]
    public async Task<IActionResult> PublicarAsync([FromBody] PublicacaoDeVinculosRequest? request, CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await publicador.PublicarAsync(
                new SolicitacaoDePublicacaoDeVinculos(request?.Categoria ?? string.Empty, request?.Observacao),
                cancellationToken);
            return Accepted($"/api/midias/publicacao/{resultado.PublicacaoId}", PublicacaoDeVinculosDtoMapper.Aceita(resultado));
        }
        catch (CategoriaDeMidiaInvalidaException ex)
        {
            return BadRequest(new { titulo = "Categoria inválida", mensagem = ex.Message });
        }
        catch (PublicacaoDeVinculosEmCursoException ex)
        {
            return Conflict(new { titulo = "Publicação em andamento", mensagem = ex.Message, publicacaoId = ex.PublicacaoAtivaId });
        }
        catch (InventarioDeMidiasAusenteException ex)
        {
            return Conflict(new { titulo = "Inventário não encontrado", mensagem = ex.Message });
        }
        catch (PublicacaoDeVinculosFalhouException ex)
        {
            return StatusCode(500, new
            {
                titulo = "Publicação falhou; rollback aplicado",
                mensagem = ex.Message,
                publicacaoId = ex.PublicacaoId,
                categoria = ex.Categoria.ToString(),
            });
        }
    }

    [HttpGet("{publicacaoId:guid}")]
    public async Task<IActionResult> ObterStatusAsync(Guid publicacaoId, CancellationToken cancellationToken)
    {
        var resultado = await publicador.ObterStatusAsync(publicacaoId, cancellationToken);
        if (resultado is null)
        {
            return NotFound(new { titulo = "Publicação não encontrada", mensagem = "Não há publicação de vínculos com o identificador informado." });
        }

        return Ok(PublicacaoDeVinculosDtoMapper.Status(resultado));
    }
}
