using System.Text.RegularExpressions;
using DarkestDungeon.Application.Midias;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Itens;

namespace DarkestDungeon.Infrastructure.Midias;

public static class ResolvedorDeMidiaDeNivel
{
    private static readonly Regex NivelNoNome = new(@"(?:weapon|armou?r)[_-]?(\d)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static MidiaDeItem Resolver(IReadOnlyList<ArquivoDeInventarioDeMidia> arquivos, ClasseDeHeroi classe, int nivel, bool arma)
    {
        var pasta = PastaDaClasse(classe);
        var pngs = arquivos.Where(a =>
            a.CaminhoOrigem.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
            && CaminhoDaClasse(a, pasta)
            && NomeCompativel(a, arma)).ToArray();

        var escolhido = pngs.FirstOrDefault(a => ExtrairNivel(a) == nivel)
            ?? pngs.ElementAtOrDefault(nivel - 1);

        if (escolhido is null)
        {
            return MidiaDeItem.Pendente();
        }

        var id = RelativoInventario(escolhido);
        var spine = arquivos.FirstOrDefault(a =>
            CaminhoDaClasse(a, pasta)
            && NomeCompativel(a, arma)
            && EhArquivoSpine(a));

        var conjunto = spine is not null ? RelativoInventario(spine) : null;

        return MidiaDeItem.Ok(id, escolhido.Sha256, conjunto);
    }

    private static bool NomeCompativel(ArquivoDeInventarioDeMidia arquivo, bool arma)
    {
        var nome = Path.GetFileName(arquivo.CaminhoOrigem);
        return arma
            ? nome.Contains("weapon", StringComparison.OrdinalIgnoreCase)
            : nome.Contains("armour", StringComparison.OrdinalIgnoreCase) || nome.Contains("armor", StringComparison.OrdinalIgnoreCase);
    }

    private static bool EhArquivoSpine(ArquivoDeInventarioDeMidia arquivo)
    {
        var extensao = Path.GetExtension(arquivo.CaminhoOrigem);
        return extensao.Equals(".atlas", StringComparison.OrdinalIgnoreCase)
            || extensao.Equals(".skel", StringComparison.OrdinalIgnoreCase);
    }

    private static bool CaminhoDaClasse(ArquivoDeInventarioDeMidia arquivo, string pasta)
    {
        if (!string.IsNullOrWhiteSpace(arquivo.Classe)
            && arquivo.Classe.Equals(pasta, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return arquivo.CaminhoOrigem.Replace('\\', '/').Contains("/" + pasta + "/", StringComparison.OrdinalIgnoreCase)
            || arquivo.CaminhoDestino.Replace('\\', '/').Contains("/" + pasta + "/", StringComparison.OrdinalIgnoreCase);
    }

    private static int? ExtrairNivel(ArquivoDeInventarioDeMidia arquivo)
    {
        var match = NivelNoNome.Match(Path.GetFileNameWithoutExtension(arquivo.CaminhoOrigem));
        return match.Success && int.TryParse(match.Groups[1].Value, out var nivel) ? nivel : null;
    }

    private static string RelativoInventario(ArquivoDeInventarioDeMidia arquivo)
    {
        var destino = arquivo.CaminhoDestino.Replace('\\', '/');
        if (destino.Length <= 200)
        {
            return destino;
        }

        return Path.GetFileName(destino);
    }

    public static string PastaDaClasse(ClasseDeHeroi classe) => classe switch
    {
        ClasseDeHeroi.Abominacao => "abomination",
        ClasseDeHeroi.Antiquario => "antiquarian",
        ClasseDeHeroi.Besteiro => "arbalest",
        ClasseDeHeroi.CacadorDeRecompensas => "bounty_hunter",
        ClasseDeHeroi.Cruzado => "crusader",
        ClasseDeHeroi.LadraoDeCova => "grave_robber",
        ClasseDeHeroi.BoboDaCorte => "jester",
        ClasseDeHeroi.MestreDeCaca => "houndmaster",
        ClasseDeHeroi.Leproso => "leper",
        ClasseDeHeroi.Infernal => "hellion",
        ClasseDeHeroi.Bandido => "highwayman",
        ClasseDeHeroi.Musqueteiro => "musketeer",
        ClasseDeHeroi.Veterano => "man_at_arms",
        ClasseDeHeroi.Ocultista => "occultist",
        ClasseDeHeroi.MedicoDaPeste => "plague_doctor",
        ClasseDeHeroi.Vestal => "vestal",
        ClasseDeHeroi.Flagelante => "flagellant",
        ClasseDeHeroi.Rompedor => "shieldbreaker",
        ClasseDeHeroi.Duelista => "duelist",
        ClasseDeHeroi.Fugitivo => "runaway",
        _ => classe.ToString().ToLowerInvariant(),
    };
}
