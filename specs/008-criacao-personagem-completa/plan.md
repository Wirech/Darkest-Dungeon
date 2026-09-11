# Implementation Plan: Criação Completa de Personagem

**Branch**: `008-criacao-personagem-completa` | **Date**: 2026-09-10 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from [spec.md](spec.md)

## Summary

Corrigir o fluxo de criação para receber somente nome, classe, nível do herói, nível da arma, nível da armadura e aparência. O backend derivará os equipamentos elegíveis, resistências e associações de habilidades da classe, inicializando quatro habilidades de combate e quatro de acampamento no nível 1 e todas as demais no nível 0. O modelo de `Personagem`, o contrato, a persistência e a interface de criação serão atualizados com compatibilidade para dados antigos.

## Technical Context

**Language/Version**: C#/.NET 10

**Primary Dependencies**: ASP.NET Core, Entity Framework Core, SQL Server, xUnit e FluentAssertions existentes; interface estática existente em HTML/CSS/JavaScript

**Storage**: SQL Server oficial com migração versionada; InMemory nos testes existentes

**Testing**: Testes de domínio, arquitetura e API existentes; testes de contrato HTTP e integração de criação; smoke test da interface

**Target Platform**: API .NET hospedada no ambiente atual, consumida por navegador moderno

**Project Type**: Serviço web em camadas com frontend estático integrado

**Performance Goals**: Criar um personagem e retornar seu estado completo em até 3 segundos em condições normais; nenhuma consulta adicional manual de habilidades/equipamentos necessária para completar o cadastro

**Constraints**: Persistência oficial em SQL Server; regras de negócio fora de controllers; criação sem persistência parcial; mensagens em PT-BR; compatibilidade de leitura de personagens antigos; níveis de equipamento 1..5, nível do herói 0..6 e aparência A..D

**Scale/Scope**: Uma operação de criação, uma entidade `Personagem`, até cinco níveis por item, todas as habilidades de combate/acampamento da classe e três categorias de validação

## Constitution Check

Todos os gates passam:

- **Arquitetura em camadas**: domínio conterá invariantes; Application orquestrará derivação; Infrastructure consultará catálogos e persistirá; API apenas receberá/mapeará contratos; frontend não acessará banco.
- **SQL Server**: novos campos serão adicionados por migração versionada; leitura legada terá defaults compatíveis; nenhuma tabela será manipulada diretamente pelo controller.
- **Contratos verificáveis**: `POST /personagens` e `GET /personagens/{id}` terão testes de sucesso, regra 4+4, níveis, aparência, validações e ausência de persistência parcial.
- **PT-BR**: mensagens de domínio/API e textos da interface permanecerão em Português do Brasil.
- **Operação remota e simplicidade**: o contrato reduz o payload e mantém a mesma origem; não serão introduzidos serviços ou dependências frontend novos.

**Gate status**: PASS. A migração é necessária para corrigir o modelo e está prevista no escopo, sem violação constitucional.

## Project Structure

### Documentation (this feature)

```text
specs/008-criacao-personagem-completa/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── personagens-criacao.md
└── tasks.md                 # será criado por /speckit-tasks
```

### Source Code (repository root)

```text
src/
├── DarkestDungeon.Domain/
│   ├── Seres/Personagem.cs
│   └── Personagens/HabilidadeDePersonagem.cs
├── DarkestDungeon.Application/
│   ├── Abstractions/IPersonagemService.cs
│   ├── Personagens/PersonagemDtos.cs
│   ├── Personagens/PersonagemService.cs
│   ├── Personagens/PersonagemMapper.cs
│   └── Personagens/Commands/PersonagemCommands.cs
├── DarkestDungeon.Api/
│   ├── Contracts/Catalogo/PersonagemContracts.cs
│   ├── Controllers/Catalogo/PersonagensController.cs
│   └── wwwroot/personagens/
│       ├── index.html
│       └── personagens.js
└── DarkestDungeon.Infrastructure/
    ├── Data/DarkestDungeonDbContext.cs
    ├── Migrations/
    └── Repositories/
        ├── PersonagemRepository.cs
        ├── HabilidadeRepository.cs
        └── ItemRepository.cs

tests/
├── DarkestDungeon.Domain.Tests/
│   └── PersonagemCriacaoCompletaTests.cs
└── DarkestDungeon.Api.Tests/
    └── PersonagensCriacaoCompletaTests.cs
```

**Structure Decision**: Preservar os quatro projetos existentes. O domínio recebe os atributos e invariantes do personagem; Application concentra o caso de uso e a seleção 4+4; Infrastructure consulta classes/habilidades/itens e aplica a migração; API ajusta o contrato; a UI existente usa somente as seis escolhas essenciais.

## Phase 0: Research Summary

As decisões estão em [research.md](research.md): contrato mínimo, níveis explícitos no personagem, seleção determinística por `NomeExibicao`/ID, validação antes da gravação, defaults legados e perfil explícito para atributos não escolhidos.

## Phase 1: Design Summary

- Entidades, campos, derivação e estados: [data-model.md](data-model.md).
- Contratos de criação e consulta: [contracts/personagens-criacao.md](contracts/personagens-criacao.md).
- Cenários executáveis: [quickstart.md](quickstart.md).

## Implementation Phases

### Phase A: Modelo e persistência

Adicionar níveis de arma/armadura ao personagem, confirmar aparência persistida, atualizar o mapeamento EF Core e criar migração versionada com defaults compatíveis para dados antigos. Garantir que o detalhe e o mapper exponham os novos campos.

### Phase B: Caso de uso de criação

Substituir o request de criação manual por uma configuração mínima. Validar classe, equipamento elegível, níveis e quantidade de habilidades. Derivar arma/armadura e todas as habilidades da classe, selecionar quatro por categoria de forma determinística e criar as associações com níveis 1/0 em uma operação atômica.

### Phase C: Contrato e interface

Atualizar controller e contratos HTTP, adaptar a tela existente para solicitar somente as seis escolhas e apresentar o resultado completo. Remover dependência da seleção manual de habilidades no fluxo padrão.

### Phase D: Testes e compatibilidade

Adicionar testes de domínio/API para sucesso, 4+4, bloqueios, níveis, aparência, erros de catálogo, ausência de persistência parcial e leitura de dados antigos. Executar a validação completa e o quickstart.

## Constitution Check (Post-Design)

- **Arquitetura em camadas**: PASS. Derivação não será implementada no controller ou no frontend.
- **SQL Server**: PASS. Os novos campos terão migração; InMemory será usado apenas nos testes.
- **Contratos verificáveis**: PASS. O contrato documenta request, retorno e erros, e o plano exige testes para cada comportamento relevante.
- **PT-BR**: PASS. Mensagens e interface permanecem em PT-BR.
- **Operação remota e simplicidade**: PASS. O payload menor simplifica o uso remoto sem adicionar infraestrutura.

**Post-design gate status**: PASS.

## Complexity Tracking

Nenhuma violação constitucional foi identificada. A migração é complexidade necessária para persistir escolhas que hoje não existem no modelo; a alternativa de inferência foi rejeitada por não preservar o estado selecionado pelo usuário.
