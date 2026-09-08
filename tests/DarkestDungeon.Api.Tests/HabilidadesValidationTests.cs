using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Classes;
using DarkestDungeon.Domain.Habilidades;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests;

public class HabilidadesValidationTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public HabilidadesValidationTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    private async Task<Guid> ObterClasseIdAsync(DarkestDungeon.Domain.Classes.ClasseDeHeroi classe)
    {
        var lista = await client.GetFromJsonAsync<IReadOnlyList<ClasseResumoDto>>("/classes");
        return lista!.Single(c => c.Classe == classe).Id;
    }

    [Fact]
    public async Task POST_habilidade_com_nome_duplicado_deve_retornar_400()
    {
        var cruzadoId = await ObterClasseIdAsync(DarkestDungeon.Domain.Classes.ClasseDeHeroi.Cruzado);
        var nome = $"Golpe Único {Guid.NewGuid():N}";
        var request = new CriarHabilidadeDeCombateRequest(nome, "Unique Strike", "Descrição", new[] { cruzadoId },
            new[] { 1 }, new[] { 1, 2 }, false, 5, 5, 5, Array.Empty<EfeitoDeHabilidadeRequest>(), null);

        var primeiro = await client.PostAsJsonAsync("/habilidades/combate", request);
        primeiro.StatusCode.Should().Be(HttpStatusCode.Created);

        var segundo = await client.PostAsJsonAsync("/habilidades/combate", request);
        segundo.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var erro = await segundo.Content.ReadFromJsonAsync<ErroResponse>();
        erro!.Mensagem.Should().Contain("Já existe");
    }

    [Fact]
    public async Task POST_habilidade_de_combate_sem_classes_deve_retornar_400()
    {
        var request = new CriarHabilidadeDeCombateRequest(
            $"Sem Classe {Guid.NewGuid():N}", "No Class", "Descrição",
            Array.Empty<Guid>(), new[] { 1 }, new[] { 1 }, false, 0, 0, 0,
            Array.Empty<EfeitoDeHabilidadeRequest>(), null);

        var response = await client.PostAsJsonAsync("/habilidades/combate", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task POST_habilidade_com_posicao_invalida_deve_retornar_400()
    {
        var cruzadoId = await ObterClasseIdAsync(DarkestDungeon.Domain.Classes.ClasseDeHeroi.Cruzado);
        var request = new CriarHabilidadeDeCombateRequest(
            $"Posição Inválida {Guid.NewGuid():N}", "Invalid Position", "Descrição",
            new[] { cruzadoId }, new[] { 7 }, new[] { 1 }, false, 0, 0, 0,
            Array.Empty<EfeitoDeHabilidadeRequest>(), null);

        var response = await client.PostAsJsonAsync("/habilidades/combate", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
