using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Itens;
using DarkestDungeon.Domain.Classes;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature013;

public sealed class AcessoriosListaFiltradaTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public AcessoriosListaFiltradaTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_acessorios_lista_irrestritos_e_exclusivos_da_classe()
    {
        var irrestrito = await Feature013Helpers.CriarAcessorioAsync(
            client,
            Feature013Helpers.Acessorio("Anel Livre 013"));
        var cruzado = await Feature013Helpers.CriarAcessorioAsync(
            client,
            Feature013Helpers.Acessorio("Anel Cruzado 013", ClasseDeHeroi.Cruzado));
        var ocultista = await Feature013Helpers.CriarAcessorioAsync(
            client,
            Feature013Helpers.Acessorio("Anel Ocultista 013", ClasseDeHeroi.Ocultista));

        var response = await client.GetAsync($"/acessorios?classe={ClasseDeHeroi.Cruzado}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var lista = await response.Content.ReadFromJsonAsync<IReadOnlyList<ItemDetalheDto>>();
        lista.Should().NotBeNull();
        lista.Should().Contain(i => i.Id == irrestrito.Id);
        lista.Should().Contain(i => i.Id == cruzado.Id);
        lista.Should().NotContain(i => i.Id == ocultista.Id);
        lista.Should().OnlyContain(i => i.Tipo == TipoDeItemDto.Acessorio);
    }

    [Fact]
    public async Task GET_acessorios_omite_excluirId_do_outro_espaco()
    {
        var a = await Feature013Helpers.CriarAcessorioAsync(client, Feature013Helpers.Acessorio("Excluir A 013"));
        var b = await Feature013Helpers.CriarAcessorioAsync(client, Feature013Helpers.Acessorio("Excluir B 013"));

        var response = await client.GetAsync($"/acessorios?classe={ClasseDeHeroi.Cruzado}&excluirId={a.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var lista = await response.Content.ReadFromJsonAsync<IReadOnlyList<ItemDetalheDto>>();
        lista.Should().NotBeNull();
        lista.Should().NotContain(i => i.Id == a.Id);
        lista.Should().Contain(i => i.Id == b.Id);
    }

    [Fact]
    public async Task GET_acessorios_classe_invalida_retorna_400()
    {
        var response = await client.GetAsync("/acessorios?classe=NaoExiste");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GET_acessorios_retorna_200_com_array()
    {
        var response = await client.GetAsync($"/acessorios?classe={ClasseDeHeroi.Duelista}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var lista = await response.Content.ReadFromJsonAsync<IReadOnlyList<ItemDetalheDto>>();
        lista.Should().NotBeNull();
    }
}
