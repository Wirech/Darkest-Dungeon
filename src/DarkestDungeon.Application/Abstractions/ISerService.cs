using DarkestDungeon.Application.Seres;
using DarkestDungeon.Application.Validation;

namespace DarkestDungeon.Application.Abstractions;

public interface ISerService : IIdentificavelService<SerDto>
{
    Task<ResultadoOperacao<SerDto>> CriarAsync(CriarSerCommand command, CancellationToken cancellationToken = default);
}