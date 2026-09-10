using DarkestDungeon.Application.Midias;
using DarkestDungeon.Domain.Itens;

namespace DarkestDungeon.Infrastructure.Midias;

public static class ResolvedorDeAcessorios
{
    public static (Acessorio Item, bool Novo) ResolverOuCriar(
        IReadOnlyList<Acessorio> existentes,
        ArquivoDeInventarioDeMidia arquivo)
    {
        var stem = Path.GetFileNameWithoutExtension(arquivo.Identificador ?? arquivo.CaminhoOrigem).Trim();
        if (stem.Length > 80)
        {
            stem = stem[..80];
        }

        var existente = existentes.FirstOrDefault(a =>
            a.NomeOriginal.Equals(stem, StringComparison.OrdinalIgnoreCase));
        var midia = MidiaDePng(arquivo);
        if (existente is not null)
        {
            existente.DefinirMidia(midia);
            return (existente, false);
        }

        var novo = new Acessorio(
            stem,
            stem,
            stem,
            RaridadeDeAcessorio.Comum,
            Array.Empty<EfeitoDeAcessorio>(),
            classeExclusiva: null,
            conjuntoId: null,
            id: Guid.NewGuid());
        novo.DefinirMidia(midia);
        return (novo, true);
    }

    public static MidiaDeItem MidiaDePng(ArquivoDeInventarioDeMidia arquivo)
    {
        var id = arquivo.CaminhoDestino.Replace('\\', '/');
        if (id.Length > 200)
        {
            id = Path.GetFileName(id);
        }

        return MidiaDeItem.Ok(id, arquivo.Sha256);
    }
}
