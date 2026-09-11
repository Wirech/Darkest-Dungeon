---

description: "Task list for Equipar Trinkets no Personagem"
---

# Tasks: Equipar Trinkets no Personagem

**Input**: Design documents from `specs/013-equipar-trinkets/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/coleta-trinkets.md`, `contracts/personagens-trinkets.md`, `quickstart.md`

**Tests**: Incluídos — a constituição exige contratos de backend verificáveis; a spec define Independent Test e SC-001..SC-010.

**Organization**: Tasks agrupadas por user story (US1 catálogo wiki, US2 dois espaços no card, US3 ficha efetiva segura).

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Pode rodar em paralelo (arquivos diferentes, sem depender de fatia incompleta).
- **[Story]**: `US1`, `US2` ou `US3` conforme `spec.md`.
- Toda task inclui caminho de arquivo.

## Path Conventions

- Solução em camadas: `src/DarkestDungeon.*`, `tests/`, `tools/`
- Snapshots: `specs/013-equipar-trinkets/wiki-snapshots/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Pasta de snapshots e flag do coletor, sem minerar trinkets ainda.

- [X] T001 Create snapshot folder docs (schema, DLC in scope, lacuna policy) in `specs/013-equipar-trinkets/wiki-snapshots/README.md`.
- [X] T002 [P] Add `--trinkets` / `--saida` routing and PT-BR usage text without breaking class-catalog mode in `tools/DarkestDungeon.WikiCatalogCollector/Program.cs` and `tools/DarkestDungeon.WikiCatalogCollector/ColetorDeCatalogoWiki.cs`.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Mutação de Acessório, DTOs compartilhados, mapa canônico e calculador da ficha efetiva. Bloqueia upsert (US1) e card (US2/US3).

**Checkpoint**: Acessório pode substituir efeitos sem trocar Id; Personagem DTOs têm dois espaços posicionais + `fichaBase`/`fichaEfetiva`; calculador existe como tipo puro.

- [X] T003 Add official-catalog mutator on `Acessorio` (replace effects, update rarity/class/texts; never change `Id`, `ConjuntoId`, or `Midia`) in `src/DarkestDungeon.Domain/Itens/Acessorio.cs`.
- [X] T004 Add `AtualizarAsync` and `ObterAcessorioPorNomeOriginalAsync` to `src/DarkestDungeon.Application/Abstractions/IItemService.cs` (`IItemRepository`) and implement them in `src/DarkestDungeon.Infrastructure/Data/` (existing item repository).
- [X] T005 [P] Add `FichaDePersonagemDto`, `EspacoTrinketDto`, `fichaBase`, `fichaEfetiva`, `espacoTrinket1` and `espacoTrinket2` to `src/DarkestDungeon.Application/Personagens/PersonagemDtos.cs`.
- [X] T006 [P] Add canonical wiki-effect-name → sheet-field map (HP/DMG %, PROT/CRIT/resists/virtue percentage-points, ACC/DODGE/SPD points) in `src/DarkestDungeon.Application/Personagens/MapaDeEfeitoDeTrinket.cs`.
- [X] T007 Implement `FichaEfetivaDePersonagem` (each effect vs **base**, no compound %, Ceiling after sum on integer fields, resist 0–100, HP max ≥ 1, HP atual clamped for display only) in `src/DarkestDungeon.Application/Personagens/FichaEfetivaDePersonagem.cs`.
- [X] T008 [P] Add `EquiparAcessorioNoEspacoCommand` (`PersonagemId`, `Espaco` 1|2, `AcessorioId` nullable) in `src/DarkestDungeon.Application/Personagens/Commands/PersonagemCommands.cs`.
- [X] T009 Map optional SQL Server filtered unique index on `Acessorio.NomeOriginal` in `src/DarkestDungeon.Infrastructure/Data/DarkestDungeonDbContext.cs`.
- [X] T010 Create the versioned EF Core migration for the acessório name index in `src/DarkestDungeon.Infrastructure/Migrations/`.

---

## Phase 3: User Story 1 - Completar o catálogo de trinkets a partir da wiki (Priority: P1) 🎯 MVP

**Goal**: Coletar da wiki (base + DLC) com melhor esforço, gravar snapshots e fazer upsert de `Acessorio` por `NomeOriginal` sem inventar efeito e sem trocar Id.

**Independent Test**: Rodar a coleta (ou fixtures sem rede); dezenas de trinkets com nome rastreável, raridade válida e efeitos quando a wiki publica; lacunas registradas; reexecução 0 duplicatas; runtime não chama a wiki.

### Tests for User Story 1

- [X] T011 [P] [US1] Add no-network wikitext fixture tests (rarity enum, class restriction, positive/negative effects, DLC page, incomplete page → lacuna and no invented value) in `tests/DarkestDungeon.Api.Tests/Feature013/ParserTrinketsWikiTests.cs`.
- [X] T012 [P] [US1] Add domain/seed tests that upsert matches `NomeOriginal` case-insensitive, preserves `Id`/`Midia`, replaces effects, and creates a new Guid when missing in `tests/DarkestDungeon.Domain.Tests/Feature013/AcessorioUpsertOficialTests.cs`.
- [X] T013 [P] [US1] Add architecture tests forbidding wiki `HttpClient` / `wiki.gg` usage from Domain, Application and Api runtime in `tests/DarkestDungeon.Architecture.Tests/Feature013WikiIsolamentoTests.cs`.

### Implementation for User Story 1

- [X] T014 [US1] Implement trinket page discovery (base index + DLC pages listed on the same wiki) in `tools/DarkestDungeon.WikiCatalogCollector/CatalogoDePaginasTrinkets.cs`.
- [X] T015 [P] [US1] Add snapshot JSON models (`nomeOriginal`, rarity, class, effects, `fonteUrl`, lacunas) in `tools/DarkestDungeon.WikiCatalogCollector/ModelosDeSnapshotTrinket.cs`.
- [X] T016 [US1] Implement trinket wikitext parser (no invented rarity/value; unknown unit → lacuna) in `tools/DarkestDungeon.WikiCatalogCollector/ParserDeTrinkets.cs`.
- [X] T017 [US1] Implement best-effort executor (continue after lacuna, PT-BR report, exit 0 unless infra/429) writing JSON under `specs/013-equipar-trinkets/wiki-snapshots/` in `tools/DarkestDungeon.WikiCatalogCollector/ColetorDeTrinkets.cs`.
- [X] T018 [US1] Implement idempotent `AcessoriosOficiaisSeed` from committed snapshots only (upsert via T003/T004) in `src/DarkestDungeon.Infrastructure/Data/Seeds/AcessoriosOficiaisSeed.cs`.
- [X] T019 [US1] Wire snapshot upsert at startup without blocking the API when snapshots are partial in `src/DarkestDungeon.Api/Program.cs`.
- [X] T020 [US1] Commit a sample snapshot set (mix of common/rare, with and without class restriction, **at least one DLC** fixture) in `specs/013-equipar-trinkets/wiki-snapshots/`.

**Checkpoint**: Catálogo local tem trinkets oficiais upsertados; lacuna não inventa valor; card ainda pode funcionar com o subconjunto gravado.

---

## Phase 4: User Story 2 - Equipar e desequipar até dois trinkets no card (Priority: P1)

**Goal**: Dois espaços independentes no card; lista só do que a classe pode usar; PUT por espaço; o outro espaço não é zerado.

**Independent Test**: Abrir card sem trinket, escolher um em cada espaço, ver bônus somados; esvaziar um espaço e ver só aquele bônus sair; recarregar e persistir escolhas.

### Tests for User Story 2

- [X] T021 [P] [US2] Add API tests for `GET /acessorios?classe=&excluirId=` (unrestricted + matching exclusive listed; other-class exclusive omitted; other-slot id omitted; empty list `200 []`) in `tests/DarkestDungeon.Api.Tests/Feature013/AcessoriosListaFiltradaTests.cs`.
- [X] T022 [P] [US2] Add API tests for `PUT /personagens/{id}/acessorios/{espaco}` (equip, clear, independent slots, duplicate `400` PT-BR, exclusive class `400`, missing trinket `400`, missing character `404`, invalid space `400`) in `tests/DarkestDungeon.Api.Tests/Feature013/PersonagensEspacoTrinketTests.cs`.
- [X] T023 [P] [US2] Add API tests that `GET /personagens` and `GET /personagens/{id}` expose two positional slots in `tests/DarkestDungeon.Api.Tests/Feature013/PersonagensCardEspacosTests.cs`.
- [X] T024 [P] [US2] Add API test that `POST /personagens/{id}/equipar` without `acessoriosIds` does **not** clear trinket slots in `tests/DarkestDungeon.Api.Tests/Feature013/PersonagensEquiparNaoZeraTrinketsTests.cs`.

### Implementation for User Story 2

- [X] T025 [US2] Implement filtered accessory listing on `IItemService` / `ItemService` in `src/DarkestDungeon.Application/Itens/ItemService.cs`.
- [X] T026 [US2] Add `GET /acessorios` (query `classe`, optional `excluirId`) and request/response contracts in `src/DarkestDungeon.Api/Controllers/Catalogo/ItensController.cs` and `src/DarkestDungeon.Api/Contracts/Catalogo/ItemContracts.cs`.
- [X] T027 [US2] Implement `EquiparEspacoAsync` (one slot, class check, duplicate check, transaction per character) in `src/DarkestDungeon.Application/Personagens/PersonagemService.cs` and `src/DarkestDungeon.Application/Abstractions/IPersonagemService.cs`.
- [X] T028 [US2] Add `PUT /personagens/{id}/acessorios/{espaco}` in `src/DarkestDungeon.Api/Controllers/Catalogo/PersonagensController.cs` and `src/DarkestDungeon.Api/Contracts/Catalogo/PersonagemContracts.cs`.
- [X] T029 [US2] Map positional slots (orphan id → empty, no fatal) in `src/DarkestDungeon.Application/Personagens/PersonagemMapper.cs`.
- [X] T030 [US2] Stop treating omitted `AcessoriosIds` as empty wipe in `src/DarkestDungeon.Application/Personagens/PersonagemService.cs` (`EquiparAsync`).
- [X] T031 [US2] Render two PT-BR trinket selectors that load `GET /acessorios` and call `PUT` per slot in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.js` and `src/DarkestDungeon.Api/wwwroot/personagens/index.html`.
- [X] T032 [P] [US2] Style empty/occupied/unavailable slot states in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.css`.

**Checkpoint**: Card tem dois espaços filtrados e persistidos; arma/armadura `POST /equipar` não apaga trinkets.

---

## Phase 5: User Story 3 - Aplicar e reverter bônus de forma segura (Priority: P1)

**Goal**: Ficha base nunca é reescrita; card mostra efetiva em destaque e base visível; unidades FR-013a/teto FR-013b; falha não deixa estado híbrido.

**Independent Test**: Anotar atributos antes; equipar dois; desequipar dois; base idêntica. Forçar falha no meio da troca → nenhum híbrido.

### Tests for User Story 3

- [X] T033 [P] [US3] Add calculator tests (30 HP +10% +10% = 36; 23 HP +10% = 26; 0 PROT +10% = 10; unmapped effect ignored; resist clamp; HP max ≥ 1) in `tests/DarkestDungeon.Domain.Tests/Feature013/FichaEfetivaDePersonagemTests.cs`.
- [X] T034 [P] [US3] Add API tests that persisted combat columns stay equal to creation after equip/unequip and that emptying both slots restores efetiva = base in `tests/DarkestDungeon.Api.Tests/Feature013/FichaBaseImutavelTests.cs`.
- [X] T035 [P] [US3] Add API test that a simulated mid-swap failure leaves previous slot pair (0 hybrid) in `tests/DarkestDungeon.Api.Tests/Feature013/TrocaTrinketAtomicaTests.cs`.
- [X] T036 [P] [US3] Add test that `DerivacaoDeAtributosOficiais` / `POST /personagens` still ignore trinkets in `tests/DarkestDungeon.Api.Tests/Feature013/CriacaoSemTrinketTests.cs`.

### Implementation for User Story 3

- [X] T037 [US3] Wire `FichaEfetivaDePersonagem` into list/detail mapping so top-level combat fields equal **efetiva** and `fichaBase` remains persisted values in `src/DarkestDungeon.Application/Personagens/PersonagemMapper.cs`.
- [X] T038 [US3] Persist only slot `Guid?` values; never write HP/ACC/PROT/DODGE/SPD/CRIT/DMG/virtue/resists on equip/remove in `src/DarkestDungeon.Application/Personagens/PersonagemService.cs`.
- [X] T039 [US3] Show ficha efetiva highlighted and ficha base visible on the same card in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.js`, `src/DarkestDungeon.Api/wwwroot/personagens/index.html` and `src/DarkestDungeon.Api/wwwroot/personagens/personagens.css`.
- [X] T040 [US3] Disable slot controls until the PUT finishes (last successful state wins) in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.js`.
- [X] T041 [P] [US3] Add architecture test that this feature flow does not reference `IPublicadorAtomicoService` in `tests/DarkestDungeon.Architecture.Tests/Feature013PublicadorIsolamentoTests.cs`.

**Checkpoint**: Desequipar devolve a criação; card compara base vs efetiva; publicação 005 intocada.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Contratos, quickstart, suíte e isolamento.

- [X] T042 [P] Align final JSON field names across `specs/013-equipar-trinkets/contracts/personagens-trinkets.md` and `specs/013-equipar-trinkets/data-model.md`.
- [X] T043 [P] Verify collector, PUT-per-slot and base/efetiva scenarios in `specs/013-equipar-trinkets/quickstart.md`.
- [X] T044 Run Domain, Architecture and API tests plus `dotnet build DarkestDungeon.sln --no-restore --configuration Release` and fix regressions in the affected projects.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: T001–T002; T002 paralelo a T001.
- **Foundational (Phase 2)**: Depende do Setup. Bloqueia US1 (mutator/repositório) e US2/US3 (DTOs/calculador/comando).
- **US1 (Phase 3)**: Depende da Foundational. **MVP**. Card da US2 pode usar acessórios de teste se a coleta ainda for parcial.
- **US2 (Phase 4)**: Depende da Foundational (slots/DTOs). Não exige snapshots 013 completos (dois Acessórios de teste bastam).
- **US3 (Phase 5)**: Depende de US2 (PUT/GET existem) e T007 (calculador).
- **Polish (Phase 6)**: Depende das três histórias.

### User Story Dependencies

- **US1 (P1)**: Independente após o mutator; entrega o catálogo.
- **US2 (P1)**: Independente do coletor wiki se houver acessórios seed/teste; precisa de T005/T008.
- **US3 (P1)**: Precisa do PUT da US2 e do calculador T007.

### Parallel Opportunities

- T001–T002.
- T005, T006, T008 em paralelo com T003–T004 se os nomes do data-model já estiverem fechados.
- T011–T013 em paralelo; T015 em paralelo com T014.
- T021–T024 em paralelo.
- T031 UI pode seguir o contrato T023 enquanto T027–T028 estabilizam o HTTP.
- T033–T036 em paralelo; T041 em paralelo com T037–T040.
- T042–T043 em paralelo.

## Parallel Execution Examples

### User Story 1

```text
Task: T011 - Parser fixtures in tests/DarkestDungeon.Api.Tests/Feature013/ParserTrinketsWikiTests.cs
Task: T012 - Upsert Id preservation in tests/DarkestDungeon.Domain.Tests/Feature013/AcessorioUpsertOficialTests.cs
Task: T013 - Wiki isolation in tests/DarkestDungeon.Architecture.Tests/Feature013WikiIsolamentoTests.cs
```

### User Story 2

```text
Task: T021 - GET /acessorios in tests/DarkestDungeon.Api.Tests/Feature013/AcessoriosListaFiltradaTests.cs
Task: T022 - PUT por espaço in tests/DarkestDungeon.Api.Tests/Feature013/PersonagensEspacoTrinketTests.cs
Task: T023 - Slots no card in tests/DarkestDungeon.Api.Tests/Feature013/PersonagensCardEspacosTests.cs
Task: T024 - POST /equipar não zera in tests/DarkestDungeon.Api.Tests/Feature013/PersonagensEquiparNaoZeraTrinketsTests.cs
```

### User Story 3

```text
Task: T033 - Calculador in tests/DarkestDungeon.Domain.Tests/Feature013/FichaEfetivaDePersonagemTests.cs
Task: T034 - Base imutável in tests/DarkestDungeon.Api.Tests/Feature013/FichaBaseImutavelTests.cs
Task: T035 - Atomicidade in tests/DarkestDungeon.Api.Tests/Feature013/TrocaTrinketAtomicaTests.cs
Task: T041 - Sem IPublicadorAtomicoService in tests/DarkestDungeon.Architecture.Tests/Feature013PublicadorIsolamentoTests.cs
```

## Implementation Strategy

### MVP (User Story 1)

Coletor best-effort + upsert por `NomeOriginal`. O curador consulta o inventário de trinkets mesmo antes do card.

### Incremental

1. Foundational (mutator, DTOs, calculador).
2. US1 catálogo.
3. US2 espaços no card (acessórios de teste se a wiki ainda for parcial).
4. US3 ficha segura + UI base/efetiva.
5. Polish e suíte.

### Constraints (do not violate)

- Não alterar `IPublicadorAtomicoService` nem `DerivacaoDeAtributosOficiais` (além de garantir que continua sem trinkets).
- Não reabrir ícones/Spine (006/011).
- Não recalcular criação 008/009.
- Não persistir ficha efetiva.
- Wiki só no CLI de curadoria.
