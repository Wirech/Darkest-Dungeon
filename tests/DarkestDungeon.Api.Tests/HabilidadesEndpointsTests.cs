using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Classes;
using DarkestDungeon.Application.Habilidades;
using DarkestDungeon.Domain.Habilidades;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests;

public class HabilidadesEndpointsTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public HabilidadesEndpointsTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    private async Task<Guid> ObterClasseIdAsync(DarkestDungeon.Domain.Classes.ClasseDeHeroi classe)
    {
        var lista = await client.GetFromJsonAsync<IReadOnlyList<ClasseResumoDto>>("/classes");
        return lista!.Single(c => c.Classe == classe).Id;
    }

    [Fact]
    public async Task POST_habilidade_combate_deve_criar_e_retornar_201()
    {
        var cruzadoId = await ObterClasseIdAsync(DarkestDungeon.Domain.Classes.ClasseDeHeroi.Cruzado);
        var request = new CriarHabilidadeDeCombateRequest(
            NomeExibicao: $"Testar Fé {Guid.NewGuid():N}",
            NomeOriginal: "Test Faith",
            Descricao: "Golpe abençoado.",
            ClassesIds: new[] { cruzadoId },
            PosicoesValidas: new[] { 1, 2 },
            PosicoesQueAtinge: new[] { 1, 2 },
            AlvoEmArea: false,
            ModificadorDano: 10,
            ModificadorAcerto: 5,
            ModificadorCritico: 3,
            Efeitos: Array.Empty<EfeitoDeHabilidadeRequest>(),
            LimitePorUso: null);

        var response = await client.PostAsJsonAsync("/habilidades/combate", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var detalhe = await response.Content.ReadFromJsonAsync<HabilidadeDetalheDto>();
        detalhe!.NomeExibicao.Should().Be(request.NomeExibicao);
        detalhe.Categoria.Should().Be(CategoriaDeHabilidadeDto.Combate);
    }

    [Fact]
    public async Task POST_habilidade_acampamento_deve_criar()
    {
        var bandidoId = await ObterClasseIdAsync(DarkestDungeon.Domain.Classes.ClasseDeHeroi.Bandido);
        var request = new CriarHabilidadeDeAcampamentoRequest(
            NomeExibicao: $"Descanso Rápido {Guid.NewGuid():N}",
            NomeOriginal: "Quick Rest",
            Descricao: "Descanso curto.",
            ClassesIds: new[] { bandidoId },
            CustoDeDescanso: 2,
            Alvo: AlvoDeAcampamento.Self,
            Efeitos: Array.Empty<EfeitoDeHabilidadeRequest>(),
            LimitePorUso: null);

        var response = await client.PostAsJsonAsync("/habilidades/acampamento", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task POST_habilidade_inimigo_deve_criar()
    {
        var request = new CriarHabilidadeDeInimigoRequest(
            NomeExibicao: $"Golpe Sombrio {Guid.NewGuid():N}",
            NomeOriginal: "Dark Blow",
            Descricao: "Ataque do inimigo.",
            CondicaoDeAparecer: "Turno 1",
            ChanceDeExecucao: 30,
            Efeitos: Array.Empty<EfeitoDeHabilidadeRequest>());

        var response = await client.PostAsJsonAsync("/habilidades/inimigo", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GET_habilidade_por_id_inexistente_deve_retornar_404()
    {
        var response = await client.GetAsync($"/habilidades/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
