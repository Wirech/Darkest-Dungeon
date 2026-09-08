using DarkestDungeon.Domain.Classes;

namespace DarkestDungeon.Application.Classes;

public sealed record ResistenciasDto(
    decimal Atordoamento,
    decimal Sangramento,
    decimal Envenenamento,
    decimal Debuff,
    decimal Movimento,
    decimal Doenca,
    decimal GolpeMortal,
    decimal Armadilha);

public sealed record ClasseResumoDto(Guid Id, ClasseDeHeroi Classe, string NomeExibicao, string NomeOriginal);

public sealed record ClasseDetalheDto(
    Guid Id,
    ClasseDeHeroi Classe,
    string NomeExibicao,
    string NomeOriginal,
    ResistenciasDto ResistenciasBase);
