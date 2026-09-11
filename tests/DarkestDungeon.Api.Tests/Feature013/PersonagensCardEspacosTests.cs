using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Itens;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature013;

public sealed class PersonagensCardEspacosTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public PersonagensCardEspacosTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_lista_e_detalhe_expoe_dois_espacos_posicionais()
    {
        var personagem = await Feature013Helpers.CriarPersonagemAsync(client);
        var a = await Feature013Helpers.CriarAcessorioAsync(client, Feature013Helpers.Acessorio("Card A 013"));
        var b = await Feature013Helpers.CriarAcessorioAsync(client, Feature013Helpers.Acessorio("Card B 013"));
        await Feature013Helpers.EquiparEspacoAsync(client, personagem.Id, 1, a.Id);
        await Feature013Helpers.EquiparEspacoAsync(client, personagem.Id, 2, b.Id);

        var detalheResponse = await client.GetAsync($"/personagens/{personagem.Id}");
        detalheResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var detalhe = await detalheResponse.Content.ReadFromJsonAsync<PersonagemDetalheDto>();
        detalhe!.EspacoTrinket1!.AcessorioId.Should().Be(a.Id);
        detalhe.EspacoTrinket1.NomeExibicao.Should().Be(a.NomeExibicao);
        detalhe.EspacoTrinket1.Raridade.Should().Be(RaridadeDeAcessorio.Comum);
        detalhe.EspacoTrinket2!.AcessorioId.Should().Be(b.Id);
        detalhe.FichaBase.Should().NotBeNull();
        detalhe.FichaEfetiva.Should().NotBeNull();

        var lista = await client.GetFromJsonAsync<IReadOnlyList<PersonagemResumoDto>>("/personagens");
        var resumo = lista.Should().Contain(p => p.Id == personagem.Id).Which;
        resumo.EspacoTrinket1!.AcessorioId.Should().Be(a.Id);
        resumo.EspacoTrinket2!.AcessorioId.Should().Be(b.Id);
        resumo.FichaBase.Should().NotBeNull();
        resumo.FichaEfetiva.Should().NotBeNull();
    }
}
