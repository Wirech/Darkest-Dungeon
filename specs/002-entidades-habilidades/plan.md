# Implementation Plan: Entidades e Catálogo de Habilidades

**Branch**: `002-entidades-habilidades` | **Date**: 2026-09-07 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/002-entidades-habilidades/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

Expandir a entidade `Ser` com `Personagem` e `Inimigo`, criar `Item` e transformar
`Habilidade` em entidade independente, com catálogo por categoria e disponibilidade.
Personagens recebem automaticamente a configuração de sua classe; Inimigos podem
ter quantidade arbitrária de habilidades e resistências próprias somadas à base do
tipo. A implementação seguirá o padrão existente de serviços, DTOs, repositórios,
controllers e testes, usando herança TPT no EF Core e associações relacionais.

## Technical Context

<!--
  ACTION REQUIRED: Replace the content in this section with the technical details
  for the project. The structure here is presented in advisory capacity to guide
  the iteration process.
-->

**Language/Version**: C# com .NET 10

**Primary Dependencies**: ASP.NET Core, Entity Framework Core 10, SQL Server, Swashbuckle, xUnit, FluentAssertions, NetArchTest

**Storage**: SQL Server; InMemory somente nos testes de API existentes

**Testing**: `dotnet test`, testes de domínio, contrato de API, persistência, concorrência e arquitetura

**Target Platform**: serviço web ASP.NET Core hospedado pelo responsável, acessível por internet ou VPN

**Project Type**: web-service backend em arquitetura de quatro camadas

**Performance Goals**: manter o comportamento atual para pelo menos 20 consultas simultâneas; listagem inicial de habilidades sem paginação, pois o catálogo inicial é pequeno

**Constraints**: mensagens e contratos textuais em PT-BR; persistência SQL Server; sem segredos versionados; criação de Personagem atômica; regras de domínio fora da API

**Scale/Scope**: 20 classes de Personagem, 7 tipos de Inimigo, até 6 habilidades padrão por categoria de classe, 3 habilidades de Acampamento equipadas por aventura e quantidade arbitrária de habilidades de Inimigo

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Arquitetura em camadas**: PASS. Domínio permanece independente; contratos ficam em Application; API apenas mapeia HTTP; Infrastructure concentra EF Core.
- **SQL Server e persistência isolada**: PASS. Novas tabelas e relações serão configuradas no `DarkestDungeonDbContext`, com migration versionada e configuração por ambiente.
- **Contratos verificáveis**: PASS. Cada novo controller terá testes de sucesso, validação, não encontrado, regras de associação e formato de resposta.
- **PT-BR**: PASS. Requests, responses e mensagens voltadas ao usuário seguirão os nomes e mensagens do contrato existente.
- **Operação remota e simplicidade**: PASS. A feature não adiciona autenticação ou infraestrutura nova; configurações continuam externas ao código.
- **DI e interfaces**: PASS. Repositórios e serviços serão registrados pelas extensões existentes e cobertos por testes de resolução.

## Project Structure

### Documentation (this feature)

```text
specs/002-entidades-habilidades/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md        # Phase 1 output (/speckit-plan command)
├── quickstart.md        # Phase 1 output (/speckit-plan command)
├── contracts/           # Phase 1 output (/speckit-plan command)
└── tasks.md             # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

### Source Code (repository root)

```text
src/
├── DarkestDungeon.Domain/
│   ├── Common/
│   ├── Seres/                 # Personagem, Inimigo e regras comuns
│   ├── Habilidades/           # Habilidade e vínculos de domínio
│   └── Itens/                 # Item e contratos de equipamento
├── DarkestDungeon.Application/
│   ├── Abstractions/          # repositórios e serviços
│   ├── Habilidades/
│   ├── Personagens/
│   ├── Inimigos/
│   └── Itens/
├── DarkestDungeon.Infrastructure/
│   ├── Data/                  # DbContext, configurações e migration
│   └── Repositories/
└── DarkestDungeon.Api/
  ├── Controllers/
  ├── Contracts/
  └── Extensions/

tests/
├── DarkestDungeon.Domain.Tests/
├── DarkestDungeon.Api.Tests/
└── DarkestDungeon.Architecture.Tests/
```

**Structure Decision**: Manter a solução atual em quatro projetos e expandir cada
camada por área de domínio. `Ser` continua sendo a base; `Personagem` e `Inimigo`
serão persistidos com TPT. `Habilidade`, `Item` e configurações de classe serão
entidades independentes relacionadas por tabelas de associação, evitando listas
serializadas e mantendo integridade referencial no SQL Server.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Nenhuma | N/A | A solução usa os quatro projetos já existentes e os padrões já adotados. |

## Phase 0: Research Summary

- Usar TPT para `Ser`/`Personagem`/`Inimigo`, permitindo novos derivados sem inflar a tabela base.
- Usar tabelas de associação para habilidades de classe, habilidades de Personagem e habilidades de Inimigo.
- Copiar as habilidades da configuração de classe para o Personagem no momento da criação; alterações futuras da classe não alteram personagens existentes.
- Representar estados de habilidade de Personagem em entidade de vínculo, incluindo `Habilitada`, `Treinada` e `Equipada` conforme a categoria.
- Manter efeitos detalhados de Habilidade e atributos específicos avançados de Item fora desta fase.

## Phase 1: Design Notes

- Resistências base de Tipo de Inimigo serão um catálogo estático com 5 para cada resistência; resistência própria do Inimigo será validada entre 0 e 100 e somada sem limitar o total.
- A criação de Personagem validará classe, configuração e disponibilidade das habilidades antes de persistir Ser, derivado e vínculos na mesma unidade de trabalho.
- Aflição e Virtude serão campos opcionais mutuamente exclusivos e persistentes; Stress e flags de sobrevivência serão propriedades do Personagem.
- Armadura, Arma e Acessório referenciarão `Item` ou derivado de Item; o contrato mínimo de Item será nome, descrição e identificador.

## Constitution Check: Post-Design

- **Arquitetura em camadas**: PASS. O modelo não exige referências novas do Domain para camadas externas; contratos internos estão em `contracts/interfaces.md`.
- **Persistência SQL Server**: PASS. TPT e tabelas de vínculo são compatíveis com SQL Server e serão entregues por migration versionada.
- **Contratos verificáveis**: PASS. `contracts/openapi.yaml` define rotas, requests, responses e erros; o quickstart lista os cenários de sucesso e falha.
- **PT-BR e operação**: PASS. Os nomes públicos e mensagens permanecem em PT-BR, sem introduzir segredos ou infraestrutura adicional.
- **DI**: PASS. Os serviços e repositórios têm contratos explícitos e devem ser resolvidos pelos registros existentes de composição.
