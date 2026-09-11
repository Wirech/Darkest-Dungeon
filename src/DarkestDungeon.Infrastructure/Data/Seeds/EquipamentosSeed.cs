using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Itens;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

/// Seed idempotente das 20 armas e 20 armaduras oficiais a partir dos snapshots locais.
public static class EquipamentosSeed
{
    public static IReadOnlyList<Item> Materializar()
    {
        var snapshots = LeitorDeSnapshotsOficiais.Carregar();
        var itens = new List<Item>();
        foreach (var valor in Enum.GetValues<ClasseDeHeroi>())
        {
            if (!snapshots.TryGetValue(valor, out var snapshot))
            {
                continue;
            }

            if (snapshot.Arma.Niveis.Count != 5 || snapshot.Armadura.Niveis.Count != 5)
            {
                continue;
            }

            var nomeOriginal = snapshot.ClasseDeHeroiEnum;
            itens.Add(new Arma(
                $"Arma oficial — {valor}",
                $"Official weapon — {nomeOriginal}",
                "Tabela oficial de arma (forma humana).",
                valor,
                snapshot.Arma.Niveis.OrderBy(n => n.Nivel).Select(n =>
                    new NivelDeArma(n.Nivel, n.DanoMinimo, n.DanoMaximo, n.Critico, n.Velocidade)),
                GerarIdDeterministico(valor, 1)));

            itens.Add(new Armadura(
                $"Armadura oficial — {valor}",
                $"Official armor — {nomeOriginal}",
                "Tabela oficial de armadura (forma humana). HpAdicional = MAX HP da wiki.",
                valor,
                snapshot.Armadura.Niveis.OrderBy(n => n.Nivel).Select(n =>
                    new NivelDeArmadura(n.Nivel, n.HpMaximo, n.Esquiva)),
                GerarIdDeterministico(valor, 2)));
        }

        return itens;
    }

    public static Guid IdDeArma(ClasseDeHeroi classe) => GerarIdDeterministico(classe, 1);

    public static Guid IdDeArmadura(ClasseDeHeroi classe) => GerarIdDeterministico(classe, 2);

    private static Guid GerarIdDeterministico(ClasseDeHeroi valor, byte discriminador)
    {
        var bytes = new byte[16];
        BitConverter.GetBytes((int)valor).CopyTo(bytes, 0);
        bytes[4] = discriminador;
        bytes[6] = 0x40;
        bytes[8] = 0x80;
        bytes[9] = 0x09;
        return new Guid(bytes);
    }
}
