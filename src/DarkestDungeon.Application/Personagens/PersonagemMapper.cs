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
        return new PersonagemResumoDto(
            personagem.Id,
            personagem.Nome,
            personagem.Classe,
            classe?.NomeExibicao ?? personagem.Classe.ToString(),
            personagem.HpAtual,
            personagem.HpMaximo,
            personagem.Stress,
            personagem.Nivel,
            MapearHabilidades(personagem, habilidades, resolvedor),
            personagem.Precisao,
            personagem.Protecao,
            personagem.Esquiva,
            personagem.Velocidade,
            personagem.Critico,
            personagem.DanoBaseMinimo,
            personagem.DanoBaseMaximo,
            personagem.PassosAFrente,
            personagem.PassosAtras,
            new ResistenciasDePersonagemDto(
                personagem.Resistencias.Atordoamento,
                personagem.Resistencias.Sangramento,
                personagem.Resistencias.Envenenamento,
                personagem.Resistencias.Debuff,
                personagem.Resistencias.Movimento,
                personagem.ResistenciasExtras.Doenca,
                personagem.ResistenciasExtras.GolpeMortal,
                personagem.ResistenciasExtras.Armadilha),
            personagem.NivelDaArma,
            personagem.NivelDaArmadura,
            personagem.Aparencia,
            classe?.Religiosa ?? false,
            classe?.ProvisaoInicial,
            classe?.BonusAoCriticoDaClasse ?? string.Empty,
            midias);
    }

    public static PersonagemDetalheDto ParaDto(
        Personagem personagem,
        IReadOnlyDictionary<Guid, Item>? itens = null,
        IReadOnlyDictionary<Guid, Habilidade>? habilidades = null,
        Classe? classe = null,
        IResolvedorDeMidiasDoCard? resolvedor = null)
    {
        itens ??= new Dictionary<Guid, Item>();
        var arma = LocalizarArma(personagem, itens);
        var armadura = LocalizarArmadura(personagem, itens);
        MidiaDeEquipamentoDePersonagemDto? midiaArma = null;
        if (arma is not null)
        {
            var nivel = personagem.NivelDaArma is { } nivelArma
                ? arma.Niveis.FirstOrDefault(n => n.Nivel == nivelArma)
                : null;
            if (nivel is not null)
            {
                midiaArma = new MidiaDeEquipamentoDePersonagemDto(
                    nivel.Nivel,
                    nivel.Midia.Status.ToString(),
                    nivel.Midia.ArquivoInventarioId,
                    nivel.Midia.ConjuntoSpineId,
                    nivel.Midia.HashArquivo);
            }
        }

        MidiaDeEquipamentoDePersonagemDto? midiaArmadura = null;
        if (armadura is not null)
        {
            var nivel = personagem.NivelDaArmadura is { } nivelArmadura
                ? armadura.Niveis.FirstOrDefault(n => n.Nivel == nivelArmadura)
                : null;
            if (nivel is not null)
            {
                midiaArmadura = new MidiaDeEquipamentoDePersonagemDto(
                    nivel.Nivel,
                    nivel.Midia.Status.ToString(),
                    nivel.Midia.ArquivoInventarioId,
                    nivel.Midia.ConjuntoSpineId,
                    nivel.Midia.HashArquivo);
            }
        }

        var acessoriosIds = new[] { personagem.AcessorioEquipado1Id, personagem.AcessorioEquipado2Id }
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToArray();
        var midiasAcessorios = acessoriosIds
            .Where(itens.ContainsKey)
            .Select(id => itens[id])
            .OfType<Acessorio>()
            .Select(a => new MidiaDeAcessorioDePersonagemDto(
                a.Id,
                a.Midia.Status.ToString(),
                a.Midia.ArquivoInventarioId,
                a.Midia.HashArquivo))
            .ToArray();

        return new(
            personagem.Id,
            personagem.Nome,
            personagem.Classe,
            personagem.HpMaximo,
            personagem.HpAtual,
            personagem.Stress,
            personagem.ChanceDeVirtude,
            personagem.Aflicao,
            personagem.Virtude,
            new ResistenciasBaseDto(
                personagem.Resistencias.Atordoamento,
                personagem.Resistencias.Sangramento,
                personagem.Resistencias.Envenenamento,
                personagem.Resistencias.Debuff,
                personagem.Resistencias.Movimento,
                personagem.ResistenciasExtras.Doenca,
                personagem.ResistenciasExtras.GolpeMortal,
                personagem.ResistenciasExtras.Armadilha),
            personagem.ResistenciasExtras.Doenca,
            personagem.ResistenciasExtras.GolpeMortal,
            personagem.ResistenciasExtras.Armadilha,
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
            personagem.Precisao,
            personagem.Protecao,
            personagem.Esquiva,
            personagem.Velocidade,
            personagem.Critico,
            personagem.DanoBaseMinimo,
            personagem.DanoBaseMaximo,
            personagem.PassosAFrente,
            personagem.PassosAtras,
            classe?.Religiosa ?? false,
            classe?.ProvisaoInicial,
            classe?.BonusAoCriticoDaClasse ?? string.Empty,
            MontarMidias(personagem, classe, resolvedor, itens));
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

        var arma = LocalizarArma(personagem, itens);
        var armadura = LocalizarArmadura(personagem, itens);
        return new MidiasDoPersonagemDto(
            resolvedor.ResolverRetrato(personagem.Classe, personagem.Aparencia, classe),
            resolvedor.ResolverCorpoInteiro(personagem.Classe, personagem.Aparencia),
            resolvedor.ResolverArma(personagem.Classe, personagem.NivelDaArma, arma),
            resolvedor.ResolverArmadura(personagem.Classe, personagem.NivelDaArmadura, armadura));
    }

    private static Arma? LocalizarArma(Personagem personagem, IReadOnlyDictionary<Guid, Item>? itens)
    {
        if (itens is null)
        {
            return null;
        }

        if (personagem.ArmaEquipadaId is { } armaId && itens.TryGetValue(armaId, out var item) && item is Arma arma)
        {
            return arma;
        }

        return itens.Values.OfType<Arma>().FirstOrDefault(a => a.ClasseElegivel == personagem.Classe);
    }

    private static Armadura? LocalizarArmadura(Personagem personagem, IReadOnlyDictionary<Guid, Item>? itens)
    {
        if (itens is null)
        {
            return null;
        }

        if (personagem.ArmaduraEquipadaId is { } armaduraId && itens.TryGetValue(armaduraId, out var item) && item is Armadura armadura)
        {
            return armadura;
        }

        return itens.Values.OfType<Armadura>().FirstOrDefault(a => a.ClasseElegivel == personagem.Classe);
    }
}
