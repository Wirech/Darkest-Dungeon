using DarkestDungeon.Domain.Itens;

namespace DarkestDungeon.Application.Midias;

public sealed record PublicacaoDeVinculosRequest(string? Categoria, string? Observacao);

public sealed record PublicacaoDeVinculosAceitaDto(
    Guid PublicacaoId,
    string Categoria,
    DateTimeOffset IniciadaEm,
    string LinkStatus);

public sealed record PublicacaoDeVinculosStatusDto(
    Guid PublicacaoId,
    string Categoria,
    string Estado,
    DateTimeOffset IniciadaEm,
    DateTimeOffset? ConcluidaEm,
    int ItensAtualizados,
    int VinculosOk,
    int VinculosPendentes,
    int AcessoriosNovos);

public static class PublicacaoDeVinculosDtoMapper
{
    public static PublicacaoDeVinculosAceitaDto Aceita(ResultadoDePublicacaoDeVinculos resultado) => new(
        resultado.PublicacaoId,
        resultado.Categoria.ToString(),
        resultado.IniciadaEm,
        $"/api/midias/publicacao/{resultado.PublicacaoId}");

    public static PublicacaoDeVinculosStatusDto Status(ResultadoDePublicacaoDeVinculos resultado) => new(
        resultado.PublicacaoId,
        resultado.Categoria.ToString(),
        CategoriaDeMidiaParser.EstadoDeApi(resultado.Estado),
        resultado.IniciadaEm,
        resultado.ConcluidaEm,
        resultado.ItensAtualizados,
        resultado.VinculosOk,
        resultado.VinculosPendentes,
        resultado.AcessoriosNovos);
}

public sealed class PublicacaoDeVinculosFalhouException : Exception
{
    public Guid PublicacaoId { get; }
    public CategoriaDeMidiaDeItem Categoria { get; }

    public PublicacaoDeVinculosFalhouException(Guid publicacaoId, CategoriaDeMidiaDeItem categoria)
        : base("A transação foi revertida. Nenhum vínculo desta categoria permaneceu pela metade.")
    {
        PublicacaoId = publicacaoId;
        Categoria = categoria;
    }
}
