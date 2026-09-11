namespace DarkestDungeon.Api.Tests.Feature010;

internal static class AcervoDoCardTestHelper
{
    internal static string RaizDoRepositorio
    {
        get
        {
            var atual = AppContext.BaseDirectory;
            for (var nivel = 0; nivel < 10; nivel++)
            {
                if (File.Exists(Path.Combine(atual, "DarkestDungeon.sln")))
                {
                    return atual;
                }

                var pai = Directory.GetParent(atual);
                if (pai is null)
                {
                    break;
                }

                atual = pai.FullName;
            }

            throw new DirectoryNotFoundException("Não foi possível localizar a raiz do repositório (DarkestDungeon.sln).");
        }
    }

    internal static string Herois => Path.Combine(RaizDoRepositorio, "assets", "herois");

    internal static string EquipamentosItens => Path.Combine(RaizDoRepositorio, "assets", "equipamentos-itens");

    internal static string RetratoCruzadoARelativo => "arquivos/cruzado/crusader_A/crusader_portrait_roster.png";

    internal static string UrlRetratoCruzadoA => $"/acervo/herois/{RetratoCruzadoARelativo}";
}
