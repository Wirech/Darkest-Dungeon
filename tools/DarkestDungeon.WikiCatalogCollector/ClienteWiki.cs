using System.Net;
using System.Text.RegularExpressions;

namespace DarkestDungeon.WikiCatalogCollector;

internal sealed class ClienteWiki : IDisposable
{
    private const string BaseWiki = "https://darkestdungeon.wiki.gg/wiki/";
    private readonly HttpClient _http;

    public ClienteWiki()
    {
        _http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(45)
        };
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("DarkestDungeon.WikiCatalogCollector/1.0 (catalogo-oficial; local dump)");
    }

    public async Task<(string UrlFinal, string Wikitext)> ObterWikitextAsync(
        string tituloCanonico,
        CancellationToken cancellationToken)
    {
        var url = MontarUrl(tituloCanonico);
        var (urlFinal, texto) = await BaixarAsync(url, cancellationToken);
        var redirect = ParserDeWikitext.ExtrairDestinoDeRedirect(texto);
        var hops = 0;
        while (redirect is not null && hops < 3)
        {
            hops++;
            url = MontarUrl(redirect);
            (urlFinal, texto) = await BaixarAsync(url, cancellationToken);
            redirect = ParserDeWikitext.ExtrairDestinoDeRedirect(texto);
        }

        return (urlFinal, texto);
    }

    private async Task<(string Url, string Texto)> BaixarAsync(string url, CancellationToken cancellationToken)
    {
        using var resposta = await _http.GetAsync(url, cancellationToken);
        if (resposta.StatusCode == HttpStatusCode.TooManyRequests)
        {
            throw new InvalidOperationException($"HTTP 429 em {url}. Encerrar sem inventar dados.");
        }

        if (!resposta.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"HTTP {(int)resposta.StatusCode} em {url}.");
        }

        var texto = await resposta.Content.ReadAsStringAsync(cancellationToken);
        return (url, texto);
    }

    private static string MontarUrl(string titulo)
    {
        var limpo = Regex.Replace(titulo.Trim(), @"\s+", "_");
        if (limpo.Contains("action=", StringComparison.OrdinalIgnoreCase))
        {
            return BaseWiki + Uri.EscapeDataString(limpo);
        }

        return $"{BaseWiki}{limpo}?action=raw";
    }

    public void Dispose() => _http.Dispose();
}
