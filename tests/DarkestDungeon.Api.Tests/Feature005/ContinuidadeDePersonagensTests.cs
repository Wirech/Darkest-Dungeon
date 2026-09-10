using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Domain.Classes;
using FluentAssertions;
using Xunit;

namespace DarkestDungeon.Api.Tests.Feature005;

/// T158: valida SC-009 — Personagem criado antes da publicação continua válido depois.
public sealed class ContinuidadeDePersonagensTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public ContinuidadeDePersonagensTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Personagem_criado_antes_da_publicacao_continua_valido_depois_SC009()
    {
        var criar = await client.PostAsJsonAsync("/personagens", new
        {
            nome = "Sir Reginald T158",
            classe = ClasseDeHeroi.Cruzado,
            hpMaximo = 30,
            hpAtual = 30,
            velocidade = 4,
            critico = 3m,
            danoBaseMinimo = 6,
            danoBaseMaximo = 12,
            movimento = 2,
            bonusDeCritico = 15m,
            tamanho = 2,
            acoesPorTurno = 1,
            esquiva = 5m,
            precisao = 0m,
            protecao = 0m,
            nivel = 0,
            stress = 0,
            chanceDeVirtude = 25,
            habilidadesEquipadas = new Guid[0],
        });
        criar.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.OK);
        var criado = await criar.Content.ReadFromJsonAsync<JsonElement>();
        var personagemId = criado.GetProperty("id").GetGuid();

        // Executa publicação (em ambiente Testing usa InMemory + detector retorna 0).
        var pub = await client.PostAsJsonAsync("/api/publicacao", new
        {
            confirmacaoJanelaManutencao = true,
            observacao = "T158 continuidade",
        });
        pub.StatusCode.Should().BeOneOf(HttpStatusCode.Accepted, HttpStatusCode.OK);

        // Consulta o personagem depois.
        var depois = await client.GetAsync($"/personagens/{personagemId}");
        depois.StatusCode.Should().Be(HttpStatusCode.OK, "SC-009: Personagem deve continuar válido após publicação");
        var body = await depois.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("id").GetGuid().Should().Be(personagemId);
        body.GetProperty("nome").GetString().Should().Be("Sir Reginald T158");
    }
}
