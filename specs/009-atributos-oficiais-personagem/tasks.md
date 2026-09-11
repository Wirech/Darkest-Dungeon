---

description: "Task list for Atributos Oficiais do Personagem"
---

# Tasks: Atributos Oficiais do Personagem

**Input**: Design documents from `specs/009-atributos-oficiais-personagem/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/catalogo-oficial.md`, `contracts/personagens-atributos.md`, `quickstart.md`

**Tests**: Incluídos — a constituição exige contratos de backend verificáveis; a spec define testes independentes e SC-001..SC-007.

**Organization**: Tasks agrupadas por user story (US1 catálogo local, US2 criação fiel, US3 card).

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Pode rodar em paralelo (arquivos diferentes, sem depender de fatia incompleta).
- **[Story]**: `US1`, `US2` ou `US3` conforme `spec.md`.
- Toda task inclui caminho de arquivo.

## Path Conventions

- Solução em camadas: `src/DarkestDungeon.*`, `tests/`, `tools/`
- Snapshots: `specs/009-atributos-oficiais-personagem/wiki-snapshots/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Estrutura do coletor de curadoria e pasta de snapshots, sem ainda minerar números.

- [X] T001 Create the curadoria CLI project `tools/DarkestDungeon.WikiCatalogCollector/DarkestDungeon.WikiCatalogCollector.csproj` (net10, BCL only) and add it to `DarkestDungeon.sln`.
- [X] T002 [P] Document the per-class JSON schema and slug table in `specs/009-atributos-oficiais-personagem/wiki-snapshots/README.md`.
- [X] T003 [P] Add CLI argument parsing (`--saida`) and PT-BR usage text in `tools/DarkestDungeon.WikiCatalogCollector/Program.cs`.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Modelo persistido e DTOs que todas as histórias usam. Sem números da wiki ainda.

**Checkpoint**: `Classe` e `Personagem` representam passos frente/atrás e extras de classe; migração lê legado sem quebrar.

- [X] T004 Add `PassosAFrente`, `PassosAtras`, `Religiosa`, `ProvisaoInicial` and `BonusAoCriticoDaClasse` with invariants to `src/DarkestDungeon.Domain/Classes/Classe.cs`.
- [X] T005 Add `PassosAFrente` and `PassosAtras` (legacy-compatible) to `src/DarkestDungeon.Domain/Seres/Personagem.cs` without collapsing them into `Ser.Movimento`.
- [X] T006 [P] Extend `ClasseDetalheDto` with the new class-profile fields in `src/DarkestDungeon.Application/Classes/ClasseDtos.cs`.
- [X] T007 [P] Extend `PersonagemResumoDto` and `PersonagemDetalheDto` with combat attributes, eight resistances, passos, equipment levels, appearance and class extras in `src/DarkestDungeon.Application/Personagens/PersonagemDtos.cs`.
- [X] T008 Add `DerivacaoDeAtributosOficiais` (HP/DODGE from armor, DMG/CRIT/SPD from weapon, ACC/PROT 0, resistências base+10×nível teto 100) in `src/DarkestDungeon.Application/Personagens/DerivacaoDeAtributosOficiais.cs`.
- [X] T009 Map the new `Classe` and `Personagem` columns with legacy defaults in `src/DarkestDungeon.Infrastructure/Data/DarkestDungeonDbContext.cs`.
- [X] T010 Create the versioned EF Core migration for class extras and character passos in `src/DarkestDungeon.Infrastructure/Migrations/`.
- [X] T011 Add PT-BR catalog-incomplete / missing-official-table outcomes in `src/DarkestDungeon.Application/Validation/ResultadoOperacao.cs`.

---

## Phase 3: User Story 1 - Completar o catálogo oficial de atributos (Priority: P1) 🎯 MVP

**Goal**: Baixar uma vez as tabelas oficiais (20×5 arma/armadura, deslocamento, religiosa, provisão, bônus ao crítico; Abominação humana) e gravá-las no catálogo local. Sem cobertura completa, a criação fiel não é considerada pronta.

**Independent Test**: Validar 20 JSON locais com 5 níveis de arma, 5 de armadura e os campos extras; seed idempotente; lacuna falha sem inventar valor; criação/consulta não chamam a wiki.

### Tests for User Story 1

- [X] T012 [P] [US1] Add snapshot coverage tests (20 files, 5+5 levels, no null official fields, Abomination `forma: humana` only) reading committed JSON in `tests/DarkestDungeon.Api.Tests/Feature009/WikiSnapshotsCoberturaTests.cs` with no network.
- [X] T013 [P] [US1] Add architecture tests forbidding wiki `HttpClient` usage from Domain, Application and Api runtime in `tests/DarkestDungeon.Architecture.Tests/WikiIsolamentoTests.cs`.
- [X] T014 [P] [US1] Add domain/seed tests that official weapon/armor materialization is 20×5 and Musketeer is a distinct record in `tests/DarkestDungeon.Domain.Tests/Feature009/EquipamentosOficiaisTests.cs`.

### Implementation for User Story 1

- [X] T015 [US1] Implement wikitext fetch (`?action=raw`) and infobox/equipment parsers in `tools/DarkestDungeon.WikiCatalogCollector/` (pages of the 20 `ClasseDeHeroi` Description names).
- [X] T016 [US1] Implement fail-fast validation (missing class/level/field, ambiguous number, beast-only Abomination block) with PT-BR report and non-zero exit in `tools/DarkestDungeon.WikiCatalogCollector/`.
- [X] T017 [US1] Run the collector into `specs/009-atributos-oficiais-personagem/wiki-snapshots/` and **stop for case-by-case decision** if any gap remains; do not invent, interpolate or copy Arbalest onto Musketeer.
- [X] T018 [US1] Implement idempotent `EquipamentosSeed` (deterministic IDs, `NivelDeArmadura.HpAdicional` = wiki MAX HP) in `src/DarkestDungeon.Infrastructure/Data/Seeds/EquipamentosSeed.cs` from the committed snapshots only.
- [X] T019 [US1] Populate new `Classe` profile fields from snapshots in `src/DarkestDungeon.Infrastructure/Data/Seeds/ClassesSeed.cs`.
- [X] T020 [US1] Wire idempotent equipment and class-profile seed at startup in `src/DarkestDungeon.Api/Program.cs`.

**Checkpoint**: Catálogo local 20×5 + deslocamento + extras existe; coletor/seed não inventam valores; runtime não fala com a wiki.

---

## Phase 4: User Story 2 - Criar personagem com atributos fiéis (Priority: P1)

**Goal**: Os seis campos da 008 continuam; o backend grava HP/esquiva/arma, precisão 0, proteção 0, passos da classe, resistências com bônus de resolução e 4+4 habilidades. Sem trinkets. Abominação humana.

**Independent Test**: Criar Cruzado (e Abominação) com níveis explícitos e conferir atributos contra o snapshot local; catálogo incompleto recusa e não persiste.

### Tests for User Story 2

- [X] T021 [P] [US2] Add domain tests for `DerivacaoDeAtributosOficiais` (armor HP/DODGE, weapon DMG/CRIT/SPD, ACC/PROT 0, six resists +10 p.p.×level cap 100, deathblow/trap unchanged) in `tests/DarkestDungeon.Domain.Tests/Feature009/DerivacaoDeAtributosOficiaisTests.cs`.
- [X] T022 [P] [US2] Add API tests for faithful create: sample of at least 3 classes × 3 resolve levels, precision/protection/stress 0, passos from class, 4+4 skills unchanged, in `tests/DarkestDungeon.Api.Tests/Feature009/PersonagensCriacaoFielTests.cs`.
- [X] T023 [P] [US2] Add API test that incomplete official catalog rejects create with PT-BR error and no partial `Personagem` in `tests/DarkestDungeon.Api.Tests/Feature009/PersonagensCatalogoIncompletoTests.cs`.
- [X] T024 [P] [US2] Add API test that Abomination create uses human-form snapshot numbers only in `tests/DarkestDungeon.Api.Tests/Feature009/AbominacaoFormaHumanaTests.cs`.

### Implementation for User Story 2

- [X] T025 [US2] Apply official derivation in complete-create mode (six user fields only) in `src/DarkestDungeon.Application/Personagens/PersonagemService.cs`; set `BonusDeCritico` of the hero to 0; copy `PassosAFrente`/`PassosAtras` from class; do not copy religiosa/provisão/crit-buff onto `Personagem`.
- [X] T026 [US2] Refuse faithful create when the class lacks official 5 weapon levels, 5 armor levels or both movement steps in `src/DarkestDungeon.Application/Personagens/PersonagemService.cs`.
- [X] T027 [US2] Map passos and derived combat fields on create/detail in `src/DarkestDungeon.Application/Personagens/PersonagemMapper.cs`.
- [X] T028 [US2] Keep `POST /personagens` request as the six essential fields in `src/DarkestDungeon.Api/Contracts/Catalogo/PersonagemContracts.cs` and `src/DarkestDungeon.Api/Controllers/Catalogo/PersonagensController.cs`.

**Checkpoint**: Criação fiel persiste números oficiais locais; catálogo incompleto não grava herói.

---

## Phase 5: User Story 3 - Ver o card completo do personagem por categoria (Priority: P1)

**Goal**: Lista/card mostra HP, Stress, Atributos, Resistências, habilidades de combate e de acampamento (com nível, inclusive 0), mais níveis/aparência e extras da **classe** via join.

**Independent Test**: Abrir a lista com um herói fiel e ler as seis categorias e os extras sem segundo clique/request.

### Tests for User Story 3

- [X] T029 [P] [US3] Add API tests that `GET /personagens` includes HP, stress, attributes (ACC/PROT/DODGE/SPD/CRIT/DMG/passos), eight resists, all skills with category+level (including 0), hero/weapon/armor levels, appearance, and class extras in `tests/DarkestDungeon.Api.Tests/Feature009/PersonagensCardListaTests.cs`.
- [X] T030 [P] [US3] Add API test that `GET /classes/{id}` exposes religiosa, provisão inicial, bônus ao crítico and passos in `tests/DarkestDungeon.Api.Tests/Feature009/ClassesPerfilExtrasTests.cs`.
- [X] T031 [P] [US3] Add compatibility test that legacy characters remain readable without silent recálculo in `tests/DarkestDungeon.Api.Tests/Feature009/PersonagensLegadoSemRecalculoTests.cs`.

### Implementation for User Story 3

- [X] T032 [US3] Enrich `ListarAsync` with persisted combat stats, eight resistances, skill levels including 0, and class extras joined from `Classe` in `src/DarkestDungeon.Application/Personagens/PersonagemService.cs`.
- [X] T033 [US3] Map class extras on `GET /classes/{id}` in `src/DarkestDungeon.Application/Classes/ClasseService.cs`.
- [X] T034 [US3] Render six PT-BR categories plus header (níveis/aparência) and class extras; show locked skills at level 0 in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.js` and `src/DarkestDungeon.Api/wwwroot/personagens/index.html`.
- [X] T035 [US3] Style the categorized card layout in `src/DarkestDungeon.Api/wwwroot/personagens/personagens.css`.

**Checkpoint**: Um avaliador lê o card completo na lista sem ação extra.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Contratos, quickstart, suíte e isolamento da wiki.

- [X] T036 [P] Align final field names in `specs/009-atributos-oficiais-personagem/contracts/personagens-atributos.md` and `specs/009-atributos-oficiais-personagem/data-model.md`.
- [X] T037 [P] Verify collector, create and card scenarios in `specs/009-atributos-oficiais-personagem/quickstart.md`.
- [X] T038 Run Domain, Architecture and API tests plus `dotnet build DarkestDungeon.sln --no-restore --configuration Release` and fix regressions in the affected projects.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: T001–T003 em paralelo após T001 criar o projeto.
- **Foundational (Phase 2)**: Depende do Setup; bloqueia US1 seed (precisa das colunas) e US2/US3 (DTOs/fórmula).
- **US1 (Phase 3)**: Depende da Foundational. **MVP**. Sem snapshots+seed válidos, US2 não pode ser fiel.
- **US2 (Phase 4)**: Depende de US1 (números locais). Reusa o POST de seis campos da 008.
- **US3 (Phase 5)**: Depende de US2 para ter herói fiel na lista; o join de extras da classe pode avançar em paralelo após T004/T006/T019.
- **Polish (Phase 6)**: Depende das três histórias.

### User Story Dependencies

- **US1 (P1)**: Independente após o modelo; entrega o catálogo local.
- **US2 (P1)**: Precisa do seed oficial; testável via API sem UI.
- **US3 (P1)**: Precisa do payload enriquecido da criação/lista; UI pode seguir o contrato T007/T029.

### Parallel Opportunities

- T002–T003 após T001.
- T006–T007 em paralelo com T004–T005 se os nomes de campo já estiverem no data-model.
- T012–T014 em paralelo antes/durante T015–T016 (fixtures podem usar JSON de exemplo até T017 commitar os 20 oficiais).
- T021–T024 em paralelo.
- T029–T031 em paralelo; T034–T035 no frontend enquanto T032 estabiliza o JSON.
- T036–T037 em paralelo.

## Parallel Execution Examples

### User Story 1

```text
Task: T012 - Cobertura dos snapshots em tests/DarkestDungeon.Api.Tests/Feature009/WikiSnapshotsCoberturaTests.cs
Task: T013 - Isolamento wiki em tests/DarkestDungeon.Architecture.Tests/WikiIsolamentoTests.cs
Task: T014 - Seed 20×5 em tests/DarkestDungeon.Domain.Tests/Feature009/EquipamentosOficiaisTests.cs
```

### User Story 2

```text
Task: T021 - Fórmula em tests/DarkestDungeon.Domain.Tests/Feature009/DerivacaoDeAtributosOficiaisTests.cs
Task: T022/T023/T024 - Contratos HTTP em tests/DarkestDungeon.Api.Tests/Feature009/
```

### User Story 3

```text
Task: T029/T030/T031 - Lista, classe e legado em tests/DarkestDungeon.Api.Tests/Feature009/
Task: T034/T035 - Card em src/DarkestDungeon.Api/wwwroot/personagens/
```

## Implementation Strategy

### MVP First (US1 + foundation)

1. Setup + Foundational (modelo e migração).
2. Coletor + snapshots + seed 20×5. Se faltar campo, **parar e perguntar**.
3. Só então oferecer criação fiel.

### Incremental Delivery

1. US2: derivar atributos na criação; recusar catálogo incompleto.
2. US3: enriquecer lista e card.
3. Polish: suíte Release + quickstart.

### Independent Test Criteria

- **US1**: 20 snapshots locais completos; seed idempotente; lacuna não inventa; Domain/Application/Api sem HTTP à wiki.
- **US2**: Criação com seis campos gera HP/arma/armadura/passos/resistências oficiais; ACC/PROT/stress 0; Abominação humana; falha sem persistência parcial.
- **US3**: `GET /personagens` basta para o card (seis categorias, níveis, aparência, extras da classe, habilidades nível 0 visíveis).

## Notes

- Não marcar task concluída sem código, testes afetados e validação do plano.
- T017 é o único ponto em que a wiki é acessada; o produto nunca a consulta depois.
- Números oficiais não devem aparecer inventados em `tasks.md` nem em contratos — só nos snapshots commitados.
- Migração SQL Server deve ser revisada contra o snapshot EF existente.
- Personagens antigos: leitura compatível, sem recálculo no GET.
