namespace DarkestDungeon.Application.Seres;

public sealed record ResistenciasDto(
    decimal Atordoamento,
    decimal Sangramento,
    decimal Envenenamento,
    decimal Debuff,
    decimal Movimento);