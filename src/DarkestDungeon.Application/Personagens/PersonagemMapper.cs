using DarkestDungeon.Domain.Personagens;
using DarkestDungeon.Domain.Seres;

namespace DarkestDungeon.Application.Personagens;

internal static class PersonagemMapper
{
    public static PersonagemDetalheDto ParaDto(Personagem personagem) => new(
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
        new[] { personagem.AcessorioEquipado1Id, personagem.AcessorioEquipado2Id }.Where(id => id.HasValue).Select(id => id!.Value).ToArray());

    public static InimigoDetalheDto ParaDto(Inimigo inimigo) => new(
        inimigo.Id,
        inimigo.Nome,
        inimigo.TipoDeInimigo,
        inimigo.HabilidadesIds);
}
