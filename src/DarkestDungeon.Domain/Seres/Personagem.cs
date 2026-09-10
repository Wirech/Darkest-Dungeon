using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Personagens;

namespace DarkestDungeon.Domain.Seres;

/// Herói jogável — especialização de `Ser` (feature 001).
/// Herda as 5 resistências de `Ser`; carrega `ResistenciasExtras` próprias (FR-015).
/// Individualidades e Doenças estão fora do escopo desta feature (Q1).
public sealed class Personagem : Ser
{
    private readonly List<HabilidadeDePersonagem> habilidades = new();

    /// Limite máximo de habilidades de Combate por Personagem (FR-009 da Feature 003 — mantido para retrocompat).
    /// Feature 005 (Q2): combate não tem limite operacional — qualquer habilidade `NumeroDoNivel >= 1` é usável.
    public const int LimiteHabilidadesDeCombate = 6;

    /// Limite máximo de habilidades de Acampamento por Personagem (FR-009 da Feature 003 — mantido para retrocompat).
    /// Feature 005 (Q2): acampamento usa `LimiteEquipadasAcampamento` = 3 para o número de habilidades marcadas `Equipada`.
    public const int LimiteHabilidadesDeAcampamento = 6;

    /// Feature 005 (FR-007c): no máximo 3 habilidades de acampamento podem estar `Equipada=true` simultaneamente por Personagem.
    public const int LimiteEquipadasAcampamento = 3;

    private Personagem()
    {
        ResistenciasExtras = new ResistenciasExtrasDePersonagem(0, 0, 0);
        Inventario = new Inventario();
        Aparencia = AparenciaDePersonagem.A;
        Experiencia = 0;
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
        Aparencia = AparenciaDePersonagem.A;
        Experiencia = 0;
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

    /// Feature 005 (FR-007h): variante visual do herói (A/B/C/D). Padrão `A` na criação.
    public AparenciaDePersonagem Aparencia { get; private set; }

    /// Feature 005 (FR-007k): XP acumulado, distinto de `Nivel`. `Nivel` é derivado desta via `TabelaDeExperiencia.Resolver`.
    public int Experiencia { get; private set; }

    /// Feature 005 (FR-007l): retorna o `NivelDeResolucao` atual (Curioso..Lenda) baseado em `Experiencia`.
    public NivelDeResolucao NivelDeResolucao => TabelaDeExperiencia.Resolver(Experiencia);

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

    /// Feature 005 (FR-007h + SC-013): define a variante visual do Personagem.
    public void DefinirAparencia(AparenciaDePersonagem aparencia)
    {
        if (!Enum.IsDefined(aparencia))
        {
            throw new ArgumentOutOfRangeException(nameof(aparencia), "Aparência inválida. Valores aceitos: A, B, C, D.");
        }

        Aparencia = aparencia;
    }

    /// Feature 005 (FR-007k/l): soma XP; se cruzar limiar, sobe Nivel (base) e aplica +10% acumulado nas 5 resistências + trap disarm.
    public void GanharExperiencia(int quantidade)
    {
        if (quantidade < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantidade), "Experiência ganha deve ser >= 0.");
        }

        var nivelAnterior = NivelDeResolucao.Valor;
        Experiencia += quantidade;
        var nivelNovo = NivelDeResolucao.Valor;
        if (nivelNovo > nivelAnterior)
        {
            var deltaResistencia = (nivelNovo - nivelAnterior) * 10m;
            AplicarBonusPorNivel(deltaResistencia);
            base.AtualizarNivel(nivelNovo);
        }
    }

    /// Feature 005 (FR-007l + SC-015): +10 pp por nível ganho nas 5 resistências e na chance de desarmar Armadilha (via ResistenciasExtras.Armadilha).
    private void AplicarBonusPorNivel(decimal deltaPercentual)
    {
        var resistenciasNovas = new Resistencias(
            atordoamento: Math.Min(200m, Resistencias.Atordoamento + deltaPercentual),
            sangramento: Math.Min(200m, Resistencias.Sangramento + deltaPercentual),
            envenenamento: Math.Min(200m, Resistencias.Envenenamento + deltaPercentual),
            debuff: Math.Min(200m, Resistencias.Debuff + deltaPercentual),
            movimento: Math.Min(200m, Resistencias.Movimento + deltaPercentual));
        base.AtualizarResistencias(resistenciasNovas);

        var extrasNovas = new ResistenciasExtrasDePersonagem(
            doenca: ResistenciasExtras.Doenca,
            golpeMortal: ResistenciasExtras.GolpeMortal,
            armadilha: Math.Min(200m, ResistenciasExtras.Armadilha + deltaPercentual));
        ResistenciasExtras = extrasNovas;
    }

    /// Feature 005 (FR-007c): equipa uma habilidade de acampamento (deve estar treinada; limite 3 simultâneas por Personagem).
    public void EquiparHabilidadeAcampamento(Guid habilidadeDePersonagemId, Func<Guid, bool> ehHabilidadeDeAcampamento)
    {
        ArgumentNullException.ThrowIfNull(ehHabilidadeDeAcampamento);
        var alvo = habilidades.FirstOrDefault(h => h.HabilidadeId == habilidadeDePersonagemId)
            ?? throw new InvalidOperationException($"Habilidade '{habilidadeDePersonagemId}' não pertence a este Personagem.");

        if (!ehHabilidadeDeAcampamento(alvo.HabilidadeId))
        {
            throw new InvalidOperationException("Apenas habilidades de acampamento podem ser equipadas para expedição.");
        }

        if (!alvo.TreinadaPorNivel && !alvo.Treinada)
        {
            throw new InvalidOperationException($"A habilidade '{alvo.HabilidadeId}' ainda não foi treinada (Nível 0). Treine-a antes de equipá-la.");
        }

        var equipadasAgora = habilidades.Count(h => h.Equipada && ehHabilidadeDeAcampamento(h.HabilidadeId));
        if (!alvo.Equipada && equipadasAgora >= LimiteEquipadasAcampamento)
        {
            throw new InvalidOperationException("Apenas 3 habilidades de acampamento podem estar equipadas simultaneamente. Desequipe uma antes.");
        }

        alvo.Equipar();
    }

    /// Feature 005 (FR-007c): desequipa uma habilidade de acampamento.
    public void DesequiparHabilidadeAcampamento(Guid habilidadeDePersonagemId)
    {
        var alvo = habilidades.FirstOrDefault(h => h.HabilidadeId == habilidadeDePersonagemId)
            ?? throw new InvalidOperationException($"Habilidade '{habilidadeDePersonagemId}' não pertence a este Personagem.");
        alvo.Desequipar();
    }
}
