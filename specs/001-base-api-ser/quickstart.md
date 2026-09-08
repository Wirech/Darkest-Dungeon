# Quickstart: Base de API e Recurso Ser

Este guia descreve como validar a feature depois da implementação. Ele não
substitui `tasks.md`; serve como roteiro de execução e verificação ponta a
ponta.

## Prerequisites

- .NET SDK 10 instalado.
- Docker ou SQL Server acessível para testes de integração.
- String de conexão fornecida por variável de ambiente, sem segredo versionado.

## Environment

```powershell
$env:ConnectionStrings__DarkestDungeonDb = "Server=localhost;Database=DarkestDungeon;User Id=sa;Password=<senha>;TrustServerCertificate=True"
```

## Expected Setup Commands

```powershell
dotnet restore
dotnet ef database update --project src/DarkestDungeon.Infrastructure --startup-project src/DarkestDungeon.Api
dotnet test
dotnet run --project src/DarkestDungeon.Api
```

## Endpoint Validation Scenarios

### Create Ser

Request:

```http
POST /seres HTTP/1.1
Content-Type: application/json

{
  "nome": "Cruzado",
  "tipo": "Herói",
  "hpMaximo": 33,
  "hpAtual": 33,
  "velocidade": 1,
  "critico": 5,
  "danoBaseMinimo": 7,
  "danoBaseMaximo": 13,
  "movimento": 2,
  "bonusDeCritico": 0,
  "tamanho": 1,
  "acoesPorTurno": 1,
  "esquiva": 5,
  "precisao": 85,
  "protecao": 0,
  "nivel": 0,
  "resistencias": {
    "atordoamento": 40,
    "sangramento": 30,
    "envenenamento": 20,
    "debuff": 25,
    "movimento": 35
  }
}
```

Expected:

- Status `201 Created`.
- Response includes `id` as UUID.
- Response echoes all Ser attributes, damage range and resistências in PT-BR-compatible JSON field names.
- `Location` header points to `/seres/{id}`.

### Get Existing Ser

Request:

```http
GET /seres/{id} HTTP/1.1
```

Expected:

- Status `200 OK`.
- Response `id` equals requested ID.
- Response includes Nome, Tipo, HP, damage range, combate, defesa, mobilidade, turno and resistências attributes.

### Get Missing Ser

Request:

```http
GET /seres/00000000-0000-0000-0000-000000000000 HTTP/1.1
```

Expected:

- Status `404 Not Found`.
- Response body includes `mensagem: "Ser não encontrado."`.

### Reject Invalid Ser

Request with `hpAtual` greater than `hpMaximo`:

```http
POST /seres HTTP/1.1
Content-Type: application/json

{
  "nome": "Cultista",
  "tipo": "Inimigo",
  "hpMaximo": 10,
  "hpAtual": 20,
  "velocidade": 2,
  "critico": 1,
  "danoBaseMinimo": 3,
  "danoBaseMaximo": 5,
  "movimento": 1,
  "bonusDeCritico": 0,
  "tamanho": 1,
  "acoesPorTurno": 1,
  "esquiva": 0,
  "precisao": 75,
  "protecao": 0,
  "nivel": 0,
  "resistencias": {
    "atordoamento": 10,
    "sangramento": 10,
    "envenenamento": 10,
    "debuff": 10,
    "movimento": 10
  }
}
```

Expected:

- Status `400 Bad Request`.
- Response body includes a PT-BR validation message for `hpAtual`.
- No partial Ser is persisted.

### Reject Invalid Damage Range

Request with `danoBaseMinimo` greater than `danoBaseMaximo`:

```http
POST /seres HTTP/1.1
Content-Type: application/json

{
  "nome": "Brigante",
  "tipo": "Inimigo",
  "hpMaximo": 12,
  "hpAtual": 12,
  "velocidade": 4,
  "critico": 3,
  "danoBaseMinimo": 8,
  "danoBaseMaximo": 4,
  "movimento": 2,
  "bonusDeCritico": 0,
  "tamanho": 1,
  "acoesPorTurno": 1,
  "esquiva": 5,
  "precisao": 80,
  "protecao": 0,
  "nivel": 1,
  "resistencias": {
    "atordoamento": 20,
    "sangramento": 20,
    "envenenamento": 20,
    "debuff": 20,
    "movimento": 20
  }
}
```

Expected:

- Status `400 Bad Request`.
- Response body includes a PT-BR validation message for `danoBaseMinimo`.
- No partial Ser is persisted.

### Reject Resistance Out Of Range

Request with a resistance greater than `100`:

```http
POST /seres HTTP/1.1
Content-Type: application/json

{
  "nome": "Guardião",
  "tipo": "Chefe",
  "hpMaximo": 80,
  "hpAtual": 80,
  "velocidade": 2,
  "critico": 10,
  "danoBaseMinimo": 11,
  "danoBaseMaximo": 17,
  "movimento": 1,
  "bonusDeCritico": 15,
  "tamanho": 4,
  "acoesPorTurno": 2,
  "esquiva": 10,
  "precisao": 95,
  "protecao": 40,
  "nivel": 6,
  "resistencias": {
    "atordoamento": 120,
    "sangramento": 80,
    "envenenamento": 80,
    "debuff": 70,
    "movimento": 90
  }
}
```

Expected:

- Status `400 Bad Request`.
- Response body includes a PT-BR validation message for `resistencias.atordoamento`.
- No partial Ser is persisted.

## Persistence Validation

1. Create a Ser and record its ID.
2. Stop and start the API.
3. Query `GET /seres/{id}`.
4. Confirm status `200 OK` and unchanged ID/attributes.

## Concurrency Validation

1. Seed at least one Ser.
2. Run 20 simultaneous GET requests to `/seres/{id}`.
3. Confirm all successful responses contain the same ID and no inconsistent body.

## Dependency Injection Validation

1. Run the architecture test suite.
2. Confirm the DI container resolves the main application services, repositories and SQL Server DbContext.
3. Confirm failures in service registration fail the validation before the API is used.

Expected:

- `IIdentificavelService`, `ISerService`, `IRepositorioIdentificavel`, `ISerRepository` and `DarkestDungeonDbContext` resolve successfully.
- DI registration remains split by layer registration methods.

## Architecture Validation

1. Run the architecture test suite.
2. Confirm Domain has 0 references to Api or Infrastructure.
3. Confirm Application depends on abstractions instead of concrete Infrastructure implementations.
4. Confirm a future derived type of Ser can reuse base validation rules without duplicating HP, damage, bounds or resistance validation.

Expected:

- Layer dependency tests pass.
- DI validation tests pass.
- Ser extensibility tests pass.

## Validation Results

- 2026-09-07: `dotnet build DarkestDungeon.sln --no-restore` passou.
- 2026-09-07: suíte completa de testes passou com 20 testes aprovados e 0 falhas.
- 2026-09-07: migração inicial `InitialSerSchema` gerada em `src/DarkestDungeon.Infrastructure/Data/Migrations/`.
- 2026-09-07: convergência implementada; suíte completa passou com 22 testes aprovados e 0 falhas.
- 2026-09-07: migração inicial regenerada com CHECK constraints para HP, dano, percentuais, tamanho, nível e resistências.
