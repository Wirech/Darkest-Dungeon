using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Personagens;
using DarkestDungeon.Domain.Seres;
using FluentAssertions;
using Xunit;

namespace DarkestDungeon.Domain.Tests.Feature005;

public sealed class PersonagemFeature005Tests
{
    private static Personagem CriarCruzadoBase()
    {
        return new Personagem(
            nome: "Reinaldo o Impávido",
            classe: ClasseDeHeroi.Cruzado,
            hpMaximo: 33,
            hpAtual: 33,
            velocidade: 4,
            critico: 3.5m,
            danoBaseMinimo: 6,
            danoBaseMaximo: 12,
            movimento: 2,
            bonusDeCritico: 5m,
            tamanho: 1,
            acoesPorTurno: 1,
            esquiva: 10m,
            precisao: 90m,
            protecao: 0m,
            nivel: 0,
            resistencias: new Resistencias(30m, 20m, 30m, 30m, 30m),
            resistenciasExtras: new ResistenciasExtrasDePersonagem(0m, 67m, 30m),
            stress: 0,
            chanceDeVirtude: 25);
    }

    [Fact]
    public void Novo_Personagem_recebe_aparencia_A_por_padrao()
    {
        var personagem = CriarCruzadoBase();
        personagem.Aparencia.Should().Be(AparenciaDePersonagem.A);
    }

    [Fact]
    public void Novo_Personagem_recebe_Experiencia_zero_e_Nivel_Curioso()
    {
        var personagem = CriarCruzadoBase();
        personagem.Experiencia.Should().Be(0);
        personagem.NivelDeResolucao.Nome.Should().Be("Curioso");
        personagem.NivelDeResolucao.NomeOriginal.Should().Be("Seeker");
        personagem.NivelDeResolucao.Valor.Should().Be(0);
    }

    [Fact]
    public void DefinirAparencia_aceita_A_B_C_D()
    {
        var personagem = CriarCruzadoBase();
        personagem.DefinirAparencia(AparenciaDePersonagem.C);
        personagem.Aparencia.Should().Be(AparenciaDePersonagem.C);
    }

    [Fact]
    public void DefinirAparencia_fora_do_enum_lanca()
    {
        var personagem = CriarCruzadoBase();
        var acao = () => personagem.DefinirAparencia((AparenciaDePersonagem)99);
        acao.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Aparência inválida*");
    }

    [Fact]
    public void GanharExperiencia_com_2_XP_sobe_para_Aprendiz_e_aplica_bonus_10pct()
    {
        var personagem = CriarCruzadoBase();
        var atordoamentoAntes = personagem.Resistencias.Atordoamento;
        var armadilhaAntes = personagem.ResistenciasExtras.Armadilha;

        personagem.GanharExperiencia(2);

        personagem.NivelDeResolucao.Nome.Should().Be("Aprendiz");
        personagem.NivelDeResolucao.NomeOriginal.Should().Be("Apprentice");
        personagem.NivelDeResolucao.Valor.Should().Be(1);
        personagem.Resistencias.Atordoamento.Should().Be(atordoamentoAntes + 10m);
        personagem.Resistencias.Sangramento.Should().BeGreaterThan(0m);
        personagem.ResistenciasExtras.Armadilha.Should().Be(armadilhaAntes + 10m);
    }

    [Fact]
    public void GanharExperiencia_ate_48_sobe_para_Lenda_com_bonus_60pct()
    {
        var personagem = CriarCruzadoBase();
        var atordoamentoAntes = personagem.Resistencias.Atordoamento;

        personagem.GanharExperiencia(48);

        personagem.NivelDeResolucao.Nome.Should().Be("Lenda");
        personagem.NivelDeResolucao.NomeOriginal.Should().Be("Legend");
        personagem.NivelDeResolucao.Valor.Should().Be(6);
        personagem.Resistencias.Atordoamento.Should().Be(atordoamentoAntes + 60m);
    }

    [Fact]
    public void GanharExperiencia_XP_negativo_lanca()
    {
        var personagem = CriarCruzadoBase();
        var acao = () => personagem.GanharExperiencia(-1);
        acao.Should().Throw<ArgumentOutOfRangeException>();
    }
}
