using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Classes;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature009;

public sealed class PersonagensLegadoSemRecalculoTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public PersonagensLegadoSemRecalculoTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_legado_deve_manter_hp_e_resistencias_gravados()
    {
        var criado = await client.PostAsJsonAsync("/personagens", new CriarPersonagemRequest(
            Nome: $"Legado {Guid.NewGuid():N}",
            Classe: ClasseDeHeroi.Cruzado,
            HpMaximo: 22, HpAtual: 18, Velocidade: 9, Critico: 1,
            DanoBaseMinimo: 1, DanoBaseMaximo: 2, Movimento: 2, BonusDeCritico: 0,
            Tamanho: 1, AcoesPorTurno: 1, Esquiva: 4, Precisao: 11, Protecao: 2, Nivel: 0,
            Stress: 40, ChanceDeVirtude: 25));
        criado.StatusCode.Should().Be(HttpStatusCode.Created);
        var detalhe = await criado.Content.ReadFromJsonAsync<PersonagemDetalheDto>();

        var lido = await client.GetFromJsonAsync<PersonagemDetalheDto>($"/personagens/{detalhe!.Id}");
        lido!.HpMaximo.Should().Be(22);
        lido.HpAtual.Should().Be(18);
        lido.Stress.Should().Be(40);
        lido.Precisao.Should().Be(11);
        lido.Protecao.Should().Be(2);
    }
}
