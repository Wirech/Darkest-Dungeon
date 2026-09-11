using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Itens;
using DarkestDungeon.Infrastructure.Data.Seeds;
using FluentAssertions;

namespace DarkestDungeon.Domain.Tests.Feature013;

public sealed class AcessorioUpsertOficialTests
{
    [Fact]
    public void Mutator_preserva_id_midia_e_conjunto_e_substitui_efeitos()
    {
        var id = Guid.NewGuid();
        var conjunto = Guid.NewGuid();
        var acessorio = new Acessorio(
            "lucky_test_amulet",
            "lucky_test_amulet",
            "seed",
            RaridadeDeAcessorio.Comum,
            Array.Empty<EfeitoDeAcessorio>(),
            conjuntoId: conjunto,
            id: id);
        acessorio.DefinirMidia(MidiaDeItem.Ok("arquivos/acessorio/lucky.png", new string('a', 64)));

        acessorio.AtualizarCatalogoOficial(
            "Amuleto da Sorte",
            "Lucky Amulet",
            "Efeitos oficiais",
            RaridadeDeAcessorio.Rara,
            ClasseDeHeroi.Cruzado,
            [new EfeitoDeAcessorio("ACC", 5m, UnidadeDeEfeitoDeAcessorio.Pontos, SinalDeEfeito.Positivo)]);

        acessorio.Id.Should().Be(id);
        acessorio.ConjuntoId.Should().Be(conjunto);
        acessorio.Midia.ArquivoInventarioId.Should().Be("arquivos/acessorio/lucky.png");
        acessorio.NomeOriginal.Should().Be("Lucky Amulet");
        acessorio.Raridade.Should().Be(RaridadeDeAcessorio.Rara);
        acessorio.Efeitos.Should().ContainSingle().Which.Nome.Should().Be("ACC");
    }

    [Fact]
    public void Seed_casa_nome_original_sem_duplicar_e_cria_quando_falta()
    {
        var existente = new Acessorio(
            "vela",
            "Ancestor's Candle",
            "vazio",
            RaridadeDeAcessorio.Comum,
            Array.Empty<EfeitoDeAcessorio>(),
            id: Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"));
        var lista = new List<Item> { existente };
        var criados = new List<Acessorio>();

        var aplicados = AcessoriosOficiaisSeed.Aplicar(lista, criados.Add);

        aplicados.Should().BeGreaterThan(0);
        criados.Should().NotContain(a => string.Equals(a.NomeOriginal, "Ancestor's Candle", StringComparison.OrdinalIgnoreCase));
        existente.Id.Should().Be(Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"));
        existente.Efeitos.Should().NotBeEmpty();
        criados.Should().Contain(a => string.Equals(a.NomeOriginal, "Bloodied Fetish", StringComparison.OrdinalIgnoreCase));
        criados.Select(a => a.NomeOriginal).Distinct(StringComparer.OrdinalIgnoreCase).Should().HaveCount(criados.Count);
    }
}
