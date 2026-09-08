using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Application.Habilidades;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Habilidades;
using DarkestDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DarkestDungeon.Infrastructure.Repositories;

public sealed class HabilidadeRepository : IHabilidadeRepository
{
    private readonly DarkestDungeonDbContext contexto;

    public HabilidadeRepository(DarkestDungeonDbContext contexto)
    {
        this.contexto = contexto;
    }

    public Task<Habilidade?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        contexto.Habilidades.AsNoTracking().FirstOrDefaultAsync(h => h.Id == id, cancellationToken);

    public Task<Habilidade?> ObterPorNomeExibicaoAsync(string nomeExibicao, CancellationToken cancellationToken = default) =>
        contexto.Habilidades.AsNoTracking().FirstOrDefaultAsync(h => h.NomeExibicao == nomeExibicao, cancellationToken);

    public async Task<IReadOnlyCollection<Habilidade>> ListarAsync(CategoriaDeHabilidadeDto? categoria, Guid? classeId, CancellationToken cancellationToken = default)
    {
        IQueryable<Habilidade> query = contexto.Habilidades.AsNoTracking();

        query = categoria switch
        {
            CategoriaDeHabilidadeDto.Combate => query.OfType<HabilidadeDeCombate>(),
            CategoriaDeHabilidadeDto.Acampamento => query.OfType<HabilidadeDeAcampamento>(),
            CategoriaDeHabilidadeDto.Inimigo => query.OfType<HabilidadeDeInimigo>(),
            _ => query,
        };

        if (classeId is { } classe)
        {
            var idsHabilidades = contexto.ClassesHabilidades
                .AsNoTracking()
                .Where(a => a.ClasseId == classe)
                .Select(a => a.HabilidadeId);

            query = query.Where(h => idsHabilidades.Contains(h.Id));
        }

        return await query.OrderBy(h => h.NomeExibicao).ToArrayAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<Habilidade>> ListarPorClasseAsync(Guid classeId, CancellationToken cancellationToken = default)
    {
        var idsHabilidades = contexto.ClassesHabilidades
            .AsNoTracking()
            .Where(a => a.ClasseId == classeId)
            .Select(a => a.HabilidadeId);

        return await contexto.Habilidades
            .AsNoTracking()
            .Where(h => idsHabilidades.Contains(h.Id))
            .OrderBy(h => h.NomeExibicao)
            .ToArrayAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task AdicionarAsync(Habilidade habilidade, IReadOnlyCollection<ClasseHabilidade> associacoes, CancellationToken cancellationToken = default)
    {
        contexto.Habilidades.Add(habilidade);
        contexto.ClassesHabilidades.AddRange(associacoes);
        await contexto.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<ClasseHabilidade>> ListarAssociacoesPorHabilidadeAsync(Guid habilidadeId, CancellationToken cancellationToken = default) =>
        await contexto.ClassesHabilidades
            .AsNoTracking()
            .Where(a => a.HabilidadeId == habilidadeId)
            .ToArrayAsync(cancellationToken).ConfigureAwait(false);
}
