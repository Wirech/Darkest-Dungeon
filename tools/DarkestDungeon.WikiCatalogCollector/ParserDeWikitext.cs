using System.Globalization;
using System.Text.RegularExpressions;

namespace DarkestDungeon.WikiCatalogCollector;

internal static class ParserDeWikitext
{
    private static readonly Regex RegexRedirect = new(
        @"^#REDIRECT\s*\[\[([^\]]+)\]\]",
        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

    private static readonly Regex RegexInfobox = new(
        @"\{\{\s*(?:Template:)?CharacterInfobox\b(?<corpo>[\s\S]*?)\n\}\}",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex RegexEquipment = new(
        @"\{\{\s*(?:Template:)?EquipmentInfoBox\b(?<corpo>[\s\S]*?)\n\}\}",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex RegexParametro = new(
        @"^\|\s*(?<chave>[^=|]+?)\s*=\s*(?<valor>.*)$",
        RegexOptions.Multiline | RegexOptions.Compiled);

    public static string? ExtrairDestinoDeRedirect(string wikitext)
    {
        var match = RegexRedirect.Match(wikitext);
        return match.Success ? match.Groups[1].Value.Trim() : null;
    }

    public static ResultadoParse Parsear(string wikitext, PaginaDeClasseWiki pagina)
    {
        var lacunas = new List<LacunaDeColeta>();
        var infobox = ExtrairPrimeiroTemplate(wikitext, RegexInfobox);
        var equipamento = ExtrairPrimeiroTemplate(wikitext, RegexEquipment);

        if (infobox is null)
        {
            lacunas.Add(new LacunaDeColeta(pagina.NomeEnum, "CharacterInfobox", "Template CharacterInfobox ausente."));
        }

        if (equipamento is null)
        {
            lacunas.Add(new LacunaDeColeta(pagina.NomeEnum, "EquipmentInfoBox", "Template EquipmentInfoBox ausente."));
        }

        if (infobox is null || equipamento is null)
        {
            return new ResultadoParse(null, lacunas);
        }

        var camposInfobox = ExtrairParametros(infobox);
        var camposEquipamento = ExtrairParametros(equipamento);

        var passosFrente = LerPasso(camposInfobox, "forward", pagina, lacunas);
        var passosAtras = LerPasso(camposInfobox, "backward", pagina, lacunas);
        var religiosa = InterpretarReligiosa(camposInfobox.GetValueOrDefault("religious"));
        var provisao = LimparWikilink(camposInfobox.GetValueOrDefault("provisions") ?? string.Empty);
        var bonusCritico = InterpretarBonusCritico(
            camposInfobox.GetValueOrDefault("critbonus"),
            pagina,
            lacunas);

        var arma = new SnapshotDeArma();
        var armadura = new SnapshotDeArmadura();
        for (var nivel = 1; nivel <= 5; nivel++)
        {
            var dano = InterpretarDano(camposEquipamento.GetValueOrDefault($"DMG{nivel}"), pagina, $"DMG{nivel}", lacunas);
            var crit = InterpretarDecimalComPercentual(camposEquipamento.GetValueOrDefault($"CRT{nivel}"), pagina, $"CRT{nivel}", lacunas);
            var spd = InterpretarInteiro(camposEquipamento.GetValueOrDefault($"SPD{nivel}"), pagina, $"SPD{nivel}", lacunas);
            var hp = InterpretarInteiro(camposEquipamento.GetValueOrDefault($"HP{nivel}"), pagina, $"HP{nivel}", lacunas);
            var dodge = InterpretarDecimalComPercentual(camposEquipamento.GetValueOrDefault($"DODGE{nivel}"), pagina, $"DODGE{nivel}", lacunas);

            if (dano is not null && crit is not null && spd is not null)
            {
                arma.Niveis.Add(new NivelDeArmaSnapshot
                {
                    Nivel = nivel,
                    DanoMinimo = dano.Value.Min,
                    DanoMaximo = dano.Value.Max,
                    Critico = crit.Value,
                    Velocidade = spd.Value
                });
            }

            if (hp is not null && dodge is not null)
            {
                armadura.Niveis.Add(new NivelDeArmaduraSnapshot
                {
                    Nivel = nivel,
                    HpMaximo = hp.Value,
                    Esquiva = dodge.Value
                });
            }
        }

        if (lacunas.Count > 0)
        {
            return new ResultadoParse(null, lacunas);
        }

        return new ResultadoParse(
            new SnapshotDeClasseOficial
            {
                ClasseDeHeroiEnum = pagina.NomeEnum,
                Forma = "humana",
                PassosAFrente = passosFrente,
                PassosAtras = passosAtras,
                Religiosa = religiosa,
                ProvisaoInicial = provisao,
                BonusAoCritico = bonusCritico ?? string.Empty,
                Arma = arma,
                Armadura = armadura
            },
            lacunas);
    }

    private static string? ExtrairPrimeiroTemplate(string wikitext, Regex regex)
    {
        var match = regex.Match(wikitext);
        return match.Success ? match.Groups["corpo"].Value : null;
    }

    private static Dictionary<string, string> ExtrairParametros(string corpo)
    {
        var mapa = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (Match match in RegexParametro.Matches(corpo))
        {
            var chave = match.Groups["chave"].Value.Trim();
            var valor = match.Groups["valor"].Value.Trim();
            if (!string.IsNullOrWhiteSpace(chave))
            {
                mapa[chave] = valor;
            }
        }

        return mapa;
    }

    private static int LerPasso(
        IReadOnlyDictionary<string, string> campos,
        string chave,
        PaginaDeClasseWiki pagina,
        List<LacunaDeColeta> lacunas)
    {
        // Template:CharacterInfobox: {{{forward|1}}} / {{{backward|1}}} quando o parâmetro é omitido.
        if (!campos.TryGetValue(chave, out var bruto) || string.IsNullOrWhiteSpace(bruto))
        {
            return 1;
        }

        if (!int.TryParse(bruto.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var valor) || valor < 0)
        {
            lacunas.Add(new LacunaDeColeta(pagina.NomeEnum, chave, $"Passo inválido: '{bruto}'."));
            return 0;
        }

        return valor;
    }

    private static bool InterpretarReligiosa(string? bruto)
    {
        if (string.IsNullOrWhiteSpace(bruto))
        {
            return false;
        }

        return bruto.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase)
            || bruto.Trim().Equals("true", StringComparison.OrdinalIgnoreCase)
            || bruto.Trim() == "1";
    }

    private static string? InterpretarBonusCritico(string? bruto, PaginaDeClasseWiki pagina, List<LacunaDeColeta> lacunas)
    {
        if (string.IsNullOrWhiteSpace(bruto))
        {
            lacunas.Add(new LacunaDeColeta(pagina.NomeEnum, "critbonus", "Campo critbonus ausente."));
            return null;
        }

        var texto = LimparWikilink(bruto);
        texto = Regex.Replace(texto, @"<[^>]+>", string.Empty);
        texto = Regex.Replace(texto, @"\}+\s*$", string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(texto) || texto == "-")
        {
            lacunas.Add(new LacunaDeColeta(
                pagina.NomeEnum,
                "critbonus",
                $"Texto oficial do Crit Buff Bonus vazio ou placeholder: '{bruto}'."));
            return null;
        }

        return texto;
    }

    private static (int Min, int Max)? InterpretarDano(string? bruto, PaginaDeClasseWiki pagina, string campo, List<LacunaDeColeta> lacunas)
    {
        if (string.IsNullOrWhiteSpace(bruto))
        {
            lacunas.Add(new LacunaDeColeta(pagina.NomeEnum, campo, "Campo ausente."));
            return null;
        }

        var match = Regex.Match(bruto.Trim(), @"^(-?\d+)\s*-\s*(-?\d+)$");
        if (!match.Success)
        {
            lacunas.Add(new LacunaDeColeta(pagina.NomeEnum, campo, $"Faixa de dano inválida: '{bruto}'."));
            return null;
        }

        return (int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
            int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture));
    }

    private static int? InterpretarInteiro(string? bruto, PaginaDeClasseWiki pagina, string campo, List<LacunaDeColeta> lacunas)
    {
        if (string.IsNullOrWhiteSpace(bruto))
        {
            lacunas.Add(new LacunaDeColeta(pagina.NomeEnum, campo, "Campo ausente."));
            return null;
        }

        var limpo = bruto.Trim().TrimEnd('%');
        if (!int.TryParse(limpo, NumberStyles.Integer, CultureInfo.InvariantCulture, out var valor))
        {
            if (decimal.TryParse(limpo, NumberStyles.Number, CultureInfo.InvariantCulture, out var decimalValor)
                && decimalValor == decimal.Truncate(decimalValor))
            {
                return (int)decimalValor;
            }

            lacunas.Add(new LacunaDeColeta(pagina.NomeEnum, campo, $"Inteiro inválido: '{bruto}'."));
            return null;
        }

        return valor;
    }

    private static decimal? InterpretarDecimalComPercentual(string? bruto, PaginaDeClasseWiki pagina, string campo, List<LacunaDeColeta> lacunas)
    {
        if (string.IsNullOrWhiteSpace(bruto))
        {
            lacunas.Add(new LacunaDeColeta(pagina.NomeEnum, campo, "Campo ausente."));
            return null;
        }

        var limpo = bruto.Trim().TrimEnd('%');
        if (!decimal.TryParse(limpo, NumberStyles.Number, CultureInfo.InvariantCulture, out var valor))
        {
            lacunas.Add(new LacunaDeColeta(pagina.NomeEnum, campo, $"Decimal inválido: '{bruto}'."));
            return null;
        }

        return valor;
    }

    private static string LimparWikilink(string bruto)
    {
        var texto = bruto.Trim();
        texto = Regex.Replace(texto, @"\[\[(?:[^\|\]]*\|)?([^\]]+)\]\]", "$1");
        texto = Regex.Replace(texto, @"\{\{([^}|]+)(?:\|[^}]*)?\}\}", "$1");
        return Regex.Replace(texto, @"'{2,}", string.Empty).Trim();
    }
}

internal sealed record ResultadoParse(SnapshotDeClasseOficial? Snapshot, IReadOnlyList<LacunaDeColeta> Lacunas);
