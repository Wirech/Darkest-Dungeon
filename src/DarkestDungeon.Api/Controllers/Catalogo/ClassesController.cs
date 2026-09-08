using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Application.Classes;
using DarkestDungeon.Application.Habilidades;
using Microsoft.AspNetCore.Mvc;

namespace DarkestDungeon.Api.Controllers.Catalogo;

[Route("classes")]
public sealed class ClassesController : EntidadeControllerBase
{
    private readonly IClasseService service;

    public ClassesController(IClasseService service)
    {
        this.service = service;
    }

    [HttpGet]
    public async Task<ActionResult> Listar(CancellationToken cancellationToken) =>
        MapearResultado(await service.ListarAsync(cancellationToken), valor => valor);

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> Obter(Guid id, CancellationToken cancellationToken) =>
        MapearResultado(await service.ObterAsync(id, cancellationToken), valor => valor);

    [HttpGet("{id:guid}/habilidades")]
    public async Task<ActionResult> ObterHabilidades(Guid id, CancellationToken cancellationToken) =>
        MapearResultado(await service.ObterHabilidadesAsync(id, cancellationToken), valor => valor);
}
