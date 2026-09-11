using System.Text.Json;
using FluentAssertions;
using DarkestDungeon.MediaCollector.Inventario;

namespace DarkestDungeon.Api.Tests.ColetaMidias;

public sealed class GeradorDoManifestoDeCampingTests
{
    [Fact]
    public void Aliases_MapeiamNomesWikiParaStemDoJogo()
    {
        AliasesDeCamping.IdDoJogo("Wound Care").Should().Be("first_aid");
        AliasesDeCamping.IdDoJogo("Snuff Box").Should().Be("forage");
        AliasesDeCamping.IdDoJogo("Again!").Should().Be("again");
        AliasesDeCamping.NomeDoArquivo("Encourage").Should().Be("camp_skill_encourage.png");
        AliasesDeCamping.Leftovers.Should().Contain("bandage");
    }

    [Fact]
    public async Task AtualizarAsync_Acrescenta79AssociacoesEPreservaCombate()
    {
        var saida = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(saida);
        try
        {
            var combate = new ManifestoDeHabilidades(1, [
                new AssociacaoDeHabilidade("Cruzado", "crusader.ability.one.png", ["Smite"]),
            ]);
            await File.WriteAllTextAsync(
                Path.Combine(saida, "manifesto-habilidades.json"),
                JsonSerializer.Serialize(combate, new JsonSerializerOptions { WriteIndented = true }));

            await GeradorDoManifestoDeCamping.AtualizarAsync(saida, CancellationToken.None);

            var lido = await LeitorDoManifesto.LerAsync(Path.Combine(saida, "manifesto-habilidades.json"), CancellationToken.None);
            lido.Associacoes.Should().Contain(a => a.Arquivo == "crusader.ability.one.png" && a.Habilidades.Contains("Smite"));
            var camping = lido.Associacoes.Where(a => a.Arquivo.StartsWith("camp_skill_")).ToArray();
            camping.Should().HaveCount(79);
            camping.Should().Contain(a => a.Habilidades.Contains("Wound Care") && a.Arquivo == "camp_skill_first_aid.png");
            camping.Should().NotContain(a => a.Arquivo.Contains("bandage"));
            camping.SelectMany(a => a.Habilidades).Should().NotContain("bandage");
        }
        finally
        {
            if (Directory.Exists(saida)) Directory.Delete(saida, true);
        }
    }
}
