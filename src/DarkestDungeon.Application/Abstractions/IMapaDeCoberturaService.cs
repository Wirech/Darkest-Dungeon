using DarkestDungeon.Application.Cobertura;
using DarkestDungeon.Application.Validation;
using DarkestDungeon.Domain.Cobertura;

namespace DarkestDungeon.Application.Abstractions;

public interface IMapaDeCoberturaRepository
{
    Task<IReadOnlyCollection<EntradaDoMapaDeCobertura>> ListarAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<EntradaDoMapaDeCobertura>> ListarPorClasseAsync(Guid classeId, CancellationToken cancellationToken = default);
    Task AdicionarOuAtualizarAsync(EntradaDoMapaDeCobertura entrada, CancellationToken cancellationToken = default);
}

public interface IMapaDeCoberturaService
{
    Task<ResultadoOperacao<IReadOnlyCollection<MapaDeCoberturaPorClasseDto>>> ObterCompletoAsync(CancellationToken cancellationToken = default);
    Task<ResultadoOperacao<MapaDeCoberturaPorClasseDto>> ObterPorClasseAsync(Guid classeId, CancellationToken cancellationToken = default);
    Task<ResultadoOperacao<EntradaCoberturaDto>> RegistrarAsync(RegistrarCoberturaCommand command, CancellationToken cancellationToken = default);
}
