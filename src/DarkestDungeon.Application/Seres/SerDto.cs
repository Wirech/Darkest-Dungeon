namespace DarkestDungeon.Application.Seres;

public sealed record SerDto(
    Guid Id,
    string Nome,
    string Tipo,
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
    ResistenciasDto Resistencias);