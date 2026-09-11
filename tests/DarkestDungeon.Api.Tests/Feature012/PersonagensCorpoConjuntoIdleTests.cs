using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Personagens;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature012;

public sealed class PersonagensCorpoConjuntoIdleTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public PersonagensCorpoConjuntoIdleTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_personagens_deve_devolver_texturas_de_paleta_distintas_e_atlas_da_classe()
    {
        var a = await PersonagemCardTestHelper.CriarCruzado(client, AparenciaDePersonagem.A);
        var b = await PersonagemCardTestHelper.CriarCruzado(client, AparenciaDePersonagem.B);

        var lista = await client.GetAsync("/personagens");
        lista.StatusCode.Should().Be(HttpStatusCode.OK);
        var cards = await lista.Content.ReadFromJsonAsync<IReadOnlyList<PersonagemResumoDto>>();
        var cardA = cards!.Single(p => p.Id == a.Id);
        var cardB = cards.Single(p => p.Id == b.Id);

        var idleA = cardA.Midias!.CorpoInteiro.ConjuntoIdle;
        var idleB = cardB.Midias!.CorpoInteiro.ConjuntoIdle;
        idleA.Should().NotBeNull();
        idleB.Should().NotBeNull();
        idleA!.UrlTextura.Should().Contain("_A");
        idleB!.UrlTextura.Should().Contain("_B");
        idleA.UrlTextura.Should().NotBe(idleB.UrlTextura);
        idleA.UrlAtlas.Should().Contain("arquivos/cruzado/anim/");
        idleA.UrlAtlas.Should().NotContain("_A");
        idleA.UrlAtlas.Should().NotContain("_B");
        idleA.UrlEsqueleto.Should().Contain("arquivos/cruzado/anim/");
        idleA.UrlEsqueleto.Should().NotContain("_A");
        idleA.UrlAtlas.Should().Be(idleB.UrlAtlas);
        idleA.UrlEsqueleto.Should().Be(idleB.UrlEsqueleto);
    }

    [Fact]
    public async Task Url_do_slot_ainda_contem_sprite_idle_e_pasta_da_aparencia()
    {
        var criado = await PersonagemCardTestHelper.CriarCruzado(client, AparenciaDePersonagem.B);
        var lista = await client.GetFromJsonAsync<IReadOnlyList<PersonagemResumoDto>>("/personagens");
        var card = lista!.Single(p => p.Id == criado.Id);

        card.Midias!.CorpoInteiro.Status.Should().Be("OK");
        card.Midias.CorpoInteiro.Url.Should().Contain("sprite.idle");
        card.Midias.CorpoInteiro.Url.Should().Contain("_B");
        card.Midias.CorpoInteiro.Url.Should().NotContain("portrait_roster");
        card.Midias.CorpoInteiro.Url.Should().NotContain("_A/");
    }
}

public sealed class PersonagensCorpoIdleIncompletoTests : IClassFixture<AcervoSemIdleDoCruzadoFactory>
{
    private readonly HttpClient client;

    public PersonagensCorpoIdleIncompletoTests(AcervoSemIdleDoCruzadoFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Idle_incompleto_fica_pendente_sem_substituir_retrato()
    {
        var criado = await PersonagemCardTestHelper.CriarCruzado(client, AparenciaDePersonagem.A);
        var lista = await client.GetAsync("/personagens");
        lista.StatusCode.Should().Be(HttpStatusCode.OK);
        var cards = await lista.Content.ReadFromJsonAsync<IReadOnlyList<PersonagemResumoDto>>();
        var card = cards!.Single(p => p.Id == criado.Id);

        card.Midias!.CorpoInteiro.Status.Should().Be("Pendente");
        card.Midias.CorpoInteiro.Url.Should().BeNull();
        card.Midias.CorpoInteiro.ArquivoInventarioId.Should().BeNull();
        card.Midias.CorpoInteiro.ConjuntoIdle.Should().BeNull();
        card.Midias.Retrato.Status.Should().Be("OK");
        card.Midias.Retrato.Url.Should().Contain("portrait_roster");
        var emEspera = card.Midias.CorpoInteiro.Versoes!.Single(v => v.Id == "emEspera");
        var animado = card.Midias.CorpoInteiro.Versoes!.Single(v => v.Id == "animado");
        emEspera.Disponivel.Should().BeFalse();
        animado.Disponivel.Should().BeFalse();
    }
}
