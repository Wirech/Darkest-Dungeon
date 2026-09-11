using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DarkestDungeon.Infrastructure.Repositories;

public sealed class ClasseRepository : IClasseRepository
{
    private readonly DarkestDungeonDbContext contexto;

    public ClasseRepository(DarkestDungeonDbContext contexto)
    {
        this.contexto = contexto;
    }

    public async Task<IReadOnlyCollection<Classe>> ListarAsync(CancellationToken cancellationToken = default) =>
        await contexto.Classes.AsNoTracking().Include(c => c.Assets).OrderBy(c => c.ClasseDeHeroi).ToArrayAsync(cancellationToken).ConfigureAwait(false);

    public Task<Classe?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        contexto.Classes.AsNoTracking().Include(c => c.Assets).FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<Classe?> ObterPorEnumAsync(ClasseDeHeroi classe, CancellationToken cancellationToken = default) =>
        contexto.Classes.AsNoTracking().Include(c => c.Assets).FirstOrDefaultAsync(c => c.ClasseDeHeroi == classe, cancellationToken);

    public Task<bool> ExisteAsync(Guid id, CancellationToken cancellationToken = default) =>
        contexto.Classes.AsNoTracking().AnyAsync(c => c.Id == id, cancellationToken);
}
