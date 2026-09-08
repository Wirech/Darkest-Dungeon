using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Cobertura;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

/// Seed inicial do Mapa de Cobertura: cria uma entrada `Coletado` para cada resistência base
/// de cada classe (todas as 20 classes com dados oficiais mineirados em darkestdungeon.wiki.gg).
/// Habilidades ficam ausentes até a mineração de skills (T059/a/b/c).
public static class MapaDeCoberturaSeed
{
    private static readonly string[] Resistencias =
    {
        "Atordoamento", "Sangramento", "Envenenamento", "Debuff", "Movimento",
        "Doenca", "GolpeMortal", "Armadilha",
    };

    public static IEnumerable<EntradaDoMapaDeCobertura> Materializar()
    {
        foreach (var classe in Enum.GetValues<ClasseDeHeroi>())
        {
            foreach (var resistencia in Resistencias)
            {
                yield return new EntradaDoMapaDeCobertura(
                    classe,
                    CategoriaDeCobertura.ResistenciaBase,
                    resistencia,
                    EstadoDeAtributo.Coletado);
            }
        }
    }
}
