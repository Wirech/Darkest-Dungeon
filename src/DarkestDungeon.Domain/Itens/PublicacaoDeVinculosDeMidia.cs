namespace DarkestDungeon.Domain.Itens;

public enum EstadoDePublicacaoDeVinculos
{
    EmCurso,
    Concluida,
    RollbackAplicado,
}

/// Rastreio de publicação de vínculos 006. Não reutiliza o log/fluxo da Feature 005.
public sealed class PublicacaoDeVinculosDeMidia
{
    private PublicacaoDeVinculosDeMidia()
    {
    }

    public PublicacaoDeVinculosDeMidia(CategoriaDeMidiaDeItem categoria, DateTimeOffset iniciadaEm, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        Categoria = categoria;
        IniciadaEm = iniciadaEm;
        Estado = EstadoDePublicacaoDeVinculos.EmCurso;
    }

    public Guid Id { get; private set; }
    public CategoriaDeMidiaDeItem Categoria { get; private set; }
    public DateTimeOffset IniciadaEm { get; private set; }
    public DateTimeOffset? ConcluidaEm { get; private set; }
    public EstadoDePublicacaoDeVinculos Estado { get; private set; }
    public int ItensAtualizados { get; private set; }
    public int VinculosOk { get; private set; }
    public int VinculosPendentes { get; private set; }
    public int AcessoriosNovos { get; private set; }

    public void Concluir(DateTimeOffset concluidaEm, int itensAtualizados, int vinculosOk, int vinculosPendentes, int acessoriosNovos = 0)
    {
        Estado = EstadoDePublicacaoDeVinculos.Concluida;
        ConcluidaEm = concluidaEm;
        ItensAtualizados = itensAtualizados;
        VinculosOk = vinculosOk;
        VinculosPendentes = vinculosPendentes;
        AcessoriosNovos = acessoriosNovos;
    }

    public void MarcarRollback(DateTimeOffset concluidaEm)
    {
        Estado = EstadoDePublicacaoDeVinculos.RollbackAplicado;
        ConcluidaEm = concluidaEm;
    }
}
