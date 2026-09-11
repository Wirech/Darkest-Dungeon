using System.ComponentModel;
using System.Reflection;
using DarkestDungeon.Domain.Classes;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

/// Seed inicial das 20 classes oficiais de heróis com as 8 resistências base
/// mineradas em darkestdungeon.wiki.gg (2026-09-07).
public static class ClassesSeed
{
    private static readonly IReadOnlyDictionary<ClasseDeHeroi, string> NomesExibicao = new Dictionary<ClasseDeHeroi, string>
    {
        [ClasseDeHeroi.Abominacao] = "Abominação",
        [ClasseDeHeroi.Antiquario] = "Antiquário",
        [ClasseDeHeroi.Besteiro] = "Besteiro",
        [ClasseDeHeroi.CacadorDeRecompensas] = "Caçador de Recompensas",
        [ClasseDeHeroi.Cruzado] = "Cruzado",
        [ClasseDeHeroi.LadraoDeCova] = "Ladrão de Cova",
        [ClasseDeHeroi.BoboDaCorte] = "Bobo da Corte",
        [ClasseDeHeroi.MestreDeCaca] = "Mestre de Caça",
        [ClasseDeHeroi.Leproso] = "Leproso",
        [ClasseDeHeroi.Infernal] = "Infernal",
        [ClasseDeHeroi.Bandido] = "Bandido",
        [ClasseDeHeroi.Musqueteiro] = "Musqueteiro",
        [ClasseDeHeroi.Veterano] = "Veterano",
        [ClasseDeHeroi.Ocultista] = "Ocultista",
        [ClasseDeHeroi.MedicoDaPeste] = "Médico da Peste",
        [ClasseDeHeroi.Vestal] = "Vestal",
        [ClasseDeHeroi.Flagelante] = "Flagelante",
        [ClasseDeHeroi.Rompedor] = "Rompedor",
        [ClasseDeHeroi.Duelista] = "Duelista",
        [ClasseDeHeroi.Fugitivo] = "Fugitivo",
    };

    /// Resistências oficiais mineradas. Ordem dos campos:
    /// (Atordoamento, Sangramento, Envenenamento, Debuff, Movimento, Doença, GolpeMortal, Armadilha).
    private static readonly IReadOnlyDictionary<ClasseDeHeroi, ResistenciasDeClasse> ResistenciasBase = new Dictionary<ClasseDeHeroi, ResistenciasDeClasse>
    {
        [ClasseDeHeroi.Abominacao] = new(40, 30, 60, 20, 40, 20, 67, 10),
        [ClasseDeHeroi.Antiquario] = new(20, 20, 20, 20, 20, 20, 67, 10),
        [ClasseDeHeroi.Besteiro] = new(40, 30, 30, 30, 40, 30, 67, 10),
        [ClasseDeHeroi.CacadorDeRecompensas] = new(40, 30, 30, 30, 40, 20, 67, 40),
        [ClasseDeHeroi.Cruzado] = new(40, 30, 30, 30, 40, 30, 67, 10),
        [ClasseDeHeroi.LadraoDeCova] = new(20, 30, 50, 30, 20, 30, 67, 50),
        [ClasseDeHeroi.BoboDaCorte] = new(20, 30, 40, 40, 20, 20, 67, 30),
        [ClasseDeHeroi.MestreDeCaca] = new(40, 40, 40, 30, 40, 30, 67, 40),
        [ClasseDeHeroi.Leproso] = new(60, 10, 40, 40, 60, 20, 67, 10),
        [ClasseDeHeroi.Infernal] = new(40, 40, 40, 30, 40, 30, 67, 20),
        [ClasseDeHeroi.Bandido] = new(30, 30, 30, 30, 30, 30, 67, 40),
        [ClasseDeHeroi.Musqueteiro] = new(40, 30, 30, 30, 40, 30, 67, 10),
        [ClasseDeHeroi.Veterano] = new(40, 40, 30, 30, 40, 30, 67, 10),
        [ClasseDeHeroi.Ocultista] = new(20, 40, 30, 60, 20, 40, 67, 10),
        [ClasseDeHeroi.MedicoDaPeste] = new(20, 20, 60, 50, 20, 50, 67, 20),
        [ClasseDeHeroi.Vestal] = new(30, 40, 30, 30, 30, 30, 67, 10),
        [ClasseDeHeroi.Flagelante] = new(50, 65, 30, 30, 50, 40, 73, 0),
        [ClasseDeHeroi.Rompedor] = new(50, 30, 20, 30, 50, 30, 67, 20),
        [ClasseDeHeroi.Duelista] = new(30, 30, 30, 40, 30, 30, 67, 10),
        [ClasseDeHeroi.Fugitivo] = new(20, 40, 40, 20, 30, 30, 67, 30),
    };

    public static IReadOnlyList<Classe> Materializar()
    {
        var snapshots = LeitorDeSnapshotsOficiais.Carregar();
        var lista = new List<Classe>();
        foreach (var valor in Enum.GetValues<ClasseDeHeroi>())
        {
            var nomeOriginal = ObterNomeOriginal(valor);
            var nomeExibicao = NomesExibicao[valor];
            var resistencias = ResistenciasBase[valor];
            snapshots.TryGetValue(valor, out var snapshot);
            lista.Add(new Classe(
                valor,
                nomeExibicao,
                nomeOriginal,
                resistencias,
                GerarIdDeterministico(valor),
                snapshot?.PassosAFrente ?? 0,
                snapshot?.PassosAtras ?? 0,
                snapshot?.Religiosa ?? false,
                snapshot?.ProvisaoInicial ?? string.Empty,
                snapshot?.BonusAoCritico ?? string.Empty));
        }

        return lista;
    }

    public static int AplicarPerfilOficial(IEnumerable<Classe> classes)
    {
        ArgumentNullException.ThrowIfNull(classes);
        var snapshots = LeitorDeSnapshotsOficiais.Carregar();
        var atualizadas = 0;
        foreach (var classe in classes)
        {
            if (!snapshots.TryGetValue(classe.ClasseDeHeroi, out var snapshot))
            {
                continue;
            }

            if (classe.PassosAFrente == snapshot.PassosAFrente
                && classe.PassosAtras == snapshot.PassosAtras
                && classe.Religiosa == snapshot.Religiosa
                && classe.ProvisaoInicial == snapshot.ProvisaoInicial
                && classe.BonusAoCriticoDaClasse == snapshot.BonusAoCritico)
            {
                continue;
            }

            classe.DefinirPerfilOficial(
                snapshot.PassosAFrente,
                snapshot.PassosAtras,
                snapshot.Religiosa,
                snapshot.ProvisaoInicial,
                snapshot.BonusAoCritico);
            atualizadas++;
        }

        return atualizadas;
    }

    private static string ObterNomeOriginal(ClasseDeHeroi valor)
    {
        var membro = typeof(ClasseDeHeroi).GetMember(valor.ToString()).Single();
        var atributo = membro.GetCustomAttribute<DescriptionAttribute>();
        return atributo?.Description ?? valor.ToString();
    }

    /// GUID determinístico por valor de enum para permitir seed reprodutível.
    private static Guid GerarIdDeterministico(ClasseDeHeroi valor)
    {
        var bytes = new byte[16];
        BitConverter.GetBytes((int)valor).CopyTo(bytes, 0);
        bytes[6] = 0x40; // versão 4 (aleatório) — reprodutível
        bytes[8] = 0x80;
        return new Guid(bytes);
    }
}
