namespace DarkestDungeon.Api.Contracts;

public sealed record ErroResponse(string Mensagem, IReadOnlyCollection<ErroCampoResponse>? Erros = null);

public sealed record ErroCampoResponse(string Campo, string Mensagem);