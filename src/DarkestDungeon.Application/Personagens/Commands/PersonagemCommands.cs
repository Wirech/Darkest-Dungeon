using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Personagens;
using DarkestDungeon.Domain.Seres;

namespace DarkestDungeon.Application.Personagens.Commands;

public sealed record CriarPersonagemCommand(
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
    IReadOnlyCollection<Guid>? HabilidadesEquipadas,
    int? NivelDaArma = null,
    int? NivelDaArmadura = null,
    AparenciaDePersonagem Aparencia = AparenciaDePersonagem.A);

public sealed record EquiparPersonagemCommand(
    Guid PersonagemId,
    Guid? ArmaId,
    Guid? ArmaduraId,
    IReadOnlyCollection<Guid>? AcessoriosIds);

public sealed record EquiparAcessorioNoEspacoCommand(
    Guid PersonagemId,
    int Espaco,
    Guid? AcessorioId);

public sealed record CriarInimigoCommand(
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
