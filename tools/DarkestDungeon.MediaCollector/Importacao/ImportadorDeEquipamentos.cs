using System.Security.Cryptography;
using DarkestDungeon.MediaCollector.Configuracao;
using DarkestDungeon.MediaCollector.Inventario;

namespace DarkestDungeon.MediaCollector.Importacao;

public sealed class ImportadorDeEquipamentos(OpcoesDoColetor opcoes)
{
    private static readonly string[] Extensoes = [".png", ".atlas", ".skel"];
    private const string Declaracao = "Assets importados de instalação local licenciada; redistribuição não autorizada.";

    public async Task<ResultadoDaImportacao> ExecutarAsync(CancellationToken cancellationToken)
    {
        var resultado = new ResultadoDaImportacao(opcoes.DiretorioDeOrigem, Declaracao, [], [], [], [], []);
        var mapeamento = MapeamentoDePastas.Carregar(opcoes.CaminhoDoMapeamento);
        var categoriasFiltro = opcoes.Categorias;
        var hashesHerois = await ReusoDeInventarioHerois.CarregarPorHashAsync(opcoes.CaminhoDoInventarioHerois, cancellationToken);
        var destinosPorHash = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var origem in mapeamento.Origens)
        {
            if (categoriasFiltro.Count > 0 && !categoriasFiltro.Contains(origem.Categoria, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            var algumaPasta = false;
            foreach (var pasta in MapeamentoDePastas.PastasBaseDaOrigem(opcoes.DiretorioDeOrigem, origem).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (!Directory.Exists(pasta))
                {
                    resultado.Lacunas.Add(new(
                        origem.Categoria,
                        "Pasta esperada não encontrada na instalação.",
                        pasta,
                        DateTime.UtcNow,
                        origem.Categoria));
                    continue;
                }

                algumaPasta = true;
                foreach (var arquivo in Directory.EnumerateFiles(pasta, "*", SearchOption.AllDirectories))
                {
                    if (!Extensoes.Contains(Path.GetExtension(arquivo), StringComparer.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var relativo = Path.GetRelativePath(opcoes.DiretorioDeOrigem, arquivo);
                    var classe = MapeamentoDePastas.Classificar(relativo, mapeamento);
                    if (!classe.Categoria.Equals(origem.Categoria, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    await ImportarArquivoAsync(resultado, arquivo, relativo, origem.Categoria, classe.Classe, hashesHerois, destinosPorHash, cancellationToken);
                }
            }

            if (!algumaPasta && categoriasFiltro.Count == 0)
            {
                resultado.Lacunas.Add(new(
                    origem.Categoria,
                    "Pasta esperada não encontrada na instalação.",
                    origem.Globais.FirstOrDefault() ?? opcoes.DiretorioDeOrigem,
                    DateTime.UtcNow,
                    origem.Categoria));
            }
        }

        var misc = Path.Combine(opcoes.DiretorioDeOrigem, "inventory", "misc");
        if (Directory.Exists(misc))
        {
            foreach (var arquivo in Directory.EnumerateFiles(misc, "*", SearchOption.AllDirectories))
            {
                if (!Extensoes.Contains(Path.GetExtension(arquivo), StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                var relativo = Path.GetRelativePath(opcoes.DiretorioDeOrigem, arquivo);
                await ImportarArquivoAsync(resultado, arquivo, relativo, "NaoAssociado", null, hashesHerois, destinosPorHash, cancellationToken);
            }
        }

        if (!opcoes.Simular)
        {
            await ArmazenamentoDeInventario.SalvarAtomicamenteAsync(opcoes.DiretorioDeSaida, resultado, cancellationToken);
        }

        return resultado;
    }

    private async Task ImportarArquivoAsync(
        ResultadoDaImportacao resultado,
        string origem,
        string relativo,
        string categoria,
        string? classe,
        IReadOnlyDictionary<string, (string CaminhoDestino, long Tamanho)> hashesHerois,
        Dictionary<string, string> destinosPorHash,
        CancellationToken cancellationToken)
    {
        var bytes = await File.ReadAllBytesAsync(origem, cancellationToken);
        var sha256 = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
        var tamanho = bytes.LongLength;
        var pastaCategoria = categoria.ToLowerInvariant();
        var destinoAbsoluto = Path.Combine(opcoes.DiretorioDeSaida, "arquivos", pastaCategoria, SanitizarRelativo(relativo));
        var destinoRelativo = Path.GetRelativePath(opcoes.DiretorioDeSaida, destinoAbsoluto).Replace('\\', '/');
        var reutilizado = false;

        if (hashesHerois.TryGetValue(sha256, out var heroi))
        {
            reutilizado = true;
            destinoRelativo = heroi.CaminhoDestino.Replace('\\', '/');
            tamanho = heroi.Tamanho == 0 ? tamanho : heroi.Tamanho;
        }
        else if (destinosPorHash.TryGetValue(sha256, out var existente))
        {
            reutilizado = true;
            destinoRelativo = existente;
        }
        else if (!opcoes.Simular)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(destinoAbsoluto)!);
            await File.WriteAllBytesAsync(destinoAbsoluto, bytes, cancellationToken);
            destinosPorHash[sha256] = destinoRelativo;
        }
        else
        {
            destinosPorHash[sha256] = destinoRelativo;
        }

        resultado.Arquivos.Add(new(
            classe ?? string.Empty,
            origem,
            destinoRelativo,
            sha256,
            tamanho,
            reutilizado,
            categoria));
    }

    private static string SanitizarRelativo(string relativo)
    {
        var limpo = relativo.Replace("..", string.Empty).Replace('\\', Path.DirectorySeparatorChar);
        return limpo.TrimStart(Path.DirectorySeparatorChar);
    }
}
