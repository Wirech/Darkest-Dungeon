namespace DarkestDungeon.WikiCatalogCollector;

internal static class PontoDeEntradaDoColetor
{
    public static async Task<int> Main(string[] args)
    {
        return await ColetorDeCatalogoWiki.ExecutarAsync(args).ConfigureAwait(false);
    }
}

