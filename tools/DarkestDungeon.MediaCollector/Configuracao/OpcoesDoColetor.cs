namespace DarkestDungeon.MediaCollector.Configuracao;

public sealed record OpcoesDoColetor(
    string DiretorioDeOrigem,
    string DiretorioDeSaida,
    IReadOnlyList<string> Classes,
    string? CaminhoDoManifesto,
    bool Simular,
    bool Continuar,
    bool GerarManifesto,
    IReadOnlyList<string> Categorias,
    string? CaminhoDoMapeamento,
    string? CaminhoDoInventarioHerois,
    bool Camping)
{
    public bool ModoEquipamentos => Categorias.Count > 0;

    public bool ModoCamping => Camping;

    public static bool TentarCriar(string[] argumentos, out OpcoesDoColetor? opcoes, out string? erro)
    {
        string? origem = null;
        string? saida = null;
        string? manifesto = null;
        string? mapeamento = null;
        string? inventarioHerois = null;
        var classes = new List<string>();
        var categorias = new List<string>();
        var simular = false;
        var continuar = false;
        var gerarManifesto = false;
        var camping = false;
        var aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["arma"] = "Arma",
            ["armadura"] = "Armadura",
            ["acessorio"] = "Acessorio",
            ["acampamento"] = "ItemDeAcampamento",
            ["itemdeacampamento"] = "ItemDeAcampamento",
            ["consumivel"] = "Consumivel",
        };

        for (var indice = 0; indice < argumentos.Length; indice++)
        {
            var argumento = argumentos[indice];
            string? valor = null;
            if (argumento is "--origem" or "--saida" or "--classe" or "--manifesto" or "--categoria" or "--mapeamento" or "--inventario-herois")
            {
                if (++indice >= argumentos.Length || argumentos[indice].StartsWith("--", StringComparison.Ordinal))
                {
                    opcoes = null;
                    erro = $"A opção '{argumento}' exige um valor.";
                    return false;
                }

                valor = argumentos[indice];
            }

            switch (argumento)
            {
                case "--origem": origem = valor; break;
                case "--saida": saida = valor; break;
                case "--classe": classes.Add(valor!); break;
                case "--manifesto": manifesto = valor; break;
                case "--simular": simular = true; break;
                case "--continuar": continuar = true; break;
                case "--gerar-manifesto": gerarManifesto = true; break;
                case "--camping": camping = true; break;
                case "--categoria":
                    if (!aliases.TryGetValue(valor!.Trim(), out var canonica))
                    {
                        opcoes = null;
                        erro = "Informe uma categoria: arma, armadura, acessorio, acampamento ou consumivel.";
                        return false;
                    }
                    categorias.Add(canonica);
                    break;
                case "--mapeamento": mapeamento = valor; break;
                case "--inventario-herois": inventarioHerois = valor; break;
                default: opcoes = null; erro = $"Opção desconhecida: '{argumento}'."; return false;
            }
        }

        if (string.IsNullOrWhiteSpace(origem) || !Directory.Exists(origem))
        {
            opcoes = null;
            erro = "Informe um diretório de instalação existente com --origem.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(saida))
        {
            opcoes = null;
            erro = "Informe o diretório de saída com --saida.";
            return false;
        }

        if (camping && categorias.Count > 0)
        {
            opcoes = null;
            erro = "A opção --camping não combina com --categoria.";
            return false;
        }

        opcoes = new(
            Path.GetFullPath(origem),
            Path.GetFullPath(saida),
            classes,
            manifesto is null ? null : Path.GetFullPath(manifesto),
            simular,
            continuar,
            gerarManifesto,
            categorias,
            mapeamento is null ? null : Path.GetFullPath(mapeamento),
            inventarioHerois is null ? null : Path.GetFullPath(inventarioHerois),
            camping);
        erro = null;
        return true;
    }
}