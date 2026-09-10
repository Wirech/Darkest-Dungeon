using DarkestDungeon.Domain.Common;

namespace DarkestDungeon.Domain.Habilidades;

/// Base identificável comum a todas as habilidades do jogo (FR-002).
/// Discriminada em `HabilidadeDeHeroi` e `HabilidadeDeInimigo`.
public abstract class Habilidade : EntidadeIdentificavel
{
    private readonly List<NivelDeHabilidade> niveis = new();

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

    /// Atualiza a descrição PT-BR — usado pela auditoria/reconciliação com a wiki.
    public void DefinirDescricao(string novaDescricao)
    {
        if (novaDescricao is null || novaDescricao.Length > 400)
        {
            throw new ArgumentException("Descrição deve ter até 400 caracteres.", nameof(novaDescricao));
        }

        Descricao = novaDescricao.Trim();
    }

    /// Coleção de 5 níveis (Level 1..5) preenchida por `DefinirNiveis` durante seed/mineração.
    /// Vazia = habilidade single-level herdada da Feature 003 (compatibilidade retroativa).
    public IReadOnlyList<NivelDeHabilidade> Niveis => niveis;

    /// Define os 5 níveis desta habilidade. Deve conter exatamente 5 entradas com `NumeroDoNivel` 1..5 sem lacunas.
    /// Chamado pelo pipeline de seed/mineração de Feature 005.
    public void DefinirNiveis(IEnumerable<NivelDeHabilidade> niveisNovos)
    {
        ArgumentNullException.ThrowIfNull(niveisNovos);
        var arr = niveisNovos.ToArray();
        if (arr.Length != 5)
        {
            throw new ArgumentException("Uma Habilidade deve ter exatamente 5 níveis (Level 1..5).", nameof(niveisNovos));
        }

        var ordenados = arr.OrderBy(nivel => nivel.NumeroDoNivel).ToArray();
        for (int indice = 0; indice < ordenados.Length; indice++)
        {
            if (ordenados[indice].NumeroDoNivel != indice + 1)
            {
                throw new ArgumentException($"Nível na posição {indice + 1} tem NumeroDoNivel={ordenados[indice].NumeroDoNivel} (esperado {indice + 1}).", nameof(niveisNovos));
            }
        }

        niveis.Clear();
        niveis.AddRange(ordenados);
    }
}
