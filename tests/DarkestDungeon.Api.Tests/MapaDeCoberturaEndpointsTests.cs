using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Classes;
using DarkestDungeon.Application.Cobertura;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Cobertura;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests;

public class MapaDeCoberturaEndpointsTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public MapaDeCoberturaEndpointsTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_mapa_de_cobertura_deve_retornar_uma_entrada_por_classe()
    {
        var response = await client.GetAsync("/mapa-de-cobertura");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var mapa = await response.Content.ReadFromJsonAsync<IReadOnlyList<MapaDeCoberturaPorClasseDto>>();
        mapa.Should().NotBeNull().And.HaveCount(20);
    }

    [Fact]
    public async Task GET_mapa_por_classe_inexistente_deve_retornar_404()
    {
        var response = await client.GetAsync($"/mapa-de-cobertura/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task POST_registrar_e_GET_por_classe_deve_refletir_estado()
    {
        var lista = await client.GetFromJsonAsync<IReadOnlyList<ClasseResumoDto>>("/classes");
        var bandido = lista!.Single(c => c.Classe == ClasseDeHeroi.Bandido);

        var chave = $"Sangramento-{Guid.NewGuid():N}";
        var registrar = await client.PostAsJsonAsync("/mapa-de-cobertura", new RegistrarCoberturaCommand(
            ClasseDeHeroi.Bandido,
            CategoriaDeCobertura.ResistenciaBase,
            chave,
            EstadoDeAtributo.Coletado,
            "extraído da wiki"));

        registrar.StatusCode.Should().Be(HttpStatusCode.OK);

        var consulta = await client.GetFromJsonAsync<MapaDeCoberturaPorClasseDto>($"/mapa-de-cobertura/{bandido.Id}");
        consulta!.Entradas.Should().Contain(e => e.ChaveDoAtributo == chave && e.Estado == EstadoDeAtributo.Coletado);
    }
}
