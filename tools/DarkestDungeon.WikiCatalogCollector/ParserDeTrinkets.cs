using System.Globalization;
using System.Text.RegularExpressions;

namespace DarkestDungeon.WikiCatalogCollector;

internal static class ParserDeTrinkets
{
    private static readonly Regex CampoInfobox = new(
        @"^\|\s*(?<k>[^=]+?)\s*=\s*(?<v>.*)$",
        RegexOptions.Multiline | RegexOptions.Compiled);

    private static readonly Regex EfeitoLinha = new(
        @"(?<sinal>[+\-])\s*(?<valor>\d+(?:[.,]\d+)?)\s*(?<pct>%)?\s*(?<nome>[A-Za-zÀ-ú][A-Za-zÀ-ú0-9 /]+)",
        RegexOptions.Compiled);

    private static readonly IReadOnlyDictionary<string, string> Raridades = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["common"] = "Comum",
        ["comum"] = "Comum",
        ["uncommon"] = "Incomum",
        ["incomum"] = "Incomum",
        ["rare"] = "Rara",
        ["rara"] = "Rara",
        ["very rare"] = "MuitoRara",
        ["veryrare"] = "MuitoRara",
        ["muito rara"] = "MuitoRara",
        ["crimson court"] = "CrimsonCourt",
        ["crimsoncourt"] = "CrimsonCourt",
        ["crystalline"] = "Crystalline",
        ["color of madness"] = "Crystalline",
        ["set"] = "Set",
        ["ancestral"] = "Set",
    };

    private static readonly IReadOnlyDictionary<string, string> Classes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["Abomination"] = "Abominacao",
        ["Antiquarian"] = "Antiquario",
        ["Arbalest"] = "Besteiro",
        ["Bounty Hunter"] = "CacadorDeRecompensas",
        ["Crusader"] = "Cruzado",
        ["Grave Robber"] = "LadraoDeCova",
        ["Jester"] = "BoboDaCorte",
        ["Houndmaster"] = "MestreDeCaca",
        ["Leper"] = "Leproso",
        ["Hellion"] = "Infernal",
        ["Highwayman"] = "Bandido",
        ["Musketeer"] = "Musqueteiro",
        ["Man-at-Arms"] = "Veterano",
        ["Occultist"] = "Ocultista",
        ["Plague Doctor"] = "MedicoDaPeste",
        ["Vestal"] = "Vestal",
        ["Flagellant"] = "Flagelante",
        ["Shieldbreaker"] = "Rompedor",
        ["Duelist"] = "Duelista",
        ["Runaway"] = "Fugitivo",
        ["Cruzado"] = "Cruzado",
        ["Ocultista"] = "Ocultista",
    };

    private static readonly HashSet<string> NomesPercentuais = new(StringComparer.OrdinalIgnoreCase)
    {
        "MAX HP", "HP", "MAXHP", "DMG", "Damage", "Dano",
        "PROT", "Protection", "Proteção", "Protecao",
        "CRIT", "Critical", "Crítico", "Critico",
        "Virtue Chance", "Virtue", "Chance de virtude",
        "Stun", "Stun Resist", "Atordoamento",
        "Bleed", "Bleed Resist", "Sangramento",
        "Blight", "Blight Resist", "Envenenamento",
        "Debuff", "Debuff Resist",
        "Move", "Move Resist", "Movimento",
        "Disease", "Disease Resist", "Doença", "Doenca",
        "Death Blow", "Death Blow Resist", "Golpe Mortal",
        "Trap", "Trap Resist", "Armadilha",
    };

    private static readonly HashSet<string> NomesPontos = new(StringComparer.OrdinalIgnoreCase)
    {
        "ACC", "Accuracy", "Precisão", "Precisao",
        "DODGE", "Dodge", "Esquiva",
        "SPD", "Speed", "Velocidade",
    };

    public static ResultadoParseTrinket Parsear(string wikitext, string fonteUrl, string? tituloPagina = null)
    {
        var agora = DateTimeOffset.UtcNow.ToString("O", CultureInfo.InvariantCulture);
        var pagina = tituloPagina ?? ExtrairNome(wikitext) ?? "trinket-desconhecido";
        var lacunas = new List<LacunaDeTrinket>();

        if (string.IsNullOrWhiteSpace(wikitext))
        {
            lacunas.Add(Lacuna(pagina, "wikitext", "Página vazia; nenhum valor inventado.", agora));
            return new ResultadoParseTrinket(null, lacunas);
        }

        var campos = ExtrairCampos(wikitext);
        var nomeOriginal = PrimeiroNaoVazio(campos, "name", "nome") ?? tituloPagina;
        if (string.IsNullOrWhiteSpace(nomeOriginal))
        {
            lacunas.Add(Lacuna(pagina, "nomeOriginal", "Nome original ausente na wiki.", agora));
            return new ResultadoParseTrinket(null, lacunas);
        }

        var snapshot = new SnapshotDeTrinketOficial
        {
            NomeOriginal = nomeOriginal.Trim(),
            NomeExibicao = PrimeiroNaoVazio(campos, "nomeexibicao", "display") ?? nomeOriginal.Trim(),
            Descricao = PrimeiroNaoVazio(campos, "description", "descricao") ?? string.Empty,
            FonteUrl = fonteUrl,
            ConjuntoIdWiki = PrimeiroNaoVazio(campos, "set", "conjunto"),
        };

        var raridadeBruta = PrimeiroNaoVazio(campos, "rarity", "raridade");
        if (string.IsNullOrWhiteSpace(raridadeBruta))
        {
            lacunas.Add(Lacuna(nomeOriginal, "raridade", "Raridade ausente; não foi atribuído Comum.", agora));
        }
        else if (TentarMapearRaridade(raridadeBruta, out var raridade))
        {
            snapshot.Raridade = raridade;
        }
        else
        {
            lacunas.Add(Lacuna(nomeOriginal, "raridade", $"Raridade ilegível: '{raridadeBruta}'.", agora));
        }

        var classeBruta = PrimeiroNaoVazio(campos, "class", "classe", "restriction", "restricted");
        if (!string.IsNullOrWhiteSpace(classeBruta) && TentarMapearClasse(classeBruta, out var classe))
        {
            snapshot.ClasseExclusiva = classe;
        }

        var textoEfeitos = string.Join('\n',
            campos.Where(kv => kv.Key.Contains("effect", StringComparison.OrdinalIgnoreCase)
                               || kv.Key.Contains("efeito", StringComparison.OrdinalIgnoreCase))
                .Select(kv => kv.Value));
        if (string.IsNullOrWhiteSpace(textoEfeitos))
        {
            textoEfeitos = wikitext;
        }

        foreach (Match match in EfeitoLinha.Matches(textoEfeitos))
        {
            var nome = NormalizarNomeEfeito(match.Groups["nome"].Value);
            if (nome.Length is 0 or > 80)
            {
                continue;
            }

            if (!decimal.TryParse(
                    match.Groups["valor"].Value.Replace(',', '.'),
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out var valor))
            {
                lacunas.Add(Lacuna(nomeOriginal, "efeito.valor", $"Valor ilegível em '{match.Value}'.", agora));
                continue;
            }

            var percentual = match.Groups["pct"].Success;
            var unidadeEsperada = UnidadeEsperada(nome);
            if (unidadeEsperada is null)
            {
                continue;
            }

            if (percentual && unidadeEsperada == "Pontos")
            {
                lacunas.Add(Lacuna(nomeOriginal, "efeito.unidade", $"Unidade incompatível para '{nome}' (esperado pontos).", agora));
                continue;
            }

            if (!percentual && unidadeEsperada == "Percentual")
            {
                lacunas.Add(Lacuna(nomeOriginal, "efeito.unidade", $"Unidade incompatível para '{nome}' (esperado percentual).", agora));
                continue;
            }

            snapshot.Efeitos.Add(new EfeitoDeTrinketSnapshot
            {
                Nome = nome,
                Valor = valor,
                Unidade = unidadeEsperada,
                Sinal = match.Groups["sinal"].Value == "-" ? "Negativo" : "Positivo",
            });
        }

        if (snapshot.Efeitos.Count == 0 && campos.Keys.Any(k => k.Contains("effect", StringComparison.OrdinalIgnoreCase)))
        {
            lacunas.Add(Lacuna(nomeOriginal, "efeitos", "Efeitos publicados mas ilegíveis; nenhum valor inventado.", agora));
        }

        snapshot.Lacunas = lacunas;
        return new ResultadoParseTrinket(snapshot, lacunas);
    }

    private static Dictionary<string, string> ExtrairCampos(string wikitext)
    {
        var campos = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (Match match in CampoInfobox.Matches(wikitext))
        {
            var chave = match.Groups["k"].Value.Trim().ToLowerInvariant();
            var valor = Regex.Replace(match.Groups["v"].Value, @"'{2,}|<br\s*/?>", " ", RegexOptions.IgnoreCase).Trim();
            if (chave.Length > 0)
            {
                campos[chave] = valor;
            }
        }

        return campos;
    }

    private static string? ExtrairNome(string wikitext)
    {
        var match = Regex.Match(wikitext, @"^\|\s*name\s*=\s*(.+)$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value.Trim() : null;
    }

    private static string? PrimeiroNaoVazio(IReadOnlyDictionary<string, string> campos, params string[] chaves)
    {
        foreach (var chave in chaves)
        {
            if (campos.TryGetValue(chave, out var valor) && !string.IsNullOrWhiteSpace(valor))
            {
                return valor.Trim();
            }
        }

        return null;
    }

    private static bool TentarMapearRaridade(string bruto, out string raridade)
    {
        var chave = Regex.Replace(bruto, @"\[\[|\]\]|'{2,}", "").Trim();
        if (Raridades.TryGetValue(chave, out raridade!))
        {
            return true;
        }

        raridade = string.Empty;
        return false;
    }

    private static bool TentarMapearClasse(string bruto, out string classe)
    {
        var texto = Regex.Replace(bruto, @"\[\[|\]\]", "").Trim();
        foreach (var par in Classes)
        {
            if (texto.Contains(par.Key, StringComparison.OrdinalIgnoreCase))
            {
                classe = par.Value;
                return true;
            }
        }

        classe = string.Empty;
        return false;
    }

    private static string NormalizarNomeEfeito(string bruto)
    {
        var nome = Regex.Replace(bruto, @"\s+", " ").Trim();
        if (nome.EndsWith(" if", StringComparison.OrdinalIgnoreCase))
        {
            nome = nome[..^3].Trim();
        }

        return nome.Length > 80 ? nome[..80] : nome;
    }

    private static string? UnidadeEsperada(string nome)
    {
        if (NomesPercentuais.Contains(nome))
        {
            return "Percentual";
        }

        if (NomesPontos.Contains(nome))
        {
            return "Pontos";
        }

        foreach (var conhecido in NomesPercentuais.Concat(NomesPontos))
        {
            if (nome.StartsWith(conhecido, StringComparison.OrdinalIgnoreCase))
            {
                return NomesPontos.Contains(conhecido) ? "Pontos" : "Percentual";
            }
        }

        return null;
    }

    private static LacunaDeTrinket Lacuna(string pagina, string campo, string motivo, string momento) => new()
    {
        TrinketOuPagina = pagina,
        Campo = campo,
        Motivo = motivo,
        Momento = momento,
    };
}
