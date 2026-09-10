using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Itens;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Classes;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature006;

public sealed class ItensEPersonagemMidiaTests : IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory factory;
    private readonly HttpClient client;

    public ItensEPersonagemMidiaTests(ApiTestFactory factory)
    {
        this.factory = factory;
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_item_sem_publicacao_devolve_midia_pendente_200()
    {
        var criar = await client.PostAsJsonAsync("/armas", new CriarArmaRequest(
            $"Espada {Guid.NewGuid():N}", "Sword", "Descrição", ClasseDeHeroi.Cruzado, CincoNiveisArma()));
        var item = await criar.Content.ReadFromJsonAsync<ItemDetalheDto>();

        var response = await client.GetAsync($"/itens/{item!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var detalhe = await response.Content.ReadFromJsonAsync<ItemDetalheDto>();
        detalhe!.NiveisArma.Should().HaveCount(5);
        detalhe.NiveisArma!.All(n => n.Midia!.Status == "Pendente").Should().BeTrue();
    }

    [Fact]
    public async Task GET_item_apos_publicacao_hash_bate_com_inventario()
    {
        var criar = await client.PostAsJsonAsync("/armas", new CriarArmaRequest(
            $"Espada {Guid.NewGuid():N}", "Sword", "Descrição", ClasseDeHeroi.Cruzado, CincoNiveisArma()));
        var item = await criar.Content.ReadFromJsonAsync<ItemDetalheDto>();
        using var pasta = new TemporaryDirectory();
        var hash = InventarioDeMidiasTestHelper.HashPng;
        var inventario = InventarioDeMidiasTestHelper.GravarInventario(pasta.Path, [
            InventarioDeMidiasTestHelper.Arquivo("Arma", @"C:\dd\heroes\crusader\crusader_weapon_1.png", "arquivos/arma/w1.png", hash, "crusader"),
            InventarioDeMidiasTestHelper.Arquivo("Arma", @"C:\dd\heroes\crusader\crusader_weapon_2.png", "arquivos/arma/w2.png", hash, "crusader"),
            InventarioDeMidiasTestHelper.Arquivo("Arma", @"C:\dd\heroes\crusader\crusader_weapon_3.png", "arquivos/arma/w3.png", hash, "crusader"),
            InventarioDeMidiasTestHelper.Arquivo("Arma", @"C:\dd\heroes\crusader\crusader_weapon_4.png", "arquivos/arma/w4.png", hash, "crusader"),
            InventarioDeMidiasTestHelper.Arquivo("Arma", @"C:\dd\heroes\crusader\crusader_weapon_5.png", "arquivos/arma/w5.png", hash, "crusader"),
        ]);
        var http = InventarioDeMidiasTestHelper.ClienteComInventario(factory, inventario);
        (await http.PostAsJsonAsync("/api/midias/publicacao", new { categoria = "arma" })).StatusCode.Should().Be(HttpStatusCode.Accepted);

        var detalhe = await http.GetFromJsonAsync<ItemDetalheDto>($"/itens/{item!.Id}");
        detalhe!.NiveisArma!.Select(n => n.Midia!.HashArquivo).Should().OnlyContain(h => h == hash);
    }

    [Fact]
    public async Task GET_personagem_com_arma_pendente_retorna_200()
    {
        var criarArma = await client.PostAsJsonAsync("/armas", new CriarArmaRequest(
            $"Espada {Guid.NewGuid():N}", "Sword", "Descrição", ClasseDeHeroi.Cruzado, CincoNiveisArma()));
        var arma = await criarArma.Content.ReadFromJsonAsync<ItemDetalheDto>();
        var personagem = await client.PostAsJsonAsync("/personagens", new CriarPersonagemRequest(
            Nome: $"Herói {Guid.NewGuid():N}",
            Classe: ClasseDeHeroi.Cruzado,
            HpMaximo: 30, HpAtual: 30, Velocidade: 4, Critico: 5m,
            DanoBaseMinimo: 6, DanoBaseMaximo: 10, Movimento: 2, BonusDeCritico: 3m,
            Tamanho: 1, AcoesPorTurno: 1, Esquiva: 10m, Precisao: 5m, Protecao: 0m, Nivel: 0,
            Stress: 0, ChanceDeVirtude: 25,
            HabilidadesEquipadas: null));
        var p = await personagem.Content.ReadFromJsonAsync<PersonagemDetalheDto>();
        await client.PostAsJsonAsync($"/personagens/{p!.Id}/equipar", new EquiparPersonagemRequest(arma!.Id, null, null));

        var response = await client.GetAsync($"/personagens/{p.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var detalhe = await response.Content.ReadFromJsonAsync<PersonagemDetalheDto>();
        detalhe!.MidiaArmaEquipada.Should().NotBeNull();
        detalhe.MidiaArmaEquipada!.Status.Should().Be("Pendente");
        detalhe.MidiaArmaEquipada.Nivel.Should().Be(1);
    }

    private static NivelDeArmaRequest[] CincoNiveisArma() =>
        Enumerable.Range(1, 5).Select(n => new NivelDeArmaRequest(n, 5 + n, 8 + n, 4m + n, 3 + n)).ToArray();
}
