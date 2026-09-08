using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Domain.Seres;
using DarkestDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DarkestDungeon.Infrastructure.Repositories;

public sealed class SerRepository : ISerRepository
{
    private readonly DarkestDungeonDbContext context;

    public SerRepository(DarkestDungeonDbContext context)
    {
        this.context = context;
    }

    public async Task<Ser?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Seres.FirstOrDefaultAsync(ser => ser.Id == id, cancellationToken);
    }

    public async Task AdicionarAsync(Ser entidade, CancellationToken cancellationToken = default)
    {
        await context.Seres.AddAsync(entidade, cancellationToken);
    }

    public Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}