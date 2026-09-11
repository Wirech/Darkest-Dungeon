---

description: "Task list for Criação Completa de Personagem"
---

# Tasks: Criação Completa de Personagem

**Input**: Design documents from `specs/008-criacao-personagem-completa/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/personagens-criacao.md`, `quickstart.md`

**Organization**: Tasks are grouped by user story and ordered around the shared model and creation use case.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel only when files and incomplete implementation slices do not overlap.
- **[Story]**: Maps a task to `US1`, `US2` or `US3` from `spec.md`.
- Every task includes an exact file path.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare migration/test boundaries and preserve the existing frontend entry point.

- [X] T001 [P] Add the feature test fixture naming and test-data isolation notes to `tests/DarkestDungeon.Api.Tests/Fixtures/ApiTestFactory.cs`.
- [X] T002 [P] Update the character creation form field ownership in `src/DarkestDungeon.Api/wwwroot/personagens/index.html` to reserve the six essential choices and remove manual skill selection from the default flow.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish the persistent model, DTO shape, repository capabilities and migration required by all stories.

**Checkpoint**: The domain and database can represent appearance, selected equipment levels and complete skill associations without breaking old reads.

- [X] T003 Add `NivelDaArma`, `NivelDaArmadura` and explicit appearance/equipment invariants to `src/DarkestDungeon.Domain/Seres/Personagem.cs`.
- [X] T004 Extend `HabilidadeDePersonagem` construction and consistency rules for `NumeroDoNivel` 0..5 in `src/DarkestDungeon.Domain/Personagens/HabilidadeDePersonagem.cs`.
- [X] T005 [P] Add explicit creation configuration and result fields to `src/DarkestDungeon.Application/Personagens/Commands/PersonagemCommands.cs` and `src/DarkestDungeon.Application/Personagens/PersonagemDtos.cs`.
- [X] T006 [P] Extend the character detail mapper with appearance, equipment levels and skill category/level fields in `src/DarkestDungeon.Application/Personagens/PersonagemMapper.cs`.
- [X] T007 Add repository methods to load class-eligible items and ordered class skills in `src/DarkestDungeon.Application/Abstractions/IItemService.cs`, `src/DarkestDungeon.Application/Abstractions/IHabilidadeService.cs` and `src/DarkestDungeon.Application/Abstractions/IPersonagemService.cs`.
- [X] T008 Implement the item-level lookup and deterministic skill ordering queries in `src/DarkestDungeon.Infrastructure/Repositories/ItemRepository.cs` and `src/DarkestDungeon.Infrastructure/Repositories/HabilidadeRepository.cs`.
- [X] T009 Map new character fields with legacy-compatible defaults in `src/DarkestDungeon.Infrastructure/Data/DarkestDungeonDbContext.cs`.
- [X] T010 Create the versioned EF Core migration for appearance and equipment-level fields in `src/DarkestDungeon.Infrastructure/Migrations/`.
- [X] T011 Add shared error mapping for invalid configuration, missing catalog items, insufficient skills and catalog conflicts in `src/DarkestDungeon.Api/Controllers/EntidadeControllerBase.cs` and `src/DarkestDungeon.Application/Validation/ResultadoOperacao.cs`.

---

## Phase 3: User Story 1 - Criar personagem a partir das escolhas essenciais (Priority: P1) 🎯 MVP

**Goal**: Create a complete character from name, class, hero level, weapon level, armor level and appearance, deriving all related catalog references.

**Independent Test**: Submit a valid six-choice request and confirm the created detail preserves the choices, resolves class-eligible equipment and contains no manually supplied skill/item IDs.

### Tests for User Story 1

- [X] T012 [P] [US1] Add domain tests for valid appearance and equipment-level invariants in `tests/DarkestDungeon.Domain.Tests/PersonagemCriacaoCompletaTests.cs`.
- [X] T013 [P] [US1] Add API contract tests for the six-field request, selected appearance, selected equipment IDs and selected equipment levels in `tests/DarkestDungeon.Api.Tests/PersonagensCriacaoCompletaTests.cs`.

### Implementation for User Story 1

- [X] T014 [US1] Replace the creation request and command mapping with name, class, hero level, weapon level, armor level and appearance in `src/DarkestDungeon.Api/Contracts/Catalogo/PersonagemContracts.cs` and `src/DarkestDungeon.Api/Controllers/Catalogo/PersonagensController.cs`.
- [X] T015 [US1] Implement class-based weapon and armor derivation, level validation and initial attribute/resistance construction in `src/DarkestDungeon.Application/Personagens/PersonagemService.cs`.
- [X] T016 [US1] Persist selected equipment IDs, equipment levels and appearance atomically through `src/DarkestDungeon.Infrastructure/Repositories/PersonagemRepository.cs` and `src/DarkestDungeon.Infrastructure/Data/DarkestDungeonDbContext.cs`.
- [X] T017 [US1] Update the character detail response and frontend result rendering for appearance and equipment levels in `src/DarkestDungeon.Application/Personagens/PersonagemMapper.cs` and `src/DarkestDungeon.Api/wwwroot/personagens/personagens.js`.

**Checkpoint**: A valid six-choice request creates a complete character with stable derived references and the choices are visible after consultation.

---

## Phase 4: User Story 2 - Inicializar progressão de habilidades (Priority: P1)

**Goal**: Automatically associate every combat/camp skill of the class, with exactly four per category at level 1 and the remainder blocked at level 0.

**Independent Test**: Create a character for a class with enough skills and count level 1 and level 0 associations separately by category.

### Tests for User Story 2

- [X] T018 [P] [US2] Add domain tests proving `NumeroDoNivel`, `Treinada` and `Equipada` remain consistent for level 0 and level 1 associations in `tests/DarkestDungeon.Domain.Tests/PersonagemCriacaoCompletaTests.cs`.
- [X] T019 [P] [US2] Add API integration tests asserting exactly four combat and four camp skills at level 1 and all remaining skills at level 0 in `tests/DarkestDungeon.Api.Tests/PersonagensCriacaoCompletaTests.cs`.
- [X] T020 [P] [US2] Add API test proving insufficient combat/camp skill catalog rejects creation and leaves no character in `tests/DarkestDungeon.Api.Tests/PersonagensCriacaoCompletaTests.cs`.

### Implementation for User Story 2

- [X] T021 [US2] Implement deterministic class-skill partitioning, alphabetical/ID ordering and minimum-four validation in `src/DarkestDungeon.Application/Personagens/PersonagemService.cs`.
- [X] T022 [US2] Create every `HabilidadeDePersonagem` association with level 1 for the first four per category and level 0 for the rest in `src/DarkestDungeon.Domain/Seres/Personagem.cs` and `src/DarkestDungeon.Domain/Personagens/HabilidadeDePersonagem.cs`.
- [X] T023 [US2] Remove dependence on manually supplied skill IDs from the frontend creation flow and show the derived skill summary in `src/DarkestDungeon.Api/wwwroot/personagens/index.html`, `src/DarkestDungeon.Api/wwwroot/personagens/personagens.js` and `src/DarkestDungeon.Api/wwwroot/personagens/personagens.css`.
- [X] T024 [US2] Expose skill category, level and training state consistently in the character detail DTO and mapper in `src/DarkestDungeon.Application/Personagens/PersonagemDtos.cs` and `src/DarkestDungeon.Application/Personagens/PersonagemMapper.cs`.

**Checkpoint**: Every new valid character has the complete class skill set with the required 4+4 level-1 initialization and blocked remainder.

---

## Phase 5: User Story 3 - Rejeitar escolhas incompatíveis (Priority: P1)

**Goal**: Reject invalid levels, appearances, equipment and catalog states before any partial character is persisted.

**Independent Test**: Submit each invalid configuration and verify the PT-BR error, status code and absence of a newly persisted character.

### Tests for User Story 3

- [X] T025 [P] [US3] Add API tests for hero-level, weapon-level, armor-level and appearance range validation in `tests/DarkestDungeon.Api.Tests/PersonagensCriacaoCompletaTests.cs`.
- [X] T026 [P] [US3] Add API tests for class-ineligible/missing equipment, ambiguous catalog and invalid class configuration in `tests/DarkestDungeon.Api.Tests/PersonagensCriacaoCompletaTests.cs`.
- [X] T027 [P] [US3] Add a persistence assertion that failed creation does not leave a partial character or skill associations in `tests/DarkestDungeon.Api.Tests/PersonagensCriacaoCompletaTests.cs`.
- [X] T028 [P] [US3] Add compatibility coverage for reading a legacy character with absent appearance/equipment-level data in `tests/DarkestDungeon.Api.Tests/PersonagensCriacaoCompletaTests.cs`.

### Implementation for User Story 3

- [X] T029 [US3] Implement PT-BR validation and catalog-conflict outcomes in `src/DarkestDungeon.Application/Personagens/PersonagemService.cs` and `src/DarkestDungeon.Application/Validation/ResultadoOperacao.cs`.
- [X] T030 [US3] Wrap character and skill-association persistence in an atomic operation and preserve legacy read defaults in `src/DarkestDungeon.Infrastructure/Repositories/PersonagemRepository.cs` and `src/DarkestDungeon.Infrastructure/Data/DarkestDungeonDbContext.cs`.
- [X] T031 [US3] Update frontend fields, range validation and server-error rendering for the six essential choices in `src/DarkestDungeon.Api/wwwroot/personagens/index.html` and `src/DarkestDungeon.Api/wwwroot/personagens/personagens.js`.

**Checkpoint**: Invalid configuration never creates a partial character and every failure is understandable in PT-BR.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Validate migration, full contracts, compatibility and the user-facing creation flow.

- [X] T032 [P] Update the creation contract and model documentation with final field names/defaults in `specs/008-criacao-personagem-completa/contracts/personagens-criacao.md` and `specs/008-criacao-personagem-completa/data-model.md`.
- [X] T033 [P] Add the complete six-choice, 4+4 and invalid-input scenarios to `specs/008-criacao-personagem-completa/quickstart.md` and verify the browser flow in `src/DarkestDungeon.Api/wwwroot/personagens/`.
- [X] T034 Run all Domain, Architecture and API tests plus `dotnet build DarkestDungeon.sln --no-restore --configuration Release` and resolve regressions in the affected files.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: T001-T002 can start immediately and in parallel.
- **Foundational (Phase 2)**: Depends on Setup and blocks all user stories; migration and model must be coherent before use-case work.
- **US1 (Phase 3)**: Depends on Foundational and establishes the complete six-choice creation contract.
- **US2 (Phase 4)**: Depends on US1's derived creation path because the automatic skill associations are part of the same transaction.
- **US3 (Phase 5)**: Depends on Foundational and the final creation contract; validation tests may be prepared before US1/US2 implementation but the final behavior follows them.
- **Polish (Phase 6)**: Depends on all three stories.

### User Story Dependencies

- **US1 (P1)**: MVP after Foundational; independently demonstrates complete equipment/appearance derivation.
- **US2 (P1)**: Depends on US1's creation orchestration; independently verifies skill initialization and can be tested without the frontend.
- **US3 (P1)**: Depends on the same creation contract; independently verifies rejection and transaction safety.

### Parallel Opportunities

- T001-T002 can run in parallel.
- T003-T006 can be split across Domain and Application files; T007-T010 follow the interfaces/model decisions.
- US1 T012-T013 can run in parallel before T014-T017.
- US2 T018-T020 can run in parallel; T023 can proceed in the frontend while T021-T022 are implemented if the contract shape is stable.
- US3 T025-T028 can run in parallel; T031 can proceed independently of repository transaction work after the request contract is finalized.
- T032-T033 can run in parallel after the contract settles.

## Parallel Execution Examples

### User Story 1

```text
Task: T012 - Domain invariants in tests/DarkestDungeon.Domain.Tests/PersonagemCriacaoCompletaTests.cs
Task: T013 - API contract in tests/DarkestDungeon.Api.Tests/PersonagensCriacaoCompletaTests.cs
```

### User Story 2

```text
Task: T018/T019/T020 - Domain and API tests for skill initialization in tests/
Task: T023 - Remove manual skill selection from src/DarkestDungeon.Api/wwwroot/personagens/
```

### User Story 3

```text
Task: T025/T026/T027/T028 - Invalid and compatibility tests in tests/DarkestDungeon.Api.Tests/PersonagensCriacaoCompletaTests.cs
Task: T031 - Six-choice frontend validation in src/DarkestDungeon.Api/wwwroot/personagens/
```

## Implementation Strategy

### MVP First (US1 + required foundation)

1. Complete Setup and Foundational phases.
2. Implement US1 with the six-choice request, persisted appearance and equipment levels.
3. Validate a complete character detail before adding automatic skill initialization.

### Incremental Delivery

1. Add US2 and validate the complete 4+4 level initialization.
2. Add US3 validation, atomicity and legacy compatibility.
3. Run migration, full suite and browser quickstart.

### Independent Test Criteria

- **US1**: A valid request with six choices returns a character preserving the choices and derived equipment references.
- **US2**: A created character has exactly four combat and four camp skills at level 1, with all remaining class skills at level 0.
- **US3**: Invalid choices return PT-BR errors and leave no new character or partial associations; legacy records remain readable.

## Notes

- Tests are included because the constitution requires endpoint contract coverage and the feature changes persistence.
- `[P]` is used only where tasks can avoid the same incomplete file slice.
- Migration generation must be reviewed against the existing SQL Server snapshot before applying it.
- Do not mark a task complete until code, affected tests and the quickstart/build validation pass.
