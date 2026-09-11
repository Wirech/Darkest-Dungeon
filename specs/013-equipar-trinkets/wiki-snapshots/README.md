# Snapshots de trinkets (curadoria)

Pasta versionada dos JSON coletados da wiki oficial (`darkestdungeon.wiki.gg`).  
A API **não** lê a wiki; o seed de `Acessorio` usa **somente** estes arquivos.

## Schema (`{slug}.json`)

| Campo | Tipo | Regra |
|---|---|---|
| `nomeOriginal` | string | Chave de upsert (EN da wiki). |
| `nomeExibicao` | string | PT-BR se existir; senão o original. |
| `descricao` | string | Vazio permitido se a wiki não publicar. |
| `raridade` | string | Enum: `Comum`, `Incomum`, `Rara`, `MuitoRara`, `CrimsonCourt`, `Crystalline`, `Set`. Ausente/ilegível → lacuna, **não** chute. |
| `classeExclusiva` | string ou `null` | Enum de herói só se a wiki restringir. |
| `conjuntoIdWiki` | string opcional | Só rastreio; **não** vira bônus de conjunto. |
| `fonteUrl` | URL | Página de origem. |
| `efeitos[]` | lista | `nome`, `valor` (decimal), `unidade` (`Percentual` \| `Pontos`), `sinal` (`Positivo` \| `Negativo`). |
| `lacunas[]` | lista opcional | Campo, motivo observável, momento. |

## DLC no escopo

Incluir trinkets listados na wiki do jogo base **e** DLC ligados na mesma fonte, por exemplo:

- Crimson Court (`CrimsonCourt`)
- Color of Madness / Crystalline (`Crystalline`)
- Conjuntos listados (`Set`)
- Classes DLC (Flagelante, Rompedor, Duelista, Fugitivo) quando a wiki publicar trinkets exclusivos

Não inventar página que a wiki não listar.

## Política de lacuna

- Campo ausente, ambíguo ou unidade incompatível com o mapa canônico → registrar lacuna e **não** preencher valor.
- A coleta **continua** nos demais itens (melhor esforço).
- Reexecução sobrescreve o JSON do mesmo slug; upsert SQL casa `NomeOriginal` sem duplicar `Id`.
- Snapshot sem efeitos e com lacuna **não** zera efeitos oficiais já curados.

## Coleta

```powershell
dotnet run --project tools/DarkestDungeon.WikiCatalogCollector -- --trinkets --saida specs/013-equipar-trinkets/wiki-snapshots
```
