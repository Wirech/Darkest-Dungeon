using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Classes;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature010;

public sealed class PersonagensCardEquipamentoTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public PersonagensCardEquipamentoTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Arma_nivel_1_e_5_devem_usar_indices_0_e_4()
    {
        var um = await Criar(nivelArma: 1, nivelArmadura: 1);
        var cinco = await Criar(nivelArma: 5, nivelArmadura: 5);

        var lista = await client.GetFromJsonAsync<IReadOnlyList<PersonagemResumoDto>>("/personagens");
        var card1 = lista!.Single(p => p.Id == um.Id);
        var card5 = lista.Single(p => p.Id == cinco.Id);

        card1.Midias!.Arma.Url.Should().Contain("eqp_weapon_0");
        card5.Midias!.Arma.Url.Should().Contain("eqp_weapon_4");
        card1.Midias.Arma.Url.Should().NotContain("eqp_weapon_4");
        card5.Midias.Arma.Url.Should().NotContain("eqp_weapon_0");
        card1.Midias.Armadura.Url.Should().Contain("eqp_armour_0");
        card5.Midias.Armadura.Url.Should().Contain("eqp_armour_4");
        card1.Midias.Arma.Nivel.Should().Be(1);
        card5.Midias.Arma.Nivel.Should().Be(5);
    }

    [Fact]
    public async Task Sem_nivel_de_arma_o_slot_fica_pendente()
    {
        var response = await client.PostAsJsonAsync("/personagens", new CriarPersonagemRequest(
            Nome: $"Sem arma {Guid.NewGuid():N}",
            Classe: ClasseDeHeroi.Cruzado,
            HpMaximo: 33, HpAtual: 30, Velocidade: 1, Critico: 3,
            DanoBaseMinimo: 6, DanoBaseMaximo: 12, Movimento: 2, BonusDeCritico: 0,
            Tamanho: 1, AcoesPorTurno: 1, Esquiva: 5, Precisao: 0, Protecao: 0, Nivel: 0,
            Stress: 12, ChanceDeVirtude: 25));
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var detalhe = await response.Content.ReadFromJsonAsync<PersonagemDetalheDto>();

        var lista = await client.GetAsync("/personagens");
        lista.StatusCode.Should().Be(HttpStatusCode.OK);
        var card = (await lista.Content.ReadFromJsonAsync<IReadOnlyList<PersonagemResumoDto>>())!
            .Single(p => p.Id == detalhe!.Id);

        card.Midias!.Arma.Status.Should().Be("Pendente");
        card.Midias.Arma.Url.Should().BeNull();
        card.Midias.Arma.Nivel.Should().BeNull();
        card.Midias.Armadura.Status.Should().Be("Pendente");
        card.Midias.Armadura.Url.Should().BeNull();
        card.Midias.Retrato.Should().NotBeNull();
    }

    private async Task<PersonagemDetalheDto> Criar(int nivelArma, int nivelArmadura)
    {
        var response = await client.PostAsJsonAsync("/personagens", new CriarPersonagemRequest(
            Nome: $"Eqp {nivelArma}-{nivelArmadura} {Guid.NewGuid():N}",
            Classe: ClasseDeHeroi.Cruzado,
            HpMaximo: 33, HpAtual: 30, Velocidade: 1, Critico: 3,
            DanoBaseMinimo: 6, DanoBaseMaximo: 12, Movimento: 2, BonusDeCritico: 0,
            Tamanho: 1, AcoesPorTurno: 1, Esquiva: 5, Precisao: 0, Protecao: 0, Nivel: 0,
            Stress: 12, ChanceDeVirtude: 25,
            NivelDaArma: nivelArma,
            NivelDaArmadura: nivelArmadura));
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var detalhe = await response.Content.ReadFromJsonAsync<PersonagemDetalheDto>();
        detalhe.Should().NotBeNull();
        return detalhe!;
    }
}
