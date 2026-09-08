using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Personagens;

namespace DarkestDungeon.Domain.Seres;

/// Herói jogável — especialização de `Ser` (feature 001).
/// Herda as 5 resistências de `Ser`; carrega `ResistenciasExtras` próprias (FR-015).
/// Individualidades e Doenças estão fora do escopo desta feature (Q1).
public sealed class Personagem : Ser
{
    private readonly List<HabilidadeDePersonagem> habilidades = new();

    /// Limite máximo de habilidades de Combate por Personagem (FR-009).
    public const int LimiteHabilidadesDeCombate = 6;

    /// Limite máximo de habilidades de Acampamento por Personagem (FR-009).
    public const int LimiteHabilidadesDeAcampamento = 6;

    private Personagem()
    {
        ResistenciasExtras = new ResistenciasExtrasDePersonagem(0, 0, 0);
        Inventario = new Inventario();
    }

    public Personagem(
        string nome,
        ClasseDeHeroi classe,
        int hpMaximo,
        int hpAtual,
        int velocidade,
        decimal critico,
        int danoBaseMinimo,
        int danoBaseMaximo,
        int movimento,
        decimal bonusDeCritico,
        int tamanho,
        int acoesPorTurno,
        decimal esquiva,
        decimal precisao,
        decimal protecao,
        int nivel,
        Resistencias resistencias,
        ResistenciasExtrasDePersonagem resistenciasExtras,
        int stress,
        int chanceDeVirtude,
        Guid? id = null)
        : base(nome, "Personagem", hpMaximo, hpAtual, velocidade, critico, danoBaseMinimo, danoBaseMaximo, movimento, bonusDeCritico, tamanho, acoesPorTurno, esquiva, precisao, protecao, nivel, resistencias, id)
    {
        if (stress is < 0 or > 200)
        {
            throw new ArgumentOutOfRangeException(nameof(stress), "Stress deve estar entre 0 e 200.");
        }

        if (chanceDeVirtude is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(chanceDeVirtude), "Chance de virtude deve estar entre 0 e 100.");
        }

        Classe = classe;
        ResistenciasExtras = resistenciasExtras ?? throw new ArgumentNullException(nameof(resistenciasExtras));
        Stress = stress;
        ChanceDeVirtude = chanceDeVirtude;
        Inventario = new Inventario();
    }

    public ClasseDeHeroi Classe { get; private set; }
    public ResistenciasExtrasDePersonagem ResistenciasExtras { get; private set; }
    public int Stress { get; private set; }
    public int ChanceDeVirtude { get; private set; }
    public string? Aflicao { get; private set; }
    public string? Virtude { get; private set; }
    public bool EstadoPortasDaMorte { get; private set; }
    public bool RecuperouPortasDaMorte { get; private set; }
    public bool RecuperouAtaqueCardiaco { get; private set; }
    public Guid? ArmaEquipadaId { get; private set; }
    public Guid? ArmaduraEquipadaId { get; private set; }
    public Guid? AcessorioEquipado1Id { get; private set; }
    public Guid? AcessorioEquipado2Id { get; private set; }
    public IReadOnlyList<HabilidadeDePersonagem> Habilidades => habilidades;
    public Inventario Inventario { get; private set; }

    public void AdicionarHabilidade(HabilidadeDePersonagem habilidade)
    {
        ArgumentNullException.ThrowIfNull(habilidade);
        if (habilidades.Any(item => item.HabilidadeId == habilidade.HabilidadeId))
        {
            throw new ArgumentException("Habilidade já foi atribuída a este Personagem.", nameof(habilidade));
        }

        habilidades.Add(habilidade);
    }

    public void DefinirAflicao(string? aflicao)
    {
        if (aflicao is not null && string.IsNullOrWhiteSpace(aflicao))
        {
            throw new ArgumentException("Aflição deve conter texto ou ser nula.", nameof(aflicao));
        }

        if (aflicao is not null && Virtude is not null)
        {
            throw new InvalidOperationException("Aflição e Virtude não podem coexistir.");
        }

        Aflicao = aflicao?.Trim();
    }

    public void DefinirVirtude(string? virtude)
    {
        if (virtude is not null && string.IsNullOrWhiteSpace(virtude))
        {
            throw new ArgumentException("Virtude deve conter texto ou ser nula.", nameof(virtude));
        }

        if (virtude is not null && Aflicao is not null)
        {
            throw new InvalidOperationException("Aflição e Virtude não podem coexistir.");
        }

        Virtude = virtude?.Trim();
    }

    public void EquiparArma(Guid? armaId) => ArmaEquipadaId = armaId;
    public void EquiparArmadura(Guid? armaduraId) => ArmaduraEquipadaId = armaduraId;

    public void EquiparAcessorios(Guid? acessorio1Id, Guid? acessorio2Id)
    {
        if (acessorio1Id is not null && acessorio1Id == acessorio2Id)
        {
            throw new ArgumentException("Não é possível equipar o mesmo Acessório em dois slots.");
        }

        AcessorioEquipado1Id = acessorio1Id;
        AcessorioEquipado2Id = acessorio2Id;
    }
}
