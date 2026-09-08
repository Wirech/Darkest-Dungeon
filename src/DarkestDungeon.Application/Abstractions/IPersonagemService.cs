using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Application.Personagens.Commands;
using DarkestDungeon.Application.Validation;
using DarkestDungeon.Domain.Seres;

namespace DarkestDungeon.Application.Abstractions;

public interface IPersonagemRepository
{
    Task<Personagem?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Personagem personagem, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Personagem personagem, CancellationToken cancellationToken = default);
}

public interface IInimigoRepository
{
    Task<Inimigo?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Inimigo inimigo, CancellationToken cancellationToken = default);
}

public interface IPersonagemService
{
    Task<ResultadoOperacao<PersonagemDetalheDto>> ObterAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ResultadoOperacao<PersonagemDetalheDto>> CriarAsync(CriarPersonagemCommand command, CancellationToken cancellationToken = default);
    Task<ResultadoOperacao<PersonagemDetalheDto>> EquiparAsync(EquiparPersonagemCommand command, CancellationToken cancellationToken = default);
}

public interface IInimigoService
{
    Task<ResultadoOperacao<InimigoDetalheDto>> ObterAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ResultadoOperacao<InimigoDetalheDto>> CriarAsync(CriarInimigoCommand command, CancellationToken cancellationToken = default);
}
