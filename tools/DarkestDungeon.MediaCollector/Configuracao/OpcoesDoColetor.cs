namespace DarkestDungeon.MediaCollector.Configuracao;

public sealed record OpcoesDoColetor(
    string DiretorioDeOrigem,
    string DiretorioDeSaida,
    IReadOnlyList<string> Classes,
    string? CaminhoDoManifesto,
    bool Simular,
    bool Continuar,
    bool GerarManifesto)
{
    public static bool TentarCriar(string[] argumentos, out OpcoesDoColetor? opcoes, out string? erro)
    {
        string? origem = null;
        string? saida = null;
        string? manifesto = null;
        var classes = new List<string>();
        var simular = false;
        var continuar = false;
        var gerarManifesto = false;

        for (var indice = 0; indice < argumentos.Length; indice++)
        {
            var argumento = argumentos[indice];
            string? valor = null;
            if (argumento is "--origem" or "--saida" or "--classe" or "--manifesto")
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

        opcoes = new(Path.GetFullPath(origem), Path.GetFullPath(saida), classes, manifesto is null ? null : Path.GetFullPath(manifesto), simular, continuar, gerarManifesto);
        erro = null;
        return true;
    }
}