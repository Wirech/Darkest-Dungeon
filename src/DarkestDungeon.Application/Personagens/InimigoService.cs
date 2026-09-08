using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Application.Personagens.Commands;
using DarkestDungeon.Application.Validation;
using DarkestDungeon.Domain.Seres;

namespace DarkestDungeon.Application.Personagens;

public sealed class InimigoService : IInimigoService
{
    private readonly IInimigoRepository inimigos;

    public InimigoService(IInimigoRepository inimigos)
    {
        this.inimigos = inimigos;
    }

    public async Task<ResultadoOperacao<InimigoDetalheDto>> ObterAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var inimigo = await inimigos.ObterPorIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (inimigo is null)
        {
            return ResultadoOperacao<InimigoDetalheDto>.NaoEncontrado($"Inimigo '{id}' não encontrado.");
        }

        return ResultadoOperacao<InimigoDetalheDto>.Ok(PersonagemMapper.ParaDto(inimigo));
    }

    public async Task<ResultadoOperacao<InimigoDetalheDto>> CriarAsync(CriarInimigoCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var resistencias = new Resistencias(
                command.Atordoamento,
                command.Sangramento,
                command.Envenenamento,
                command.Debuff,
                command.MovimentoResistencia);

            var inimigo = new Inimigo(
                command.Nome,
                command.Tipo,
                command.HpMaximo,
                command.HpAtual,
                command.Velocidade,
                command.Critico,
                command.DanoBaseMinimo,
                command.DanoBaseMaximo,
                command.Movimento,
                command.BonusDeCritico,
                command.Tamanho,
                command.AcoesPorTurno,
                command.Esquiva,
                command.Precisao,
                command.Protecao,
                command.Nivel,
                resistencias,
                command.HabilidadesIds);

            await inimigos.AdicionarAsync(inimigo, cancellationToken).ConfigureAwait(false);
            return ResultadoOperacao<InimigoDetalheDto>.Ok(PersonagemMapper.ParaDto(inimigo));
        }
        catch (ArgumentException ex)
        {
            return ResultadoOperacao<InimigoDetalheDto>.Invalido(ex.Message, new ErroOperacao(ex.ParamName ?? "requisicao", ex.Message));
        }
    }
}
