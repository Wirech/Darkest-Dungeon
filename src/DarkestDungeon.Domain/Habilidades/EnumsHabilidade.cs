namespace DarkestDungeon.Domain.Habilidades;

/// A quem o Efeito de Habilidade se aplica.
public enum AlvoDeEfeito
{
    Self,
    Aliado,
    Inimigo,
}

/// Unidade numérica do valor de um Efeito de Habilidade.
public enum UnidadeDeEfeito
{
    Percentual,
    Pontos,
    Rodadas,
}

/// Escopo em que um LimitePorUso é reiniciado.
public enum EscopoDeLimite
{
    Batalha,
    Acampamento,
}

/// Alvo de uma Habilidade de Acampamento.
public enum AlvoDeAcampamento
{
    Self,
    UmAliado,
    TodosOsAliados,
    PartyInteira,
    SelfEUmAliado,
    Party,
}
