using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Common;

namespace DarkestDungeon.Domain.Cobertura;

/// Entrada consultável do Mapa de Cobertura (FR-017, FR-018).
/// Chave lógica: (ClasseDeHeroi, Categoria, ChaveDoAtributo).
public sealed class EntradaDoMapaDeCobertura : EntidadeIdentificavel
{
    private EntradaDoMapaDeCobertura()
    {
        ChaveDoAtributo = string.Empty;
    }

    public EntradaDoMapaDeCobertura(
        ClasseDeHeroi classe,
        CategoriaDeCobertura categoria,
        string chaveDoAtributo,
        EstadoDeAtributo estado,
        string? notas = null,
        Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(chaveDoAtributo) || chaveDoAtributo.Length > 80)
        {
            throw new ArgumentException("Chave do atributo deve ter entre 1 e 80 caracteres.", nameof(chaveDoAtributo));
        }

        if (notas is not null && notas.Length > 200)
        {
            throw new ArgumentException("Notas devem ter no máximo 200 caracteres.", nameof(notas));
        }

        Id = id.GetValueOrDefault(Guid.NewGuid());
        Classe = classe;
        Categoria = categoria;
        ChaveDoAtributo = chaveDoAtributo.Trim();
        Estado = estado;
        Notas = notas?.Trim();
    }

    public ClasseDeHeroi Classe { get; private set; }
    public CategoriaDeCobertura Categoria { get; private set; }
    public string ChaveDoAtributo { get; private set; }
    public EstadoDeAtributo Estado { get; private set; }
    public string? Notas { get; private set; }

    public void AtualizarEstado(EstadoDeAtributo novo, string? notas = null)
    {
        Estado = novo;
        Notas = notas?.Trim();
    }
}
