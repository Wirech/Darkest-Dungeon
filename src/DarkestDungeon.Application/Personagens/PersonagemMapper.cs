using DarkestDungeon.Application.Midias;
using DarkestDungeon.Domain.Itens;
using DarkestDungeon.Domain.Personagens;
using DarkestDungeon.Domain.Seres;

namespace DarkestDungeon.Application.Personagens;

internal static class PersonagemMapper
{
    public static PersonagemDetalheDto ParaDto(Personagem personagem, IReadOnlyDictionary<Guid, Item>? itens = null)
    {
        itens ??= new Dictionary<Guid, Item>();
        var nivelMidia = MidiaDeItemDtoMapper.NivelDeMidia(personagem.NivelDeResolucao.Valor);
        MidiaDeEquipamentoDePersonagemDto? midiaArma = null;
        if (personagem.ArmaEquipadaId is { } armaId && itens.TryGetValue(armaId, out var armaItem) && armaItem is Arma arma)
        {
            var nivel = arma.Niveis.FirstOrDefault(n => n.Nivel == nivelMidia) ?? arma.Niveis.First();
            midiaArma = new MidiaDeEquipamentoDePersonagemDto(
                nivel.Nivel,
                nivel.Midia.Status.ToString(),
                nivel.Midia.ArquivoInventarioId,
                nivel.Midia.ConjuntoSpineId,
                nivel.Midia.HashArquivo);
        }

        MidiaDeEquipamentoDePersonagemDto? midiaArmadura = null;
        if (personagem.ArmaduraEquipadaId is { } armaduraId && itens.TryGetValue(armaduraId, out var armaduraItem) && armaduraItem is Armadura armadura)
        {
            var nivel = armadura.Niveis.FirstOrDefault(n => n.Nivel == nivelMidia) ?? armadura.Niveis.First();
            midiaArmadura = new MidiaDeEquipamentoDePersonagemDto(
                nivel.Nivel,
                nivel.Midia.Status.ToString(),
                nivel.Midia.ArquivoInventarioId,
                nivel.Midia.ConjuntoSpineId,
                nivel.Midia.HashArquivo);
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
                personagem.Resistencias.Movimento),
            personagem.ResistenciasExtras.Doenca,
            personagem.ResistenciasExtras.GolpeMortal,
            personagem.ResistenciasExtras.Armadilha,
            personagem.Habilidades.Select(h => new HabilidadeDePersonagemDto(h.HabilidadeId, h.Habilitada, h.Treinada, h.Equipada)).ToArray(),
            personagem.ArmaEquipadaId,
            personagem.ArmaduraEquipadaId,
            acessoriosIds,
            midiaArma,
            midiaArmadura,
            midiasAcessorios);
    }

    public static InimigoDetalheDto ParaDto(Inimigo inimigo) => new(
        inimigo.Id,
        inimigo.Nome,
        inimigo.TipoDeInimigo,
        inimigo.HabilidadesIds);
}
