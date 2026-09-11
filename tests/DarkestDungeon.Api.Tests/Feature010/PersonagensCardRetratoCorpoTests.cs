using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Personagens;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature010;

public sealed class PersonagensCardRetratoCorpoTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public PersonagensCardRetratoCorpoTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_personagens_deve_devolver_retratos_distintos_para_aparencias_diferentes()
    {
        var a = await CriarCruzado(AparenciaDePersonagem.A);
        var b = await CriarCruzado(AparenciaDePersonagem.B);

        var lista = await client.GetAsync("/personagens");
        lista.StatusCode.Should().Be(HttpStatusCode.OK);
        var cards = await lista.Content.ReadFromJsonAsync<IReadOnlyList<PersonagemResumoDto>>();
        var cardA = cards!.Single(p => p.Id == a.Id);
        var cardB = cards.Single(p => p.Id == b.Id);

        cardA.Midias.Should().NotBeNull();
        cardB.Midias.Should().NotBeNull();
        cardA.Midias!.Retrato.Url.Should().NotBe(cardB.Midias!.Retrato.Url);
        cardA.Midias.CorpoInteiro.Url.Should().NotBe(cardB.Midias.CorpoInteiro.Url);
        cardA.Midias.Retrato.Url.Should().Contain("_A");
        cardB.Midias.Retrato.Url.Should().Contain("_B");
        cardA.Midias.CorpoInteiro.Url.Should().Contain("_A");
        cardB.Midias.CorpoInteiro.Url.Should().Contain("_B");
    }

    [Fact]
    public async Task Retrato_e_corpo_nao_devem_substituir_a_outra_pose_nem_outra_aparencia()
    {
        var criado = await CriarCruzado(AparenciaDePersonagem.B);
        var lista = await client.GetFromJsonAsync<IReadOnlyList<PersonagemResumoDto>>("/personagens");
        var card = lista!.Single(p => p.Id == criado.Id);

        card.Midias.Should().NotBeNull();
        if (card.Midias!.Retrato.Status == "Pendente")
        {
            card.Midias.Retrato.Url.Should().BeNull();
        }
        else
        {
            card.Midias.Retrato.Url.Should().Contain("portrait_roster");
            card.Midias.Retrato.Url.Should().NotContain("sprite.idle");
            card.Midias.Retrato.Url.Should().Contain("_B");
            card.Midias.Retrato.Url.Should().NotContain("_A/");
        }

        if (card.Midias.CorpoInteiro.Status == "Pendente")
        {
            card.Midias.CorpoInteiro.Url.Should().BeNull();
        }
        else
        {
            card.Midias.CorpoInteiro.Url.Should().Contain("sprite.idle");
            card.Midias.CorpoInteiro.Url.Should().NotContain("portrait_roster");
            card.Midias.CorpoInteiro.Url.Should().Contain("_B");
            card.Midias.CorpoInteiro.Url.Should().NotContain("_A/");
        }
    }

    private async Task<PersonagemDetalheDto> CriarCruzado(AparenciaDePersonagem aparencia)
    {
        var response = await client.PostAsJsonAsync("/personagens", new CriarPersonagemRequest(
            Nome: $"Retrato {aparencia} {Guid.NewGuid():N}",
            Classe: ClasseDeHeroi.Cruzado,
            HpMaximo: 33, HpAtual: 30, Velocidade: 1, Critico: 3,
            DanoBaseMinimo: 6, DanoBaseMaximo: 12, Movimento: 2, BonusDeCritico: 0,
            Tamanho: 1, AcoesPorTurno: 1, Esquiva: 5, Precisao: 0, Protecao: 0, Nivel: 0,
            Stress: 12, ChanceDeVirtude: 25,
            Aparencia: aparencia));
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var detalhe = await response.Content.ReadFromJsonAsync<PersonagemDetalheDto>();
        detalhe.Should().NotBeNull();
        return detalhe!;
    }
}
