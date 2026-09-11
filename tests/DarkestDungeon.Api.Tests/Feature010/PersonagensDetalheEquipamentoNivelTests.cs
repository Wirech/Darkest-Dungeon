using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Classes;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature010;

public sealed class PersonagensDetalheEquipamentoNivelTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public PersonagensDetalheEquipamentoNivelTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Detalhe_deve_usar_nivel_cadastrado_e_nao_resolucao()
    {
        var criado = await client.PostAsJsonAsync("/personagens", new CriarPersonagemRequest(
            Nome: $"Detalhe eqp {Guid.NewGuid():N}",
            Classe: ClasseDeHeroi.Cruzado,
            HpMaximo: 33, HpAtual: 30, Velocidade: 1, Critico: 3,
            DanoBaseMinimo: 6, DanoBaseMaximo: 12, Movimento: 2, BonusDeCritico: 0,
            Tamanho: 1, AcoesPorTurno: 1, Esquiva: 5, Precisao: 0, Protecao: 0, Nivel: 6,
            Stress: 12, ChanceDeVirtude: 25,
            NivelDaArma: 1,
            NivelDaArmadura: 2));
        criado.StatusCode.Should().Be(HttpStatusCode.Created);
        var detalheCriado = await criado.Content.ReadFromJsonAsync<PersonagemDetalheDto>();

        var response = await client.GetAsync($"/personagens/{detalheCriado!.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var detalhe = await response.Content.ReadFromJsonAsync<PersonagemDetalheDto>();

        detalhe!.NivelDaArma.Should().Be(1);
        detalhe.NivelDaArmadura.Should().Be(2);
        detalhe.Midias.Should().NotBeNull();
        detalhe.Midias!.Arma.Nivel.Should().Be(1);
        detalhe.Midias.Armadura.Nivel.Should().Be(2);
        detalhe.Midias.Arma.Url.Should().Contain("eqp_weapon_0");
        detalhe.Midias.Armadura.Url.Should().Contain("eqp_armour_1");
        if (detalhe.MidiaArmaEquipada is not null)
        {
            detalhe.MidiaArmaEquipada.Nivel.Should().Be(1);
        }

        if (detalhe.MidiaArmaduraEquipada is not null)
        {
            detalhe.MidiaArmaduraEquipada.Nivel.Should().Be(2);
        }
    }
}
