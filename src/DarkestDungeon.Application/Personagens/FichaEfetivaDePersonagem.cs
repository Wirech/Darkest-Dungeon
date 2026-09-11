using DarkestDungeon.Domain.Itens;
using DarkestDungeon.Domain.Seres;

namespace DarkestDungeon.Application.Personagens;

public static class FichaEfetivaDePersonagem
{
    public static FichaDePersonagemDto DaBase(Personagem personagem) =>
        new(
            personagem.HpMaximo,
            personagem.HpAtual,
            personagem.Precisao,
            personagem.Protecao,
            personagem.Esquiva,
            personagem.Velocidade,
            personagem.Critico,
            personagem.DanoBaseMinimo,
            personagem.DanoBaseMaximo,
            personagem.ChanceDeVirtude,
            new ResistenciasDePersonagemDto(
                personagem.Resistencias.Atordoamento,
                personagem.Resistencias.Sangramento,
                personagem.Resistencias.Envenenamento,
                personagem.Resistencias.Debuff,
                personagem.Resistencias.Movimento,
                personagem.ResistenciasExtras.Doenca,
                personagem.ResistenciasExtras.GolpeMortal,
                personagem.ResistenciasExtras.Armadilha));

    public static FichaDePersonagemDto Calcular(Personagem personagem, IEnumerable<Acessorio?> acessorios)
    {
        var baseFicha = DaBase(personagem);
        var efeitos = (acessorios ?? Array.Empty<Acessorio?>())
            .Where(a => a is not null)
            .SelectMany(a => a!.Efeitos)
            .ToArray();

        if (efeitos.Length == 0)
        {
            return baseFicha;
        }

        var hpMax = TetoInteiro(baseFicha.HpMaximo + Somar(efeitos, CampoDeFichaDeTrinket.HpMaximo, baseFicha.HpMaximo));
        hpMax = Math.Max(1, hpMax);
        var hpAtual = Math.Clamp(personagem.HpAtual, 1, hpMax);
        var danoMin = TetoInteiro(baseFicha.DanoBaseMinimo + Somar(efeitos, CampoDeFichaDeTrinket.Dano, baseFicha.DanoBaseMinimo));
        var danoMax = TetoInteiro(baseFicha.DanoBaseMaximo + Somar(efeitos, CampoDeFichaDeTrinket.Dano, baseFicha.DanoBaseMaximo));
        var velocidade = TetoInteiro(baseFicha.Velocidade + Somar(efeitos, CampoDeFichaDeTrinket.Velocidade, baseFicha.Velocidade));

        return new FichaDePersonagemDto(
            hpMax,
            hpAtual,
            baseFicha.Precisao + Somar(efeitos, CampoDeFichaDeTrinket.Precisao, baseFicha.Precisao),
            baseFicha.Protecao + Somar(efeitos, CampoDeFichaDeTrinket.Protecao, baseFicha.Protecao),
            baseFicha.Esquiva + Somar(efeitos, CampoDeFichaDeTrinket.Esquiva, baseFicha.Esquiva),
            velocidade,
            baseFicha.Critico + Somar(efeitos, CampoDeFichaDeTrinket.Critico, baseFicha.Critico),
            danoMin,
            danoMax,
            TetoInteiro(baseFicha.ChanceDeVirtude + Somar(efeitos, CampoDeFichaDeTrinket.ChanceDeVirtude, baseFicha.ChanceDeVirtude)),
            new ResistenciasDePersonagemDto(
                LimitarResistencia(baseFicha.Resistencias.Atordoamento + Somar(efeitos, CampoDeFichaDeTrinket.ResistenciaAtordoamento, baseFicha.Resistencias.Atordoamento)),
                LimitarResistencia(baseFicha.Resistencias.Sangramento + Somar(efeitos, CampoDeFichaDeTrinket.ResistenciaSangramento, baseFicha.Resistencias.Sangramento)),
                LimitarResistencia(baseFicha.Resistencias.Envenenamento + Somar(efeitos, CampoDeFichaDeTrinket.ResistenciaEnvenenamento, baseFicha.Resistencias.Envenenamento)),
                LimitarResistencia(baseFicha.Resistencias.Debuff + Somar(efeitos, CampoDeFichaDeTrinket.ResistenciaDebuff, baseFicha.Resistencias.Debuff)),
                LimitarResistencia(baseFicha.Resistencias.Movimento + Somar(efeitos, CampoDeFichaDeTrinket.ResistenciaMovimento, baseFicha.Resistencias.Movimento)),
                LimitarResistencia(baseFicha.Resistencias.Doenca + Somar(efeitos, CampoDeFichaDeTrinket.ResistenciaDoenca, baseFicha.Resistencias.Doenca)),
                LimitarResistencia(baseFicha.Resistencias.GolpeMortal + Somar(efeitos, CampoDeFichaDeTrinket.ResistenciaGolpeMortal, baseFicha.Resistencias.GolpeMortal)),
                LimitarResistencia(baseFicha.Resistencias.Armadilha + Somar(efeitos, CampoDeFichaDeTrinket.ResistenciaArmadilha, baseFicha.Resistencias.Armadilha))));
    }

    private static decimal Somar(IEnumerable<EfeitoDeAcessorio> efeitos, CampoDeFichaDeTrinket campo, decimal baseValor)
    {
        decimal total = 0;
        foreach (var efeito in efeitos)
        {
            if (!MapaDeEfeitoDeTrinket.TentarMapear(efeito.Nome, efeito.Unidade, out var map) || map.Campo != campo)
            {
                continue;
            }

            var sinal = efeito.Sinal == SinalDeEfeito.Negativo ? -1m : 1m;
            total += map.Modo switch
            {
                ModoDeAplicacaoDeTrinket.PercentualDaBase => baseValor * (efeito.Valor / 100m) * sinal,
                _ => efeito.Valor * sinal,
            };
        }

        return total;
    }

    private static int TetoInteiro(decimal valor)
    {
        var teto = Math.Ceiling(valor);
        if (teto > int.MaxValue)
        {
            return int.MaxValue;
        }

        if (teto < int.MinValue)
        {
            return int.MinValue;
        }

        return (int)teto;
    }

    private static decimal LimitarResistencia(decimal valor) => Math.Clamp(valor, 0m, 100m);
}
