using System.ComponentModel.DataAnnotations;

namespace DarkestDungeon.Api.Contracts;

public sealed record CriarSerRequest(
    [Required(ErrorMessage = "Nome deve ser informado.")] string Nome,
    [Required(ErrorMessage = "Tipo deve ser informado.")] string Tipo,
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
    [Required(ErrorMessage = "Resistências devem ser informadas.")] ResistenciasRequest Resistencias);