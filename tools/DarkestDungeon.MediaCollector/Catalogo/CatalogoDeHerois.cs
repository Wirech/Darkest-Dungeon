using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Infrastructure.Data.Seeds;

namespace DarkestDungeon.MediaCollector.Catalogo;

public sealed record HeroiDoCatalogo(ClasseDeHeroi Classe, string NomeOriginal, string NomeExibicao, IReadOnlyList<string> Habilidades);

public static class CatalogoDeHerois
{
    public static IReadOnlyList<HeroiDoCatalogo> ObterTodos()
    {
        var habilidades = HabilidadesSeed.ObterNomesPorClasse();
        return ClassesSeed.Materializar()
            .Select(classe => new HeroiDoCatalogo(classe.ClasseDeHeroi, classe.NomeOriginal, classe.NomeExibicao, habilidades[classe.ClasseDeHeroi]))
            .ToArray();
    }
}