using DarkestDungeon.Application.Personagens.Commands;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Personagens;
using DarkestDungeon.Domain.Seres;

namespace DarkestDungeon.Api.Contracts.Catalogo;

public sealed record CriarPersonagemRequest(
    string Nome,
    ClasseDeHeroi Classe,
    int HpMaximo,
    int HpAtual,
    int Velocidade,
    decimal Critico,
    int DanoBaseMinimo,
    int DanoBaseMaximo,
    int Movimento,
    decimal BonusDeCritico,
    int Tamanho,
    int AcoesPorTurno,
    decimal Esquiva,
    decimal Precisao,
    decimal Protecao,
    int Nivel,
    int Stress,
    int ChanceDeVirtude,
    IReadOnlyCollection<Guid>? HabilidadesEquipadas = null,
    int? NivelDaArma = null,
    int? NivelDaArmadura = null,
    AparenciaDePersonagem Aparencia = AparenciaDePersonagem.A);

public sealed record EquiparPersonagemRequest(
    Guid? ArmaId,
    Guid? ArmaduraId,
    IReadOnlyCollection<Guid>? AcessoriosIds);

public sealed record CriarInimigoRequest(
    string Nome,
    TipoDeInimigo Tipo,
    int HpMaximo,
    int HpAtual,
    int Velocidade,
    decimal Critico,
    int DanoBaseMinimo,
    int DanoBaseMaximo,
    int Movimento,
    decimal BonusDeCritico,
    int Tamanho,
    int AcoesPorTurno,
    decimal Esquiva,
    decimal Precisao,
    decimal Protecao,
    int Nivel,
    decimal Atordoamento,
    decimal Sangramento,
    decimal Envenenamento,
    decimal Debuff,
    decimal MovimentoResistencia,
    IReadOnlyCollection<Guid>? HabilidadesIds);

internal static class PersonagemRequestMapper
{
    public static CriarPersonagemCommand ParaCommand(this CriarPersonagemRequest r) => new(
        r.Nome ?? string.Empty, r.Classe, r.HpMaximo, r.HpAtual, r.Velocidade, r.Critico,
        r.DanoBaseMinimo, r.DanoBaseMaximo, r.Movimento, r.BonusDeCritico, r.Tamanho, r.AcoesPorTurno,
        r.Esquiva, r.Precisao, r.Protecao, r.Nivel, r.Stress, r.ChanceDeVirtude,
        r.HabilidadesEquipadas, r.NivelDaArma, r.NivelDaArmadura, r.Aparencia);

    public static EquiparPersonagemCommand ParaCommand(this EquiparPersonagemRequest r, Guid personagemId) =>
        new(personagemId, r.ArmaId, r.ArmaduraId, r.AcessoriosIds);

    public static CriarInimigoCommand ParaCommand(this CriarInimigoRequest r) => new(
        r.Nome ?? string.Empty, r.Tipo, r.HpMaximo, r.HpAtual, r.Velocidade, r.Critico,
        r.DanoBaseMinimo, r.DanoBaseMaximo, r.Movimento, r.BonusDeCritico, r.Tamanho, r.AcoesPorTurno,
        r.Esquiva, r.Precisao, r.Protecao, r.Nivel, r.Atordoamento, r.Sangramento,
        r.Envenenamento, r.Debuff, r.MovimentoResistencia, r.HabilidadesIds);
}
