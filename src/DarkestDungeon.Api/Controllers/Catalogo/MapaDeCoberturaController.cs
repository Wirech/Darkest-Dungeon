using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Application.Cobertura;
using Microsoft.AspNetCore.Mvc;

namespace DarkestDungeon.Api.Controllers.Catalogo;

[Route("mapa-de-cobertura")]
public sealed class MapaDeCoberturaController : EntidadeControllerBase
{
    private readonly IMapaDeCoberturaService service;

    public MapaDeCoberturaController(IMapaDeCoberturaService service)
    {
        this.service = service;
    }

    [HttpGet]
    public async Task<ActionResult> Obter(CancellationToken cancellationToken) =>
        MapearResultado(await service.ObterCompletoAsync(cancellationToken), v => v);

    [HttpGet("{classeId:guid}")]
    public async Task<ActionResult> ObterPorClasse(Guid classeId, CancellationToken cancellationToken) =>
        MapearResultado(await service.ObterPorClasseAsync(classeId, cancellationToken), v => v);

    [HttpPost]
    public async Task<ActionResult> Registrar([FromBody] RegistrarCoberturaCommand command, CancellationToken cancellationToken) =>
        MapearResultado(await service.RegistrarAsync(command, cancellationToken), v => v);
}
