# Implementation Plan: Frontend Interativo de Personagens

**Branch**: `007-frontend-personagens` | **Date**: 2026-09-10 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from [spec.md](spec.md)

## Summary

Entregar uma tela interativa de personagens capaz de consultar, criar e excluir registros persistidos. Como o repositório não possui frontend, a solução adiciona uma interface web estática servida pela mesma aplicação da API e completa os contratos backend de listagem e exclusão necessários para a experiência.

## Technical Context

**Language/Version**: C#/.NET 10 no backend; HTML, CSS e JavaScript modular no frontend

**Primary Dependencies**: ASP.NET Core existente, Entity Framework Core existente e APIs nativas do navegador; nenhum framework frontend novo

**Storage**: SQL Server oficial via infraestrutura existente; InMemory apenas nos testes

**Testing**: xUnit existente para domínio/API/arquitetura; roteiro manual de navegador para a UI e validação responsiva

**Target Platform**: Navegadores modernos em desktop e viewport estreita, com a API hospedada no ambiente atual

**Project Type**: Serviço web .NET com interface web estática integrada

**Performance Goals**: Lista e confirmação de criação visíveis ao usuário em até 3 segundos em condições normais; ações sem duplicidade durante operações em andamento

**Constraints**: PT-BR na interface e erros; SQL Server como persistência oficial; regras de personagem continuam no backend; sem autenticação nova nesta feature; sem dependência frontend adicional sem benefício demonstrável

**Scale/Scope**: Uma tela, três fluxos principais (listar, criar, excluir), catálogo existente de classes/habilidades e uma lista de personagens por sessão

## Constitution Check

Todos os gates passam:

- **Arquitetura em camadas**: a UI chama contratos HTTP; regras e persistência permanecem em Application, Domain e Infrastructure.
- **SQL Server**: nenhuma persistência nova fora do `DarkestDungeonDbContext`; testes podem usar InMemory conforme padrão existente.
- **Contratos verificáveis**: novos `GET /personagens` e `DELETE /personagens/{id}` terão testes de sucesso e erros; criação existente será coberta nos fluxos da tela.
- **PT-BR**: textos visíveis e mensagens da interface serão em Português do Brasil.
- **Operação remota e simplicidade**: mesma origem evita CORS e segundo serviço; configurações sensíveis permanecem fora do código.

**Gate status**: PASS. Não há violação constitucional que exija justificativa.

## Project Structure

### Documentation (this feature)

```text
specs/007-frontend-personagens/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── personagens-api.md
└── tasks.md                 # será criado por /speckit-tasks
```

### Source Code (repository root)

```text
src/
├── DarkestDungeon.Api/
│   ├── Controllers/Catalogo/PersonagensController.cs
│   ├── Contracts/Catalogo/PersonagemContracts.cs
│   └── wwwroot/personagens/
│       ├── index.html
│       ├── personagens.css
│       └── personagens.js
├── DarkestDungeon.Application/
│   ├── Abstractions/IPersonagemService.cs
│   └── Personagens/
│       ├── PersonagemService.cs
│       ├── PersonagemDtos.cs
│       └── Commands/PersonagemCommands.cs
└── DarkestDungeon.Infrastructure/
    └── Repositories/PersonagemRepository.cs

tests/
└── DarkestDungeon.Api.Tests/
    ├── PersonagensEndpointsTests.cs
    └── PersonagensCrudEndpointsTests.cs
```

**Structure Decision**: Manter o backend dividido nos quatro projetos existentes e adicionar a UI estática dentro de `src/DarkestDungeon.Api/wwwroot/personagens`. Os contratos e regras continuam nas camadas atuais; testes de contrato permanecem em `tests/DarkestDungeon.Api.Tests`.

## Phase 0: Research Summary

As decisões e alternativas estão em [research.md](research.md). Os pontos resolvidos foram hospedagem same-origin, ausência de framework frontend, necessidade de novos contratos de listagem/exclusão, DTO específico para lista e validação dupla.

## Phase 1: Design Summary

- Modelo de dados e estados da tela: [data-model.md](data-model.md).
- Contratos HTTP da integração: [contracts/personagens-api.md](contracts/personagens-api.md).
- Guia de execução e validação: [quickstart.md](quickstart.md).

## Implementation Phases

### Phase A: Backend de consulta e ciclo de vida

Adicionar métodos de listar e remover ao repositório/serviço, criar o DTO de resumo, expor `GET /personagens` e `DELETE /personagens/{id}`, e cobrir sucesso e erros com testes de API. Revisar comportamento de exclusão das entidades dependentes para não remover classes, habilidades ou itens compartilhados.

### Phase B: Interface web

Adicionar a página de personagens, estilos responsivos e módulo de interação. Implementar estados de carregamento, vazio, erro, formulário, confirmação de exclusão, bloqueio de submissão duplicada e mensagens PT-BR. Alimentar classe e habilidades pelos catálogos existentes.

### Phase C: Integração e validação

Servir a página pela API em `/personagens/index.html` (mantendo `/personagens` para o JSON da lista), validar o fluxo completo contra SQL Server e executar os testes backend. Repetir o roteiro em viewport estreita e larga, verificar os estados de falha e confirmar que a lista reflete criação/exclusão sem recarga manual.

## Constitution Check (Post-Design)

- **Arquitetura em camadas**: PASS. UI não acessa banco; Application coordena casos de uso e Infrastructure persiste.
- **SQL Server**: PASS. Exclusão e listagem usam o repositório existente e o contexto oficial.
- **Contratos verificáveis**: PASS. O contrato está documentado e possui cenários de sucesso, vazio, validação e inexistência.
- **PT-BR**: PASS. Textos da UI, mensagens e documentação funcional serão em PT-BR.
- **Operação remota e simplicidade**: PASS. A interface usa a mesma origem e nenhuma credencial será adicionada ao frontend.

**Post-design gate status**: PASS.

## Complexity Tracking

Nenhuma violação constitucional ou complexidade excepcional foi identificada.
