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

    public async Task<IReadOnlyList<Item>> ListarPorIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var lista = ids.Distinct().ToArray();
        if (lista.Length == 0)
        {
            return [];
        }

        return await contexto.Itens.AsNoTracking().Where(i => lista.Contains(i.Id)).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Item>> ListarTodosAsync(CancellationToken cancellationToken = default) =>
        await contexto.Itens.AsNoTracking().ToListAsync(cancellationToken);

    public async Task AdicionarAsync(Item item, CancellationToken cancellationToken = default)
    {
        contexto.Itens.Add(item);
        await contexto.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task AtualizarAsync(Item item, CancellationToken cancellationToken = default)
    {
        contexto.Itens.Update(item);
        await contexto.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public Task<Acessorio?> ObterAcessorioPorNomeOriginalAsync(string nomeOriginal, CancellationToken cancellationToken = default)
    {
        var chave = nomeOriginal.Trim();
        return contexto.Itens
            .OfType<Acessorio>()
            .FirstOrDefaultAsync(a => a.NomeOriginal.ToLower() == chave.ToLower(), cancellationToken);
    }
}
