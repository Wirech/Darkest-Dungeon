using DarkestDungeon.Application.Classes;
using DarkestDungeon.Application.Midias;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Personagens;
using DarkestDungeon.Domain.Seres;

namespace DarkestDungeon.Application.Personagens;

public sealed record ResistenciasBaseDto(
    decimal Atordoamento,
    decimal Sangramento,
    decimal Envenenamento,
    decimal Debuff,
    decimal Movimento,
    decimal Doenca = 0,
    decimal GolpeMortal = 0,
    decimal Armadilha = 0);

public sealed record ResistenciasDePersonagemDto(
    decimal Atordoamento,
    decimal Sangramento,
    decimal Envenenamento,
    decimal Debuff,
    decimal Movimento,
    decimal Doenca,
    decimal GolpeMortal,
    decimal Armadilha);

public sealed record HabilidadeDePersonagemDto(
    Guid HabilidadeId,
    bool Habilitada,
    bool Treinada,
    bool Equipada,
    int NumeroDoNivel = 0,
    string? Nome = null,
    string? Categoria = null,
    SlotDeMidiaDoCardDto? Midia = null);

public sealed record PersonagemResumoDto(
    Guid Id,
    string Nome,
    ClasseDeHeroi Classe,
    string NomeClasse,
    int HpAtual,
    int HpMaximo,
    int Stress,
    int Nivel,
    IReadOnlyList<HabilidadeDePersonagemDto> Habilidades,
    decimal Precisao = 0,
    decimal Protecao = 0,
    decimal Esquiva = 0,
    int Velocidade = 0,
    decimal Critico = 0,
    int DanoBaseMinimo = 0,
    int DanoBaseMaximo = 0,
    int? PassosAFrente = null,
    int? PassosAtras = null,
    ResistenciasDePersonagemDto? Resistencias = null,
    int? NivelDaArma = null,
    int? NivelDaArmadura = null,
    AparenciaDePersonagem Aparencia = AparenciaDePersonagem.A,
    bool ClasseReligiosa = false,
    string? ProvisaoInicial = null,
    string BonusAoCriticoDaClasse = "",
    MidiasDoPersonagemDto? Midias = null);

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
    IReadOnlyList<MidiaDeAcessorioDePersonagemDto>? MidiasAcessoriosEquipados = null,
    AparenciaDePersonagem Aparencia = AparenciaDePersonagem.A,
    int? NivelDaArma = null,
    int? NivelDaArmadura = null,
    decimal Precisao = 0,
    decimal Protecao = 0,
    decimal Esquiva = 0,
    int Velocidade = 0,
    decimal Critico = 0,
    int DanoBaseMinimo = 0,
    int DanoBaseMaximo = 0,
    int? PassosAFrente = null,
    int? PassosAtras = null,
    bool ClasseReligiosa = false,
    string? ProvisaoInicial = null,
    string BonusAoCriticoDaClasse = "",
    MidiasDoPersonagemDto? Midias = null);

public sealed record InimigoDetalheDto(
    Guid Id,
    string Nome,
    TipoDeInimigo Tipo,
    IReadOnlyList<Guid> HabilidadesIds);
