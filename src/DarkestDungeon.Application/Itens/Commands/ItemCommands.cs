using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Itens;

namespace DarkestDungeon.Application.Itens.Commands;

public sealed record NivelDeArmaCommand(int Nivel, int DanoMinimo, int DanoMaximo, decimal Critico, int Velocidade);

public sealed record NivelDeArmaduraCommand(int Nivel, int HpAdicional, decimal Esquiva);

public sealed record EfeitoDeAcessorioCommand(string Nome, decimal Valor, UnidadeDeEfeitoDeAcessorio Unidade, SinalDeEfeito Sinal);

public sealed record CriarArmaCommand(
    string NomeExibicao,
    string NomeOriginal,
    string Descricao,
    ClasseDeHeroi ClasseElegivel,
    IReadOnlyCollection<NivelDeArmaCommand> Niveis);

public sealed record CriarArmaduraCommand(
    string NomeExibicao,
    string NomeOriginal,
    string Descricao,
    ClasseDeHeroi ClasseElegivel,
    IReadOnlyCollection<NivelDeArmaduraCommand> Niveis);

public sealed record CriarAcessorioCommand(
    string NomeExibicao,
    string NomeOriginal,
    string Descricao,
    RaridadeDeAcessorio Raridade,
    ClasseDeHeroi? ClasseExclusiva,
    Guid? ConjuntoId,
    IReadOnlyCollection<EfeitoDeAcessorioCommand> Efeitos);
