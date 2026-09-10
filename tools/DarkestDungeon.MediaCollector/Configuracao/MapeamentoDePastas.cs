using System.Text.Json;
using System.Text.RegularExpressions;

namespace DarkestDungeon.MediaCollector.Configuracao;

public sealed class OrigemDeMapeamento
{
    public string Categoria { get; set; } = string.Empty;
    public string[] Globais { get; set; } = [];
    public string? FiltroNome { get; set; }
    public string[] Extensoes { get; set; } = [];
}

public sealed class DocumentoDeMapeamento
{
    public OrigemDeMapeamento[] Origens { get; set; } = [];
}

public sealed class ClassificacaoDeArquivo
{
    public string Categoria { get; init; } = "NaoAssociado";
    public string? Classe { get; init; }
}

public static class MapeamentoDePastas
{
    public static DocumentoDeMapeamento Carregar(string? caminho)
    {
        var efetivo = caminho
            ?? Path.Combine(AppContext.BaseDirectory, "Configuracao", "mapeamento-pastas.json");
        if (!File.Exists(efetivo))
        {
            var aoLado = Path.Combine(AppContext.BaseDirectory, "mapeamento-pastas.json");
            efetivo = File.Exists(aoLado) ? aoLado : Path.Combine("tools", "DarkestDungeon.MediaCollector", "Configuracao", "mapeamento-pastas.json");
        }

        if (!File.Exists(efetivo))
        {
            efetivo = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "tools", "DarkestDungeon.MediaCollector", "Configuracao", "mapeamento-pastas.json"));
        }

        if (!File.Exists(efetivo))
        {
            throw new FileNotFoundException("Arquivo de mapeamento de pastas não encontrado.", caminho);
        }

        var json = File.ReadAllText(efetivo);
        return JsonSerializer.Deserialize<DocumentoDeMapeamento>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? new DocumentoDeMapeamento();
    }

    public static bool CaminhoCasaComGlob(string relativo, string glob)
    {
        var normalizado = relativo.Replace('\\', '/').TrimStart('/');
        var globNormalizado = glob.Replace('\\', '/');
        var nucleo = globNormalizado.EndsWith("/**", StringComparison.Ordinal)
            ? globNormalizado[..^3]
            : globNormalizado;
        var padrao = "^" + Regex.Escape(nucleo)
            .Replace("\\*\\*", ".*")
            .Replace("\\*", "[^/]*");
        if (globNormalizado.EndsWith("/**", StringComparison.Ordinal))
        {
            padrao += "(/.*)?";
        }

        padrao += "$";
        return Regex.IsMatch(normalizado, padrao, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }

    public static bool PassaNoFiltro(string nomeArquivo, string? filtroNome)
    {
        if (string.IsNullOrWhiteSpace(filtroNome))
        {
            return true;
        }

        foreach (var termo in filtroNome.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (nomeArquivo.Contains(termo, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    public static string? ExtrairPastaDeHeroi(string relativo)
    {
        var partes = relativo.Replace('\\', '/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        for (var i = 0; i < partes.Length - 1; i++)
        {
            if (partes[i].Equals("heroes", StringComparison.OrdinalIgnoreCase))
            {
                return partes[i + 1];
            }
        }

        return null;
    }

    public static ClassificacaoDeArquivo Classificar(string relativo, DocumentoDeMapeamento documento)
    {
        var nome = Path.GetFileName(relativo);
        var extensao = Path.GetExtension(relativo);
        foreach (var origem in documento.Origens)
        {
            if (!origem.Extensoes.Contains(extensao, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!origem.Globais.Any(glob => CaminhoCasaComGlob(relativo, glob)))
            {
                continue;
            }

            if (!PassaNoFiltro(nome, origem.FiltroNome))
            {
                continue;
            }

            return new ClassificacaoDeArquivo
            {
                Categoria = origem.Categoria,
                Classe = ExtrairPastaDeHeroi(relativo),
            };
        }

        return new ClassificacaoDeArquivo { Categoria = "NaoAssociado" };
    }

    public static IEnumerable<string> PastasBaseDaOrigem(string raiz, OrigemDeMapeamento origem)
    {
        foreach (var glob in origem.Globais)
        {
            var cortado = glob.Replace('\\', '/').Replace("/**", string.Empty);
            if (cortado.Contains("**/", StringComparison.Ordinal))
            {
                var depois = cortado[(cortado.IndexOf("**/", StringComparison.Ordinal) + 3)..];
                var dlc = Path.Combine(raiz, "dlc");
                if (Directory.Exists(dlc))
                {
                    foreach (var pastaDlc in Directory.EnumerateDirectories(dlc))
                    {
                        yield return Path.Combine(pastaDlc, depois.Replace('/', Path.DirectorySeparatorChar));
                    }
                }

                continue;
            }

            yield return Path.Combine(raiz, cortado.Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
