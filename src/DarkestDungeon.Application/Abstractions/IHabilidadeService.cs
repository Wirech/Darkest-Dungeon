using DarkestDungeon.Application.Habilidades;
using DarkestDungeon.Application.Habilidades.Commands;
using DarkestDungeon.Application.Validation;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Application.Abstractions;

public interface IHabilidadeRepository
{
    Task<Habilidade?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Habilidade?> ObterPorNomeExibicaoAsync(string nomeExibicao, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Habilidade>> ListarAsync(CategoriaDeHabilidadeDto? categoria, Guid? classeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Habilidade>> ListarPorClasseAsync(Guid classeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ClasseHabilidade>> ListarAssociacoesPorHabilidadeAsync(Guid habilidadeId, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Habilidade habilidade, IReadOnlyCollection<ClasseHabilidade> associacoes, CancellationToken cancellationToken = default);
}

public interface IHabilidadeService
{
    Task<ResultadoOperacao<HabilidadeDetalheDto>> ObterAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ResultadoOperacao<IReadOnlyCollection<HabilidadeResumoDto>>> ListarAsync(CategoriaDeHabilidadeDto? categoria, Guid? classeId, CancellationToken cancellationToken = default);
    Task<ResultadoOperacao<HabilidadeDetalheDto>> CriarDeCombateAsync(CriarHabilidadeDeCombateCommand command, CancellationToken cancellationToken = default);
    Task<ResultadoOperacao<HabilidadeDetalheDto>> CriarDeAcampamentoAsync(CriarHabilidadeDeAcampamentoCommand command, CancellationToken cancellationToken = default);
    Task<ResultadoOperacao<HabilidadeDetalheDto>> CriarDeInimigoAsync(CriarHabilidadeDeInimigoCommand command, CancellationToken cancellationToken = default);
}
