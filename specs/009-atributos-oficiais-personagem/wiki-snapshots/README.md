# Snapshots oficiais de atributos (Feature 009)

Pasta de curadoria. Os JSON aqui são a **única** fonte numérica do seed de arma/armadura e do perfil extra de `Classe`. A API **não** consulta a wiki em tempo de uso.

Geração:

```powershell
dotnet run --project tools/DarkestDungeon.WikiCatalogCollector -- --saida specs/009-atributos-oficiais-personagem/wiki-snapshots
```

## Schema por classe (`{slug}.json`)

```json
{
  "classeDeHeroiEnum": "Cruzado",
  "fonteUrl": "https://darkestdungeon.wiki.gg/wiki/Crusader?action=raw",
  "forma": "humana",
  "passosAFrente": 1,
  "passosAtras": 1,
  "religiosa": true,
  "provisaoInicial": "8 Food, 2 Holy Water",
  "bonusAoCritico": "+15% PROT",
  "arma": {
    "niveis": [
      { "nivel": 1, "danoMinimo": 6, "danoMaximo": 12, "critico": 3.0, "velocidade": 1 },
      { "nivel": 2, "danoMinimo": 0, "danoMaximo": 0, "critico": 0.0, "velocidade": 0 },
      { "nivel": 3, "danoMinimo": 0, "danoMaximo": 0, "critico": 0.0, "velocidade": 0 },
      { "nivel": 4, "danoMinimo": 0, "danoMaximo": 0, "critico": 0.0, "velocidade": 0 },
      { "nivel": 5, "danoMinimo": 0, "danoMaximo": 0, "critico": 0.0, "velocidade": 0 }
    ]
  },
  "armadura": {
    "niveis": [
      { "nivel": 1, "hpMaximo": 33, "esquiva": 5.0 },
      { "nivel": 2, "hpMaximo": 0, "esquiva": 0.0 },
      { "nivel": 3, "hpMaximo": 0, "esquiva": 0.0 },
      { "nivel": 4, "hpMaximo": 0, "esquiva": 0.0 },
      { "nivel": 5, "hpMaximo": 0, "esquiva": 0.0 }
    ]
  }
}
```

Os números do exemplo são **formato**. Valores reais vêm do coletor. Nenhum campo oficial pode ser nulo. `forma` é sempre `humana` (Abominação besta não entra). `armadura.niveis[].hpMaximo` vira `NivelDeArmadura.HpAdicional` no seed.

## Tabela de slugs

Arquivo local = slug PT-BR. Página wiki = `Description` de `ClasseDeHeroi` (inglês).

| Enum | Slug do arquivo | Página wiki (`Description`) |
|---|---|---|
| Abominacao | abominacao | Abomination |
| Antiquario | antiquario | Antiquarian |
| Besteiro | besteiro | Arbalest |
| CacadorDeRecompensas | cacador-de-recompensas | Bounty Hunter |
| Cruzado | cruzado | Crusader |
| LadraoDeCova | ladrao-de-cova | Grave Robber |
| BoboDaCorte | bobo-da-corte | Jester |
| MestreDeCaca | mestre-de-caca | Houndmaster |
| Leproso | leproso | Leper |
| Infernal | infernal | Hellion |
| Bandido | bandido | Highwayman |
| Musqueteiro | musqueteiro | Musketeer |
| Veterano | veterano | Man-at-Arms |
| Ocultista | ocultista | Occultist |
| MedicoDaPeste | medico-da-peste | Plague Doctor |
| Vestal | vestal | Vestal |
| Flagelante | flagelante | Flagellant |
| Rompedor | rompedor | Shieldbreaker |
| Duelista | duelista | Duelist |
| Fugitivo | fugitivo | Runaway |

URL de coleta: `https://darkestdungeon.wiki.gg/wiki/{Page}?action=raw` (espaço → `_`).
