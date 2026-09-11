using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Habilidades;
using DarkestDungeon.Domain.Itens;
using DarkestDungeon.Domain.Personagens;

namespace DarkestDungeon.Application.Midias;

/// Porta de leitura das mídias do card. Implementação na Infrastructure (sem System.IO nesta camada).
public interface IResolvedorDeMidiasDoCard
{
    SlotDeMidiaDoCardDto ResolverRetrato(ClasseDeHeroi classe, AparenciaDePersonagem aparencia, Classe? catalogo);

    SlotDeMidiaDoCardDto ResolverCorpoInteiro(ClasseDeHeroi classe, AparenciaDePersonagem aparencia);

    SlotDeMidiaDoCardDto ResolverArma(ClasseDeHeroi classe, int? nivelDaArma, Arma? arma);

    SlotDeMidiaDoCardDto ResolverArmadura(ClasseDeHeroi classe, int? nivelDaArmadura, Armadura? armadura);

    SlotDeMidiaDoCardDto ResolverHabilidade(ClasseDeHeroi classe, Habilidade habilidade);
}
