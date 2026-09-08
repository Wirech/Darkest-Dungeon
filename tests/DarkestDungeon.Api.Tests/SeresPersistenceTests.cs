using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Tests.Fixtures;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests;

public class SeresPersistenceTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public SeresPersistenceTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task ConsultarSer_AposCriacao_DeveManterMesmoIdEAtributos()
    {
        var createResponse = await client.PostAsJsonAsync("/seres", SeresEndpointsTests.CriarRequest());
        var criado = await createResponse.Content.ReadFromJsonAsync<Contracts.SerResponse>();

        var getResponse = await client.GetAsync($"/seres/{criado!.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var consultado = await getResponse.Content.ReadFromJsonAsync<Contracts.SerResponse>();
        consultado!.Id.Should().Be(criado.Id);
        consultado.DanoBaseMinimo.Should().Be(criado.DanoBaseMinimo);
        consultado.Resistencias.Movimento.Should().Be(criado.Resistencias.Movimento);
    }
}