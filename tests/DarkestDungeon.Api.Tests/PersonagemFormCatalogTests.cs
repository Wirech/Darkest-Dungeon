using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Application.Classes;
using DarkestDungeon.Application.Habilidades;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Domain.Classes;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests;

public sealed class PersonagemFormCatalogTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public PersonagemFormCatalogTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_classes_deve_fornecer_opcoes_para_o_formulario()
    {
        var response = await client.GetAsync("/classes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var classes = await response.Content.ReadFromJsonAsync<IReadOnlyList<ClasseResumoDto>>();
        classes.Should().NotBeNullOrEmpty();
        classes!.Should().Contain(classe => classe.Classe == ClasseDeHeroi.Cruzado);
    }

    [Fact]
    public async Task GET_habilidades_da_classe_deve_fornecer_opcoes_validas()
    {
        var classes = await client.GetFromJsonAsync<IReadOnlyList<ClasseResumoDto>>("/classes");
        var cruzado = classes!.Single(classe => classe.Classe == ClasseDeHeroi.Cruzado);

        var response = await client.GetAsync($"/classes/{cruzado.Id}/habilidades");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var habilidades = await response.Content.ReadFromJsonAsync<IReadOnlyList<HabilidadeResumoDto>>();
        habilidades.Should().NotBeNullOrEmpty();
        habilidades!.Should().OnlyContain(habilidade => habilidade.Id != Guid.Empty && habilidade.NomeExibicao.Length > 0);
    }
}