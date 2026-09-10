using DarkestDungeon.Domain.Itens;

namespace DarkestDungeon.Application.Midias;

public static class CategoriaDeMidiaParser
{
    public static bool TentarAnalisar(string? valor, out CategoriaDeMidiaDeItem categoria)
    {
        categoria = default;
        if (string.IsNullOrWhiteSpace(valor))
        {
            return false;
        }

        switch (valor.Trim().ToLowerInvariant())
        {
            case "arma":
                categoria = CategoriaDeMidiaDeItem.Arma;
                return true;
            case "armadura":
                categoria = CategoriaDeMidiaDeItem.Armadura;
                return true;
            case "acessorio":
                categoria = CategoriaDeMidiaDeItem.Acessorio;
                return true;
            case "acampamento":
            case "itemdeacampamento":
                categoria = CategoriaDeMidiaDeItem.ItemDeAcampamento;
                return true;
            case "consumivel":
                categoria = CategoriaDeMidiaDeItem.Consumivel;
                return true;
            default:
                return false;
        }
    }

    public static string NomeDeApi(CategoriaDeMidiaDeItem categoria) => categoria switch
    {
        CategoriaDeMidiaDeItem.Arma => "Arma",
        CategoriaDeMidiaDeItem.Armadura => "Armadura",
        CategoriaDeMidiaDeItem.Acessorio => "Acessório",
        CategoriaDeMidiaDeItem.ItemDeAcampamento => "Item de acampamento/provisão",
        CategoriaDeMidiaDeItem.Consumivel => "Consumível",
        _ => categoria.ToString(),
    };

    public static string EstadoDeApi(EstadoDePublicacaoDeVinculos estado) => estado switch
    {
        EstadoDePublicacaoDeVinculos.EmCurso => "Em curso",
        EstadoDePublicacaoDeVinculos.Concluida => "Concluída",
        EstadoDePublicacaoDeVinculos.RollbackAplicado => "Rollback aplicado",
        _ => estado.ToString(),
    };

    public static string Inventario(CategoriaDeMidiaDeItem categoria) => categoria switch
    {
        CategoriaDeMidiaDeItem.Arma => "Arma",
        CategoriaDeMidiaDeItem.Armadura => "Armadura",
        CategoriaDeMidiaDeItem.Acessorio => "Acessorio",
        CategoriaDeMidiaDeItem.ItemDeAcampamento => "ItemDeAcampamento",
        CategoriaDeMidiaDeItem.Consumivel => "Consumivel",
        _ => categoria.ToString(),
    };
}
