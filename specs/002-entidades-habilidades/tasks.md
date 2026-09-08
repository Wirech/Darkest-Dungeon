---

### description: "Task list for Entidades e Catálogo de Habilidades"
---

# Tasks: Entidades e Catálogo de Habilidades

**Input**: Design documents from `/specs/002-entidades-habilidades/`

**Prerequisites**: [plan.md](plan.md), [spec.md](spec.md), [research.md](research.md), [data-model.md](data-model.md), [contracts/](contracts/)

**Organization**: Tasks are grouped by user story so each story can be implemented and validated independently after the foundational phase.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Preparar a solução existente para a expansão sem alterar o contrato de Ser.

- [ ] T001 [P] Confirmar o baseline compilável executando `dotnet build DarkestDungeon.slnx` e registrar o resultado em `specs/002-entidades-habilidades/quickstart.md`
- [ ] T002 [P] Mapear os registros atuais de DI e pontos de extensão em `src/DarkestDungeon.Api/Extensions/ApplicationServiceCollectionExtensions.cs` e `src/DarkestDungeon.Api/Extensions/InfrastructureServiceCollectionExtensions.cs`
- [ ] T003 [P] Criar as pastas de domínio, aplicação, infraestrutura, API e testes previstas em `specs/002-entidades-habilidades/plan.md`
- [ ] T004 [P] Atualizar a documentação de endpoints da feature em `specs/002-entidades-habilidades/contracts/openapi.yaml` somente quando um contrato implementado divergir do desenho planejado

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Construir as abstrações e persistência compartilhadas que bloqueiam todas as histórias.

- [ ] T005 [P] Criar enums e catálogos de domínio para `CategoriaHabilidade`, `DisponibilidadeHabilidade`, `ClassePersonagem` e `TipoInimigo` em `src/DarkestDungeon.Domain/Habilidades` e `src/DarkestDungeon.Domain/Seres`
- [ ] T006 [P] Criar a entidade independente `Habilidade` com nome, descrição, categoria, disponibilidade e validações em `src/DarkestDungeon.Domain/Habilidades/Habilidade.cs`
- [ ] T007 [P] Criar a entidade independente `Item` com nome, descrição e base para herdeiros em `src/DarkestDungeon.Domain/Itens/Item.cs`
- [ ] T008 Criar `Personagem` e `Inimigo` como derivados de `Ser`, reutilizando validações e identificador em `src/DarkestDungeon.Domain/Seres/Personagem.cs` e `src/DarkestDungeon.Domain/Seres/Inimigo.cs`
- [ ] T009 Criar value objects e vínculos de domínio para habilidades de Personagem, habilidades de Inimigo, configuração de classe, individualidades, doenças e inventário em `src/DarkestDungeon.Domain/Habilidades` e `src/DarkestDungeon.Domain/Seres`
- [ ] T010 Criar regras de domínio para Stress, Chance de Virtude, Aflição/Virtude mutuamente exclusivas, flags de sobrevivência, treinamento e equipamento de Acampamento em `src/DarkestDungeon.Domain/Seres/Personagem.cs`
- [ ] T011 Criar regras de domínio para resistência própria, soma com base 5 do tipo e quantidade arbitrária de habilidades em `src/DarkestDungeon.Domain/Seres/Inimigo.cs`
- [ ] T012 [P] Criar interfaces de repositório para Habilidade, Item, configuração de classe, Personagem e Inimigo em `src/DarkestDungeon.Application/Abstractions`
- [ ] T013 [P] Criar contratos de serviço, comandos, DTOs e `ResultadoOperacao` específico para as novas entidades em `src/DarkestDungeon.Application/Habilidades`, `src/DarkestDungeon.Application/Personagens`, `src/DarkestDungeon.Application/Inimigos` e `src/DarkestDungeon.Application/Itens`
- [ ] T014 Configurar o `DarkestDungeonDbContext` com TPT para Ser/Personagem/Inimigo, entidades Item/Habilidade e tabelas relacionais de vínculos em `src/DarkestDungeon.Infrastructure/Data/DarkestDungeonDbContext.cs`
- [ ] T015 Configurar chaves, índices, unicidade, limites, relacionamentos e constraints SQL Server para a feature em `src/DarkestDungeon.Infrastructure/Data/Configurations`
- [X] T016 Criar migration versionada para as novas tabelas e relacionamentos em `src/DarkestDungeon.Infrastructure/Migrations`
- [ ] T017 [P] Implementar repositórios de Habilidade, Item, configuração, Personagem e Inimigo em `src/DarkestDungeon.Infrastructure/Repositories`
- [X] T018 Registrar os novos repositórios e serviços nas extensões de DI em `src/DarkestDungeon.Api/Extensions/ApplicationServiceCollectionExtensions.cs` e `src/DarkestDungeon.Api/Extensions/InfrastructureServiceCollectionExtensions.cs`
- [ ] T019 [P] Adicionar testes de arquitetura para impedir dependências indevidas e testar resolução dos serviços da feature em `tests/DarkestDungeon.Architecture.Tests/LayerDependencyTests.cs` e `tests/DarkestDungeon.Architecture.Tests/DependencyResolutionTests.cs`

**Checkpoint**: A base de domínio, persistência, DI e contratos está pronta; as histórias podem avançar por prioridade ou em paralelo onde os arquivos não conflitem.

## Phase 3: User Story 1 - Consultar o catálogo de habilidades (Priority: P1) 🎯 MVP

**Goal**: Cadastrar, consultar e listar Habilidades por categoria e disponibilidade.

**Independent Test**: Criar Habilidades de Combate e Acampamento, consultar a lista completa e filtros; confirmar `400` para nome, categoria ou disponibilidade inválidos.

### Tests for User Story 1

- [ ] T020 [P] [US1] Criar testes de domínio para validade de nome, categoria e disponibilidade em `tests/DarkestDungeon.Domain.Tests/HabilidadeTests.cs`
- [ ] T021 [P] [US1] Criar testes de contrato para `POST /habilidades` e `GET /habilidades` em `tests/DarkestDungeon.Api.Tests/HabilidadesEndpointsTests.cs`
- [ ] T022 [P] [US1] Criar testes de filtro e não encontrado para `GET /habilidades/{id}` em `tests/DarkestDungeon.Api.Tests/HabilidadesQueryTests.cs`

### Implementation for User Story 1

- [ ] T023 [P] [US1] Implementar `HabilidadeService`, comandos, DTOs e mapeamentos manuais em `src/DarkestDungeon.Application/Habilidades`
- [ ] T024 [US1] Implementar `HabilidadesController` com POST, GET por ID e listagem filtrável em `src/DarkestDungeon.Api/Controllers/HabilidadesController.cs`
- [ ] T025 [US1] Criar requests e responses PT-BR para Habilidade em `src/DarkestDungeon.Api/Contracts/HabilidadeContracts.cs`
- [ ] T026 [US1] Implementar filtros por categoria e disponibilidade no repositório em `src/DarkestDungeon.Infrastructure/Repositories/HabilidadeRepository.cs`
- [ ] T027 [US1] Executar os testes de US1 e confirmar persistência sem registro parcial em `tests/DarkestDungeon.Api.Tests/HabilidadesPersistenceTests.cs`

**Checkpoint**: O catálogo global de Habilidades está utilizável sem depender de Personagem, Inimigo ou Item.

## Phase 4: User Story 2 - Configurar habilidades por classe de personagem (Priority: P2)

**Goal**: Manter até seis habilidades padrão por categoria para cada classe, rejeitando duplicidades e habilidades inacessíveis a Personagem.

**Independent Test**: Configurar uma classe, consultar sua configuração, ultrapassar limites e tentar incluir habilidade exclusiva de Inimigo.

### Tests for User Story 2

- [ ] T028 [P] [US2] Criar testes de domínio para os 20 valores de `ClassePersonagem`, limites, duplicidade e disponibilidade em `tests/DarkestDungeon.Domain.Tests/ConfiguracaoClassePersonagemTests.cs`
- [ ] T029 [P] [US2] Criar testes de contrato para `GET/PUT /classes-personagens/{classe}/habilidades` em `tests/DarkestDungeon.Api.Tests/ConfiguracaoClasseEndpointsTests.cs`
- [ ] T030 [P] [US2] Criar teste de persistência para substituição atômica da configuração em `tests/DarkestDungeon.Api.Tests/ConfiguracaoClassePersistenceTests.cs`

### Implementation for User Story 2

- [ ] T031 [P] [US2] Implementar `ConfiguracaoClassePersonagemService` e comandos de substituição em `src/DarkestDungeon.Application/Personagens/ConfiguracaoClassePersonagemService.cs`
- [ ] T032 [US2] Implementar repositório de configuração com carregamento das Habilidades relacionadas em `src/DarkestDungeon.Infrastructure/Repositories/ConfiguracaoClassePersonagemRepository.cs`
- [ ] T033 [US2] Implementar `ClassesPersonagensController` com GET e PUT em `src/DarkestDungeon.Api/Controllers/ClassesPersonagensController.cs`
- [ ] T034 [US2] Criar requests e responses da configuração de classe em `src/DarkestDungeon.Api/Contracts/ConfiguracaoClasseContracts.cs`
- [ ] T035 [US2] Garantir que a substituição da configuração não altere vínculos de Personagens já persistidos em `src/DarkestDungeon.Application/Personagens/ConfiguracaoClassePersonagemService.cs`

**Checkpoint**: Cada classe possui uma configuração consultável e validada independentemente da criação de Personagens.

## Phase 5: User Story 3 - Criar personagem com habilidades da classe (Priority: P3)

**Goal**: Criar e consultar Personagem com atributos próprios, habilidades copiadas da classe e estados iniciais corretos.

**Independent Test**: Criar Personagem para uma classe configurada; conferir Ser, até seis Combate, seis Acampamento não treinadas/não equipadas, Stress, Chance de Virtude, equipamento, inventário e estados psicológicos.

### Tests for User Story 3

- [ ] T036 [P] [US3] Criar testes de domínio para Personagem, Stress, Chance de Virtude, Aflição/Virtude e flags em `tests/DarkestDungeon.Domain.Tests/PersonagemTests.cs`
- [ ] T037 [P] [US3] Criar testes de domínio para treinamento, equipamento e limite de três habilidades de Acampamento em `tests/DarkestDungeon.Domain.Tests/PersonagemHabilidadesTests.cs`
- [ ] T038 [P] [US3] Criar testes de contrato para `POST/GET /personagens` e atribuição automática em `tests/DarkestDungeon.Api.Tests/PersonagensEndpointsTests.cs`
- [ ] T039 [P] [US3] Criar testes de persistência e atomicidade de Personagem com vínculos em `tests/DarkestDungeon.Api.Tests/PersonagensPersistenceTests.cs`

### Implementation for User Story 3

- [ ] T040 [P] [US3] Implementar comandos, DTOs e `PersonagemService` com cópia da configuração vigente em `src/DarkestDungeon.Application/Personagens`
- [ ] T041 [US3] Implementar criação atômica e consulta agregada de Personagem em `src/DarkestDungeon.Infrastructure/Repositories/PersonagemRepository.cs`
- [ ] T042 [US3] Implementar `PersonagensController` com POST e GET em `src/DarkestDungeon.Api/Controllers/PersonagensController.cs`
- [ ] T043 [US3] Criar requests e responses de Personagem, incluindo habilidades, estados, equipamentos e inventário em `src/DarkestDungeon.Api/Contracts/PersonagemContracts.cs`
- [ ] T044 [US3] Implementar operações de treinamento e equipamento de Acampamento com validação de três equipadas em `src/DarkestDungeon.Application/Personagens/PersonagemService.cs`
- [ ] T045 [US3] Implementar mapeamento de Armadura, Arma, Acessório e quatro espaços de Inventário para Item em `src/DarkestDungeon.Application/Personagens` e `src/DarkestDungeon.Infrastructure/Data`

**Checkpoint**: Personagem pode ser criado e consultado com todas as regras próprias sem modificar o contrato base de Ser.

## Phase 6: User Story 4 - Criar inimigo com habilidades próprias (Priority: P4)

**Goal**: Criar e consultar Inimigo com tipo controlado, resistências base + próprias e número arbitrário de Habilidades.

**Independent Test**: Criar Inimigos com zero, uma e várias habilidades; confirmar tipo válido, exclusividade de Habilidade e resistência efetiva base 5 + própria.

### Tests for User Story 4

- [ ] T046 [P] [US4] Criar testes de domínio para tipos de Inimigo, resistência própria e cálculo efetivo em `tests/DarkestDungeon.Domain.Tests/InimigoTests.cs`
- [ ] T047 [P] [US4] Criar testes de contrato para `POST/GET /inimigos` em `tests/DarkestDungeon.Api.Tests/InimigosEndpointsTests.cs`
- [ ] T048 [P] [US4] Criar teste de persistência para zero e quantidade arbitrária de Habilidades em `tests/DarkestDungeon.Api.Tests/InimigosPersistenceTests.cs`

### Implementation for User Story 4

- [ ] T049 [P] [US4] Implementar `InimigoService`, comandos, DTOs e cálculo de resistências efetivas em `src/DarkestDungeon.Application/Inimigos`
- [ ] T050 [US4] Implementar criação e consulta de Inimigo com vínculos de Habilidade em `src/DarkestDungeon.Infrastructure/Repositories/InimigoRepository.cs`
- [ ] T051 [US4] Implementar `InimigosController` com POST e GET em `src/DarkestDungeon.Api/Controllers/InimigosController.cs`
- [ ] T052 [US4] Criar requests e responses de Inimigo em `src/DarkestDungeon.Api/Contracts/InimigoContracts.cs`
- [ ] T053 [US4] Garantir que Habilidades exclusivas de Inimigo não sejam aceitas na configuração de classe ou Personagem em `src/DarkestDungeon.Application/Inimigos` e `src/DarkestDungeon.Application/Personagens`

**Checkpoint**: Inimigo pode usar qualquer quantidade de Habilidades válidas sem herdar o limite de Personagem.

## Phase 7: User Story 5 - Criar e consultar Item (Priority: P5)

**Goal**: Criar e consultar Item identificável e disponibilizá-lo para equipamentos e Inventário futuros.

**Independent Test**: Criar Item válido, consultar pelo ID, rejeitar nome vazio e confirmar que Item não recebe atributos de Ser indevidamente.

### Tests for User Story 5

- [ ] T054 [P] [US5] Criar testes de domínio para nome e criação de Item em `tests/DarkestDungeon.Domain.Tests/ItemTests.cs`
- [ ] T055 [P] [US5] Criar testes de contrato para `POST/GET /itens` em `tests/DarkestDungeon.Api.Tests/ItensEndpointsTests.cs`
- [ ] T056 [P] [US5] Criar teste de persistência e consulta de Item em `tests/DarkestDungeon.Api.Tests/ItensPersistenceTests.cs`

### Implementation for User Story 5

- [ ] T057 [P] [US5] Implementar `ItemService`, comandos, DTOs e mapeamentos em `src/DarkestDungeon.Application/Itens`
- [ ] T058 [US5] Implementar persistência de Item e herdeiros em `src/DarkestDungeon.Infrastructure/Repositories/ItemRepository.cs`
- [ ] T059 [US5] Implementar `ItensController` com POST e GET em `src/DarkestDungeon.Api/Controllers/ItensController.cs`
- [ ] T060 [US5] Criar requests e responses de Item em `src/DarkestDungeon.Api/Contracts/ItemContracts.cs`

**Checkpoint**: Item está disponível como recurso independente e pode ser usado pelos vínculos de Personagem.

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Consolidar documentação, testes e validação ponta a ponta.

- [ ] T061 [P] Atualizar `specs/002-entidades-habilidades/contracts/openapi.yaml` para refletir o contrato implementado e exemplos PT-BR reais
- [ ] T062 [P] Atualizar `specs/002-entidades-habilidades/contracts/interfaces.md` com assinaturas finais e decisões de DI implementadas
- [ ] T063 [P] Atualizar `docs/sdd-comandos.md` com o fluxo de implementação e validação da feature
- [ ] T064 [P] Adicionar testes de regressão para o contrato existente de Ser em `tests/DarkestDungeon.Api.Tests/SeresEndpointsTests.cs` e `tests/DarkestDungeon.Api.Tests/SeresValidationTests.cs`
- [ ] T065 [P] Adicionar teste de concorrência com pelo menos 20 consultas simultâneas aos novos recursos em `tests/DarkestDungeon.Api.Tests/FeatureConcurrencyTests.cs`
- [ ] T066 Executar `dotnet test` para toda a solução e corrigir somente falhas relacionadas à feature em `tests`
- [ ] T067 Executar os cenários de `specs/002-entidades-habilidades/quickstart.md` com SQL Server e confirmar migration, persistência e reinício
- [ ] T068 Revisar mensagens, nomes públicos, limites e respostas para conformidade com PT-BR e a constituição em todos os projetos afetados

## Dependencies & Execution Order

### Phase Dependencies

- Setup (Phase 1) não depende de outras fases.
- Foundational (Phase 2) depende do Setup e bloqueia todas as histórias.
- US1 depende da entidade Habilidade, repositório e migration da Phase 2.
- US2 depende de US1 para referenciar Habilidades cadastradas.
- US3 depende de US1 e US2 para copiar o catálogo da classe.
- US4 depende de US1 para referenciar Habilidades, mas pode ser desenvolvido em paralelo com US2 após a Phase 2.
- US5 depende apenas da base da Phase 2 e pode ser desenvolvido em paralelo com US1-US4 após a migration.
- Polish depende das histórias selecionadas para entrega.

### User Story Dependencies

- **US1 (P1)**: independente após Foundational; MVP recomendado.
- **US2 (P2)**: depende de US1 para associar Habilidades existentes.
- **US3 (P3)**: depende de US1 e US2 para atribuição automática.
- **US4 (P4)**: depende de US1; independente de US2 e US3.
- **US5 (P5)**: independente após Foundational; integra com US3 quando equipamentos forem usados.

### Within Each User Story

- Testes de domínio e contrato devem ser criados antes da implementação correspondente e inicialmente falhar.
- Modelos e vínculos devem existir antes de serviços.
- Serviços devem existir antes de controllers.
- Persistência e mapeamento devem ser validados antes do checkpoint da história.

## Parallel Opportunities

- Setup T001-T004 pode ser distribuído por arquivos diferentes.
- Foundational T005-T007, T012-T013 e T017-T019 são paralelizáveis quando não houver conflito no DbContext.
- US1: T020-T022 podem ser escritos em paralelo; T023 e T026 podem avançar em arquivos diferentes antes do controller T024.
- US2: T028-T030 são paralelizáveis; T031, T032 e T034 podem ser distribuídos por camadas.
- US3: T036-T039 são paralelizáveis; T040, T041 e T043 podem avançar em camadas diferentes.
- US4: T046-T048 são paralelizáveis; T049, T050 e T052 podem avançar em camadas diferentes.
- US5: T054-T056 são paralelizáveis; T057, T058 e T060 podem avançar em camadas diferentes.
- Depois da Phase 2, US4 e US5 podem ser desenvolvidas em paralelo com US1; US2 e US3 seguem a dependência indicada.

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Concluir Setup e Foundational.
2. Implementar US1: cadastro, consulta e filtros de Habilidade.
3. Executar os testes de US1 e validar `quickstart.md` parcialmente.
4. Parar para demonstrar o catálogo antes de avançar para associações.

### Incremental Delivery

1. Foundation + US1: catálogo de Habilidades.
2. US2: configurações por classe.
3. US3: Personagem e atribuição automática.
4. US4: Inimigos e resistências.
5. US5: Item e integração com equipamentos.
6. Polish: contratos, regressão, concorrência, migration e quickstart completo.

### Parallel Team Strategy

1. Uma pessoa conclui Setup + Foundational e estabiliza o DbContext/migration.
2. Depois da Foundation: equipe A implementa US1, equipe B implementa US4, equipe C implementa US5.
3. Após US1: equipe A ou B implementa US2.
4. Após US2: equipe responsável implementa US3.

## Notes

- Todas as tarefas seguem o formato obrigatório `- [ ] T### [P?] [US?] descrição com caminho`.
- O marcador `[P]` só aparece em tarefas de arquivos ou áreas que podem ser trabalhadas sem depender de outra tarefa incompleta.
- Nenhuma tarefa cria efeitos detalhados de Habilidade ou atributos avançados de Item; esses itens permanecem fora do escopo desta feature.

## Phase 9: Convergence

**Purpose**: Fechar as lacunas encontradas entre a implementação atual, a especificação, o plano e a constituição.

- [ ] T069 Implementar mapeamento EF Core TPT completo para Ser, Personagem e Inimigo, incluindo tabelas de vínculos de habilidades, equipamentos, inventário e constraints em `src/DarkestDungeon.Infrastructure/Data/DarkestDungeonDbContext.cs` e `src/DarkestDungeon.Infrastructure/Data/Configurations` (CRITICAL; Constitution II; F1; partial)
- [X] T070 Criar migration versionada para Habilidade, Item, ConfiguraçãoClassePersonagem, Personagem, Inimigo e todas as associações em `src/DarkestDungeon.Infrastructure/Migrations` (CRITICAL; Constitution II; T016; F1; missing)
- [ ] T071 Remover o uso de `NotMapped` nos vínculos persistentes e implementar entidades de associação com estados Habilitada, Treinada e Equipada em `src/DarkestDungeon.Domain/Habilidades`, `src/DarkestDungeon.Domain/Seres` e `src/DarkestDungeon.Infrastructure/Data` (HIGH; FR-021; F3; partial)
- [X] T072 Implementar `InimigoService`, `InimigoRepository`, `InimigosController` e contratos POST/GET com quantidade arbitrária de habilidades e resistências efetivas em `src/DarkestDungeon.Application/Inimigos`, `src/DarkestDungeon.Infrastructure/Repositories/InimigoRepository.cs`, `src/DarkestDungeon.Api/Controllers/InimigosController.cs` e `src/DarkestDungeon.Api/Contracts/InimigoContracts.cs` (HIGH; US4; FR-014–FR-017; F4; missing)
- [ ] T073 Completar Personagem com persistência e endpoints para treinamento/equipamento de Acampamento, limite de três equipadas, equipamentos, inventário, flags de sobrevivência e Aflição/Virtude em `src/DarkestDungeon.Application/Personagens`, `src/DarkestDungeon.Infrastructure/Repositories/PersonagemRepository.cs`, `src/DarkestDungeon.Api/Controllers/PersonagensController.cs` e `src/DarkestDungeon.Api/Contracts/PersonagemContracts.cs` (HIGH; US3; FR-012–FR-012m; F5; partial)
- [ ] T074 Corrigir carregamento e substituição atômica das habilidades de `ConfiguracaoClassePersonagem`, preservando associações após reinício e sem alterar fotografias de Personagens existentes em `src/DarkestDungeon.Infrastructure/Repositories/ConfiguracaoClassePersonagemRepository.cs` e `src/DarkestDungeon.Application/Personagens/ConfiguracaoClassePersonagemService.cs` (HIGH; FR-006–FR-009; F6; partial)
- [ ] T075 Criar testes de domínio para Habilidade, Item, configuração de classe, Personagem, Inimigo, vínculos, resistências, Stress, Chance de Virtude e estados psicológicos em `tests/DarkestDungeon.Domain.Tests` (HIGH; T020, T028, T036, T037, T046, T054; F2/F7; missing)
- [ ] T076 Criar testes de contrato, validação, persistência e formato para Habilidade, configuração de classe, Personagem, Inimigo e Item em `tests/DarkestDungeon.Api.Tests` (CRITICAL; Constitution III; T021–T022, T027, T029–T030, T038–T039, T047–T048, T055–T056; F2/F7; missing)
- [ ] T077 Criar teste de resolução de DI para todos os serviços e repositórios da feature e atualizar testes de arquitetura em `tests/DarkestDungeon.Architecture.Tests/DependencyResolutionTests.cs` e `tests/DarkestDungeon.Architecture.Tests/LayerDependencyTests.cs` (HIGH; Constitution I/III; FR-023; F9; missing)
- [ ] T078 Sincronizar `specs/002-entidades-habilidades/contracts/openapi.yaml`, `specs/002-entidades-habilidades/contracts/interfaces.md` e `specs/002-entidades-habilidades/quickstart.md` com as rotas, estados, erros e respostas implementados (MEDIUM; plan: contratos; F8; partial)
- [ ] T079 Executar o quickstart ponta a ponta com SQL Server, migration, reinício da aplicação e consultas posteriores, registrando o resultado em `specs/002-entidades-habilidades/quickstart.md` (HIGH; SC-008; T067; F10; missing)

## Phase 10: Convergence

**Purpose**: Fechar as lacunas restantes identificadas após a segunda rodada de implementação.

- [X] T080 Substituir coleções `NotMapped` por entidades de associação persistentes para habilidades de Personagem, habilidades de Inimigo, equipamentos, inventário, individualidades, doenças e estados Habilitada/Treinada/Equipada em `src/DarkestDungeon.Domain/Habilidades`, `src/DarkestDungeon.Domain/Seres` e `src/DarkestDungeon.Infrastructure/Data` (CRITICAL; Constitution II; FR-021; F11; partial)
- [X] T081 Atualizar e gerar migration para todas as associações e estados persistentes da feature em `src/DarkestDungeon.Infrastructure/Migrations` e validar criação do banco no SQL Server em ambiente de teste (CRITICAL; Constitution II; FR-021; F11; partial)
- [ ] T082 Implementar `IInimigoRepository` e `InimigoRepository` com carregamento agregado das habilidades e resistências efetivas, substituindo o repositório genérico no `InimigoService` em `src/DarkestDungeon.Application/Abstractions/IInimigoRepository.cs`, `src/DarkestDungeon.Infrastructure/Repositories/InimigoRepository.cs` e `src/DarkestDungeon.Application/Inimigos/InimigoService.cs` (HIGH; US4; FR-015–FR-017; F12; missing)
- [ ] T083 Corrigir `ConfiguracaoClasseRepository` para carregar as associações de Combate e Acampamento e garantir substituição atômica sem alterar fotografias de Personagens em `src/DarkestDungeon.Infrastructure/Repositories/ConfiguracaoClasseRepository.cs` e `src/DarkestDungeon.Application/Personagens/ConfiguracaoClassePersonagemService.cs` (HIGH; US2/US3; FR-006–FR-011; F13; partial)
- [ ] T084 Completar endpoints e serviços de Personagem para treinamento/equipamento, limite de três habilidades equipadas, equipamentos, inventário, flags de sobrevivência, individualidades, doenças e Aflição/Virtude em `src/DarkestDungeon.Api/Controllers/PersonagensController.cs`, `src/DarkestDungeon.Api/Contracts/PersonagemContracts.cs` e `src/DarkestDungeon.Application/Personagens` (HIGH; US3; FR-012–FR-012m; F14; partial)
- [ ] T085 Criar testes específicos de domínio, API, persistência, formato e DI para Habilidade, Item, configuração de classe, Personagem, Inimigo e associações em `tests/DarkestDungeon.Domain.Tests`, `tests/DarkestDungeon.Api.Tests` e `tests/DarkestDungeon.Architecture.Tests` (CRITICAL; Constitution III; T075–T077; F15; missing)
- [ ] T086 Sincronizar `specs/002-entidades-habilidades/contracts/openapi.yaml`, `specs/002-entidades-habilidades/contracts/interfaces.md` e `specs/002-entidades-habilidades/quickstart.md` com os contratos HTTP reais e executar o quickstart completo contra SQL Server, registrando o resultado ou o bloqueio operacional (HIGH; FR-022; SC-008; F16/F17; partial/missing)
