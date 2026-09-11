using FluentAssertions;
using DarkestDungeon.MediaCollector.Configuracao;

namespace DarkestDungeon.Api.Tests.ColetaMidias;

public sealed class ContratoImportadorCliTests
{
    [Fact]
    public void TentarCriar_RejeitaOrigemAusente()
    {
        OpcoesDoColetor.TentarCriar(["--saida", "resultado"], out var opcoes, out var erro).Should().BeFalse();
        opcoes.Should().BeNull();
        erro.Should().Contain("--origem");
    }

    [Fact]
    public void TentarCriar_AceitaImportacaoLocal()
    {
        var origem = Path.GetTempPath();
        OpcoesDoColetor.TentarCriar(["--origem", origem, "--saida", "resultado", "--classe", "Antiquarian", "--simular"], out var opcoes, out _).Should().BeTrue();
        opcoes!.DiretorioDeOrigem.Should().Be(Path.GetFullPath(origem));
        opcoes.Simular.Should().BeTrue();
    }

    [Fact]
    public void TentarCriar_AceitaCamping()
    {
        var origem = Path.GetTempPath();
        OpcoesDoColetor.TentarCriar(["--origem", origem, "--saida", "resultado", "--camping"], out var opcoes, out var erro).Should().BeTrue(erro);
        opcoes!.ModoCamping.Should().BeTrue();
        opcoes.ModoEquipamentos.Should().BeFalse();
    }

    [Fact]
    public void TentarCriar_RejeitaCampingComCategoria()
    {
        OpcoesDoColetor.TentarCriar(
            ["--origem", Path.GetTempPath(), "--saida", "resultado", "--camping", "--categoria", "arma"],
            out var opcoes,
            out var erro).Should().BeFalse();
        opcoes.Should().BeNull();
        erro.Should().Contain("--camping");
        erro.Should().Contain("--categoria");
    }
}
