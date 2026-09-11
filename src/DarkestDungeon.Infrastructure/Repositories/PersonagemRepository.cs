using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Domain.Seres;
using DarkestDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DarkestDungeon.Infrastructure.Repositories;

public sealed class PersonagemRepository : IPersonagemRepository
{
    private readonly DarkestDungeonDbContext contexto;

    public PersonagemRepository(DarkestDungeonDbContext contexto)
    {
        this.contexto = contexto;
    }

    public async Task<IReadOnlyCollection<Personagem>> ListarAsync(CancellationToken cancellationToken = default) =>
        await contexto.Seres
            .OfType<Personagem>()
            .AsNoTracking()
            .OrderBy(p => p.Nome)
            .ToArrayAsync(cancellationToken)
            .ConfigureAwait(false);

    public Task<Personagem?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        contexto.Seres.OfType<Personagem>().AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task AdicionarAsync(Personagem personagem, CancellationToken cancellationToken = default)
    {
        contexto.Seres.Add(personagem);
        await contexto.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task AtualizarAsync(Personagem personagem, CancellationToken cancellationToken = default)
    {
        contexto.Seres.Update(personagem);
        await contexto.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var personagem = await contexto.Seres
            .OfType<Personagem>()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken)
            .ConfigureAwait(false);
        if (personagem is null)
        {
            return false;
        }

        contexto.Seres.Remove(personagem);
        await contexto.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return true;
    }
}

public sealed class InimigoRepository : IInimigoRepository
{
    private readonly DarkestDungeonDbContext contexto;

    public InimigoRepository(DarkestDungeonDbContext contexto)
    {
        this.contexto = contexto;
    }

    public Task<Inimigo?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        contexto.Seres.OfType<Inimigo>().AsNoTracking().FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task AdicionarAsync(Inimigo inimigo, CancellationToken cancellationToken = default)
    {
        contexto.Seres.Add(inimigo);
        await contexto.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
