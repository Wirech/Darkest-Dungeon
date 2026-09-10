namespace DarkestDungeon.Application.Publicacao;

/// Detecta sessões ativas de jogadores no SQL Server via `sys.dm_exec_sessions` (research R3).
public interface IDetectorDeSessoesAtivas
{
    Task<int> ContarSessoesAtivasAsync(CancellationToken cancellationToken);
}

/// Serviço que executa a publicação atômica das habilidades no SQL Server.
public interface IPublicadorAtomicoService
{
    /// Executa a publicação. Retorna resultado com estado final (Concluida ou RollbackAplicado).
    /// Lança se: (a) confirmação de janela ausente, (b) publicação já em curso, (c) sessões ativas > 0.
    Task<ResultadoDePublicacao> PublicarAsync(SolicitacaoDePublicacao solicitacao, CancellationToken cancellationToken);

    /// Consulta o status de uma publicação por ID. Retorna null se desconhecida.
    Task<ResultadoDePublicacao?> ObterStatusAsync(Guid publicacaoId, CancellationToken cancellationToken);

    /// Consulta o log estruturado de uma publicação (filtros opcionais por nível e campo).
    Task<IReadOnlyList<LogDePublicacao>> ObterLogsAsync(Guid publicacaoId, NivelDeLog? nivelFiltro, string? campoFiltro, CancellationToken cancellationToken);
}

/// Exceção específica para publicação bloqueada por sessões ativas.
public sealed class SessoesAtivasException : Exception
{
    public int SessoesAtivas { get; }

    public SessoesAtivasException(int sessoesAtivas)
        : base($"Publicação bloqueada: existem {sessoesAtivas} sessões ativas. Aguarde janela de manutenção.")
    {
        SessoesAtivas = sessoesAtivas;
    }
}

/// Exceção para tentativa concorrente de publicação.
public sealed class PublicacaoEmCursoException : Exception
{
    public Guid PublicacaoAtivaId { get; }

    public PublicacaoEmCursoException(Guid publicacaoAtivaId)
        : base($"Publicação em andamento (PublicacaoId={publicacaoAtivaId}). Aguarde a conclusão.")
    {
        PublicacaoAtivaId = publicacaoAtivaId;
    }
}

/// Exceção para confirmação de janela de manutenção ausente.
public sealed class ConfirmacaoAusenteException : Exception
{
    public ConfirmacaoAusenteException()
        : base("Confirmação obrigatória: 'confirmacaoJanelaManutencao' deve ser true.")
    {
    }
}
