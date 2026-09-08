using DarkestDungeon.Application.Itens;
using DarkestDungeon.Application.Itens.Commands;
using DarkestDungeon.Application.Validation;
using DarkestDungeon.Domain.Itens;

namespace DarkestDungeon.Application.Abstractions;

public interface IItemRepository
{
    Task<Item?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Item item, CancellationToken cancellationToken = default);
}

public interface IItemService
{
    Task<ResultadoOperacao<ItemDetalheDto>> ObterAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ResultadoOperacao<ItemDetalheDto>> CriarArmaAsync(CriarArmaCommand command, CancellationToken cancellationToken = default);
    Task<ResultadoOperacao<ItemDetalheDto>> CriarArmaduraAsync(CriarArmaduraCommand command, CancellationToken cancellationToken = default);
    Task<ResultadoOperacao<ItemDetalheDto>> CriarAcessorioAsync(CriarAcessorioCommand command, CancellationToken cancellationToken = default);
}
