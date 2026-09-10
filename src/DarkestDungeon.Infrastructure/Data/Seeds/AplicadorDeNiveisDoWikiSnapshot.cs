using System.Data;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Habilidades;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DarkestDungeon.Infrastructure.Data.Seeds;

/// Aplicador de níveis (Level 1..5) a partir dos wiki-snapshots minerados em
/// `specs/005-auditoria-habilidades-mineradas/wiki-snapshots/*.json`.
///
/// Executa após `HabilidadesSeed.Materializar()` e antes do `SaveChanges` inicial:
/// para cada habilidade cujo `NomeOriginal` bate com uma habilidade do snapshot,
/// constrói 5 `NivelDeHabilidade` (para combate: com `ModificadorDano/Acerto/Critico`
/// e `ValorDeEfeito` por nível; para acampamento: 5 níveis idênticos com
/// `CustoDeDescanso` replicado).
///
/// Fallback: habilidades sem match no snapshot (ex.: Encourage/Wound Care/Pep Talk
/// shared globais) recebem 5 níveis idênticos derivados do próprio objeto para
/// satisfazer o invariante SC-010 (toda habilidade tem 5 níveis).
///
/// Cobre tasks T060–T081 num único ponto (evita 22 arquivos de seed manuais).
public static class AplicadorDeNiveisDoWikiSnapshot
{
    private const string DiretorioPadrao = "specs/005-auditoria-habilidades-mineradas/wiki-snapshots";

    private static readonly IReadOnlyDictionary<ClasseDeHeroi, string> SlugPorClasse = new Dictionary<ClasseDeHeroi, string>
    {
        [ClasseDeHeroi.Abominacao] = "abominacao",
        [ClasseDeHeroi.Antiquario] = "antiquario",
        [ClasseDeHeroi.Bandido] = "bandido",
        [ClasseDeHeroi.Besteiro] = "besteiro",
        [ClasseDeHeroi.BoboDaCorte] = "bobo-da-corte",
        [ClasseDeHeroi.CacadorDeRecompensas] = "cacador-de-recompensas",
        [ClasseDeHeroi.Cruzado] = "cruzado",
        [ClasseDeHeroi.Duelista] = "duelista",
        [ClasseDeHeroi.Flagelante] = "flagelante",
        [ClasseDeHeroi.Fugitivo] = "fugitivo",
        [ClasseDeHeroi.Infernal] = "infernal",
        [ClasseDeHeroi.LadraoDeCova] = "ladrao-de-cova",
        [ClasseDeHeroi.Leproso] = "leproso",
        [ClasseDeHeroi.MedicoDaPeste] = "medico-da-peste",
        [ClasseDeHeroi.MestreDeCaca] = "mestre-de-caca",
        [ClasseDeHeroi.Musqueteiro] = "musqueteiro",
        [ClasseDeHeroi.Ocultista] = "ocultista",
        [ClasseDeHeroi.Rompedor] = "rompedor",
        [ClasseDeHeroi.Vestal] = "vestal",
        [ClasseDeHeroi.Veterano] = "veterano",
    };

    private static readonly Regex RegexNumero = new(@"[-+]?\d+(?:\.\d+)?", RegexOptions.Compiled);

    /// Habilidades single-level por design (não geram Pendente ao cair no fallback).
    private static readonly HashSet<string> ShardedGlobaisSemNiveis = new(StringComparer.OrdinalIgnoreCase)
    {
        "Encourage", "Wound Care", "Pep Talk",
    };

    public sealed record ResultadoAplicacao(
        int Aplicadas,
        int Fallback,
        int Ignoradas,
        IReadOnlyList<string> HabilidadesEmFallback);

    /// Persiste os 5 níveis (e seus ValoresDeEfeito) via SQL bulk INSERT, ignorando o change tracker do EF —
    /// necessário porque owned types com Update() causam DbUpdateConcurrencyException quando o parent está Unchanged.
    /// Faz DELETE + INSERT numa transação. Retorna quantidade de linhas inseridas em NiveisDeHabilidade.
    public static int PersistirNiveisEmSql(DbContext context, IReadOnlyCollection<Habilidade> habilidades)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(habilidades);

        var comCincoNiveis = habilidades.Where(h => h.Niveis.Count == 5).ToList();
        if (comCincoNiveis.Count == 0)
        {
            return 0;
        }

        using var tx = context.Database.BeginTransaction();

        context.Database.ExecuteSqlRaw("DELETE FROM ValoresDeEfeitoDeNivel;");
        context.Database.ExecuteSqlRaw("DELETE FROM NiveisDeHabilidade;");

        // UPDATE em lote da coluna Descricao — reconcilia texto do seed com o snapshot da wiki.
        foreach (var loteHabilidades in Chunk(comCincoNiveis, 50))
        {
            foreach (var h in loteHabilidades)
            {
                context.Database.ExecuteSqlRaw(
                    "UPDATE Habilidades SET Descricao = @p0 WHERE Id = @p1",
                    new SqlParameter("@p0", SqlDbType.NVarChar, 400) { Value = h.Descricao },
                    new SqlParameter("@p1", SqlDbType.UniqueIdentifier) { Value = h.Id });
            }
        }

        // UPDATE campos escalares específicos do subtipo (Feature 005 convergence T156).
        foreach (var h in comCincoNiveis)
        {
            if (h is HabilidadeDeCombate combate)
            {
                context.Database.ExecuteSqlRaw(
                    "UPDATE Habilidades SET AlvoEmArea = @p0, ModificadorDano = @p1, ModificadorAcerto = @p2, ModificadorCritico = @p3 WHERE Id = @p4",
                    new SqlParameter("@p0", SqlDbType.Bit) { Value = combate.AlvoEmArea },
                    new SqlParameter("@p1", SqlDbType.Decimal) { Precision = 9, Scale = 2, Value = combate.ModificadorDano },
                    new SqlParameter("@p2", SqlDbType.Decimal) { Precision = 9, Scale = 2, Value = combate.ModificadorAcerto },
                    new SqlParameter("@p3", SqlDbType.Decimal) { Precision = 9, Scale = 2, Value = combate.ModificadorCritico },
                    new SqlParameter("@p4", SqlDbType.UniqueIdentifier) { Value = combate.Id });
            }
            else if (h is HabilidadeDeAcampamento acamp)
            {
                context.Database.ExecuteSqlRaw(
                    "UPDATE Habilidades SET CustoDeDescanso = @p0, Alvo = @p1 WHERE Id = @p2",
                    new SqlParameter("@p0", SqlDbType.Int) { Value = acamp.CustoDeDescanso },
                    new SqlParameter("@p1", SqlDbType.NVarChar, 30) { Value = acamp.Alvo.ToString() },
                    new SqlParameter("@p2", SqlDbType.UniqueIdentifier) { Value = acamp.Id });
            }
        }

        // Feature 005 T156: DELETE + INSERT dos owned effects para sincronizar com o seed atualizado.
        context.Database.ExecuteSqlRaw("DELETE FROM EfeitosHabilidadeCombate;");
        context.Database.ExecuteSqlRaw("DELETE FROM EfeitosHabilidadeAcampamento;");
        foreach (var h in comCincoNiveis)
        {
            IEnumerable<EfeitoDeHabilidade> efeitos;
            string tabela;
            string fkColuna;
            if (h is HabilidadeDeCombate c)
            {
                efeitos = c.Efeitos;
                tabela = "EfeitosHabilidadeCombate";
                fkColuna = "HabilidadeDeCombateId";
            }
            else if (h is HabilidadeDeAcampamento a)
            {
                efeitos = a.Efeitos;
                tabela = "EfeitosHabilidadeAcampamento";
                fkColuna = "HabilidadeDeAcampamentoId";
            }
            else
            {
                continue;
            }

            var idxEfeito = 1;
            foreach (var e in efeitos)
            {
                var sql = $"INSERT INTO {tabela} ({fkColuna}, NomeDoEfeito, Alvo, Valor, Unidade, ChanceBase, DuracaoEmRodadas) VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6)";
                context.Database.ExecuteSqlRaw(sql,
                    new SqlParameter("@p0", SqlDbType.UniqueIdentifier) { Value = h.Id },
                    new SqlParameter("@p1", SqlDbType.NVarChar, 60) { Value = e.NomeDoEfeito },
                    new SqlParameter("@p2", SqlDbType.NVarChar, 30) { Value = e.Alvo.ToString() },
                    new SqlParameter("@p3", SqlDbType.Decimal) { Precision = 9, Scale = 2, Value = e.Valor },
                    new SqlParameter("@p4", SqlDbType.NVarChar, 30) { Value = e.Unidade.ToString() },
                    new SqlParameter("@p5", SqlDbType.Decimal) { Precision = 5, Scale = 2, Value = e.ChanceBase },
                    new SqlParameter("@p6", SqlDbType.Int) { IsNullable = true, Value = (object?)e.DuracaoEmRodadas ?? DBNull.Value });
                idxEfeito++;
            }
        }

        var niveisInseridos = 0;

        // Batches de ~200 níveis por INSERT (SQL Server aceita até 1000 rows por VALUES).
        foreach (var loteHabilidades in Chunk(comCincoNiveis, 40))
        {
            var sb = new StringBuilder(
                "INSERT INTO NiveisDeHabilidade (HabilidadeId, NumeroDoNivel, ModificadorDano, ModificadorAcerto, ModificadorCritico, CustoDeDescanso) VALUES ");
            var parms = new List<SqlParameter>();
            int idx = 0;

            foreach (var h in loteHabilidades)
            {
                foreach (var n in h.Niveis)
                {
                    if (idx > 0)
                    {
                        sb.Append(',');
                    }

                    sb.Append("(@p").Append(idx * 6).Append(",@p").Append(idx * 6 + 1)
                      .Append(",@p").Append(idx * 6 + 2).Append(",@p").Append(idx * 6 + 3)
                      .Append(",@p").Append(idx * 6 + 4).Append(",@p").Append(idx * 6 + 5).Append(')');

                    parms.Add(new SqlParameter($"@p{idx * 6}", SqlDbType.UniqueIdentifier) { Value = h.Id });
                    parms.Add(new SqlParameter($"@p{idx * 6 + 1}", SqlDbType.Int) { Value = n.NumeroDoNivel });
                    parms.Add(new SqlParameter($"@p{idx * 6 + 2}", SqlDbType.Decimal) { Precision = 9, Scale = 2, Value = n.ModificadorDano });
                    parms.Add(new SqlParameter($"@p{idx * 6 + 3}", SqlDbType.Decimal) { Precision = 9, Scale = 2, Value = n.ModificadorAcerto });
                    parms.Add(new SqlParameter($"@p{idx * 6 + 4}", SqlDbType.Decimal) { Precision = 9, Scale = 2, Value = n.ModificadorCritico });
                    parms.Add(new SqlParameter($"@p{idx * 6 + 5}", SqlDbType.Int) { IsNullable = true, Value = (object?)n.CustoDeDescanso ?? DBNull.Value });
                    idx++;
                }
            }

            if (idx > 0)
            {
                niveisInseridos += context.Database.ExecuteSqlRaw(sb.ToString(), parms.ToArray());
            }
        }

        foreach (var loteHabilidades in Chunk(comCincoNiveis, 25))
        {
            var sb = new StringBuilder(
                "INSERT INTO ValoresDeEfeitoDeNivel (NivelDeHabilidadeHabilidadeId, NivelDeHabilidadeNumeroDoNivel, TipoDoEfeito, Valor, Chance) VALUES ");
            var parms = new List<SqlParameter>();
            int idx = 0;

            foreach (var h in loteHabilidades)
            {
                foreach (var n in h.Niveis)
                {
                    foreach (var v in n.ValoresDeEfeito)
                    {
                        if (idx > 0)
                        {
                            sb.Append(',');
                        }

                        sb.Append("(@p").Append(idx * 5).Append(",@p").Append(idx * 5 + 1)
                          .Append(",@p").Append(idx * 5 + 2).Append(",@p").Append(idx * 5 + 3)
                          .Append(",@p").Append(idx * 5 + 4).Append(')');

                        parms.Add(new SqlParameter($"@p{idx * 5}", SqlDbType.UniqueIdentifier) { Value = h.Id });
                        parms.Add(new SqlParameter($"@p{idx * 5 + 1}", SqlDbType.Int) { Value = n.NumeroDoNivel });
                        parms.Add(new SqlParameter($"@p{idx * 5 + 2}", SqlDbType.NVarChar, 60) { Value = v.TipoDoEfeito });
                        parms.Add(new SqlParameter($"@p{idx * 5 + 3}", SqlDbType.Decimal) { Precision = 9, Scale = 2, Value = v.Valor });
                        parms.Add(new SqlParameter($"@p{idx * 5 + 4}", SqlDbType.Decimal) { Precision = 5, Scale = 2, IsNullable = true, Value = (object?)v.Chance ?? DBNull.Value });
                        idx++;
                    }
                }
            }

            if (idx > 0)
            {
                context.Database.ExecuteSqlRaw(sb.ToString(), parms.ToArray());
            }
        }

        tx.Commit();
        return niveisInseridos;
    }

    private static IEnumerable<List<T>> Chunk<T>(IEnumerable<T> source, int tamanho)
    {
        var lote = new List<T>(tamanho);
        foreach (var item in source)
        {
            lote.Add(item);
            if (lote.Count >= tamanho)
            {
                yield return lote;
                lote = new List<T>(tamanho);
            }
        }

        if (lote.Count > 0)
        {
            yield return lote;
        }
    }

    /// Aplica os 5 níveis em cada habilidade. Habilidades já com `Niveis.Count == 5` são puladas.
    public static ResultadoAplicacao Aplicar(IReadOnlyCollection<Habilidade> habilidades, string? diretorioSnapshots = null)
    {
        ArgumentNullException.ThrowIfNull(habilidades);

        var diretorio = diretorioSnapshots ?? ResolverDiretorio();
        if (diretorio is null)
        {
            return new ResultadoAplicacao(0, 0, habilidades.Count, Array.Empty<string>());
        }

        var porNomeOriginal = habilidades.ToDictionary(h => h.NomeOriginal, h => h, StringComparer.OrdinalIgnoreCase);
        int aplicadas = 0;

        var slugsAProcessar = SlugPorClasse.Values.Concat(new[] { "compartilhadas" });
        foreach (var slug in slugsAProcessar)
        {
            var caminho = Path.Combine(diretorio, slug + ".json");
            if (!File.Exists(caminho))
            {
                continue;
            }

            using var doc = JsonDocument.Parse(File.ReadAllText(caminho));
            if (!doc.RootElement.TryGetProperty("habilidades", out var listaHabilidades) || listaHabilidades.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (var elemento in listaHabilidades.EnumerateArray())
            {
                if (!elemento.TryGetProperty("nomeOriginal", out var nomeProp) || nomeProp.ValueKind != JsonValueKind.String)
                {
                    continue;
                }

                var nomeOriginal = nomeProp.GetString();
                if (string.IsNullOrWhiteSpace(nomeOriginal) || !porNomeOriginal.TryGetValue(nomeOriginal, out var alvo))
                {
                    continue;
                }

                if (elemento.TryGetProperty("descricao", out var descProp) && descProp.ValueKind == JsonValueKind.String)
                {
                    var descricaoWiki = descProp.GetString();
                    if (!string.IsNullOrWhiteSpace(descricaoWiki) && !string.Equals(descricaoWiki, alvo.Descricao, StringComparison.Ordinal))
                    {
                        alvo.DefinirDescricao(descricaoWiki.Length > 400 ? descricaoWiki[..400] : descricaoWiki);
                    }
                }

                if (alvo.Niveis.Count == 5)
                {
                    continue;
                }

                var niveis = ConstruirNiveis(elemento);
                if (niveis is null || niveis.Count != 5)
                {
                    continue;
                }

                alvo.DefinirNiveis(niveis);
                aplicadas++;
            }
        }

        int fallback = 0;
        var nomesEmFallback = new List<string>();
        foreach (var habilidade in habilidades)
        {
            if (habilidade.Niveis.Count == 5)
            {
                continue;
            }

            habilidade.DefinirNiveis(NiveisFallback(habilidade));
            fallback++;
            nomesEmFallback.Add(habilidade.NomeOriginal);
        }

        return new ResultadoAplicacao(aplicadas, fallback, 0, nomesEmFallback);
    }

    /// FR-003b: registra entradas `Pendente` em `MapaDeCobertura` para habilidades cujos Level 2..5 caíram no fallback
    /// (ou seja, wiki não publicou dados por nível). As shared globais single-level (Encourage/Wound Care/Pep Talk)
    /// são ignoradas — são declaradas single-level por design do jogo.
    /// Retorna a quantidade de entradas inseridas em MapaDeCobertura.
    public static int RegistrarPendenciasDeNivelNoMapa(DbContext context, ResultadoAplicacao resultado, IReadOnlyCollection<Habilidade> habilidades)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(resultado);
        ArgumentNullException.ThrowIfNull(habilidades);

        var candidatos = resultado.HabilidadesEmFallback
            .Where(nome => !ShardedGlobaisSemNiveis.Contains(nome))
            .ToArray();
        if (candidatos.Length == 0)
        {
            return 0;
        }

        // Descobre a Classe de cada habilidade em fallback via associação ClassesHabilidades.
        var porNome = habilidades.ToDictionary(h => h.NomeOriginal, h => h.Id, StringComparer.OrdinalIgnoreCase);
        var vinculos = context.Set<ClasseHabilidade>().AsNoTracking();
        var classes = context.Set<Classe>().AsNoTracking().ToDictionary(c => c.Id, c => c.ClasseDeHeroi);

        int inseridos = 0;
        foreach (var nome in candidatos)
        {
            if (!porNome.TryGetValue(nome, out var idHab))
            {
                continue;
            }
            var idsClasse = vinculos.Where(ch => ch.HabilidadeId == idHab).Select(ch => ch.ClasseId).ToArray();
            foreach (var idClasse in idsClasse)
            {
                if (!classes.TryGetValue(idClasse, out var classeEnum))
                {
                    continue;
                }
                for (int nivel = 2; nivel <= 5; nivel++)
                {
                    var chave = $"{nome}:L{nivel}";
                    var jaExiste = context.Set<Domain.Cobertura.EntradaDoMapaDeCobertura>()
                        .Any(m => m.Classe == classeEnum
                                  && m.Categoria == Domain.Cobertura.CategoriaDeCobertura.NivelDeHabilidade
                                  && m.ChaveDoAtributo == chave);
                    if (jaExiste)
                    {
                        continue;
                    }
                    context.Set<Domain.Cobertura.EntradaDoMapaDeCobertura>().Add(new Domain.Cobertura.EntradaDoMapaDeCobertura(
                        classeEnum,
                        Domain.Cobertura.CategoriaDeCobertura.NivelDeHabilidade,
                        chave,
                        Domain.Cobertura.EstadoDeAtributo.Pendente));
                    inseridos++;
                }
            }
        }

        if (inseridos > 0)
        {
            context.SaveChanges();
        }

        return inseridos;
    }

    private static IReadOnlyList<NivelDeHabilidade>? ConstruirNiveis(JsonElement elemento)
    {
        var tipo = elemento.TryGetProperty("tipo", out var t) && t.ValueKind == JsonValueKind.String ? t.GetString() : null;
        return string.Equals(tipo, "Acampamento", StringComparison.OrdinalIgnoreCase)
            ? ConstruirNiveisAcampamento(elemento)
            : ConstruirNiveisCombate(elemento);
    }

    private static IReadOnlyList<NivelDeHabilidade> ConstruirNiveisCombate(JsonElement elemento)
    {
        var dano = new decimal?[5];
        var acerto = new decimal?[5];
        var critico = new decimal?[5];

        if (elemento.TryGetProperty("campos", out var campos) && campos.ValueKind == JsonValueKind.Object)
        {
            LerArrayDecimal(campos, "modificadorDano", dano);
            LerArrayDecimal(campos, "modificadorAcerto", acerto);
            LerArrayDecimal(campos, "modificadorCritico", critico);
        }

        var efeitosPorNivel = new List<ValorDeEfeito>[5];
        for (int i = 0; i < 5; i++)
        {
            efeitosPorNivel[i] = new List<ValorDeEfeito>();
        }

        if (elemento.TryGetProperty("efeitos", out var efeitos) && efeitos.ValueKind == JsonValueKind.Array)
        {
            foreach (var efeito in efeitos.EnumerateArray())
            {
                var tipoDoEfeito = efeito.TryGetProperty("tipoDoEfeito", out var tp) && tp.ValueKind == JsonValueKind.String
                    ? tp.GetString()
                    : null;
                if (string.IsNullOrWhiteSpace(tipoDoEfeito))
                {
                    continue;
                }

                var valores = new decimal?[5];
                LerArrayDecimal(efeito, "valorPorNivel", valores);

                var chances = new decimal?[5];
                if (efeito.TryGetProperty("chancePorNivel", out _))
                {
                    LerArrayDecimal(efeito, "chancePorNivel", chances);
                }

                for (int i = 0; i < 5; i++)
                {
                    if (valores[i] is null)
                    {
                        continue;
                    }

                    var chance = ClampChance(chances[i]);
                    efeitosPorNivel[i].Add(new ValorDeEfeito(TruncarTipo(tipoDoEfeito!), valores[i]!.Value, chance));
                }
            }
        }

        var niveis = new List<NivelDeHabilidade>(5);
        for (int i = 0; i < 5; i++)
        {
            niveis.Add(new NivelDeHabilidade(
                numeroDoNivel: i + 1,
                modificadorDano: dano[i] ?? 0m,
                modificadorAcerto: acerto[i] ?? 0m,
                modificadorCritico: critico[i] ?? 0m,
                valoresDeEfeito: efeitosPorNivel[i]));
        }

        return niveis;
    }

    private static IReadOnlyList<NivelDeHabilidade> ConstruirNiveisAcampamento(JsonElement elemento)
    {
        int? custo = null;
        if (elemento.TryGetProperty("custoDeDescanso", out var c) && c.ValueKind == JsonValueKind.Number && c.TryGetInt32(out var ci) && ci is >= 0 and <= 20)
        {
            custo = ci;
        }

        var efeitos = new List<ValorDeEfeito>();
        if (elemento.TryGetProperty("efeitos", out var efeitosDoc) && efeitosDoc.ValueKind == JsonValueKind.Array)
        {
            foreach (var efeito in efeitosDoc.EnumerateArray())
            {
                var tipoDoEfeito = efeito.TryGetProperty("tipoDoEfeito", out var tp) && tp.ValueKind == JsonValueKind.String
                    ? tp.GetString()
                    : null;
                if (string.IsNullOrWhiteSpace(tipoDoEfeito))
                {
                    continue;
                }

                decimal? valor = null;
                if (efeito.TryGetProperty("valor", out var v))
                {
                    valor = ExtrairDecimal(v);
                }

                efeitos.Add(new ValorDeEfeito(TruncarTipo(tipoDoEfeito!), valor ?? 0m));
            }
        }

        var niveis = new List<NivelDeHabilidade>(5);
        for (int i = 0; i < 5; i++)
        {
            // Camping skills são single-level no jogo — replicamos os mesmos valores em L1..L5.
            var copia = efeitos.Select(e => new ValorDeEfeito(e.TipoDoEfeito, e.Valor, e.Chance)).ToList();
            niveis.Add(new NivelDeHabilidade(
                numeroDoNivel: i + 1,
                modificadorDano: 0m,
                modificadorAcerto: 0m,
                modificadorCritico: 0m,
                custoDeDescanso: custo,
                valoresDeEfeito: copia));
        }

        return niveis;
    }

    private static IReadOnlyList<NivelDeHabilidade> NiveisFallback(Habilidade habilidade)
    {
        // Usado para habilidades sem match no snapshot (compartilhadas globais: Encourage/Wound Care/Pep Talk).
        decimal dano = 0m, acerto = 0m, critico = 0m;
        int? custo = null;

        if (habilidade is HabilidadeDeCombate combate)
        {
            dano = combate.ModificadorDano;
            acerto = combate.ModificadorAcerto;
            critico = combate.ModificadorCritico;
        }
        else if (habilidade is HabilidadeDeAcampamento acampamento)
        {
            custo = acampamento.CustoDeDescanso;
        }

        var niveis = new List<NivelDeHabilidade>(5);
        for (int i = 0; i < 5; i++)
        {
            niveis.Add(new NivelDeHabilidade(
                numeroDoNivel: i + 1,
                modificadorDano: dano,
                modificadorAcerto: acerto,
                modificadorCritico: critico,
                custoDeDescanso: custo));
        }

        return niveis;
    }

    private static void LerArrayDecimal(JsonElement pai, string prop, decimal?[] destino)
    {
        if (!pai.TryGetProperty(prop, out var arr) || arr.ValueKind != JsonValueKind.Array)
        {
            return;
        }

        int i = 0;
        foreach (var el in arr.EnumerateArray())
        {
            if (i >= destino.Length)
            {
                break;
            }

            destino[i] = ExtrairDecimal(el);
            i++;
        }
    }

    private static decimal? ExtrairDecimal(JsonElement el)
    {
        switch (el.ValueKind)
        {
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return null;
            case JsonValueKind.Number:
                return el.GetDecimal();
            case JsonValueKind.True:
                return 1m;
            case JsonValueKind.False:
                return 0m;
            case JsonValueKind.String:
                var s = el.GetString();
                if (string.IsNullOrWhiteSpace(s))
                {
                    return null;
                }

                var match = RegexNumero.Match(s);
                if (match.Success && decimal.TryParse(match.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var d))
                {
                    return d;
                }

                return null;
            default:
                return null;
        }
    }

    private static decimal? ClampChance(decimal? chance)
    {
        if (chance is null)
        {
            return null;
        }

        if (chance < 0m)
        {
            return 0m;
        }

        if (chance > 200m)
        {
            return 200m;
        }

        return chance;
    }

    private static string TruncarTipo(string tipo)
    {
        var t = tipo.Trim();
        return t.Length > 60 ? t[..60] : t;
    }

    private static string? ResolverDiretorio()
    {
        var atual = AppContext.BaseDirectory;
        for (int nivel = 0; nivel < 8; nivel++)
        {
            var candidato = Path.Combine(atual, DiretorioPadrao);
            if (Directory.Exists(candidato))
            {
                return candidato;
            }

            var pai = Directory.GetParent(atual)?.FullName;
            if (pai is null)
            {
                return null;
            }

            atual = pai;
        }

        return null;
    }
}
