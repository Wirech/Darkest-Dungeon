using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Application.Itens.Commands;
using DarkestDungeon.Application.Validation;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Itens;

namespace DarkestDungeon.Application.Itens;

public sealed class ItemService : IItemService
{
    private readonly IItemRepository itens;

    public ItemService(IItemRepository itens)
    {
        this.itens = itens;
    }

    public async Task<ResultadoOperacao<ItemDetalheDto>> ObterAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await itens.ObterPorIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (item is null)
        {
            return ResultadoOperacao<ItemDetalheDto>.NaoEncontrado($"Item '{id}' não encontrado.");
        }

        return ResultadoOperacao<ItemDetalheDto>.Ok(ItemMapper.ParaDto(item));
    }

    public async Task<ResultadoOperacao<ItemDetalheDto>> CriarArmaAsync(CriarArmaCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var arma = new Arma(
                command.NomeExibicao,
                command.NomeOriginal,
                command.Descricao,
                command.ClasseElegivel,
                command.Niveis.Select(n => new NivelDeArma(n.Nivel, n.DanoMinimo, n.DanoMaximo, n.Critico, n.Velocidade)));

            await itens.AdicionarAsync(arma, cancellationToken).ConfigureAwait(false);
            return ResultadoOperacao<ItemDetalheDto>.Ok(ItemMapper.ParaDto(arma));
        }
        catch (ArgumentException ex)
        {
            return ResultadoOperacao<ItemDetalheDto>.Invalido(ex.Message, new ErroOperacao(ex.ParamName ?? "requisicao", ex.Message));
        }
    }

    public async Task<ResultadoOperacao<ItemDetalheDto>> CriarArmaduraAsync(CriarArmaduraCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var armadura = new Armadura(
                command.NomeExibicao,
                command.NomeOriginal,
                command.Descricao,
                command.ClasseElegivel,
                command.Niveis.Select(n => new NivelDeArmadura(n.Nivel, n.HpAdicional, n.Esquiva)));

            await itens.AdicionarAsync(armadura, cancellationToken).ConfigureAwait(false);
            return ResultadoOperacao<ItemDetalheDto>.Ok(ItemMapper.ParaDto(armadura));
        }
        catch (ArgumentException ex)
        {
            return ResultadoOperacao<ItemDetalheDto>.Invalido(ex.Message, new ErroOperacao(ex.ParamName ?? "requisicao", ex.Message));
        }
    }

    public async Task<ResultadoOperacao<ItemDetalheDto>> CriarAcessorioAsync(CriarAcessorioCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var acessorio = new Acessorio(
                command.NomeExibicao,
                command.NomeOriginal,
                command.Descricao,
                command.Raridade,
                command.Efeitos.Select(e => new EfeitoDeAcessorio(e.Nome, e.Valor, e.Unidade, e.Sinal)),
                command.ClasseExclusiva,
                command.ConjuntoId);

            await itens.AdicionarAsync(acessorio, cancellationToken).ConfigureAwait(false);
            return ResultadoOperacao<ItemDetalheDto>.Ok(ItemMapper.ParaDto(acessorio));
        }
        catch (ArgumentException ex)
        {
            return ResultadoOperacao<ItemDetalheDto>.Invalido(ex.Message, new ErroOperacao(ex.ParamName ?? "requisicao", ex.Message));
        }
    }

    public async Task<ResultadoOperacao<ItemDetalheDto>> CriarItemDeAcampamentoAsync(CriarItemSimplesCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var item = new ItemDeAcampamento(command.NomeExibicao, command.NomeOriginal, command.Descricao);
            await itens.AdicionarAsync(item, cancellationToken).ConfigureAwait(false);
            return ResultadoOperacao<ItemDetalheDto>.Ok(ItemMapper.ParaDto(item));
        }
        catch (ArgumentException ex)
        {
            return ResultadoOperacao<ItemDetalheDto>.Invalido(ex.Message, new ErroOperacao(ex.ParamName ?? "requisicao", ex.Message));
        }
    }

    public async Task<ResultadoOperacao<ItemDetalheDto>> CriarConsumivelAsync(CriarItemSimplesCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var item = new Consumivel(command.NomeExibicao, command.NomeOriginal, command.Descricao);
            await itens.AdicionarAsync(item, cancellationToken).ConfigureAwait(false);
            return ResultadoOperacao<ItemDetalheDto>.Ok(ItemMapper.ParaDto(item));
        }
        catch (ArgumentException ex)
        {
            return ResultadoOperacao<ItemDetalheDto>.Invalido(ex.Message, new ErroOperacao(ex.ParamName ?? "requisicao", ex.Message));
        }
    }

    public async Task<ResultadoOperacao<IReadOnlyList<ItemDetalheDto>>> ListarAcessoriosAsync(
        ClasseDeHeroi classe,
        Guid? excluirId,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(classe))
        {
            return ResultadoOperacao<IReadOnlyList<ItemDetalheDto>>.Invalido(
                "Classe inválida.",
                new ErroOperacao("classe", "Classe inválida."));
        }

        var todos = await itens.ListarTodosAsync(cancellationToken).ConfigureAwait(false);
        var lista = todos
            .OfType<Acessorio>()
            .Where(a => a.ClasseExclusiva is null || a.ClasseExclusiva == classe)
            .Where(a => excluirId is null || a.Id != excluirId)
            .Select(ItemMapper.ParaDto)
            .ToArray();

        return ResultadoOperacao<IReadOnlyList<ItemDetalheDto>>.Ok(lista);
    }
}
