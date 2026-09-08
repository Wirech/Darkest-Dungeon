using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts;
using DarkestDungeon.Api.Tests.Fixtures;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests;

public class SeresValidationTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public SeresValidationTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task CriarSer_ComHpAtualMaiorQueMaximo_DeveRetornar400()
    {
        var request = SeresEndpointsTests.CriarRequest() with { HpMaximo = 10, HpAtual = 20 };

        var response = await client.PostAsJsonAsync("/seres", request);

        await AssertBadRequest(response, "HP Atual deve ser menor ou igual ao HP máximo.");
    }

    [Fact]
    public async Task CriarSer_ComRangeDeDanoInvalido_DeveRetornar400()
    {
        var request = SeresEndpointsTests.CriarRequest() with { DanoBaseMinimo = 8, DanoBaseMaximo = 4 };

        var response = await client.PostAsJsonAsync("/seres", request);

        await AssertBadRequest(response, "Dano base mínimo deve ser menor ou igual ao Dano base máximo.");
    }

    [Fact]
    public async Task CriarSer_ComResistenciaForaDaFaixa_DeveRetornar400()
    {
        var request = SeresEndpointsTests.CriarRequest() with
        {
            Resistencias = SeresEndpointsTests.CriarRequest().Resistencias with { Atordoamento = 120 }
        };

        var response = await client.PostAsJsonAsync("/seres", request);

        await AssertBadRequest(response, "Resistência de Atordoamento deve estar entre 0 e 100.");
    }

    [Fact]
    public async Task CriarSer_SemNome_DeveRetornar400()
    {
        var request = SeresEndpointsTests.CriarRequest() with { Nome = "   " };

        var response = await client.PostAsJsonAsync("/seres", request);

        await AssertBadRequest(response, "Nome deve ser informado.");
    }

    private static async Task AssertBadRequest(HttpResponseMessage response, string mensagemEsperada)
    {
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var erro = await response.Content.ReadFromJsonAsync<ErroResponse>();
        erro.Should().NotBeNull();
        var mensagens = new[] { erro!.Mensagem }.Concat(erro.Erros?.Select(item => item.Mensagem) ?? Array.Empty<string>());
        mensagens.Should().Contain(mensagemEsperada);
        erro.Erros.Should().NotBeNullOrEmpty();
    }
}