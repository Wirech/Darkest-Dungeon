using DarkestDungeon.Application.Auditoria;
using FluentAssertions;
using Xunit;

namespace DarkestDungeon.Api.Tests.Feature005;

/// Unit tests do comparador de 15 campos (T144-T147) do AuditoriaWikiService.
/// Rodam contra o método privado indiretamente via provedores fake em memória.
public sealed class AuditoriaComparadorTests
{
    [Fact]
    public async Task Auditoria_reporta_divergencia_de_posicoesValidas_quando_rank_do_snapshot_difere()
    {
        var snapshot = SnapshotComHabilidade(rank: new List<int> { 1, 2 });
        var seed = SeedadaBase() with { PosicoesValidas = new[] { 1, 2, 3 }, Tipo = "Combate" };

        var relatorio = await GerarRelatorio(snapshot, seed);
        DeveTerDiffNoCampo(relatorio, "posicoesValidas");
    }

    [Fact]
    public async Task Auditoria_reporta_divergencia_de_alvoEmArea()
    {
        var snapshot = SnapshotComHabilidade(alvoEmArea: true);
        var seed = SeedadaBase() with { AlvoEmArea = false, Tipo = "Combate" };

        var relatorio = await GerarRelatorio(snapshot, seed);
        DeveTerDiffNoCampo(relatorio, "alvoEmArea");
    }

    [Fact]
    public async Task Auditoria_reporta_divergencia_de_modificadorDano_no_level1()
    {
        var snapshot = SnapshotComHabilidade(modificadorDanoL1: 20m);
        var seed = SeedadaBase() with { ModificadorDano = 0m, Tipo = "Combate" };

        var relatorio = await GerarRelatorio(snapshot, seed);
        DeveTerDiffNoCampo(relatorio, "modificadorDano");
    }

    [Fact]
    public async Task Auditoria_reporta_efeito_agrupado_como_simplificado_FR010()
    {
        var snapshot = SnapshotComHabilidade();
        var seed = SeedadaBase() with { NomesDeEfeitosSeedados = new[] { "Bônus de Resistências" }, QuantidadeDeEfeitosSeedados = 1 };

        var relatorio = await GerarRelatorio(snapshot, seed);
        DeveTerDiffNoCampo(relatorio, "efeitos.simplificado");
    }

    [Fact]
    public async Task Auditoria_reporta_valor_so_na_descricao_FR011()
    {
        var snapshot = SnapshotComHabilidade();
        var seed = SeedadaBase() with { Descricao = "Reduz tocha em 15 pontos", NomesDeEfeitosSeedados = new[] { "Sangramento" }, QuantidadeDeEfeitosSeedados = 1 };

        var relatorio = await GerarRelatorio(snapshot, seed);
        DeveTerDiffNoCampo(relatorio, "efeitos.valorSoNaDescricao");
    }

    [Fact]
    public async Task Auditoria_reporta_efeito_vazio_omitido_no_seed_como_Faltando_SC006()
    {
        var snapshot = SnapshotComHabilidade(comEfeitos: true);
        var seed = SeedadaBase() with { QuantidadeDeEfeitosSeedados = 0, NomesDeEfeitosSeedados = Array.Empty<string>() };

        var relatorio = await GerarRelatorio(snapshot, seed);
        DeveTerDiffNoCampo(relatorio, "efeitos");
    }

    [Fact]
    public async Task Auditoria_nao_reporta_efeito_vazio_justificado_SC006()
    {
        var snapshot = SnapshotComHabilidade();
        var seed = SeedadaBase() with { QuantidadeDeEfeitosSeedados = 0, NomesDeEfeitosSeedados = Array.Empty<string>() };

        var relatorio = await GerarRelatorio(snapshot, seed);
        NaoDeveTerDiffNoCampo(relatorio, "efeitos");
    }

    [Fact]
    public async Task Auditoria_captura_ChancesCapadas_FR013_quando_wiki_declara_mais_de_100pct()
    {
        var snapshot = new AuditoriaWikiService.SnapshotDeClasse
        {
            Habilidades = new List<AuditoriaWikiService.HabilidadeDoSnapshot>
            {
                new()
                {
                    NomeExibicao = "X",
                    NomeOriginal = "SkillX",
                    Tipo = "Combate",
                    Descricao = "d",
                    Efeitos = new List<AuditoriaWikiService.EfeitoDoSnapshot>
                    {
                        new()
                        {
                            TipoDoEfeito = "Atordoamento",
                            ChancePorNivel = new List<System.Text.Json.JsonElement>
                            {
                                System.Text.Json.JsonDocument.Parse("100").RootElement,
                                System.Text.Json.JsonDocument.Parse("110").RootElement,
                                System.Text.Json.JsonDocument.Parse("120").RootElement,
                                System.Text.Json.JsonDocument.Parse("130").RootElement,
                                System.Text.Json.JsonDocument.Parse("140").RootElement,
                            },
                        },
                    },
                },
            },
        };
        var seed = new HabilidadeSeedada(Guid.NewGuid(), "X", "SkillX", "d", 5) { Tipo = "Combate" };

        var relatorio = await GerarRelatorio(snapshot, seed);
        relatorio.ChancesCapadas.Should().HaveCount(4);
        relatorio.ChancesCapadas[0].ChanceOriginal.Should().Be(110m);
        relatorio.ChancesCapadas[0].ChanceAplicada.Should().Be(100m);
    }

    private static AuditoriaWikiService.SnapshotDeClasse SnapshotComHabilidade(
        List<int>? rank = null,
        bool? alvoEmArea = null,
        decimal? modificadorDanoL1 = null,
        bool comEfeitos = false)
    {
        var hab = new AuditoriaWikiService.HabilidadeDoSnapshot
        {
            NomeExibicao = "N",
            NomeOriginal = "SkillX",
            Tipo = "Combate",
            Descricao = "d",
            Rank = rank,
            AlvoEmArea = alvoEmArea,
        };
        if (modificadorDanoL1.HasValue)
        {
            hab.Campos = new AuditoriaWikiService.CamposDoSnapshot
            {
                ModificadorDano = new List<System.Text.Json.JsonElement>
                {
                    System.Text.Json.JsonDocument.Parse(modificadorDanoL1.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)).RootElement,
                },
            };
        }
        if (comEfeitos)
        {
            hab.Efeitos = new List<AuditoriaWikiService.EfeitoDoSnapshot>
            {
                new() { TipoDoEfeito = "Sangramento", ValorPorNivel = new List<System.Text.Json.JsonElement>() },
            };
        }
        return new AuditoriaWikiService.SnapshotDeClasse
        {
            Habilidades = new List<AuditoriaWikiService.HabilidadeDoSnapshot> { hab },
        };
    }

    private static HabilidadeSeedada SeedadaBase() =>
        new(Guid.NewGuid(), "N", "SkillX", "d", 5);

    private static async Task<RelatorioDeAuditoria> GerarRelatorio(AuditoriaWikiService.SnapshotDeClasse snapshot, HabilidadeSeedada seed)
    {
        var provedorSnap = new ProvedorFake(snapshot);
        var provedorSeed = new ProvedorHabFake(seed);
        var provedorAssets = new ProvedorAssetsFake();
        var servico = new AuditoriaWikiService(provedorSnap, provedorSeed, provedorAssets);
        return await servico.GerarRelatorioAsync(DarkestDungeon.Domain.Classes.ClasseDeHeroi.Cruzado, CancellationToken.None);
    }

    private static void DeveTerDiffNoCampo(RelatorioDeAuditoria r, string campo) =>
        r.Linhas.SelectMany(l => l.Diffs).Should().Contain(d => d.Campo == campo);

    private static void NaoDeveTerDiffNoCampo(RelatorioDeAuditoria r, string campo) =>
        r.Linhas.SelectMany(l => l.Diffs).Should().NotContain(d => d.Campo == campo);

    private sealed class ProvedorFake(AuditoriaWikiService.SnapshotDeClasse? snap) : IProvedorDeSnapshotsWiki
    {
        public Task<AuditoriaWikiService.SnapshotDeClasse?> CarregarSnapshotAsync(DarkestDungeon.Domain.Classes.ClasseDeHeroi classe, CancellationToken cancellationToken) =>
            Task.FromResult(snap);
    }

    private sealed class ProvedorHabFake(HabilidadeSeedada seed) : IProvedorDeHabilidadesSeed
    {
        public Task<IReadOnlyList<HabilidadeSeedada>> ObterHabilidadesDaClasseAsync(DarkestDungeon.Domain.Classes.ClasseDeHeroi classe, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<HabilidadeSeedada>>(new[] { seed });
    }

    private sealed class ProvedorAssetsFake : IProvedorDeAssetsAuditados
    {
        public Task<(int coletados, int pendentes)> ContarAssetsAsync(CancellationToken cancellationToken) =>
            Task.FromResult<(int, int)>((0, 0));
    }
}
