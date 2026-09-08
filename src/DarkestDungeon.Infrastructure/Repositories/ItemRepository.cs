using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Domain.Itens;
using DarkestDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DarkestDungeon.Infrastructure.Repositories;

public sealed class ItemRepository : IItemRepository
{
    private readonly DarkestDungeonDbContext contexto;

    public ItemRepository(DarkestDungeonDbContext contexto)
    {
        this.contexto = contexto;
    }

    public Task<Item?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        contexto.Itens.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task AdicionarAsync(Item item, CancellationToken cancellationToken = default)
    {
        contexto.Itens.Add(item);
        await contexto.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
