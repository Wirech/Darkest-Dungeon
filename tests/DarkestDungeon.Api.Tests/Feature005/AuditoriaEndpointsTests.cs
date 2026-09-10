using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace DarkestDungeon.Api.Tests.Feature005;

/// Contract tests da Feature 005 — endpoints de auditoria.
public sealed class AuditoriaEndpointsTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public AuditoriaEndpointsTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_relatorio_retorna_200_com_resumo()
    {
        var response = await client.GetAsync("/api/auditoria/relatorio");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        json.GetProperty("resumo").GetProperty("totalHabilidades").GetInt32().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GET_relatorio_por_classe_conhecida_retorna_200()
    {
        var response = await client.GetAsync("/api/auditoria/relatorio/classes/Cruzado");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_relatorio_por_classe_desconhecida_retorna_404_PtBr()
    {
        var response = await client.GetAsync("/api/auditoria/relatorio/classes/ClasseInexistente");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Classe desconhecida");
    }

    [Fact]
    public async Task GET_cobertura_retorna_200_com_totais_de_assets()
    {
        var response = await client.GetAsync("/api/auditoria/cobertura");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        json.GetProperty("totalHabilidades").GetInt32().Should().BeGreaterThan(0);
        (json.GetProperty("assetsColetados").GetInt32() + json.GetProperty("assetsPendentes").GetInt32()).Should().Be(80);
    }
}
