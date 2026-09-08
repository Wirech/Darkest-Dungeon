using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Habilidades;
using FluentAssertions;

namespace DarkestDungeon.Domain.Tests;

public class ClasseTests
{
    private static ResistenciasDeClasse ResistenciasPadrao() => new(10, 10, 10, 10, 10, 10, 10, 10);

    [Fact]
    public void Classe_valida_deve_ser_criada_com_atributos()
    {
        var classe = new Classe(ClasseDeHeroi.Cruzado, "Cruzado", "Crusader", ResistenciasPadrao());

        classe.ClasseDeHeroi.Should().Be(ClasseDeHeroi.Cruzado);
        classe.NomeExibicao.Should().Be("Cruzado");
        classe.NomeOriginal.Should().Be("Crusader");
        classe.ResistenciasBase.Should().NotBeNull();
    }

    [Fact]
    public void Classe_com_nome_muito_longo_deve_ser_rejeitada()
    {
        var criar = () => new Classe(ClasseDeHeroi.Cruzado, new string('A', 61), "Original", ResistenciasPadrao());

        criar.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ClasseHabilidade_deve_rejeitar_ids_vazios()
    {
        var criarClasse = () => new ClasseHabilidade(Guid.Empty, Guid.NewGuid());
        var criarHabilidade = () => new ClasseHabilidade(Guid.NewGuid(), Guid.Empty);

        criarClasse.Should().Throw<ArgumentException>();
        criarHabilidade.Should().Throw<ArgumentException>();
    }
}
