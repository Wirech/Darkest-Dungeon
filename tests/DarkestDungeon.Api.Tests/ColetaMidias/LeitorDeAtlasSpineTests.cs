using FluentAssertions;
using DarkestDungeon.MediaCollector.Spine;

namespace DarkestDungeon.Api.Tests.ColetaMidias;

public sealed class LeitorDeAtlasSpineTests
{
    [Fact]
    public void Ler_ExtraiTexturaERegioes()
    {
        const string atlas = "hero.png\nsize: 100, 100\nfilter: Linear,Linear\n\ncorpo\n  xy: 1, 2\narma\n  xy: 3, 4\n";

        var resultado = LeitorDeAtlasSpine.Ler(atlas);

        resultado.Textura.Should().Be("hero.png");
        resultado.Regioes.Should().BeEquivalentTo("corpo", "arma");
    }
}