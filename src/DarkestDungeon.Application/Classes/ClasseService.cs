using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Application.Classes;
using DarkestDungeon.Application.Habilidades;
using DarkestDungeon.Application.Validation;

namespace DarkestDungeon.Application.Classes;

public sealed class ClasseService : IClasseService
{
    private readonly IClasseRepository classes;
    private readonly IHabilidadeRepository habilidades;

    public ClasseService(IClasseRepository classes, IHabilidadeRepository habilidades)
    {
        this.classes = classes;
        this.habilidades = habilidades;
    }

    public async Task<ResultadoOperacao<IReadOnlyCollection<ClasseResumoDto>>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var lista = await classes.ListarAsync(cancellationToken).ConfigureAwait(false);
        var resumo = lista
            .Select(c => new ClasseResumoDto(c.Id, c.ClasseDeHeroi, c.NomeExibicao, c.NomeOriginal))
            .ToArray();
        return ResultadoOperacao<IReadOnlyCollection<ClasseResumoDto>>.Ok(resumo);
    }

    public async Task<ResultadoOperacao<ClasseDetalheDto>> ObterAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var classe = await classes.ObterPorIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (classe is null)
        {
            return ResultadoOperacao<ClasseDetalheDto>.NaoEncontrado($"Classe '{id}' não encontrada.");
        }

        var resistencias = new ResistenciasDto(
            classe.ResistenciasBase.Atordoamento,
            classe.ResistenciasBase.Sangramento,
            classe.ResistenciasBase.Envenenamento,
            classe.ResistenciasBase.Debuff,
            classe.ResistenciasBase.Movimento,
            classe.ResistenciasBase.Doenca,
            classe.ResistenciasBase.GolpeMortal,
            classe.ResistenciasBase.Armadilha);

        return ResultadoOperacao<ClasseDetalheDto>.Ok(new ClasseDetalheDto(
            classe.Id,
            classe.ClasseDeHeroi,
            classe.NomeExibicao,
            classe.NomeOriginal,
            resistencias));
    }

    public async Task<ResultadoOperacao<IReadOnlyCollection<HabilidadeResumoDto>>> ObterHabilidadesAsync(Guid classeId, CancellationToken cancellationToken = default)
    {
        if (!await classes.ExisteAsync(classeId, cancellationToken).ConfigureAwait(false))
        {
            return ResultadoOperacao<IReadOnlyCollection<HabilidadeResumoDto>>.NaoEncontrado($"Classe '{classeId}' não encontrada.");
        }

        var lista = await habilidades.ListarPorClasseAsync(classeId, cancellationToken).ConfigureAwait(false);
        var resumo = lista.Select(HabilidadeMapper.ParaResumo).ToArray();
        return ResultadoOperacao<IReadOnlyCollection<HabilidadeResumoDto>>.Ok(resumo);
    }
}
