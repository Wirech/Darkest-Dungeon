using System.Text.RegularExpressions;
using FluentAssertions;

namespace DarkestDungeon.Architecture.Tests;

public sealed class WikiIsolamentoTests
{
    private static readonly Regex Wiki = new(@"wiki\.gg", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    [Fact]
    public void Domain_Application_e_Api_nao_devem_consultar_a_wiki()
    {
        var raiz = EncontrarRaiz();
        raiz.Should().NotBeNull();
        var pastas = new[]
        {
            Path.Combine(raiz!, "src", "DarkestDungeon.Domain"),
            Path.Combine(raiz!, "src", "DarkestDungeon.Application"),
            Path.Combine(raiz!, "src", "DarkestDungeon.Api")
        };

        var ofensores = new List<string>();
        foreach (var pasta in pastas)
        {
            foreach (var arquivo in Directory.GetFiles(pasta, "*.cs", SearchOption.AllDirectories))
            {
                if (arquivo.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                    || arquivo.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                {
                    continue;
                }

                var texto = File.ReadAllText(arquivo);
                if (Wiki.IsMatch(texto))
                {
                    ofensores.Add(arquivo);
                }
            }
        }

        ofensores.Should().BeEmpty("o runtime não consulta darkestdungeon.wiki.gg");
    }

    private static string? EncontrarRaiz()
    {
        var atual = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (atual is not null)
        {
            if (File.Exists(Path.Combine(atual.FullName, "DarkestDungeon.sln")))
            {
                return atual.FullName;
            }

            atual = atual.Parent;
        }

        return null;
    }
}
