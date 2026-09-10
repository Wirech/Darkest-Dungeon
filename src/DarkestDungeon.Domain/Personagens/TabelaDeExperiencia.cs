namespace DarkestDungeon.Domain.Personagens;

/// Tabela oficial de limiares de XP para subida de `NivelDeResolucao`.
/// **Modo único** — o sistema opera apenas no modo mais difícil (equivalente a Darkest/Stygian oficial).
/// Valores canônicos da wiki: `{2, 8, 14, 24, 36, 48}` para atingir os níveis 1..6 respectivamente.
public static class TabelaDeExperiencia
{
    /// Limiares acumulados de XP para atingir cada nível de Resolução.
    /// Índice 0 = XP para atingir Nível 1; índice 5 = XP para atingir Nível 6.
    public static readonly IReadOnlyList<int> Limiares = new[] { 2, 8, 14, 24, 36, 48 };

    /// Resolve o `NivelDeResolucao` atual a partir do XP acumulado.
    public static NivelDeResolucao Resolver(int experiencia)
    {
        if (experiencia < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(experiencia), "Experiência deve ser >= 0.");
        }

        int nivelAtingido = 0;
        for (int indice = 0; indice < Limiares.Count; indice++)
        {
            if (experiencia >= Limiares[indice])
            {
                nivelAtingido = indice + 1;
            }
            else
            {
                break;
            }
        }

        return NivelDeResolucao.De(nivelAtingido);
    }

    /// Quantos pontos de XP faltam para o próximo nível. Retorna 0 se já está no nível 6.
    public static int XpFaltandoParaProximoNivel(int experiencia)
    {
        if (experiencia < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(experiencia), "Experiência deve ser >= 0.");
        }

        foreach (var limiar in Limiares)
        {
            if (experiencia < limiar)
            {
                return limiar - experiencia;
            }
        }

        return 0;
    }
}
