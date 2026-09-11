using System.Net;
using DarkestDungeon.Api.Tests.Fixtures;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature010;

public sealed class AcervoEstaticoTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public AcervoEstaticoTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Retrato_existente_deve_retornar_png()
    {
        var response = await client.GetAsync(AcervoDoCardTestHelper.UrlRetratoCruzadoA);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("image/png");
    }

    [Fact]
    public async Task Caminho_inexistente_deve_retornar_404()
    {
        var response = await client.GetAsync("/acervo/herois/arquivos/inexistente/nao-existe.png");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
