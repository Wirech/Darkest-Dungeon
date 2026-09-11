using DarkestDungeon.Api.Contracts;
using DarkestDungeon.Application.Validation;
using Microsoft.AspNetCore.Mvc;

namespace DarkestDungeon.Api.Controllers;

[ApiController]
public abstract class EntidadeControllerBase : ControllerBase
{
    protected ActionResult MapearResultado<T>(ResultadoOperacao<T> resultado, Func<T, object> mapearSucesso)
    {
        return resultado.Status switch
        {
            StatusOperacao.Sucesso when resultado.Valor is not null => Ok(mapearSucesso(resultado.Valor)),
            StatusOperacao.EntradaInvalida => BadRequest(CriarErro(resultado)),
            StatusOperacao.NaoEncontrado => NotFound(new ErroResponse(resultado.Mensagem)),
            StatusOperacao.FalhaPersistencia => StatusCode(StatusCodes.Status500InternalServerError, new ErroResponse(resultado.Mensagem)),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new ErroResponse("Erro inesperado."))
        };
    }

    protected ActionResult MapearCriacao<T>(ResultadoOperacao<T> resultado, Func<T, object> mapearSucesso, Func<T, string> criarLocation)
    {
        return resultado.Status switch
        {
            StatusOperacao.Sucesso when resultado.Valor is not null => Created(criarLocation(resultado.Valor), mapearSucesso(resultado.Valor)),
            StatusOperacao.EntradaInvalida => BadRequest(CriarErro(resultado)),
            StatusOperacao.NaoEncontrado => NotFound(new ErroResponse(resultado.Mensagem)),
            StatusOperacao.FalhaPersistencia => StatusCode(StatusCodes.Status500InternalServerError, new ErroResponse(resultado.Mensagem)),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new ErroResponse("Erro inesperado."))
        };
    }

    protected ActionResult MapearExclusao(ResultadoOperacao<bool> resultado)
    {
        return resultado.Status switch
        {
            StatusOperacao.Sucesso when resultado.Valor == true => NoContent(),
            StatusOperacao.NaoEncontrado => NotFound(new ErroResponse(resultado.Mensagem)),
            StatusOperacao.EntradaInvalida => BadRequest(CriarErro(resultado)),
            StatusOperacao.FalhaPersistencia => StatusCode(StatusCodes.Status500InternalServerError, new ErroResponse(resultado.Mensagem)),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new ErroResponse("Erro inesperado."))
        };
    }

    private static ErroResponse CriarErro<T>(ResultadoOperacao<T> resultado)
    {
        var erros = resultado.Erros
            .Select(erro => new ErroCampoResponse(erro.Campo, erro.Mensagem))
            .ToArray();

        return new ErroResponse(resultado.Mensagem, erros.Length == 0 ? null : erros);
    }
}