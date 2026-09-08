using DarkestDungeon.Domain.Common;

namespace DarkestDungeon.Application.Abstractions;

public interface IRepositorioIdentificavel<TEntity>
    where TEntity : EntidadeIdentificavel
{
    Task<TEntity?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AdicionarAsync(TEntity entidade, CancellationToken cancellationToken = default);
    Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
}