using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Classes;
using DarkestDungeon.Application.Habilidades;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests;

public class ClassesEndpointsTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public ClassesEndpointsTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_classes_deve_retornar_exatamente_20_classes()
    {
        var response = await client.GetAsync("/classes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var lista = await response.Content.ReadFromJsonAsync<IReadOnlyList<ClasseResumoDto>>();
        lista.Should().NotBeNull().And.HaveCount(20);
    }

    [Fact]
    public async Task GET_classes_por_id_deve_retornar_ClasseDetalhe()
    {
        var lista = await client.GetFromJsonAsync<IReadOnlyList<ClasseResumoDto>>("/classes");
        var cruzado = lista!.First(c => c.Classe == DarkestDungeon.Domain.Classes.ClasseDeHeroi.Cruzado);

        var response = await client.GetAsync($"/classes/{cruzado.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var detalhe = await response.Content.ReadFromJsonAsync<ClasseDetalheDto>();
        detalhe!.NomeExibicao.Should().Be("Cruzado");
        detalhe.NomeOriginal.Should().Be("Crusader");
        detalhe.ResistenciasBase.Should().NotBeNull();
    }

    [Fact]
    public async Task GET_classes_por_id_inexistente_deve_retornar_404()
    {
        var response = await client.GetAsync($"/classes/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GET_classes_habilidades_para_classe_inexistente_deve_retornar_404()
    {
        var response = await client.GetAsync($"/classes/{Guid.NewGuid()}/habilidades");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GET_classes_habilidades_para_classe_valida_deve_retornar_lista()
    {
        var lista = await client.GetFromJsonAsync<IReadOnlyList<ClasseResumoDto>>("/classes");
        var bandido = lista!.First(c => c.Classe == DarkestDungeon.Domain.Classes.ClasseDeHeroi.Bandido);

        var response = await client.GetAsync($"/classes/{bandido.Id}/habilidades");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var habilidades = await response.Content.ReadFromJsonAsync<IReadOnlyList<HabilidadeResumoDto>>();
        habilidades.Should().NotBeNull();
    }
}
