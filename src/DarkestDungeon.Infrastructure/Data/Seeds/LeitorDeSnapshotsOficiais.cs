using System.Text.Json;
using System.Text.RegularExpressions;
using DarkestDungeon.Domain.Classes;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

public static class LeitorDeSnapshotsOficiais
{
    internal const string DiretorioRelativo = "specs/009-atributos-oficiais-personagem/wiki-snapshots";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    internal static readonly IReadOnlyDictionary<ClasseDeHeroi, string> SlugPorClasse = new Dictionary<ClasseDeHeroi, string>
    {
        [ClasseDeHeroi.Abominacao] = "abominacao",
        [ClasseDeHeroi.Antiquario] = "antiquario",
        [ClasseDeHeroi.Besteiro] = "besteiro",
        [ClasseDeHeroi.CacadorDeRecompensas] = "cacador-de-recompensas",
        [ClasseDeHeroi.Cruzado] = "cruzado",
        [ClasseDeHeroi.LadraoDeCova] = "ladrao-de-cova",
        [ClasseDeHeroi.BoboDaCorte] = "bobo-da-corte",
        [ClasseDeHeroi.MestreDeCaca] = "mestre-de-caca",
        [ClasseDeHeroi.Leproso] = "leproso",
        [ClasseDeHeroi.Infernal] = "infernal",
        [ClasseDeHeroi.Bandido] = "bandido",
        [ClasseDeHeroi.Musqueteiro] = "musqueteiro",
        [ClasseDeHeroi.Veterano] = "veterano",
        [ClasseDeHeroi.Ocultista] = "ocultista",
        [ClasseDeHeroi.MedicoDaPeste] = "medico-da-peste",
        [ClasseDeHeroi.Vestal] = "vestal",
        [ClasseDeHeroi.Flagelante] = "flagelante",
        [ClasseDeHeroi.Rompedor] = "rompedor",
        [ClasseDeHeroi.Duelista] = "duelista",
        [ClasseDeHeroi.Fugitivo] = "fugitivo"
    };

    public static string? EncontrarDiretorio()
    {
        var atual = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (atual is not null)
        {
            var candidato = Path.Combine(atual.FullName, DiretorioRelativo);
            if (Directory.Exists(candidato))
            {
                return candidato;
            }

            atual = atual.Parent;
        }

        return null;
    }

    public static IReadOnlyDictionary<ClasseDeHeroi, SnapshotOficialDeClasse> Carregar()
    {
        var diretorio = EncontrarDiretorio();
        if (diretorio is null)
        {
            return new Dictionary<ClasseDeHeroi, SnapshotOficialDeClasse>();
        }

        var mapa = new Dictionary<ClasseDeHeroi, SnapshotOficialDeClasse>();
        foreach (var (classe, slug) in SlugPorClasse)
        {
            var caminho = Path.Combine(diretorio, $"{slug}.json");
            if (!File.Exists(caminho))
            {
                continue;
            }

            var json = File.ReadAllText(caminho);
            var snapshot = JsonSerializer.Deserialize<SnapshotOficialDeClasse>(json, JsonOptions);
            if (snapshot is null)
            {
                continue;
            }

            snapshot.BonusAoCritico = SanitizarBonusAoCritico(snapshot.BonusAoCritico);
            mapa[classe] = snapshot;
        }

        return mapa;
    }

    internal static string SanitizarBonusAoCritico(string? bruto)
    {
        if (string.IsNullOrWhiteSpace(bruto))
        {
            return string.Empty;
        }

        var texto = bruto.Trim();
        texto = Regex.Replace(texto, @"\[\[(?:[^\|\]]*\|)?([^\]]+)\]\]", "$1");
        texto = Regex.Replace(texto, @"\{\{([^}|]+)(?:\|[^}]*)?\}\}", "$1");
        texto = Regex.Replace(texto, @"\}+\s*$", string.Empty).Trim();
        return texto;
    }
}

public sealed class SnapshotOficialDeClasse
{
    public string ClasseDeHeroiEnum { get; set; } = string.Empty;
    public string FonteUrl { get; set; } = string.Empty;
    public string Forma { get; set; } = "humana";
    public int PassosAFrente { get; set; }
    public int PassosAtras { get; set; }
    public bool Religiosa { get; set; }
    public string ProvisaoInicial { get; set; } = string.Empty;
    public string BonusAoCritico { get; set; } = string.Empty;
    public SnapshotOficialDeArma Arma { get; set; } = new();
    public SnapshotOficialDeArmadura Armadura { get; set; } = new();
}

public sealed class SnapshotOficialDeArma
{
    public List<NivelOficialDeArma> Niveis { get; set; } = new();
}

public sealed class SnapshotOficialDeArmadura
{
    public List<NivelOficialDeArmadura> Niveis { get; set; } = new();
}

public sealed class NivelOficialDeArma
{
    public int Nivel { get; set; }
    public int DanoMinimo { get; set; }
    public int DanoMaximo { get; set; }
    public decimal Critico { get; set; }
    public int Velocidade { get; set; }
}

public sealed class NivelOficialDeArmadura
{
    public int Nivel { get; set; }
    public int HpMaximo { get; set; }
    public decimal Esquiva { get; set; }
}
