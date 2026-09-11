using System.Security.Cryptography;
using System.Text.Json;
using FluentAssertions;
using DarkestDungeon.MediaCollector.Configuracao;
using DarkestDungeon.MediaCollector.Importacao;
using DarkestDungeon.MediaCollector.Inventario;

namespace DarkestDungeon.Api.Tests.ColetaMidias;

public sealed class ImportadorDeCampingTests
{
    private static readonly byte[] Png = [137, 80, 78, 71, 13, 10, 26, 10, 1];

    [Fact]
    public async Task ExecutarAsync_CopiaBytesIdenticosEMesclaInventario004()
    {
        var raiz = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var origem = Path.Combine(raiz, "origem");
        var saida = Path.Combine(raiz, "saida");
        try
        {
            var vanilla = Path.Combine(origem, "raid", "camping", "skill_icons");
            Directory.CreateDirectory(vanilla);
            await File.WriteAllBytesAsync(Path.Combine(vanilla, "camp_skill_encourage.png"), Png);
            await File.WriteAllBytesAsync(Path.Combine(vanilla, "camp_skill_bandage.png"), Png);

            var dlc = Path.Combine(origem, "dlc", "crimson_court", "raid", "camping", "skill_icons");
            Directory.CreateDirectory(dlc);
            await File.WriteAllBytesAsync(Path.Combine(dlc, "camp_skill_lash_anger.png"), Png);
            await File.WriteAllBytesAsync(Path.Combine(dlc, "camp_skill_encourage.png"), [1, 2, 3]);

            var existente = new ResultadoDaImportacao(
                origem,
                "Assets importados de instalação local licenciada; redistribuição não autorizada.",
                [new("Cruzado", "origem/idle.png", "arquivos/cruzado/idle.png", "abc", 3, false, "Retrato")],
                [],
                [],
                [],
                [new("Cruzado", 1, 0, 0, 0)]);
            await ArmazenamentoDeInventario.SalvarAtomicamenteAsync(saida, existente, CancellationToken.None);

            OpcoesDoColetor.TentarCriar(["--origem", origem, "--saida", saida, "--camping"], out var opcoes, out var erro)
                .Should().BeTrue(erro);
            var resultado = await new ImportadorDeCamping(opcoes!).ExecutarAsync(CancellationToken.None);

            resultado.Arquivos.Should().Contain(a => a.CaminhoDestino.EndsWith("camp_skill_encourage.png"));
            resultado.Arquivos.Should().Contain(a => a.CaminhoDestino.EndsWith("camp_skill_lash_anger.png"));
            resultado.Arquivos.Should().NotContain(a => a.CaminhoDestino.Contains("bandage"));
            resultado.Arquivos.Should().OnlyContain(a => a.Categoria == "HabilidadeDeAcampamento");

            var destinoEncourage = Path.Combine(saida, "arquivos", "acampamento", "camp_skill_encourage.png");
            File.ReadAllBytes(destinoEncourage).Should().Equal(Png);
            Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(destinoEncourage))).ToLowerInvariant()
                .Should().Be(resultado.Arquivos.Single(a => a.CaminhoDestino.Contains("encourage")).Sha256);

            var inventario = JsonSerializer.Deserialize<ResultadoDaImportacao>(
                await File.ReadAllTextAsync(Path.Combine(saida, "inventario.json")))!;
            inventario.Arquivos.Should().Contain(a => a.CaminhoDestino == "arquivos/cruzado/idle.png");
            inventario.Arquivos.Should().Contain(a => a.CaminhoDestino.Contains("acampamento/camp_skill_encourage.png"));
        }
        finally
        {
            if (Directory.Exists(raiz)) Directory.Delete(raiz, true);
        }
    }

    [Fact]
    public async Task ExecutarAsync_PastaAusenteViraLacuna()
    {
        var raiz = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var origem = Path.Combine(raiz, "origem");
        var saida = Path.Combine(raiz, "saida");
        Directory.CreateDirectory(origem);
        try
        {
            OpcoesDoColetor.TentarCriar(["--origem", origem, "--saida", saida, "--camping"], out var opcoes, out _).Should().BeTrue();
            var resultado = await new ImportadorDeCamping(opcoes!).ExecutarAsync(CancellationToken.None);
            resultado.Arquivos.Should().BeEmpty();
            resultado.Lacunas.Should().Contain(l => l.Categoria == "HabilidadeDeAcampamento");
        }
        finally
        {
            if (Directory.Exists(raiz)) Directory.Delete(raiz, true);
        }
    }

    [Fact]
    public async Task ExecutarAsync_SimularNaoGrava()
    {
        var raiz = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var origem = Path.Combine(raiz, "origem");
        var saida = Path.Combine(raiz, "saida");
        var vanilla = Path.Combine(origem, "raid", "camping", "skill_icons");
        Directory.CreateDirectory(vanilla);
        await File.WriteAllBytesAsync(Path.Combine(vanilla, "camp_skill_encourage.png"), Png);
        try
        {
            OpcoesDoColetor.TentarCriar(["--origem", origem, "--saida", saida, "--camping", "--simular"], out var opcoes, out _).Should().BeTrue();
            var resultado = await new ImportadorDeCamping(opcoes!).ExecutarAsync(CancellationToken.None);
            resultado.Arquivos.Should().ContainSingle();
            File.Exists(Path.Combine(saida, "inventario.json")).Should().BeFalse();
            Directory.Exists(Path.Combine(saida, "arquivos", "acampamento")).Should().BeFalse();
        }
        finally
        {
            if (Directory.Exists(raiz)) Directory.Delete(raiz, true);
        }
    }
}
