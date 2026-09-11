using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Personagens;
using DarkestDungeon.Domain.Seres;
using FluentAssertions;

namespace DarkestDungeon.Domain.Tests;

public sealed class PersonagemCriacaoCompletaTests
{
    [Fact]
    public void Personagem_deve_preservar_niveis_de_equipamento_e_aparencia()
    {
        var personagem = new Personagem(
            "Cruzado",
            ClasseDeHeroi.Cruzado,
            30,
            30,
            4,
            5,
            6,
            10,
            2,
            3,
            1,
            1,
            10,
            5,
            0,
            3,
            new Resistencias(10, 10, 10, 10, 10),
            new ResistenciasExtrasDePersonagem(10, 10, 10),
            0,
            25,
            nivelDaArma: 2,
            nivelDaArmadura: 3,
            aparencia: AparenciaDePersonagem.C);

        personagem.NivelDaArma.Should().Be(2);
        personagem.NivelDaArmadura.Should().Be(3);
        personagem.Aparencia.Should().Be(AparenciaDePersonagem.C);
    }

    [Fact]
    public void Habilidade_nivel_zero_fica_bloqueada_e_nivel_um_fica_treinada()
    {
        var bloqueada = new HabilidadeDePersonagem(Guid.NewGuid(), numeroDoNivel: 0);
        var treinada = new HabilidadeDePersonagem(Guid.NewGuid(), numeroDoNivel: 1);

        bloqueada.Treinada.Should().BeFalse();
        bloqueada.TreinadaPorNivel.Should().BeFalse();
        treinada.Treinada.Should().BeTrue();
        treinada.TreinadaPorNivel.Should().BeTrue();
    }
}
