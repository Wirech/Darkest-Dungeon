using DarkestDungeon.Domain.Common;

namespace DarkestDungeon.Domain.Classes;

/// Uma das 20 classes oficiais de heróis (FR-008).
/// Fornece as resistências base copiadas ao Personagem na criação (FR-016).
public sealed class Classe : EntidadeIdentificavel
{
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
}
