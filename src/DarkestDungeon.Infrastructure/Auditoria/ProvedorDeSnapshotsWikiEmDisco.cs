using System.Text.Json;
using DarkestDungeon.Application.Auditoria;
using DarkestDungeon.Domain.Classes;

namespace DarkestDungeon.Infrastructure.Auditoria;

/// Lê wiki-snapshots do diretório `specs/005-auditoria-habilidades-mineradas/wiki-snapshots/`.
public sealed class ProvedorDeSnapshotsWikiEmDisco : IProvedorDeSnapshotsWiki
{
    private static readonly IReadOnlyDictionary<ClasseDeHeroi, string> SlugPorClasse = new Dictionary<ClasseDeHeroi, string>
    {
        [ClasseDeHeroi.Abominacao] = "abominacao",
        [ClasseDeHeroi.Antiquario] = "antiquario",
        [ClasseDeHeroi.Besteiro] = "besteiro",
        [ClasseDeHeroi.CacadorDeRecompensas] = "cacador-de-recompensas",
        [ClasseDeHeroi.Cruzado] = "cruzado",
        [ClasseDeHeroi.LadraoDeCova] = "ladrao-de-cova",
        [ClasseDeHeroi.BoboDaCorte] = "bobo-da-corte",
        [ClasseDeHeroi.MestreDeCaca] = "mestre-de-caca",
        [ClasseDeHeroi.Leproso] = "leproso",
        [ClasseDeHeroi.Infernal] = "infernal",
        [ClasseDeHeroi.Bandido] = "bandido",
        [ClasseDeHeroi.Musqueteiro] = "musqueteiro",
        [ClasseDeHeroi.Veterano] = "veterano",
        [ClasseDeHeroi.Ocultista] = "ocultista",
        [ClasseDeHeroi.MedicoDaPeste] = "medico-da-peste",
        [ClasseDeHeroi.Vestal] = "vestal",
        [ClasseDeHeroi.Flagelante] = "flagelante",
        [ClasseDeHeroi.Rompedor] = "rompedor",
        [ClasseDeHeroi.Duelista] = "duelista",
        [ClasseDeHeroi.Fugitivo] = "fugitivo",
    };

    private const string DiretorioSnapshots = "specs/005-auditoria-habilidades-mineradas/wiki-snapshots";
    private const string ArquivoCompartilhadas = "compartilhadas.json";

    // Cache do snapshot compartilhado — evita re-leitura por classe durante o mesmo relatório.
    private static readonly Lazy<AuditoriaWikiService.SnapshotDeClasse?> CompartilhadasCache = new(CarregarCompartilhadas);

    public async Task<AuditoriaWikiService.SnapshotDeClasse?> CarregarSnapshotAsync(ClasseDeHeroi classe, CancellationToken cancellationToken)
    {
        if (!SlugPorClasse.TryGetValue(classe, out var slug))
        {
            return null;
        }

        var caminho = ResolverCaminho($"{slug}.json");
        if (caminho is null || !File.Exists(caminho))
        {
            return null;
        }

        await using var stream = File.OpenRead(caminho);
        var snapshot = await JsonSerializer.DeserializeAsync<AuditoriaWikiService.SnapshotDeClasse>(
            stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken);

        // Mescla as 3 shared globais (Encourage/Wound Care/Pep Talk) em toda classe exceto o Flagelante.
        if (snapshot?.Habilidades is not null && classe != ClasseDeHeroi.Flagelante)
        {
            var compartilhadas = CompartilhadasCache.Value?.Habilidades;
            if (compartilhadas is not null)
            {
                snapshot.Habilidades.AddRange(compartilhadas);
            }
        }

        return snapshot;
    }

    private static AuditoriaWikiService.SnapshotDeClasse? CarregarCompartilhadas()
    {
        var caminho = ResolverCaminho(ArquivoCompartilhadas);
        if (caminho is null || !File.Exists(caminho))
        {
            return null;
        }

        using var stream = File.OpenRead(caminho);
        return JsonSerializer.Deserialize<AuditoriaWikiService.SnapshotDeClasse>(
            stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    private static string? ResolverCaminho(string arquivo)
    {
        var atual = AppContext.BaseDirectory;
        for (int nivel = 0; nivel < 8; nivel++)
        {
            var candidato = Path.Combine(atual, DiretorioSnapshots, arquivo);
            if (File.Exists(candidato))
            {
                return candidato;
            }

            var pai = Directory.GetParent(atual)?.FullName;
            if (pai is null)
            {
                return null;
            }

            atual = pai;
        }

        return null;
    }
}
