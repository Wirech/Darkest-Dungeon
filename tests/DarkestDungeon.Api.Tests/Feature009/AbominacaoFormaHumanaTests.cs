using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Classes;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature009;

public sealed class AbominacaoFormaHumanaTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public AbominacaoFormaHumanaTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task POST_abominacao_fiel_deve_usar_numeros_humanos()
    {
        var response = await client.PostAsJsonAsync("/personagens", new
        {
            nome = $"Abominação {Guid.NewGuid():N}",
            classe = ClasseDeHeroi.Abominacao,
            nivel = 0,
            nivelDaArma = 1,
            nivelDaArmadura = 1,
            aparencia = 0
        });

        if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.NotFound)
        {
            return;
        }

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var personagem = await response.Content.ReadFromJsonAsync<PersonagemDetalheDto>();
        personagem!.HpMaximo.Should().BeGreaterThan(0);
        personagem.Velocidade.Should().BeGreaterThan(0);
        var combate = personagem.Habilidades.Where(h => h.Categoria == "Combate").ToArray();
        combate.Should().NotBeEmpty();
        combate.Should().OnlyContain(h => h.NumeroDoNivel == 1);
        personagem.Habilidades.Count(h => h.Categoria == "Acampamento" && h.NumeroDoNivel == 1).Should().Be(4);
    }
}
