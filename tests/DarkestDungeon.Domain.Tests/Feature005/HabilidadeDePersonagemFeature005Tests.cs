using DarkestDungeon.Domain.Personagens;
using FluentAssertions;
using Xunit;

namespace DarkestDungeon.Domain.Tests.Feature005;

public sealed class HabilidadeDePersonagemFeature005Tests
{
    [Fact]
    public void Novo_construtor_com_numeroDoNivel_0_indica_bloqueada()
    {
        var habilidade = new HabilidadeDePersonagem(Guid.NewGuid(), habilitada: true, treinada: false, equipada: false, numeroDoNivel: 0);

        habilidade.NumeroDoNivel.Should().Be(0);
        habilidade.TreinadaPorNivel.Should().BeFalse();
        habilidade.Treinada.Should().BeFalse();
    }

    [Fact]
    public void DefinirNivel_1_a_5_marca_treinada()
    {
        var habilidade = new HabilidadeDePersonagem(Guid.NewGuid());

        habilidade.DefinirNivel(3);

        habilidade.NumeroDoNivel.Should().Be(3);
        habilidade.TreinadaPorNivel.Should().BeTrue();
        habilidade.Treinada.Should().BeTrue();
    }

    [Fact]
    public void DefinirNivel_0_reseta_treinada()
    {
        var habilidade = new HabilidadeDePersonagem(Guid.NewGuid(), treinada: true, numeroDoNivel: 3);

        habilidade.DefinirNivel(0);

        habilidade.NumeroDoNivel.Should().Be(0);
        habilidade.TreinadaPorNivel.Should().BeFalse();
        habilidade.Treinada.Should().BeFalse();
    }

    [Fact]
    public void DefinirNivel_0_em_habilidade_equipada_lanca()
    {
        var habilidade = new HabilidadeDePersonagem(Guid.NewGuid(), treinada: true, equipada: true, numeroDoNivel: 2);

        var acao = () => habilidade.DefinirNivel(0);
        acao.Should().Throw<InvalidOperationException>()
            .WithMessage("*Desequipe*");
    }

    [Fact]
    public void DefinirNivel_fora_de_0_5_lanca()
    {
        var habilidade = new HabilidadeDePersonagem(Guid.NewGuid());
        var acao = () => habilidade.DefinirNivel(6);
        acao.Should().Throw<ArgumentOutOfRangeException>();
    }
}
