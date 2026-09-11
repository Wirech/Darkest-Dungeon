namespace DarkestDungeon.WikiCatalogCollector;

internal static class ColetorDeCatalogoWiki
{
    internal const string TextoDeUso =
        """
        Coletor de catálogo oficial (curadoria). Baixa wikitext uma vez e grava snapshots JSON.

        Uso:
          dotnet run --project tools/DarkestDungeon.WikiCatalogCollector -- --saida <pasta>

        Argumentos:
          --saida <pasta>   Destino dos 20 arquivos {slug}.json (obrigatório).
          --ajuda           Exibe este texto.

        Regras:
          - Não interpola, não inventa e não copia Besteiro para Musqueteiro.
          - Lacuna (campo ausente, ambíguo ou forma besta sem bloco humano) falha com código ≠ 0.
          - A API de personagens nunca executa este coletor.
        """;

    public static async Task<int> ExecutarAsync(string[] args, CancellationToken cancellationToken = default)
    {
        if (args.Length == 0 || args.Any(arg => arg is "--ajuda" or "-h" or "/?"))
        {
            Console.WriteLine(TextoDeUso);
            return args.Length == 0 ? 1 : 0;
        }

        if (!TentarLerSaida(args, out var pastaSaida, out var erro))
        {
            Console.Error.WriteLine(erro);
            Console.Error.WriteLine();
            Console.Error.WriteLine(TextoDeUso);
            return 1;
        }

        Directory.CreateDirectory(pastaSaida);
        return await CatalogoWikiExecutor.ExecutarColetaAsync(pastaSaida, cancellationToken).ConfigureAwait(false);
    }

    internal static bool TentarLerSaida(string[] args, out string pastaSaida, out string erro)
    {
        pastaSaida = string.Empty;
        erro = string.Empty;

        for (var i = 0; i < args.Length; i++)
        {
            if (!string.Equals(args[i], "--saida", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (i + 1 >= args.Length || string.IsNullOrWhiteSpace(args[i + 1]) || args[i + 1].StartsWith('-'))
            {
                erro = "Informe o caminho da pasta após --saida.";
                return false;
            }

            pastaSaida = Path.GetFullPath(args[i + 1].Trim());
            return true;
        }

        erro = "O argumento --saida é obrigatório.";
        return false;
    }
}
