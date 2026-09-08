using FluentAssertions;
using DarkestDungeon.MediaCollector.Catalogo;
using DarkestDungeon.MediaCollector.Configuracao;
using DarkestDungeon.MediaCollector.Importacao;

namespace DarkestDungeon.Api.Tests.ColetaMidias;

public sealed class DescobridorDeHeroisTests
{
    [Fact]
    public async Task ExecutarAsync_DescobreHeroisBaseEDlc()
    {
        var raiz = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            CriarAsset(raiz, "heroes/antiquarian/anim", "antiquarian.sprite.idle");
            CriarAsset(raiz, "dlc/1234_musketeer/musketeer/heroes/musketeer/anim", "musketeer.sprite.idle");
            OpcoesDoColetor.TentarCriar(["--origem", raiz, "--saida", Path.Combine(raiz, "saida"), "--classe", "Antiquarian", "--classe", "Musketeer"], out var opcoes, out _).Should().BeTrue();
            var herois = CatalogoDeHerois.ObterTodos().Where(item => item.NomeOriginal is "Antiquarian" or "Musketeer").ToArray();

            var resultado = await new ImportadorLocal(opcoes!).ExecutarAsync(herois, CancellationToken.None);

            resultado.Arquivos.Should().OnlyContain(arquivo => arquivo.Classe == "Antiquário" || arquivo.Classe == "Musqueteiro");
            resultado.Arquivos.Should().HaveCountGreaterThan(0);
            resultado.Lacunas.Should().NotContain(item => item.Motivo.Contains("Diretório do herói"));
        }
        finally
        {
            if (Directory.Exists(raiz)) Directory.Delete(raiz, true);
        }
    }

    [Fact]
    public async Task ExecutarAsync_RegistraLacunaQuandoClasseAusente()
    {
        var raiz = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(raiz, "heroes"));
        try
        {
            OpcoesDoColetor.TentarCriar(["--origem", raiz, "--saida", Path.Combine(raiz, "saida"), "--classe", "Antiquarian"], out var opcoes, out _).Should().BeTrue();
            var heroi = CatalogoDeHerois.ObterTodos().Single(item => item.NomeOriginal == "Antiquarian");

            var resultado = await new ImportadorLocal(opcoes!).ExecutarAsync([heroi], CancellationToken.None);

            resultado.Lacunas.Should().ContainSingle().Which.Motivo.Should().Contain("Diretório do herói");
        }
        finally
        {
            if (Directory.Exists(raiz)) Directory.Delete(raiz, true);
        }
    }

    private static void CriarAsset(string raiz, string subCaminho, string nomeBase)
    {
        var diretorio = Path.Combine(raiz, subCaminho);
        Directory.CreateDirectory(diretorio);
        File.WriteAllBytes(Path.Combine(diretorio, $"{nomeBase}.png"), [137, 80, 78, 71, 13, 10, 26, 10, 1]);
        File.WriteAllText(Path.Combine(diretorio, $"{nomeBase}.atlas"), $"{nomeBase}.png\nsize: 1,1\n");
        File.WriteAllBytes(Path.Combine(diretorio, $"{nomeBase}.skel"), [1, 2, 3]);
    }
}
