using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Personagens;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature013;

public sealed class PersonagensEquiparNaoZeraTrinketsTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public PersonagensEquiparNaoZeraTrinketsTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task POST_equipar_sem_acessoriosIds_nao_zera_espacos()
    {
        var personagem = await Feature013Helpers.CriarPersonagemAsync(client);
        var a = await Feature013Helpers.CriarAcessorioAsync(client, Feature013Helpers.Acessorio("Não zera 013"));
        await Feature013Helpers.EquiparEspacoAsync(client, personagem.Id, 1, a.Id);

        var response = await client.PostAsJsonAsync(
            $"/personagens/{personagem.Id}/equipar",
            new { armaId = (Guid?)null, armaduraId = (Guid?)null });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var detalhe = await response.Content.ReadFromJsonAsync<PersonagemDetalheDto>();
        detalhe!.EspacoTrinket1!.AcessorioId.Should().Be(a.Id);
    }
}
