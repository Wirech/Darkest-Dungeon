using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

/// Seed das habilidades oficiais mineradas em darkestdungeon.wiki.gg (2026-09-07).
///
/// Convenção de leitura da wiki para posições:
/// - Rank (posições do herói): dots esquerda→direita = rank 4,3,2,1 (backline→frontline).
/// - Target (posições atingidas): dots esquerda→direita = target 1,2,3,4 (frontline→backline).
public static partial class HabilidadesSeed
{
    private static readonly IReadOnlyDictionary<ClasseDeHeroi, string[]> NomesPorClasse = new Dictionary<ClasseDeHeroi, string[]>
    {
        [ClasseDeHeroi.Cruzado] = new[]
        {
            "Smite", "Zealous Accusation", "Stunning Blow", "Bulwark of Faith", "Battle Heal", "Holy Lance", "Inspiring Cry",
            "Unshakable Leader", "Stand Tall", "Zealous Speech", "Zealous Vigil",
        },
        [ClasseDeHeroi.Vestal] = new[]
        {
            "Mace Bash", "Judgement", "Dazzling Light", "Divine Grace", "Divine Comfort", "Illumination", "Hand of Light",
            "Bless", "Chant", "Pray", "Sanctuary",
        },
        [ClasseDeHeroi.Ocultista] = new[]
        {
            "Sacrificial Stab", "Abyssal Artillery", "Weakening Curse", "Wyrd Reconstruction", "Vulnerability Hex", "Hands from the Abyss", "Daemon's Pull",
            "Abandon Hope", "Dark Ritual", "Dark Strength", "Unspeakable Commune",
        },
        [ClasseDeHeroi.MedicoDaPeste] = new[]
        {
            "Noxious Blast", "Plague Grenade", "Blinding Gas", "Incision", "Battlefield Medicine", "Emboldening Vapours", "Disorienting Blast",
            "Experimental Vapours", "Leeches", "The Cure", "Self-Medicate",
        },
        [ClasseDeHeroi.Besteiro] = new[]
        {
            "Sniper Shot", "Suppressing Fire", "Sniper's Mark", "Bola", "Blindfire", "Battlefield Bandage", "Rallying Flare",
            "Field Dressing", "Marching Plan", "Restring Crossbow", "Triage",
        },
        [ClasseDeHeroi.Musqueteiro] = new[]
        {
            "Aimed Shot", "Smokescreen", "Call the Shot", "Buckshot", "Sidearm", "Patch Up", "Skeet Shot",
            "Field Dressing", "Marching Plan", "Clean Musket", "Triage",
        },
        [ClasseDeHeroi.Veterano] = new[]
        {
            "Crush", "Rampart", "Bellow", "Defender", "Retribution", "Command", "Bolster",
            "Maintain Equipment", "Tactics", "Instruction", "Weapons Practice",
        },
        [ClasseDeHeroi.Bandido] = new[]
        {
            "Wicked Slice", "Pistol Shot", "Point Blank Shot", "Grapeshot Blast", "Tracking Shot", "Duelist's Advance", "Open Vein",
            "Gallows Humor", "Unparalleled Finesse", "Clean Guns", "Bandit's Sense",
        },
        [ClasseDeHeroi.LadraoDeCova] = new[]
        {
            "Pick to the Face", "Lunge", "Flashing Daggers", "Shadow Fade", "Thrown Dagger", "Poison Dart", "Toxin Trickery",
            "Snuff Box", "Gallows Humor", "Night Moves", "Pilfer",
        },
        [ClasseDeHeroi.BoboDaCorte] = new[]
        {
            "Dirk Stab", "Harvest", "Finale", "Solo", "Slice Off", "Battle Ballad", "Inspiring Tune",
            "Turn Back Time", "Every Rose Has Its Thorn", "Tiger's Eye", "Mockery",
        },
        [ClasseDeHeroi.Leproso] = new[]
        {
            "Chop", "Hew", "Purge", "Revenge", "Withstand", "Solemnity", "Intimidate",
            "Let the Mask Down", "Bloody Shroud", "Reflection", "Quarantine",
        },
        [ClasseDeHeroi.Infernal] = new[]
        {
            "Wicked Hack", "Iron Swan", "Barbaric Yawp", "If It Bleeds", "Breakthrough", "Adrenaline Rush", "Bleed Out",
            "Battle Trance", "Revel", "Reject the Gods", "Sharpen Spear",
        },
        [ClasseDeHeroi.Abominacao] = new[]
        {
            "Transform", "Manacles", "Beast's Bile", "Absolution", "Rake", "Rage", "Slam",
            "Anger Management", "Psych Up", "The Quickening", "Eldritch Blood",
        },
        [ClasseDeHeroi.Antiquario] = new[]
        {
            "Nervous Stab", "Festering Vapours", "Get Down!", "Flashpowder", "Fortifying Vapours", "Invigorating Vapours", "Protect Me",
            "Resupply", "Trinket Scrounge", "Strange Powders", "Curious Incantation",
        },
        [ClasseDeHeroi.CacadorDeRecompensas] = new[]
        {
            "Collect Bounty", "Mark for Death", "Come Hither", "Uppercut", "Flashbang", "Finish Him", "Caltrops",
            "This Is How We Do It", "Tracking", "Planned Takedown", "Scout Ahead",
        },
        [ClasseDeHeroi.MestreDeCaca] = new[]
        {
            "Hound's Rush", "Hound's Harry", "Target Whistle", "Cry Havoc", "Guard Dog", "Lick Wounds", "Blackjack",
            "Hound's Watch", "Therapy Dog", "Man's Best Friend", "Release the Hound",
        },
        [ClasseDeHeroi.Flagelante] = new[]
        {
            "Punish", "Rain of Sorrows", "Exsanguinate", "Reclaim", "Redeem", "Endure", "Suffer",
            "Lash's Anger", "Lash's Solace", "Lash's Kiss", "Lash's Cure",
        },
        [ClasseDeHeroi.Rompedor] = new[]
        {
            "Pierce", "Puncture", "Adder's Kiss", "Impale", "Expose", "Captivate", "Serpent Sway",
            "Snake Eyes", "Snake Skin", "Sandstorm", "Adder's Embrace",
        },
        [ClasseDeHeroi.Duelista] = new[]
        {
            "Anticipation", "Touché", "Feint", "Disengage", "Flèche", "Coup de Grâce", "The Boot",
            "Meditation", "Preparation", "Ruthless Instruction", "Again!",
        },
        [ClasseDeHeroi.Fugitivo] = new[]
        {
            "Searing Strike", "Firefly", "Run and Hide", "Ransack", "Hearthlight", "Controlled Burn", "Backdraft",
            "Kindle", "Cauterize", "Play with Fire", "Pick Pocket",
        },
    };

    private static readonly string[] AcampamentoCompartilhadasNomes = { "Encourage", "Wound Care", "Pep Talk" };

    public static IReadOnlyDictionary<ClasseDeHeroi, IReadOnlyList<string>> ObterNomesPorClasse()
        => NomesPorClasse.ToDictionary(
            item => item.Key,
            item => (IReadOnlyList<string>)item.Value.Concat(AcampamentoCompartilhadasNomes).ToArray());

    public static IReadOnlyList<Habilidade> Materializar()
    {
        var lista = new List<Habilidade>();
        lista.AddRange(CruzadoCombate());
        lista.AddRange(CruzadoAcampamento());
        lista.AddRange(VestalCombate());
        lista.AddRange(VestalAcampamento());
        lista.AddRange(OcultistaCombate());
        lista.AddRange(OcultistaAcampamento());
        lista.AddRange(MedicoDaPesteCombate());
        lista.AddRange(MedicoDaPesteAcampamento());
        lista.AddRange(BesteiroCombate());
        lista.AddRange(BesteiroAcampamento());
        lista.AddRange(MusqueteiroCombate());
        lista.AddRange(MusqueteiroAcampamento());
        lista.AddRange(VeteranoCombate());
        lista.AddRange(VeteranoAcampamento());
        lista.AddRange(BandidoCombate());
        lista.AddRange(BandidoAcampamento());
        lista.AddRange(LadraoDeCovaCombate());
        lista.AddRange(LadraoDeCovaAcampamento());
        lista.AddRange(BoboDaCorteCombate());
        lista.AddRange(BoboDaCorteAcampamento());
        lista.AddRange(LeprosoCombate());
        lista.AddRange(LeprosoAcampamento());
        lista.AddRange(InfernalCombate());
        lista.AddRange(InfernalAcampamento());
        lista.AddRange(AbominacaoCombate());
        lista.AddRange(AbominacaoAcampamento());
        lista.AddRange(AntiquarioCombate());
        lista.AddRange(AntiquarioAcampamento());
        lista.AddRange(CacadorDeRecompensasCombate());
        lista.AddRange(CacadorDeRecompensasAcampamento());
        lista.AddRange(MestreDeCacaCombate());
        lista.AddRange(MestreDeCacaAcampamento());
        lista.AddRange(FlagelanteCombate());
        lista.AddRange(FlagelanteAcampamento());
        lista.AddRange(RompedorCombate());
        lista.AddRange(RompedorAcampamento());
        lista.AddRange(DuelistaCombate());
        lista.AddRange(DuelistaAcampamento());
        lista.AddRange(FugitivoCombate());
        lista.AddRange(FugitivoAcampamento());
        lista.AddRange(AcampamentoCompartilhadas());
        return lista;
    }

    public static IEnumerable<ClasseHabilidade> MaterializarAssociacoes(IReadOnlyDictionary<ClasseDeHeroi, Guid> idsDeClasse)
    {
        ArgumentNullException.ThrowIfNull(idsDeClasse);

        foreach (var (classe, nomes) in NomesPorClasse)
        {
            var idClasse = idsDeClasse[classe];
            foreach (var nome in nomes)
            {
                yield return new ClasseHabilidade(idClasse, IdDeterministico(nome));
            }
        }

        // Compartilhadas: aplicadas a todas as classes exceto o Flagelante.
        foreach (var classe in Enum.GetValues<ClasseDeHeroi>())
        {
            if (classe == ClasseDeHeroi.Flagelante)
            {
                continue;
            }

            var idClasse = idsDeClasse[classe];
            foreach (var nome in AcampamentoCompartilhadasNomes)
            {
                yield return new ClasseHabilidade(idClasse, IdDeterministico(nome));
            }
        }
    }

    private static Guid IdDeterministico(string nomeOriginal)
    {
        var bytes = System.Security.Cryptography.MD5.HashData(
            System.Text.Encoding.UTF8.GetBytes($"Habilidade:{nomeOriginal}"));
        bytes[6] = (byte)((bytes[6] & 0x0F) | 0x40);
        bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80);
        return new Guid(bytes);
    }

    // Helper para reduzir repetição em `new EfeitoDeHabilidade(...)` no restante das partials.
    private static EfeitoDeHabilidade Efeito(string nome, AlvoDeEfeito alvo, decimal valor, UnidadeDeEfeito unidade, decimal chance = 100m, int? duracao = null)
        => new(nome, alvo, valor, unidade, chance, duracao);
}
