using DarkestDungeon.MediaCollector.Spine;

namespace DarkestDungeon.MediaCollector.Inventario;

public sealed record ArquivoImportado(
    string Classe,
    string CaminhoOrigem,
    string CaminhoDestino,
    string Sha256,
    long TamanhoBytes,
    bool Reutilizado,
    string? Categoria = null);

public sealed record LacunaDeImportacao(
    string Classe,
    string Motivo,
    string CaminhoConsultado,
    DateTime TentadoEmUtc,
    string? Categoria = null);

public sealed record ResumoDaClasse(
    string Classe,
    int Arquivos,
    int Reutilizados,
    int Conjuntos,
    int Lacunas);

public sealed record ResultadoDaImportacao(
    string InstalacaoOrigem,
    string DeclaracaoDeUso,
    List<ArquivoImportado> Arquivos,
    List<ConjuntoSpine> Conjuntos,
    List<AssociacaoDeHabilidade> AssociacoesDeHabilidades,
    List<LacunaDeImportacao> Lacunas,
    List<ResumoDaClasse> ResumoPorClasse);