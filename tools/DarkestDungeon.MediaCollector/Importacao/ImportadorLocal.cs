using System.Security.Cryptography;
using System.Runtime.InteropServices;
using DarkestDungeon.MediaCollector.Catalogo;
using DarkestDungeon.MediaCollector.Configuracao;
using DarkestDungeon.MediaCollector.Inventario;
using DarkestDungeon.MediaCollector.Spine;

namespace DarkestDungeon.MediaCollector.Importacao;

public sealed class ImportadorLocal(OpcoesDoColetor opcoes)
{
    private readonly Dictionary<string, string> destinosPorHash = new(StringComparer.Ordinal);
    private static readonly IReadOnlyDictionary<string, string> DiretoriosPorClasse = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["Abomination"] = "abomination", ["Antiquarian"] = "antiquarian", ["Arbalest"] = "arbalest",
        ["Bounty Hunter"] = "bounty_hunter", ["Crusader"] = "crusader", ["Grave Robber"] = "grave_robber",
        ["Jester"] = "jester", ["Houndmaster"] = "houndmaster", ["Leper"] = "leper", ["Hellion"] = "hellion",
        ["Highwayman"] = "highwayman", ["Musketeer"] = "musketeer", ["Man-at-Arms"] = "man_at_arms",
        ["Occultist"] = "occultist", ["Plague Doctor"] = "plague_doctor", ["Vestal"] = "vestal",
        ["Flagellant"] = "flagellant", ["Shieldbreaker"] = "shieldbreaker", ["Duelist"] = "duelist", ["Runaway"] = "runaway",
    };

    public async Task<ResultadoDaImportacao> ExecutarAsync(IReadOnlyList<HeroiDoCatalogo> herois, CancellationToken cancellationToken)
    {
        var resultado = new ResultadoDaImportacao(opcoes.DiretorioDeOrigem, "Assets importados de instalação local licenciada; redistribuição não autorizada.", [], [], [], [], []);
        var inventarioAnterior = opcoes.Continuar
            ? await ArmazenamentoDeInventario.CarregarAnteriorAsync(opcoes.DiretorioDeSaida, cancellationToken)
            : new Dictionary<string, (string Sha256, long Tamanho)>(StringComparer.OrdinalIgnoreCase);
        foreach (var heroi in herois)
        {
            var diretorio = LocalizarDiretorioDoHeroi(heroi.NomeOriginal);
            if (diretorio is null)
            {
                resultado.Lacunas.Add(new(heroi.NomeExibicao, "Diretório do herói não encontrado na instalação ou DLCs.", opcoes.DiretorioDeOrigem, DateTime.UtcNow));
                continue;
            }

            var arquivos = Directory.EnumerateFiles(diretorio, "*", SearchOption.AllDirectories)
                .Where(caminho => new[] { ".png", ".atlas", ".skel" }.Contains(Path.GetExtension(caminho), StringComparer.OrdinalIgnoreCase));
            var arquivosDaClasse = arquivos.ToArray();
            var hashesPorCaminho = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var origem in arquivosDaClasse)
            {
                var relativo = Path.GetRelativePath(diretorio, origem);
                var destino = Path.Combine(opcoes.DiretorioDeSaida, "arquivos", NormalizarNome(heroi.NomeExibicao), relativo);
                string? hashAnterior = null;
                long tamanhoAnterior = 0;
                if (opcoes.Continuar && inventarioAnterior.TryGetValue(origem, out var registro) && File.Exists(destino))
                {
                    hashAnterior = registro.Sha256;
                    tamanhoAnterior = registro.Tamanho;
                }
                byte[]? bytes = null;
                string sha256;
                long tamanho;
                if (hashAnterior is not null)
                {
                    sha256 = hashAnterior;
                    tamanho = tamanhoAnterior;
                }
                else
                {
                    bytes = await File.ReadAllBytesAsync(origem, cancellationToken);
                    sha256 = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
                    tamanho = bytes.LongLength;
                }
                hashesPorCaminho[origem] = sha256;
                var reutilizado = false;
                if (!opcoes.Simular)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destino)!);
                    if (destinosPorHash.TryGetValue(sha256, out var destinoExistente))
                    {
                        if (!File.Exists(destino) && (!OperatingSystem.IsWindows() || !CreateHardLink(destino, destinoExistente, IntPtr.Zero)))
                        {
                            bytes ??= await File.ReadAllBytesAsync(origem, cancellationToken);
                            await File.WriteAllBytesAsync(destino, bytes, cancellationToken);
                        }
                        reutilizado = true;
                    }
                    else if (hashAnterior is not null)
                    {
                        reutilizado = true;
                        destinosPorHash[sha256] = destino;
                    }
                    else
                    {
                        reutilizado = opcoes.Continuar && File.Exists(destino)
                            && Convert.ToHexString(SHA256.HashData(await File.ReadAllBytesAsync(destino, cancellationToken))).ToLowerInvariant() == sha256;
                        if (!reutilizado) await File.WriteAllBytesAsync(destino, bytes!, cancellationToken);
                        destinosPorHash[sha256] = destino;
                    }
                }

                resultado.Arquivos.Add(new(heroi.NomeExibicao, origem, Path.GetRelativePath(opcoes.DiretorioDeSaida, destino).Replace('\\', '/'), sha256, tamanho, reutilizado));
            }

            foreach (var caminhoAtlas in arquivosDaClasse.Where(caminho => Path.GetExtension(caminho).Equals(".atlas", StringComparison.OrdinalIgnoreCase)))
            {
                var conjunto = ConstrutorDeConjuntosSpine.Construir(heroi.NomeExibicao, caminhoAtlas, await File.ReadAllTextAsync(caminhoAtlas, cancellationToken), arquivosDaClasse, hashesPorCaminho);
                resultado.Conjuntos.Add(conjunto);
                if (!File.Exists(conjunto.Textura) || conjunto.Esqueleto is null)
                {
                    resultado.Lacunas.Add(new(heroi.NomeExibicao, "Conjunto Spine incompleto.", caminhoAtlas, DateTime.UtcNow));
                }
            }
        }

        var caminhoManifesto = opcoes.CaminhoDoManifesto ?? Path.Combine(opcoes.DiretorioDeSaida, "manifesto-habilidades.json");
        if (File.Exists(caminhoManifesto))
        {
            var manifesto = await LeitorDoManifesto.LerAsync(caminhoManifesto, cancellationToken);
            foreach (var associacao in manifesto.Associacoes)
            {
                if (resultado.Arquivos.Any(arquivo => arquivo.Classe.Equals(associacao.Classe, StringComparison.OrdinalIgnoreCase) && Path.GetFileName(arquivo.CaminhoOrigem).Equals(associacao.Arquivo, StringComparison.OrdinalIgnoreCase)))
                {
                    resultado.AssociacoesDeHabilidades.Add(associacao);
                }
                else
                {
                    resultado.Lacunas.Add(new(associacao.Classe, "Arquivo do manifesto não foi localizado.", associacao.Arquivo, DateTime.UtcNow));
                }
            }
        }

        foreach (var heroi in herois)
        {
            resultado.ResumoPorClasse.Add(new(heroi.NomeExibicao,
                resultado.Arquivos.Count(item => item.Classe == heroi.NomeExibicao),
                resultado.Arquivos.Count(item => item.Classe == heroi.NomeExibicao && item.Reutilizado),
                resultado.Conjuntos.Count(item => item.Classe == heroi.NomeExibicao),
                resultado.Lacunas.Count(item => item.Classe == heroi.NomeExibicao)));
        }

        if (!opcoes.Simular)
        {
            await ArmazenamentoDeInventario.SalvarAtomicamenteAsync(opcoes.DiretorioDeSaida, resultado, cancellationToken);
        }

        return resultado;
    }

    private string? LocalizarDiretorioDoHeroi(string nomeOriginal)
    {
        var diretorioEsperado = DiretoriosPorClasse[nomeOriginal];
        return Directory.EnumerateDirectories(opcoes.DiretorioDeOrigem, diretorioEsperado, SearchOption.AllDirectories)
            .FirstOrDefault(caminho => Path.GetFileName(Path.GetDirectoryName(caminho) ?? string.Empty).Equals("heroes", StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizarNome(string valor)
    {
        var normalizado = valor.Normalize(System.Text.NormalizationForm.FormD)
            .Where(caractere => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(caractere) != System.Globalization.UnicodeCategory.NonSpacingMark)
            .Select(caractere => char.IsLetterOrDigit(caractere) ? char.ToLowerInvariant(caractere) : '-')
            .ToArray();
        return string.Join('-', new string(normalizado).Split('-', StringSplitOptions.RemoveEmptyEntries));
    }

    [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool CreateHardLink(string nomeDoLink, string nomeDoArquivoExistente, IntPtr atributosDeSeguranca);
}