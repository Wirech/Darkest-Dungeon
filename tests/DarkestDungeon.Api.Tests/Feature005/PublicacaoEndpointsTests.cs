using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DarkestDungeon.Api.Tests.Fixtures;
using FluentAssertions;
using Xunit;

namespace DarkestDungeon.Api.Tests.Feature005;

/// Contract tests da Feature 005 — publicação atômica.
public sealed class PublicacaoEndpointsTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public PublicacaoEndpointsTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task POST_publicacao_sem_confirmacao_retorna_400_PtBr()
    {
        var response = await client.PostAsJsonAsync("/api/publicacao", new
        {
            confirmacaoJanelaManutencao = false,
            observacao = "teste",
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Confirmação obrigatória");
    }

    [Fact]
    public async Task POST_publicacao_com_confirmacao_retorna_202_e_id_valido()
    {
        var response = await client.PostAsJsonAsync("/api/publicacao", new
        {
            confirmacaoJanelaManutencao = true,
            observacao = "teste automatizado",
        });

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var publicacaoId = json.GetProperty("publicacaoId").GetGuid();
        publicacaoId.Should().NotBeEmpty();

        // Consulta status
        var responseStatus = await client.GetAsync($"/api/publicacao/{publicacaoId}/status");
        responseStatus.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_status_de_publicacao_desconhecida_retorna_404()
    {
        var response = await client.GetAsync($"/api/publicacao/{Guid.NewGuid()}/status");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GET_logs_de_publicacao_conhecida_retorna_lista()
    {
        // Primeiro dispara uma publicação
        var responsePub = await client.PostAsJsonAsync("/api/publicacao", new
        {
            confirmacaoJanelaManutencao = true,
            observacao = "para logs",
        });
        responsePub.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var json = await responsePub.Content.ReadFromJsonAsync<JsonElement>();
        var publicacaoId = json.GetProperty("publicacaoId").GetGuid();

        var responseLogs = await client.GetAsync($"/api/publicacao/{publicacaoId}/logs");
        responseLogs.StatusCode.Should().Be(HttpStatusCode.OK);

        var logs = await responseLogs.Content.ReadFromJsonAsync<JsonElement>();
        logs.GetProperty("total").GetInt32().Should().BeGreaterThanOrEqualTo(1);
    }
}
