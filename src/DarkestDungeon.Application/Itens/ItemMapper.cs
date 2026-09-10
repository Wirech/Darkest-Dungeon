using DarkestDungeon.Application.Itens;
using DarkestDungeon.Application.Midias;
using DarkestDungeon.Domain.Itens;

namespace DarkestDungeon.Application.Itens;

internal static class ItemMapper
{
    public static ItemDetalheDto ParaDto(Item item) => item switch
    {
        Arma arma => new ItemDetalheDto(
            arma.Id,
            TipoDeItemDto.Arma,
            arma.NomeExibicao,
            arma.NomeOriginal,
            arma.Descricao,
            arma.ClasseElegivel,
            arma.Niveis.Select(n => new ArmaNivelDto(n.Nivel, n.DanoMinimo, n.DanoMaximo, n.Critico, n.Velocidade, MidiaDeItemDtoMapper.De(n.Midia))).ToArray(),
            null,
            null,
            null,
            null,
            null,
            null),
        Armadura armadura => new ItemDetalheDto(
            armadura.Id,
            TipoDeItemDto.Armadura,
            armadura.NomeExibicao,
            armadura.NomeOriginal,
            armadura.Descricao,
            armadura.ClasseElegivel,
            null,
            armadura.Niveis.Select(n => new ArmaduraNivelDto(n.Nivel, n.HpAdicional, n.Esquiva, MidiaDeItemDtoMapper.De(n.Midia))).ToArray(),
            null,
            null,
            null,
            null,
            null),
        Acessorio acessorio => new ItemDetalheDto(
            acessorio.Id,
            TipoDeItemDto.Acessorio,
            acessorio.NomeExibicao,
            acessorio.NomeOriginal,
            acessorio.Descricao,
            null,
            null,
            null,
            acessorio.Raridade,
            acessorio.ClasseExclusiva,
            acessorio.ConjuntoId,
            acessorio.Efeitos.Select(e => new EfeitoDeAcessorioDto(e.Nome, e.Valor, e.Unidade, e.Sinal)).ToArray(),
            MidiaDeItemDtoMapper.De(acessorio.Midia)),
        ItemDeAcampamento acampamento => new ItemDetalheDto(
            acampamento.Id,
            TipoDeItemDto.ItemDeAcampamento,
            acampamento.NomeExibicao,
            acampamento.NomeOriginal,
            acampamento.Descricao,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            MidiaDeItemDtoMapper.De(acampamento.Midia)),
        Consumivel consumivel => new ItemDetalheDto(
            consumivel.Id,
            TipoDeItemDto.Consumivel,
            consumivel.NomeExibicao,
            consumivel.NomeOriginal,
            consumivel.Descricao,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            MidiaDeItemDtoMapper.De(consumivel.Midia)),
        _ => throw new InvalidOperationException($"Tipo de Item inesperado: {item.GetType().Name}"),
    };
}
