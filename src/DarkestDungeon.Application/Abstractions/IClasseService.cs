using DarkestDungeon.Application.Classes;
using DarkestDungeon.Application.Habilidades;
using DarkestDungeon.Application.Validation;
using DarkestDungeon.Domain.Classes;

namespace DarkestDungeon.Application.Abstractions;

public interface IClasseRepository
{
    Task<IReadOnlyCollection<Classe>> ListarAsync(CancellationToken cancellationToken = default);
    Task<Classe?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Classe?> ObterPorEnumAsync(ClasseDeHeroi classe, CancellationToken cancellationToken = default);
    Task<bool> ExisteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IClasseService
{
    Task<ResultadoOperacao<IReadOnlyCollection<ClasseResumoDto>>> ListarAsync(CancellationToken cancellationToken = default);
    Task<ResultadoOperacao<ClasseDetalheDto>> ObterAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ResultadoOperacao<IReadOnlyCollection<HabilidadeResumoDto>>> ObterHabilidadesAsync(Guid classeId, CancellationToken cancellationToken = default);
}
