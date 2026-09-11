# Contrato: Coleta de trinkets (curadoria)

Ferramenta: `tools/DarkestDungeon.WikiCatalogCollector` em modo trinkets. **Não** é endpoint da API. **Não** usa `IPublicadorAtomicoService`.

## Invocação

```powershell
dotnet run --project tools/DarkestDungeon.WikiCatalogCollector -- --trinkets --saida specs/013-equipar-trinkets/wiki-snapshots
```

Rede só nesta invocação. Timeout/429: abortar com relatório PT-BR (mesmo espírito do `ClienteWiki` atual).

## Entrada

- Wiki oficial `darkestdungeon.wiki.gg`, wikitext `?action=raw`.
- Índice de trinkets do jogo base **e** DLC listados (Crimson Court, Color of Madness / Crystalline e similares ligados na mesma fonte).
- Sem inventar página que a wiki não listar.

## Saída

- Um JSON por trinket em `--saida` (ver [data-model.md](../data-model.md)).
- Relatório de lacunas em PT-BR (stdout e/ou arquivo ao lado dos snapshots): trinket/página, campo, motivo, momento.
- Código de saída `0` se a execução terminou (mesmo com lacunas parciais). Código ≠ 0 só para falha de infraestrutura (rede, 429, disco), não para item incompleto isolado.

## Upsert no catálogo SQL (passo posterior à coleta)

Operação de aplicação/infra sobre os snapshots **já gravados** (seed ou comando de curadoria local). Match `NomeOriginal`:

| Caso | Resultado |
|---|---|
| Existe acessório | Preserva `Id`; substitui efeitos; atualiza raridade/classe/textos se a wiki informou |
| Não existe | Cria acessório novo |
| Snapshot com lacuna de efeitos | Não inventa efeito; não apaga efeitos oficiais já curados salvo se a wiki publicou lista vazia explícita — na dúvida, registrar lacuna e **não** zerar |

Idempotência: segunda execução, mesmos snapshots → 0 duplicatas de `NomeOriginal`.

## Fora deste contrato

- Ícones / Spine / `icons_equip`.
- Runtime da API.
- Publicação 005.
---
