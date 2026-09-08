using DarkestDungeon.Api.Contracts;
using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Application.Seres;
using Microsoft.AspNetCore.Mvc;

namespace DarkestDungeon.Api.Controllers;

[Route("seres")]
public sealed class SeresController : EntidadeControllerBase
{
    private readonly ISerService service;

    public SeresController(ISerService service)
    {
        this.service = service;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<SerResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErroResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ObterPorId(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await service.ObterPorIdAsync(id, cancellationToken);
        return MapearResultado(resultado, MapearResponse);
    }

    [HttpGet("{id}")]
    [ProducesResponseType<ErroResponse>(StatusCodes.Status400BadRequest)]
    public ActionResult ObterPorIdInvalido(string id)
    {
        return BadRequest(new ErroResponse(
            "Identificador inválido.",
            new[] { new ErroCampoResponse("id", "Identificador inválido.") }));
    }

    [HttpPost]
    [ProducesResponseType<SerResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErroResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Criar(CriarSerRequest request, CancellationToken cancellationToken)
    {
        var command = new CriarSerCommand(
            request.Nome,
            request.Tipo,
            request.HpMaximo,
            request.HpAtual,
            request.Velocidade,
            request.Critico,
            request.DanoBaseMinimo,
            request.DanoBaseMaximo,
            request.Movimento,
            request.BonusDeCritico,
            request.Tamanho,
            request.AcoesPorTurno,
            request.Esquiva,
            request.Precisao,
            request.Protecao,
            request.Nivel,
            new ResistenciasDto(
                request.Resistencias.Atordoamento,
                request.Resistencias.Sangramento,
                request.Resistencias.Envenenamento,
                request.Resistencias.Debuff,
                request.Resistencias.Movimento));

        var resultado = await service.CriarAsync(command, cancellationToken);
        return MapearCriacao(resultado, MapearResponse, ser => $"/seres/{ser.Id}");
    }

    private static SerResponse MapearResponse(SerDto ser) => new(
        ser.Id,
        ser.Nome,
        ser.Tipo,
        ser.HpMaximo,
        ser.HpAtual,
        ser.Velocidade,
        ser.Critico,
        ser.DanoBaseMinimo,
        ser.DanoBaseMaximo,
        ser.Movimento,
        ser.BonusDeCritico,
        ser.Tamanho,
        ser.AcoesPorTurno,
        ser.Esquiva,
        ser.Precisao,
        ser.Protecao,
        ser.Nivel,
        new ResistenciasResponse(
            ser.Resistencias.Atordoamento,
            ser.Resistencias.Sangramento,
            ser.Resistencias.Envenenamento,
            ser.Resistencias.Debuff,
            ser.Resistencias.Movimento));
}