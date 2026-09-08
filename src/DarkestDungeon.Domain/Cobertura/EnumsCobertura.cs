namespace DarkestDungeon.Domain.Cobertura;

/// Categorias catalogaáveis do Mapa de Cobertura.
public enum CategoriaDeCobertura
{
    HabilidadeCombate,
    HabilidadeAcampamento,
    ResistenciaBase,
}

/// Estado de coleta de um atributo no Mapa de Cobertura. Três estados distintos (FR-018).
public enum EstadoDeAtributo
{
    Coletado,
    Pendente,
    NaoAplicavel,
}
