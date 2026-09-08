using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Application.Classes;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Cobertura;
using DarkestDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DarkestDungeon.Infrastructure.Repositories;

public sealed class MapaDeCoberturaRepository : IMapaDeCoberturaRepository
{
    private readonly DarkestDungeonDbContext contexto;

    public MapaDeCoberturaRepository(DarkestDungeonDbContext contexto)
    {
        this.contexto = contexto;
    }

    public async Task<IReadOnlyCollection<EntradaDoMapaDeCobertura>> ListarAsync(CancellationToken cancellationToken = default) =>
        await contexto.MapaDeCobertura.AsNoTracking().ToArrayAsync(cancellationToken).ConfigureAwait(false);

    public async Task<IReadOnlyCollection<EntradaDoMapaDeCobertura>> ListarPorClasseAsync(Guid classeId, CancellationToken cancellationToken = default)
    {
        var classe = await contexto.Classes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == classeId, cancellationToken).ConfigureAwait(false);
        if (classe is null)
        {
            return Array.Empty<EntradaDoMapaDeCobertura>();
        }

        return await contexto.MapaDeCobertura
            .AsNoTracking()
            .Where(e => e.Classe == classe.ClasseDeHeroi)
            .ToArrayAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task AdicionarOuAtualizarAsync(EntradaDoMapaDeCobertura entrada, CancellationToken cancellationToken = default)
    {
        var existente = await contexto.MapaDeCobertura
            .FirstOrDefaultAsync(e =>
                e.Classe == entrada.Classe &&
                e.Categoria == entrada.Categoria &&
                e.ChaveDoAtributo == entrada.ChaveDoAtributo, cancellationToken).ConfigureAwait(false);

        if (existente is null)
        {
            contexto.MapaDeCobertura.Add(entrada);
        }
        else
        {
            existente.AtualizarEstado(entrada.Estado, entrada.Notas);
        }

        await contexto.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
