using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Classes;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature010;

public sealed class PersonagensCardHabilidadesMidiaTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public PersonagensCardHabilidadesMidiaTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Mesma_habilidade_em_niveis_diferentes_compartilha_url()
    {
        var um = await CriarFiel();
        var dois = await CriarFiel();
        var lista = await client.GetFromJsonAsync<IReadOnlyList<PersonagemResumoDto>>("/personagens");
        var card1 = lista!.Single(p => p.Id == um.Id);
        var card2 = lista.Single(p => p.Id == dois.Id);

        var combate = card1.Habilidades.Where(h => h.Categoria == "Combate").ToArray();
        combate.Should().NotBeEmpty();
        combate.Select(h => h.NumeroDoNivel).Should().Contain(0);
        combate.Select(h => h.NumeroDoNivel).Should().Contain(n => n >= 1);
        foreach (var habilidade in combate)
        {
            var par = card2.Habilidades.Single(h => h.HabilidadeId == habilidade.HabilidadeId);
            habilidade.Midia.Should().NotBeNull();
            par.Midia.Should().NotBeNull();
            habilidade.Midia!.Url.Should().Be(par.Midia!.Url);
        }

        var porId = combate.GroupBy(h => h.HabilidadeId);
        foreach (var grupo in porId)
        {
            grupo.Select(h => h.Midia!.Url).Distinct().Should().HaveCount(1);
        }
    }

    [Fact]
    public async Task Acampamento_sem_png_fica_pendente_e_personagem_sem_habilidades_nao_tem_orfaos()
    {
        var fiel = await CriarFiel();
        var vazio = await client.PostAsJsonAsync("/personagens", new CriarPersonagemRequest(
            Nome: $"Sem skills {Guid.NewGuid():N}",
            Classe: ClasseDeHeroi.Cruzado,
            HpMaximo: 33, HpAtual: 30, Velocidade: 1, Critico: 3,
            DanoBaseMinimo: 6, DanoBaseMaximo: 12, Movimento: 2, BonusDeCritico: 0,
            Tamanho: 1, AcoesPorTurno: 1, Esquiva: 5, Precisao: 0, Protecao: 0, Nivel: 0,
            Stress: 12, ChanceDeVirtude: 25));
        vazio.StatusCode.Should().Be(HttpStatusCode.Created);
        var detalheVazio = await vazio.Content.ReadFromJsonAsync<PersonagemDetalheDto>();

        var lista = await client.GetFromJsonAsync<IReadOnlyList<PersonagemResumoDto>>("/personagens");
        var cardFiel = lista!.Single(p => p.Id == fiel.Id);
        var cardVazio = lista.Single(p => p.Id == detalheVazio!.Id);

        var acampamento = cardFiel.Habilidades.Where(h => h.Categoria == "Acampamento").ToArray();
        acampamento.Should().NotBeEmpty();
        foreach (var habilidade in acampamento)
        {
            habilidade.Nome.Should().NotBeNullOrWhiteSpace();
            habilidade.Midia.Should().NotBeNull();
            habilidade.Midia!.Status.Should().BeOneOf("Pendente", "OK");
        }

        cardVazio.Habilidades.Should().BeEmpty();
    }

    private async Task<PersonagemDetalheDto> CriarFiel()
    {
        var response = await client.PostAsJsonAsync("/personagens", new CriarPersonagemRequest(
            Nome: $"Skills {Guid.NewGuid():N}",
            Classe: ClasseDeHeroi.Cruzado,
            HpMaximo: 33, HpAtual: 30, Velocidade: 1, Critico: 3,
            DanoBaseMinimo: 6, DanoBaseMaximo: 12, Movimento: 2, BonusDeCritico: 0,
            Tamanho: 1, AcoesPorTurno: 1, Esquiva: 5, Precisao: 0, Protecao: 0, Nivel: 0,
            Stress: 12, ChanceDeVirtude: 25,
            NivelDaArma: 1,
            NivelDaArmadura: 1));
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var detalhe = await response.Content.ReadFromJsonAsync<PersonagemDetalheDto>();
        detalhe.Should().NotBeNull();
        return detalhe!;
    }
}
