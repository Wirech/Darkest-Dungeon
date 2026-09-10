using DarkestDungeon.Domain.Itens;
using FluentAssertions;

namespace DarkestDungeon.Domain.Tests.Feature006;

public sealed class MidiaDeItemTests
{
    [Fact]
    public void Ok_exige_arquivo_e_hash_de_64()
    {
        var hash = new string('a', 64);
        var midia = MidiaDeItem.Ok("cruzado/weapon_1.png", hash);

        midia.Status.Should().Be(StatusDeMidia.OK);
        midia.ArquivoInventarioId.Should().Be("cruzado/weapon_1.png");
        midia.HashArquivo.Should().Be(hash);
    }

    [Fact]
    public void Pendente_permite_nulos()
    {
        var midia = MidiaDeItem.Pendente();

        midia.Status.Should().Be(StatusDeMidia.Pendente);
        midia.ArquivoInventarioId.Should().BeNull();
        midia.HashArquivo.Should().BeNull();
        midia.ConjuntoSpineId.Should().BeNull();
    }

    [Fact]
    public void Construtor_rejeita_hash_diferente_de_64()
    {
        var acao = () => new MidiaDeItem("arquivo.png", null, "curto", StatusDeMidia.Pendente);
        acao.Should().Throw<ArgumentException>().WithMessage("*SHA-256*64 caracteres*");
    }

    [Fact]
    public void Ok_sem_arquivo_rejeita()
    {
        var acao = () => new MidiaDeItem(null, null, new string('b', 64), StatusDeMidia.OK);
        acao.Should().Throw<ArgumentException>().WithMessage("*OK exige*");
    }

    [Fact]
    public void ArquivoInventarioId_maior_que_200_rejeita()
    {
        var acao = () => new MidiaDeItem(new string('x', 201), null, null, StatusDeMidia.Pendente);
        acao.Should().Throw<ArgumentException>().WithMessage("*200*");
    }
}
