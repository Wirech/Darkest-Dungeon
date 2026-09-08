using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Application.Validation;
using DarkestDungeon.Domain.Seres;

namespace DarkestDungeon.Application.Seres;

public sealed class SerService : ISerService, IIdentificavelService<SerDto>
{
    private readonly ISerRepository repository;

    public SerService(ISerRepository repository)
    {
        this.repository = repository;
    }

    public async Task<ResultadoOperacao<SerDto>> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return ResultadoOperacao<SerDto>.Invalido(
                "Identificador inválido.",
                new ErroOperacao("id", "Identificador inválido."));
        }

        var ser = await repository.ObterPorIdAsync(id, cancellationToken);
        return ser is null
            ? ResultadoOperacao<SerDto>.NaoEncontrado("Ser não encontrado.")
            : ResultadoOperacao<SerDto>.Ok(Mapear(ser));
    }

    public async Task<ResultadoOperacao<SerDto>> CriarAsync(CriarSerCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var resistencias = new Resistencias(
                command.Resistencias.Atordoamento,
                command.Resistencias.Sangramento,
                command.Resistencias.Envenenamento,
                command.Resistencias.Debuff,
                command.Resistencias.Movimento);

            var ser = new Ser(
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
                resistencias);

            await repository.AdicionarAsync(ser, cancellationToken);
            await repository.SalvarAlteracoesAsync(cancellationToken);

            return ResultadoOperacao<SerDto>.Ok(Mapear(ser));
        }
        catch (ArgumentException ex)
        {
            var campo = string.IsNullOrWhiteSpace(ex.ParamName) ? "ser" : CamelCase(ex.ParamName);
            var mensagem = LimparMensagem(ex.Message);
            return ResultadoOperacao<SerDto>.Invalido(mensagem, new ErroOperacao(campo, mensagem));
        }
        catch (Exception)
        {
            return ResultadoOperacao<SerDto>.FalhaPersistencia("Não foi possível persistir o Ser.");
        }
    }

    private static SerDto Mapear(Ser ser) => new(
        ser.Id,
        ser.Nome,
        ser.Tipo,
        ser.HpMaximo,
        ser.HpAtual,
        ser.Velocidade,
        ser.Critico,
        ser.DanoBaseMinimo,
        ser.DanoBaseMaximo,
        ser.Movimento,
        ser.BonusDeCritico,
        ser.Tamanho,
        ser.AcoesPorTurno,
        ser.Esquiva,
        ser.Precisao,
        ser.Protecao,
        ser.Nivel,
        new ResistenciasDto(
            ser.Resistencias.Atordoamento,
            ser.Resistencias.Sangramento,
            ser.Resistencias.Envenenamento,
            ser.Resistencias.Debuff,
            ser.Resistencias.Movimento));

    private static string CamelCase(string valor) => char.ToLowerInvariant(valor[0]) + valor[1..];

    private static string LimparMensagem(string mensagem)
    {
        var indiceParametro = mensagem.IndexOf(" (Parameter", StringComparison.Ordinal);
        return indiceParametro < 0 ? mensagem : mensagem[..indiceParametro];
    }
}