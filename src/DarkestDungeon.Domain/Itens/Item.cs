using DarkestDungeon.Domain.Common;

namespace DarkestDungeon.Domain.Itens;

/// Base identificável abstrata para qualquer objeto do jogo (FR-010).
/// Discriminada em `Arma`, `Armadura`, `Acessorio`, `ItemDeAcampamento` e `Consumivel`.
public abstract class Item : EntidadeIdentificavel
{
    protected Item()
    {
        NomeExibicao = string.Empty;
        NomeOriginal = string.Empty;
        Descricao = string.Empty;
    }

    protected Item(string nomeExibicao, string nomeOriginal, string descricao, Guid? id = null)
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

    protected void AtualizarTextosDoCatalogo(string nomeExibicao, string nomeOriginal, string descricao)
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

        NomeExibicao = nomeExibicao.Trim();
        NomeOriginal = nomeOriginal.Trim();
        Descricao = descricao.Trim();
    }
}
