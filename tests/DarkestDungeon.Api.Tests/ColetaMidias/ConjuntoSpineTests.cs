using FluentAssertions;
using DarkestDungeon.MediaCollector.Spine;

namespace DarkestDungeon.Api.Tests.ColetaMidias;

public sealed class ConjuntoSpineTests
{
    [Fact]
    public void Construir_AssociaAtlasTexturaEsqueletoERegioes()
    {
        var diretorio = Path.Combine(Path.GetTempPath(), "spine");
        var atlas = Path.Combine(diretorio, "hero.sprite.idle.atlas");
        var skel = Path.Combine(diretorio, "hero.sprite.idle.skel");
        var textura = Path.Combine(diretorio, "hero.sprite.idle.png");
        var hashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [atlas] = "aaa",
            [textura] = "bbb",
            [skel] = "ccc",
        };
        var resultado = ConstrutorDeConjuntosSpine.Construir("Antiquário", atlas, "hero.sprite.idle.png\nsize: 1,1\n\nbody\n  xy: 0,0\n", [atlas, skel], hashes);

        resultado.Classe.Should().Be("Antiquário");
        resultado.Textura.Should().Be(textura);
        resultado.Esqueleto.Should().Be(skel);
        resultado.Regioes.Should().ContainSingle().Which.Should().Be("body");
        resultado.Estado.Should().Be("idle");
        resultado.AtlasSha256.Should().Be("aaa");
        resultado.TexturaSha256.Should().Be("bbb");
        resultado.EsqueletoSha256.Should().Be("ccc");
    }

    [Fact]
    public async Task Construir_LeFixtureEstaticaDoDisco()
    {
        var diretorio = Path.Combine(AppContext.BaseDirectory, "ColetaMidias", "Fixtures", "Spine");
        var atlas = Path.Combine(diretorio, "heroi.sprite.idle.atlas");
        var conteudo = await File.ReadAllTextAsync(atlas);
        var arquivos = Directory.EnumerateFiles(diretorio).ToArray();

        var resultado = ConstrutorDeConjuntosSpine.Construir("Antiquário", atlas, conteudo, arquivos);

        resultado.Textura.Should().EndWith("heroi.sprite.idle.png");
        resultado.Esqueleto.Should().EndWith("heroi.sprite.idle.skel");
        resultado.Regioes.Should().BeEquivalentTo("regiao_a", "regiao_b");
    }
}