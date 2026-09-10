using DarkestDungeon.Domain.Itens;

namespace DarkestDungeon.Application.Midias;

public interface ICoberturaDeMidiasService
{
    Task<RelatorioDeCoberturaDeMidias> GerarAsync(CategoriaDeMidiaDeItem? categoria, CancellationToken cancellationToken);
}
