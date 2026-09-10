namespace DarkestDungeon.Domain.Personagens;

/// Value object imutável representando o Resolve Level do Personagem (0..6).
/// `Nome` PT-BR canônico usado em API/mensagens; `NomeOriginal` inglês para rastreabilidade com a wiki.
public sealed record NivelDeResolucao
{
    public const int ValorMaximo = 6;

    private static readonly IReadOnlyDictionary<int, (string Nome, string NomeOriginal)> NomesPorValor =
        new Dictionary<int, (string, string)>
        {
            [0] = ("Curioso", "Seeker"),
            [1] = ("Aprendiz", "Apprentice"),
            [2] = ("Aventureiro", "Adventurer"),
            [3] = ("Veterano", "Veteran"),
            [4] = ("Mestre", "Master"),
            [5] = ("Campeão", "Champion"),
            [6] = ("Lenda", "Legend"),
        };

    private NivelDeResolucao()
    {
        Nome = string.Empty;
        NomeOriginal = string.Empty;
    }

    public NivelDeResolucao(int valor, string nome, string nomeOriginal, int bonusResistenciaPercentual)
    {
        Valor = valor;
        Nome = nome;
        NomeOriginal = nomeOriginal;
        BonusResistenciaPercentual = bonusResistenciaPercentual;
    }

    public int Valor { get; private init; }
    public string Nome { get; private init; }
    public string NomeOriginal { get; private init; }
    public int BonusResistenciaPercentual { get; private init; }

    /// Cria uma instância nomeada a partir do valor 0..6.
    public static NivelDeResolucao De(int valor)
    {
        if (!NomesPorValor.TryGetValue(valor, out var nomes))
        {
            throw new ArgumentOutOfRangeException(nameof(valor), $"Nível de Resolução deve estar entre 0 e {ValorMaximo}.");
        }

        return new NivelDeResolucao(valor, nomes.Nome, nomes.NomeOriginal, valor * 10);
    }
}
