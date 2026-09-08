# Interface Contracts: Base de API e Recurso Ser

Este documento define contratos internos entre camadas para reduzir acoplamento,
facilitar novos tipos derivados de `Ser` e validar resolução por injeção de
dependência.

## Layer Dependency Rules

| Layer | May Reference | Must Not Reference |
|-------|---------------|--------------------|
| DarkestDungeon.Api | Application, Domain contracts used in HTTP mapping | Infrastructure internals beyond registration extensions |
| DarkestDungeon.Application | Domain, application abstractions | Api, concrete Infrastructure implementations |
| DarkestDungeon.Domain | None of the outer layers | Api, Application, Infrastructure |
| DarkestDungeon.Infrastructure | Application abstractions, Domain | Api controllers or HTTP contracts |

## Required Interfaces

### IIdentificavelService

Purpose: provide reusable lookup behavior for resources identifiable by `Guid`.

Required operations:

- Get an identifiable resource by ID.
- Return a result that distinguishes success, invalid ID and not found.
- Avoid leaking persistence-specific errors to API consumers.

### ISerService

Purpose: coordinate Ser creation and lookup use cases.

Required operations:

- Create a Ser from a validated command.
- Retrieve a Ser by ID.
- Return PT-BR-compatible operation errors for validation and persistence failures.

### IRepositorioIdentificavel

Purpose: provide a persistence abstraction for identifiable resources.

Required operations:

- Fetch an entity by `Guid`.
- Persist a new entity with a unique `Guid`.
- Keep database-specific behavior inside Infrastructure.

### ISerRepository

Purpose: provide Ser-specific persistence operations through the Application abstraction boundary.

Required operations:

- Insert a Ser with `Resistencias`.
- Fetch a Ser with `Resistencias` by ID.
- Preserve `DanoBaseMinimo` and `DanoBaseMaximo` as separate persisted values.

## Dependency Injection Registration

The startup composition must resolve these services through the DI container:

- `IIdentificavelService` implementation for identifiable lookup behavior.
- `ISerService` implementation for Ser use cases.
- `IRepositorioIdentificavel` implementation where generic persistence is used.
- `ISerRepository` implementation for Ser persistence.
- `DarkestDungeonDbContext` configured with SQL Server.

## Validation Contract

Architecture tests must verify:

- Domain has 0 references to Api or Infrastructure.
- Application has 0 references to Api or concrete Infrastructure classes.
- DI container resolves the services listed above.
- Adding a future derived type of `Ser` does not require duplicating common HP,
  damage, bounds or resistance validation rules.
