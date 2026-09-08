using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Personagens;
using FluentAssertions;

namespace DarkestDungeon.Domain.Tests;

public class ResistenciasDeClasseTests
{
    [Fact]
    public void Valores_dentro_do_range_devem_ser_aceitos()
    {
        var r = new ResistenciasDeClasse(10, 20, 30, 40, 50, 60, 70, 80);
        r.Doenca.Should().Be(60);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Valores_fora_do_range_devem_ser_rejeitados(decimal invalido)
    {
        var criar = () => new ResistenciasDeClasse(invalido, 0, 0, 0, 0, 0, 0, 0);
        criar.Should().Throw<ArgumentOutOfRangeException>();
    }
}

public class PersonagemBasicTests
{
    [Fact]
    public void HabilidadeDePersonagem_deve_impedir_equipar_sem_treinar()
    {
        var criar = () => new HabilidadeDePersonagem(Guid.NewGuid(), habilitada: true, treinada: false, equipada: true);
        criar.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Personagem_LimiteHabilidadesDeCombate_deve_ser_6()
    {
        DarkestDungeon.Domain.Seres.Personagem.LimiteHabilidadesDeCombate.Should().Be(6);
        DarkestDungeon.Domain.Seres.Personagem.LimiteHabilidadesDeAcampamento.Should().Be(6);
    }
}
