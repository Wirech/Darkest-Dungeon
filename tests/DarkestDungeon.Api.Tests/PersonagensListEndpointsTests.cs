using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Classes;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests;

public sealed class PersonagensListEndpointsTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public PersonagensListEndpointsTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_personagens_sem_registros_deve_retornar_lista_vazia()
    {
        using var isolatedFactory = new ApiTestFactory();
        using var isolatedClient = isolatedFactory.CreateClient();

        var existentes = await isolatedClient.GetFromJsonAsync<IReadOnlyList<PersonagemResumoDto>>("/personagens");
        foreach (var personagem in existentes ?? Array.Empty<PersonagemResumoDto>())
        {
            await isolatedClient.DeleteAsync($"/personagens/{personagem.Id}");
        }

        var response = await isolatedClient.GetAsync("/personagens");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var personagens = await response.Content.ReadFromJsonAsync<IReadOnlyList<PersonagemResumoDto>>();
        personagens.Should().NotBeNull();
        personagens.Should().BeEmpty();
    }

    [Fact]
    public async Task GET_personagens_deve_retornar_resumo_com_campos_principais()
    {
        var request = new CriarPersonagemRequest(
            Nome: "Leitor de Catálogo",
            Classe: ClasseDeHeroi.Cruzado,
            HpMaximo: 33,
            HpAtual: 30,
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
            Stress: 12,
            ChanceDeVirtude: 5,
            HabilidadesEquipadas: null);

        var criado = await client.PostAsJsonAsync("/personagens", request);
        criado.StatusCode.Should().Be(HttpStatusCode.Created);

        var response = await client.GetAsync("/personagens");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var personagens = await response.Content.ReadFromJsonAsync<IReadOnlyList<PersonagemResumoDto>>();

        var personagem = personagens.Should().Contain(item => item.Nome == request.Nome).Which;
        personagem.Nome.Should().Be(request.Nome);
        personagem.Classe.Should().Be(request.Classe);
        personagem.NomeClasse.Should().NotBeNullOrWhiteSpace();
        personagem.HpAtual.Should().Be(request.HpAtual);
        personagem.HpMaximo.Should().Be(request.HpMaximo);
        personagem.Stress.Should().Be(request.Stress);
        personagem.Nivel.Should().Be(request.Nivel);
        personagem.Habilidades.Should().BeEmpty();
    }

    [Fact]
    public async Task GET_personagens_com_rota_invalida_deve_retornar_404()
    {
        var response = await client.GetAsync("/personagens/nao-e-um-guid");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
