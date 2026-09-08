using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Domain.Common;
using DarkestDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DarkestDungeon.Infrastructure.Repositories;

public sealed class RepositorioIdentificavel<TEntity> : IRepositorioIdentificavel<TEntity>
    where TEntity : EntidadeIdentificavel
{
    private readonly DarkestDungeonDbContext context;

    public RepositorioIdentificavel(DarkestDungeonDbContext context)
    {
        this.context = context;
    }

    public async Task<TEntity?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Set<TEntity>().FirstOrDefaultAsync(entidade => entidade.Id == id, cancellationToken);
    }

    public async Task AdicionarAsync(TEntity entidade, CancellationToken cancellationToken = default)
    {
        await context.Set<TEntity>().AddAsync(entidade, cancellationToken);
    }

    public Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}