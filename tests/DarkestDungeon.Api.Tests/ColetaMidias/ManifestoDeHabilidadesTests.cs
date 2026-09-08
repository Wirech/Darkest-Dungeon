using System.Text.Json;
using FluentAssertions;
using DarkestDungeon.MediaCollector.Inventario;

namespace DarkestDungeon.Api.Tests.ColetaMidias;

public sealed class ManifestoDeHabilidadesTests
{
    [Fact]
    public async Task LerAsync_AceitaPropriedadesEmCamelCase()
    {
        var caminho = Path.Combine(Path.GetTempPath(), $"manifesto-{Guid.NewGuid()}.json");
        var manifesto = new
        {
            versao = 1,
            associacoes = new[]
            {
                new { classe = "Antiquário", arquivo = "antiquarian.ability.one.png", habilidades = new[] { "Nervous Stab" } }
            }
        };
        await File.WriteAllTextAsync(caminho, JsonSerializer.Serialize(manifesto));
        try
        {
            var lido = await LeitorDoManifesto.LerAsync(caminho, CancellationToken.None);

            lido.Versao.Should().Be(1);
            lido.Associacoes.Should().ContainSingle();
            lido.Associacoes[0].Classe.Should().Be("Antiquário");
            lido.Associacoes[0].Arquivo.Should().Be("antiquarian.ability.one.png");
            lido.Associacoes[0].Habilidades.Should().ContainSingle().Which.Should().Be("Nervous Stab");
        }
        finally
        {
            if (File.Exists(caminho)) File.Delete(caminho);
        }
    }

    [Fact]
    public async Task LerAsync_RejeitaJsonVazio()
    {
        var caminho = Path.Combine(Path.GetTempPath(), $"vazio-{Guid.NewGuid()}.json");
        await File.WriteAllTextAsync(caminho, "null");
        try
        {
            await FluentActions.Awaiting(() => LeitorDoManifesto.LerAsync(caminho, CancellationToken.None))
                .Should().ThrowAsync<InvalidDataException>();
        }
        finally
        {
            if (File.Exists(caminho)) File.Delete(caminho);
        }
    }
}
