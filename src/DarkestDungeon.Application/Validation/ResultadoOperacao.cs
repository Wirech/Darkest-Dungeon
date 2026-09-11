namespace DarkestDungeon.Application.Validation;

public enum StatusOperacao
{
    Sucesso,
    EntradaInvalida,
    NaoEncontrado,
    FalhaPersistencia
}

public sealed record ErroOperacao(string Campo, string Mensagem);

public sealed class ResultadoOperacao<T>
{
    private ResultadoOperacao(StatusOperacao status, T? valor, string mensagem, IReadOnlyCollection<ErroOperacao> erros)
    {
        Status = status;
        Valor = valor;
        Mensagem = mensagem;
        Erros = erros;
    }

    public StatusOperacao Status { get; }
    public T? Valor { get; }
    public string Mensagem { get; }
    public IReadOnlyCollection<ErroOperacao> Erros { get; }
    public bool Sucesso => Status == StatusOperacao.Sucesso;

    public static ResultadoOperacao<T> Ok(T valor) => new(StatusOperacao.Sucesso, valor, string.Empty, Array.Empty<ErroOperacao>());

    public static ResultadoOperacao<T> Invalido(string mensagem, params ErroOperacao[] erros) => new(StatusOperacao.EntradaInvalida, default, mensagem, erros);

    public static ResultadoOperacao<T> NaoEncontrado(string mensagem) => new(StatusOperacao.NaoEncontrado, default, mensagem, Array.Empty<ErroOperacao>());

    public static ResultadoOperacao<T> FalhaPersistencia(string mensagem) => new(StatusOperacao.FalhaPersistencia, default, mensagem, Array.Empty<ErroOperacao>());

    public static ResultadoOperacao<T> CatalogoIncompleto(string mensagem, params ErroOperacao[] erros) =>
        new(
            StatusOperacao.EntradaInvalida,
            default,
            string.IsNullOrWhiteSpace(mensagem)
                ? "O catálogo oficial está incompleto para esta classe."
                : mensagem,
            erros.Length == 0
                ? new[] { new ErroOperacao("catalogo", "Faltam tabela oficial de arma, armadura ou deslocamento.") }
                : erros);

    public static ResultadoOperacao<T> TabelaOficialAusente(string mensagem) =>
        new(
            StatusOperacao.NaoEncontrado,
            default,
            string.IsNullOrWhiteSpace(mensagem)
                ? "Tabela oficial de equipamento não encontrada."
                : mensagem,
            new[] { new ErroOperacao("catalogo", "Arma ou armadura oficial da classe não está catalogada.") });
}