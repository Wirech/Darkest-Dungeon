namespace DarkestDungeon.Application.Publicacao;

/// Estado final de uma publicação.
public enum EstadoDePublicacao
{
    EmExecucao = 0,
    Concluida = 1,
    RollbackAplicado = 2,
    Rejeitada = 3,
}

/// Resultado retornado ao endpoint `POST /api/publicacao`.
public sealed record ResultadoDePublicacao(
    Guid PublicacaoId,
    DateTime IniciadaEm,
    DateTime? ConcluidaEm,
    EstadoDePublicacao Estado,
    int HabilidadesAtualizadas,
    int NiveisAtualizados,
    int Erros,
    string? CampoQueFalhou,
    string? HabilidadeAfetada,
    string? Mensagem
);
