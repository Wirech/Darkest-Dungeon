using DarkestDungeon.Api.Contracts;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Domain.Classes;
using Microsoft.AspNetCore.Mvc;

namespace DarkestDungeon.Api.Controllers.Catalogo;

[Route("itens")]
public sealed class ItensController : EntidadeControllerBase
{
    private readonly IItemService service;

    public ItensController(IItemService service)
    {
        this.service = service;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> Obter(Guid id, CancellationToken cancellationToken) =>
        MapearResultado(await service.ObterAsync(id, cancellationToken), valor => valor);
}

[Route("armas")]
public sealed class ArmasController : EntidadeControllerBase
{
    private readonly IItemService service;

    public ArmasController(IItemService service)
    {
        this.service = service;
    }

    [HttpPost]
    public async Task<ActionResult> Criar([FromBody] CriarArmaRequest request, CancellationToken cancellationToken) =>
        MapearCriacao(await service.CriarArmaAsync(request.ParaCommand(), cancellationToken), v => v, v => $"/itens/{v.Id}");
}

[Route("armaduras")]
public sealed class ArmadurasController : EntidadeControllerBase
{
    private readonly IItemService service;

    public ArmadurasController(IItemService service)
    {
        this.service = service;
    }

    [HttpPost]
    public async Task<ActionResult> Criar([FromBody] CriarArmaduraRequest request, CancellationToken cancellationToken) =>
        MapearCriacao(await service.CriarArmaduraAsync(request.ParaCommand(), cancellationToken), v => v, v => $"/itens/{v.Id}");
}

[Route("acessorios")]
public sealed class AcessoriosController : EntidadeControllerBase
{
    private readonly IItemService service;

    public AcessoriosController(IItemService service)
    {
        this.service = service;
    }

    [HttpGet]
    public async Task<ActionResult> Listar([FromQuery] ClasseDeHeroi? classe, [FromQuery] Guid? excluirId, CancellationToken cancellationToken)
    {
        if (classe is null || !Enum.IsDefined(classe.Value))
        {
            return BadRequest(new ErroResponse(
                "Classe inválida.",
                new[] { new ErroCampoResponse("classe", "Informe uma classe válida.") }));
        }

        return MapearResultado(await service.ListarAcessoriosAsync(classe.Value, excluirId, cancellationToken), v => v);
    }

    [HttpPost]
    public async Task<ActionResult> Criar([FromBody] CriarAcessorioRequest request, CancellationToken cancellationToken) =>
        MapearCriacao(await service.CriarAcessorioAsync(request.ParaCommand(), cancellationToken), v => v, v => $"/itens/{v.Id}");
}

[Route("itens-acampamento")]
public sealed class ItensDeAcampamentoController : EntidadeControllerBase
{
    private readonly IItemService service;

    public ItensDeAcampamentoController(IItemService service)
    {
        this.service = service;
    }

    [HttpPost]
    public async Task<ActionResult> Criar([FromBody] CriarItemSimplesRequest request, CancellationToken cancellationToken) =>
        MapearCriacao(await service.CriarItemDeAcampamentoAsync(request.ParaCommand(), cancellationToken), v => v, v => $"/itens/{v.Id}");
}

[Route("consumiveis")]
public sealed class ConsumiveisController : EntidadeControllerBase
{
    private readonly IItemService service;

    public ConsumiveisController(IItemService service)
    {
        this.service = service;
    }

    [HttpPost]
    public async Task<ActionResult> Criar([FromBody] CriarItemSimplesRequest request, CancellationToken cancellationToken) =>
        MapearCriacao(await service.CriarConsumivelAsync(request.ParaCommand(), cancellationToken), v => v, v => $"/itens/{v.Id}");
}
