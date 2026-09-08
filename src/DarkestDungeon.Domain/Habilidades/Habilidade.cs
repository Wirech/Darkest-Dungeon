using DarkestDungeon.Domain.Common;

namespace DarkestDungeon.Domain.Habilidades;

/// Base identificável comum a todas as habilidades do jogo (FR-002).
/// Discriminada em `HabilidadeDeHeroi` e `HabilidadeDeInimigo`.
public abstract class Habilidade : EntidadeIdentificavel
{
    protected Habilidade()
    {
        NomeExibicao = string.Empty;
        NomeOriginal = string.Empty;
        Descricao = string.Empty;
    }

    protected Habilidade(string nomeExibicao, string nomeOriginal, string descricao, Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(nomeExibicao))
        {
            throw new ArgumentException("Nome de exibição deve ser informado.", nameof(nomeExibicao));
        }

        if (nomeExibicao.Length > 80)
        {
            throw new ArgumentException("Nome de exibição deve ter no máximo 80 caracteres.", nameof(nomeExibicao));
        }

        if (string.IsNullOrWhiteSpace(nomeOriginal))
        {
            throw new ArgumentException("Nome original deve ser informado.", nameof(nomeOriginal));
        }

        if (nomeOriginal.Length > 80)
        {
            throw new ArgumentException("Nome original deve ter no máximo 80 caracteres.", nameof(nomeOriginal));
        }

        if (descricao is null || descricao.Length > 400)
        {
            throw new ArgumentException("Descrição deve ter até 400 caracteres.", nameof(descricao));
        }

        Id = id.GetValueOrDefault(Guid.NewGuid());
        NomeExibicao = nomeExibicao.Trim();
        NomeOriginal = nomeOriginal.Trim();
        Descricao = descricao.Trim();
    }

    public string NomeExibicao { get; private set; }
    public string NomeOriginal { get; private set; }
    public string Descricao { get; private set; }
}
