using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts;
using DarkestDungeon.Api.Tests.Fixtures;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests;

public class SeresEndpointsTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public SeresEndpointsTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task CriarEConsultarSer_ComDadosValidos_DeveRetornarCriadoEConsultaOk()
    {
        var createResponse = await client.PostAsJsonAsync("/seres", CriarRequest());

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        createResponse.Headers.Location?.ToString().Should().StartWith("/seres/");
        var criado = await createResponse.Content.ReadFromJsonAsync<SerResponse>();
        criado.Should().NotBeNull();
        criado!.Id.Should().NotBe(Guid.Empty);
        criado.DanoBaseMinimo.Should().Be(7);
        criado.DanoBaseMaximo.Should().Be(13);
        criado.Resistencias.Atordoamento.Should().Be(40);

        var getResponse = await client.GetAsync($"/seres/{criado.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var consultado = await getResponse.Content.ReadFromJsonAsync<SerResponse>();
        consultado.Should().NotBeNull();
        consultado!.Id.Should().Be(criado.Id);
        consultado.Nome.Should().Be("Cruzado");
    }

    public static CriarSerRequest CriarRequest() => new(
        "Cruzado",
        "Herói",
        HpMaximo: 33,
        HpAtual: 33,
        Velocidade: 1,
        Critico: 5,
        DanoBaseMinimo: 7,
        DanoBaseMaximo: 13,
        Movimento: 2,
        BonusDeCritico: 0,
        Tamanho: 1,
        AcoesPorTurno: 1,
        Esquiva: 5,
        Precisao: 85,
        Protecao: 0,
        Nivel: 0,
        new ResistenciasRequest(40, 30, 20, 25, 35));
}