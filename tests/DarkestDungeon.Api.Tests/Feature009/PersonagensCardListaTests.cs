using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Classes;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature009;

public sealed class PersonagensCardListaTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public PersonagensCardListaTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_personagens_deve_incluir_categorias_do_card()
    {
        var criado = await client.PostAsJsonAsync("/personagens", new CriarPersonagemRequest(
            Nome: $"Card {Guid.NewGuid():N}",
            Classe: ClasseDeHeroi.Cruzado,
            HpMaximo: 33, HpAtual: 30, Velocidade: 1, Critico: 3,
            DanoBaseMinimo: 6, DanoBaseMaximo: 12, Movimento: 2, BonusDeCritico: 0,
            Tamanho: 1, AcoesPorTurno: 1, Esquiva: 5, Precisao: 0, Protecao: 0, Nivel: 0,
            Stress: 12, ChanceDeVirtude: 25,
            NivelDaArma: 1,
            NivelDaArmadura: 1));
        criado.StatusCode.Should().Be(HttpStatusCode.Created);
        var detalhe = await criado.Content.ReadFromJsonAsync<PersonagemDetalheDto>();

        var lista = await client.GetFromJsonAsync<IReadOnlyList<PersonagemResumoDto>>("/personagens");
        var card = lista!.Single(p => p.Id == detalhe!.Id);
        card.HpMaximo.Should().BeGreaterThan(0);
        card.Stress.Should().Be(0);
        card.Precisao.Should().Be(0);
        card.Protecao.Should().Be(0);
        card.Esquiva.Should().BeGreaterThanOrEqualTo(0);
        card.Velocidade.Should().BeGreaterThan(0);
        card.Resistencias.Should().NotBeNull();
        card.Aparencia.ToString().Should().NotBeNullOrWhiteSpace();
        card.PassosAFrente.Should().NotBeNull();
        card.PassosAtras.Should().NotBeNull();
        card.ClasseReligiosa.Should().BeTrue();
        card.ProvisaoInicial.Should().NotBeNullOrWhiteSpace();
        card.BonusAoCriticoDaClasse.Should().NotBeNull();
        card.Midias.Should().NotBeNull();
        card.Midias!.Retrato.Should().NotBeNull();
        card.Midias.CorpoInteiro.Should().NotBeNull();
        card.Midias.Arma.Should().NotBeNull();
        card.Midias.Armadura.Should().NotBeNull();
        card.Midias.CorpoInteiro.Versoes.Should().NotBeNull();
        card.Midias.CorpoInteiro.Versoes!.Select(v => v.Id).Should().Contain(new[] { "emEspera", "animado", "caminhada" });
        card.Habilidades.Should().Contain(h => h.Categoria == "Acampamento");
    }
}
