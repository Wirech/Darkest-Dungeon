# Implementation Plan: Base de API e Recurso Ser

**Branch**: `001-base-api-ser` | **Date**: 2026-09-07 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/001-base-api-ser/spec.md`

## Summary

Criar a base de uma API backend em camadas para recursos identificáveis por ID,
com persistência em SQL Server, usando `Ser` como primeiro recurso concreto e
entidade genérica para especializações futuras. A entrega planejada inclui
contrato de consulta por ID, cadastro e consulta de Ser, validações de domínio
para ranges de atributos e resistências, respostas em PT-BR, contratos por
interface entre camadas, validação do contêiner de DI e testes de endpoint
cobrindo status e corpo das respostas.

## Technical Context

**Language/Version**: C# 14 com .NET SDK 10.0.400

**Primary Dependencies**: ASP.NET Core 10, Entity Framework Core 10, Microsoft.EntityFrameworkCore.SqlServer, Swashbuckle/OpenAPI, xUnit, Microsoft.AspNetCore.Mvc.Testing, Testcontainers for .NET com SQL Server

**Storage**: SQL Server como banco oficial; EF Core para mapeamento e migrações versionadas

**Testing**: xUnit com WebApplicationFactory para testes de endpoints; Testcontainers SQL Server para validar persistência real em cenários de integração; NetArchTest ou ArchUnitNET para validar dependências entre camadas; teste de inicialização para validar resolução do contêiner de DI

**Target Platform**: Servidor Windows ou Linux com .NET 10, acessível via internet ou VPN

**Project Type**: Web service backend

**Performance Goals**: Suportar pelo menos 20 usuários simultâneos consultando registros existentes sem inconsistência ou duplicação de IDs

**Constraints**: Mensagens destinadas ao usuário em PT-BR; segredos fora do repositório; endpoints stateless quando possível; validação de entrada obrigatória; arquitetura em camadas obrigatória; contratos por interface entre camadas; dependências registradas e validadas pelo contêiner de DI

**Scale/Scope**: Escopo inicial caseiro com um recurso concreto (`Ser` herdável), contrato genérico por ID, atributos de combate com ranges explícitos, resistências, persistência mínima para uso remoto e base extensível para tipos derivados com baixo retrabalho

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Evidence |
|-----------|--------|----------|
| I. Arquitetura em Camadas | PASS | Estrutura planejada separa Api, Application, Domain e Infrastructure. |
| II. SQL Server como Persistência Oficial | PASS | Storage definido como SQL Server com EF Core e migrações versionadas. |
| III. Contratos de Backend Verificáveis | PASS | Testes planejados com WebApplicationFactory cobrindo método, rota, status e corpo. |
| IV. Português do Brasil como Idioma do Produto | PASS | Respostas e mensagens de erro devem ser PT-BR. |
| V. Operação Remota e Simplicidade Proporcional | PASS | API stateless, configuração por ambiente, DI validado e suporte a internet/VPN sem segurança corporativa pesada. |

No gate failures. A arquitetura em quatro projetos é necessária para cumprir a
constituição de camadas sem misturar domínio, aplicação, infraestrutura e API.

## Project Structure

### Documentation (this feature)

```text
specs/001-base-api-ser/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── interfaces.md
│   └── openapi.yaml
└── tasks.md
```

### Source Code (repository root)

```text
DarkestDungeon.sln
src/
├── DarkestDungeon.Api/
│   ├── Controllers/
│   │   ├── EntidadeControllerBase.cs
│   │   └── SeresController.cs
│   ├── Contracts/
│   │   ├── CriarSerRequest.cs
│   │   ├── ErroResponse.cs
│   │   └── SerResponse.cs
│   ├── Extensions/
│   │   ├── ApplicationServiceCollectionExtensions.cs
│   │   ├── InfrastructureServiceCollectionExtensions.cs
│   │   └── ServiceCollectionExtensions.cs
│   ├── Program.cs
│   └── appsettings.json
├── DarkestDungeon.Application/
│   ├── Abstractions/
│   │   ├── IIdentificavelService.cs
│   │   ├── IRepositorioIdentificavel.cs
│   │   └── ISerService.cs
│   ├── Seres/
│   │   ├── CriarSerCommand.cs
│   │   ├── ResistenciasDto.cs
│   │   ├── SerDto.cs
│   │   └── SerService.cs
│   └── Validation/
│       └── ResultadoOperacao.cs
├── DarkestDungeon.Domain/
│   ├── Common/
│   │   └── EntidadeIdentificavel.cs
│   └── Seres/
│       ├── Resistencias.cs
│       └── Ser.cs
└── DarkestDungeon.Infrastructure/
    ├── Data/
    │   ├── DarkestDungeonDbContext.cs
    │   └── Migrations/
    └── Repositories/
        ├── ISerRepository.cs
        └── SerRepository.cs

tests/
├── DarkestDungeon.Api.Tests/
│   ├── Fixtures/
│   │   └── ApiTestFactory.cs
│   └── SeresEndpointsTests.cs
├── DarkestDungeon.Architecture.Tests/
│   ├── DependencyInjectionTests.cs
│   ├── LayerDependencyTests.cs
│   └── SerExtensibilityTests.cs
└── DarkestDungeon.Domain.Tests/
    └── SerTests.cs
```

**Structure Decision**: Usar uma solução .NET com quatro projetos principais:
Api para contratos HTTP, Application para casos de uso, Domain para regras e
entidades, Infrastructure para EF Core/SQL Server. Testes de endpoint ficam em
`tests/DarkestDungeon.Api.Tests` e validam sempre status e retornos. Testes de
arquitetura ficam em `tests/DarkestDungeon.Architecture.Tests` e validam DI,
dependências entre projetos e extensão futura de `Ser`.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| N/A | No constitution gate violation identified. | N/A |

## Phase 0: Research

See [research.md](research.md).

## Phase 1: Design & Contracts

See [data-model.md](data-model.md), [contracts/openapi.yaml](contracts/openapi.yaml), [contracts/interfaces.md](contracts/interfaces.md), and [quickstart.md](quickstart.md).

## Post-Design Constitution Check

| Principle | Status | Evidence |
|-----------|--------|----------|
| Arquitetura em Camadas | PASS | Data model stays in Domain; EF Core mapping stays in Infrastructure; HTTP contract stays in Api. |
| SQL Server | PASS | Research and quickstart require SQL Server; tests use SQL Server-compatible integration path. |
| Testes de Backend | PASS | Quickstart and contract define endpoint tests for success, not found, invalid ID, range validation and persistence failures. |
| PT-BR | PASS | Contract examples and error responses use PT-BR. |
| Operação Remota | PASS | Configuration and validation guide use environment-based connection strings for hosted execution. |
| Evolução e DI | PASS | Design includes interface contracts, centralized service registration, DI resolution tests and architecture tests for layer dependencies. |
