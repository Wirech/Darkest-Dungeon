using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Classes;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Classes;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests;

public sealed class PersonagensDeleteEndpointsTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public PersonagensDeleteEndpointsTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task DELETE_personagem_existente_deve_retornar_204_e_remover_da_lista()
    {
        var criado = await CriarPersonagemAsync();

        var response = await client.DeleteAsync($"/personagens/{criado.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var consulta = await client.GetAsync($"/personagens/{criado.Id}");
        consulta.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DELETE_personagem_inexistente_deve_retornar_404()
    {
        var response = await client.DeleteAsync($"/personagens/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DELETE_personagem_com_id_invalido_deve_retornar_400()
    {
        var response = await client.DeleteAsync("/personagens/id-invalido");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DELETE_personagem_nao_deve_remover_classe_compartilhada()
    {
        var classesAntes = await client.GetFromJsonAsync<IReadOnlyList<ClasseResumoDto>>("/classes");
        var criado = await CriarPersonagemAsync();
        await client.DeleteAsync($"/personagens/{criado.Id}");
        var classesDepois = await client.GetFromJsonAsync<IReadOnlyList<ClasseResumoDto>>("/classes");

        classesDepois.Should().HaveSameCount(classesAntes!);
        classesDepois!.Should().Contain(classe => classe.Classe == ClasseDeHeroi.Cruzado);
    }

    private async Task<PersonagemDetalheDto> CriarPersonagemAsync()
    {
        var response = await client.PostAsJsonAsync("/personagens", new CriarPersonagemRequest(
            Nome: $"Excluir {Guid.NewGuid():N}",
            Classe: ClasseDeHeroi.Cruzado,
            HpMaximo: 30,
            HpAtual: 30,
            Velocidade: 4,
            Critico: 5,
            DanoBaseMinimo: 6,
            DanoBaseMaximo: 10,
            Movimento: 2,
            BonusDeCritico: 3,
            Tamanho: 1,
            AcoesPorTurno: 1,
            Esquiva: 10,
            Precisao: 5,
            Protecao: 0,
            Nivel: 0,
            Stress: 0,
            ChanceDeVirtude: 25,
            HabilidadesEquipadas: null));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<PersonagemDetalheDto>())!;
    }
}
