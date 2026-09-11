---

description: "Task list for Corpo Inteiro Composto e Animado"
---

# Tasks: Corpo Inteiro Composto e Animado

**Input**: Design documents from `specs/012-corpo-inteiro-animado/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/personagens-corpo-inteiro.md`, `quickstart.md`

**Tests**: Incluídos — constituição III (contratos HTTP), spec SC-001..SC-008, contrato com 7 testes exigidos, plano Fases A e D.

**Organization**: Tasks agrupadas por user story (US1 corpo composto, US2 seletor de versão, US3 card utilizável).

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Pode rodar em paralelo (arquivos diferentes, sem depender de fatia incompleta).
- **[Story]**: `US1`, `US2` ou `US3` conforme `spec.md`.
- Toda task inclui caminho de arquivo.

## Path Conventions

- Solução em camadas: `src/DarkestDungeon.*`, `tests/`
- Acervo: `assets/herois/`
- UI: `src/DarkestDungeon.Api/wwwroot/personagens/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Pasta de testes 012 e MIME de `.atlas`/`.skel` no acervo estático, sem ainda resolver conjuntos Spine.

- [X] T001 Create Feature 012 API test folder and reuse acervo helper from Feature 010 in `tests/DarkestDungeon.Api.Tests/Feature012/` (helper paths via `tests/DarkestDungeon.Api.Tests/Feature010/AcervoDoCardTestHelper.cs`).
- [X] T002 [P] Map static MIME `.atlas` → `text/plain` (utf-8 allowed) and `.skel` → `application/octet-stream` on `/acervo/herois` in `src/DarkestDungeon.Api/Program.cs`.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: DTOs de conjunto/versão no slot de corpo. Nenhuma história do player funciona sem o contrato enriquecido.

**⚠️ CRITICAL**: Nenhuma user story começa até esta fase terminar.

- [X] T003 Extend `SlotDeMidiaDoCardDto` with optional `ConjuntoIdle`, `ConjuntoWalk` and `Versoes` in `src/DarkestDungeon.Application/Midias/MidiasDoCardDtos.cs` (records `ConjuntoDeCorpoDto` and `VersaoDoCorpoDto`; `OK` still requires `Url` + `ArquivoInventarioId`; Pendente ⇒ `Url` null).
- [X] T004 [P] Add PT-BR version ids/labels (`emEspera`/`Em espera`, `animado`/`Animado`, `caminhada`/`Caminhada`) as constants next to `SlotDeMidiaDoCard` in `src/DarkestDungeon.Application/Midias/MidiasDoCardDtos.cs`.
- [X] T005 Keep `IResolvedorDeMidiasDoCard.ResolverCorpoInteiro` returning the enriched slot (no `System.IO`) in `src/DarkestDungeon.Application/Midias/IResolvedorDeMidiasDoCard.cs`.

**Checkpoint**: DTO serializa conjuntos; MIME atlas/skel configurado; resolvedor ainda pode devolver só o PNG idle até a US1.

---

## Phase 3: User Story 1 - Ver o herói montado como no jogo (Priority: P1) 🎯 MVP

**Goal**: `GET /personagens` descreve o conjunto idle (atlas+skel da classe + PNG da paleta); o card mostra o herói **composto** (Spine 2.1 pausado em idle), nunca a folha de partes; ausência = **sem imagem**.

**Independent Test**: Dois Cruzados A vs B: `conjuntoIdle.urlTextura` em pastas `_A` vs `_B`; `urlAtlas`/`urlEsqueleto` em `arquivos/cruzado/anim/` sem paleta; na UI o espaço “Corpo inteiro” é silhueta montada, não grade de membros.

### Tests for User Story 1

- [X] T006 [P] [US1] Add API tests that two Crusaders with different `aparencia` get distinct `conjuntoIdle.urlTextura` (`_A` vs `_B`) and shared-class `urlAtlas`/`urlEsqueleto` under `arquivos/cruzado/anim/` in `tests/DarkestDungeon.Api.Tests/Feature012/PersonagensCorpoConjuntoIdleTests.cs`.
- [X] T007 [P] [US1] Add tests that slot `url` still contains `sprite.idle` and the appearance folder (Feature 010 regression) when idle is complete in `tests/DarkestDungeon.Api.Tests/Feature012/PersonagensCorpoConjuntoIdleTests.cs`.
- [X] T008 [P] [US1] Add tests that incomplete idle yields `status: Pendente`, `url` null, HTTP 200, without substituting the parts sheet or another appearance in `tests/DarkestDungeon.Api.Tests/Feature012/PersonagensCorpoConjuntoIdleTests.cs`.
- [X] T009 [P] [US1] Add static-file tests: Cruzado idle `.atlas` 200 text, `.skel` 200 `application/octet-stream`, paleta PNG 200 `image/png`, missing path 404 in `tests/DarkestDungeon.Api.Tests/Feature012/AcervoSpineEstaticoTests.cs`.

### Implementation for User Story 1

- [X] T010 [US1] Resolve idle trio in `ResolverCorpoInteiro`: atlas+skel at `arquivos/{pasta}/anim/{prefixo}.sprite.idle.{atlas,skel}` and PNG at `{prefixo}_{A-D}/anim/{prefixo}.sprite.idle.png`; `status` OK only if all three exist in `src/DarkestDungeon.Infrastructure/Midias/ResolvedorDeMidiasDoCard.cs`.
- [X] T011 [US1] Fill `versoes` with three entries; `emEspera` and `animado` `disponivel` iff idle complete in `src/DarkestDungeon.Infrastructure/Midias/ResolvedorDeMidiasDoCard.cs`.
- [X] T012 [US1] Pass enriched `midias.corpoInteiro` through `PersonagemMapper.ParaResumo` / `ParaDto` in `src/DarkestDungeon.Application/Personagens/PersonagemMapper.cs`.
- [X] T013 [P] [US1] Vendor Spine 2.1 runtime + license into `src/DarkestDungeon.Api/wwwroot/personagens/vendor/spine-2.1/` (no CDN, no npm).
- [X] T014 [US1] Load vendor script from `src/DarkestDungeon.Api/wwwroot/personagens/index.html`.
- [X] T015 [US1] Replace corpo `<img>` with Spine canvas using `conjuntoIdle`; pose paused at idle (“Em espera”); failure → `sem imagem`; never show the atlas sheet as the main visual in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.js`.
- [X] T016 [US1] Size/clip the corpo canvas inside `.media-slot` (overflow hidden, pixelated) in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.css`.

**Checkpoint**: Lista autossuficiente + card composto na pose de espera; Pendente não mostra folha de partes.

---

## Phase 4: User Story 2 - Escolher a versão visual do corpo (Priority: P1)

**Goal**: Seletor PT-BR por card: **Em espera**, **Animado** (loop idle), **Caminhada** (loop walk no lugar). Independente por personagem; recarregar volta a Em espera; ataque não aparece.

**Independent Test**: Alternar versões num card com idle+walk completos; o vizinho não muda; F5 zera todos para Em espera; Caminhada indisponível se walk incompleto.

### Tests for User Story 2

- [X] T017 [P] [US2] Add API tests that idle+walk complete ⇒ three `versoes` `disponivel: true` and `conjuntoWalk` URLs use class `anim/` atlas/skel plus paleta PNG `sprite.walk` in `tests/DarkestDungeon.Api.Tests/Feature012/PersonagensCorpoVersoesTests.cs`.
- [X] T018 [P] [US2] Add API test that missing walk ⇒ `caminhada.disponivel === false`, `conjuntoWalk` null, idle `status` OK in `tests/DarkestDungeon.Api.Tests/Feature012/PersonagensCorpoVersoesTests.cs`.
- [X] T019 [P] [US2] Add static-file tests for walk `.atlas`/`.skel`/PNG paleta in `tests/DarkestDungeon.Api.Tests/Feature012/AcervoSpineEstaticoTests.cs`.

### Implementation for User Story 2

- [X] T020 [US2] Resolve walk trio (`sprite.walk`) the same way as idle; do not expose `sprite.attack_*` in `src/DarkestDungeon.Infrastructure/Midias/ResolvedorDeMidiasDoCard.cs`.
- [X] T021 [US2] Render per-card selector with labels `Em espera`, `Animado`, `Caminhada`; disable unavailable options; default `emEspera` in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.js`.
- [X] T022 [US2] Keep selected version in page memory keyed by `personagem.id` (no localStorage, no POST); `carregarLista`/reload resets to Em espera in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.js`.
- [X] T023 [US2] Play Animado as looping idle; Caminhada as looping walk **in place** (cancel root translation); stay inside the corpo slot in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.js`.
- [X] T024 [US2] Style the selector so it does not cover retrato or actions in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.css`.

**Checkpoint**: Três versões no card; escolha temporária; walk no lugar; ataque ausente.

---

## Phase 5: User Story 3 - Manter o card utilizável enquanto o corpo anima (Priority: P2)

**Goal**: Retrato, equipamento, habilidades e ações continuam legíveis e clicáveis com o corpo composto ou animado; viewport estreita não cobre dados essenciais.

**Independent Test**: Com Caminhada ligada, ler nome/HP/habilidade e acionar Excluir (cancelar); o herói não sai do `figure` “Corpo inteiro”.

### Tests for User Story 3

- [X] T025 [P] [US3] Extend Feature 009/010/011 list regression so HP, resistências, acampamento and `midias` still serialize with the new corpo fields in `tests/DarkestDungeon.Api.Tests/Feature009/PersonagensCardListaTests.cs`.

### Implementation for User Story 3

- [X] T026 [US3] Set canvas `pointer-events` so animation does not steal clicks from card actions or skill lines in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.js` and `src/DarkestDungeon.Api/wwwroot/personagens/personagens.css`.
- [X] T027 [US3] Keep overflow hidden on the corpo slot; walk must not overlap retrato, text or neighbor cards in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.css`.
- [X] T028 [US3] Preserve create/delete and **sem imagem** fallback when the player fails in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.js`.

**Checkpoint**: Card 009–011 usável com Animado/Caminhada; SC-008 visual.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Camadas, regressão, quickstart.

- [X] T029 [P] Add architecture assertion that Application DTOs / `IResolvedorDeMidiasDoCard` still do not reference `System.IO` in `tests/DarkestDungeon.Architecture.Tests/`.
- [X] T030 [P] Confirm Feature 010 retrato/corpo URL tests still pass (`sprite.idle` + paleta) in `tests/DarkestDungeon.Api.Tests/Feature010/PersonagensCardRetratoCorpoTests.cs`.
- [X] T031 Run `dotnet test tests/DarkestDungeon.Api.Tests/DarkestDungeon.Api.Tests.csproj --filter "FullyQualifiedName~Feature010|FullyQualifiedName~Feature011|FullyQualifiedName~Feature012"` and fix regressions.
- [X] T032 Run `specs/012-corpo-inteiro-animado/quickstart.md` against `http://localhost:5140` (lista JSON + `/personagens/index.html`: composto, seletor, recarregar, recorte).

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: Sem dependências.
- **Foundational (Phase 2)**: Depende do Setup — **bloqueia** todas as user stories.
- **US1 (Phase 3)**: Após Phase 2 — MVP (contrato idle + player composto).
- **US2 (Phase 4)**: Após US1 (mesmo canvas/seletor); contrato walk pode ser escrito em paralelo aos testes da US1.
- **US3 (Phase 5)**: Após US2 (animação já existe para recortar/clicar).
- **Polish (Phase 6)**: Após as stories desejadas.

### User Story Dependencies

- **User Story 1 (P1)**: Após Phase 2. Independente do seletor (padrão = espera composta).
- **User Story 2 (P1)**: Depende do player idle da US1 para Animado; walk é incremento do mesmo resolvedor/JS.
- **User Story 3 (P2)**: Depende do canvas da US1/US2; testável com Caminhada ligada.

### Within Each User Story

- Testes de contrato primeiro (devem falhar) → resolvedor → mapper → UI.
- Não importar mídia (FR-009). Não migrar SQL. Não persistir versão.

### Parallel Opportunities

- T002 paralelo a T001.
- T004 paralelo a T003.
- T006, T007, T008, T009 paralelos; T013 paralelo a T010–T012.
- T017, T018, T019 paralelos após Phase 2.
- T029 e T030 paralelos no polish.
- `personagens.js` e `ResolvedorDeMidiasDoCard.cs` são pontos de serialização (não paralelizar writers no mesmo arquivo).

---

## Parallel Example: User Story 1

```text
T006 PersonagensCorpoConjuntoIdleTests (paletas)
T007 regressão url sprite.idle
T008 Pendente sem folha
T009 AcervoSpineEstaticoTests idle
```

Depois: T010 → T011 → T012; T013 ∥ T010; então T014 → T015 → T016.

## Parallel Example: User Story 2

```text
T017 versões idle+walk
T018 walk ausente
T019 estático walk
```

Depois, em série no JS: T020 → T021 → T022 → T023 → T024.

## Parallel Example: User Story 3

```text
T025 regressão lista 009
```

Depois T026 → T027 → T028 (mesmo CSS/JS — serializar).

---

## Implementation Strategy

### MVP (User Story 1 only)

Phase 1 + 2 + US1 (T001–T016): herói composto na espera, contrato idle, MIME Spine, **sem imagem**. Já cumpre “não é folha de partes”.

### Incremental Delivery

1. MVP visual composto (espera).
2. US2 seletor + Animado + Caminhada no lugar.
3. US3 recorte, cliques, viewport.
4. Polish / quickstart.

### Incremental Testing

- US1: dois Cruzados A vs B + atlas/skel 200 + UI montada.
- US2: três versões disponíveis; walk Pendente; F5 → Em espera.
- US3: Caminhada não cobre Excluir nem sai do slot.
- Cada story permanece utilizável se walk estiver Pendente.

---

## Notes

- Runtime MUST be Spine **2.1** (skel `2.1.27`); not 4.x.
- Atlas first-line PNG name MUST resolve to paleta `urlTextura`, not class-folder PNG.
- Forma humana only (Abominação como na 010). Sem `sprite.attack_*` no seletor.
