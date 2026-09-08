namespace DarkestDungeon.Api.Contracts;

public sealed record ResistenciasRequest(
    decimal Atordoamento,
    decimal Sangramento,
    decimal Envenenamento,
    decimal Debuff,
    decimal Movimento);