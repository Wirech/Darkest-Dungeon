---

description: "Task list for Frontend Interativo de Personagens"
---

# Tasks: Frontend Interativo de Personagens

**Input**: Design documents from `specs/007-frontend-personagens/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/personagens-api.md`, `quickstart.md`

**Organization**: Tasks are grouped by user story so each story can be implemented and validated independently after the foundational phase.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel when it touches a different file and has no dependency on incomplete work.
- **[Story]**: Maps a task to `US1`, `US2` or `US3` from `spec.md`.
- Every task names the exact file or path it creates or changes.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare the web UI location and keep the implementation aligned with the documented contract.

- [X] T001 [P] Create the static UI directory and placeholder ownership structure at `src/DarkestDungeon.Api/wwwroot/personagens/`.
- [X] T002 [P] Confirm static asset inclusion and content-copy behavior for the UI in `src/DarkestDungeon.Api/DarkestDungeon.Api.csproj`.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish the shared contracts and persistence operations required by every user story.

**Checkpoint**: The API can represent list/delete use cases through application abstractions, and the API can serve the same-origin UI assets.

- [X] T003 Extend the character summary and associated-skill DTOs in `src/DarkestDungeon.Application/Personagens/PersonagemDtos.cs` with the fields defined in `specs/007-frontend-personagens/data-model.md`.
- [X] T004 Add list and delete operations to `IPersonagemRepository` and `IPersonagemService` in `src/DarkestDungeon.Application/Abstractions/IPersonagemService.cs`.
- [X] T005 Configure static-file serving and the `/personagens` page fallback in `src/DarkestDungeon.Api/Program.cs`, preserving `/api`-style controller routing and development tools.
- [X] T006 Add the shared HTTP error/status mapping needed by list, create and delete operations in `src/DarkestDungeon.Api/Controllers/EntidadeControllerBase.cs` and `src/DarkestDungeon.Application/Validation/ResultadoOperacao.cs`.

---

## Phase 3: User Story 1 - Consultar personagens cadastrados (Priority: P1) 🎯 MVP

**Goal**: Let the user open the characters page and see populated, empty, loading and error states with the required character summary.

**Independent Test**: With zero and multiple characters in the test database, open `/personagens/` and confirm the corresponding state, displayed fields, retry action and no false empty state while loading.

### Tests for User Story 1

- [X] T007 [P] [US1] Add API contract tests for `GET /personagens` success, empty list and response fields in `tests/DarkestDungeon.Api.Tests/PersonagensListEndpointsTests.cs`.
- [X] T008 [P] [US1] Add API error-path coverage for list failures and invalid persistence state in `tests/DarkestDungeon.Api.Tests/PersonagensListEndpointsTests.cs`.

### Implementation for User Story 1

- [X] T009 [US1] Implement `ListarAsync` in `src/DarkestDungeon.Infrastructure/Repositories/PersonagemRepository.cs` using the character query without returning domain entities directly.
- [X] T010 [US1] Implement the summary projection and list use case in `src/DarkestDungeon.Application/Personagens/PersonagemService.cs`, including class and skill presentation data.
- [X] T011 [US1] Expose `GET /personagens` in `src/DarkestDungeon.Api/Controllers/Catalogo/PersonagensController.cs` with the documented `200 OK` empty-list behavior.
- [X] T012 [P] [US1] Create the accessible page shell and list/empty/loading/error regions in `src/DarkestDungeon.Api/wwwroot/personagens/index.html`.
- [X] T013 [P] [US1] Create the responsive visual system and list/card states in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.css`.
- [X] T014 [US1] Implement the list fetch, state transitions, retry action and character summary rendering in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.js`.

**Checkpoint**: `GET /personagens` and the page list can be demonstrated independently with populated, empty, loading and error states.

---

## Phase 4: User Story 2 - Criar um personagem (Priority: P1)

**Goal**: Let the user complete, validate and submit a character form, then see the new character in the list without manual reload.

**Independent Test**: Open the creation form, submit invalid data and confirm field errors; submit valid data and confirm `201 Created`, success feedback and the new row/card in the list.

### Tests for User Story 2

- [X] T015 [P] [US2] Extend creation endpoint coverage for valid payload, invalid ranges, missing name and class/skill incompatibility in `tests/DarkestDungeon.Api.Tests/PersonagensEndpointsTests.cs`.
- [X] T016 [P] [US2] Add catalog-loading contract coverage for class and class-skill selection data in `tests/DarkestDungeon.Api.Tests/PersonagemFormCatalogTests.cs`.

### Implementation for User Story 2

- [X] T017 [US2] Add the create-form fields, accessible labels, validation message regions and cancel action in `src/DarkestDungeon.Api/wwwroot/personagens/index.html`.
- [X] T018 [US2] Add form layout, field-error, disabled-submit and success/error styles in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.css`.
- [X] T019 [US2] Implement class loading, class-specific skill loading, client-side range validation and duplicate-submit protection in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.js`.
- [X] T020 [US2] Implement the create request, preserve form values on failure, render PT-BR service errors and refresh the list after success in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.js`.

**Checkpoint**: A valid character can be created from the page and appears in the list; invalid input never produces a request or false success.

---

## Phase 5: User Story 3 - Excluir um personagem (Priority: P2)

**Goal**: Let the user confirm and persistently delete a listed character while preserving the list on cancel or failure.

**Independent Test**: Select delete, cancel the confirmation and verify the character remains; confirm deletion and verify `204 No Content`, removal from the list and the empty state when it was the last character.

### Tests for User Story 3

- [X] T021 [P] [US3] Add API contract tests for `DELETE /personagens/{id}` success, missing ID, invalid ID and conflict/error mapping in `tests/DarkestDungeon.Api.Tests/PersonagensDeleteEndpointsTests.cs`.
- [X] T022 [P] [US3] Add persistence tests proving deletion removes only the selected character and leaves shared classes, skills and items intact in `tests/DarkestDungeon.Api.Tests/PersonagensDeleteEndpointsTests.cs`.

### Implementation for User Story 3

- [X] T023 [US3] Implement `RemoverAsync` in `src/DarkestDungeon.Infrastructure/Repositories/PersonagemRepository.cs` with not-found detection and persistence of the deletion.
- [X] T024 [US3] Implement the delete use case and business/error mapping in `src/DarkestDungeon.Application/Personagens/PersonagemService.cs`.
- [X] T025 [US3] Expose `DELETE /personagens/{id}` with `204`, `404`, `400` and applicable `409` responses in `src/DarkestDungeon.Api/Controllers/Catalogo/PersonagensController.cs`.
- [X] T026 [US3] Implement confirmation dialog, delete-in-progress state, success/error feedback and list/empty-state refresh in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.js`.

**Checkpoint**: Deletion is explicit, persistent, accurately reflected in the UI and never reports success when the service fails.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Verify the complete feature against the documented quickstart and constitution.

- [X] T027 [P] Add browser smoke scenarios for populated, empty, loading, error, create and delete states to `specs/007-frontend-personagens/quickstart.md` and record the observed result.
- [X] T028 [P] Add keyboard/focus and narrow-viewport checks for `src/DarkestDungeon.Api/wwwroot/personagens/index.html` and `src/DarkestDungeon.Api/wwwroot/personagens/personagens.css`.
- [X] T029 Run the complete API, domain and architecture test commands from `specs/007-frontend-personagens/quickstart.md` and resolve regressions in the affected files.
- [X] T030 Run `dotnet build DarkestDungeon.sln --no-restore --configuration Release` and verify the generated static page is served by `src/DarkestDungeon.Api/Program.cs`.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies; T001 and T002 can run in parallel.
- **Foundational (Phase 2)**: Depends on Setup; T003-T006 block all user stories.
- **User Story 1 (Phase 3)**: Depends on Foundational; establishes the list and page shell used by later stories.
- **User Story 2 (Phase 4)**: Depends on Foundational and integrates with the US1 page/list refresh, so it follows US1 in the default sequence.
- **User Story 3 (Phase 5)**: Depends on Foundational and integrates with the US1 page/list state; it can be developed in parallel with US2 after the shared page shell exists, but the default sequence is US1 → US2 → US3.
- **Polish (Phase 6)**: Depends on all desired stories being complete.

### User Story Dependencies

- **US1 (P1)**: No dependency on another user story after Foundational; MVP.
- **US2 (P1)**: Depends on the shared API/UI foundation and the US1 list refresh path; creation remains independently testable.
- **US3 (P2)**: Depends on the shared API/UI foundation and the US1 list state; deletion remains independently testable.

### Parallel Opportunities

- T001-T002 can run in parallel.
- T003 and T004 can run in parallel; T005-T006 can follow the shared contract decisions.
- In US1, T007-T008 and T012-T013 can run in parallel; T009-T011 remain ordered backend work.
- In US2, T015-T016 and T017-T018 can run in parallel; T019-T020 are ordered JavaScript integration tasks.
- In US3, T021-T022 can run in parallel; T023-T025 are ordered backend work.
- After Foundational, separate contributors can work on US2 backend tests and US3 backend tests in parallel, provided they coordinate changes to shared controller/service files.

## Parallel Execution Examples

### User Story 1

```text
Task: T007/T008 - API contract and error tests in tests/DarkestDungeon.Api.Tests/PersonagensListEndpointsTests.cs
Task: T012/T013 - page shell and responsive styles in src/DarkestDungeon.Api/wwwroot/personagens/
```

### User Story 2

```text
Task: T015/T016 - backend/catalog tests in tests/DarkestDungeon.Api.Tests/
Task: T017/T018 - form markup and styling in src/DarkestDungeon.Api/wwwroot/personagens/
```

### User Story 3

```text
Task: T021/T022 - delete contract and persistence tests in tests/DarkestDungeon.Api.Tests/PersonagensDeleteEndpointsTests.cs
Task: T026 - delete interaction after the backend contract is available in src/DarkestDungeon.Api/wwwroot/personagens/personagens.js
```

## Implementation Strategy

### MVP First (US1 + required foundation)

1. Complete Phase 1 Setup.
2. Complete Phase 2 Foundational.
3. Complete Phase 3 User Story 1.
4. Validate list, empty, loading and error states independently.
5. Demonstrate the read-only catalog before adding mutations.

### Incremental Delivery

1. Add US2 creation and validate it without changing the list contract.
2. Add US3 deletion with explicit confirmation and persistence tests.
3. Complete Phase 6 browser, responsive and full-suite validation.

### Independent Test Criteria

- **US1**: A user can see populated/empty/loading/error list states and retry a failed query.
- **US2**: A user can create a valid character, receive field errors for invalid input and see the new record without manual reload.
- **US3**: A user must confirm deletion; cancel preserves the record, success removes it, and failure preserves it with an error.

## Notes

- Tests are included because the constitution requires endpoint contract coverage for backend changes.
- `[P]` appears only on tasks that can be performed independently without modifying the same incomplete implementation slice.
- Do not mark a task complete until its code, relevant tests and validation command have passed.
