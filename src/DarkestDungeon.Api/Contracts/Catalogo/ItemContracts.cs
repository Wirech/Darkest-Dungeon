using DarkestDungeon.Application.Itens.Commands;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Itens;

namespace DarkestDungeon.Api.Contracts.Catalogo;

public sealed record NivelDeArmaRequest(int Nivel, int DanoMinimo, int DanoMaximo, decimal Critico, int Velocidade);

public sealed record NivelDeArmaduraRequest(int Nivel, int HpAdicional, decimal Esquiva);

public sealed record EfeitoDeAcessorioRequest(string Nome, decimal Valor, UnidadeDeEfeitoDeAcessorio Unidade, SinalDeEfeito Sinal);

public sealed record CriarArmaRequest(
    string NomeExibicao,
    string NomeOriginal,
    string Descricao,
    ClasseDeHeroi ClasseElegivel,
    IReadOnlyCollection<NivelDeArmaRequest> Niveis);

public sealed record CriarArmaduraRequest(
    string NomeExibicao,
    string NomeOriginal,
    string Descricao,
    ClasseDeHeroi ClasseElegivel,
    IReadOnlyCollection<NivelDeArmaduraRequest> Niveis);

public sealed record CriarAcessorioRequest(
    string NomeExibicao,
    string NomeOriginal,
    string Descricao,
    RaridadeDeAcessorio Raridade,
    ClasseDeHeroi? ClasseExclusiva,
    Guid? ConjuntoId,
    IReadOnlyCollection<EfeitoDeAcessorioRequest> Efeitos);

internal static class ItemRequestMapper
{
    public static CriarArmaCommand ParaCommand(this CriarArmaRequest request) =>
        new(
            request.NomeExibicao ?? string.Empty,
            request.NomeOriginal ?? string.Empty,
            request.Descricao ?? string.Empty,
            request.ClasseElegivel,
            (request.Niveis ?? Array.Empty<NivelDeArmaRequest>())
                .Select(n => new NivelDeArmaCommand(n.Nivel, n.DanoMinimo, n.DanoMaximo, n.Critico, n.Velocidade)).ToArray());

    public static CriarArmaduraCommand ParaCommand(this CriarArmaduraRequest request) =>
        new(
            request.NomeExibicao ?? string.Empty,
            request.NomeOriginal ?? string.Empty,
            request.Descricao ?? string.Empty,
            request.ClasseElegivel,
            (request.Niveis ?? Array.Empty<NivelDeArmaduraRequest>())
                .Select(n => new NivelDeArmaduraCommand(n.Nivel, n.HpAdicional, n.Esquiva)).ToArray());

    public static CriarAcessorioCommand ParaCommand(this CriarAcessorioRequest request) =>
        new(
            request.NomeExibicao ?? string.Empty,
            request.NomeOriginal ?? string.Empty,
            request.Descricao ?? string.Empty,
            request.Raridade,
            request.ClasseExclusiva,
            request.ConjuntoId,
            (request.Efeitos ?? Array.Empty<EfeitoDeAcessorioRequest>())
                .Select(e => new EfeitoDeAcessorioCommand(e.Nome, e.Valor, e.Unidade, e.Sinal)).ToArray());
}
