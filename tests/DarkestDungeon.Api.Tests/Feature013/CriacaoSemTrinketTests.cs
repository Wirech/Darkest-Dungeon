using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Personagens;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature013;

public sealed class CriacaoSemTrinketTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public CriacaoSemTrinketTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task POST_personagens_ignora_trinkets_e_ficha_efetiva_igual_a_base()
    {
        var request = Feature013Helpers.Personagem(hp: 33);
        var response = await client.PostAsJsonAsync("/personagens", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var detalhe = await response.Content.ReadFromJsonAsync<PersonagemDetalheDto>();

        detalhe!.EspacoTrinket1!.AcessorioId.Should().BeNull();
        detalhe.EspacoTrinket2!.AcessorioId.Should().BeNull();
        detalhe.FichaBase.Should().NotBeNull();
        detalhe.FichaEfetiva.Should().BeEquivalentTo(detalhe.FichaBase);
        detalhe.HpMaximo.Should().Be(request.HpMaximo);
        detalhe.FichaBase!.HpMaximo.Should().Be(request.HpMaximo);
    }
}
