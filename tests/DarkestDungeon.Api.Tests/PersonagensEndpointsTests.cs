using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Seres;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests;

public class PersonagensEndpointsTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public PersonagensEndpointsTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    private static CriarPersonagemRequest CriarBase(ClasseDeHeroi classe = ClasseDeHeroi.Cruzado, IReadOnlyCollection<Guid>? habilidades = null) =>
        new(
            Nome: $"Herói {Guid.NewGuid():N}",
            Classe: classe,
            HpMaximo: 30, HpAtual: 30, Velocidade: 4, Critico: 5m,
            DanoBaseMinimo: 6, DanoBaseMaximo: 10, Movimento: 2, BonusDeCritico: 3m,
            Tamanho: 1, AcoesPorTurno: 1, Esquiva: 10m, Precisao: 5m, Protecao: 0m, Nivel: 0,
            Stress: 0, ChanceDeVirtude: 25,
            HabilidadesEquipadas: habilidades);

    [Fact]
    public async Task POST_personagem_com_classe_existente_deve_retornar_201_com_resistencias_copiadas()
    {
        var response = await client.PostAsJsonAsync("/personagens", CriarBase());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var detalhe = await response.Content.ReadFromJsonAsync<PersonagemDetalheDto>();
        detalhe!.Classe.Should().Be(ClasseDeHeroi.Cruzado);
        detalhe.Resistencias.Should().NotBeNull();
    }

    [Fact]
    public async Task GET_personagem_por_id_valido_deve_retornar_200()
    {
        var criado = await client.PostAsJsonAsync("/personagens", CriarBase());
        var personagem = await criado.Content.ReadFromJsonAsync<PersonagemDetalheDto>();

        var response = await client.GetAsync($"/personagens/{personagem!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_personagem_por_id_inexistente_deve_retornar_404()
    {
        var response = await client.GetAsync($"/personagens/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task POST_personagem_com_habilidade_de_outra_classe_deve_retornar_400_FR009a()
    {
        // Cria uma habilidade para Ocultista
        var lista = await client.GetFromJsonAsync<IReadOnlyList<DarkestDungeon.Application.Classes.ClasseResumoDto>>("/classes");
        var ocultistaId = lista!.Single(c => c.Classe == ClasseDeHeroi.Ocultista).Id;
        var habRequest = new CriarHabilidadeDeCombateRequest(
            $"Ocultismo {Guid.NewGuid():N}", "Occultism", "Descrição", new[] { ocultistaId },
            new[] { 1 }, new[] { 1, 2 }, false, 5, 5, 5, Array.Empty<EfeitoDeHabilidadeRequest>(), null);
        var habCriada = await client.PostAsJsonAsync("/habilidades/combate", habRequest);
        var habilidade = await habCriada.Content.ReadFromJsonAsync<DarkestDungeon.Application.Habilidades.HabilidadeDetalheDto>();

        // Tenta atribuir a Cruzado
        var response = await client.PostAsJsonAsync("/personagens",
            CriarBase(ClasseDeHeroi.Cruzado, new[] { habilidade!.Id }));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

public class InimigosEndpointsTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public InimigosEndpointsTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    private static CriarInimigoRequest CriarBase() => new(
        Nome: $"Inimigo {Guid.NewGuid():N}",
        Tipo: TipoDeInimigo.Besta,
        HpMaximo: 20, HpAtual: 20, Velocidade: 3, Critico: 2m,
        DanoBaseMinimo: 4, DanoBaseMaximo: 6, Movimento: 2, BonusDeCritico: 0m,
        Tamanho: 1, AcoesPorTurno: 1, Esquiva: 5m, Precisao: 5m, Protecao: 0m, Nivel: 0,
        Atordoamento: 10, Sangramento: 20, Envenenamento: 30, Debuff: 40, MovimentoResistencia: 50,
        HabilidadesIds: null);

    [Fact]
    public async Task POST_inimigo_deve_retornar_201()
    {
        var response = await client.PostAsJsonAsync("/inimigos", CriarBase());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var detalhe = await response.Content.ReadFromJsonAsync<InimigoDetalheDto>();
        detalhe!.Tipo.Should().Be(TipoDeInimigo.Besta);
    }

    [Fact]
    public async Task GET_inimigo_por_id_inexistente_deve_retornar_404()
    {
        var response = await client.GetAsync($"/inimigos/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
