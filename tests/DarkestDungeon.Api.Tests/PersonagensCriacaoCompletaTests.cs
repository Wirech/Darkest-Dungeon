using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Classes;
using DarkestDungeon.Application.Habilidades;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Classes;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests;

public sealed class PersonagensCriacaoCompletaTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public PersonagensCriacaoCompletaTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task POST_personagem_com_niveis_invalidos_deve_retornar_400()
    {
        var response = await client.PostAsJsonAsync("/personagens", new
        {
            nome = "Herói inválido",
            classe = ClasseDeHeroi.Cruzado,
            nivel = 3,
            nivelDaArma = 0,
            nivelDaArmadura = 6,
            aparencia = 2
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task POST_personagem_deve_inicializar_habilidades_4_mais_4_quando_catalogo_de_itens_estiver_disponivel()
    {
        var classe = await ObterClasseAsync(ClasseDeHeroi.Cruzado);
        await CriarArmaAsync(ClasseDeHeroi.Cruzado);
        await CriarArmaduraAsync(ClasseDeHeroi.Cruzado);

        var habilidadesDaClasse = await client.GetFromJsonAsync<IReadOnlyList<HabilidadeResumoDto>>($"/classes/{classe.Id}/habilidades");
        var combateIds = habilidadesDaClasse!
            .Where(h => h.Categoria == CategoriaDeHabilidadeDto.Combate)
            .Select(h => h.Id)
            .ToHashSet();
        var acampamentoIds = habilidadesDaClasse!
            .Where(h => h.Categoria == CategoriaDeHabilidadeDto.Acampamento)
            .Select(h => h.Id)
            .ToHashSet();
        combateIds.Count.Should().BeGreaterThanOrEqualTo(4);
        acampamentoIds.Count.Should().BeGreaterThanOrEqualTo(4);

        var response = await client.PostAsJsonAsync("/personagens", new
        {
            nome = $"Cruzado completo {Guid.NewGuid():N}",
            classe = ClasseDeHeroi.Cruzado,
            nivel = 3,
            nivelDaArma = 2,
            nivelDaArmadura = 3,
            aparencia = 2
        });

        var responseBody = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Created, responseBody);
        var personagem = await response.Content.ReadFromJsonAsync<PersonagemDetalheDto>();
        personagem.Should().NotBeNull();
        personagem!.Aparencia.ToString().Should().Be("C");
        personagem.NivelDaArma.Should().Be(2);
        personagem.NivelDaArmadura.Should().Be(3);
        personagem.HpMaximo.Should().BeGreaterThan(0);
        personagem.HpAtual.Should().Be(personagem.HpMaximo);

        var habilidadesCombate = personagem.Habilidades.Where(h => combateIds.Contains(h.HabilidadeId)).ToArray();
        var habilidadesAcampamento = personagem.Habilidades.Where(h => acampamentoIds.Contains(h.HabilidadeId)).ToArray();
        habilidadesCombate.Count(h => h.NumeroDoNivel == 1).Should().Be(4);
        habilidadesAcampamento.Count(h => h.NumeroDoNivel == 1).Should().Be(4);
        habilidadesCombate.Concat(habilidadesAcampamento).Should().OnlyContain(h => h.NumeroDoNivel == 0 || h.NumeroDoNivel == 1);
    }

    private async Task<ClasseResumoDto> ObterClasseAsync(ClasseDeHeroi classe)
    {
        var classes = await client.GetFromJsonAsync<IReadOnlyList<ClasseResumoDto>>("/classes");
        return classes!.Single(item => item.Classe == classe);
    }

    private async Task CriarArmaAsync(ClasseDeHeroi classe)
    {
        var response = await client.PostAsJsonAsync("/armas", new CriarArmaRequest(
            $"Arma de teste {Guid.NewGuid():N}", "Test Weapon", "Arma", classe,
            Enumerable.Range(1, 5).Select(n => new NivelDeArmaRequest(n, n, n + 3, 1, 1)).ToArray()));
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    private async Task CriarArmaduraAsync(ClasseDeHeroi classe)
    {
        var response = await client.PostAsJsonAsync("/armaduras", new CriarArmaduraRequest(
            $"Armadura de teste {Guid.NewGuid():N}", "Test Armor", "Armadura", classe,
            Enumerable.Range(1, 5).Select(n => new NivelDeArmaduraRequest(n, n, 1)).ToArray()));
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
