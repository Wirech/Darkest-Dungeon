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
}
