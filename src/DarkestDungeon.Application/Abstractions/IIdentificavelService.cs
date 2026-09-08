using DarkestDungeon.Application.Validation;

namespace DarkestDungeon.Application.Abstractions;

public interface IIdentificavelService<TDto>
{
    Task<ResultadoOperacao<TDto>> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
}