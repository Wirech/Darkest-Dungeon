using System.Net;
using System.Text.Json;
using DarkestDungeon.Api.Tests.Fixtures;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests;

public class ContratoOpenApiTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public ContratoOpenApiTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Theory]
    [InlineData("/classes")]
    [InlineData("/habilidades")]
    [InlineData("/mapa-de-cobertura")]
    public async Task Endpoints_publicos_devem_responder_para_GET(string path)
    {
        var response = await client.GetAsync(path);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("/personagens/00000000-0000-0000-0000-000000000000")]
    [InlineData("/inimigos/00000000-0000-0000-0000-000000000000")]
    [InlineData("/itens/00000000-0000-0000-0000-000000000000")]
    public async Task Endpoints_por_id_devem_retornar_404_para_id_inexistente(string path)
    {
        var response = await client.GetAsync(path);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
