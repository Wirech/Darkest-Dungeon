using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Tests.Fixtures;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests;

public class SeresConcurrencyTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public SeresConcurrencyTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task ConsultarSer_ComVinteRequisicoesSimultaneas_DeveRetornarConsistente()
    {
        var createResponse = await client.PostAsJsonAsync("/seres", SeresEndpointsTests.CriarRequest());
        var criado = await createResponse.Content.ReadFromJsonAsync<Contracts.SerResponse>();

        var responses = await Task.WhenAll(Enumerable.Range(0, 20).Select(_ => client.GetAsync($"/seres/{criado!.Id}")));

        responses.Should().OnlyContain(response => response.StatusCode == HttpStatusCode.OK);
        var corpos = await Task.WhenAll(responses.Select(response => response.Content.ReadFromJsonAsync<Contracts.SerResponse>()));
        corpos.Should().OnlyContain(ser => ser!.Id == criado!.Id);
    }
}