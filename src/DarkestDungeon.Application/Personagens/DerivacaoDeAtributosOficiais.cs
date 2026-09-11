using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Itens;
using DarkestDungeon.Domain.Personagens;
using DarkestDungeon.Domain.Seres;

namespace DarkestDungeon.Application.Personagens;

/// Fórmula fiel sem acessórios: HP/DODGE da armadura, DMG/CRIT/SPD da arma,
/// ACC/PROT 0, resistências base + 10 p.p. × nível de resolução (teto 100).
public static class DerivacaoDeAtributosOficiais
{
    public const decimal PrecisaoOficial = 0m;
    public const decimal ProtecaoOficial = 0m;
    public const int StressOficial = 0;
    public const int ChanceDeVirtudeOficial = 25;
    public const int TamanhoOficial = 1;
    public const int AcoesPorTurnoOficiais = 1;
    public const decimal BonusDeCriticoDoHeroi = 0m;
    public const int BonusDeResistenciaPorNivel = 10;

    public sealed record AtributosDerivados(
        int HpMaximo,
        int HpAtual,
        decimal Esquiva,
        int DanoBaseMinimo,
        int DanoBaseMaximo,
        decimal Critico,
        int Velocidade,
        decimal Precisao,
        decimal Protecao,
        int Stress,
        int ChanceDeVirtude,
        int Tamanho,
        int AcoesPorTurno,
        decimal BonusDeCritico,
        int PassosAFrente,
        int PassosAtras,
        Resistencias Resistencias,
        ResistenciasExtrasDePersonagem ResistenciasExtras);

    public static decimal AplicarBonusDeResolucao(decimal baseDaClasse, int nivelDeResolucao)
    {
        if (nivelDeResolucao is < 0 or > 6)
        {
            throw new ArgumentOutOfRangeException(nameof(nivelDeResolucao), "Nível de resolução deve estar entre 0 e 6.");
        }

        var efetivo = baseDaClasse + (BonusDeResistenciaPorNivel * nivelDeResolucao);
        return efetivo > 100m ? 100m : efetivo;
    }

    public static AtributosDerivados Derivar(
        Classe classe,
        NivelDeArma nivelDaArma,
        NivelDeArmadura nivelDaArmadura,
        int nivelDeResolucao)
    {
        ArgumentNullException.ThrowIfNull(classe);
        ArgumentNullException.ThrowIfNull(nivelDaArma);
        ArgumentNullException.ThrowIfNull(nivelDaArmadura);

        var resistencias = new Resistencias(
            AplicarBonusDeResolucao(classe.ResistenciasBase.Atordoamento, nivelDeResolucao),
            AplicarBonusDeResolucao(classe.ResistenciasBase.Sangramento, nivelDeResolucao),
            AplicarBonusDeResolucao(classe.ResistenciasBase.Envenenamento, nivelDeResolucao),
            AplicarBonusDeResolucao(classe.ResistenciasBase.Debuff, nivelDeResolucao),
            AplicarBonusDeResolucao(classe.ResistenciasBase.Movimento, nivelDeResolucao));

        var extras = new ResistenciasExtrasDePersonagem(
            AplicarBonusDeResolucao(classe.ResistenciasBase.Doenca, nivelDeResolucao),
            classe.ResistenciasBase.GolpeMortal,
            classe.ResistenciasBase.Armadilha);

        return new AtributosDerivados(
            nivelDaArmadura.HpAdicional,
            nivelDaArmadura.HpAdicional,
            nivelDaArmadura.Esquiva,
            nivelDaArma.DanoMinimo,
            nivelDaArma.DanoMaximo,
            nivelDaArma.Critico,
            nivelDaArma.Velocidade,
            PrecisaoOficial,
            ProtecaoOficial,
            StressOficial,
            ChanceDeVirtudeOficial,
            TamanhoOficial,
            AcoesPorTurnoOficiais,
            BonusDeCriticoDoHeroi,
            classe.PassosAFrente,
            classe.PassosAtras,
            resistencias,
            extras);
    }

    public static bool CatalogoOficialCompleto(Classe classe, Arma? arma, Armadura? armadura)
    {
        if (classe is null || !classe.PossuiDeslocamentoOficial)
        {
            return false;
        }

        return arma is { Niveis.Count: 5 } && armadura is { Niveis.Count: 5 };
    }
}
