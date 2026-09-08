using System.ComponentModel;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Cobertura;
using DarkestDungeon.Domain.Habilidades;
using DarkestDungeon.Domain.Itens;
using DarkestDungeon.Domain.Seres;
using FluentAssertions;

namespace DarkestDungeon.Domain.Tests;

public class EnumsDoCatalogoTests
{
    [Fact]
    public void ClasseDeHeroi_deve_conter_exatamente_20_valores()
    {
        Enum.GetValues<ClasseDeHeroi>().Length.Should().Be(20);
    }

    [Theory]
    [InlineData(ClasseDeHeroi.Cruzado, "Crusader")]
    [InlineData(ClasseDeHeroi.Bandido, "Highwayman")]
    [InlineData(ClasseDeHeroi.Ocultista, "Occultist")]
    [InlineData(ClasseDeHeroi.LadraoDeCova, "Grave Robber")]
    public void ClasseDeHeroi_deve_manter_nome_original_em_ingles_em_Description(ClasseDeHeroi classe, string original)
    {
        var membro = typeof(ClasseDeHeroi).GetMember(classe.ToString()).Single();
        var atributo = membro.GetCustomAttributes(typeof(DescriptionAttribute), false).Single() as DescriptionAttribute;

        atributo.Should().NotBeNull();
        atributo!.Description.Should().Be(original);
    }

    [Fact]
    public void TipoDeInimigo_deve_conter_exatamente_7_valores()
    {
        Enum.GetValues<TipoDeInimigo>().Length.Should().Be(7);
    }

    [Fact]
    public void AlvoDeEfeito_UnidadeDeEfeito_EscopoDeLimite_AlvoDeAcampamento_devem_existir()
    {
        Enum.GetValues<AlvoDeEfeito>().Should().Contain(new[] { AlvoDeEfeito.Self, AlvoDeEfeito.Aliado, AlvoDeEfeito.Inimigo });
        Enum.GetValues<UnidadeDeEfeito>().Should().Contain(new[] { UnidadeDeEfeito.Percentual, UnidadeDeEfeito.Pontos, UnidadeDeEfeito.Rodadas });
        Enum.GetValues<EscopoDeLimite>().Should().Contain(new[] { EscopoDeLimite.Batalha, EscopoDeLimite.Acampamento });
        Enum.GetValues<AlvoDeAcampamento>().Length.Should().Be(4);
    }

    [Fact]
    public void RaridadeDeAcessorio_deve_conter_exatamente_7_valores_fechados()
    {
        var raridades = Enum.GetValues<RaridadeDeAcessorio>();
        raridades.Length.Should().Be(7);
        raridades.Should().Contain(new[]
        {
            RaridadeDeAcessorio.Comum,
            RaridadeDeAcessorio.Incomum,
            RaridadeDeAcessorio.Rara,
            RaridadeDeAcessorio.MuitoRara,
            RaridadeDeAcessorio.CrimsonCourt,
            RaridadeDeAcessorio.Crystalline,
            RaridadeDeAcessorio.Set,
        });
    }

    [Fact]
    public void SinalDeEfeito_deve_conter_Positivo_e_Negativo()
    {
        Enum.GetValues<SinalDeEfeito>().Should().Contain(new[] { SinalDeEfeito.Positivo, SinalDeEfeito.Negativo });
    }

    [Fact]
    public void CategoriaDeCobertura_e_EstadoDeAtributo_devem_ter_valores_esperados()
    {
        Enum.GetValues<CategoriaDeCobertura>().Length.Should().Be(3);
        Enum.GetValues<EstadoDeAtributo>().Should().Contain(new[]
        {
            EstadoDeAtributo.Coletado,
            EstadoDeAtributo.Pendente,
            EstadoDeAtributo.NaoAplicavel,
        });
    }
}
