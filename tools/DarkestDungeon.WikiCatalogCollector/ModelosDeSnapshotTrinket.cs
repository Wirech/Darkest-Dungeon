namespace DarkestDungeon.WikiCatalogCollector;

internal sealed class SnapshotDeTrinketOficial
{
    public string NomeOriginal { get; set; } = string.Empty;
    public string NomeExibicao { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? Raridade { get; set; }
    public string? ClasseExclusiva { get; set; }
    public string? ConjuntoIdWiki { get; set; }
    public string FonteUrl { get; set; } = string.Empty;
    public List<EfeitoDeTrinketSnapshot> Efeitos { get; set; } = new();
    public List<LacunaDeTrinket> Lacunas { get; set; } = new();
}

internal sealed class EfeitoDeTrinketSnapshot
{
    public string Nome { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Unidade { get; set; } = string.Empty;
    public string Sinal { get; set; } = string.Empty;
}

internal sealed class LacunaDeTrinket
{
    public string TrinketOuPagina { get; set; } = string.Empty;
    public string Campo { get; set; } = string.Empty;
    public string Motivo { get; set; } = string.Empty;
    public string Momento { get; set; } = string.Empty;
}

internal sealed record ResultadoParseTrinket(SnapshotDeTrinketOficial? Snapshot, IReadOnlyList<LacunaDeTrinket> Lacunas);
