using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Classes;
using DarkestDungeon.Application.Itens;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Itens;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests;

public class PersonagensEquipamentoTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public PersonagensEquipamentoTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    private static NivelDeArmaRequest[] CincoNiveisArma() =>
        Enumerable.Range(1, 5).Select(n => new NivelDeArmaRequest(n, 5 + n, 8 + n, 4m + n, 3 + n)).ToArray();

    private static CriarPersonagemRequest CriarPersonagemBase(ClasseDeHeroi classe) => new(
        Nome: $"Herói {Guid.NewGuid():N}",
        Classe: classe,
        HpMaximo: 30, HpAtual: 30, Velocidade: 4, Critico: 5m,
        DanoBaseMinimo: 6, DanoBaseMaximo: 10, Movimento: 2, BonusDeCritico: 3m,
        Tamanho: 1, AcoesPorTurno: 1, Esquiva: 10m, Precisao: 5m, Protecao: 0m, Nivel: 0,
        Stress: 0, ChanceDeVirtude: 25,
        HabilidadesEquipadas: null);

    [Fact]
    public async Task Equipar_arma_com_classe_igual_deve_retornar_200()
    {
        var personagem = await client.PostAsJsonAsync("/personagens", CriarPersonagemBase(ClasseDeHeroi.Cruzado));
        var p = await personagem.Content.ReadFromJsonAsync<PersonagemDetalheDto>();

        var arma = await client.PostAsJsonAsync("/armas", new CriarArmaRequest(
            $"Espada {Guid.NewGuid():N}", "Sword", "Descrição", ClasseDeHeroi.Cruzado, CincoNiveisArma()));
        var a = await arma.Content.ReadFromJsonAsync<ItemDetalheDto>();

        var equipar = await client.PostAsJsonAsync($"/personagens/{p!.Id}/equipar",
            new EquiparPersonagemRequest(a!.Id, null, null));

        equipar.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Equipar_arma_de_classe_diferente_deve_retornar_400()
    {
        var personagem = await client.PostAsJsonAsync("/personagens", CriarPersonagemBase(ClasseDeHeroi.Cruzado));
        var p = await personagem.Content.ReadFromJsonAsync<PersonagemDetalheDto>();

        var arma = await client.PostAsJsonAsync("/armas", new CriarArmaRequest(
            $"Adaga {Guid.NewGuid():N}", "Dagger", "Descrição", ClasseDeHeroi.Bandido, CincoNiveisArma()));
        var a = await arma.Content.ReadFromJsonAsync<ItemDetalheDto>();

        var equipar = await client.PostAsJsonAsync($"/personagens/{p!.Id}/equipar",
            new EquiparPersonagemRequest(a!.Id, null, null));

        equipar.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Equipar_acessorio_exclusivo_de_outra_classe_deve_retornar_400()
    {
        var personagem = await client.PostAsJsonAsync("/personagens", CriarPersonagemBase(ClasseDeHeroi.Cruzado));
        var p = await personagem.Content.ReadFromJsonAsync<PersonagemDetalheDto>();

        var acessorio = await client.PostAsJsonAsync("/acessorios", new CriarAcessorioRequest(
            $"Amuleto Ocultista {Guid.NewGuid():N}",
            "Occultist Amulet",
            "Descrição",
            RaridadeDeAcessorio.Rara,
            ClasseExclusiva: ClasseDeHeroi.Ocultista,
            ConjuntoId: null,
            Efeitos: Array.Empty<EfeitoDeAcessorioRequest>()));
        var ac = await acessorio.Content.ReadFromJsonAsync<ItemDetalheDto>();

        var equipar = await client.PostAsJsonAsync($"/personagens/{p!.Id}/equipar",
            new EquiparPersonagemRequest(null, null, new[] { ac!.Id }));

        equipar.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Equipar_armadura_de_mesma_classe_deve_retornar_200()
    {
        var personagem = await client.PostAsJsonAsync("/personagens", CriarPersonagemBase(ClasseDeHeroi.Cruzado));
        var p = await personagem.Content.ReadFromJsonAsync<PersonagemDetalheDto>();

        var niveisArmadura = Enumerable.Range(1, 5)
            .Select(n => new NivelDeArmaduraRequest(n, 10 * n, 5m + n))
            .ToArray();
        var armadura = await client.PostAsJsonAsync("/armaduras", new CriarArmaduraRequest(
            $"Placa {Guid.NewGuid():N}", "Plate", "Descrição", ClasseDeHeroi.Cruzado, niveisArmadura));
        var arm = await armadura.Content.ReadFromJsonAsync<ItemDetalheDto>();

        var equipar = await client.PostAsJsonAsync($"/personagens/{p!.Id}/equipar",
            new EquiparPersonagemRequest(null, arm!.Id, null));

        equipar.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Equipar_armadura_de_classe_diferente_deve_retornar_400()
    {
        var personagem = await client.PostAsJsonAsync("/personagens", CriarPersonagemBase(ClasseDeHeroi.Cruzado));
        var p = await personagem.Content.ReadFromJsonAsync<PersonagemDetalheDto>();

        var niveisArmadura = Enumerable.Range(1, 5)
            .Select(n => new NivelDeArmaduraRequest(n, 10 * n, 5m + n))
            .ToArray();
        var armadura = await client.PostAsJsonAsync("/armaduras", new CriarArmaduraRequest(
            $"Couro {Guid.NewGuid():N}", "Leather", "Descrição", ClasseDeHeroi.Bandido, niveisArmadura));
        var arm = await armadura.Content.ReadFromJsonAsync<ItemDetalheDto>();

        var equipar = await client.PostAsJsonAsync($"/personagens/{p!.Id}/equipar",
            new EquiparPersonagemRequest(null, arm!.Id, null));

        equipar.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
