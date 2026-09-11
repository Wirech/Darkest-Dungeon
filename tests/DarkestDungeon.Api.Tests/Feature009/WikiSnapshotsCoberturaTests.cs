using System.Text.Json;
using DarkestDungeon.Domain.Classes;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature009;

public sealed class WikiSnapshotsCoberturaTests
{
    private static readonly string[] Slugs =
    {
        "abominacao", "antiquario", "besteiro", "cacador-de-recompensas", "cruzado",
        "ladrao-de-cova", "bobo-da-corte", "mestre-de-caca", "leproso", "infernal",
        "bandido", "musqueteiro", "veterano", "ocultista", "medico-da-peste",
        "vestal", "flagelante", "rompedor", "duelista", "fugitivo"
    };

    [Fact]
    public void Snapshots_commitados_devem_cobrir_20_classes_com_5_mais_5_niveis()
    {
        var pasta = EncontrarPasta();
        pasta.Should().NotBeNull("a pasta de snapshots oficiais deve existir");

        foreach (var slug in Slugs)
        {
            var caminho = Path.Combine(pasta!, $"{slug}.json");
            File.Exists(caminho).Should().BeTrue($"faltou {slug}.json");
            using var doc = JsonDocument.Parse(File.ReadAllText(caminho));
            var raiz = doc.RootElement;
            raiz.GetProperty("forma").GetString().Should().Be("humana");
            raiz.GetProperty("classeDeHeroiEnum").GetString().Should().NotBeNullOrWhiteSpace();
            raiz.GetProperty("passosAFrente").ValueKind.Should().Be(JsonValueKind.Number);
            raiz.GetProperty("passosAtras").ValueKind.Should().Be(JsonValueKind.Number);
            raiz.GetProperty("religiosa").ValueKind.Should().BeOneOf(JsonValueKind.True, JsonValueKind.False);
            raiz.GetProperty("provisaoInicial").GetString().Should().NotBeNull();
            var bonus = raiz.GetProperty("bonusAoCritico").GetString();
            bonus.Should().NotBeNullOrWhiteSpace();
            bonus.Should().NotContain("}}");
            raiz.GetProperty("arma").GetProperty("niveis").GetArrayLength().Should().Be(5);
            raiz.GetProperty("armadura").GetProperty("niveis").GetArrayLength().Should().Be(5);
        }

        Enum.GetValues<ClasseDeHeroi>().Should().HaveCount(20);
    }

    [Fact]
    public void Abominacao_deve_ser_somente_forma_humana()
    {
        var pasta = EncontrarPasta();
        if (pasta is null)
        {
            return;
        }

        var caminho = Path.Combine(pasta, "abominacao.json");
        if (!File.Exists(caminho))
        {
            return;
        }

        using var doc = JsonDocument.Parse(File.ReadAllText(caminho));
        doc.RootElement.GetProperty("forma").GetString().Should().Be("humana");
    }

    private static string? EncontrarPasta()
    {
        var atual = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (atual is not null)
        {
            var candidato = Path.Combine(atual.FullName, "specs", "009-atributos-oficiais-personagem", "wiki-snapshots");
            if (Directory.Exists(candidato))
            {
                return candidato;
            }

            atual = atual.Parent;
        }

        return null;
    }
}
