using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Application.Validation;
using DarkestDungeon.Domain.Cobertura;

namespace DarkestDungeon.Application.Cobertura;

public sealed class MapaDeCoberturaService : IMapaDeCoberturaService
{
    private readonly IMapaDeCoberturaRepository repositorio;
    private readonly IClasseRepository classes;

    public MapaDeCoberturaService(IMapaDeCoberturaRepository repositorio, IClasseRepository classes)
    {
        this.repositorio = repositorio;
        this.classes = classes;
    }

    public async Task<ResultadoOperacao<IReadOnlyCollection<MapaDeCoberturaPorClasseDto>>> ObterCompletoAsync(CancellationToken cancellationToken = default)
    {
        var listaClasses = await classes.ListarAsync(cancellationToken).ConfigureAwait(false);
        var todasEntradas = await repositorio.ListarAsync(cancellationToken).ConfigureAwait(false);

        var porClasse = todasEntradas.GroupBy(e => e.Classe).ToDictionary(g => g.Key, g => g.ToArray());

        var mapa = listaClasses
            .Select(classe => new MapaDeCoberturaPorClasseDto(
                classe.Id,
                classe.ClasseDeHeroi,
                porClasse.TryGetValue(classe.ClasseDeHeroi, out var entradas)
                    ? entradas.Select(e => new EntradaCoberturaDto(e.Categoria, e.ChaveDoAtributo, e.Estado, e.Notas)).ToArray()
                    : Array.Empty<EntradaCoberturaDto>()))
            .ToArray();

        return ResultadoOperacao<IReadOnlyCollection<MapaDeCoberturaPorClasseDto>>.Ok(mapa);
    }

    public async Task<ResultadoOperacao<MapaDeCoberturaPorClasseDto>> ObterPorClasseAsync(Guid classeId, CancellationToken cancellationToken = default)
    {
        var classe = await classes.ObterPorIdAsync(classeId, cancellationToken).ConfigureAwait(false);
        if (classe is null)
        {
            return ResultadoOperacao<MapaDeCoberturaPorClasseDto>.NaoEncontrado($"Classe '{classeId}' não encontrada.");
        }

        var entradas = await repositorio.ListarPorClasseAsync(classeId, cancellationToken).ConfigureAwait(false);
        var mapa = new MapaDeCoberturaPorClasseDto(
            classe.Id,
            classe.ClasseDeHeroi,
            entradas.Select(e => new EntradaCoberturaDto(e.Categoria, e.ChaveDoAtributo, e.Estado, e.Notas)).ToArray());

        return ResultadoOperacao<MapaDeCoberturaPorClasseDto>.Ok(mapa);
    }

    public async Task<ResultadoOperacao<EntradaCoberturaDto>> RegistrarAsync(RegistrarCoberturaCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var entrada = new EntradaDoMapaDeCobertura(command.Classe, command.Categoria, command.ChaveDoAtributo, command.Estado, command.Notas);
            await repositorio.AdicionarOuAtualizarAsync(entrada, cancellationToken).ConfigureAwait(false);
            return ResultadoOperacao<EntradaCoberturaDto>.Ok(new EntradaCoberturaDto(entrada.Categoria, entrada.ChaveDoAtributo, entrada.Estado, entrada.Notas));
        }
        catch (ArgumentException ex)
        {
            return ResultadoOperacao<EntradaCoberturaDto>.Invalido(ex.Message, new ErroOperacao(ex.ParamName ?? "requisicao", ex.Message));
        }
    }
}
