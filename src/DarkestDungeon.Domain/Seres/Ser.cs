using DarkestDungeon.Domain.Common;

namespace DarkestDungeon.Domain.Seres;

public class Ser : EntidadeIdentificavel
{
    protected Ser()
    {
        Nome = string.Empty;
        Tipo = string.Empty;
        Resistencias = new Resistencias(0, 0, 0, 0, 0);
    }

    public Ser(
        string nome,
        string tipo,
        int hpMaximo,
        int hpAtual,
        int velocidade,
        decimal critico,
        int danoBaseMinimo,
        int danoBaseMaximo,
        int movimento,
        decimal bonusDeCritico,
        int tamanho,
        int acoesPorTurno,
        decimal esquiva,
        decimal precisao,
        decimal protecao,
        int nivel,
        Resistencias resistencias,
        Guid? id = null)
    {
        Nome = NormalizarTexto(nome, nameof(Nome));
        Tipo = NormalizarTexto(tipo, nameof(Tipo));
        ValidarNaoNegativo(hpMaximo, nameof(HpMaximo), "HP máximo");
        ValidarNaoNegativo(hpAtual, nameof(HpAtual), "HP Atual");
        ValidarNaoNegativo(velocidade, nameof(Velocidade), "Velocidade");
        Resistencias.ValidarPercentual(critico, nameof(Critico), "Crítico");
        ValidarNaoNegativo(danoBaseMinimo, nameof(DanoBaseMinimo), "Dano base mínimo");
        ValidarNaoNegativo(danoBaseMaximo, nameof(DanoBaseMaximo), "Dano base máximo");
        ValidarNaoNegativo(movimento, nameof(Movimento), "Movimento");
        Resistencias.ValidarPercentual(bonusDeCritico, nameof(BonusDeCritico), "Bonus de Crítico");
        ValidarFaixa(tamanho, nameof(Tamanho), "Tamanho", 0, 4);
        ValidarNaoNegativo(acoesPorTurno, nameof(AcoesPorTurno), "Ações por turno");
        ValidarNaoNegativo(esquiva, nameof(Esquiva), "Esquiva");
        ValidarNaoNegativo(precisao, nameof(Precisao), "Precisão");
        Resistencias.ValidarPercentual(protecao, nameof(Protecao), "Proteção");
        ValidarFaixa(nivel, nameof(Nivel), "Nível", 0, 6);

        if (hpAtual > hpMaximo)
        {
            throw new ArgumentException("HP Atual deve ser menor ou igual ao HP máximo.", nameof(HpAtual));
        }

        if (danoBaseMinimo > danoBaseMaximo)
        {
            throw new ArgumentException("Dano base mínimo deve ser menor ou igual ao Dano base máximo.", nameof(DanoBaseMinimo));
        }

        Id = id.GetValueOrDefault(Guid.NewGuid());
        HpMaximo = hpMaximo;
        HpAtual = hpAtual;
        Velocidade = velocidade;
        Critico = critico;
        DanoBaseMinimo = danoBaseMinimo;
        DanoBaseMaximo = danoBaseMaximo;
        Movimento = movimento;
        BonusDeCritico = bonusDeCritico;
        Tamanho = tamanho;
        AcoesPorTurno = acoesPorTurno;
        Esquiva = esquiva;
        Precisao = precisao;
        Protecao = protecao;
        Nivel = nivel;
        Resistencias = resistencias ?? throw new ArgumentException("Resistências devem ser informadas.", nameof(resistencias));
    }

    public string Nome { get; private set; }
    public string Tipo { get; private set; }
    public int HpMaximo { get; private set; }
    public int HpAtual { get; private set; }
    public int Velocidade { get; private set; }
    public decimal Critico { get; private set; }
    public int DanoBaseMinimo { get; private set; }
    public int DanoBaseMaximo { get; private set; }
    public int Movimento { get; private set; }
    public decimal BonusDeCritico { get; private set; }
    public int Tamanho { get; private set; }
    public int AcoesPorTurno { get; private set; }
    public decimal Esquiva { get; private set; }
    public decimal Precisao { get; private set; }
    public decimal Protecao { get; private set; }
    public int Nivel { get; private set; }
    public Resistencias Resistencias { get; private set; }

    /// Ajusta o `Nivel` derivadamente — usado por `Personagem` quando `Experiencia` cruza limiares (Feature 005).
    protected void AtualizarNivel(int novoNivel)
    {
        if (novoNivel is < 0 or > 6)
        {
            throw new ArgumentOutOfRangeException(nameof(novoNivel), "Nivel deve estar entre 0 e 6.");
        }

        Nivel = novoNivel;
    }

    /// Ajusta as resistências (usado por `Personagem` ao aplicar bônus de nível — Feature 005).
    protected void AtualizarResistencias(Resistencias novas)
    {
        Resistencias = novas ?? throw new ArgumentNullException(nameof(novas));
    }

    protected static string NormalizarTexto(string valor, string campo)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ArgumentException($"{campo} deve ser informado.", campo);
        }

        return valor.Trim();
    }

    protected static void ValidarNaoNegativo(decimal valor, string campo, string nomeExibicao)
    {
        if (valor < 0)
        {
            throw new ArgumentException($"{nomeExibicao} não pode ser negativo.", campo);
        }
    }

    protected static void ValidarFaixa(int valor, string campo, string nomeExibicao, int minimo, int maximo)
    {
        if (valor < minimo || valor > maximo)
        {
            throw new ArgumentException($"{nomeExibicao} deve estar entre {minimo} e {maximo}.", campo);
        }
    }
}