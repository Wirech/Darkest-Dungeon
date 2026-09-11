using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Itens;
using DarkestDungeon.Infrastructure.Data.Seeds;
using FluentAssertions;

namespace DarkestDungeon.Domain.Tests.Feature009;

public sealed class EquipamentosOficiaisTests
{
    [Fact]
    public void Seed_oficial_quando_completo_deve_materializar_20_armas_e_20_armaduras_distintas()
    {
        var itens = EquipamentosSeed.Materializar();
        if (itens.Count == 0)
        {
            return;
        }

        var armas = itens.OfType<Arma>().ToArray();
        var armaduras = itens.OfType<Armadura>().ToArray();
        armas.Should().HaveCount(20);
        armaduras.Should().HaveCount(20);
        armas.Select(a => a.ClasseElegivel).Distinct().Should().HaveCount(20);
        armaduras.Select(a => a.ClasseElegivel).Distinct().Should().HaveCount(20);
        armas.Should().OnlyContain(a => a.Niveis.Count == 5);
        armaduras.Should().OnlyContain(a => a.Niveis.Count == 5);

        var besteiro = armas.Single(a => a.ClasseElegivel == ClasseDeHeroi.Besteiro);
        var musqueteiro = armas.Single(a => a.ClasseElegivel == ClasseDeHeroi.Musqueteiro);
        musqueteiro.Id.Should().NotBe(besteiro.Id);
        musqueteiro.ClasseElegivel.Should().Be(ClasseDeHeroi.Musqueteiro);
    }
}
