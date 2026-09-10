using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Habilidades;

namespace DarkestDungeon.Application.Auditoria;

/// Implementação da auditoria: lê wiki-snapshots do diretório da feature, compara com os dados semeados no banco e produz o relatório.
/// Estratégia de comparação:
/// - Chave: `nomeOriginal` (inglês) da habilidade.
/// - Level 1: compara os campos herdados da Feature 003 no seed com `campos.*[0]` do snapshot.
/// - Levels 2..5: se ausentes na coleção `Niveis` do seed, adiciona `Pendente` ao Mapa de Cobertura (NivelDeHabilidade).
/// - Snapshot vazio ou ausente: marca todas as habilidades da classe como `Faltando`.
public sealed class AuditoriaWikiService : IAuditoriaWikiService
{
    private readonly IProvedorDeSnapshotsWiki provedor;
    private readonly IProvedorDeHabilidadesSeed provedorHabilidades;
    private readonly IProvedorDeAssetsAuditados provedorAssets;

    public AuditoriaWikiService(
        IProvedorDeSnapshotsWiki provedor,
        IProvedorDeHabilidadesSeed provedorHabilidades,
        IProvedorDeAssetsAuditados provedorAssets)
    {
        this.provedor = provedor ?? throw new ArgumentNullException(nameof(provedor));
        this.provedorHabilidades = provedorHabilidades ?? throw new ArgumentNullException(nameof(provedorHabilidades));
        this.provedorAssets = provedorAssets ?? throw new ArgumentNullException(nameof(provedorAssets));
    }

    public async Task<RelatorioDeAuditoria> GerarRelatorioAsync(ClasseDeHeroi? filtroClasse = null, CancellationToken cancellationToken = default)
    {
        var linhas = new List<LinhaDeAuditoria>();
        var todasClasses = filtroClasse.HasValue
            ? new[] { filtroClasse.Value }
            : Enum.GetValues<ClasseDeHeroi>();

        int niveisPendentes = 0;
        var chancesCapadas = new List<ChanceBaseCapada>();

        foreach (var classe in todasClasses)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var snapshot = await provedor.CarregarSnapshotAsync(classe, cancellationToken);
            var seedadas = await provedorHabilidades.ObterHabilidadesDaClasseAsync(classe, cancellationToken);

            if (snapshot?.Habilidades is not null)
            {
                ExtrairChancesCapadas(snapshot, chancesCapadas);
            }

            foreach (var seedada in seedadas)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var (status, diffs, pendenteDeNivel) = AvaliarHabilidade(seedada, snapshot);
                if (pendenteDeNivel)
                {
                    niveisPendentes += 4; // Levels 2..5 pendentes se snapshot vazio
                }

                linhas.Add(new LinhaDeAuditoria(
                    HabilidadeId: seedada.Id,
                    NomeExibicao: seedada.NomeExibicao,
                    NomeOriginal: seedada.NomeOriginal,
                    ClasseDoDono: classe,
                    Status: status,
                    Diffs: diffs
                ));
            }

            // Detecta habilidades no snapshot que não existem no seed
            if (snapshot?.Habilidades is not null)
            {
                var nomesSeedados = seedadas.Select(h => h.NomeOriginal).ToHashSet(StringComparer.OrdinalIgnoreCase);
                foreach (var habWiki in snapshot.Habilidades)
                {
                    if (string.IsNullOrWhiteSpace(habWiki.NomeOriginal))
                    {
                        continue;
                    }

                    if (!nomesSeedados.Contains(habWiki.NomeOriginal))
                    {
                        linhas.Add(new LinhaDeAuditoria(
                            HabilidadeId: Guid.Empty,
                            NomeExibicao: habWiki.NomeExibicao ?? habWiki.NomeOriginal,
                            NomeOriginal: habWiki.NomeOriginal,
                            ClasseDoDono: classe,
                            Status: StatusDeAuditoria.Faltando,
                            Diffs: new[] { new DiffDeCampo("(existe apenas na wiki)", habWiki.NomeOriginal, "(ausente no seed)", "Habilidade na wiki não semeada.") }
                        ));
                    }
                }
            }
        }

        var (assetsColetados, assetsPendentes) = await provedorAssets.ContarAssetsAsync(cancellationToken);
        var resumo = new ResumoDeAuditoria(
            TotalHabilidades: linhas.Count,
            Ok: linhas.Count(l => l.Status == StatusDeAuditoria.OK),
            Parcial: linhas.Count(l => l.Status == StatusDeAuditoria.Parcial),
            Faltando: linhas.Count(l => l.Status == StatusDeAuditoria.Faltando),
            NiveisPendentes: niveisPendentes,
            AssetsColetados: assetsColetados,
            AssetsPendentes: assetsPendentes
        );

        return new RelatorioDeAuditoria(DateTime.UtcNow, resumo, linhas, chancesCapadas.OrderBy(c => c.NomeOriginal).ThenBy(c => c.NumeroDoNivel).ToList());
    }

    /// FR-013: coleta chances base > 100% do snapshot (capadas em 100% na aplicação).
    private static void ExtrairChancesCapadas(SnapshotDeClasse snapshot, List<ChanceBaseCapada> destino)
    {
        if (snapshot.Habilidades is null)
        {
            return;
        }
        foreach (var hab in snapshot.Habilidades)
        {
            if (hab.Efeitos is null || string.IsNullOrWhiteSpace(hab.NomeOriginal))
            {
                continue;
            }
            foreach (var efeito in hab.Efeitos)
            {
                if (efeito.ChancePorNivel is null || string.IsNullOrWhiteSpace(efeito.TipoDoEfeito))
                {
                    continue;
                }
                for (int i = 0; i < efeito.ChancePorNivel.Count && i < 5; i++)
                {
                    var valor = ExtrairNumero(efeito.ChancePorNivel[i]);
                    if (valor.HasValue && valor.Value > 100m)
                    {
                        destino.Add(new ChanceBaseCapada(
                            NomeOriginal: hab.NomeOriginal,
                            TipoDoEfeito: efeito.TipoDoEfeito,
                            NumeroDoNivel: i + 1,
                            ChanceOriginal: valor.Value,
                            ChanceAplicada: 100m));
                    }
                }
            }
        }
    }

    public async Task SalvarComoMarkdownAsync(RelatorioDeAuditoria relatorio, string caminhoAbsoluto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(relatorio);
        ArgumentException.ThrowIfNullOrWhiteSpace(caminhoAbsoluto);

        var sb = new StringBuilder();
        sb.AppendLine("# Relatório de Auditoria — Feature 005");
        sb.AppendLine();
        sb.AppendLine($"**Gerado em**: {relatorio.GeradoEm:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine();
        sb.AppendLine("## Resumo");
        sb.AppendLine();
        sb.AppendLine($"- Total de habilidades classificadas: **{relatorio.Resumo.TotalHabilidades}**");
        sb.AppendLine($"- OK: {relatorio.Resumo.Ok}");
        sb.AppendLine($"- Parcial: {relatorio.Resumo.Parcial}");
        sb.AppendLine($"- Faltando: {relatorio.Resumo.Faltando}");
        sb.AppendLine($"- Níveis pendentes (Level 2..5 ainda não minerados): {relatorio.Resumo.NiveisPendentes}");
        sb.AppendLine($"- Assets Classe × Aparência coletados: **{relatorio.Resumo.AssetsColetados} / {relatorio.Resumo.AssetsColetados + relatorio.Resumo.AssetsPendentes}**");
        sb.AppendLine();

        var porClasse = relatorio.Linhas.GroupBy(l => l.ClasseDoDono).OrderBy(g => g.Key.ToString());
        foreach (var grupo in porClasse)
        {
            cancellationToken.ThrowIfCancellationRequested();
            sb.AppendLine($"## {grupo.Key}");
            sb.AppendLine();
            sb.AppendLine("| Habilidade | Status | Divergências |");
            sb.AppendLine("|---|---|---|");
            foreach (var linha in grupo.OrderBy(l => l.NomeExibicao))
            {
                var divs = linha.Diffs.Count == 0
                    ? "—"
                    : string.Join("<br/>", linha.Diffs.Select(d => $"`{d.Campo}`: esperado `{d.ValorEsperado}`, atual `{d.ValorAtual}` ({d.Observacao})"));
                sb.AppendLine($"| {linha.NomeExibicao} ({linha.NomeOriginal}) | **{linha.Status}** | {divs} |");
            }

            sb.AppendLine();
        }

        // FR-013: seção de chances base > 100% capadas em 100%.
        if (relatorio.ChancesCapadas.Count > 0)
        {
            sb.AppendLine("## FR-013 — Chance base > 100% capada em 100%");
            sb.AppendLine();
            sb.AppendLine("Habilidades cuja wiki oficial declara chance base > 100% em algum nível. A aplicação aplica cap em 100% em runtime (a coluna `Chance` no BD armazena o valor original para rastreabilidade).");
            sb.AppendLine();
            sb.AppendLine("| Habilidade | Efeito | Nível | Chance Wiki | Chance Aplicada |");
            sb.AppendLine("|---|---|---|---|---|");
            foreach (var cap in relatorio.ChancesCapadas)
            {
                sb.AppendLine($"| `{cap.NomeOriginal}` | {cap.TipoDoEfeito} | L{cap.NumeroDoNivel} | {cap.ChanceOriginal.ToString("0.##", CultureInfo.InvariantCulture)}% | {cap.ChanceAplicada.ToString("0.##", CultureInfo.InvariantCulture)}% |");
            }
            sb.AppendLine();
        }

        var dir = Path.GetDirectoryName(caminhoAbsoluto);
        if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        await File.WriteAllTextAsync(caminhoAbsoluto, sb.ToString(), new UTF8Encoding(false), cancellationToken);
    }

    private static (StatusDeAuditoria status, IReadOnlyList<DiffDeCampo> diffs, bool pendenteNivel) AvaliarHabilidade(
        HabilidadeSeedada seedada,
        SnapshotDeClasse? snapshot)
    {
        if (snapshot?.Habilidades is null || snapshot.Habilidades.Count == 0)
        {
            return (StatusDeAuditoria.Faltando,
                new[] { new DiffDeCampo("(snapshot ausente)", "Level 1..5 da wiki", "(não minerado)", "Nenhum wiki-snapshot preenchido para esta classe.") },
                pendenteNivel: true);
        }

        var candidato = snapshot.Habilidades
            .FirstOrDefault(h => string.Equals(h.NomeOriginal, seedada.NomeOriginal, StringComparison.OrdinalIgnoreCase));

        if (candidato is null)
        {
            return (StatusDeAuditoria.Faltando,
                new[] { new DiffDeCampo("nomeOriginal", seedada.NomeOriginal, "(ausente no snapshot)", "Habilidade seedada não encontrada no snapshot da wiki.") },
                pendenteNivel: true);
        }

        var diffs = new List<DiffDeCampo>();

        if (!string.IsNullOrWhiteSpace(candidato.NomeExibicao) && !string.Equals(candidato.NomeExibicao, seedada.NomeExibicao, StringComparison.Ordinal))
        {
            diffs.Add(new DiffDeCampo("nomeExibicao", candidato.NomeExibicao, seedada.NomeExibicao, "Divergência em nome PT-BR."));
        }

        if (!string.IsNullOrWhiteSpace(candidato.Descricao) && !string.Equals(candidato.Descricao, seedada.Descricao, StringComparison.Ordinal))
        {
            diffs.Add(new DiffDeCampo("descricao", candidato.Descricao, seedada.Descricao, "Divergência em descrição."));
        }

        // FR-003: comparação dos 15 campos.
        CompararTipo(candidato, seedada, diffs);
        CompararPosicoes(candidato, seedada, diffs);
        CompararAlvoEmArea(candidato, seedada, diffs);
        CompararModificadoresLevel1(candidato, seedada, diffs);
        CompararLimitePorUso(candidato, seedada, diffs);
        CompararCustoDeDescanso(candidato, seedada, diffs);
        CompararAlvoAcampamento(candidato, seedada, diffs);

        // US1/AC3 + SC-006: diferenciar efeitos vazios justificados (snapshot também vazio) de omitidos.
        AvaliarEfeitosPresenca(candidato, seedada, diffs);

        // FR-010: heurística de simplificação em strings genéricas.
        DetectarEfeitoSimplificado(seedada, diffs);

        // FR-011: heurística de valores só na descrição.
        DetectarValoresSoNaDescricao(seedada, diffs);

        if (seedada.QuantidadeDeNiveisSeedados < 5)
        {
            diffs.Add(new DiffDeCampo("niveis", "5 níveis (Level 1..5)", seedada.QuantidadeDeNiveisSeedados.ToString(CultureInfo.InvariantCulture), "Coleção Niveis não completa — mineração de Level 2..5 pendente."));
        }

        var status = diffs.Count == 0 ? StatusDeAuditoria.OK : StatusDeAuditoria.Parcial;
        return (status, diffs, pendenteNivel: seedada.QuantidadeDeNiveisSeedados < 5);
    }

    private static void CompararTipo(HabilidadeDoSnapshot cand, HabilidadeSeedada seed, List<DiffDeCampo> diffs)
    {
        if (string.IsNullOrWhiteSpace(cand.Tipo) || string.IsNullOrWhiteSpace(seed.Tipo))
        {
            return;
        }
        if (!string.Equals(cand.Tipo, seed.Tipo, StringComparison.OrdinalIgnoreCase))
        {
            diffs.Add(new DiffDeCampo("tipo", cand.Tipo, seed.Tipo, "Tipo de habilidade divergente (Combate/Acampamento)."));
        }
    }

    private static void CompararPosicoes(HabilidadeDoSnapshot cand, HabilidadeSeedada seed, List<DiffDeCampo> diffs)
    {
        if (cand.Rank is { Count: > 0 } && seed.PosicoesValidas.Count > 0)
        {
            var wiki = cand.Rank.OrderBy(v => v).ToArray();
            var atual = seed.PosicoesValidas.OrderBy(v => v).ToArray();
            if (!wiki.SequenceEqual(atual))
            {
                diffs.Add(new DiffDeCampo("posicoesValidas", string.Join(",", wiki), string.Join(",", atual), "Divergência em rank/posicoesValidas."));
            }
        }

        if (cand.Target is { Count: > 0 } && seed.PosicoesQueAtinge.Count > 0)
        {
            var wiki = cand.Target.OrderBy(v => v).ToArray();
            var atual = seed.PosicoesQueAtinge.OrderBy(v => v).ToArray();
            if (!wiki.SequenceEqual(atual))
            {
                diffs.Add(new DiffDeCampo("posicoesQueAtinge", string.Join(",", wiki), string.Join(",", atual), "Divergência em target/posicoesQueAtinge."));
            }
        }
    }

    private static void CompararAlvoEmArea(HabilidadeDoSnapshot cand, HabilidadeSeedada seed, List<DiffDeCampo> diffs)
    {
        if (cand.AlvoEmArea.HasValue && seed.AlvoEmArea.HasValue && cand.AlvoEmArea.Value != seed.AlvoEmArea.Value)
        {
            diffs.Add(new DiffDeCampo("alvoEmArea", cand.AlvoEmArea.Value.ToString(), seed.AlvoEmArea.Value.ToString(), "Divergência em alvo em área."));
        }
    }

    private static void CompararModificadoresLevel1(HabilidadeDoSnapshot cand, HabilidadeSeedada seed, List<DiffDeCampo> diffs)
    {
        CompararUmModificador("modificadorDano", cand.Campos?.ModificadorDano, seed.ModificadorDano, diffs);
        CompararUmModificador("modificadorAcerto", cand.Campos?.ModificadorAcerto, seed.ModificadorAcerto, diffs);
        CompararUmModificador("modificadorCritico", cand.Campos?.ModificadorCritico, seed.ModificadorCritico, diffs);
    }

    private static void CompararUmModificador(string nome, List<JsonElement>? arr, decimal? atualL1, List<DiffDeCampo> diffs)
    {
        if (arr is null || arr.Count == 0 || !atualL1.HasValue)
        {
            return;
        }
        var wikiL1 = ExtrairNumero(arr[0]);
        if (!wikiL1.HasValue)
        {
            return;
        }
        if (wikiL1.Value != atualL1.Value)
        {
            diffs.Add(new DiffDeCampo(nome, wikiL1.Value.ToString(CultureInfo.InvariantCulture), atualL1.Value.ToString(CultureInfo.InvariantCulture), $"Divergência em {nome} (Level 1)."));
        }
    }

    private static void CompararLimitePorUso(HabilidadeDoSnapshot cand, HabilidadeSeedada seed, List<DiffDeCampo> diffs)
    {
        if (cand.LimitePorUso is not null && seed.LimitePorUsoMaximo.HasValue)
        {
            if (cand.LimitePorUso.Quantidade != seed.LimitePorUsoMaximo.Value)
            {
                diffs.Add(new DiffDeCampo("limitePorUso.quantidade", cand.LimitePorUso.Quantidade.ToString(CultureInfo.InvariantCulture), seed.LimitePorUsoMaximo.Value.ToString(CultureInfo.InvariantCulture), "Divergência em limitePorUso."));
            }
            if (!string.IsNullOrWhiteSpace(cand.LimitePorUso.Escopo) && !string.Equals(cand.LimitePorUso.Escopo, seed.LimitePorUsoEscopo, StringComparison.OrdinalIgnoreCase))
            {
                diffs.Add(new DiffDeCampo("limitePorUso.escopo", cand.LimitePorUso.Escopo, seed.LimitePorUsoEscopo ?? "(nulo)", "Divergência em escopo do limite."));
            }
        }
        else if (cand.LimitePorUso is not null && !seed.LimitePorUsoMaximo.HasValue)
        {
            diffs.Add(new DiffDeCampo("limitePorUso", $"{cand.LimitePorUso.Quantidade} usos/{cand.LimitePorUso.Escopo}", "(ausente)", "Wiki declara limite; seed não persistiu."));
        }
        else if (cand.LimitePorUso is null && seed.LimitePorUsoMaximo.HasValue)
        {
            diffs.Add(new DiffDeCampo("limitePorUso", "(sem limite)", seed.LimitePorUsoMaximo.Value.ToString(CultureInfo.InvariantCulture), "Seed declara limite; wiki não."));
        }
    }

    private static void CompararCustoDeDescanso(HabilidadeDoSnapshot cand, HabilidadeSeedada seed, List<DiffDeCampo> diffs)
    {
        if (!string.Equals(cand.Tipo, "Acampamento", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        if (cand.CustoDeDescanso.HasValue && seed.CustoDeDescanso.HasValue && cand.CustoDeDescanso.Value != seed.CustoDeDescanso.Value)
        {
            diffs.Add(new DiffDeCampo("custoDeDescanso", cand.CustoDeDescanso.Value.ToString(CultureInfo.InvariantCulture), seed.CustoDeDescanso.Value.ToString(CultureInfo.InvariantCulture), "Divergência em custo de descanso."));
        }
    }

    private static void CompararAlvoAcampamento(HabilidadeDoSnapshot cand, HabilidadeSeedada seed, List<DiffDeCampo> diffs)
    {
        if (!string.Equals(cand.Tipo, "Acampamento", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        if (!string.IsNullOrWhiteSpace(cand.Alvo) && !string.IsNullOrWhiteSpace(seed.Alvo))
        {
            var alvoWikiNormalizado = NormalizarAlvoWiki(cand.Alvo);
            if (!string.Equals(alvoWikiNormalizado, seed.Alvo, StringComparison.OrdinalIgnoreCase))
            {
                diffs.Add(new DiffDeCampo("alvo", cand.Alvo, seed.Alvo, "Divergência em alvo de acampamento."));
            }
        }
    }

    /// Mapeia os nomes do alvo usados nos snapshots (EN) para o enum `AlvoDeAcampamento` (PT-BR).
    private static string NormalizarAlvoWiki(string alvoDoSnapshot)
    {
        return alvoDoSnapshot.Trim().ToLowerInvariant() switch
        {
            "self" => "Self",
            "onecompanion" or "one companion" or "one ally" => "UmAliado",
            "allcompanions" or "all companions" => "TodosOsAliados",
            "selfandallcompanions" => "PartyInteira",
            "selfandonecompanion" or "self and one companion" => "SelfEUmAliado",
            "party" => "Party",
            _ => alvoDoSnapshot,
        };
    }

    private static void AvaliarEfeitosPresenca(HabilidadeDoSnapshot cand, HabilidadeSeedada seed, List<DiffDeCampo> diffs)
    {
        var wikiTemEfeitos = cand.Efeitos is { Count: > 0 };
        var seedTemEfeitos = seed.QuantidadeDeEfeitosSeedados > 0;

        if (wikiTemEfeitos && !seedTemEfeitos)
        {
            var nomesWiki = string.Join(", ", cand.Efeitos!.Where(e => !string.IsNullOrWhiteSpace(e.TipoDoEfeito)).Select(e => e.TipoDoEfeito!));
            diffs.Add(new DiffDeCampo("efeitos", nomesWiki, "(vazio)", "Wiki lista efeitos; seed omitiu."));
        }
        // Caso oposto (seed tem, wiki não) é considerado "unrequested" mas não é diff — não bloqueia OK.
    }

    private static void DetectarEfeitoSimplificado(HabilidadeSeedada seed, List<DiffDeCampo> diffs)
    {
        // Heurística: nome de efeito com "Bônus de Resistências" ou "Resistências +N%" indica agrupamento genérico.
        foreach (var nome in seed.NomesDeEfeitosSeedados)
        {
            if (Regex.IsMatch(nome, @"(bônus de resistências|resistências)", RegexOptions.IgnoreCase))
            {
                diffs.Add(new DiffDeCampo(
                    "efeitos.simplificado",
                    "N efeitos individuais (Atordoamento/Sangramento/Envenenamento/Debuff/Movimento)",
                    nome,
                    "Efeito agrupado detectado; considere expandir em N efeitos individuais (FR-010)."));
                break;
            }
        }
    }

    private static readonly Regex RegexValoresNaDescricao = new(
        @"(?:tocha|torch|estresse|stress|cura|heal)\s*(?:em\s+|de\s+)?[+-]?\s*\d+",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static void DetectarValoresSoNaDescricao(HabilidadeSeedada seed, List<DiffDeCampo> diffs)
    {
        if (string.IsNullOrWhiteSpace(seed.Descricao))
        {
            return;
        }
        var matches = RegexValoresNaDescricao.Matches(seed.Descricao);
        if (matches.Count == 0)
        {
            return;
        }
        // Se há valores numéricos na descrição, verifica se aparecem nos nomes dos efeitos estruturados.
        var descLower = string.Join(", ", matches.Select(m => m.Value));
        var efeitosLower = string.Join(" ", seed.NomesDeEfeitosSeedados).ToLowerInvariant();
        var faltantes = matches.Where(m => !efeitosLower.Contains(m.Value.ToLowerInvariant().Split(' ')[0])).ToArray();
        if (faltantes.Length > 0)
        {
            diffs.Add(new DiffDeCampo(
                "efeitos.valorSoNaDescricao",
                "efeito estruturado com valor",
                string.Join(", ", faltantes.Select(m => m.Value)),
                "Valor numérico presente apenas na descrição; considere adicionar em efeitos[] (FR-011)."));
        }
    }

    private static decimal? ExtrairNumero(JsonElement el)
    {
        switch (el.ValueKind)
        {
            case JsonValueKind.Number:
                return el.GetDecimal();
            case JsonValueKind.String:
                var s = el.GetString();
                if (string.IsNullOrWhiteSpace(s))
                {
                    return null;
                }
                var m = Regex.Match(s, @"[-+]?\d+(?:\.\d+)?");
                if (m.Success && decimal.TryParse(m.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var d))
                {
                    return d;
                }
                return null;
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return null;
            default:
                return null;
        }
    }

    /// Snapshot deserializado de um wiki-snapshot JSON.
    public sealed class SnapshotDeClasse
    {
        public string? Classe { get; set; }
        public string? ClasseDeHeroiEnum { get; set; }
        public string? FonteUrl { get; set; }
        public string? Status { get; set; }
        public List<HabilidadeDoSnapshot>? Habilidades { get; set; }
    }

    public sealed class HabilidadeDoSnapshot
    {
        public string? NomeExibicao { get; set; }
        public string? NomeOriginal { get; set; }
        public string? Tipo { get; set; }
        public string? Descricao { get; set; }
        public string? Range { get; set; }
        public List<int>? Rank { get; set; }
        public List<int>? Target { get; set; }
        public bool? AlvoEmArea { get; set; }
        public int? CustoDeDescanso { get; set; }
        public string? Alvo { get; set; }
        public LimitePorUsoDoSnapshot? LimitePorUso { get; set; }
        public CamposDoSnapshot? Campos { get; set; }
        public List<EfeitoDoSnapshot>? Efeitos { get; set; }
    }

    public sealed class LimitePorUsoDoSnapshot
    {
        public int Quantidade { get; set; }
        public string? Escopo { get; set; }
    }

    public sealed class CamposDoSnapshot
    {
        public List<JsonElement>? ModificadorDano { get; set; }
        public List<JsonElement>? ModificadorAcerto { get; set; }
        public List<JsonElement>? ModificadorCritico { get; set; }
    }

    public sealed class EfeitoDoSnapshot
    {
        public string? TipoDoEfeito { get; set; }
        public List<JsonElement>? ValorPorNivel { get; set; }
        public List<JsonElement>? ChancePorNivel { get; set; }
        public JsonElement? Valor { get; set; }
    }
}

/// Representação leve de uma habilidade semeada para a comparação — carrega apenas os campos usados pelo AuditoriaWikiService.
public sealed record HabilidadeSeedada(
    Guid Id,
    string NomeExibicao,
    string NomeOriginal,
    string Descricao,
    int QuantidadeDeNiveisSeedados
)
{
    public string Tipo { get; init; } = string.Empty;
    public IReadOnlyList<int> PosicoesValidas { get; init; } = Array.Empty<int>();
    public IReadOnlyList<int> PosicoesQueAtinge { get; init; } = Array.Empty<int>();
    public bool? AlvoEmArea { get; init; }
    public decimal? ModificadorDano { get; init; }
    public decimal? ModificadorAcerto { get; init; }
    public decimal? ModificadorCritico { get; init; }
    public int QuantidadeDeEfeitosSeedados { get; init; }
    public IReadOnlyList<string> NomesDeEfeitosSeedados { get; init; } = Array.Empty<string>();
    public int? CustoDeDescanso { get; init; }
    public string? Alvo { get; init; }
    public int? LimitePorUsoMaximo { get; init; }
    public string? LimitePorUsoEscopo { get; init; }
}

/// Provedor de snapshots da wiki (implementação padrão lê arquivos JSON de disco).
public interface IProvedorDeSnapshotsWiki
{
    Task<AuditoriaWikiService.SnapshotDeClasse?> CarregarSnapshotAsync(ClasseDeHeroi classe, CancellationToken cancellationToken);
}

/// Provedor das habilidades semeadas por classe (implementação padrão usa DbContext via Infrastructure).
public interface IProvedorDeHabilidadesSeed
{
    Task<IReadOnlyList<HabilidadeSeedada>> ObterHabilidadesDaClasseAsync(ClasseDeHeroi classe, CancellationToken cancellationToken);
}

/// Provedor de contagens de assets vinculados ao inventário 004.
public interface IProvedorDeAssetsAuditados
{
    Task<(int coletados, int pendentes)> ContarAssetsAsync(CancellationToken cancellationToken);
}
