---

description: "Task list for Imagens no Card do Personagem"
---

# Tasks: Imagens no Card do Personagem

**Input**: Design documents from `specs/010-imagens-card-personagem/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/personagens-card-midias.md`, `quickstart.md`

**Tests**: Incluídos — constituição III (contratos HTTP), spec SC-001..SC-005, contrato com 7 testes exigidos, plano Fase D.

**Organization**: Tasks agrupadas por user story (US1 retrato+corpo, US2 arma/armadura, US3 ícones de habilidade).

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Pode rodar em paralelo (arquivos diferentes, sem depender de fatia incompleta).
- **[Story]**: `US1`, `US2` ou `US3` conforme `spec.md`.
- Toda task inclui caminho de arquivo.

## Path Conventions

- Solução em camadas: `src/DarkestDungeon.*`, `tests/`
- Acervo: `assets/herois/`, `assets/equipamentos-itens/`
- UI: `src/DarkestDungeon.Api/wwwroot/personagens/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Pastas de teste e configuração de raízes do acervo, sem ainda resolver slots.

- [X] T001 Create Feature 010 API test folder and shared helper that points at `assets/herois` / `assets/equipamentos-itens` in `tests/DarkestDungeon.Api.Tests/Feature010/AcervoDoCardTestHelper.cs`.
- [X] T002 [P] Add acervo root settings (`Acervo:Herois`, `Acervo:EquipamentosItens`) in `src/DarkestDungeon.Api/appsettings.json` and `src/DarkestDungeon.Api/appsettings.Development.json`.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: DTO de slot, porta do resolvedor, static files `/acervo` e DI. Nenhuma história de card funciona sem isso.

**⚠️ CRITICAL**: Nenhuma user story começa até esta fase terminar.

- [X] T003 Create `SlotDeMidiaDoCardDto` (`Tipo`, `Rotulo`, `Status`, `ArquivoInventarioId`, `HashArquivo`, `Url`, `Nivel`) and `MidiasDoPersonagemDto` (`Retrato`, `CorpoInteiro`, `Arma`, `Armadura`) in `src/DarkestDungeon.Application/Midias/MidiasDoCardDtos.cs` with `OK` ⇒ `Url` + `ArquivoInventarioId` required and Pendente ⇒ `Url` null.
- [X] T004 [P] Add optional `Midias` (`MidiasDoPersonagemDto?`) to `PersonagemResumoDto` and `PersonagemDetalheDto` in `src/DarkestDungeon.Application/Personagens/PersonagemDtos.cs` without removing 009 fields.
- [X] T005 [P] Add optional `Midia` (`SlotDeMidiaDoCardDto?`) to `HabilidadeDePersonagemDto` in `src/DarkestDungeon.Application/Personagens/PersonagemDtos.cs`.
- [X] T006 Create `IResolvedorDeMidiasDoCard` (retrato, corpo, arma, armadura, habilidade → `SlotDeMidiaDoCardDto`) in `src/DarkestDungeon.Application/Midias/IResolvedorDeMidiasDoCard.cs` with no `System.IO`.
- [X] T007 Implement inventory/manifest lookup (004 `inventario.json` + `manifesto-habilidades.json`, 006 inventory when hash lives under equipamentos) in `src/DarkestDungeon.Infrastructure/Midias/ResolvedorDeMidiasDoCard.cs`; build relative URLs `/acervo/herois/{CaminhoDestino}` or `/acervo/equipamentos-itens/{CaminhoDestino}` never disk paths.
- [X] T008 Map static files `/acervo/herois` → `assets/herois` and `/acervo/equipamentos-itens` → `assets/equipamentos-itens` (reject path traversal) in `src/DarkestDungeon.Api/Program.cs`.
- [X] T009 Register `IResolvedorDeMidiasDoCard` in `src/DarkestDungeon.Api/Extensions/InfrastructureServiceCollectionExtensions.cs` (and Testing factory if it rebuilds the container).

**Checkpoint**: DTOs e `/acervo` existem; resolvedor devolve Pendente quando o arquivo não está no inventário.

---

## Phase 3: User Story 1 - Reconhecer o herói pelo retrato da aparência (Priority: P1) 🎯 MVP

**Goal**: O card/lista mostra retrato (`portrait_roster`) e corpo inteiro (`sprite.idle.png`) da classe na paleta A–D cadastrada; ausência = espaço + **sem imagem**.

**Independent Test**: Dois personagens da mesma classe com aparências distintas; cada card mostra retrato e corpo da própria pasta (`*_A` vs `*_B`); atributos 009 continuam legíveis.

### Tests for User Story 1

- [X] T010 [P] [US1] Add API contract tests that `GET /personagens` after creating two Crusaders with different `aparencia` returns distinct `midias.retrato.url` / `midias.corpoInteiro.url` (folders `_A` vs other letter) and HTTP 200 in `tests/DarkestDungeon.Api.Tests/Feature010/PersonagensCardRetratoCorpoTests.cs`.
- [X] T011 [P] [US1] Add tests that existing `portrait_roster` under `/acervo/herois/` returns 200 PNG and a missing path returns 404 in `tests/DarkestDungeon.Api.Tests/Feature010/AcervoEstaticoTests.cs`.
- [X] T012 [P] [US1] Add test that missing portrait or idle yields `status: Pendente`, `url: null`, HTTP 200, without substituting the other pose or another appearance in `tests/DarkestDungeon.Api.Tests/Feature010/PersonagensCardRetratoCorpoTests.cs`.

### Implementation for User Story 1

- [X] T013 [US1] Resolve retrato from `Classe.Assets` matching `Personagem.Aparencia` (`portrait_roster` / `ConjuntoSpineId`) in `src/DarkestDungeon.Infrastructure/Midias/ResolvedorDeMidiasDoCard.cs`.
- [X] T014 [US1] Resolve corpo inteiro as `{prefixo}_{A|B|C|D}/anim/*.sprite.idle.png` for the same appearance (no Spine, no atlas crop) in `src/DarkestDungeon.Infrastructure/Midias/ResolvedorDeMidiasDoCard.cs`.
- [X] T015 [US1] Fill `midias.retrato` and `midias.corpoInteiro` in `PersonagemMapper.ParaResumo` / `ParaDto` and call the resolver from `ListarAsync` / `ObterAsync` in `src/DarkestDungeon.Application/Personagens/PersonagemMapper.cs` and `src/DarkestDungeon.Application/Personagens/PersonagemService.cs`.
- [X] T016 [US1] Render labeled Retrato and Corpo inteiro slots; `status !== OK` or `img.onerror` → text `sem imagem` in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.js` and `src/DarkestDungeon.Api/wwwroot/personagens/personagens.css`.

**Checkpoint**: Lista autossuficiente mostra retrato + corpo da aparência certa; Pendente não quebra o card.

---

## Phase 4: User Story 2 - Ver arma e armadura no nível atual (Priority: P1)

**Goal**: Ícones de arma/armadura seguem `NivelDaArma` / `NivelDaArmadura` (arquivo índice N−1); nulo ou ausente → **sem imagem**; detalhe deixa de usar `NivelDeMidia(resolução)`.

**Independent Test**: Dois da mesma classe com arma 1 e 5 (`eqp_weapon_0` vs `eqp_weapon_4`); personagem sem nível de arma mostra Pendente, sem inventar nível 1.

### Tests for User Story 2

- [X] T017 [P] [US2] Add API tests that two same-class heroes with `nivelDaArma` 1 and 5 get `eqp_weapon_0` vs `eqp_weapon_4` (or equivalent ids) and matching armor index in `tests/DarkestDungeon.Api.Tests/Feature010/PersonagensCardEquipamentoTests.cs`.
- [X] T018 [P] [US2] Add API test that null weapon/armor level or missing file yields equipment slot `Pendente` / `url` null / HTTP 200 without inventing level 1 in `tests/DarkestDungeon.Api.Tests/Feature010/PersonagensCardEquipamentoTests.cs`.
- [X] T019 [P] [US2] Add API test that `GET /personagens/{id}` `midiaArmaEquipada` / `midiaArmaduraEquipada` (and `midias.arma` / `midias.armadura`) use cadastrado levels not resolve-level mapping in `tests/DarkestDungeon.Api.Tests/Feature010/PersonagensDetalheEquipamentoNivelTests.cs`.

### Implementation for User Story 2

- [X] T020 [US2] Resolve arma/armadura by `NivelDaArma`/`NivelDaArmadura` (prefer `MidiaDeItem` OK at that level, else `eqp_weapon_{N-1}` / `eqp_armour_{N-1}`) in `src/DarkestDungeon.Infrastructure/Midias/ResolvedorDeMidiasDoCard.cs`.
- [X] T021 [US2] Stop using `MidiaDeItemDtoMapper.NivelDeMidia(personagem.NivelDeResolucao)` for equipped media; use cadastrado levels in `src/DarkestDungeon.Application/Personagens/PersonagemMapper.cs`.
- [X] T022 [US2] Load class weapons/armors (not only `ArmaEquipadaId`) in `ListarAsync` so the card works without equipped FKs in `src/DarkestDungeon.Application/Personagens/PersonagemService.cs`.
- [X] T023 [US2] Render labeled Arma and Armadura slots next to existing level text with `sem imagem` fallback in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.js` and `src/DarkestDungeon.Api/wwwroot/personagens/personagens.css`.

**Checkpoint**: Equipamento do card bate com o nível escrito; detalhe alinhado; ausência visível.

---

## Phase 5: User Story 3 - Ver as imagens das habilidades no card (Priority: P2)

**Goal**: Cada habilidade listada (combate e acampamento, inclusive Nv. 0) tem ícone único; nível só no texto; acampamento sem PNG importado → **sem imagem**; seção vazia sem slots órfãos.

**Independent Test**: Card com combate Nv. 0 e Nv. ≥1: mesmo `url`; acampamento sem arquivo mostra espaço + texto, nome e nível permanecem.

### Tests for User Story 3

- [X] T024 [P] [US3] Add API tests that the same `habilidadeId` at Nv. 0 and Nv. 3 shares `midia.url` (or both Pendente) in `tests/DarkestDungeon.Api.Tests/Feature010/PersonagensCardHabilidadesMidiaTests.cs`.
- [X] T025 [P] [US3] Add API test that listed camping skills without imported PNG are `Pendente` still with nome + `numeroDoNivel`, and a character with no skills has no orphan media slots in `tests/DarkestDungeon.Api.Tests/Feature010/PersonagensCardHabilidadesMidiaTests.cs`.

### Implementation for User Story 3

- [X] T026 [US3] Map combate `NomeOriginal` + class prefix through `assets/herois/manifesto-habilidades.json` to `{classe}.ability.*.png` (icon independent of `NumeroDoNivel`) in `src/DarkestDungeon.Infrastructure/Midias/ResolvedorDeMidiasDoCard.cs`.
- [X] T027 [US3] Attach `habilidades[].midia` in `PersonagemMapper.MapearHabilidades` for every listed skill including Nv. 0 in `src/DarkestDungeon.Application/Personagens/PersonagemMapper.cs`.
- [X] T028 [US3] Show icon + nome + `Nv. n` per skill line; Pendente/`onerror` → `sem imagem`; empty sections stay empty in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.js` and `src/DarkestDungeon.Api/wwwroot/personagens/personagens.css`.

**Checkpoint**: Combate e acampamento listados têm slot de ícone; Nv. 0 não muda o PNG.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Regressão 009, camadas, layout estreito, quickstart.

- [X] T029 [P] Extend `GET /personagens` 009 regression to still assert HP, resistências, passos, extras da classe plus new `midias` in `tests/DarkestDungeon.Api.Tests/Feature009/PersonagensCardListaTests.cs`.
- [X] T030 [P] Add architecture assertion that Application `IResolvedorDeMidiasDoCard` / DTOs do not reference `System.IO` in `tests/DarkestDungeon.Architecture.Tests/`.
- [X] T031 Keep create/delete usable when images are Pendente (FR-010) in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.js`.
- [X] T032 Tighten card CSS so retrato, corpo, equipamento and skills stay readable on a narrow viewport without covering Excluir in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.css`.
- [X] T033 Run `specs/010-imagens-card-personagem/quickstart.md` against `http://localhost:5140` (`dotnet test --filter FullyQualifiedName~Feature010` then list + `/personagens/index.html`).

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: Sem dependências.
- **Foundational (Phase 2)**: Depende do Setup — **bloqueia** todas as user stories.
- **US1 (Phase 3)**: Após Phase 2 — MVP.
- **US2 (Phase 4)**: Após Phase 2; pode seguir US1 no mesmo mapper/JS, mas o contrato de equipamento é testável sozinho.
- **US3 (Phase 5)**: Após Phase 2; ícones de habilidade não exigem retrato OK.
- **Polish (Phase 6)**: Após as stories desejadas.

### User Story Dependencies

- **User Story 1 (P1)**: Após Phase 2. Independente de arma/habilidade.
- **User Story 2 (P1)**: Após Phase 2. Pode integrar o mesmo `midias` da US1; testável com retrato Pendente.
- **User Story 3 (P2)**: Após Phase 2. Independente se `habilidades[].midia` estiver no DTO (T005).

### Within Each User Story

- Testes primeiro (devem falhar) → resolvedor → mapper/serviço → UI.
- Não importar mídia (FR-009). Não migrar SQL. Não recalcular heróis antigos.

### Parallel Opportunities

- T002 paralelo a T001.
- T003, T004, T005 paralelos após T001/T002.
- T010, T011, T012 paralelos; T017–T019 paralelos; T024–T025 paralelos.
- T029 e T030 paralelos no polish.
- Com equipe: US1/US2/US3 após Phase 2 em paralelo (cuidado com `PersonagemMapper.cs` e `personagens.js` — serializar esses arquivos).

---

## Parallel Example: User Story 1

```text
T010 PersonagensCardRetratoCorpoTests (aparências distintas)
T011 AcervoEstaticoTests
T012 Pendente sem substituir pose
```

Depois, em série: T013 → T014 → T015 → T016.

## Parallel Example: User Story 2

```text
T017 níveis 1 vs 5
T018 nível nulo / arquivo ausente
T019 detalhe usa nível cadastrado
```

## Parallel Example: User Story 3

```text
T024 mesmo URL Nv. 0 e Nv. 3
T025 acampamento Pendente / sem órfãos
```

---

## Implementation Strategy

### MVP (User Story 1 only)

Phase 1 + 2 + US1 (T001–T016): retrato e corpo no card, `/acervo`, **sem imagem**. Já identifica o herói.

### Incremental Delivery

1. MVP visual (aparência).
2. US2 equipamento no nível certo + correção do detalhe 006.
3. US3 ícones de habilidade.
4. Polish / quickstart.

### Incremental Testing

- US1: dois Cruzados A vs B + PNG estático.
- US2: arma 1 vs 5; detalhe ≠ resolução.
- US3: Nv. 0 = Nv. 3; camping Pendente.
- Cada story permanece utilizável se as outras slots estiverem Pendente.
