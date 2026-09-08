namespace DarkestDungeon.MediaCollector.Spine;

public sealed record ConjuntoSpine(
    string Classe,
    string Nome,
    string Atlas,
    string Textura,
    string? Esqueleto,
    string Estado,
    int? Paleta,
    IReadOnlyList<string> Regioes,
    string? AtlasSha256,
    string? TexturaSha256,
    string? EsqueletoSha256);

public static class ConstrutorDeConjuntosSpine
{
    public static ConjuntoSpine Construir(
        string classe,
        string caminhoAtlas,
        string conteudoAtlas,
        IEnumerable<string> arquivos,
        IReadOnlyDictionary<string, string>? hashesPorCaminho = null)
    {
        var atlas = LeitorDeAtlasSpine.Ler(conteudoAtlas);
        var diretorio = Path.GetDirectoryName(caminhoAtlas)!;
        var textura = Path.Combine(diretorio, atlas.Textura);
        var baseNome = Path.GetFileNameWithoutExtension(caminhoAtlas);
        var esqueleto = arquivos.FirstOrDefault(caminho => Path.GetFileNameWithoutExtension(caminho).Equals(baseNome, StringComparison.OrdinalIgnoreCase) && Path.GetExtension(caminho).Equals(".skel", StringComparison.OrdinalIgnoreCase));
        var estado = baseNome.Split('.').Last();
        int? paleta = int.TryParse(baseNome.Split('.').LastOrDefault(parte => parte.All(char.IsDigit)), out var valor) ? valor : null;
        var hashes = hashesPorCaminho ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        return new(
            classe,
            baseNome,
            caminhoAtlas,
            textura,
            esqueleto,
            estado,
            paleta,
            atlas.Regioes,
            hashes.GetValueOrDefault(caminhoAtlas),
            hashes.GetValueOrDefault(textura),
            esqueleto is null ? null : hashes.GetValueOrDefault(esqueleto));
    }
}