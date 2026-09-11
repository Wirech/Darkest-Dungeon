using System.Text.Json;
using System.Text.Json.Serialization;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Itens;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

public static class AcessoriosOficiaisSeed
{
    internal const string DiretorioRelativo = "specs/013-equipar-trinkets/wiki-snapshots";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public static string? EncontrarDiretorio()
    {
        var atual = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (atual is not null)
        {
            var candidato = Path.Combine(atual.FullName, DiretorioRelativo);
            if (Directory.Exists(candidato))
            {
                return candidato;
            }

            atual = atual.Parent;
        }

        return null;
    }

    public static IReadOnlyList<SnapshotDeAcessorioOficial> CarregarSnapshots()
    {
        var diretorio = EncontrarDiretorio();
        if (diretorio is null)
        {
            return [];
        }

        var lista = new List<SnapshotDeAcessorioOficial>();
        foreach (var arquivo in Directory.GetFiles(diretorio, "*.json"))
        {
            try
            {
                var json = File.ReadAllText(arquivo);
                var snapshot = JsonSerializer.Deserialize<SnapshotDeAcessorioOficial>(json, JsonOptions);
                if (snapshot is null || string.IsNullOrWhiteSpace(snapshot.NomeOriginal))
                {
                    continue;
                }

                lista.Add(snapshot);
            }
            catch (JsonException)
            {
            }
        }

        return lista;
    }

    public static int Aplicar(IEnumerable<Item> itensExistentes, Action<Acessorio> adicionar)
    {
        var existentes = itensExistentes.OfType<Acessorio>().ToList();
        var aplicados = 0;
        foreach (var snapshot in CarregarSnapshots())
        {
            var efeitos = snapshot.Efeitos
                .Where(e => !string.IsNullOrWhiteSpace(e.Nome))
                .Select(e => new EfeitoDeAcessorio(e.Nome, e.Valor, e.Unidade, e.Sinal))
                .ToArray();
            var lacunaDeEfeitos = snapshot.Lacunas.Any(l =>
                l.Campo.Contains("efeito", StringComparison.OrdinalIgnoreCase));
            var existente = existentes.FirstOrDefault(a =>
                string.Equals(a.NomeOriginal, snapshot.NomeOriginal, StringComparison.OrdinalIgnoreCase));
            if (existente is not null)
            {
                existente.AtualizarCatalogoOficial(
                    string.IsNullOrWhiteSpace(snapshot.NomeExibicao) ? existente.NomeExibicao : snapshot.NomeExibicao,
                    snapshot.NomeOriginal,
                    snapshot.Descricao ?? existente.Descricao,
                    snapshot.Raridade,
                    snapshot.ClasseExclusiva,
                    lacunaDeEfeitos && efeitos.Length == 0 ? null : efeitos);
                aplicados++;
                continue;
            }

            var novo = new Acessorio(
                string.IsNullOrWhiteSpace(snapshot.NomeExibicao) ? snapshot.NomeOriginal : snapshot.NomeExibicao,
                snapshot.NomeOriginal,
                snapshot.Descricao ?? string.Empty,
                snapshot.Raridade ?? RaridadeDeAcessorio.Comum,
                efeitos,
                snapshot.ClasseExclusiva);
            adicionar(novo);
            existentes.Add(novo);
            aplicados++;
        }

        return aplicados;
    }

    public sealed class SnapshotDeAcessorioOficial
    {
        public string NomeOriginal { get; set; } = string.Empty;
        public string NomeExibicao { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public RaridadeDeAcessorio? Raridade { get; set; }
        public ClasseDeHeroi? ClasseExclusiva { get; set; }
        public string? ConjuntoIdWiki { get; set; }
        public string FonteUrl { get; set; } = string.Empty;
        public List<EfeitoDeAcessorioSnapshot> Efeitos { get; set; } = new();
        public List<LacunaDeAcessorioSnapshot> Lacunas { get; set; } = new();
    }

    public sealed class EfeitoDeAcessorioSnapshot
    {
        public string Nome { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public UnidadeDeEfeitoDeAcessorio Unidade { get; set; }
        public SinalDeEfeito Sinal { get; set; }
    }

    public sealed class LacunaDeAcessorioSnapshot
    {
        public string TrinketOuPagina { get; set; } = string.Empty;
        public string Campo { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public string Momento { get; set; } = string.Empty;
    }
}
