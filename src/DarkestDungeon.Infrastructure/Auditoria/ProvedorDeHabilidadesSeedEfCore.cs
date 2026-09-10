using DarkestDungeon.Application.Auditoria;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Habilidades;
using DarkestDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DarkestDungeon.Infrastructure.Auditoria;

/// Carrega habilidades seedadas via DbContext, agrupadas pela associação Classe × Habilidade.
public sealed class ProvedorDeHabilidadesSeedEfCore : IProvedorDeHabilidadesSeed
{
    private readonly DarkestDungeonDbContext contexto;

    public ProvedorDeHabilidadesSeedEfCore(DarkestDungeonDbContext contexto)
    {
        this.contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
    }

    public async Task<IReadOnlyList<HabilidadeSeedada>> ObterHabilidadesDaClasseAsync(ClasseDeHeroi classe, CancellationToken cancellationToken)
    {
        var classeEntidade = await contexto.Classes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ClasseDeHeroi == classe, cancellationToken);
        if (classeEntidade is null)
        {
            return Array.Empty<HabilidadeSeedada>();
        }

        var idsHabilidades = await contexto.ClassesHabilidades
            .AsNoTracking()
            .Where(ch => ch.ClasseId == classeEntidade.Id)
            .Select(ch => ch.HabilidadeId)
            .ToListAsync(cancellationToken);

        if (idsHabilidades.Count == 0)
        {
            return Array.Empty<HabilidadeSeedada>();
        }

        var habilidades = await contexto.Habilidades
            .AsNoTracking()
            .Include(h => h.Niveis)
            .Where(h => idsHabilidades.Contains(h.Id))
            .ToListAsync(cancellationToken);

        var resultado = new List<HabilidadeSeedada>(habilidades.Count);
        foreach (var h in habilidades)
        {
            var qtdNiveis = h.Niveis.Count;
            var seedada = new HabilidadeSeedada(h.Id, h.NomeExibicao, h.NomeOriginal, h.Descricao, qtdNiveis);

            if (h is HabilidadeDeCombate combate)
            {
                seedada = seedada with
                {
                    Tipo = "Combate",
                    PosicoesValidas = combate.PosicoesValidas,
                    PosicoesQueAtinge = combate.PosicoesQueAtinge,
                    AlvoEmArea = combate.AlvoEmArea,
                    ModificadorDano = combate.ModificadorDano,
                    ModificadorAcerto = combate.ModificadorAcerto,
                    ModificadorCritico = combate.ModificadorCritico,
                    QuantidadeDeEfeitosSeedados = combate.Efeitos.Count,
                    NomesDeEfeitosSeedados = combate.Efeitos.Select(e => e.NomeDoEfeito).ToArray(),
                    LimitePorUsoMaximo = combate.LimitePorUso?.MaximoUsos,
                    LimitePorUsoEscopo = combate.LimitePorUso?.Escopo.ToString(),
                };
            }
            else if (h is HabilidadeDeAcampamento acamp)
            {
                seedada = seedada with
                {
                    Tipo = "Acampamento",
                    QuantidadeDeEfeitosSeedados = acamp.Efeitos.Count,
                    NomesDeEfeitosSeedados = acamp.Efeitos.Select(e => e.NomeDoEfeito).ToArray(),
                    CustoDeDescanso = acamp.CustoDeDescanso,
                    Alvo = acamp.Alvo.ToString(),
                    LimitePorUsoMaximo = acamp.LimitePorUso?.MaximoUsos,
                    LimitePorUsoEscopo = acamp.LimitePorUso?.Escopo.ToString(),
                };
            }

            resultado.Add(seedada);
        }

        return resultado;
    }
}
