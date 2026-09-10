using System.Security.Cryptography;
using FluentAssertions;
using DarkestDungeon.MediaCollector.Configuracao;
using DarkestDungeon.MediaCollector.Importacao;

namespace DarkestDungeon.Api.Tests.ColetaMidias;

public sealed class ImportadorDeEquipamentosTests
{
    private static readonly byte[] Png = [137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 0];

    [Fact]
    public async Task ExecutarAsync_CopiaPngComHashIdenticoECategoriaCorreta()
    {
        var raiz = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var origem = Path.Combine(raiz, "origem");
        var saida = Path.Combine(raiz, "saida");
        CopiarFixture(origem);
        try
        {
            OpcoesDoColetor.TentarCriar(
                ["--origem", origem, "--saida", saida, "--categoria", "arma", "--categoria", "armadura", "--categoria", "acessorio", "--categoria", "acampamento", "--categoria", "consumivel", "--mapeamento", Mapeamento()],
                out var opcoes,
                out var erro).Should().BeTrue(erro);
            var resultado = await new ImportadorDeEquipamentos(opcoes!).ExecutarAsync(CancellationToken.None);

            resultado.Arquivos.Should().Contain(a => a.Categoria == "Arma");
            resultado.Arquivos.Should().Contain(a => a.Categoria == "Armadura");
            resultado.Arquivos.Should().Contain(a => a.Categoria == "Acessorio");
            resultado.Arquivos.Should().Contain(a => a.Categoria == "ItemDeAcampamento" && a.CaminhoOrigem.Contains("provision"));
            resultado.Arquivos.Should().NotContain(a => a.Categoria == "Consumivel" && a.CaminhoOrigem.Contains("provision"));
            resultado.Arquivos.Should().Contain(a => a.Categoria == "Consumivel");
            resultado.Arquivos.Should().Contain(a => a.Categoria == "NaoAssociado");

            var arma = resultado.Arquivos.First(a => a.Categoria == "Arma");
            var hashOrigem = Convert.ToHexString(SHA256.HashData(await File.ReadAllBytesAsync(arma.CaminhoOrigem))).ToLowerInvariant();
            arma.Sha256.Should().Be(hashOrigem);
            File.ReadAllBytes(Path.Combine(saida, arma.CaminhoDestino.Replace('/', Path.DirectorySeparatorChar)))
                .Should().Equal(Png);
            File.Exists(Path.Combine(saida, "inventario.json")).Should().BeTrue();
        }
        finally
        {
            if (Directory.Exists(raiz)) Directory.Delete(raiz, true);
        }
    }

    [Fact]
    public async Task ExecutarAsync_DedupePorHashNaoDuplicaBlob()
    {
        var raiz = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var origem = Path.Combine(raiz, "origem");
        var saida = Path.Combine(raiz, "saida");
        CopiarFixture(origem);
        Directory.CreateDirectory(Path.Combine(origem, "heroes", "crusader"));
        await File.WriteAllBytesAsync(Path.Combine(origem, "heroes", "crusader", "crusader_weapon_dup.png"), Png);
        try
        {
            OpcoesDoColetor.TentarCriar(
                ["--origem", origem, "--saida", saida, "--categoria", "arma", "--mapeamento", Mapeamento()],
                out var opcoes,
                out _).Should().BeTrue();
            var resultado = await new ImportadorDeEquipamentos(opcoes!).ExecutarAsync(CancellationToken.None);
            resultado.Arquivos.Count(a => a.Reutilizado).Should().BeGreaterThan(0);
        }
        finally
        {
            if (Directory.Exists(raiz)) Directory.Delete(raiz, true);
        }
    }

    [Fact]
    public async Task ExecutarAsync_PastaAusenteViraLacunaEContinua()
    {
        var raiz = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var origem = Path.Combine(raiz, "origem");
        var saida = Path.Combine(raiz, "saida");
        Directory.CreateDirectory(Path.Combine(origem, "heroes", "crusader"));
        await File.WriteAllBytesAsync(Path.Combine(origem, "heroes", "crusader", "crusader_weapon_1.png"), Png);
        try
        {
            OpcoesDoColetor.TentarCriar(
                ["--origem", origem, "--saida", saida, "--categoria", "arma", "--categoria", "acessorio", "--mapeamento", Mapeamento(), "--continuar"],
                out var opcoes,
                out _).Should().BeTrue();
            var resultado = await new ImportadorDeEquipamentos(opcoes!).ExecutarAsync(CancellationToken.None);
            resultado.Lacunas.Should().Contain(l => l.Categoria == "Acessorio");
            resultado.Arquivos.Should().Contain(a => a.Categoria == "Arma");
        }
        finally
        {
            if (Directory.Exists(raiz)) Directory.Delete(raiz, true);
        }
    }

    [Fact]
    public void TentarCriar_ClasseSemCategoria_NaoEntraNoModoEquipamentos()
    {
        var origem = Path.GetTempPath();
        OpcoesDoColetor.TentarCriar(["--origem", origem, "--saida", "saida", "--classe", "Antiquarian"], out var opcoes, out _).Should().BeTrue();
        opcoes!.ModoEquipamentos.Should().BeFalse();
        opcoes.Classes.Should().Contain("Antiquarian");
    }

    [Fact]
    public void TentarCriar_CategoriaInvalida_RetornaErroPtBr()
    {
        OpcoesDoColetor.TentarCriar(["--origem", Path.GetTempPath(), "--saida", "saida", "--categoria", "inimigo"], out var opcoes, out var erro).Should().BeFalse();
        opcoes.Should().BeNull();
        erro.Should().Contain("arma, armadura, acessorio, acampamento ou consumivel");
    }

    private static void CopiarFixture(string destino)
    {
        var origem = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "ColetaMidias", "Fixtures", "Equipamentos"));
        foreach (var arquivo in Directory.EnumerateFiles(origem, "*", SearchOption.AllDirectories))
        {
            var relativo = Path.GetRelativePath(origem, arquivo);
            var alvo = Path.Combine(destino, relativo);
            Directory.CreateDirectory(Path.GetDirectoryName(alvo)!);
            File.Copy(arquivo, alvo, overwrite: true);
        }
    }

    private static string Mapeamento() => Path.GetFullPath(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "..",
        "tools", "DarkestDungeon.MediaCollector", "Configuracao", "mapeamento-pastas.json"));
}
