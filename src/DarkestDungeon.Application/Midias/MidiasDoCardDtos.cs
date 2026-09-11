namespace DarkestDungeon.Application.Midias;

/// Slot visual do card. `Status` OK exige `Url` e `ArquivoInventarioId`; Pendente exige `Url` nula.
public sealed record SlotDeMidiaDoCardDto(
    string Tipo,
    string Rotulo,
    string Status,
    string? ArquivoInventarioId,
    string? HashArquivo,
    string? Url,
    int? Nivel = null,
    Guid? HabilidadeId = null,
    ConjuntoDeCorpoDto? ConjuntoIdle = null,
    ConjuntoDeCorpoDto? ConjuntoWalk = null,
    IReadOnlyList<VersaoDoCorpoDto>? Versoes = null,
    IReadOnlyList<ConjuntoDeCorpoDto>? Conjuntos = null);

public sealed record ConjuntoDeCorpoDto(
    string Ciclo,
    string UrlAtlas,
    string UrlEsqueleto,
    string UrlTextura);

public sealed record VersaoDoCorpoDto(
    string Id,
    string Rotulo,
    bool Disponivel);

public sealed record MidiasDoPersonagemDto(
    SlotDeMidiaDoCardDto Retrato,
    SlotDeMidiaDoCardDto CorpoInteiro,
    SlotDeMidiaDoCardDto Arma,
    SlotDeMidiaDoCardDto Armadura);

public static class SlotDeMidiaDoCard
{
    public const string StatusOk = "OK";
    public const string StatusPendente = "Pendente";
    public const string TipoRetrato = "retrato";
    public const string TipoCorpoInteiro = "corpoInteiro";
    public const string TipoArma = "arma";
    public const string TipoArmadura = "armadura";
    public const string TipoHabilidade = "habilidade";
    public const string RotuloRetrato = "Retrato";
    public const string RotuloCorpoInteiro = "Corpo inteiro";
    public const string RotuloArma = "Arma";
    public const string RotuloArmadura = "Armadura";
    public const string IdEmEspera = "emEspera";
    public const string IdAnimado = "animado";
    public const string IdCaminhada = "caminhada";
    public const string RotuloEmEspera = "Em espera";
    public const string RotuloAnimado = "Animado";
    public const string RotuloCaminhada = "Caminhada";
    public const string CicloIdle = "idle";
    public const string CicloWalk = "walk";

    public static string RotuloDoCiclo(string ciclo)
    {
        var chave = ciclo.Trim().ToLowerInvariant();
        return chave switch
        {
            CicloIdle => RotuloAnimado,
            CicloWalk => RotuloCaminhada,
            "afflicted" => "Aflito",
            "camp" => "Acampamento",
            "combat" => "Combate",
            "defend" => "Defesa",
            "heroic" => "Heróico",
            "investigate" => "Investigar",
            "dead" => "Morto",
            _ when chave.StartsWith("attack_", StringComparison.Ordinal)
                => "Ataque · " + Capitalizar(chave["attack_".Length..].Replace('_', ' ')),
            _ => Capitalizar(chave.Replace('_', ' ')),
        };
    }

    public static int OrdemDoCiclo(string ciclo)
    {
        var chave = ciclo.Trim().ToLowerInvariant();
        return chave switch
        {
            "combat" => 0,
            "defend" => 1,
            "heroic" => 2,
            "afflicted" => 3,
            "investigate" => 4,
            "camp" => 5,
            "dead" => 6,
            _ when chave.StartsWith("attack_", StringComparison.Ordinal) => 10,
            _ => 20,
        };
    }

    private static string Capitalizar(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return texto;
        }

        return string.Join(' ', texto.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(parte => char.ToUpperInvariant(parte[0]) + parte[1..]));
    }

    public static SlotDeMidiaDoCardDto Ok(
        string tipo,
        string rotulo,
        string arquivoInventarioId,
        string? hashArquivo,
        string url,
        int? nivel = null,
        Guid? habilidadeId = null,
        ConjuntoDeCorpoDto? conjuntoIdle = null,
        ConjuntoDeCorpoDto? conjuntoWalk = null,
        IReadOnlyList<VersaoDoCorpoDto>? versoes = null,
        IReadOnlyList<ConjuntoDeCorpoDto>? conjuntos = null)
    {
        if (string.IsNullOrWhiteSpace(arquivoInventarioId))
        {
            throw new ArgumentException("Status OK exige arquivo de inventário.", nameof(arquivoInventarioId));
        }

        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("Status OK exige URL relativa.", nameof(url));
        }

        return new SlotDeMidiaDoCardDto(
            tipo,
            rotulo,
            StatusOk,
            arquivoInventarioId,
            hashArquivo,
            url,
            nivel,
            habilidadeId,
            conjuntoIdle,
            conjuntoWalk,
            versoes,
            conjuntos);
    }

    public static SlotDeMidiaDoCardDto Pendente(
        string tipo,
        string rotulo,
        int? nivel = null,
        Guid? habilidadeId = null,
        ConjuntoDeCorpoDto? conjuntoWalk = null,
        IReadOnlyList<VersaoDoCorpoDto>? versoes = null,
        IReadOnlyList<ConjuntoDeCorpoDto>? conjuntos = null) =>
        new(tipo, rotulo, StatusPendente, null, null, null, nivel, habilidadeId, null, conjuntoWalk, versoes, conjuntos);
}
