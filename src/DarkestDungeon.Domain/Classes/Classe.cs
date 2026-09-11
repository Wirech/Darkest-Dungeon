using DarkestDungeon.Domain.Common;

namespace DarkestDungeon.Domain.Classes;

/// Uma das 20 classes oficiais de heróis (FR-008).
/// Fornece as resistências base copiadas ao Personagem na criação (FR-016).
public sealed class Classe : EntidadeIdentificavel
{
    private readonly List<AssetsDeClasse> assets = new();

    private Classe()
    {
        NomeExibicao = string.Empty;
        NomeOriginal = string.Empty;
        ResistenciasBase = new ResistenciasDeClasse(0, 0, 0, 0, 0, 0, 0, 0);
        ProvisaoInicial = string.Empty;
        BonusAoCriticoDaClasse = string.Empty;
    }

    public Classe(
        ClasseDeHeroi classeDeHeroi,
        string nomeExibicao,
        string nomeOriginal,
        ResistenciasDeClasse resistenciasBase,
        Guid? id = null,
        int passosAFrente = 0,
        int passosAtras = 0,
        bool religiosa = false,
        string provisaoInicial = "",
        string bonusAoCriticoDaClasse = "")
    {
        if (string.IsNullOrWhiteSpace(nomeExibicao) || nomeExibicao.Length > 60)
        {
            throw new ArgumentException("Nome de exibição da Classe deve ter entre 1 e 60 caracteres.", nameof(nomeExibicao));
        }

        if (string.IsNullOrWhiteSpace(nomeOriginal) || nomeOriginal.Length > 60)
        {
            throw new ArgumentException("Nome original da Classe deve ter entre 1 e 60 caracteres.", nameof(nomeOriginal));
        }

        Id = id.GetValueOrDefault(Guid.NewGuid());
        ClasseDeHeroi = classeDeHeroi;
        NomeExibicao = nomeExibicao.Trim();
        NomeOriginal = nomeOriginal.Trim();
        ResistenciasBase = resistenciasBase ?? throw new ArgumentNullException(nameof(resistenciasBase));
        DefinirPerfilOficial(passosAFrente, passosAtras, religiosa, provisaoInicial, bonusAoCriticoDaClasse);
    }

    public ClasseDeHeroi ClasseDeHeroi { get; private set; }
    public string NomeExibicao { get; private set; }
    public string NomeOriginal { get; private set; }
    public ResistenciasDeClasse ResistenciasBase { get; private set; }

    /// Deslocamento oficial à frente (wiki Movement). Copiado ao Personagem na criação fiel.
    public int PassosAFrente { get; private set; }

    /// Deslocamento oficial para trás (wiki Movement). Copiado ao Personagem na criação fiel.
    public int PassosAtras { get; private set; }

    /// Infobox Religious. Permanece no perfil da Classe; o card lê via join.
    public bool Religiosa { get; private set; }

    /// Infobox Provisions. Permanece no perfil da Classe; o card lê via join.
    public string ProvisaoInicial { get; private set; } = string.Empty;

    /// Infobox Crit Buff Bonus (texto oficial, p.ex. "+15% PROT"). Não é o `BonusDeCritico` do herói.
    public string BonusAoCriticoDaClasse { get; private set; } = string.Empty;

    public bool PossuiDeslocamentoOficial => PassosAFrente > 0 || PassosAtras > 0;

    public void DefinirPerfilOficial(
        int passosAFrente,
        int passosAtras,
        bool religiosa,
        string? provisaoInicial,
        string? bonusAoCriticoDaClasse)
    {
        if (passosAFrente < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(passosAFrente), "Passos à frente devem ser >= 0.");
        }

        if (passosAtras < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(passosAtras), "Passos atrás devem ser >= 0.");
        }

        provisaoInicial ??= string.Empty;
        if (provisaoInicial.Length > 200)
        {
            throw new ArgumentException("Provisão inicial deve ter no máximo 200 caracteres.", nameof(provisaoInicial));
        }

        bonusAoCriticoDaClasse ??= string.Empty;
        if (bonusAoCriticoDaClasse.Length > 200)
        {
            throw new ArgumentException("Bônus ao crítico da classe deve ter no máximo 200 caracteres.", nameof(bonusAoCriticoDaClasse));
        }

        PassosAFrente = passosAFrente;
        PassosAtras = passosAtras;
        Religiosa = religiosa;
        ProvisaoInicial = provisaoInicial.Trim();
        BonusAoCriticoDaClasse = bonusAoCriticoDaClasse.Trim();
    }

    /// Feature 005 (FR-007i): 4 registros (A/B/C/D) vinculando cada aparência ao inventário Spine da Feature 004.
    public IReadOnlyList<AssetsDeClasse> Assets => assets;

    /// Define os 4 assets (A/B/C/D). Deve conter exatamente 4 entradas, uma por AparenciaDePersonagem.
    public void DefinirAssets(IEnumerable<AssetsDeClasse> assetsNovos)
    {
        ArgumentNullException.ThrowIfNull(assetsNovos);
        var arr = assetsNovos.ToArray();
        var distintos = arr.Select(asset => asset.Aparencia).Distinct().ToArray();
        if (arr.Length != 4 || distintos.Length != 4)
        {
            throw new ArgumentException("Assets de Classe devem conter exatamente 4 entradas distintas (A/B/C/D).", nameof(assetsNovos));
        }

        assets.Clear();
        assets.AddRange(arr.OrderBy(asset => asset.Aparencia));
    }
}
