using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Itens;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Itens;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature006;

public sealed class CoberturaMidiasEndpointsTests : IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory factory;
    private readonly HttpClient client;

    public CoberturaMidiasEndpointsTests(ApiTestFactory factory)
    {
        this.factory = factory;
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_cobertura_retorna_cinco_categorias_e_soma_esperados()
    {
        var response = await client.GetAsync("/api/midias/cobertura");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var categorias = json.GetProperty("categorias");
        categorias.GetArrayLength().Should().Be(5);
        var nomes = categorias.EnumerateArray().Select(c => c.GetProperty("categoria").GetString()).ToArray();
        nomes.Should().Contain(["Arma", "Armadura", "Acessório", "Item de acampamento/provisão", "Consumível"]);
        nomes.Should().NotContain("NaoAssociado");
        foreach (var cat in categorias.EnumerateArray())
        {
            var esperados = cat.GetProperty("esperados").GetInt32();
            var soma = cat.GetProperty("ok").GetInt32() + cat.GetProperty("parcial").GetInt32() + cat.GetProperty("pendente").GetInt32();
            soma.Should().Be(esperados);
        }

        json.GetProperty("categorias")[0].GetProperty("esperados").GetInt32().Should().BeGreaterThanOrEqualTo(20);
    }

    [Fact]
    public async Task GET_cobertura_query_invalida_400_PtBr()
    {
        var response = await client.GetAsync("/api/midias/cobertura?categoria=inimigo");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).Should().Contain("Categoria inválida");
    }

    [Fact]
    public async Task GET_cobertura_path_invalido_404()
    {
        var response = await client.GetAsync("/api/midias/cobertura/categorias/inimigo");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GET_cobertura_arma_parcial_e_ok()
    {
        await client.PostAsJsonAsync("/armas", new CriarArmaRequest(
            $"Espada {Guid.NewGuid():N}", "Sword", "Descrição", ClasseDeHeroi.Cruzado, CincoNiveisArma()));
        using var pasta = new TemporaryDirectory();
        var inventario = InventarioDeMidiasTestHelper.GravarInventario(pasta.Path, [
            InventarioDeMidiasTestHelper.Arquivo("Arma", @"C:\dd\heroes\crusader\crusader_weapon_1.png", "arquivos/arma/w1.png", classe: "crusader"),
            InventarioDeMidiasTestHelper.Arquivo("Arma", @"C:\dd\heroes\crusader\crusader_weapon_2.png", "arquivos/arma/w2.png", classe: "crusader"),
            InventarioDeMidiasTestHelper.Arquivo("NaoAssociado", @"C:\dd\inventory\misc\orphan.png", "arquivos/naoassociado/orphan.png"),
        ]);
        var http = InventarioDeMidiasTestHelper.ClienteComInventario(factory, inventario);
        (await http.PostAsJsonAsync("/api/midias/publicacao", new { categoria = "arma" })).EnsureSuccessStatusCode();

        var response = await http.GetAsync("/api/midias/cobertura?categoria=arma");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        json.GetProperty("categorias").GetArrayLength().Should().Be(1);
        var arma = json.GetProperty("categorias")[0];
        arma.GetProperty("parcial").GetInt32().Should().BeGreaterThan(0);
        json.GetProperty("orfaos").GetArrayLength().Should().BeGreaterThan(0);
        json.GetProperty("orfaos")[0].GetProperty("motivo").GetString().Should().Contain("fora das origens");
    }

    [Fact]
    public async Task GET_cobertura_acessorio_esperados_inclui_novos_e_orfaos()
    {
        await client.PostAsJsonAsync("/acessorios", new CriarAcessorioRequest(
            $"Amuleto {Guid.NewGuid():N}",
            "seed_trinket",
            "Descrição",
            RaridadeDeAcessorio.Rara,
            null,
            null,
            Array.Empty<EfeitoDeAcessorioRequest>()));
        using var pasta = new TemporaryDirectory();
        var inventario = InventarioDeMidiasTestHelper.GravarInventario(pasta.Path, [
            InventarioDeMidiasTestHelper.Arquivo("Acessorio", @"C:\dd\inventory\trinkets\lucky_test_amulet.png", "arquivos/acessorio/lucky_test_amulet.png"),
            InventarioDeMidiasTestHelper.Arquivo("NaoAssociado", @"C:\dd\inventory\misc\orphan.png", "arquivos/naoassociado/orphan.png"),
        ]);
        var http = InventarioDeMidiasTestHelper.ClienteComInventario(factory, inventario);
        (await http.PostAsJsonAsync("/api/midias/publicacao", new { categoria = "acessorio" })).EnsureSuccessStatusCode();

        var cobertura = await http.GetFromJsonAsync<JsonElement>("/api/midias/cobertura?categoria=acessorio");
        var cat = cobertura.GetProperty("categorias")[0];
        cat.GetProperty("esperados").GetInt32().Should().BeGreaterThanOrEqualTo(2);
        cobertura.GetProperty("orfaos").EnumerateArray().Should().Contain(o => o.GetProperty("caminhoOrigem").GetString()!.Contains("orphan"));
    }

    private static NivelDeArmaRequest[] CincoNiveisArma() =>
        Enumerable.Range(1, 5).Select(n => new NivelDeArmaRequest(n, 5 + n, 8 + n, 4m + n, 3 + n)).ToArray();
}
