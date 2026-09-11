using DarkestDungeon.Domain.Itens;

namespace DarkestDungeon.Application.Personagens;

public enum CampoDeFichaDeTrinket
{
    HpMaximo,
    Dano,
    Protecao,
    Critico,
    Precisao,
    Esquiva,
    Velocidade,
    ChanceDeVirtude,
    ResistenciaAtordoamento,
    ResistenciaSangramento,
    ResistenciaEnvenenamento,
    ResistenciaDebuff,
    ResistenciaMovimento,
    ResistenciaDoenca,
    ResistenciaGolpeMortal,
    ResistenciaArmadilha,
}

public enum ModoDeAplicacaoDeTrinket
{
    PercentualDaBase,
    PontosPercentuais,
    Pontos,
}

public sealed record MapeamentoDeEfeitoDeTrinket(
    CampoDeFichaDeTrinket Campo,
    ModoDeAplicacaoDeTrinket Modo,
    UnidadeDeEfeitoDeAcessorio UnidadeEsperada);

public static class MapaDeEfeitoDeTrinket
{
    private static readonly Dictionary<string, MapeamentoDeEfeitoDeTrinket> Mapa = new(StringComparer.OrdinalIgnoreCase)
    {
        ["MAX HP"] = Pct(CampoDeFichaDeTrinket.HpMaximo),
        ["HP"] = Pct(CampoDeFichaDeTrinket.HpMaximo),
        ["MAXHP"] = Pct(CampoDeFichaDeTrinket.HpMaximo),
        ["DMG"] = Pct(CampoDeFichaDeTrinket.Dano),
        ["Damage"] = Pct(CampoDeFichaDeTrinket.Dano),
        ["Dano"] = Pct(CampoDeFichaDeTrinket.Dano),
        ["PROT"] = Pp(CampoDeFichaDeTrinket.Protecao),
        ["Protection"] = Pp(CampoDeFichaDeTrinket.Protecao),
        ["Proteção"] = Pp(CampoDeFichaDeTrinket.Protecao),
        ["Protecao"] = Pp(CampoDeFichaDeTrinket.Protecao),
        ["CRIT"] = Pp(CampoDeFichaDeTrinket.Critico),
        ["Critical"] = Pp(CampoDeFichaDeTrinket.Critico),
        ["Crítico"] = Pp(CampoDeFichaDeTrinket.Critico),
        ["Critico"] = Pp(CampoDeFichaDeTrinket.Critico),
        ["ACC"] = Pts(CampoDeFichaDeTrinket.Precisao),
        ["Accuracy"] = Pts(CampoDeFichaDeTrinket.Precisao),
        ["Precisão"] = Pts(CampoDeFichaDeTrinket.Precisao),
        ["Precisao"] = Pts(CampoDeFichaDeTrinket.Precisao),
        ["DODGE"] = Pts(CampoDeFichaDeTrinket.Esquiva),
        ["Dodge"] = Pts(CampoDeFichaDeTrinket.Esquiva),
        ["Esquiva"] = Pts(CampoDeFichaDeTrinket.Esquiva),
        ["SPD"] = Pts(CampoDeFichaDeTrinket.Velocidade),
        ["Speed"] = Pts(CampoDeFichaDeTrinket.Velocidade),
        ["Velocidade"] = Pts(CampoDeFichaDeTrinket.Velocidade),
        ["Virtue Chance"] = Pp(CampoDeFichaDeTrinket.ChanceDeVirtude),
        ["Virtue"] = Pp(CampoDeFichaDeTrinket.ChanceDeVirtude),
        ["Chance de virtude"] = Pp(CampoDeFichaDeTrinket.ChanceDeVirtude),
        ["Stun"] = Pp(CampoDeFichaDeTrinket.ResistenciaAtordoamento),
        ["Stun Resist"] = Pp(CampoDeFichaDeTrinket.ResistenciaAtordoamento),
        ["Atordoamento"] = Pp(CampoDeFichaDeTrinket.ResistenciaAtordoamento),
        ["Bleed"] = Pp(CampoDeFichaDeTrinket.ResistenciaSangramento),
        ["Bleed Resist"] = Pp(CampoDeFichaDeTrinket.ResistenciaSangramento),
        ["Sangramento"] = Pp(CampoDeFichaDeTrinket.ResistenciaSangramento),
        ["Blight"] = Pp(CampoDeFichaDeTrinket.ResistenciaEnvenenamento),
        ["Blight Resist"] = Pp(CampoDeFichaDeTrinket.ResistenciaEnvenenamento),
        ["Envenenamento"] = Pp(CampoDeFichaDeTrinket.ResistenciaEnvenenamento),
        ["Debuff"] = Pp(CampoDeFichaDeTrinket.ResistenciaDebuff),
        ["Debuff Resist"] = Pp(CampoDeFichaDeTrinket.ResistenciaDebuff),
        ["Move"] = Pp(CampoDeFichaDeTrinket.ResistenciaMovimento),
        ["Move Resist"] = Pp(CampoDeFichaDeTrinket.ResistenciaMovimento),
        ["Movimento"] = Pp(CampoDeFichaDeTrinket.ResistenciaMovimento),
        ["Disease"] = Pp(CampoDeFichaDeTrinket.ResistenciaDoenca),
        ["Disease Resist"] = Pp(CampoDeFichaDeTrinket.ResistenciaDoenca),
        ["Doença"] = Pp(CampoDeFichaDeTrinket.ResistenciaDoenca),
        ["Doenca"] = Pp(CampoDeFichaDeTrinket.ResistenciaDoenca),
        ["Death Blow"] = Pp(CampoDeFichaDeTrinket.ResistenciaGolpeMortal),
        ["Death Blow Resist"] = Pp(CampoDeFichaDeTrinket.ResistenciaGolpeMortal),
        ["Golpe Mortal"] = Pp(CampoDeFichaDeTrinket.ResistenciaGolpeMortal),
        ["Trap"] = Pp(CampoDeFichaDeTrinket.ResistenciaArmadilha),
        ["Trap Resist"] = Pp(CampoDeFichaDeTrinket.ResistenciaArmadilha),
        ["Armadilha"] = Pp(CampoDeFichaDeTrinket.ResistenciaArmadilha),
    };

    public static bool TentarMapear(string nome, UnidadeDeEfeitoDeAcessorio unidade, out MapeamentoDeEfeitoDeTrinket mapeamento)
    {
        mapeamento = null!;
        if (string.IsNullOrWhiteSpace(nome))
        {
            return false;
        }

        if (!Mapa.TryGetValue(nome.Trim(), out var encontrado))
        {
            return false;
        }

        if (encontrado.UnidadeEsperada != unidade)
        {
            return false;
        }

        mapeamento = encontrado;
        return true;
    }

    public static bool NomeReconhecido(string nome) =>
        !string.IsNullOrWhiteSpace(nome) && Mapa.ContainsKey(nome.Trim());

    public static UnidadeDeEfeitoDeAcessorio? UnidadeEsperada(string nome) =>
        Mapa.TryGetValue(nome.Trim(), out var map) ? map.UnidadeEsperada : null;

    private static MapeamentoDeEfeitoDeTrinket Pct(CampoDeFichaDeTrinket campo) =>
        new(campo, ModoDeAplicacaoDeTrinket.PercentualDaBase, UnidadeDeEfeitoDeAcessorio.Percentual);

    private static MapeamentoDeEfeitoDeTrinket Pp(CampoDeFichaDeTrinket campo) =>
        new(campo, ModoDeAplicacaoDeTrinket.PontosPercentuais, UnidadeDeEfeitoDeAcessorio.Percentual);

    private static MapeamentoDeEfeitoDeTrinket Pts(CampoDeFichaDeTrinket campo) =>
        new(campo, ModoDeAplicacaoDeTrinket.Pontos, UnidadeDeEfeitoDeAcessorio.Pontos);
}
