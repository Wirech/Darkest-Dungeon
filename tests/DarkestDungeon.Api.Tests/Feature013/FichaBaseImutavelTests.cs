using System.Net.Http.Json;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Itens;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature013;

public sealed class FichaBaseImutavelTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient client;

    public FichaBaseImutavelTests(ApiTestFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Equipar_e_desequipar_nao_reescreve_ficha_base()
    {
        var criado = await Feature013Helpers.CriarPersonagemAsync(
            client,
            Feature013Helpers.Personagem(hp: 30, protecao: 0m));
        var hpBase = criado.FichaBase!.HpMaximo;
        var protBase = criado.FichaBase.Protecao;

        var a = await Feature013Helpers.CriarAcessorioAsync(
            client,
            Feature013Helpers.Acessorio(
                "HP 10a",
                null,
                Feature013Helpers.Efeito("MAX HP", 10m)));
        var b = await Feature013Helpers.CriarAcessorioAsync(
            client,
            Feature013Helpers.Acessorio(
                "HP 10b",
                null,
                Feature013Helpers.Efeito("MAX HP", 10m)));

        var equipado = await Feature013Helpers.EquiparEspacoAsync(client, criado.Id, 1, a.Id);
        equipado = await Feature013Helpers.EquiparEspacoAsync(client, criado.Id, 2, b.Id);
        equipado.FichaBase!.HpMaximo.Should().Be(hpBase);
        equipado.FichaBase.Protecao.Should().Be(protBase);
        equipado.FichaEfetiva!.HpMaximo.Should().Be(36);
        equipado.HpMaximo.Should().Be(36);

        var vazio1 = await Feature013Helpers.EquiparEspacoAsync(client, criado.Id, 1, null);
        var vazio2 = await Feature013Helpers.EquiparEspacoAsync(client, criado.Id, 2, null);
        vazio2.FichaBase!.HpMaximo.Should().Be(hpBase);
        vazio2.FichaEfetiva!.HpMaximo.Should().Be(hpBase);
        vazio2.HpMaximo.Should().Be(hpBase);
        vazio1.FichaBase!.HpMaximo.Should().Be(hpBase);
    }
}
