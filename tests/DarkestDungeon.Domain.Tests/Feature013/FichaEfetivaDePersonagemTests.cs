using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Itens;
using DarkestDungeon.Domain.Personagens;
using DarkestDungeon.Domain.Seres;
using FluentAssertions;

namespace DarkestDungeon.Domain.Tests.Feature013;

public sealed class FichaEfetivaDePersonagemTests
{
    [Fact]
    public void Trinta_hp_mais_dez_porcento_duas_vezes_deve_ser_36()
    {
        var personagem = Criar(hp: 30);
        var a = AcessorioCom(new EfeitoDeAcessorio("MAX HP", 10m, UnidadeDeEfeitoDeAcessorio.Percentual, SinalDeEfeito.Positivo));
        var b = AcessorioCom(new EfeitoDeAcessorio("MAX HP", 10m, UnidadeDeEfeitoDeAcessorio.Percentual, SinalDeEfeito.Positivo));

        FichaEfetivaDePersonagem.Calcular(personagem, [a, b]).HpMaximo.Should().Be(36);
    }

    [Fact]
    public void Vinte_e_tres_hp_mais_dez_porcento_deve_teto_26()
    {
        var personagem = Criar(hp: 23);
        var a = AcessorioCom(new EfeitoDeAcessorio("MAX HP", 10m, UnidadeDeEfeitoDeAcessorio.Percentual, SinalDeEfeito.Positivo));

        FichaEfetivaDePersonagem.Calcular(personagem, [a]).HpMaximo.Should().Be(26);
    }

    [Fact]
    public void Prot_zero_mais_dez_pontos_percentuais_deve_ser_10()
    {
        var personagem = Criar(hp: 23, protecao: 0m);
        var a = AcessorioCom(new EfeitoDeAcessorio("PROT", 10m, UnidadeDeEfeitoDeAcessorio.Percentual, SinalDeEfeito.Positivo));

        FichaEfetivaDePersonagem.Calcular(personagem, [a]).Protecao.Should().Be(10m);
    }

    [Fact]
    public void Efeito_nao_mapeado_e_ignorado()
    {
        var personagem = Criar(hp: 30);
        var a = AcessorioCom(new EfeitoDeAcessorio("Scouting", 20m, UnidadeDeEfeitoDeAcessorio.Percentual, SinalDeEfeito.Positivo));

        var ficha = FichaEfetivaDePersonagem.Calcular(personagem, [a]);
        ficha.HpMaximo.Should().Be(30);
        ficha.Precisao.Should().Be(personagem.Precisao);
    }

    [Fact]
    public void Resistencia_deve_ficar_entre_0_e_100_e_hp_maximo_pelo_menos_1()
    {
        var personagem = Criar(hp: 5);
        var a = AcessorioCom(
            new EfeitoDeAcessorio("Stun", 200m, UnidadeDeEfeitoDeAcessorio.Percentual, SinalDeEfeito.Positivo),
            new EfeitoDeAcessorio("MAX HP", 100m, UnidadeDeEfeitoDeAcessorio.Percentual, SinalDeEfeito.Negativo));

        var ficha = FichaEfetivaDePersonagem.Calcular(personagem, [a]);
        ficha.Resistencias.Atordoamento.Should().Be(100m);
        ficha.HpMaximo.Should().BeGreaterThanOrEqualTo(1);
    }

    private static Personagem Criar(int hp, decimal protecao = 0m) => new(
        "Teste",
        ClasseDeHeroi.Cruzado,
        hp,
        hp,
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
        protecao,
        0,
        new Resistencias(40, 40, 40, 40, 40),
        new ResistenciasExtrasDePersonagem(30, 67, 10),
        0,
        25);

    private static Acessorio AcessorioCom(params EfeitoDeAcessorio[] efeitos) => new(
        "T",
        "T",
        "T",
        RaridadeDeAcessorio.Comum,
        efeitos);
}
