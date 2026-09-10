namespace DarkestDungeon.Domain.Habilidades;

/// Catálogo canônico de tipos de efeito reconhecidos pela auditoria da wiki.
/// Usado por `AuditoriaWikiService` para detectar efeitos simplificados em strings genéricas (FR-010).
public static class TiposDeEfeito
{
    public const string Sangramento = "Sangramento";
    public const string Envenenamento = "Envenenamento";
    public const string Atordoamento = "Atordoamento";
    public const string Marcar = "Marcar";
    public const string Guardar = "Guardar";
    public const string StressCausado = "StressCausado";
    public const string StressCurado = "StressCurado";
    public const string Cura = "Cura";
    public const string CuraPorRodada = "CuraPorRodada";
    public const string ReducaoDeTocha = "ReducaoDeTocha";
    public const string BuffPrecisao = "BuffPrecisao";
    public const string BuffCritico = "BuffCritico";
    public const string BuffDano = "BuffDano";
    public const string BuffProtecao = "BuffProtecao";
    public const string BuffEsquiva = "BuffEsquiva";
    public const string BuffVelocidade = "BuffVelocidade";
    public const string BuffResistenciaSangramento = "BuffResistenciaSangramento";
    public const string BuffResistenciaEnvenenamento = "BuffResistenciaEnvenenamento";
    public const string BuffResistenciaMovimento = "BuffResistenciaMovimento";
    public const string BuffResistenciaDebuff = "BuffResistenciaDebuff";
    public const string BuffResistenciaAtordoamento = "BuffResistenciaAtordoamento";
    public const string BuffChanceDesarmarArmadilha = "BuffChanceDesarmarArmadilha";
    public const string DebuffPrecisao = "DebuffPrecisao";
    public const string DebuffCritico = "DebuffCritico";
    public const string DebuffDano = "DebuffDano";
    public const string DebuffProtecao = "DebuffProtecao";
    public const string DebuffEsquiva = "DebuffEsquiva";
    public const string DebuffVelocidade = "DebuffVelocidade";
    public const string MoverAliado = "MoverAliado";
    public const string MoverInimigo = "MoverInimigo";
    public const string RemoverBuff = "RemoverBuff";
    public const string RemoverDebuff = "RemoverDebuff";
    public const string RemoverSangramento = "RemoverSangramento";
    public const string RemoverEnvenenamento = "RemoverEnvenenamento";
    public const string RemoverDoenca = "RemoverDoenca";
    public const string RemoverMarca = "RemoverMarca";
    public const string ReduzirStress = "ReduzirStress";
    public const string AumentarStress = "AumentarStress";
    public const string Provocar = "Provocar";
    public const string Confuso = "Confuso";
    public const string SangueSanto = "SangueSanto";

    /// Palavras-chave livres que sinalizam efeito de resistência composto (ex.: "Bônus de Resistências") — usado por FR-010.
    public static readonly IReadOnlyCollection<string> PalavrasChaveResistenciaComposta = new[]
    {
        "Bônus de Resistências",
        "Bonus de Resistencias",
        "Resistências +",
        "All Resistances",
    };
}
