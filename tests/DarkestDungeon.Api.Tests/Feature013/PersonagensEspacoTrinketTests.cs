using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Itens;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature013;

public sealed class PersonagensEspacoTrinketTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public PersonagensEspacoTrinketTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task PUT_equipe_esvazia_e_mantem_o_outro_espaco()
    {
        var personagem = await Feature013Helpers.CriarPersonagemAsync(client);
        var a = await Feature013Helpers.CriarAcessorioAsync(client, Feature013Helpers.Acessorio("Slot A 013"));
        var b = await Feature013Helpers.CriarAcessorioAsync(client, Feature013Helpers.Acessorio("Slot B 013"));

        var um = await Feature013Helpers.EquiparEspacoAsync(client, personagem.Id, 1, a.Id);
        um.EspacoTrinket1!.AcessorioId.Should().Be(a.Id);
        um.EspacoTrinket2!.AcessorioId.Should().BeNull();

        var dois = await Feature013Helpers.EquiparEspacoAsync(client, personagem.Id, 2, b.Id);
        dois.EspacoTrinket1!.AcessorioId.Should().Be(a.Id);
        dois.EspacoTrinket2!.AcessorioId.Should().Be(b.Id);

        var limpo = await Feature013Helpers.EquiparEspacoAsync(client, personagem.Id, 1, null);
        limpo.EspacoTrinket1!.AcessorioId.Should().BeNull();
        limpo.EspacoTrinket2!.AcessorioId.Should().Be(b.Id);
    }

    [Fact]
    public async Task PUT_duplicata_retorna_400_em_portugues()
    {
        var personagem = await Feature013Helpers.CriarPersonagemAsync(client);
        var a = await Feature013Helpers.CriarAcessorioAsync(client, Feature013Helpers.Acessorio("Dup 013"));
        await Feature013Helpers.EquiparEspacoAsync(client, personagem.Id, 1, a.Id);

        var response = await client.PutAsJsonAsync(
            $"/personagens/{personagem.Id}/acessorios/2",
            new EquiparAcessorioNoEspacoRequest(a.Id));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var erro = await response.Content.ReadFromJsonAsync<ErroResponse>();
        erro!.Mensagem.Should().Contain("mesmo Acessório");
    }

    [Fact]
    public async Task PUT_exclusivo_de_outra_classe_retorna_400()
    {
        var personagem = await Feature013Helpers.CriarPersonagemAsync(client, Feature013Helpers.Personagem(ClasseDeHeroi.Cruzado));
        var exclusivo = await Feature013Helpers.CriarAcessorioAsync(
            client,
            Feature013Helpers.Acessorio("Só Vestal 013", ClasseDeHeroi.Vestal));

        var response = await client.PutAsJsonAsync(
            $"/personagens/{personagem.Id}/acessorios/1",
            new EquiparAcessorioNoEspacoRequest(exclusivo.Id));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var erro = await response.Content.ReadFromJsonAsync<ErroResponse>();
        erro!.Mensagem.Should().Contain("exclusivo");
    }

    [Fact]
    public async Task PUT_trinket_inexistente_retorna_400()
    {
        var personagem = await Feature013Helpers.CriarPersonagemAsync(client);
        var response = await client.PutAsJsonAsync(
            $"/personagens/{personagem.Id}/acessorios/1",
            new EquiparAcessorioNoEspacoRequest(Guid.NewGuid()));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PUT_personagem_inexistente_retorna_404()
    {
        var acessorio = await Feature013Helpers.CriarAcessorioAsync(client, Feature013Helpers.Acessorio("Órfão 013"));
        var response = await client.PutAsJsonAsync(
            $"/personagens/{Guid.NewGuid()}/acessorios/1",
            new EquiparAcessorioNoEspacoRequest(acessorio.Id));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PUT_espaco_invalido_retorna_400()
    {
        var personagem = await Feature013Helpers.CriarPersonagemAsync(client);
        var acessorio = await Feature013Helpers.CriarAcessorioAsync(client, Feature013Helpers.Acessorio("Espaço 013"));
        var response = await client.PutAsJsonAsync(
            $"/personagens/{personagem.Id}/acessorios/3",
            new EquiparAcessorioNoEspacoRequest(acessorio.Id));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var erro = await response.Content.ReadFromJsonAsync<ErroResponse>();
        erro!.Mensagem.Should().Contain("Espaço");
    }
}
