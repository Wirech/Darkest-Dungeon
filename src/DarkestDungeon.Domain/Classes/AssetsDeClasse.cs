using DarkestDungeon.Domain.Cobertura;
using DarkestDungeon.Domain.Personagens;

namespace DarkestDungeon.Domain.Classes;

/// Vínculo Classe × Aparência → Conjunto Spine do inventário da Feature 004.
/// Não armazena caminho livre nem bytes; apenas o `ConjuntoSpineId` (chave do inventário 004) e o `HashArquivo` (SHA-256 do arquivo principal para detectar substituição silenciosa).
public sealed class AssetsDeClasse
{
    private AssetsDeClasse()
    {
        ConjuntoSpineId = null;
        HashArquivo = null;
    }

    public AssetsDeClasse(AparenciaDePersonagem aparencia, string? conjuntoSpineId, string? hashArquivo, EstadoDeAtributo status)
    {
        if (conjuntoSpineId is not null && conjuntoSpineId.Length > 200)
        {
            throw new ArgumentException("ConjuntoSpineId deve ter no máximo 200 caracteres.", nameof(conjuntoSpineId));
        }

        if (hashArquivo is not null && hashArquivo.Length != 64)
        {
            throw new ArgumentException("HashArquivo deve ser um SHA-256 hexadecimal de 64 caracteres.", nameof(hashArquivo));
        }

        Aparencia = aparencia;
        ConjuntoSpineId = conjuntoSpineId;
        HashArquivo = hashArquivo;
        Status = status;
    }

    public AparenciaDePersonagem Aparencia { get; private set; }
    public string? ConjuntoSpineId { get; private set; }
    public string? HashArquivo { get; private set; }
    public EstadoDeAtributo Status { get; private set; }
}
