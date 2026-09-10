namespace DarkestDungeon.Domain.Itens;

/// Vínculo visual de um item ou nível com o inventário (padrão 005: id + hash, sem caminho livre).
public sealed class MidiaDeItem
{
    private MidiaDeItem()
    {
        Status = StatusDeMidia.Pendente;
    }

    public MidiaDeItem(string? arquivoInventarioId, string? conjuntoSpineId, string? hashArquivo, StatusDeMidia status)
    {
        if (arquivoInventarioId is not null && arquivoInventarioId.Length > 200)
        {
            throw new ArgumentException("ArquivoInventarioId deve ter no máximo 200 caracteres.", nameof(arquivoInventarioId));
        }

        if (conjuntoSpineId is not null && conjuntoSpineId.Length > 200)
        {
            throw new ArgumentException("ConjuntoSpineId deve ter no máximo 200 caracteres.", nameof(conjuntoSpineId));
        }

        if (hashArquivo is not null && hashArquivo.Length != 64)
        {
            throw new ArgumentException("HashArquivo deve ser um SHA-256 hexadecimal de 64 caracteres.", nameof(hashArquivo));
        }

        if (status == StatusDeMidia.OK
            && (string.IsNullOrWhiteSpace(arquivoInventarioId) || string.IsNullOrWhiteSpace(hashArquivo)))
        {
            throw new ArgumentException("Status OK exige arquivo de inventário e hash SHA-256.");
        }

        ArquivoInventarioId = arquivoInventarioId;
        ConjuntoSpineId = conjuntoSpineId;
        HashArquivo = hashArquivo;
        Status = status;
    }

    public static MidiaDeItem Pendente() => new(null, null, null, StatusDeMidia.Pendente);

    public static MidiaDeItem Ok(string arquivoInventarioId, string hashArquivo, string? conjuntoSpineId = null) =>
        new(arquivoInventarioId, conjuntoSpineId, hashArquivo, StatusDeMidia.OK);

    public string? ArquivoInventarioId { get; private set; }
    public string? ConjuntoSpineId { get; private set; }
    public string? HashArquivo { get; private set; }
    public StatusDeMidia Status { get; private set; }
}
