namespace DarkestDungeon.Application.Publicacao;

/// Request enviado ao endpoint `POST /api/publicacao`.
public sealed record SolicitacaoDePublicacao(
    bool ConfirmacaoJanelaManutencao,
    string? Observacao
);
