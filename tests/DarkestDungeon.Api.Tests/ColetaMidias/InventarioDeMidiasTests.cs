using FluentAssertions;
using DarkestDungeon.MediaCollector.Catalogo;
using DarkestDungeon.MediaCollector.Configuracao;
using DarkestDungeon.MediaCollector.Importacao;

namespace DarkestDungeon.Api.Tests.ColetaMidias;

public sealed class InventarioDeMidiasTests
{
    [Fact]
    public async Task ExecutarAsync_ProduzInventarioComProveniencia()
    {
        var raiz = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            CriarFixture(raiz);
            OpcoesDoColetor.TentarCriar(["--origem", raiz, "--saida", Path.Combine(raiz, "saida"), "--classe", "Antiquarian"], out var opcoes, out _).Should().BeTrue();
            var heroi = CatalogoDeHerois.ObterTodos().Single(item => item.NomeOriginal == "Antiquarian");

            var resultado = await new ImportadorLocal(opcoes!).ExecutarAsync([heroi], CancellationToken.None);

            resultado.InstalacaoOrigem.Should().Be(Path.GetFullPath(raiz));
            resultado.DeclaracaoDeUso.Should().Contain("redistribuição não autorizada");
            resultado.Arquivos.Should().HaveCount(3);
            resultado.ResumoPorClasse.Should().ContainSingle().Which.Arquivos.Should().Be(3);
            resultado.Lacunas.Should().OnlyContain(item => item.TentadoEmUtc != default);
        }
        finally
        {
            if (Directory.Exists(raiz)) Directory.Delete(raiz, true);
        }
    }

    [Fact]
    public async Task ExecutarAsync_ComContinuar_ReutilizaArquivosDoInventarioAnterior()
    {
        var raiz = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            CriarFixture(raiz);
            var saida = Path.Combine(raiz, "saida");
            OpcoesDoColetor.TentarCriar(["--origem", raiz, "--saida", saida, "--classe", "Antiquarian"], out var opcoesInicial, out _).Should().BeTrue();
            var heroi = CatalogoDeHerois.ObterTodos().Single(item => item.NomeOriginal == "Antiquarian");
            await new ImportadorLocal(opcoesInicial!).ExecutarAsync([heroi], CancellationToken.None);

            OpcoesDoColetor.TentarCriar(["--origem", raiz, "--saida", saida, "--classe", "Antiquarian", "--continuar"], out var opcoesContinuar, out _).Should().BeTrue();
            var resultado = await new ImportadorLocal(opcoesContinuar!).ExecutarAsync([heroi], CancellationToken.None);

            resultado.Arquivos.Should().OnlyContain(arquivo => arquivo.Reutilizado);
        }
        finally
        {
            if (Directory.Exists(raiz)) Directory.Delete(raiz, true);
        }
    }

    private static void CriarFixture(string raiz)
    {
        var diretorio = Path.Combine(raiz, "heroes", "antiquarian", "anim");
        Directory.CreateDirectory(diretorio);
        File.WriteAllBytes(Path.Combine(diretorio, "antiquarian.sprite.idle.png"), [137, 80, 78, 71, 13, 10, 26, 10, 1]);
        File.WriteAllText(Path.Combine(diretorio, "antiquarian.sprite.idle.atlas"), "antiquarian.sprite.idle.png\nsize: 1,1\n");
        File.WriteAllBytes(Path.Combine(diretorio, "antiquarian.sprite.idle.skel"), [1, 2, 3]);
    }
}
