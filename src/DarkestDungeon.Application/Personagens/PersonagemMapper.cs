using DarkestDungeon.Application.Midias;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Itens;
using DarkestDungeon.Domain.Habilidades;
using DarkestDungeon.Domain.Personagens;
using DarkestDungeon.Domain.Seres;

namespace DarkestDungeon.Application.Personagens;

internal static class PersonagemMapper
{
    public static PersonagemResumoDto ParaResumo(
        Personagem personagem,
        Classe? classe,
        IReadOnlyDictionary<Guid, Habilidade> habilidades,
        IResolvedorDeMidiasDoCard? resolvedor = null,
        IReadOnlyDictionary<Guid, Item>? itens = null)
    {
        var midias = MontarMidias(personagem, classe, resolvedor, itens);
        var (espaco1, espaco2, acessorios) = MapearEspacos(personagem, itens);
        var fichaBase = FichaEfetivaDePersonagem.DaBase(personagem);
        var fichaEfetiva = FichaEfetivaDePersonagem.Calcular(personagem, acessorios);
        return new PersonagemResumoDto(
            personagem.Id,
            personagem.Nome,
            personagem.Classe,
            classe?.NomeExibicao ?? personagem.Classe.ToString(),
            fichaEfetiva.HpAtual,
            fichaEfetiva.HpMaximo,
            personagem.Stress,
            personagem.Nivel,
            MapearHabilidades(personagem, habilidades, resolvedor),
            fichaEfetiva.Precisao,
            fichaEfetiva.Protecao,
            fichaEfetiva.Esquiva,
            fichaEfetiva.Velocidade,
            fichaEfetiva.Critico,
            fichaEfetiva.DanoBaseMinimo,
            fichaEfetiva.DanoBaseMaximo,
            personagem.PassosAFrente,
            personagem.PassosAtras,
            fichaEfetiva.Resistencias,
            personagem.NivelDaArma,
            personagem.NivelDaArmadura,
            personagem.Aparencia,
            classe?.Religiosa ?? false,
            classe?.ProvisaoInicial,
            classe?.BonusAoCriticoDaClasse ?? string.Empty,
            midias,
            espaco1,
            espaco2,
            fichaBase,
            fichaEfetiva);
    }

    public static PersonagemDetalheDto ParaDto(
        Personagem personagem,
        IReadOnlyDictionary<Guid, Item>? itens = null,
        IReadOnlyDictionary<Guid, Habilidade>? habilidades = null,
        Classe? classe = null,
        IResolvedorDeMidiasDoCard? resolvedor = null)
    {
        itens ??= new Dictionary<Guid, Item>();
        var arma = LocalizarArmaEquipada(personagem, itens);
        var armadura = LocalizarArmaduraEquipada(personagem, itens);
        MidiaDeEquipamentoDePersonagemDto? midiaArma = MapearMidiaDeArma(arma, personagem.NivelDaArma);
        MidiaDeEquipamentoDePersonagemDto? midiaArmadura = MapearMidiaDeArmadura(armadura, personagem.NivelDaArmadura);

        var (espaco1, espaco2, acessorios) = MapearEspacos(personagem, itens);
        var acessoriosIds = new[] { espaco1.AcessorioId, espaco2.AcessorioId }
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToArray();
        var midiasAcessorios = acessorios
            .Where(a => a is not null)
            .Select(a => new MidiaDeAcessorioDePersonagemDto(
                a!.Id,
                a.Midia.Status.ToString(),
                a.Midia.ArquivoInventarioId,
                a.Midia.HashArquivo))
            .ToArray();
        var fichaBase = FichaEfetivaDePersonagem.DaBase(personagem);
        var fichaEfetiva = FichaEfetivaDePersonagem.Calcular(personagem, acessorios);

        return new(
            personagem.Id,
            personagem.Nome,
            personagem.Classe,
            fichaEfetiva.HpMaximo,
            fichaEfetiva.HpAtual,
            personagem.Stress,
            fichaEfetiva.ChanceDeVirtude,
            personagem.Aflicao,
            personagem.Virtude,
            new ResistenciasBaseDto(
                fichaEfetiva.Resistencias.Atordoamento,
                fichaEfetiva.Resistencias.Sangramento,
                fichaEfetiva.Resistencias.Envenenamento,
                fichaEfetiva.Resistencias.Debuff,
                fichaEfetiva.Resistencias.Movimento,
                fichaEfetiva.Resistencias.Doenca,
                fichaEfetiva.Resistencias.GolpeMortal,
                fichaEfetiva.Resistencias.Armadilha),
            fichaEfetiva.Resistencias.Doenca,
            fichaEfetiva.Resistencias.GolpeMortal,
            fichaEfetiva.Resistencias.Armadilha,
            MapearHabilidades(personagem, habilidades, resolvedor),
            personagem.ArmaEquipadaId,
            personagem.ArmaduraEquipadaId,
            acessoriosIds,
            midiaArma,
            midiaArmadura,
            midiasAcessorios,
            personagem.Aparencia,
            personagem.NivelDaArma,
            personagem.NivelDaArmadura,
            fichaEfetiva.Precisao,
            fichaEfetiva.Protecao,
            fichaEfetiva.Esquiva,
            fichaEfetiva.Velocidade,
            fichaEfetiva.Critico,
            fichaEfetiva.DanoBaseMinimo,
            fichaEfetiva.DanoBaseMaximo,
            personagem.PassosAFrente,
            personagem.PassosAtras,
            classe?.Religiosa ?? false,
            classe?.ProvisaoInicial,
            classe?.BonusAoCriticoDaClasse ?? string.Empty,
            MontarMidias(personagem, classe, resolvedor, itens),
            espaco1,
            espaco2,
            fichaBase,
            fichaEfetiva);
    }

    public static InimigoDetalheDto ParaDto(Inimigo inimigo) => new(
        inimigo.Id,
        inimigo.Nome,
        inimigo.TipoDeInimigo,
        inimigo.HabilidadesIds);

    private static IReadOnlyList<HabilidadeDePersonagemDto> MapearHabilidades(
        Personagem personagem,
        IReadOnlyDictionary<Guid, Habilidade>? habilidades,
        IResolvedorDeMidiasDoCard? resolvedor)
    {
        return personagem.Habilidades.Select(h =>
        {
            Habilidade? catalogo = null;
            habilidades?.TryGetValue(h.HabilidadeId, out catalogo);
            var midia = catalogo is null || resolvedor is null
                ? null
                : resolvedor.ResolverHabilidade(personagem.Classe, catalogo);
            return new HabilidadeDePersonagemDto(
                h.HabilidadeId,
                h.Habilitada,
                h.Treinada,
                h.Equipada,
                h.NumeroDoNivel,
                catalogo?.NomeExibicao,
                catalogo is HabilidadeDeCombate ? "Combate" : catalogo is HabilidadeDeAcampamento ? "Acampamento" : "Inimigo",
                midia);
        }).ToArray();
    }

    private static MidiasDoPersonagemDto? MontarMidias(
        Personagem personagem,
        Classe? classe,
        IResolvedorDeMidiasDoCard? resolvedor,
        IReadOnlyDictionary<Guid, Item>? itens)
    {
        if (resolvedor is null)
        {
            return null;
        }

        var arma = LocalizarArmaParaCard(personagem, itens);
        var armadura = LocalizarArmaduraParaCard(personagem, itens);
        return new MidiasDoPersonagemDto(
            resolvedor.ResolverRetrato(personagem.Classe, personagem.Aparencia, classe),
            resolvedor.ResolverCorpoInteiro(personagem.Classe, personagem.Aparencia),
            resolvedor.ResolverArma(personagem.Classe, personagem.NivelDaArma, arma),
            resolvedor.ResolverArmadura(personagem.Classe, personagem.NivelDaArmadura, armadura));
    }

    private static (EspacoTrinketDto Espaco1, EspacoTrinketDto Espaco2, Acessorio?[] Acessorios) MapearEspacos(
        Personagem personagem,
        IReadOnlyDictionary<Guid, Item>? itens)
    {
        var a1 = LocalizarAcessorio(personagem.AcessorioEquipado1Id, itens);
        var a2 = LocalizarAcessorio(personagem.AcessorioEquipado2Id, itens);
        return (ParaEspaco(a1), ParaEspaco(a2), [a1, a2]);
    }

    private static Acessorio? LocalizarAcessorio(Guid? id, IReadOnlyDictionary<Guid, Item>? itens)
    {
        if (id is not { } acessorioId || itens is null || !itens.TryGetValue(acessorioId, out var item))
        {
            return null;
        }

        return item as Acessorio;
    }

    private static EspacoTrinketDto ParaEspaco(Acessorio? acessorio) =>
        acessorio is null
            ? new EspacoTrinketDto(null, null, null)
            : new EspacoTrinketDto(acessorio.Id, acessorio.NomeExibicao, acessorio.Raridade);

    private static MidiaDeEquipamentoDePersonagemDto? MapearMidiaDeArma(Arma? arma, int? nivelDaArma)
    {
        if (arma is null)
        {
            return null;
        }

        var nivel = EscolherNivel(arma.Niveis.Select(n => n.Nivel), nivelDaArma);
        var tabela = arma.Niveis.FirstOrDefault(n => n.Nivel == nivel);
        return tabela is null
            ? null
            : new MidiaDeEquipamentoDePersonagemDto(
                tabela.Nivel,
                tabela.Midia.Status.ToString(),
                tabela.Midia.ArquivoInventarioId,
                tabela.Midia.ConjuntoSpineId,
                tabela.Midia.HashArquivo);
    }

    private static MidiaDeEquipamentoDePersonagemDto? MapearMidiaDeArmadura(Armadura? armadura, int? nivelDaArmadura)
    {
        if (armadura is null)
        {
            return null;
        }

        var nivel = EscolherNivel(armadura.Niveis.Select(n => n.Nivel), nivelDaArmadura);
        var tabela = armadura.Niveis.FirstOrDefault(n => n.Nivel == nivel);
        return tabela is null
            ? null
            : new MidiaDeEquipamentoDePersonagemDto(
                tabela.Nivel,
                tabela.Midia.Status.ToString(),
                tabela.Midia.ArquivoInventarioId,
                tabela.Midia.ConjuntoSpineId,
                tabela.Midia.HashArquivo);
    }

    private static int EscolherNivel(IEnumerable<int> niveis, int? preferido)
    {
        var catalogados = niveis.OrderBy(n => n).ToArray();
        if (preferido is { } nivel && catalogados.Contains(nivel))
        {
            return nivel;
        }

        return catalogados.Contains(1) ? 1 : catalogados.FirstOrDefault();
    }

    private static Arma? LocalizarArmaEquipada(Personagem personagem, IReadOnlyDictionary<Guid, Item>? itens)
    {
        if (personagem.ArmaEquipadaId is not { } armaId || itens is null || !itens.TryGetValue(armaId, out var item))
        {
            return null;
        }

        return item as Arma;
    }

    private static Armadura? LocalizarArmaduraEquipada(Personagem personagem, IReadOnlyDictionary<Guid, Item>? itens)
    {
        if (personagem.ArmaduraEquipadaId is not { } armaduraId || itens is null || !itens.TryGetValue(armaduraId, out var item))
        {
            return null;
        }

        return item as Armadura;
    }

    private static Arma? LocalizarArmaParaCard(Personagem personagem, IReadOnlyDictionary<Guid, Item>? itens) =>
        LocalizarArmaEquipada(personagem, itens)
        ?? itens?.Values.OfType<Arma>().FirstOrDefault(a => a.ClasseElegivel == personagem.Classe);

    private static Armadura? LocalizarArmaduraParaCard(Personagem personagem, IReadOnlyDictionary<Guid, Item>? itens) =>
        LocalizarArmaduraEquipada(personagem, itens)
        ?? itens?.Values.OfType<Armadura>().FirstOrDefault(a => a.ClasseElegivel == personagem.Classe);
}
