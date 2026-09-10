using DarkestDungeon.Domain.Itens;

namespace DarkestDungeon.Application.Midias;

public sealed record RelatorioDeCoberturaDeMidias(
    DateTimeOffset GeradoEm,
    IReadOnlyList<ResumoDeCategoriaDeMidia> Categorias,
    IReadOnlyList<OrfaoDeMidia> Orfaos,
    IReadOnlyList<LacunaDeInventarioDeMidia> Lacunas);

public sealed record ResumoDeCategoriaDeMidia(
    CategoriaDeMidiaDeItem Categoria,
    int Esperados,
    int Ok,
    int Parcial,
    int Pendente,
    IReadOnlyList<LinhaDeCoberturaDeItem> Linhas);

public sealed record LinhaDeCoberturaDeItem(
    Guid ItemId,
    string NomeExibicao,
    string NomeOriginal,
    StatusDeCoberturaDeItem Status,
    IReadOnlyList<int>? NiveisOk,
    IReadOnlyList<int>? NiveisPendentes);

public sealed record OrfaoDeMidia(
    string CaminhoOrigem,
    string Sha256,
    string? CaminhoDestino);
