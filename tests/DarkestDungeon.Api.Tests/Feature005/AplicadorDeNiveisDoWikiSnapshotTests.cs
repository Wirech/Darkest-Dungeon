using DarkestDungeon.Infrastructure.Data.Seeds;
using Xunit;

namespace DarkestDungeon.Api.Tests.Feature005;

public sealed class AplicadorDeNiveisDoWikiSnapshotTests
{
    [Fact]
    public void Aplicar_Popula_5_Niveis_Em_Todas_As_Habilidades_Seedadas()
    {
        var habilidades = HabilidadesSeed.Materializar();

        var resultado = AplicadorDeNiveisDoWikiSnapshot.Aplicar(habilidades);

        Assert.All(habilidades, h => Assert.Equal(5, h.Niveis.Count));
        Assert.All(habilidades, h => Assert.Equal(
            new[] { 1, 2, 3, 4, 5 },
            h.Niveis.Select(n => n.NumeroDoNivel).ToArray()));
        Assert.True(resultado.Aplicadas + resultado.Fallback == habilidades.Count);
    }

    [Fact]
    public void Aplicar_Deriva_Progressao_Real_De_Cruzado_Smite()
    {
        var habilidades = HabilidadesSeed.Materializar();
        AplicadorDeNiveisDoWikiSnapshot.Aplicar(habilidades);

        var smite = habilidades.Single(h => h.NomeOriginal == "Smite");

        // Smite: value=+0% em todos os níveis; accuracy 85→105; crit 0→4%.
        Assert.Equal(85m, smite.Niveis[0].ModificadorAcerto);
        Assert.Equal(90m, smite.Niveis[1].ModificadorAcerto);
        Assert.Equal(95m, smite.Niveis[2].ModificadorAcerto);
        Assert.Equal(100m, smite.Niveis[3].ModificadorAcerto);
        Assert.Equal(105m, smite.Niveis[4].ModificadorAcerto);
        Assert.Equal(0m, smite.Niveis[0].ModificadorDano);
        Assert.Equal(0m, smite.Niveis[4].ModificadorDano);
    }

    [Fact]
    public void Aplicar_Popula_ValoresDeEfeito_De_Punir_Flagelante()
    {
        var habilidades = HabilidadesSeed.Materializar();
        AplicadorDeNiveisDoWikiSnapshot.Aplicar(habilidades);

        var punish = habilidades.Single(h => h.NomeOriginal == "Punish");

        // Punish tem Sangramento + DebuffResistenciaSangramento por 5 níveis; verifica presença.
        Assert.All(punish.Niveis, n => Assert.True(n.ValoresDeEfeito.Count >= 2, $"Nivel {n.NumeroDoNivel} tem {n.ValoresDeEfeito.Count} valores"));
    }

    [Fact]
    public void Aplicar_Camping_Skills_Replica_Custo_Descanso_Nos_5_Niveis()
    {
        var habilidades = HabilidadesSeed.Materializar();
        AplicadorDeNiveisDoWikiSnapshot.Aplicar(habilidades);

        // Zealous Vigil (Cruzado): custoDeDescanso=4 no snapshot; replicado 5x.
        var vigilia = habilidades.Single(h => h.NomeOriginal == "Zealous Vigil");
        Assert.All(vigilia.Niveis, n => Assert.Equal(4, n.CustoDeDescanso));
    }
}
