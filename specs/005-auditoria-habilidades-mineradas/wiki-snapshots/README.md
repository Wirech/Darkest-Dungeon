# Wiki Snapshots — Feature 005

Diretório dos JSONs de mineração da wiki oficial `darkestdungeon.wiki.gg`.

Cada arquivo `{classe-slug}.json` representa a fonte-de-verdade wiki para as habilidades de uma classe, com dados de Level 1 a Level 5 conforme decisão da Q1 e Q3 da 3ª rodada de clarify.

## Schema

```json
{
  "classe": "Cruzado",
  "classeSlug": "cruzado",
  "classeDeHeroiEnum": "Cruzado",
  "fonteUrl": "https://darkestdungeon.wiki.gg/wiki/Crusader",
  "mineradoEm": "2026-09-08",
  "mineradoPor": "curador-humano-ou-agente",
  "habilidades": [
    {
      "nomeExibicao": "Golpe Sagrado",
      "nomeOriginal": "Smite",
      "tipo": "Combate",
      "descricao": "Ataque abençoado que causa dano bônus contra Impuros.",
      "posicoesValidas": [1, 2],
      "posicoesQueAtinge": [1, 2, 3, 4],
      "alvoEmArea": false,
      "campos": {
        "modificadorDano": [0, 0, 0, 0, 0],
        "modificadorAcerto": [0, 5, 10, 15, 20],
        "modificadorCritico": [0, 0, 5, 5, 10]
      },
      "efeitos": [
        {
          "tipoDoEfeito": "BuffDano",
          "valorPorNivel": [10, 15, 20, 25, 30],
          "chancePorNivel": [100, 100, 100, 100, 100],
          "aplicavelSomenteEmImpuros": true
        }
      ],
      "notas": [
        "Level 5 iguala versão pré-nerf 2016 conforme changelog oficial"
      ]
    }
  ]
}
```

## Regras de preenchimento

- **`campos.*` sempre tem 5 valores** (Level 1..5). Se a wiki não publica, usar `null` — a auditoria marca esse campo como `Pendente` na cobertura.
- **`efeitos[].valorPorNivel` sempre tem 5 valores**. Idem para `chancePorNivel`.
- **`nomeOriginal`** deve corresponder exatamente ao nome inglês da wiki (usado como chave de correspondência com o seed).
- Se a habilidade não existe no seed 003, aparecerá como divergência `Faltando`.
- Se o seed 003 tem habilidade que não está no snapshot, aparecerá como divergência `Faltando (no snapshot)`.

## Slugs por classe (arquivos esperados)

Cada classe do enum `ClasseDeHeroi` deve ter um arquivo correspondente:

| Enum | Slug (nome do arquivo) |
|---|---|
| Abominacao | `abominacao.json` |
| Antiquario | `antiquario.json` |
| Besteiro | `besteiro.json` |
| CacadorDeRecompensas | `cacador-de-recompensas.json` |
| Cruzado | `cruzado.json` |
| LadraoDeCova | `ladrao-de-cova.json` |
| BoboDaCorte | `bobo-da-corte.json` |
| MestreDeCaca | `mestre-de-caca.json` |
| Leproso | `leproso.json` |
| Infernal | `infernal.json` |
| Bandido | `bandido.json` |
| Musqueteiro | `musqueteiro.json` |
| Veterano | `veterano.json` |
| Ocultista | `ocultista.json` |
| MedicoDaPeste | `medico-da-peste.json` |
| Vestal | `vestal.json` |
| Flagelante | `flagelante.json` |
| Rompedor | `rompedor.json` |
| Duelista | `duelista.json` |
| Fugitivo | `fugitivo.json` |

## Como o serviço de auditoria consome estes arquivos

`AuditoriaWikiService.GerarRelatorio(classe?)`:

1. Carrega os arquivos JSON deste diretório.
2. Para cada habilidade seedada da classe, procura correspondência por `nomeOriginal`.
3. Compara os 15 campos exigidos.
4. Marca cada habilidade como `OK` (todos os campos batem), `Parcial` (alguns divergem) ou `Faltando` (não existe no snapshot).
5. Registra diffs textuais em `RelatorioDeAuditoria.Linhas[].Diffs`.
6. Também audita as 80 combinações Classe × Aparência (categoria `AssetsVisuais`) e os 1.095 níveis de habilidade esperados (categoria `NivelDeHabilidade` do Mapa de Cobertura).
