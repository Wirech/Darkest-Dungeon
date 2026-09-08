# Tasks: Base de API e Recurso Ser

**Input**: Design documents from `/specs/001-base-api-ser/`

**Prerequisites**: [plan.md](plan.md), [spec.md](spec.md), [research.md](research.md), [data-model.md](data-model.md), [contracts/openapi.yaml](contracts/openapi.yaml), [quickstart.md](quickstart.md)

**Tests**: Tests are required by the project constitution for backend endpoints and their returned status/body. Endpoint tests are listed before implementation tasks in each user story phase.

**Organization**: Tasks are grouped by user story so each story can be implemented, tested, and validated independently.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel because it touches different files and has no dependency on incomplete tasks.
- **[Story]**: Required only for user story phases (`US1`, `US2`, `US3`).
- Every task includes an exact target file path.

## Phase 1: Setup

**Purpose**: Create the .NET solution, projects, references, packages, and baseline configuration required by all stories.

- [X] T001 Create .NET solution file in DarkestDungeon.sln
- [X] T002 Create web API project in src/DarkestDungeon.Api/DarkestDungeon.Api.csproj
- [X] T003 Create application project in src/DarkestDungeon.Application/DarkestDungeon.Application.csproj
- [X] T004 Create domain project in src/DarkestDungeon.Domain/DarkestDungeon.Domain.csproj
- [X] T005 Create infrastructure project in src/DarkestDungeon.Infrastructure/DarkestDungeon.Infrastructure.csproj
- [X] T006 Create API and architecture test projects in tests/DarkestDungeon.Api.Tests/DarkestDungeon.Api.Tests.csproj and tests/DarkestDungeon.Architecture.Tests/DarkestDungeon.Architecture.Tests.csproj
- [X] T007 Create domain test project in tests/DarkestDungeon.Domain.Tests/DarkestDungeon.Domain.Tests.csproj
- [X] T008 Add project references in src/DarkestDungeon.Api/DarkestDungeon.Api.csproj, src/DarkestDungeon.Application/DarkestDungeon.Application.csproj, src/DarkestDungeon.Infrastructure/DarkestDungeon.Infrastructure.csproj, tests/DarkestDungeon.Api.Tests/DarkestDungeon.Api.Tests.csproj, tests/DarkestDungeon.Architecture.Tests/DarkestDungeon.Architecture.Tests.csproj, and tests/DarkestDungeon.Domain.Tests/DarkestDungeon.Domain.Tests.csproj
- [X] T009 Add ASP.NET Core, EF Core SQL Server, OpenAPI, xUnit, WebApplicationFactory, FluentAssertions, Testcontainers, and NetArchTest or ArchUnitNET packages in the appropriate .csproj files under src/ and tests/
- [X] T010 Create shared build configuration with nullable and implicit usings enabled in Directory.Build.props
- [X] T011 Create baseline API configuration with SQL Server connection-string placeholder in src/DarkestDungeon.Api/appsettings.json
- [X] T012 Create git ignore rules for .NET, SQL Server local artifacts, secrets, build outputs, and test outputs in .gitignore

## Phase 2: Foundational

**Purpose**: Implement shared architecture pieces that block all user stories.

- [X] T013 Create identifiable domain base class with Guid Id in src/DarkestDungeon.Domain/Common/EntidadeIdentificavel.cs
- [X] T014 Create operation result abstraction for success, not found, invalid input, and persistence failures in src/DarkestDungeon.Application/Validation/ResultadoOperacao.cs
- [X] T015 Create API error response contract in src/DarkestDungeon.Api/Contracts/ErroResponse.cs
- [X] T016 Create generic identifiable service interface in src/DarkestDungeon.Application/Abstractions/IIdentificavelService.cs
- [X] T017 Create SQL Server EF Core DbContext shell in src/DarkestDungeon.Infrastructure/Data/DarkestDungeonDbContext.cs
- [X] T018 Create service registration extensions for API, application, and infrastructure dependencies in src/DarkestDungeon.Api/Extensions/ServiceCollectionExtensions.cs, src/DarkestDungeon.Api/Extensions/ApplicationServiceCollectionExtensions.cs, and src/DarkestDungeon.Api/Extensions/InfrastructureServiceCollectionExtensions.cs
- [X] T019 Configure Program startup with controllers, OpenAPI, PT-BR JSON error behavior, SQL Server services, and health endpoint in src/DarkestDungeon.Api/Program.cs
- [X] T020 Create API integration test factory using SQL Server Testcontainers in tests/DarkestDungeon.Api.Tests/Fixtures/ApiTestFactory.cs

## Phase 3: User Story 1 - Consultar entidade por identificador (Priority: P1)

**Goal**: Provide a reusable ID-based endpoint contract that returns consistent success, invalid ID, and not-found responses.

**Independent Test**: Query an existing resource by valid ID, query a missing ID, and query an invalid ID while asserting status and response body.

- [X] T021 [P] [US1] Add endpoint tests for generic ID success, missing ID, and invalid ID behavior in tests/DarkestDungeon.Api.Tests/IdentificavelEndpointsTests.cs
- [X] T022 [P] [US1] Add controller base tests for PT-BR error mapping in tests/DarkestDungeon.Api.Tests/EntidadeControllerBaseTests.cs
- [X] T023 [US1] Implement generic ID controller base for identifiable resources in src/DarkestDungeon.Api/Controllers/EntidadeControllerBase.cs
- [X] T024 [US1] Implement reusable not-found and invalid-ID response mapping in src/DarkestDungeon.Api/Controllers/EntidadeControllerBase.cs
- [X] T025 [US1] Implement generic identifiable lookup contract in src/DarkestDungeon.Application/Abstractions/IIdentificavelService.cs
- [X] T026 [US1] Wire generic ID endpoint behavior into API routing conventions in src/DarkestDungeon.Api/Program.cs

## Phase 4: User Story 2 - Cadastrar e consultar Ser (Priority: P2)

**Goal**: Create and fetch a Ser with all required attributes, range validation, resistances, PT-BR errors, and the reusable ID contract.

**Independent Test**: Create a valid Ser, retrieve it by returned ID, and reject invalid requests for missing name/type, HP, `DanoBaseMinimo`/`DanoBaseMaximo`, bounded attributes, and resistances.

- [X] T027 [P] [US2] Add domain tests for Ser validation rules in tests/DarkestDungeon.Domain.Tests/SerTests.cs
- [X] T028 [P] [US2] Add endpoint tests for creating and retrieving a valid Ser in tests/DarkestDungeon.Api.Tests/SeresEndpointsTests.cs
- [X] T029 [P] [US2] Add endpoint tests for Ser validation failures in tests/DarkestDungeon.Api.Tests/SeresValidationTests.cs
- [X] T030 [US2] Implement Resistencias value object with 0 to 100 bounds in src/DarkestDungeon.Domain/Seres/Resistencias.cs
- [X] T031 [US2] Implement Ser domain entity with inherited Id, required text fields, HP rules, DanoBaseMinimo/DanoBaseMaximo range, attribute bounds, and resistances in src/DarkestDungeon.Domain/Seres/Ser.cs
- [X] T032 [P] [US2] Create request contract with all Ser fields and resistances in src/DarkestDungeon.Api/Contracts/CriarSerRequest.cs
- [X] T033 [P] [US2] Create response contract with Id, all Ser fields, DanoBaseMinimo/DanoBaseMaximo, and resistances in src/DarkestDungeon.Api/Contracts/SerResponse.cs
- [X] T034 [P] [US2] Create application DTO for Ser in src/DarkestDungeon.Application/Seres/SerDto.cs
- [X] T035 [P] [US2] Create command object for Ser creation in src/DarkestDungeon.Application/Seres/CriarSerCommand.cs
- [X] T036 [US2] Create Ser service interface for create and get-by-ID operations in src/DarkestDungeon.Application/Abstractions/ISerService.cs
- [X] T037 [US2] Implement Ser application service with validation result mapping in src/DarkestDungeon.Application/Seres/SerService.cs
- [X] T038 [US2] Create Ser repository interface in src/DarkestDungeon.Application/Abstractions/ISerRepository.cs
- [X] T039 [US2] Implement Ser repository with EF Core queries and inserts in src/DarkestDungeon.Infrastructure/Repositories/SerRepository.cs
- [X] T040 [US2] Configure Ser and Resistencias EF Core mapping in src/DarkestDungeon.Infrastructure/Data/DarkestDungeonDbContext.cs
- [X] T041 [US2] Implement Seres controller with POST /seres and GET /seres/{id} in src/DarkestDungeon.Api/Controllers/SeresController.cs
- [X] T042 [US2] Register Ser service and repository dependencies in src/DarkestDungeon.Api/Extensions/ServiceCollectionExtensions.cs
- [X] T043 [US2] Update generated OpenAPI metadata to match contracts in src/DarkestDungeon.Api/Program.cs

## Phase 5: User Story 3 - Persistir dados entre usos (Priority: P3)

**Goal**: Ensure Ser data persists in SQL Server across restarts and supports concurrent remote access without duplicated IDs or inconsistent reads.

**Independent Test**: Create a Ser, restart the API, fetch the same ID, then run 20 simultaneous GET requests and verify consistent responses.

- [X] T044 [P] [US3] Add persistence-across-restart integration test in tests/DarkestDungeon.Api.Tests/SeresPersistenceTests.cs
- [X] T045 [P] [US3] Add 20-user concurrent read integration test in tests/DarkestDungeon.Api.Tests/SeresConcurrencyTests.cs
- [X] T046 [US3] Add initial EF Core migration for Ser and Resistencias persistence in src/DarkestDungeon.Infrastructure/Data/Migrations/
- [X] T047 [US3] Ensure SQL Server migration creates unique IDs and required constraints in src/DarkestDungeon.Infrastructure/Data/Migrations/
- [X] T048 [US3] Configure production-safe connection string loading from environment in src/DarkestDungeon.Api/Extensions/ServiceCollectionExtensions.cs
- [X] T049 [US3] Add controlled PT-BR persistence failure response behavior in src/DarkestDungeon.Api/Program.cs

## Phase 6: User Story 4 - Evoluir tipos derivados com baixo retrabalho (Priority: P4)

**Goal**: Ensure the base architecture supports future Ser-derived types with low rework, interface contracts between layers, and DI container validation.

**Independent Test**: Run architecture tests confirming layer dependency rules, DI resolution, and reuse of common Ser validation for future derived types.

- [X] T050 [P] [US4] Add DI container resolution tests for IIdentificavelService, ISerService, IRepositorioIdentificavel, ISerRepository, and DarkestDungeonDbContext in tests/DarkestDungeon.Architecture.Tests/DependencyInjectionTests.cs
- [X] T051 [P] [US4] Add layer dependency tests preventing Domain references to Api or Infrastructure in tests/DarkestDungeon.Architecture.Tests/LayerDependencyTests.cs
- [X] T052 [P] [US4] Add Ser extensibility tests proving future derived types can reuse base validation rules in tests/DarkestDungeon.Architecture.Tests/SerExtensibilityTests.cs
- [X] T053 [US4] Create generic identifiable repository abstraction in src/DarkestDungeon.Application/Abstractions/IRepositorioIdentificavel.cs
- [X] T054 [US4] Refactor Ser repository contract to align with identifiable abstractions in src/DarkestDungeon.Application/Abstractions/ISerRepository.cs
- [X] T055 [US4] Centralize common Ser validation for reuse by future derived types in src/DarkestDungeon.Domain/Seres/Ser.cs
- [X] T056 [US4] Register and validate layer-specific DI modules in src/DarkestDungeon.Api/Extensions/ApplicationServiceCollectionExtensions.cs and src/DarkestDungeon.Api/Extensions/InfrastructureServiceCollectionExtensions.cs
- [X] T057 [US4] Document interface and DI contracts alignment in specs/001-base-api-ser/contracts/interfaces.md

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Align documentation, validation, and final quality checks with the Spec Kit artifacts.

- [X] T058 [P] Update README with setup, SQL Server connection string, migration, test, and run commands in README.md
- [X] T059 [P] Update API usage examples for Ser in docs/api-ser.md
- [X] T060 Validate OpenAPI output against specs/001-base-api-ser/contracts/openapi.yaml and record any intentional differences in docs/api-ser.md
- [X] T061 Validate interface contracts against specs/001-base-api-ser/contracts/interfaces.md and record any intentional differences in docs/api-ser.md
- [X] T062 Run full backend and architecture test suites and fix failures in tests/DarkestDungeon.Api.Tests/, tests/DarkestDungeon.Architecture.Tests/, and tests/DarkestDungeon.Domain.Tests/
- [X] T063 Run quickstart validation scenarios and document results in specs/001-base-api-ser/quickstart.md

## Dependencies

- Phase 1 must complete before Phase 2.
- Phase 2 must complete before any user story phase.
- User Story 1 is the MVP and should complete before User Story 2 so Ser can reuse the generic ID contract.
- User Story 2 must complete before User Story 3 because persistence and concurrency require a concrete Ser resource.
- User Story 4 should run after User Story 2 and may run before or after User Story 3, but must complete before Polish because it validates DI and extension contracts used by the final documentation.
- Polish tasks run after all user stories are complete.

## Parallel Execution Examples

### User Story 1

```text
T021 and T022 can run in parallel because they create separate test files.
After those tests exist, T023-T026 should run sequentially in the API/application contract path.
```

### User Story 2

```text
T027, T028, and T029 can run in parallel because they create separate test files.
T032, T033, T034, and T035 can run in parallel after the domain model shape is known.
T037-T043 should run in order because service, repository, mapping, controller, DI, and OpenAPI build on each other.
```

### User Story 3

```text
T044 and T045 can run in parallel because they create separate integration test files.
T046 and T047 should run sequentially because they affect the same migrations folder.
```

### User Story 4

```text
T050, T051, and T052 can run in parallel because they create separate architecture test files.
T053-T056 should run in order because abstractions, contracts, validation reuse, and DI modules build on each other.
```

## Implementation Strategy

### MVP First

Complete Phase 1, Phase 2, and Phase 3. This delivers a reusable ID-based endpoint contract that can be tested independently.

### Incremental Delivery

1. Add Ser creation and retrieval with full validation in Phase 4.
2. Add SQL Server persistence and concurrency validation in Phase 5.
3. Add DI, architecture dependency and extensibility validation in Phase 6.
4. Finish documentation and full validation in Phase 7.

### Validation Gates

- Each user story phase must have endpoint tests passing before its implementation is marked complete.
- Every completed task must be marked `[X]` by `/speckit-implement` after the task is actually implemented and validated.
- The final state must pass `dotnet test` and the quickstart scenarios.

## Phase 8: Convergence

- [X] T064 Add SQL Server Testcontainers-backed API test factory path in tests/DarkestDungeon.Api.Tests/Fixtures/ApiTestFactory.cs per T020 and plan: Testcontainers SQL Server testing (partial)
- [X] T065 Add direct PT-BR response mapping tests for EntidadeControllerBase in tests/DarkestDungeon.Api.Tests/EntidadeControllerBaseTests.cs per T022 (missing)
- [X] T066 Add SQL Server CHECK constraints for HP, damage range, percentage fields, size, level and resistances in src/DarkestDungeon.Infrastructure/Data/Migrations/ per T047 and FR-007-FR-021 (partial)
- [X] T067 Reconcile ISerRepository contract location between plan/tasks and implementation, preserving layer dependency rules, in src/DarkestDungeon.Application/Abstractions/ISerRepository.cs and specs/001-base-api-ser/tasks.md per T038/T054 and FR-023 (partial)
