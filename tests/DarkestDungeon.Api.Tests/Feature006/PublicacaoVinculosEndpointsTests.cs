using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Itens;
using DarkestDungeon.Application.Midias;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Itens;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature006;

public sealed class PublicacaoVinculosEndpointsTests : IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory factory;
    private readonly HttpClient client;

    public PublicacaoVinculosEndpointsTests(ApiTestFactory factory)
    {
        this.factory = factory;
        client = factory.CreateClient();
    }

    [Fact]
    public async Task POST_sem_categoria_retorna_400_PtBr()
    {
        var response = await client.PostAsJsonAsync("/api/midias/publicacao", new { observacao = "teste" });
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Categoria inválida");
        body.Should().Contain("arma, armadura, acessorio, acampamento ou consumivel");
    }

    [Fact]
    public async Task POST_inventario_ausente_retorna_409()
    {
        var response = await client.PostAsJsonAsync("/api/midias/publicacao", new { categoria = "arma" });
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Inventário não encontrado");
    }

    [Fact]
    public async Task POST_arma_retorna_202_e_status_concluida_com_cinco_niveis()
    {
        var arma = await CriarArmaCruzadoAsync(client);
        using var pasta = new TemporaryDirectory();
        var inventario = InventarioDeMidiasTestHelper.GravarInventario(pasta.Path, [
            InventarioDeMidiasTestHelper.Arquivo("Arma", @"C:\dd\heroes\crusader\crusader_weapon_1.png", "arquivos/arma/crusader_weapon_1.png", classe: "crusader"),
            InventarioDeMidiasTestHelper.Arquivo("Arma", @"C:\dd\heroes\crusader\crusader_weapon_2.png", "arquivos/arma/crusader_weapon_2.png", classe: "crusader"),
            InventarioDeMidiasTestHelper.Arquivo("Arma", @"C:\dd\heroes\crusader\crusader_weapon_3.png", "arquivos/arma/crusader_weapon_3.png", classe: "crusader"),
            InventarioDeMidiasTestHelper.Arquivo("Arma", @"C:\dd\heroes\crusader\crusader_weapon_4.png", "arquivos/arma/crusader_weapon_4.png", classe: "crusader"),
            InventarioDeMidiasTestHelper.Arquivo("Arma", @"C:\dd\heroes\crusader\crusader_weapon_5.png", "arquivos/arma/crusader_weapon_5.png", classe: "crusader"),
        ]);
        var http = InventarioDeMidiasTestHelper.ClienteComInventario(factory, inventario);

        var response = await http.PostAsJsonAsync("/api/midias/publicacao", new { categoria = "arma", observacao = "teste" });
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var publicacaoId = json.GetProperty("publicacaoId").GetGuid();
        json.GetProperty("linkStatus").GetString().Should().Be($"/api/midias/publicacao/{publicacaoId}");

        var status = await http.GetAsync($"/api/midias/publicacao/{publicacaoId}");
        status.StatusCode.Should().Be(HttpStatusCode.OK);
        var statusJson = await status.Content.ReadFromJsonAsync<JsonElement>();
        statusJson.GetProperty("estado").GetString().Should().Be("Concluída");

        var detalhe = await http.GetFromJsonAsync<ItemDetalheDto>($"/itens/{arma.Id}");
        detalhe!.NiveisArma.Should().HaveCount(5);
        detalhe.NiveisArma!.Select(n => n.Nivel).Should().Equal(1, 2, 3, 4, 5);
        detalhe.NiveisArma!.All(n => n.Midia!.Status == "OK").Should().BeTrue();
    }

    [Fact]
    public async Task POST_paralelo_mesma_categoria_retorna_409()
    {
        using var pasta = new TemporaryDirectory();
        var inventario = InventarioDeMidiasTestHelper.GravarInventario(pasta.Path, [
            InventarioDeMidiasTestHelper.Arquivo("Arma", @"C:\dd\heroes\crusader\crusader_weapon_1.png", "arquivos/arma/w1.png", classe: "crusader"),
        ]);
        var hash = InventarioDeMidiasTestHelper.HashPng;
        var inventarioModelo = new InventarioDeMidiasDeEquipamento(
            "fixture",
            "decl",
            [
                new ArquivoDeInventarioDeMidia("Arma", hash, @"C:\dd\heroes\crusader\crusader_weapon_1.png", "arquivos/arma/w1.png", 12, false, "crusader", "crusader_weapon_1"),
            ],
            []);
        var http = InventarioDeMidiasTestHelper.ClienteComLeitor(factory, new LeitorDeInventarioAtrasado(inventarioModelo, TimeSpan.FromMilliseconds(400)));

        var t1 = http.PostAsJsonAsync("/api/midias/publicacao", new { categoria = "arma" });
        var t2 = http.PostAsJsonAsync("/api/midias/publicacao", new { categoria = "arma" });
        var respostas = await Task.WhenAll(t1, t2);
        respostas.Select(r => r.StatusCode).Should().Contain(HttpStatusCode.Accepted);
        respostas.Select(r => r.StatusCode).Should().Contain(HttpStatusCode.Conflict);
        var conflito = respostas.Single(r => r.StatusCode == HttpStatusCode.Conflict);
        (await conflito.Content.ReadAsStringAsync()).Should().Contain("Publicação em andamento");
    }

    [Fact]
    public async Task POST_arma_com_spine_preenche_conjunto_sem_exigir_para_ok()
    {
        using var pasta = new TemporaryDirectory();
        var inventario = InventarioDeMidiasTestHelper.GravarInventario(pasta.Path, [
            InventarioDeMidiasTestHelper.Arquivo("Arma", @"C:\dd\heroes\crusader\crusader_weapon_1.png", "arquivos/arma/crusader_weapon_1.png", classe: "crusader"),
            InventarioDeMidiasTestHelper.Arquivo("Arma", @"C:\dd\heroes\crusader\crusader_weapon.atlas", "arquivos/arma/crusader_weapon.atlas", classe: "crusader"),
            InventarioDeMidiasTestHelper.Arquivo("Arma", @"C:\dd\heroes\crusader\crusader_weapon.skel", "arquivos/arma/crusader_weapon.skel", classe: "crusader"),
        ]);
        var http = InventarioDeMidiasTestHelper.ClienteComInventario(factory, inventario);
        var arma = await CriarArmaCruzadoAsync(http);

        var response = await http.PostAsJsonAsync("/api/midias/publicacao", new { categoria = "arma" });
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);

        var detalhe = await http.GetFromJsonAsync<ItemDetalheDto>($"/itens/{arma.Id}");
        detalhe!.NiveisArma.Should().HaveCount(5);
        detalhe.NiveisArma![0].Midia!.Status.Should().Be("OK");
        detalhe.NiveisArma[0].Midia!.ConjuntoSpineId.Should().Be("arquivos/arma/crusader_weapon.atlas");
        detalhe.NiveisArma.Skip(1).All(n => n.Midia!.Status is "OK" or "Pendente").Should().BeTrue();
    }

    [Fact]
    public async Task POST_nao_altera_ids_de_equipamento_do_personagem()
    {
        var personagem = await CriarPersonagemComArmaAsync(client);
        using var pasta = new TemporaryDirectory();
        var inventario = InventarioDeMidiasTestHelper.GravarInventario(pasta.Path, [
            InventarioDeMidiasTestHelper.Arquivo("Arma", @"C:\dd\heroes\crusader\crusader_weapon_1.png", "arquivos/arma/w1.png", classe: "crusader"),
        ]);
        var http = InventarioDeMidiasTestHelper.ClienteComInventario(factory, inventario);

        var antes = await http.GetFromJsonAsync<PersonagemDetalheDto>($"/personagens/{personagem.Id}");
        var response = await http.PostAsJsonAsync("/api/midias/publicacao", new { categoria = "arma" });
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var depois = await http.GetFromJsonAsync<PersonagemDetalheDto>($"/personagens/{personagem.Id}");
        depois!.ArmaEquipadaId.Should().Be(antes!.ArmaEquipadaId);
        depois.ArmaduraEquipadaId.Should().Be(antes.ArmaduraEquipadaId);
        depois.AcessoriosEquipadosIds.Should().Equal(antes.AcessoriosEquipadosIds);
    }

    [Fact]
    public async Task POST_com_leitor_que_falha_retorna_500_rollback()
    {
        var http = InventarioDeMidiasTestHelper.ClienteComLeitor(factory, new LeitorDeInventarioQueFalha());
        var response = await http.PostAsJsonAsync("/api/midias/publicacao", new { categoria = "arma" });
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("rollback aplicado");
    }

    [Fact]
    public async Task GET_publicacao_desconhecida_retorna_404()
    {
        var response = await client.GetAsync($"/api/midias/publicacao/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task POST_publicacao_005_permanece_na_rota_antiga()
    {
        var response = await client.PostAsJsonAsync("/api/publicacao", new { confirmacaoJanelaManutencao = false });
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private static NivelDeArmaRequest[] CincoNiveisArma() =>
        Enumerable.Range(1, 5).Select(n => new NivelDeArmaRequest(n, 5 + n, 8 + n, 4m + n, 3 + n)).ToArray();

    private static async Task<ItemDetalheDto> CriarArmaCruzadoAsync(HttpClient http)
    {
        var response = await http.PostAsJsonAsync("/armas", new CriarArmaRequest(
            $"Espada {Guid.NewGuid():N}", "Crusader Sword", "Descrição", ClasseDeHeroi.Cruzado, CincoNiveisArma()));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ItemDetalheDto>())!;
    }

    private static async Task<PersonagemDetalheDto> CriarPersonagemComArmaAsync(HttpClient http)
    {
        var arma = await CriarArmaCruzadoAsync(http);
        var personagem = await http.PostAsJsonAsync("/personagens", new CriarPersonagemRequest(
            Nome: $"Herói {Guid.NewGuid():N}",
            Classe: ClasseDeHeroi.Cruzado,
            HpMaximo: 30, HpAtual: 30, Velocidade: 4, Critico: 5m,
            DanoBaseMinimo: 6, DanoBaseMaximo: 10, Movimento: 2, BonusDeCritico: 3m,
            Tamanho: 1, AcoesPorTurno: 1, Esquiva: 10m, Precisao: 5m, Protecao: 0m, Nivel: 0,
            Stress: 0, ChanceDeVirtude: 25,
            HabilidadesEquipadas: null));
        var p = await personagem.Content.ReadFromJsonAsync<PersonagemDetalheDto>();
        await http.PostAsJsonAsync($"/personagens/{p!.Id}/equipar", new EquiparPersonagemRequest(arma.Id, null, null));
        return (await http.GetFromJsonAsync<PersonagemDetalheDto>($"/personagens/{p.Id}"))!;
    }
}

internal sealed class TemporaryDirectory : IDisposable
{
    public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString("N"));

    public TemporaryDirectory() => Directory.CreateDirectory(Path);

    public void Dispose()
    {
        if (Directory.Exists(Path)) Directory.Delete(Path, true);
    }
}
