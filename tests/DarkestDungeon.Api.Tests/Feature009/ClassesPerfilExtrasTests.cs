using System.Net.Http.Json;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Classes;
using DarkestDungeon.Domain.Classes;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature009;

public sealed class ClassesPerfilExtrasTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public ClassesPerfilExtrasTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_classe_deve_expor_perfil_oficial()
    {
        var classes = await client.GetFromJsonAsync<IReadOnlyList<ClasseResumoDto>>("/classes");
        var cruzado = classes!.Single(c => c.Classe == ClasseDeHeroi.Cruzado);
        var detalhe = await client.GetFromJsonAsync<ClasseDetalheDto>($"/classes/{cruzado.Id}");
        detalhe.Should().NotBeNull();
        detalhe!.PassosAFrente.Should().BeGreaterThanOrEqualTo(0);
        detalhe.PassosAtras.Should().BeGreaterThanOrEqualTo(0);
        detalhe.ProvisaoInicial.Should().NotBeNull();
        detalhe.BonusAoCriticoDaClasse.Should().NotBeNull();
        detalhe.BonusAoCriticoDaClasse.Should().NotContain("}}");
    }
}
