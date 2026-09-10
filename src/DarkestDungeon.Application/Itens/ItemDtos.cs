using DarkestDungeon.Application.Midias;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Itens;

namespace DarkestDungeon.Application.Itens;

public sealed record ArmaNivelDto(int Nivel, int DanoMinimo, int DanoMaximo, decimal Critico, int Velocidade, MidiaDeItemDto? Midia = null);

public sealed record ArmaduraNivelDto(int Nivel, int HpAdicional, decimal Esquiva, MidiaDeItemDto? Midia = null);

public sealed record EfeitoDeAcessorioDto(string Nome, decimal Valor, UnidadeDeEfeitoDeAcessorio Unidade, SinalDeEfeito Sinal);

public enum TipoDeItemDto
{
    Arma,
    Armadura,
    Acessorio,
    ItemDeAcampamento,
    Consumivel,
}

public sealed record ItemDetalheDto(
    Guid Id,
    TipoDeItemDto Tipo,
    string NomeExibicao,
    string NomeOriginal,
    string Descricao,
    ClasseDeHeroi? ClasseElegivel,
    IReadOnlyList<ArmaNivelDto>? NiveisArma,
    IReadOnlyList<ArmaduraNivelDto>? NiveisArmadura,
    RaridadeDeAcessorio? Raridade,
    ClasseDeHeroi? ClasseExclusiva,
    Guid? ConjuntoId,
    IReadOnlyList<EfeitoDeAcessorioDto>? EfeitosAcessorio,
    MidiaDeItemDto? Midia = null);
