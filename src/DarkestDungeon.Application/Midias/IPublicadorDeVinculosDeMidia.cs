using DarkestDungeon.Domain.Itens;

namespace DarkestDungeon.Application.Midias;

public sealed record SolicitacaoDePublicacaoDeVinculos(string Categoria, string? Observacao);

public sealed record ResultadoDePublicacaoDeVinculos(
    Guid PublicacaoId,
    CategoriaDeMidiaDeItem Categoria,
    EstadoDePublicacaoDeVinculos Estado,
    DateTimeOffset IniciadaEm,
    DateTimeOffset? ConcluidaEm,
    int ItensAtualizados,
    int VinculosOk,
    int VinculosPendentes,
    int AcessoriosNovos);

/// Publica vínculos catálogo ↔ inventário por categoria. Não reutiliza IPublicadorAtomicoService.
public interface IPublicadorDeVinculosDeMidia
{
    Task<ResultadoDePublicacaoDeVinculos> PublicarAsync(SolicitacaoDePublicacaoDeVinculos solicitacao, CancellationToken cancellationToken);

    Task<ResultadoDePublicacaoDeVinculos?> ObterStatusAsync(Guid publicacaoId, CancellationToken cancellationToken);
}

public sealed class CategoriaDeMidiaInvalidaException : Exception
{
    public CategoriaDeMidiaInvalidaException()
        : base("Informe uma categoria: arma, armadura, acessorio, acampamento ou consumivel.")
    {
    }
}

public sealed class PublicacaoDeVinculosEmCursoException : Exception
{
    public Guid PublicacaoAtivaId { get; }

    public PublicacaoDeVinculosEmCursoException(Guid publicacaoAtivaId, CategoriaDeMidiaDeItem categoria)
        : base($"Já existe uma publicação de vínculos da categoria {categoria} em execução. Aguarde a conclusão.")
    {
        PublicacaoAtivaId = publicacaoAtivaId;
    }
}

public sealed class InventarioDeMidiasAusenteException : Exception
{
    public InventarioDeMidiasAusenteException()
        : base("Execute o coletor de mídias de equipamento antes de publicar os vínculos.")
    {
    }
}
