using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Classes;
using DarkestDungeon.Application.Habilidades;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Habilidades;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests;

public class HabilidadesCompartilhadasTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public HabilidadesCompartilhadasTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Habilidade_compartilhada_deve_ser_um_unico_registro_em_multiplas_classes()
    {
        var lista = await client.GetFromJsonAsync<IReadOnlyList<ClasseResumoDto>>("/classes");
        var bandido = lista!.Single(c => c.Classe == ClasseDeHeroi.Bandido);
        var ladrao = lista!.Single(c => c.Classe == ClasseDeHeroi.LadraoDeCova);
        // Cria habilidade compartilhada entre Bandido e Ladrão de Cova
        var request = new CriarHabilidadeDeAcampamentoRequest(
            $"Gallows Humor {Guid.NewGuid():N}",
            "Gallows Humor",
            "Descrição compartilhada.",
            new[] { bandido.Id, ladrao.Id },
            CustoDeDescanso: 2,
            Alvo: AlvoDeAcampamento.PartyInteira,
            Efeitos: Array.Empty<EfeitoDeHabilidadeRequest>(),
            LimitePorUso: null);

        var response = await client.PostAsJsonAsync("/habilidades/acampamento", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var detalhe = await response.Content.ReadFromJsonAsync<HabilidadeDetalheDto>();
        detalhe!.ClassesIds.Should().BeEquivalentTo(new[] { bandido.Id, ladrao.Id });

        // A mesma habilidade deve aparecer em ambas as classes com o mesmo Id
        var habsBandido = await client.GetFromJsonAsync<IReadOnlyList<HabilidadeResumoDto>>($"/classes/{bandido.Id}/habilidades");
        var habsLadrao = await client.GetFromJsonAsync<IReadOnlyList<HabilidadeResumoDto>>($"/classes/{ladrao.Id}/habilidades");

        habsBandido!.Any(h => h.Id == detalhe.Id).Should().BeTrue();
        habsLadrao!.Any(h => h.Id == detalhe.Id).Should().BeTrue();
    }
}
