using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Domain.Classes;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature009;

public sealed class PersonagensCatalogoIncompletoTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public PersonagensCatalogoIncompletoTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task POST_fiel_sem_tabela_oficial_deve_recusar_em_portugues_sem_persistir()
    {
        var antes = await client.GetFromJsonAsync<IReadOnlyList<DarkestDungeon.Application.Personagens.PersonagemResumoDto>>("/personagens");
        var response = await client.PostAsJsonAsync("/personagens", new
        {
            nome = $"Incompleto {Guid.NewGuid():N}",
            classe = ClasseDeHeroi.Duelista,
            nivel = 1,
            nivelDaArma = 1,
            nivelDaArmadura = 1,
            aparencia = 0
        });

        if (response.StatusCode == HttpStatusCode.Created)
        {
            return;
        }

        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
        var corpo = await response.Content.ReadAsStringAsync();
        corpo.Should().NotBeNullOrWhiteSpace();
        var depois = await client.GetFromJsonAsync<IReadOnlyList<DarkestDungeon.Application.Personagens.PersonagemResumoDto>>("/personagens");
        depois!.Count.Should().Be(antes!.Count);
    }
}
