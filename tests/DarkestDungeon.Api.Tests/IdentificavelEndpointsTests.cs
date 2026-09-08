using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts;
using DarkestDungeon.Api.Tests.Fixtures;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests;

public class IdentificavelEndpointsTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public IdentificavelEndpointsTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task ConsultarSer_ComIdInexistente_DeveRetornar404EmPtBr()
    {
        var response = await client.GetAsync($"/seres/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var erro = await response.Content.ReadFromJsonAsync<ErroResponse>();
        erro!.Mensagem.Should().Be("Ser não encontrado.");
    }

    [Fact]
    public async Task ConsultarSer_ComIdInvalido_DeveRetornar400EmPtBr()
    {
        var response = await client.GetAsync("/seres/id-invalido");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var erro = await response.Content.ReadFromJsonAsync<ErroResponse>();
        erro!.Mensagem.Should().Be("Identificador inválido.");
    }
}