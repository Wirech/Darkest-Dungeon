using DarkestDungeon.Domain.Itens;
using FluentAssertions;

namespace DarkestDungeon.Domain.Tests.Feature006;

public sealed class ItemDeAcampamentoEConsumivelTests
{
    [Fact]
    public void ItemDeAcampamento_valido_tem_midia_pendente_por_padrao()
    {
        var item = new ItemDeAcampamento("Tocha", "torch", "Ilumina o corredor.");

        item.NomeExibicao.Should().Be("Tocha");
        item.Midia.Status.Should().Be(StatusDeMidia.Pendente);
        item.Should().NotBeAssignableTo<Acessorio>();
        item.Should().NotBeAssignableTo<Consumivel>();
    }

    [Fact]
    public void Consumivel_e_acampamento_sao_tipos_distintos()
    {
        Item acampamento = new ItemDeAcampamento("Bandagem", "bandage", "Estanca sangramento.");
        Item consumivel = new Consumivel("Dinamite", "dynamite", "Explosivo de expedição.");

        acampamento.Should().BeOfType<ItemDeAcampamento>();
        consumivel.Should().BeOfType<Consumivel>();
        acampamento.GetType().Should().NotBe(consumivel.GetType());
    }

    [Fact]
    public void Nome_maior_que_80_rejeita()
    {
        var acao = () => new ItemDeAcampamento(new string('n', 81), "ok", "desc");
        acao.Should().Throw<ArgumentException>().WithMessage("*80*");
    }

    [Fact]
    public void Descricao_maior_que_400_rejeita()
    {
        var acao = () => new Consumivel("Poção", "potion", new string('d', 401));
        acao.Should().Throw<ArgumentException>().WithMessage("*400*");
    }
}
