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
    }

    public Classe(
        ClasseDeHeroi classeDeHeroi,
        string nomeExibicao,
        string nomeOriginal,
        ResistenciasDeClasse resistenciasBase,
        Guid? id = null)
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
    }

    public ClasseDeHeroi ClasseDeHeroi { get; private set; }
    public string NomeExibicao { get; private set; }
    public string NomeOriginal { get; private set; }
    public ResistenciasDeClasse ResistenciasBase { get; private set; }

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
