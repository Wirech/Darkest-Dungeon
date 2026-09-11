using System.Net.Http.Json;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Personagens;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature012;

public sealed class PersonagensCorpoVersoesTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public PersonagensCorpoVersoesTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Idle_e_walk_completos_habilitam_tres_versoes()
    {
        var criado = await PersonagemCardTestHelper.CriarCruzado(client, AparenciaDePersonagem.A);
        var lista = await client.GetFromJsonAsync<IReadOnlyList<PersonagemResumoDto>>("/personagens");
        var card = lista!.Single(p => p.Id == criado.Id);
        var corpo = card.Midias!.CorpoInteiro;

        corpo.Status.Should().Be("OK");
        corpo.ConjuntoIdle.Should().NotBeNull();
        corpo.ConjuntoWalk.Should().NotBeNull();
        corpo.ConjuntoWalk!.Ciclo.Should().Be("walk");
        corpo.ConjuntoWalk.UrlAtlas.Should().Contain("arquivos/cruzado/anim/crusader.sprite.walk.atlas");
        corpo.ConjuntoWalk.UrlEsqueleto.Should().Contain("arquivos/cruzado/anim/crusader.sprite.walk.skel");
        corpo.ConjuntoWalk.UrlTextura.Should().Contain("crusader_A/anim/crusader.sprite.walk.png");
        corpo.Versoes.Should().HaveCountGreaterThanOrEqualTo(3);
        corpo.Versoes!.Select(v => v.Id).Should().Contain(new[] { "emEspera", "animado", "caminhada" });
        corpo.Versoes.Take(3).Select(v => v.Rotulo).Should().Equal("Em espera", "Animado", "Caminhada");
        corpo.Versoes.Take(3).Should().OnlyContain(v => v.Disponivel);
        corpo.Versoes.Should().Contain(v => v.Id.StartsWith("attack_"));
        corpo.Conjuntos.Should().NotBeNull();
        corpo.Conjuntos.Should().Contain(c => c.Ciclo == "combat");
        corpo.Conjuntos.Should().Contain(c => c.Ciclo.StartsWith("attack_"));
        string.Join(' ', new[] { corpo.ConjuntoIdle!.UrlAtlas, corpo.ConjuntoWalk.UrlAtlas }).Should().NotContain("attack");
    }
}

public sealed class PersonagensCorpoWalkAusenteTests : IClassFixture<AcervoSemWalkDoCruzadoFactory>
{
    private readonly HttpClient client;

    public PersonagensCorpoWalkAusenteTests(AcervoSemWalkDoCruzadoFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Walk_ausente_desabilita_caminhada_e_mantem_idle_ok()
    {
        var criado = await PersonagemCardTestHelper.CriarCruzado(client, AparenciaDePersonagem.A);
        var lista = await client.GetFromJsonAsync<IReadOnlyList<PersonagemResumoDto>>("/personagens");
        var corpo = lista!.Single(p => p.Id == criado.Id).Midias!.CorpoInteiro;

        corpo.Status.Should().Be("OK");
        corpo.ConjuntoIdle.Should().NotBeNull();
        corpo.ConjuntoWalk.Should().BeNull();
        corpo.Versoes!.Single(v => v.Id == "caminhada").Disponivel.Should().BeFalse();
        corpo.Versoes.Single(v => v.Id == "emEspera").Disponivel.Should().BeTrue();
    }
}
