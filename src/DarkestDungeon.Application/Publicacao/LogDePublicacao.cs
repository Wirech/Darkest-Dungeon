namespace DarkestDungeon.Application.Publicacao;

/// Entrada estruturada de log de publicação — persistida durante e após a transação.
/// Usa DbContext separado para garantir persistência mesmo após rollback (research R4 — padrão outbox invertido).
public sealed class LogDePublicacao
{
    private LogDePublicacao()
    {
        Mensagem = string.Empty;
    }

    public LogDePublicacao(
        Guid publicacaoId,
        NivelDeLog nivel,
        string mensagem,
        Guid? habilidadeId = null,
        string? nomeExibicao = null,
        string? campo = null,
        string? stackTrace = null,
        Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(mensagem))
        {
            throw new ArgumentException("Mensagem deve ser informada.", nameof(mensagem));
        }

        if (mensagem.Length > 1000)
        {
            throw new ArgumentException("Mensagem deve ter no máximo 1000 caracteres.", nameof(mensagem));
        }

        if (nomeExibicao is not null && nomeExibicao.Length > 200)
        {
            throw new ArgumentException("NomeExibicao deve ter no máximo 200 caracteres.", nameof(nomeExibicao));
        }

        if (campo is not null && campo.Length > 64)
        {
            throw new ArgumentException("Campo deve ter no máximo 64 caracteres.", nameof(campo));
        }

        Id = id ?? Guid.NewGuid();
        PublicacaoId = publicacaoId;
        Timestamp = DateTime.UtcNow;
        Nivel = nivel;
        Mensagem = mensagem;
        HabilidadeId = habilidadeId;
        NomeExibicao = nomeExibicao;
        Campo = campo;
        StackTrace = stackTrace;
    }

    public Guid Id { get; private set; }
    public Guid PublicacaoId { get; private set; }
    public DateTime Timestamp { get; private set; }
    public NivelDeLog Nivel { get; private set; }
    public Guid? HabilidadeId { get; private set; }
    public string? NomeExibicao { get; private set; }
    public string? Campo { get; private set; }
    public string Mensagem { get; private set; }
    public string? StackTrace { get; private set; }
}
