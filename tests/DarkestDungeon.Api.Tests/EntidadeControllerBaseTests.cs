using DarkestDungeon.Api.Contracts;
using DarkestDungeon.Api.Controllers;
using DarkestDungeon.Application.Validation;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace DarkestDungeon.Api.Tests;

public class EntidadeControllerBaseTests
{
    private readonly ControllerDeTeste controller = new();

    [Fact]
    public void MapearResultado_ComNaoEncontrado_DeveRetornar404ComMensagemPtBr()
    {
        var resultado = ResultadoOperacao<object>.NaoEncontrado("Ser não encontrado.");

        var response = controller.Mapear(resultado);

        var notFound = response.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFound.StatusCode.Should().Be(404);
        notFound.Value.Should().BeEquivalentTo(new ErroResponse("Ser não encontrado."));
    }

    [Fact]
    public void MapearCriacao_ComNaoEncontrado_DeveRetornar404ComMensagemPtBr()
    {
        var resultado = ResultadoOperacao<object>.NaoEncontrado("Não foi possível localizar arma e armadura elegíveis para a classe 'Cruzado'.");

        var response = controller.MapearCriacaoPublico(resultado);

        var notFound = response.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFound.StatusCode.Should().Be(404);
        notFound.Value.Should().BeEquivalentTo(new ErroResponse("Não foi possível localizar arma e armadura elegíveis para a classe 'Cruzado'."));
    }

    [Fact]
    public void MapearResultado_ComEntradaInvalida_DeveRetornar400ComErrosPtBr()
    {
        var resultado = ResultadoOperacao<object>.Invalido(
            "Identificador inválido.",
            new ErroOperacao("id", "Identificador inválido."));

        var response = controller.Mapear(resultado);

        var badRequest = response.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.StatusCode.Should().Be(400);
        badRequest.Value.Should().BeEquivalentTo(new ErroResponse(
            "Identificador inválido.",
            new[] { new ErroCampoResponse("id", "Identificador inválido.") }));
    }

    private sealed class ControllerDeTeste : EntidadeControllerBase
    {
        public ActionResult Mapear(ResultadoOperacao<object> resultado)
        {
            return MapearResultado(resultado, valor => valor);
        }

        public ActionResult MapearCriacaoPublico(ResultadoOperacao<object> resultado)
        {
            return MapearCriacao(resultado, valor => valor, _ => "/personagens");
        }
    }
}