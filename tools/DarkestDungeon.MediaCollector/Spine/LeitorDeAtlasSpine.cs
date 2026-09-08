namespace DarkestDungeon.MediaCollector.Spine;

public sealed record AtlasSpine(string Textura, IReadOnlyList<string> Regioes);

public static class LeitorDeAtlasSpine
{
    public static AtlasSpine Ler(string conteudo)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(conteudo);
        var linhas = conteudo.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n');
        var textura = linhas.FirstOrDefault(linha => linha.EndsWith(".png", StringComparison.OrdinalIgnoreCase))?.Trim();
        if (string.IsNullOrWhiteSpace(textura))
        {
            throw new InvalidDataException("O atlas não informa uma textura PNG.");
        }

        var regioes = new List<string>();
        var encontrouCabecalho = false;
        foreach (var linhaOriginal in linhas)
        {
            var linha = linhaOriginal.Trim();
            if (linha.Length == 0 || linha.Contains(':'))
            {
                continue;
            }

            if (!encontrouCabecalho && linha.Equals(textura, StringComparison.OrdinalIgnoreCase))
            {
                encontrouCabecalho = true;
                continue;
            }

            if (encontrouCabecalho)
            {
                regioes.Add(linha);
            }
        }

        return new(textura, regioes);
    }
}