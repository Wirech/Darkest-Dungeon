using System.Text.Json;
using DarkestDungeon.Application.Midias;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Cobertura;
using DarkestDungeon.Domain.Habilidades;
using DarkestDungeon.Domain.Itens;
using DarkestDungeon.Domain.Personagens;
using Microsoft.Extensions.Options;

namespace DarkestDungeon.Infrastructure.Midias;

public sealed class ResolvedorDeMidiasDoCard : IResolvedorDeMidiasDoCard
{
    private static readonly JsonSerializerOptions Json = new() { PropertyNameCaseInsensitive = true };

    internal static readonly IReadOnlyDictionary<ClasseDeHeroi, string> PrefixoNoInventario = new Dictionary<ClasseDeHeroi, string>
    {
        [ClasseDeHeroi.Abominacao] = "abomination",
        [ClasseDeHeroi.Antiquario] = "antiquarian",
        [ClasseDeHeroi.Besteiro] = "arbalest",
        [ClasseDeHeroi.CacadorDeRecompensas] = "bounty_hunter",
        [ClasseDeHeroi.Cruzado] = "crusader",
        [ClasseDeHeroi.LadraoDeCova] = "grave_robber",
        [ClasseDeHeroi.BoboDaCorte] = "jester",
        [ClasseDeHeroi.MestreDeCaca] = "houndmaster",
        [ClasseDeHeroi.Leproso] = "leper",
        [ClasseDeHeroi.Infernal] = "hellion",
        [ClasseDeHeroi.Bandido] = "highwayman",
        [ClasseDeHeroi.Musqueteiro] = "musketeer",
        [ClasseDeHeroi.Veterano] = "man_at_arms",
        [ClasseDeHeroi.Ocultista] = "occultist",
        [ClasseDeHeroi.MedicoDaPeste] = "plague_doctor",
        [ClasseDeHeroi.Vestal] = "vestal",
        [ClasseDeHeroi.Flagelante] = "flagellant",
        [ClasseDeHeroi.Rompedor] = "shieldbreaker",
        [ClasseDeHeroi.Duelista] = "duelist",
        [ClasseDeHeroi.Fugitivo] = "runaway",
    };

    internal static readonly IReadOnlyDictionary<ClasseDeHeroi, string> PastaNoInventario = new Dictionary<ClasseDeHeroi, string>
    {
        [ClasseDeHeroi.Abominacao] = "abominacao",
        [ClasseDeHeroi.Antiquario] = "antiquario",
        [ClasseDeHeroi.Besteiro] = "besteiro",
        [ClasseDeHeroi.CacadorDeRecompensas] = "cacador-de-recompensas",
        [ClasseDeHeroi.Cruzado] = "cruzado",
        [ClasseDeHeroi.LadraoDeCova] = "ladrao-de-cova",
        [ClasseDeHeroi.BoboDaCorte] = "bobo-da-corte",
        [ClasseDeHeroi.MestreDeCaca] = "mestre-de-caca",
        [ClasseDeHeroi.Leproso] = "leproso",
        [ClasseDeHeroi.Infernal] = "infernal",
        [ClasseDeHeroi.Bandido] = "bandido",
        [ClasseDeHeroi.Musqueteiro] = "musqueteiro",
        [ClasseDeHeroi.Veterano] = "veterano",
        [ClasseDeHeroi.Ocultista] = "ocultista",
        [ClasseDeHeroi.MedicoDaPeste] = "medico-da-peste",
        [ClasseDeHeroi.Vestal] = "vestal",
        [ClasseDeHeroi.Flagelante] = "flagelante",
        [ClasseDeHeroi.Rompedor] = "rompedor",
        [ClasseDeHeroi.Duelista] = "duelista",
        [ClasseDeHeroi.Fugitivo] = "fugitivo",
    };

    private readonly IndiceDoAcervo indice;

    public ResolvedorDeMidiasDoCard(IOptions<OpcoesDeAcervoDoCard> opcoes)
    {
        indice = IndiceDoAcervo.Carregar(opcoes.Value, Directory.GetCurrentDirectory());
    }

    public SlotDeMidiaDoCardDto ResolverRetrato(ClasseDeHeroi classe, AparenciaDePersonagem aparencia, Classe? catalogo)
    {
        var asset = catalogo?.Assets.FirstOrDefault(a => a.Aparencia == aparencia);
        if (asset is { Status: EstadoDeAtributo.Coletado } && !string.IsNullOrWhiteSpace(asset.ConjuntoSpineId))
        {
            var encontrado = indice.LocalizarPorCaminho(asset.ConjuntoSpineId);
            if (encontrado is not null)
            {
                return SlotDeMidiaDoCard.Ok(
                    SlotDeMidiaDoCard.TipoRetrato,
                    SlotDeMidiaDoCard.RotuloRetrato,
                    encontrado.CaminhoDestino,
                    encontrado.Sha256 ?? asset.HashArquivo,
                    encontrado.Url);
            }
        }

        return ResolverPorCaminhoEsperado(
            SlotDeMidiaDoCard.TipoRetrato,
            SlotDeMidiaDoCard.RotuloRetrato,
            classe,
            caminho => CaminhoNaPaleta(caminho, classe, aparencia) && caminho.Contains("portrait_roster", StringComparison.OrdinalIgnoreCase));
    }

    public SlotDeMidiaDoCardDto ResolverCorpoInteiro(ClasseDeHeroi classe, AparenciaDePersonagem aparencia)
    {
        var idle = ResolverConjuntoDeCorpo(classe, aparencia, SlotDeMidiaDoCard.CicloIdle);
        var walk = ResolverConjuntoDeCorpo(classe, aparencia, SlotDeMidiaDoCard.CicloWalk);
        var extras = ResolverConjuntosExtras(classe, aparencia);
        var versoes = MontarVersoes(idle is not null, walk is not null, extras);

        if (idle is null)
        {
            return SlotDeMidiaDoCard.Pendente(
                SlotDeMidiaDoCard.TipoCorpoInteiro,
                SlotDeMidiaDoCard.RotuloCorpoInteiro,
                conjuntoWalk: walk?.Conjunto,
                versoes: versoes,
                conjuntos: extras);
        }

        return SlotDeMidiaDoCard.Ok(
            SlotDeMidiaDoCard.TipoCorpoInteiro,
            SlotDeMidiaDoCard.RotuloCorpoInteiro,
            idle.Textura.CaminhoDestino,
            idle.Textura.Sha256,
            idle.Textura.Url,
            conjuntoIdle: idle.Conjunto,
            conjuntoWalk: walk?.Conjunto,
            versoes: versoes,
            conjuntos: extras);
    }

    private ConjuntoResolvido? ResolverConjuntoDeCorpo(ClasseDeHeroi classe, AparenciaDePersonagem aparencia, string ciclo)
    {
        if (!PrefixoNoInventario.TryGetValue(classe, out var prefixo))
        {
            return null;
        }

        var pasta = PastaNoInventario.TryGetValue(classe, out var pastaPt) ? pastaPt : prefixo;
        var stem = $"{prefixo}.sprite.{ciclo}";
        var atlas = indice.LocalizarPorCaminho($"arquivos/{pasta}/anim/{stem}.atlas");
        var esqueleto = indice.LocalizarPorCaminho($"arquivos/{pasta}/anim/{stem}.skel");
        var textura = indice.Localizar(
            classe,
            caminho => CaminhoNaPaleta(caminho, classe, aparencia)
                       && caminho.Contains($"/{stem}.png", StringComparison.OrdinalIgnoreCase)
                       && caminho.Contains("/anim/", StringComparison.OrdinalIgnoreCase));

        if (atlas is null || esqueleto is null || textura is null)
        {
            return null;
        }

        if (atlas.CaminhoDestino.Contains($"/{prefixo}_", StringComparison.OrdinalIgnoreCase)
            || esqueleto.CaminhoDestino.Contains($"/{prefixo}_", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return new ConjuntoResolvido(
            textura,
            new ConjuntoDeCorpoDto(ciclo, atlas.Url, esqueleto.Url, textura.Url));
    }

    private IReadOnlyList<ConjuntoDeCorpoDto> ResolverConjuntosExtras(ClasseDeHeroi classe, AparenciaDePersonagem aparencia)
    {
        if (!PrefixoNoInventario.TryGetValue(classe, out var prefixo))
        {
            return [];
        }

        var pasta = PastaNoInventario.TryGetValue(classe, out var pastaPt) ? pastaPt : prefixo;
        var extraidos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var conjuntos = new List<ConjuntoDeCorpoDto>();
        var marcador = $"arquivos/{pasta}/anim/{prefixo}.sprite.";

        foreach (var arquivo in indice.Arquivos)
        {
            var caminho = arquivo.CaminhoDestino.Replace('\\', '/');
            var indiceMarcador = caminho.IndexOf(marcador, StringComparison.OrdinalIgnoreCase);
            if (indiceMarcador < 0 || !caminho.EndsWith(".skel", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var ciclo = caminho[(indiceMarcador + marcador.Length)..^".skel".Length];
            if (string.IsNullOrWhiteSpace(ciclo)
                || ciclo.Contains('/', StringComparison.Ordinal)
                || ciclo.Equals(SlotDeMidiaDoCard.CicloIdle, StringComparison.OrdinalIgnoreCase)
                || ciclo.Equals(SlotDeMidiaDoCard.CicloWalk, StringComparison.OrdinalIgnoreCase)
                || !extraidos.Add(ciclo))
            {
                continue;
            }

            var resolvido = ResolverConjuntoDeCorpo(classe, aparencia, ciclo);
            if (resolvido is not null)
            {
                conjuntos.Add(resolvido.Conjunto);
            }
        }

        return conjuntos
            .OrderBy(c => SlotDeMidiaDoCard.OrdemDoCiclo(c.Ciclo))
            .ThenBy(c => c.Ciclo, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IReadOnlyList<VersaoDoCorpoDto> MontarVersoes(
        bool idleCompleto,
        bool walkCompleto,
        IReadOnlyList<ConjuntoDeCorpoDto> extras)
    {
        var versoes = new List<VersaoDoCorpoDto>
        {
            new(SlotDeMidiaDoCard.IdEmEspera, SlotDeMidiaDoCard.RotuloEmEspera, idleCompleto),
            new(SlotDeMidiaDoCard.IdAnimado, SlotDeMidiaDoCard.RotuloAnimado, idleCompleto),
            new(SlotDeMidiaDoCard.IdCaminhada, SlotDeMidiaDoCard.RotuloCaminhada, walkCompleto),
        };

        foreach (var extra in extras)
        {
            versoes.Add(new VersaoDoCorpoDto(extra.Ciclo, SlotDeMidiaDoCard.RotuloDoCiclo(extra.Ciclo), true));
        }

        return versoes;
    }

    private sealed record ConjuntoResolvido(ArquivoIndexado Textura, ConjuntoDeCorpoDto Conjunto);

    public SlotDeMidiaDoCardDto ResolverArma(ClasseDeHeroi classe, int? nivelDaArma, Arma? arma) =>
        ResolverEquipamento(
            SlotDeMidiaDoCard.TipoArma,
            SlotDeMidiaDoCard.RotuloArma,
            classe,
            nivelDaArma,
            arma?.Niveis.FirstOrDefault(n => n.Nivel == nivelDaArma)?.Midia,
            "eqp_weapon_");

    public SlotDeMidiaDoCardDto ResolverArmadura(ClasseDeHeroi classe, int? nivelDaArmadura, Armadura? armadura) =>
        ResolverEquipamento(
            SlotDeMidiaDoCard.TipoArmadura,
            SlotDeMidiaDoCard.RotuloArmadura,
            classe,
            nivelDaArmadura,
            armadura?.Niveis.FirstOrDefault(n => n.Nivel == nivelDaArmadura)?.Midia,
            "eqp_armour_");

    public SlotDeMidiaDoCardDto ResolverHabilidade(ClasseDeHeroi classe, Habilidade habilidade)
    {
        var rotulo = string.IsNullOrWhiteSpace(habilidade.NomeExibicao) ? "Habilidade" : habilidade.NomeExibicao;
        if (indice.TentarArquivoDeHabilidade(classe, habilidade.NomeOriginal, out var arquivo) && arquivo is not null)
        {
            return SlotDeMidiaDoCard.Ok(
                SlotDeMidiaDoCard.TipoHabilidade,
                rotulo,
                arquivo.CaminhoDestino,
                arquivo.Sha256,
                arquivo.Url,
                habilidadeId: habilidade.Id);
        }

        return SlotDeMidiaDoCard.Pendente(SlotDeMidiaDoCard.TipoHabilidade, rotulo, habilidadeId: habilidade.Id);
    }

    private SlotDeMidiaDoCardDto ResolverEquipamento(
        string tipo,
        string rotulo,
        ClasseDeHeroi classe,
        int? nivel,
        MidiaDeItem? midia,
        string prefixoArquivo)
    {
        if (nivel is null or < 1 or > 5)
        {
            return SlotDeMidiaDoCard.Pendente(tipo, rotulo);
        }

        if (midia is { Status: StatusDeMidia.OK } && !string.IsNullOrWhiteSpace(midia.ArquivoInventarioId))
        {
            var vinculado = indice.LocalizarPorIdOuCaminho(midia.ArquivoInventarioId, midia.HashArquivo);
            if (vinculado is not null)
            {
                return SlotDeMidiaDoCard.Ok(tipo, rotulo, vinculado.CaminhoDestino, vinculado.Sha256 ?? midia.HashArquivo, vinculado.Url, nivel);
            }
        }

        var indiceArquivo = nivel.Value - 1;
        var trecho = $"{prefixoArquivo}{indiceArquivo}.png";
        return ResolverPorCaminhoEsperado(
            tipo,
            rotulo,
            classe,
            caminho => caminho.Contains(trecho, StringComparison.OrdinalIgnoreCase)
                       && CaminhoDaClasse(caminho, classe),
            nivel);
    }

    private SlotDeMidiaDoCardDto ResolverPorCaminhoEsperado(
        string tipo,
        string rotulo,
        ClasseDeHeroi classe,
        Func<string, bool> predicado,
        int? nivel = null)
    {
        var arquivo = indice.Localizar(classe, predicado);
        if (arquivo is null)
        {
            return SlotDeMidiaDoCard.Pendente(tipo, rotulo, nivel);
        }

        return SlotDeMidiaDoCard.Ok(tipo, rotulo, arquivo.CaminhoDestino, arquivo.Sha256, arquivo.Url, nivel);
    }

    private static bool CaminhoDaClasse(string caminho, ClasseDeHeroi classe)
    {
        if (!PrefixoNoInventario.TryGetValue(classe, out var prefixo))
        {
            return false;
        }

        var pasta = PastaNoInventario.TryGetValue(classe, out var pastaPt) ? pastaPt : prefixo;
        return caminho.Contains($"/{prefixo}", StringComparison.OrdinalIgnoreCase)
               || caminho.Contains($"{prefixo}_", StringComparison.OrdinalIgnoreCase)
               || caminho.Contains($"{prefixo}.", StringComparison.OrdinalIgnoreCase)
               || caminho.Contains($"/{pasta}/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool CaminhoNaPaleta(string caminho, ClasseDeHeroi classe, AparenciaDePersonagem aparencia)
    {
        if (!PrefixoNoInventario.TryGetValue(classe, out var prefixo))
        {
            return false;
        }

        var pasta = $"{prefixo}_{aparencia}";
        return caminho.Contains($"/{pasta}/", StringComparison.OrdinalIgnoreCase)
               || caminho.Contains($"{pasta}/", StringComparison.OrdinalIgnoreCase);
    }

    private sealed class IndiceDoAcervo
    {
        private readonly IReadOnlyList<ArquivoIndexado> arquivos;
        private readonly IReadOnlyDictionary<string, string> manifesto;

        private IndiceDoAcervo(IReadOnlyList<ArquivoIndexado> arquivos, IReadOnlyDictionary<string, string> manifesto)
        {
            this.arquivos = arquivos;
            this.manifesto = manifesto;
        }

        public IReadOnlyList<ArquivoIndexado> Arquivos => arquivos;

        public static IndiceDoAcervo Carregar(OpcoesDeAcervoDoCard opcoes, string contentRoot)
        {
            var herois = ResolverRaiz(opcoes.Herois, contentRoot)
                         ?? ResolverRaiz(opcoes.Herois, AppContext.BaseDirectory);
            var equipamentos = ResolverRaiz(opcoes.EquipamentosItens, contentRoot)
                               ?? ResolverRaiz(opcoes.EquipamentosItens, AppContext.BaseDirectory);
            var lista = new List<ArquivoIndexado>();
            if (herois is not null)
            {
                lista.AddRange(LerInventario(herois, "/acervo/herois/"));
            }

            if (equipamentos is not null)
            {
                lista.AddRange(LerInventario(equipamentos, "/acervo/equipamentos-itens/"));
            }

            var manifesto = LerManifesto(herois);
            return new IndiceDoAcervo(lista, manifesto);
        }

        public ArquivoIndexado? Localizar(ClasseDeHeroi classe, Func<string, bool> predicado) =>
            arquivos.FirstOrDefault(a => predicado(a.CaminhoDestino) && CaminhoDaClasse(a.CaminhoDestino, classe));

        public ArquivoIndexado? LocalizarPorCaminho(string caminho)
        {
            var normalizado = Normalizar(caminho);
            return arquivos.FirstOrDefault(a =>
                string.Equals(Normalizar(a.CaminhoDestino), normalizado, StringComparison.OrdinalIgnoreCase));
        }

        public ArquivoIndexado? LocalizarPorIdOuCaminho(string id, string? hash)
        {
            var porCaminho = LocalizarPorCaminho(id);
            if (porCaminho is not null)
            {
                return porCaminho;
            }

            if (!string.IsNullOrWhiteSpace(hash))
            {
                var porHash = arquivos.FirstOrDefault(a => string.Equals(a.Sha256, hash, StringComparison.OrdinalIgnoreCase));
                if (porHash is not null)
                {
                    return porHash;
                }
            }

            return arquivos.FirstOrDefault(a =>
                string.Equals(Path.GetFileName(a.CaminhoDestino), id, StringComparison.OrdinalIgnoreCase)
                || string.Equals(a.Identificador, id, StringComparison.OrdinalIgnoreCase));
        }

        public bool TentarArquivoDeHabilidade(ClasseDeHeroi classe, string nomeOriginal, out ArquivoIndexado? arquivo)
        {
            arquivo = null;
            if (!PrefixoNoInventario.TryGetValue(classe, out var prefixo))
            {
                return false;
            }

            var chave = $"{prefixo}|{nomeOriginal.Trim()}";
            if (!manifesto.TryGetValue(chave, out var nomeArquivo) && !manifesto.TryGetValue(nomeOriginal.Trim(), out nomeArquivo))
            {
                return false;
            }

            var ehCamping = Path.GetFileName(nomeArquivo).StartsWith("camp_skill_", StringComparison.OrdinalIgnoreCase);
            arquivo = arquivos.FirstOrDefault(a =>
                a.CaminhoDestino.EndsWith(nomeArquivo, StringComparison.OrdinalIgnoreCase)
                && (ehCamping || CaminhoDaClasse(a.CaminhoDestino, classe)));
            return arquivo is not null;
        }

        private static string? ResolverRaiz(string configurado, string? origem)
        {
            if (Path.IsPathRooted(configurado) && Directory.Exists(configurado))
            {
                return configurado;
            }

            if (string.IsNullOrWhiteSpace(origem))
            {
                return null;
            }

            var atual = origem;
            for (var nivel = 0; nivel < 10; nivel++)
            {
                var candidato = Path.GetFullPath(Path.Combine(atual, configurado));
                if (Directory.Exists(candidato) || File.Exists(Path.Combine(candidato, "inventario.json")))
                {
                    return candidato;
                }

                var pai = Directory.GetParent(atual);
                if (pai is null)
                {
                    break;
                }

                atual = pai.FullName;
            }

            return null;
        }

        private static IReadOnlyList<ArquivoIndexado> LerInventario(string raiz, string prefixoUrl)
        {
            var caminho = Path.Combine(raiz, "inventario.json");
            if (!File.Exists(caminho))
            {
                return [];
            }

            using var fluxo = File.OpenRead(caminho);
            var documento = JsonSerializer.Deserialize<DocumentoInventario>(fluxo, Json);
            if (documento?.Arquivos is null)
            {
                return [];
            }

            return documento.Arquivos
                .Where(a => !string.IsNullOrWhiteSpace(a.CaminhoDestino))
                .Select(a => new ArquivoIndexado(
                    Normalizar(a.CaminhoDestino!),
                    a.Sha256,
                    prefixoUrl + Normalizar(a.CaminhoDestino!).TrimStart('/'),
                    Path.GetFileNameWithoutExtension(a.CaminhoDestino)))
                .ToArray();
        }

        private static IReadOnlyDictionary<string, string> LerManifesto(string? raizHerois)
        {
            if (raizHerois is null)
            {
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            var caminho = Path.Combine(raizHerois, "manifesto-habilidades.json");
            if (!File.Exists(caminho))
            {
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            using var fluxo = File.OpenRead(caminho);
            var documento = JsonSerializer.Deserialize<DocumentoManifesto>(fluxo, Json);
            var mapa = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (documento?.Associacoes is null)
            {
                return mapa;
            }

            foreach (var associacao in documento.Associacoes)
            {
                if (string.IsNullOrWhiteSpace(associacao.Arquivo) || associacao.Habilidades is null)
                {
                    continue;
                }

                var prefixo = PrefixoDoArquivo(associacao.Arquivo);
                foreach (var habilidade in associacao.Habilidades.Where(n => !string.IsNullOrWhiteSpace(n)))
                {
                    mapa[$"{prefixo}|{habilidade.Trim()}"] = associacao.Arquivo;
                    mapa[habilidade.Trim()] = associacao.Arquivo;
                }
            }

            return mapa;
        }

        private static string PrefixoDoArquivo(string arquivo)
        {
            var ponto = arquivo.IndexOf('.', StringComparison.Ordinal);
            return ponto <= 0 ? arquivo : arquivo[..ponto];
        }

        private static string Normalizar(string caminho) => caminho.Replace('\\', '/').TrimStart('/');
    }

    private sealed record ArquivoIndexado(string CaminhoDestino, string? Sha256, string Url, string? Identificador);

    private sealed class DocumentoInventario
    {
        public List<ArquivoInventario>? Arquivos { get; set; }
    }

    private sealed class ArquivoInventario
    {
        public string? CaminhoDestino { get; set; }
        public string? Sha256 { get; set; }
    }

    private sealed class DocumentoManifesto
    {
        public List<AssociacaoManifesto>? Associacoes { get; set; }
    }

    private sealed class AssociacaoManifesto
    {
        public string? Classe { get; set; }
        public string? Arquivo { get; set; }
        public List<string>? Habilidades { get; set; }
    }
}
