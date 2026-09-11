using System.Net;
using DarkestDungeon.Api.Tests.Fixtures;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature012;

public sealed class AcervoSpineEstaticoTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public AcervoSpineEstaticoTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Atlas_idle_do_cruzado_deve_retornar_texto()
    {
        var response = await client.GetAsync("/acervo/herois/arquivos/cruzado/anim/crusader.sprite.idle.atlas");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("text/plain");
    }

    [Fact]
    public async Task Skel_idle_do_cruzado_deve_retornar_octet_stream()
    {
        var response = await client.GetAsync("/acervo/herois/arquivos/cruzado/anim/crusader.sprite.idle.skel");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/octet-stream");
    }

    [Fact]
    public async Task Png_idle_da_paleta_deve_retornar_png()
    {
        var response = await client.GetAsync("/acervo/herois/arquivos/cruzado/crusader_A/anim/crusader.sprite.idle.png");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType!.MediaType.Should().Be("image/png");
    }

    [Fact]
    public async Task Atlas_walk_skel_e_png_da_paleta_devem_existir()
    {
        var atlas = await client.GetAsync("/acervo/herois/arquivos/cruzado/anim/crusader.sprite.walk.atlas");
        atlas.StatusCode.Should().Be(HttpStatusCode.OK);
        atlas.Content.Headers.ContentType!.MediaType.Should().Be("text/plain");

        var skel = await client.GetAsync("/acervo/herois/arquivos/cruzado/anim/crusader.sprite.walk.skel");
        skel.StatusCode.Should().Be(HttpStatusCode.OK);
        skel.Content.Headers.ContentType!.MediaType.Should().Be("application/octet-stream");

        var png = await client.GetAsync("/acervo/herois/arquivos/cruzado/crusader_A/anim/crusader.sprite.walk.png");
        png.StatusCode.Should().Be(HttpStatusCode.OK);
        png.Content.Headers.ContentType!.MediaType.Should().Be("image/png");
    }

    [Fact]
    public async Task Caminho_spine_inexistente_deve_retornar_404()
    {
        var response = await client.GetAsync("/acervo/herois/arquivos/inexistente/nao.skel");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
