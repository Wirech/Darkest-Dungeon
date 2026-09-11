using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Classes;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature011;

public sealed class PersonagensCardAcampamentoMidiaTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public PersonagensCardAcampamentoMidiaTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Acampamento_com_png_no_acervo_fica_ok_e_combate_inalterado()
    {
        var acervoEncourage = LocalizarEncourage();
        if (acervoEncourage is null)
        {
            return;
        }

        var response = await client.PostAsJsonAsync("/personagens", new CriarPersonagemRequest(
            Nome: $"Camp {Guid.NewGuid():N}",
            Classe: ClasseDeHeroi.Cruzado,
            HpMaximo: 33, HpAtual: 30, Velocidade: 1, Critico: 3,
            DanoBaseMinimo: 6, DanoBaseMaximo: 12, Movimento: 2, BonusDeCritico: 0,
            Tamanho: 1, AcoesPorTurno: 1, Esquiva: 5, Precisao: 0, Protecao: 0, Nivel: 0,
            Stress: 12, ChanceDeVirtude: 25,
            NivelDaArma: 1,
            NivelDaArmadura: 1));
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var detalhe = await response.Content.ReadFromJsonAsync<PersonagemDetalheDto>();

        var lista = await client.GetFromJsonAsync<IReadOnlyList<PersonagemResumoDto>>("/personagens");
        var card = lista!.Single(p => p.Id == detalhe!.Id);

        var acampamento = card.Habilidades.Where(h => h.Categoria == "Acampamento").ToArray();
        acampamento.Should().NotBeEmpty();
        foreach (var habilidade in acampamento)
        {
            habilidade.Midia.Should().NotBeNull();
            habilidade.Midia!.Status.Should().Be("OK");
            habilidade.Midia.Url.Should().StartWith("/acervo/herois/arquivos/acampamento/camp_skill_");
            habilidade.Midia.Url.Should().EndWith(".png");
        }

        foreach (var combate in card.Habilidades.Where(h => h.Categoria == "Combate"))
        {
            (combate.Midia?.Url ?? string.Empty).Should().NotContain("camp_skill_");
        }
    }

    private static string? LocalizarEncourage()
    {
        var atual = AppContext.BaseDirectory;
        for (var i = 0; i < 10; i++)
        {
            var candidato = Path.Combine(atual, "assets", "herois", "arquivos", "acampamento", "camp_skill_encourage.png");
            if (File.Exists(candidato))
            {
                return candidato;
            }

            var pai = Directory.GetParent(atual);
            if (pai is null)
            {
                break;
            }

            atual = pai.FullName;
        }

        return null;
    }
}
