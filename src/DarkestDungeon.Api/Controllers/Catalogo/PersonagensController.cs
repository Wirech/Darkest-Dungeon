using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Contracts;
using DarkestDungeon.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace DarkestDungeon.Api.Controllers.Catalogo;

[Route("personagens")]
public sealed class PersonagensController : EntidadeControllerBase
{
    private readonly IPersonagemService service;

    public PersonagensController(IPersonagemService service)
    {
        this.service = service;
    }

    [HttpGet]
    public async Task<ActionResult> Listar(CancellationToken cancellationToken) =>
        MapearResultado(await service.ListarAsync(cancellationToken), valor => valor);

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> Obter(Guid id, CancellationToken cancellationToken) =>
        MapearResultado(await service.ObterAsync(id, cancellationToken), v => v);

    [HttpPost]
    public async Task<ActionResult> Criar([FromBody] CriarPersonagemRequest request, CancellationToken cancellationToken) =>
        MapearCriacao(await service.CriarAsync(request.ParaCommand(), cancellationToken), v => v, v => $"/personagens/{v.Id}");

    [HttpPost("{id:guid}/equipar")]
    public async Task<ActionResult> Equipar(Guid id, [FromBody] EquiparPersonagemRequest request, CancellationToken cancellationToken) =>
        MapearResultado(await service.EquiparAsync(request.ParaCommand(id), cancellationToken), v => v);

    [HttpPut("{id:guid}/acessorios/{espaco:int}")]
    public async Task<ActionResult> EquiparEspaco(
        Guid id,
        int espaco,
        [FromBody] EquiparAcessorioNoEspacoRequest request,
        CancellationToken cancellationToken) =>
        MapearResultado(await service.EquiparEspacoAsync(request.ParaCommand(id, espaco), cancellationToken), v => v);

    [HttpDelete("{id}")]
    public async Task<ActionResult> Excluir(string id, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(id, out var idParseado))
        {
            return BadRequest(new ErroResponse(
                "Identificador inválido.",
                new[] { new ErroCampoResponse("id", "Identificador inválido.") }));
        }

        return MapearExclusao(await service.RemoverAsync(idParseado, cancellationToken));
    }
}

[Route("inimigos")]
public sealed class InimigosController : EntidadeControllerBase
{
    private readonly IInimigoService service;

    public InimigosController(IInimigoService service)
    {
        this.service = service;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> Obter(Guid id, CancellationToken cancellationToken) =>
        MapearResultado(await service.ObterAsync(id, cancellationToken), v => v);

    [HttpPost]
    public async Task<ActionResult> Criar([FromBody] CriarInimigoRequest request, CancellationToken cancellationToken) =>
        MapearCriacao(await service.CriarAsync(request.ParaCommand(), cancellationToken), v => v, v => $"/inimigos/{v.Id}");
}
