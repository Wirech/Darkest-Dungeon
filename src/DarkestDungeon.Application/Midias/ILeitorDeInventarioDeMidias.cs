namespace DarkestDungeon.Application.Midias;

public sealed record ArquivoDeInventarioDeMidia(
    string Categoria,
    string Sha256,
    string CaminhoOrigem,
    string CaminhoDestino,
    long TamanhoBytes,
    bool Reutilizado,
    string? Classe,
    string? Identificador);

public sealed record LacunaDeInventarioDeMidia(
    string Categoria,
    string Motivo,
    string CaminhoConsultado,
    DateTime TentadoEmUtc);

public sealed record InventarioDeMidiasDeEquipamento(
    string InstalacaoOrigem,
    string DeclaracaoDeUso,
    IReadOnlyList<ArquivoDeInventarioDeMidia> Arquivos,
    IReadOnlyList<LacunaDeInventarioDeMidia> Lacunas);

/// Porta de leitura do inventário JSON. Implementação na Infrastructure (sem System.IO na Application).
public interface ILeitorDeInventarioDeMidias
{
    Task<InventarioDeMidiasDeEquipamento?> LerAsync(CancellationToken cancellationToken);
}
