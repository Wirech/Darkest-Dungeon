using System.Security.Cryptography;
using DarkestDungeon.MediaCollector.Configuracao;
using DarkestDungeon.MediaCollector.Inventario;

namespace DarkestDungeon.MediaCollector.Importacao;

public sealed class ImportadorDeCamping(OpcoesDoColetor opcoes)
{
    private const string Declaracao = "Assets importados de instalação local licenciada; redistribuição não autorizada.";
    private const string ClasseInventario = "Acampamento";
    private const string Categoria = "HabilidadeDeAcampamento";

    public async Task<ResultadoDaImportacao> ExecutarAsync(CancellationToken cancellationToken)
    {
        var resultado = new ResultadoDaImportacao(opcoes.DiretorioDeOrigem, Declaracao, [], [], [], [], []);
        var porStem = DescobrirPngs();
        if (porStem.Count == 0)
        {
            var vanilla = Path.Combine(opcoes.DiretorioDeOrigem, "raid", "camping", "skill_icons");
            resultado.Lacunas.Add(new(
                ClasseInventario,
                "Pasta esperada não encontrada na instalação.",
                Directory.Exists(vanilla) ? vanilla : Path.Combine(opcoes.DiretorioDeOrigem, "raid", "camping"),
                DateTime.UtcNow,
                Categoria));
        }

        var anterior = opcoes.Continuar
            ? await ArmazenamentoDeInventario.CarregarAnteriorAsync(opcoes.DiretorioDeSaida, cancellationToken)
            : new Dictionary<string, (string Sha256, long Tamanho)>(StringComparer.OrdinalIgnoreCase);

        foreach (var (stem, origem) in porStem.OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase))
        {
            var nomeArquivo = Path.GetFileName(origem);
            var destinoAbsoluto = Path.Combine(opcoes.DiretorioDeSaida, "arquivos", "acampamento", nomeArquivo);
            var destinoRelativo = Path.GetRelativePath(opcoes.DiretorioDeSaida, destinoAbsoluto).Replace('\\', '/');

            string sha256;
            long tamanho;
            var reutilizado = false;

            if (opcoes.Continuar && anterior.TryGetValue(origem, out var registro) && File.Exists(destinoAbsoluto))
            {
                sha256 = registro.Sha256;
                tamanho = registro.Tamanho;
                reutilizado = true;
            }
            else
            {
                var bytes = await File.ReadAllBytesAsync(origem, cancellationToken);
                sha256 = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
                tamanho = bytes.LongLength;

                if (!opcoes.Simular)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destinoAbsoluto)!);
                    if (File.Exists(destinoAbsoluto)
                        && Convert.ToHexString(SHA256.HashData(await File.ReadAllBytesAsync(destinoAbsoluto, cancellationToken))).Equals(sha256, StringComparison.OrdinalIgnoreCase))
                    {
                        reutilizado = true;
                    }
                    else
                    {
                        await File.WriteAllBytesAsync(destinoAbsoluto, bytes, cancellationToken);
                    }
                }
            }

            resultado.Arquivos.Add(new(
                ClasseInventario,
                origem,
                destinoRelativo,
                sha256,
                tamanho,
                reutilizado,
                Categoria));
        }

        resultado.ResumoPorClasse.Add(new(
            ClasseInventario,
            resultado.Arquivos.Count,
            resultado.Arquivos.Count(a => a.Reutilizado),
            0,
            resultado.Lacunas.Count));

        if (!opcoes.Simular)
        {
            var mesclado = await ArmazenamentoDeInventario.MesclarCampingAsync(
                opcoes.DiretorioDeSaida,
                resultado,
                cancellationToken);
            await ArmazenamentoDeInventario.SalvarAtomicamenteAsync(opcoes.DiretorioDeSaida, mesclado, cancellationToken);

            await GeradorDoManifestoDeCamping.AtualizarAsync(opcoes.DiretorioDeSaida, cancellationToken);
        }

        return resultado;
    }

    private Dictionary<string, string> DescobrirPngs()
    {
        var porStem = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (!Directory.Exists(opcoes.DiretorioDeOrigem))
        {
            return porStem;
        }

        foreach (var arquivo in Directory.EnumerateFiles(opcoes.DiretorioDeOrigem, "camp_skill_*.png", SearchOption.AllDirectories))
        {
            var relativo = Path.GetRelativePath(opcoes.DiretorioDeOrigem, arquivo).Replace('\\', '/');
            if (!relativo.Contains("camping", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var stem = Path.GetFileNameWithoutExtension(arquivo);
            if (stem.StartsWith("camp_skill_", StringComparison.OrdinalIgnoreCase))
            {
                stem = stem["camp_skill_".Length..];
            }

            if (AliasesDeCamping.Leftovers.Contains(stem))
            {
                continue;
            }

            var ehVanilla = relativo.StartsWith("raid/", StringComparison.OrdinalIgnoreCase);
            if (ehVanilla || !porStem.ContainsKey(stem))
            {
                porStem[stem] = arquivo;
            }
        }

        return porStem;
    }
}
