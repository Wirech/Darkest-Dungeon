using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Cobertura;
using DarkestDungeon.Domain.Personagens;
using FluentAssertions;
using Xunit;

namespace DarkestDungeon.Domain.Tests.Feature005;

public sealed class AssetsDeClasseTests
{
    [Fact]
    public void Construtor_aceita_status_Coletado_com_ConjuntoSpineId_e_HashArquivo()
    {
        var hashValido = new string('a', 64);
        var asset = new AssetsDeClasse(AparenciaDePersonagem.A, "arquivos/cruzado/A/crusader_portrait.png", hashValido, EstadoDeAtributo.Coletado);

        asset.Aparencia.Should().Be(AparenciaDePersonagem.A);
        asset.ConjuntoSpineId.Should().Be("arquivos/cruzado/A/crusader_portrait.png");
        asset.HashArquivo.Should().Be(hashValido);
        asset.Status.Should().Be(EstadoDeAtributo.Coletado);
    }

    [Fact]
    public void Construtor_aceita_status_Pendente_com_campos_nulos()
    {
        var asset = new AssetsDeClasse(AparenciaDePersonagem.C, null, null, EstadoDeAtributo.Pendente);

        asset.ConjuntoSpineId.Should().BeNull();
        asset.HashArquivo.Should().BeNull();
        asset.Status.Should().Be(EstadoDeAtributo.Pendente);
    }

    [Fact]
    public void Construtor_rejeita_HashArquivo_diferente_de_64_caracteres()
    {
        var acao = () => new AssetsDeClasse(AparenciaDePersonagem.A, "path", "hash-curto", EstadoDeAtributo.Coletado);
        acao.Should().Throw<ArgumentException>()
            .WithMessage("*SHA-256*64 caracteres*");
    }
}
