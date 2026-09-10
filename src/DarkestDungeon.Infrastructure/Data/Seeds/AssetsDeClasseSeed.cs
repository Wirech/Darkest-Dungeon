using System.Text.Json;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Cobertura;
using DarkestDungeon.Domain.Personagens;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

/// Popula os assets Classe × Aparência (A/B/C/D) da Feature 005 a partir do inventário Spine da Feature 004.
/// Regra: para cada combinação Classe × Aparencia, procura no inventário 004 o arquivo `portrait_roster` da pasta `{classe_original}_{X}` (X ∈ A/B/C/D).
/// Se encontrado → `Status = Coletado` com `ConjuntoSpineId = CaminhoDestino` e `HashArquivo = Sha256`.
/// Se não encontrado → `Status = Pendente`, campos nulos.
public static class AssetsDeClasseSeed
{
    private const string CaminhoInventarioRelativo = "assets/herois/inventario.json";

    /// Mapa Classe → prefixo da pasta no inventário 004 (nome inglês do jogo original, lowercase, snake_case).
    private static readonly IReadOnlyDictionary<ClasseDeHeroi, string> PrefixoNoInventario = new Dictionary<ClasseDeHeroi, string>
    {
        [ClasseDeHeroi.Abominacao] = "abomination",
        [ClasseDeHeroi.Antiquario] = "antiquarian",
        [ClasseDeHeroi.Besteiro] = "arbalest",
        [ClasseDeHeroi.CacadorDeRecompensas] = "bounty_hunter",
        [ClasseDeHeroi.Cruzado] = "crusader",
        [ClasseDeHeroi.LadraoDeCova] = "grave_robber",
        [ClasseDeHeroi.BoboDaCorte] = "jester",
        [ClasseDeHeroi.MestreDeCaca] = "houndmaster",
        [ClasseDeHeroi.Leproso] = "leper",
        [ClasseDeHeroi.Infernal] = "hellion",
        [ClasseDeHeroi.Bandido] = "highwayman",
        [ClasseDeHeroi.Musqueteiro] = "musketeer",
        [ClasseDeHeroi.Veterano] = "man_at_arms",
        [ClasseDeHeroi.Ocultista] = "occultist",
        [ClasseDeHeroi.MedicoDaPeste] = "plague_doctor",
        [ClasseDeHeroi.Vestal] = "vestal",
        [ClasseDeHeroi.Flagelante] = "flagellant",
        [ClasseDeHeroi.Rompedor] = "shieldbreaker",
        [ClasseDeHeroi.Duelista] = "duelist",
        [ClasseDeHeroi.Fugitivo] = "runaway",
    };

    private static readonly AparenciaDePersonagem[] Aparencias = new[]
    {
        AparenciaDePersonagem.A,
        AparenciaDePersonagem.B,
        AparenciaDePersonagem.C,
        AparenciaDePersonagem.D,
    };

    /// Aplica os assets nas 20 classes do banco. Retorna estatísticas para log.
    public static ResultadoDoSeedDeAssets Aplicar(IReadOnlyDictionary<ClasseDeHeroi, Classe> classesPorEnum, string? caminhoBaseRepositorio = null)
    {
        ArgumentNullException.ThrowIfNull(classesPorEnum);
        var indice = CarregarIndiceDoInventario(caminhoBaseRepositorio);
        int coletados = 0;
        int pendentes = 0;

        foreach (var (chave, classe) in classesPorEnum)
        {
            if (classe.Assets.Count > 0)
            {
                continue;
            }

            if (!PrefixoNoInventario.TryGetValue(chave, out var prefixo))
            {
                DefinirTodasComoPendentes(classe);
                pendentes += 4;
                continue;
            }

            var registros = new List<AssetsDeClasse>(4);
            foreach (var aparencia in Aparencias)
            {
                var chaveInventario = $"{prefixo}_{aparencia}";
                if (indice.TryGetValue(chaveInventario, out var entrada))
                {
                    registros.Add(new AssetsDeClasse(aparencia, entrada.CaminhoDestino, entrada.Sha256, EstadoDeAtributo.Coletado));
                    coletados++;
                }
                else
                {
                    registros.Add(new AssetsDeClasse(aparencia, null, null, EstadoDeAtributo.Pendente));
                    pendentes++;
                }
            }

            classe.DefinirAssets(registros);
        }

        return new ResultadoDoSeedDeAssets(coletados, pendentes);
    }

    private static void DefinirTodasComoPendentes(Classe classe)
    {
        classe.DefinirAssets(Aparencias.Select(ap => new AssetsDeClasse(ap, null, null, EstadoDeAtributo.Pendente)));
    }

    /// Índice { "classe_A" → entrada de portrait_roster } montado a partir do inventário 004.
    private static IReadOnlyDictionary<string, EntradaDoInventario> CarregarIndiceDoInventario(string? caminhoBaseRepositorio)
    {
        var caminho = ResolverCaminhoAbsoluto(caminhoBaseRepositorio);
        if (caminho is null || !File.Exists(caminho))
        {
            return new Dictionary<string, EntradaDoInventario>(0);
        }

        using var stream = File.OpenRead(caminho);
        var raiz = JsonSerializer.Deserialize<InventarioRaiz>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (raiz?.Arquivos is null)
        {
            return new Dictionary<string, EntradaDoInventario>(0);
        }

        var indice = new Dictionary<string, EntradaDoInventario>(StringComparer.OrdinalIgnoreCase);
        foreach (var arquivo in raiz.Arquivos)
        {
            if (arquivo.CaminhoDestino is null || !arquivo.CaminhoDestino.Contains("portrait_roster", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // Espera padrão `arquivos/{classe}/{prefixo}_{A|B|C|D}/{prefixo}_portrait_roster.png`
            var partes = arquivo.CaminhoDestino.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var pastaAparencia = partes.FirstOrDefault(p => p.EndsWith("_A", StringComparison.OrdinalIgnoreCase)
                                                        || p.EndsWith("_B", StringComparison.OrdinalIgnoreCase)
                                                        || p.EndsWith("_C", StringComparison.OrdinalIgnoreCase)
                                                        || p.EndsWith("_D", StringComparison.OrdinalIgnoreCase));
            if (pastaAparencia is null)
            {
                continue;
            }

            indice[pastaAparencia] = arquivo;
        }

        return indice;
    }

    private static string? ResolverCaminhoAbsoluto(string? caminhoBaseRepositorio)
    {
        if (caminhoBaseRepositorio is not null)
        {
            return Path.Combine(caminhoBaseRepositorio, CaminhoInventarioRelativo);
        }

        var atual = AppContext.BaseDirectory;
        for (int nivel = 0; nivel < 8; nivel++)
        {
            var candidato = Path.Combine(atual, CaminhoInventarioRelativo);
            if (File.Exists(candidato))
            {
                return candidato;
            }

            var pai = Directory.GetParent(atual)?.FullName;
            if (pai is null)
            {
                return null;
            }

            atual = pai;
        }

        return null;
    }

    private sealed class InventarioRaiz
    {
        public string? InstalacaoOrigem { get; set; }
        public string? DeclaracaoDeUso { get; set; }
        public List<EntradaDoInventario> Arquivos { get; set; } = new();
    }

    internal sealed class EntradaDoInventario
    {
        public string? Classe { get; set; }
        public string? CaminhoOrigem { get; set; }
        public string? CaminhoDestino { get; set; }
        public string? Sha256 { get; set; }
        public long? TamanhoBytes { get; set; }
        public bool Reutilizado { get; set; }
    }
}

/// Estatísticas do seed de assets — usadas em log e testes.
public sealed record ResultadoDoSeedDeAssets(int Coletados, int Pendentes);
