namespace DarkestDungeon.Api.Contracts;

public sealed record ResistenciasResponse(
    decimal Atordoamento,
    decimal Sangramento,
    decimal Envenenamento,
    decimal Debuff,
    decimal Movimento);