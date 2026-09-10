using DarkestDungeon.Application.Auditoria;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Cobertura;
using DarkestDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DarkestDungeon.Infrastructure.Auditoria;

/// Conta assets Classe × Aparência agregando pelo `Status` (Coletado/Pendente) via join com Classes.
public sealed class ProvedorDeAssetsAuditadosEfCore : IProvedorDeAssetsAuditados
{
    private readonly DarkestDungeonDbContext contexto;

    public ProvedorDeAssetsAuditadosEfCore(DarkestDungeonDbContext contexto)
    {
        this.contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
    }

    public async Task<(int coletados, int pendentes)> ContarAssetsAsync(CancellationToken cancellationToken)
    {
        var classesComAssets = await contexto.Classes
            .Include(c => c.Assets)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        int coletados = classesComAssets.SelectMany(c => c.Assets).Count(a => a.Status == EstadoDeAtributo.Coletado);
        int pendentes = classesComAssets.SelectMany(c => c.Assets).Count(a => a.Status == EstadoDeAtributo.Pendente);
        return (coletados, pendentes);
    }
}
