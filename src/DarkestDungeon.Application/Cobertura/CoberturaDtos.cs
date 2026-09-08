using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Cobertura;

namespace DarkestDungeon.Application.Cobertura;

public sealed record EntradaCoberturaDto(CategoriaDeCobertura Categoria, string ChaveDoAtributo, EstadoDeAtributo Estado, string? Notas);

public sealed record MapaDeCoberturaPorClasseDto(Guid ClasseId, ClasseDeHeroi Classe, IReadOnlyList<EntradaCoberturaDto> Entradas);

public sealed record RegistrarCoberturaCommand(
    ClasseDeHeroi Classe,
    CategoriaDeCobertura Categoria,
    string ChaveDoAtributo,
    EstadoDeAtributo Estado,
    string? Notas);
