using FluentAssertions;
using DarkestDungeon.MediaCollector.Catalogo;
using DarkestDungeon.MediaCollector.Configuracao;
using DarkestDungeon.MediaCollector.Importacao;

namespace DarkestDungeon.Api.Tests.ColetaMidias;

public sealed class ImportadorDeArquivosTests
{
    [Fact]
    public async Task ExecutarAsync_CopiaAssetsSpineSemAlterarBytes()
    {
        var raiz = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var destino = Path.Combine(raiz, "saida");
        var animacao = Path.Combine(raiz, "heroes", "antiquarian", "anim");
        Directory.CreateDirectory(animacao);
        var png = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 1 };
        await File.WriteAllBytesAsync(Path.Combine(animacao, "antiquarian.sprite.idle.png"), png);
        await File.WriteAllTextAsync(Path.Combine(animacao, "antiquarian.sprite.idle.atlas"), "page.png\nsize: 1,1\n");
        await File.WriteAllBytesAsync(Path.Combine(animacao, "antiquarian.sprite.idle.skel"), [1, 2, 3]);

        try
        {
            OpcoesDoColetor.TentarCriar(["--origem", raiz, "--saida", destino], out var opcoes, out _).Should().BeTrue();
            var heroi = CatalogoDeHerois.ObterTodos().Single(valor => valor.NomeOriginal == "Antiquarian");
            var resultado = await new ImportadorLocal(opcoes!).ExecutarAsync([heroi], CancellationToken.None);

            resultado.Arquivos.Should().HaveCount(3);
            var arquivo = resultado.Arquivos.Single(valor => valor.CaminhoOrigem.EndsWith(".png"));
            File.ReadAllBytes(Path.Combine(destino, arquivo.CaminhoDestino)).Should().Equal(png);
            File.Exists(Path.Combine(destino, "inventario.json")).Should().BeTrue();
        }
        finally
        {
            if (Directory.Exists(raiz)) Directory.Delete(raiz, true);
        }
    }
}
