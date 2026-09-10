using System.Text.Json;
using DarkestDungeon.Application.Midias;
using Microsoft.Extensions.Options;

namespace DarkestDungeon.Infrastructure.Midias;

public sealed class LeitorDeInventarioDeMidias : ILeitorDeInventarioDeMidias
{
    private static readonly JsonSerializerOptions Json = new() { PropertyNameCaseInsensitive = true };
    private readonly OpcoesDeInventarioDeMidias opcoes;

    public LeitorDeInventarioDeMidias(IOptions<OpcoesDeInventarioDeMidias> opcoes)
    {
        this.opcoes = opcoes.Value;
    }

    public async Task<InventarioDeMidiasDeEquipamento?> LerAsync(CancellationToken cancellationToken)
    {
        var caminho = opcoes.CaminhoDoInventario;
        if (string.IsNullOrWhiteSpace(caminho) || !File.Exists(caminho))
        {
            return null;
        }

        await using var fluxo = File.OpenRead(caminho);
        var documento = await JsonSerializer.DeserializeAsync<DocumentoInventario>(fluxo, Json, cancellationToken);
        if (documento is null)
        {
            return null;
        }

        var arquivos = (documento.Arquivos ?? []).Select(a => new ArquivoDeInventarioDeMidia(
            a.Categoria ?? string.Empty,
            a.Sha256 ?? string.Empty,
            a.CaminhoOrigem ?? string.Empty,
            a.CaminhoDestino ?? string.Empty,
            a.TamanhoBytes,
            a.Reutilizado,
            a.Classe,
            IdentificadorDeArquivo(a.CaminhoOrigem, a.CaminhoDestino))).ToArray();

        var lacunas = (documento.Lacunas ?? []).Select(l => new LacunaDeInventarioDeMidia(
            l.Categoria ?? l.Classe ?? string.Empty,
            l.Motivo ?? string.Empty,
            l.CaminhoConsultado ?? string.Empty,
            l.TentadoEmUtc == default ? DateTime.UtcNow : l.TentadoEmUtc)).ToArray();

        return new InventarioDeMidiasDeEquipamento(
            documento.InstalacaoOrigem ?? string.Empty,
            documento.DeclaracaoDeUso ?? string.Empty,
            arquivos,
            lacunas);
    }

    private static string? IdentificadorDeArquivo(string? origem, string? destino)
    {
        var nome = Path.GetFileNameWithoutExtension(origem ?? destino ?? string.Empty);
        return string.IsNullOrWhiteSpace(nome) ? null : nome;
    }

    private sealed class DocumentoInventario
    {
        public string? InstalacaoOrigem { get; set; }
        public string? DeclaracaoDeUso { get; set; }
        public List<ArquivoInventario>? Arquivos { get; set; }
        public List<LacunaInventario>? Lacunas { get; set; }
    }

    private sealed class ArquivoInventario
    {
        public string? Categoria { get; set; }
        public string? Sha256 { get; set; }
        public string? CaminhoOrigem { get; set; }
        public string? CaminhoDestino { get; set; }
        public long TamanhoBytes { get; set; }
        public bool Reutilizado { get; set; }
        public string? Classe { get; set; }
    }

    private sealed class LacunaInventario
    {
        public string? Categoria { get; set; }
        public string? Classe { get; set; }
        public string? Motivo { get; set; }
        public string? CaminhoConsultado { get; set; }
        public DateTime TentadoEmUtc { get; set; }
    }
}
