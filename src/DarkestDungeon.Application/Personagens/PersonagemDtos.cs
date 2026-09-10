using DarkestDungeon.Application.Classes;
using DarkestDungeon.Application.Midias;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Seres;

namespace DarkestDungeon.Application.Personagens;

public sealed record ResistenciasBaseDto(
    decimal Atordoamento,
    decimal Sangramento,
    decimal Envenenamento,
    decimal Debuff,
    decimal Movimento);

public sealed record HabilidadeDePersonagemDto(Guid HabilidadeId, bool Habilitada, bool Treinada, bool Equipada);

public sealed record PersonagemDetalheDto(
    Guid Id,
    string Nome,
    ClasseDeHeroi Classe,
    int HpMaximo,
    int HpAtual,
    int Stress,
    int ChanceDeVirtude,
    string? Aflicao,
    string? Virtude,
    ResistenciasBaseDto Resistencias,
    decimal Doenca,
    decimal GolpeMortal,
    decimal Armadilha,
    IReadOnlyList<HabilidadeDePersonagemDto> Habilidades,
    Guid? ArmaEquipadaId,
    Guid? ArmaduraEquipadaId,
    IReadOnlyList<Guid> AcessoriosEquipadosIds,
    MidiaDeEquipamentoDePersonagemDto? MidiaArmaEquipada = null,
    MidiaDeEquipamentoDePersonagemDto? MidiaArmaduraEquipada = null,
    IReadOnlyList<MidiaDeAcessorioDePersonagemDto>? MidiasAcessoriosEquipados = null);

public sealed record InimigoDetalheDto(
    Guid Id,
    string Nome,
    TipoDeInimigo Tipo,
    IReadOnlyList<Guid> HabilidadesIds);
