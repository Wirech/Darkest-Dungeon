using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Itens;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Itens;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests;

public class ItensEndpointsTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public ItensEndpointsTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    private static NivelDeArmaRequest[] CincoNiveisArma() =>
        Enumerable.Range(1, 5).Select(n => new NivelDeArmaRequest(n, 5 + n, 8 + n, 4m + n, 3 + n)).ToArray();

    private static NivelDeArmaduraRequest[] CincoNiveisArmadura() =>
        Enumerable.Range(1, 5).Select(n => new NivelDeArmaduraRequest(n, 10 * n, 5m + n)).ToArray();

    [Fact]
    public async Task POST_arma_com_dados_validos_deve_retornar_201()
    {
        var request = new CriarArmaRequest(
            $"Espada Cruzada {Guid.NewGuid():N}",
            "Crusader Sword",
            "Espada padrão do Cruzado.",
            ClasseDeHeroi.Cruzado,
            CincoNiveisArma());

        var response = await client.PostAsJsonAsync("/armas", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var detalhe = await response.Content.ReadFromJsonAsync<ItemDetalheDto>();
        detalhe!.Tipo.Should().Be(TipoDeItemDto.Arma);
        detalhe.NiveisArma.Should().HaveCount(5);
    }

    [Fact]
    public async Task POST_arma_com_menos_de_cinco_niveis_deve_retornar_400()
    {
        var request = new CriarArmaRequest(
            $"Arma Inválida {Guid.NewGuid():N}",
            "Invalid",
            "Descrição",
            ClasseDeHeroi.Cruzado,
            CincoNiveisArma().Take(3).ToArray());

        var response = await client.PostAsJsonAsync("/armas", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task POST_armadura_com_dados_validos_deve_retornar_201()
    {
        var request = new CriarArmaduraRequest(
            $"Placa Veterana {Guid.NewGuid():N}",
            "Veteran Plate",
            "Armadura padrão.",
            ClasseDeHeroi.Veterano,
            CincoNiveisArmadura());

        var response = await client.PostAsJsonAsync("/armaduras", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task POST_acessorio_com_dados_validos_deve_retornar_201()
    {
        var request = new CriarAcessorioRequest(
            $"Amuleto {Guid.NewGuid():N}",
            "Amulet",
            "Amuleto místico.",
            RaridadeDeAcessorio.Rara,
            ClasseExclusiva: null,
            ConjuntoId: null,
            Efeitos: new[] { new EfeitoDeAcessorioRequest("Precisão", 5m, UnidadeDeEfeitoDeAcessorio.Percentual, SinalDeEfeito.Positivo) });

        var response = await client.PostAsJsonAsync("/acessorios", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GET_item_por_id_inexistente_deve_retornar_404()
    {
        var response = await client.GetAsync($"/itens/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GET_item_por_id_valido_deve_retornar_ItemDetalhe()
    {
        var criar = new CriarArmaRequest(
            $"Espada {Guid.NewGuid():N}",
            "Sword",
            "Descrição",
            ClasseDeHeroi.Cruzado,
            CincoNiveisArma());
        var criado = await client.PostAsJsonAsync("/armas", criar);
        var arma = await criado.Content.ReadFromJsonAsync<ItemDetalheDto>();

        var response = await client.GetAsync($"/itens/{arma!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var detalhe = await response.Content.ReadFromJsonAsync<ItemDetalheDto>();
        detalhe!.Id.Should().Be(arma.Id);
        detalhe.NiveisArma.Should().HaveCount(5);
    }
}
