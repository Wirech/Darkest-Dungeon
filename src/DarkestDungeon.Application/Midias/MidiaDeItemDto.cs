using DarkestDungeon.Domain.Itens;

namespace DarkestDungeon.Application.Midias;

public sealed record MidiaDeItemDto(
    string Status,
    string? ArquivoInventarioId,
    string? ConjuntoSpineId,
    string? HashArquivo);

public sealed record MidiaDeEquipamentoDePersonagemDto(
    int Nivel,
    string Status,
    string? ArquivoInventarioId,
    string? ConjuntoSpineId,
    string? HashArquivo);

public sealed record MidiaDeAcessorioDePersonagemDto(
    Guid AcessorioId,
    string Status,
    string? ArquivoInventarioId,
    string? HashArquivo);

public static class MidiaDeItemDtoMapper
{
    public static MidiaDeItemDto De(MidiaDeItem midia) => new(
        midia.Status.ToString(),
        midia.ArquivoInventarioId,
        midia.ConjuntoSpineId,
        midia.HashArquivo);

    public static int NivelDeMidia(int nivelDeResolucao) => nivelDeResolucao switch
    {
        <= 1 => 1,
        2 => 2,
        3 => 3,
        4 => 4,
        _ => 5,
    };
}
