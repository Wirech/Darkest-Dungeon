using DarkestDungeon.Domain.Itens;
using FluentAssertions;

namespace DarkestDungeon.Domain.Tests.Feature006;

public sealed class AcessorioNovoDeTrinketTests
{
    [Fact]
    public void Acessorio_de_trinket_novo_e_comum_sem_efeitos_classe_ou_conjunto()
    {
        var id = Guid.NewGuid();
        var acessorio = new Acessorio(
            "lucky_test_amulet",
            "lucky_test_amulet",
            "lucky_test_amulet",
            RaridadeDeAcessorio.Comum,
            Array.Empty<EfeitoDeAcessorio>(),
            classeExclusiva: null,
            conjuntoId: null,
            id: id);

        acessorio.Raridade.Should().Be(RaridadeDeAcessorio.Comum);
        acessorio.Efeitos.Should().BeEmpty();
        acessorio.ClasseExclusiva.Should().BeNull();
        acessorio.ConjuntoId.Should().BeNull();
        acessorio.Id.Should().Be(id);
        acessorio.Midia.Status.Should().Be(StatusDeMidia.Pendente);
    }

    [Fact]
    public void DefinirMidia_nao_altera_raridade_nem_efeitos_de_acessorio_existente()
    {
        var efeito = new EfeitoDeAcessorio("Precisão", 5m, UnidadeDeEfeitoDeAcessorio.Percentual, SinalDeEfeito.Positivo);
        var acessorio = new Acessorio(
            "Amuleto Ancestral",
            "ancestral_amulet",
            "Seed 003",
            RaridadeDeAcessorio.Rara,
            [efeito],
            classeExclusiva: null,
            conjuntoId: Guid.NewGuid());

        acessorio.DefinirMidia(MidiaDeItem.Ok("arquivos/acessorio/ancestral.png", new string('a', 64)));

        acessorio.Raridade.Should().Be(RaridadeDeAcessorio.Rara);
        acessorio.Efeitos.Should().ContainSingle().Which.Nome.Should().Be("Precisão");
        acessorio.Midia.Status.Should().Be(StatusDeMidia.OK);
    }
}
