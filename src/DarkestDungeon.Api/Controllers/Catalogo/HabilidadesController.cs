using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Application.Habilidades;
using Microsoft.AspNetCore.Mvc;

namespace DarkestDungeon.Api.Controllers.Catalogo;

[Route("habilidades")]
public sealed class HabilidadesController : EntidadeControllerBase
{
    private readonly IHabilidadeService service;

    public HabilidadesController(IHabilidadeService service)
    {
        this.service = service;
    }

    [HttpGet]
    public async Task<ActionResult> Listar([FromQuery] CategoriaDeHabilidadeDto? categoria, [FromQuery] Guid? classe, CancellationToken cancellationToken) =>
        MapearResultado(await service.ListarAsync(categoria, classe, cancellationToken), valor => valor);

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> Obter(Guid id, CancellationToken cancellationToken) =>
        MapearResultado(await service.ObterAsync(id, cancellationToken), valor => valor);

    [HttpPost("combate")]
    public async Task<ActionResult> CriarCombate([FromBody] CriarHabilidadeDeCombateRequest request, CancellationToken cancellationToken) =>
        MapearCriacao(await service.CriarDeCombateAsync(request.ParaCommand(), cancellationToken), valor => valor, valor => $"/habilidades/{valor.Id}");

    [HttpPost("acampamento")]
    public async Task<ActionResult> CriarAcampamento([FromBody] CriarHabilidadeDeAcampamentoRequest request, CancellationToken cancellationToken) =>
        MapearCriacao(await service.CriarDeAcampamentoAsync(request.ParaCommand(), cancellationToken), valor => valor, valor => $"/habilidades/{valor.Id}");

    [HttpPost("inimigo")]
    public async Task<ActionResult> CriarInimigo([FromBody] CriarHabilidadeDeInimigoRequest request, CancellationToken cancellationToken) =>
        MapearCriacao(await service.CriarDeInimigoAsync(request.ParaCommand(), cancellationToken), valor => valor, valor => $"/habilidades/{valor.Id}");
}
