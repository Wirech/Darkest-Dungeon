using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Domain.Itens;

namespace DarkestDungeon.Application.Midias;

public sealed class CoberturaDeMidiasService : ICoberturaDeMidiasService
{
    private readonly IItemRepository itens;
    private readonly ILeitorDeInventarioDeMidias leitor;

    public CoberturaDeMidiasService(IItemRepository itens, ILeitorDeInventarioDeMidias leitor)
    {
        this.itens = itens;
        this.leitor = leitor;
    }

    public async Task<RelatorioDeCoberturaDeMidias> GerarAsync(CategoriaDeMidiaDeItem? categoria, CancellationToken cancellationToken)
    {
        var inventario = await leitor.LerAsync(cancellationToken);
        var catalogo = await itens.ListarTodosAsync(cancellationToken);

        var categoriasOficiais = Enum.GetValues<CategoriaDeMidiaDeItem>();
        var escolhidas = categoria is { } filtro ? [filtro] : categoriasOficiais;
        var resumos = new List<ResumoDeCategoriaDeMidia>();
        foreach (var cat in escolhidas)
        {
            resumos.Add(Resumo(cat, catalogo));
        }

        var orfaos = (inventario?.Arquivos ?? [])
            .Where(a => string.Equals(a.Categoria, "NaoAssociado", StringComparison.OrdinalIgnoreCase))
            .Select(a => new OrfaoDeMidia(a.CaminhoOrigem, a.Sha256, a.CaminhoDestino))
            .ToArray();

        var lacunas = inventario?.Lacunas ?? [];
        return new RelatorioDeCoberturaDeMidias(DateTimeOffset.UtcNow, resumos, orfaos, lacunas);
    }

    private static ResumoDeCategoriaDeMidia Resumo(CategoriaDeMidiaDeItem categoria, IReadOnlyList<Item> catalogo)
    {
        var linhas = categoria switch
        {
            CategoriaDeMidiaDeItem.Arma => catalogo.OfType<Arma>().Select(LinhaArma).ToArray(),
            CategoriaDeMidiaDeItem.Armadura => catalogo.OfType<Armadura>().Select(LinhaArmadura).ToArray(),
            CategoriaDeMidiaDeItem.Acessorio => catalogo.OfType<Acessorio>().Select(LinhaSimples).ToArray(),
            CategoriaDeMidiaDeItem.ItemDeAcampamento => catalogo.OfType<ItemDeAcampamento>().Select(LinhaSimples).ToArray(),
            CategoriaDeMidiaDeItem.Consumivel => catalogo.OfType<Consumivel>().Select(LinhaSimples).ToArray(),
            _ => [],
        };

        var esperados = categoria is CategoriaDeMidiaDeItem.Arma or CategoriaDeMidiaDeItem.Armadura
            ? Math.Max(20, linhas.Length)
            : linhas.Length;

        return new ResumoDeCategoriaDeMidia(
            categoria,
            esperados,
            linhas.Count(l => l.Status == StatusDeCoberturaDeItem.OK),
            linhas.Count(l => l.Status == StatusDeCoberturaDeItem.Parcial),
            Math.Max(0, esperados - linhas.Count(l => l.Status is StatusDeCoberturaDeItem.OK or StatusDeCoberturaDeItem.Parcial)),
            linhas);
    }

    private static LinhaDeCoberturaDeItem LinhaArma(Arma arma)
    {
        var ok = arma.Niveis.Where(n => n.Midia.Status == StatusDeMidia.OK).Select(n => n.Nivel).OrderBy(n => n).ToArray();
        var pendentes = arma.Niveis.Where(n => n.Midia.Status != StatusDeMidia.OK).Select(n => n.Nivel).OrderBy(n => n).ToArray();
        return new LinhaDeCoberturaDeItem(arma.Id, arma.NomeExibicao, arma.NomeOriginal, StatusNiveis(ok.Length), ok, pendentes);
    }

    private static LinhaDeCoberturaDeItem LinhaArmadura(Armadura armadura)
    {
        var ok = armadura.Niveis.Where(n => n.Midia.Status == StatusDeMidia.OK).Select(n => n.Nivel).OrderBy(n => n).ToArray();
        var pendentes = armadura.Niveis.Where(n => n.Midia.Status != StatusDeMidia.OK).Select(n => n.Nivel).OrderBy(n => n).ToArray();
        return new LinhaDeCoberturaDeItem(armadura.Id, armadura.NomeExibicao, armadura.NomeOriginal, StatusNiveis(ok.Length), ok, pendentes);
    }

    private static LinhaDeCoberturaDeItem LinhaSimples(Item item)
    {
        var midia = item switch
        {
            Acessorio a => a.Midia,
            ItemDeAcampamento i => i.Midia,
            Consumivel c => c.Midia,
            _ => MidiaDeItem.Pendente(),
        };
        var status = midia.Status == StatusDeMidia.OK ? StatusDeCoberturaDeItem.OK : StatusDeCoberturaDeItem.Pendente;
        return new LinhaDeCoberturaDeItem(item.Id, item.NomeExibicao, item.NomeOriginal, status, null, null);
    }

    private static StatusDeCoberturaDeItem StatusNiveis(int ok) => ok switch
    {
        5 => StatusDeCoberturaDeItem.OK,
        0 => StatusDeCoberturaDeItem.Pendente,
        _ => StatusDeCoberturaDeItem.Parcial,
    };

}
