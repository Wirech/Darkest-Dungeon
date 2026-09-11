using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Personagens;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature013;

public sealed class TrocaTrinketAtomicaTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public TrocaTrinketAtomicaTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Falha_no_meio_da_troca_preserva_o_par_anterior()
    {
        var personagem = await Feature013Helpers.CriarPersonagemAsync(client);
        var a = await Feature013Helpers.CriarAcessorioAsync(client, Feature013Helpers.Acessorio("Atômico A 013"));
        var b = await Feature013Helpers.CriarAcessorioAsync(client, Feature013Helpers.Acessorio("Atômico B 013"));
        await Feature013Helpers.EquiparEspacoAsync(client, personagem.Id, 1, a.Id);
        await Feature013Helpers.EquiparEspacoAsync(client, personagem.Id, 2, b.Id);

        var falha = await client.PutAsJsonAsync(
            $"/personagens/{personagem.Id}/acessorios/2",
            new EquiparAcessorioNoEspacoRequest(a.Id));
        falha.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var detalhe = await client.GetFromJsonAsync<PersonagemDetalheDto>($"/personagens/{personagem.Id}");
        detalhe!.EspacoTrinket1!.AcessorioId.Should().Be(a.Id);
        detalhe.EspacoTrinket2!.AcessorioId.Should().Be(b.Id);
    }
}
