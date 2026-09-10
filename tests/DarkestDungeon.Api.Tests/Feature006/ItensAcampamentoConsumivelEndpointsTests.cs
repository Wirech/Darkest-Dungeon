using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Itens;
using DarkestDungeon.Application.Midias;
using DarkestDungeon.Domain.Itens;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature006;

public sealed class ItensAcampamentoConsumivelEndpointsTests : IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory factory;
    private readonly HttpClient client;

    public ItensAcampamentoConsumivelEndpointsTests(ApiTestFactory factory)
    {
        this.factory = factory;
        client = factory.CreateClient();
    }

    [Fact]
    public async Task POST_item_acampamento_valido_retorna_201()
    {
        var response = await client.PostAsJsonAsync("/itens-acampamento", new CriarItemSimplesRequest(
            $"Tocha {Guid.NewGuid():N}", "torch", "Ilumina o corredor."));
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var detalhe = await response.Content.ReadFromJsonAsync<ItemDetalheDto>();
        detalhe!.Tipo.Should().Be(TipoDeItemDto.ItemDeAcampamento);
        detalhe.Midia!.Status.Should().Be("Pendente");
    }

    [Fact]
    public async Task POST_consumivel_nome_vazio_retorna_400_PtBr()
    {
        var response = await client.PostAsJsonAsync("/consumiveis", new CriarItemSimplesRequest("", "potion", "desc"));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).Should().Contain("Nome");
    }

    [Fact]
    public async Task GET_apos_criar_devolve_discriminador()
    {
        var criar = await client.PostAsJsonAsync("/consumiveis", new CriarItemSimplesRequest(
            $"Poção {Guid.NewGuid():N}", "potion", "Cura."));
        var criado = await criar.Content.ReadFromJsonAsync<ItemDetalheDto>();
        var detalhe = await client.GetFromJsonAsync<ItemDetalheDto>($"/itens/{criado!.Id}");
        detalhe!.Tipo.Should().Be(TipoDeItemDto.Consumivel);
    }

    [Fact]
    public async Task POST_publicacao_acessorio_cria_extra_sem_alterar_seed()
    {
        var hash = InventarioDeMidiasTestHelper.HashPng;
        var trinket = $"lucky_test_amulet_{Guid.NewGuid():N}";
        var inventario = new InventarioDeMidiasDeEquipamento(
            "fixture",
            "decl",
            [
                new ArquivoDeInventarioDeMidia(
                    "Acessorio",
                    hash,
                    $@"C:\dd\inventory\trinkets\{trinket}.png",
                    $"arquivos/acessorio/{trinket}.png",
                    InventarioDeMidiasTestHelper.PngMinimo.LongLength,
                    false,
                    null,
                    trinket),
            ],
            []);
        var http = InventarioDeMidiasTestHelper.ClienteComLeitor(factory, new LeitorDeInventarioFixo(inventario));
        var seed = await http.PostAsJsonAsync("/acessorios", new CriarAcessorioRequest(
            $"Ancestral {Guid.NewGuid():N}",
            "ancestral_trinket",
            "Seed",
            RaridadeDeAcessorio.Rara,
            null,
            null,
            [new EfeitoDeAcessorioRequest("Precisão", 5m, UnidadeDeEfeitoDeAcessorio.Percentual, SinalDeEfeito.Positivo)]));
        var ancestral = await seed.Content.ReadFromJsonAsync<ItemDetalheDto>();
        var pub = await http.PostAsJsonAsync("/api/midias/publicacao", new { categoria = "acessorio" });
        pub.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var json = await pub.Content.ReadFromJsonAsync<JsonElement>();
        var publicacaoId = json.GetProperty("publicacaoId").GetGuid();
        var status = await http.GetFromJsonAsync<JsonElement>($"/api/midias/publicacao/{publicacaoId}");
        status.GetProperty("acessoriosNovos").GetInt32().Should().BeGreaterThanOrEqualTo(1);

        var depois = await http.GetFromJsonAsync<ItemDetalheDto>($"/itens/{ancestral!.Id}");
        depois!.Raridade.Should().Be(RaridadeDeAcessorio.Rara);
        depois.EfeitosAcessorio.Should().ContainSingle();
    }
}
