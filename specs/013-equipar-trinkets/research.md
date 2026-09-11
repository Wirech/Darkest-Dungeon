# Research: Equipar Trinkets no Personagem

**Feature**: `013-equipar-trinkets`  
**Data**: 2026-09-11

Não restam itens `NEEDS CLARIFICATION`. Decisões abaixo fecham o contexto técnico a partir da spec, da constituição 1.0.0 e do código atual (003/006/008/009).

---

## Decisão 1: Coletor de curadoria no WikiCatalogCollector, modo best-effort

**Decision**: Estender `tools/DarkestDungeon.WikiCatalogCollector` com um modo de trinkets (flag explícita, ex. `--trinkets`). Reusar `ClienteWiki` (`darkestdungeon.wiki.gg`, `?action=raw`, UA próprio, abortar em 429). **Não** reusar `CatalogoDePaginasWiki`, `ParserDeWikitext` de classe, nem o executor fail-closed da 009. Snapshots versionados em `specs/013-equipar-trinkets/wiki-snapshots/`. A API e o card **MUST NOT** chamar a wiki.

**Rationale**: FR-001/FR-004 pedem melhor esforço **incluindo DLC**, com lacuna registrada e coleta **continua**. A 009 para no primeiro campo ausente — incompatível. Constituição V: um CLI de curadoria, sem quinto projeto de runtime.

**Alternatives considered**:

- Novo `tools/DarkestDungeon.TrinketCollector`: rejeitado (duplica HTTP/429).
- Job HTTP na API: rejeitado (runtime stateless; padrão 005/009).
- Reusar o executor 009 fail-closed: rejeitado (FR-004).

---

## Decisão 2: Snapshot por trinket; lacuna não inventa efeito

**Decision**: Um JSON por trinket (`{slug}.json`) com nome de exibição, nome original, descrição, raridade mapeada ao enum fechado, classe exclusiva opcional, lista de efeitos (`nome`, `valor`, `unidade`, `sinal`) e `fonteUrl`. Página incompleta ou efeito ilegível gera **lacuna** (trinket/página, campo, motivo, momento) e **não** preenche valor. DLC (Crimson Court, Color of Madness / Crystalline, conjuntos listados) entra na mesma varredura. Índice/sumário da wiki (`Trinkets` e páginas DLC ligadas) é a porta de entrada.

**Rationale**: FR-001, FR-002, FR-004, clarificação Q3. Raridades `CrimsonCourt` / `Crystalline` / `Set` já existem em `RaridadeDeAcessorio`.

**Alternatives considered**:

- Um único JSON gigante: rejeitado (diff e reexecução piores).
- Excluir DLC: rejeitado na clarificação.
- Inventar raridade `Comum` quando a wiki omite: rejeitado (FR-002); vira lacuna. Acessório 006 já criado como `Comum` **só é atualizado** quando a wiki informar raridade válida.

---

## Decisão 3: Upsert por NomeOriginal; preservar Id

**Decision**: Match case-insensitive de `Acessorio.NomeOriginal` com o nome original do snapshot. Hit: atualizar raridade (se a wiki informar), classe exclusiva, descrição/exibição e **substituir a lista de efeitos**; **não** trocar `Id`, `ConjuntoId` nem mídia 006. Miss: criar `Acessorio` com Guid novo. Reexecução não duplica. Índice único filtrado em `NomeOriginal` para discriminador `Acessorio` (quando o provedor permitir) reforça FR-003.

**Rationale**: US1 cenários 2–5; 006 já cria acessórios vazios pelo stem do PNG. Sem mutator hoje (`Acessorio` só tem `DefinirMidia`).

**Alternatives considered**:

- Recriar seed 003: proibido.
- Match por Id de snapshot: rejeitado (006/003 não compartilham esse Id).
- Append de efeitos: rejeitado (duplicaria na reexecução).

---

## Decisão 4: Ficha efetiva só na consulta; base persistida intacta

**Decision**: Equipar/remover **não** regrava HP, precisão, proteção, esquiva, velocidade, crítico, dano, virtude nem resistências em `Ser`/`Personagem`. Um calculador de aplicação (`FichaEfetivaDePersonagem` ou equivalente) lê a ficha persistida como **base**, soma efeitos mapeados dos dois espaços e devolve **efetiva**. `DerivacaoDeAtributosOficiais` permanece sem acessórios. `POST /personagens` continua sem trinket.

**Rationale**: FR-010, FR-017, US3. Somar na persistência tornaria o desequipar irreversível.

**Alternatives considered**:

- Reescrever colunas e “desfazer” no remove: rejeitado (corrida e meia-operação).
- Tabela `FichaEfetiva` persistida: rejeitado (duplica estado; FR-011 pede consistência pela consulta).

---

## Decisão 5: Unidades por atributo, soma na base, teto depois da soma

**Decision** (clarificações Q2 e Q5):

| Atributo | Unidade |
|---|---|
| HP máximo, dano mín, dano máx | % da **ficha base** |
| Proteção, crítico, 8 resistências, chance de virtude | pontos percentuais somados |
| Precisão, esquiva, velocidade | pontos da wiki |

Vários efeitos no mesmo atributo: cada um contra a base, depois soma (sem juros). Atributos **inteiros** da ficha (ao menos HP máximo, dano mín/máx, velocidade): `Ceiling` **depois** da soma exata. Inteiro permanece inteiro. Depois do teto: HP máximo efetivo ≥ 1; resistências 0–100. HP atual persistido não é curado; na exibição fica limitado ao HP máximo **efetivo**, sem cair abaixo de 1.

**Rationale**: FR-013, FR-013a, FR-013b, SC-005.

**Alternatives considered**:

- Um único significado de “%”: rejeitado (0 PROT × 10% = 0).
- Teto por trinket: rejeitado ( inflaria 23+10%+10%).
- Manter fração no card: rejeitado na clarificação.

---

## Decisão 6: Mapa canônico de efeito → campo; resto é lacuna visível no item

**Decision**: Tabela versionada no domínio/aplicação de nomes de efeito da wiki (EN e equivalentes já usados no catálogo) para campos da ficha cobertos por FR-013. Nome não mapeado: permanece em `EfeitoDeAcessorio`, **não** altera campo, **não** inventa atributo (FR-014). Unidade discordante do atributo (ex. HP em pontos quando a wiki quis %) → lacuna na coleta, não chute no runtime.

**Rationale**: `EfeitoDeAcessorio.Nome` é string livre ≤ 80; sem enum hoje.

**Alternatives considered**:

- Enum rígido na coleta: rejeitado (wiki varia o rótulo; lacuna é o escape).
- Heurística solta (“qualquer % vira HP”): rejeitado (FR-014).

---

## Decisão 7: Contrato por espaço, sem zerar o outro slot

**Decision**: Novo comando/endpoint de **um espaço** (`espaco` 1 ou 2, `acessorioId` opcional = vazio). Transação por personagem: grava só aquele `Guid?` e devolve detalhe com ficha base + efetiva. Recusas PT-BR: personagem inexistente, trinket inexistente, classe exclusiva, mesmo Id nos dois espaços, espaço inválido.

`POST /personagens/{id}/equipar` **não** passa a ser o fluxo do card. `AcessoriosIds` omitido **não deve mais zerar** os dois slots (hoje `?? []` apaga tudo — incompatível com FR-011). Arma/armadura nesse POST permanecem como estão.

**Rationale**: US2 espaços independentes; bug atual de wipe.

**Alternatives considered**:

- Reusar `EquiparPersonagemCommand` com os dois IDs: rejeitado (o card troca um espaço; omitir o outro apagaria).
- Dois endpoints irmãos sem parâmetro de espaço: rejeitado (duplica).

---

## Decisão 8: Lista filtrada + card com base e efetiva

**Decision**: `GET` de acessórios utilizáveis pela classe do herói (sem restrição ou exclusivos dela), excluindo o Id já no outro espaço. Card da lista (`GET /personagens`) ganha dois slots posicionais, `fichaBase` e atributos de topo = **ficha efetiva**. Mesmo card mostra as duas fichas (clarificação Q4). Sem ícone 006 a seleção usa nome/raridade. Sem `IPublicadorAtomicoService`. UI impede clique duplicado; o estado é o da última gravação concluída.

**Rationale**: FR-007a, FR-015, FR-016, FR-017, SC-003/004/009/010.

**Alternatives considered**:

- Listar o catálogo inteiro e recusar só no POST: rejeitado na clarificação Q1.
- Só efetiva no JSON: rejeitado na Q4.
- Publicar via 005: rejeitado (janela de manutenção).
---
