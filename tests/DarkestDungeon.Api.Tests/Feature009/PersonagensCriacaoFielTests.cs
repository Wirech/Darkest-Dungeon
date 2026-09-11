using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Classes;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature009;

public sealed class PersonagensCriacaoFielTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public PersonagensCriacaoFielTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    public static IEnumerable<object[]> Amostras()
    {
        foreach (var classe in new[] { ClasseDeHeroi.Cruzado, ClasseDeHeroi.Vestal, ClasseDeHeroi.Bandido })
        {
            foreach (var nivel in new[] { 0, 3, 6 })
            {
                yield return new object[] { classe, nivel };
            }
        }
    }

    [Theory]
    [MemberData(nameof(Amostras))]
    public async Task POST_fiel_deve_derivar_acc_prot_stress_zero_e_passos_da_classe(ClasseDeHeroi classe, int nivel)
    {
        var response = await client.PostAsJsonAsync("/personagens", new
        {
            nome = $"Fiel {classe} {nivel} {Guid.NewGuid():N}",
            classe,
            nivel,
            nivelDaArma = 1,
            nivelDaArmadura = 1,
            aparencia = 0
        });

        if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.NotFound)
        {
            var corpo = await response.Content.ReadAsStringAsync();
            corpo.Should().Match(t => t.Contains("catálogo", StringComparison.OrdinalIgnoreCase)
                || t.Contains("oficial", StringComparison.OrdinalIgnoreCase)
                || t.Contains("deslocamento", StringComparison.OrdinalIgnoreCase));
            return;
        }

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var personagem = await response.Content.ReadFromJsonAsync<PersonagemDetalheDto>();
        personagem.Should().NotBeNull();
        personagem!.Precisao.Should().Be(0);
        personagem.Protecao.Should().Be(0);
        personagem.Stress.Should().Be(0);
        personagem.ChanceDeVirtude.Should().Be(25);
        personagem.Habilidades.Count(h => h.NumeroDoNivel == 1).Should().Be(8);
        personagem.Habilidades.Should().OnlyContain(h => h.NumeroDoNivel == 0 || h.NumeroDoNivel == 1);
    }

    [Theory]
    [InlineData(ClasseDeHeroi.Abominacao)]
    [InlineData(ClasseDeHeroi.Duelista)]
    public async Task POST_fiel_abominacao_e_duelista_devem_desbloquear_todas_as_habilidades_de_combate(ClasseDeHeroi classe)
    {
        var response = await client.PostAsJsonAsync("/personagens", new
        {
            nome = $"Exceção {classe} {Guid.NewGuid():N}",
            classe,
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
        personagem.Should().NotBeNull();
        var combate = personagem!.Habilidades.Where(h => h.Categoria == "Combate").ToArray();
        combate.Should().NotBeEmpty();
        combate.Should().OnlyContain(h => h.NumeroDoNivel == 1);
        personagem.Habilidades.Count(h => h.Categoria == "Acampamento" && h.NumeroDoNivel == 1).Should().Be(4);
    }
}
