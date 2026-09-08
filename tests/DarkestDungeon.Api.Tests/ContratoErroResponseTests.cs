using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Classes;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests;

public class ContratoErroResponseTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public ContratoErroResponseTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    private static async Task<ErroResponse?> LerErroAsync(HttpResponseMessage response)
    {
        var raw = await response.Content.ReadAsStringAsync();
        raw.Should().NotBeNullOrWhiteSpace($"todo body 400/404 deve conter ErroResponse; recebido: {raw}");
        return await response.Content.ReadFromJsonAsync<ErroResponse>();
    }

    [Fact]
    public async Task GET_classe_inexistente_deve_retornar_ErroResponse_com_mensagem_PT_BR()
    {
        var response = await client.GetAsync($"/classes/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var erro = await LerErroAsync(response);
        erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        erro.Mensagem.Should().Contain("não encontrada");
    }

    [Fact]
    public async Task GET_habilidade_inexistente_deve_retornar_ErroResponse()
    {
        var response = await client.GetAsync($"/habilidades/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var erro = await LerErroAsync(response);
        erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GET_item_inexistente_deve_retornar_ErroResponse()
    {
        var response = await client.GetAsync($"/itens/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var erro = await LerErroAsync(response);
        erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task POST_habilidade_com_classe_inexistente_deve_retornar_400_com_campo_classesIds()
    {
        var request = new CriarHabilidadeDeCombateRequest(
            $"Golpe {Guid.NewGuid():N}", "Strike", "Descrição", new[] { Guid.NewGuid() },
            new[] { 1 }, new[] { 1 }, false, 0, 0, 0, Array.Empty<EfeitoDeHabilidadeRequest>(), null);

        var response = await client.PostAsJsonAsync("/habilidades/combate", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var erro = await LerErroAsync(response);
        erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task POST_armadura_sem_cinco_niveis_deve_retornar_400_com_ErroResponse()
    {
        var request = new CriarArmaduraRequest(
            $"Placa Curta {Guid.NewGuid():N}", "Short", "Descrição",
            DarkestDungeon.Domain.Classes.ClasseDeHeroi.Cruzado,
            new[] { new NivelDeArmaduraRequest(1, 10, 5m) });

        var response = await client.PostAsJsonAsync("/armaduras", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var erro = await LerErroAsync(response);
        erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task POST_acessorio_com_conjunto_valido_e_efeitos_deve_retornar_201_e_nao_erro()
    {
        var request = new CriarAcessorioRequest(
            $"Amuleto {Guid.NewGuid():N}", "Amulet", "Descrição",
            DarkestDungeon.Domain.Itens.RaridadeDeAcessorio.Rara,
            null, null,
            new[] { new EfeitoDeAcessorioRequest("Precisão", 5m, DarkestDungeon.Domain.Itens.UnidadeDeEfeitoDeAcessorio.Percentual, DarkestDungeon.Domain.Itens.SinalDeEfeito.Positivo) });

        var response = await client.PostAsJsonAsync("/acessorios", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task POST_personagem_com_classe_invalida_deve_retornar_400_com_ErroResponse()
    {
        var body = new
        {
            nome = $"Herói {Guid.NewGuid():N}",
            classe = 999,
            hpMaximo = 30, hpAtual = 30, velocidade = 4, critico = 5,
            danoBaseMinimo = 6, danoBaseMaximo = 10, movimento = 2, bonusDeCritico = 3,
            tamanho = 1, acoesPorTurno = 1, esquiva = 10, precisao = 5, protecao = 0, nivel = 0,
            stress = 0, chanceDeVirtude = 25,
            habilidadesEquipadas = Array.Empty<Guid>(),
        };

        var response = await client.PostAsJsonAsync("/personagens", body);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task POST_inimigo_com_resistencias_invalidas_deve_retornar_400_com_ErroResponse()
    {
        var body = new
        {
            nome = $"Inimigo {Guid.NewGuid():N}",
            tipo = DarkestDungeon.Domain.Seres.TipoDeInimigo.Besta,
            hpMaximo = 20, hpAtual = 20, velocidade = 3, critico = 2,
            danoBaseMinimo = 4, danoBaseMaximo = 6, movimento = 2, bonusDeCritico = 0,
            tamanho = 1, acoesPorTurno = 1, esquiva = 5, precisao = 5, protecao = 0, nivel = 0,
            atordoamento = 200m,
            sangramento = 20m, envenenamento = 30m, debuff = 40m, movimentoResistencia = 50m,
            habilidadesIds = Array.Empty<Guid>(),
        };

        var response = await client.PostAsJsonAsync("/inimigos", body);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var erro = await LerErroAsync(response);
        erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
    }
}
