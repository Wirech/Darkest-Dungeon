---
description: "Tasks para hierarquia de entidades e catÃ¡logo oficial de herÃ³is"
---

# Tasks: Hierarquia de Entidades e CatÃ¡logo Oficial de HerÃ³is

**Input**: Design documents from `/specs/003-hierarquia-catalogo-herois/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/openapi.yaml, contracts/interfaces.md, quickstart.md

**Tests**: IncluÃ­dos por exigÃªncia do PrincÃ­pio III da constituiÃ§Ã£o (Testes de Backend VerificÃ¡veis).

**Organization**: Tasks agrupadas por user story (US1 a US5) para entrega incremental e teste independente.

**Refinamentos da /speckit-clarify (2026-09-07)**: (a) Individualidade e DoenÃ§as estÃ£o fora do
escopo desta feature; (b) Personagem MUST respeitar limite 6 habilidades de Combate + 6 de
Acampamento (FR-009); (c) toda habilidade atribuÃ­da a um Personagem MUST pertencer Ã 
associaÃ§Ã£o Classe Ã— Habilidade da Classe (FR-009a); (d) `Acessorio.ConjuntoId` Ã© apenas
metadata; (e) toda resposta 400/404 MUST usar o schema `ErroResponse` de 001 (FR-025); (f)
`HabilidadeDePersonagem` mantÃ©m referÃªncia viva a `Habilidade` (esta feature nÃ£o expÃµe
`PUT/PATCH`); (g) as sete raridades de AcessÃ³rio formam conjunto fechado (FR-021); (h)
exclusÃ£o de Classe/Habilidade/Item estÃ¡ fora de escopo, com todas as FKs protegidas por
`OnDelete(Restrict)` (FR-023); (i) inclusÃ£o de uma 21Âª classe segue procedimento documentado
(novo valor no enum + migraÃ§Ã£o + seed + Mapa de Cobertura).

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Pode rodar em paralelo (arquivos diferentes, sem dependÃªncia).
- **[Story]**: US1..US5 identifica a user story da spec.
- Todos os paths sÃ£o relativos Ã  raiz do repositÃ³rio.

## Path Conventions

- DomÃ­nio: `src/DarkestDungeon.Domain/`
- AplicaÃ§Ã£o: `src/DarkestDungeon.Application/`
- Infraestrutura: `src/DarkestDungeon.Infrastructure/`
- API: `src/DarkestDungeon.Api/`
- Testes: `tests/DarkestDungeon.Domain.Tests/`, `tests/DarkestDungeon.Api.Tests/`, `tests/DarkestDungeon.Architecture.Tests/`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Preparar a soluÃ§Ã£o existente para as novas entidades sem alterar a feature 001.

- [x] T001 Verificar que a branch `003-hierarquia-catalogo-herois` estÃ¡ ativa e que `dotnet restore` e `dotnet build` (na raiz) rodam sem erros antes de introduzir novos arquivos.
- [x] T002 Adicionar pastas base do Domain: criar `src/DarkestDungeon.Domain/Classes/`, `src/DarkestDungeon.Domain/Habilidades/`, `src/DarkestDungeon.Domain/Itens/`, `src/DarkestDungeon.Domain/Personagens/`, `src/DarkestDungeon.Domain/Cobertura/` (via `.gitkeep` se ainda sem arquivos).
- [x] T003 [P] Adicionar pastas base da Application: criar `src/DarkestDungeon.Application/Classes/`, `Habilidades/`, `Personagens/`, `Inimigos/`, `Itens/`, `Cobertura/`.
- [x] T004 [P] Adicionar pastas base da Api: criar `src/DarkestDungeon.Api/Controllers/Catalogo/` e `src/DarkestDungeon.Api/Contracts/Catalogo/`.

## Phase 2: Foundational (blocking prerequisites for all user stories)

**Purpose**: Enums, Owned Types compartilhados e infraestrutura de EF Core exigidos por todas as histÃ³rias. **MUST completar antes das fases US1â€“US5.**

- [x] T005 [P] Criar enum `ClasseDeHeroi` (20 valores) em `src/DarkestDungeon.Domain/Classes/ClasseDeHeroi.cs`, com atributos `Description` mantendo o nome original em inglÃªs.
- [x] T006 [P] Criar enum `TipoDeInimigo` (7 valores) em `src/DarkestDungeon.Domain/Seres/TipoDeInimigo.cs`.
- [x] T007 [P] Criar enums `AlvoDeEfeito`, `UnidadeDeEfeito`, `SinalDeEfeito`, `EscopoDeLimite`, `AlvoDeAcampamento`, `RaridadeDeAcessorio` em `src/DarkestDungeon.Domain/Habilidades/EnumsHabilidade.cs` e `src/DarkestDungeon.Domain/Itens/EnumsItem.cs` (separando por Ã¡rea).
- [x] T008 [P] Criar enums `CategoriaDeCobertura` e `EstadoDeAtributo` em `src/DarkestDungeon.Domain/Cobertura/EnumsCobertura.cs`.
- [x] T009 [P] Criar Owned Type `EfeitoDeHabilidade` em `src/DarkestDungeon.Domain/Habilidades/EfeitoDeHabilidade.cs` (nome, alvo, valor, unidade, duraÃ§Ã£o?, chanceBase) com validaÃ§Ãµes no construtor.
- [x] T010 [P] Criar Owned Type `LimitePorUso` em `src/DarkestDungeon.Domain/Habilidades/LimitePorUso.cs`.
- [x] T011 [P] Criar Owned Type `ResistenciasDeClasse` em `src/DarkestDungeon.Domain/Classes/ResistenciasDeClasse.cs` (8 percentuais 0â€“100).
- [x] T012 [P] Criar Owned Type `ResistenciasExtrasDePersonagem` em `src/DarkestDungeon.Domain/Personagens/ResistenciasExtrasDePersonagem.cs` (DoenÃ§a, Golpe Mortal, Armadilha).
- [x] T013 [P] Criar Owned Type `NivelDeArma` em `src/DarkestDungeon.Domain/Itens/NivelDeArma.cs` com validaÃ§Ã£o de nÃ­vel 1..5 e `DanoMaximo >= DanoMinimo`.
- [x] T014 [P] Criar Owned Type `NivelDeArmadura` em `src/DarkestDungeon.Domain/Itens/NivelDeArmadura.cs` com validaÃ§Ã£o de nÃ­vel 1..5.
- [x] T015 [P] Criar Owned Type `EfeitoDeAcessorio` em `src/DarkestDungeon.Domain/Itens/EfeitoDeAcessorio.cs`.
- [x] T016 [P] Criar Owned Type `Inventario` (4 slots) em `src/DarkestDungeon.Domain/Personagens/Inventario.cs`.
- [x] T017 [P] Criar Owned Type `HabilidadeDePersonagem` em `src/DarkestDungeon.Domain/Personagens/HabilidadeDePersonagem.cs` com invariante `Equipada â‡’ Treinada`. Guarda apenas `HabilidadeId` (referÃªncia viva a `Habilidade`); nenhum snapshot de atributos.
- [x] T018 Registrar mÃ³dulo de DI `AdicionarServicosDoCatalogo` em `src/DarkestDungeon.Application/Extensions/ServiceCollectionExtensions.cs` (esqueleto vazio; serÃ¡ preenchido por cada US).
- [x] T019 Registrar mÃ³dulo de DI `AdicionarRepositoriosDoCatalogo` em `src/DarkestDungeon.Infrastructure/Extensions/ServiceCollectionExtensions.cs` (esqueleto vazio; serÃ¡ preenchido por cada US).
- [x] T020 Chamar `AdicionarServicosDoCatalogo` e `AdicionarRepositoriosDoCatalogo` em `src/DarkestDungeon.Api/Program.cs` (apÃ³s os mÃ³dulos existentes da 001).
- [x] T021 [P] Adicionar teste `EnumsDoCatalogoTests` em `tests/DarkestDungeon.Domain.Tests/EnumsDoCatalogoTests.cs` validando: `ClasseDeHeroi` tem 20 valores; `TipoDeInimigo` tem 7 valores; todos os enums de habilidade/item existem com os valores esperados.

**Checkpoint**: FundaÃ§Ã£o pronta â€” enums, owned types compartilhados e DI vazia registrada. Nenhuma US pode iniciar antes deste checkpoint.

---

## Phase 3: User Story 1 â€” Refinar a hierarquia de entidades (Priority: P1) ðŸŽ¯ MVP

**Goal**: Introduzir `Personagem`, `Inimigo`, `Habilidade` (base), `HabilidadeDeHeroi`/`HabilidadeDeInimigo`, `HabilidadeDeCombate`/`HabilidadeDeAcampamento`, `Item` (base), `Arma`/`Armadura`/`Acessorio` como classes de domÃ­nio com regras prÃ³prias, sem duplicar campos base.

**Independent Test**: Consulta ao mapa de tipos do domÃ­nio (via teste de arquitetura) confirma a hierarquia; teste de domÃ­nio confirma que criar `Personagem` nÃ£o permite atribuir campo exclusivo de `Inimigo` e vice-versa.

### Tests for User Story 1

- [x] T022 [P] [US1] Criar `HierarquiaExtensibilidadeTests` em `tests/DarkestDungeon.Architecture.Tests/HierarquiaExtensibilidadeTests.cs` validando via NetArchTest: `Personagem` e `Inimigo` herdam de `Ser`; `HabilidadeDeHeroi` e `HabilidadeDeInimigo` herdam de `Habilidade`; `HabilidadeDeCombate` e `HabilidadeDeAcampamento` herdam de `HabilidadeDeHeroi`; `Arma`/`Armadura`/`Acessorio` herdam de `Item`.
- [x] T023 [P] [US1] Criar `HabilidadeTests` em `tests/DarkestDungeon.Domain.Tests/HabilidadeTests.cs` cobrindo criaÃ§Ã£o de cada subtipo e invariantes bÃ¡sicas (nome nÃ£o vazio, discriminator correto).
- [x] T024 [P] [US1] Criar `EfeitoDeHabilidadeTests` em `tests/DarkestDungeon.Domain.Tests/EfeitoDeHabilidadeTests.cs` cobrindo validaÃ§Ã£o de ChanceBase 0â€“100 e DuraÃ§Ã£o â‰¥ 0.
- [x] T025 [P] [US1] Criar `ItemTests` em `tests/DarkestDungeon.Domain.Tests/ItemTests.cs` cobrindo criaÃ§Ã£o de `Arma`, `Armadura` e `Acessorio` como subtipos de `Item`.

### Implementation for User Story 1

- [x] T026 [P] [US1] Criar classe abstrata `Habilidade` em `src/DarkestDungeon.Domain/Habilidades/Habilidade.cs` herdando `EntidadeIdentificavel` com `NomeExibicao`, `NomeOriginal`, `Descricao`.
- [x] T027 [P] [US1] Criar classe abstrata `HabilidadeDeHeroi` em `src/DarkestDungeon.Domain/Habilidades/HabilidadeDeHeroi.cs` com coleÃ§Ã£o `Efeitos` (owned) e `LimitePorUso?` (owned).
- [x] T028 [P] [US1] Criar `HabilidadeDeCombate` em `src/DarkestDungeon.Domain/Habilidades/HabilidadeDeCombate.cs` (PosicoesValidas, PosicoesQueAtinge, AlvoEmArea, ModificadorDano/Acerto/Critico).
- [x] T029 [P] [US1] Criar `HabilidadeDeAcampamento` em `src/DarkestDungeon.Domain/Habilidades/HabilidadeDeAcampamento.cs` (CustoDeDescanso, AlvoDeAcampamento).
- [x] T030 [P] [US1] Criar `HabilidadeDeInimigo` em `src/DarkestDungeon.Domain/Habilidades/HabilidadeDeInimigo.cs` (Efeitos, CondicaoDeAparecer, ChanceDeExecucao).
- [x] T031 [P] [US1] Criar classe abstrata `Item` em `src/DarkestDungeon.Domain/Itens/Item.cs` herdando `EntidadeIdentificavel`.
- [x] T032 [P] [US1] Criar `Arma` em `src/DarkestDungeon.Domain/Itens/Arma.cs` com `ClasseElegivel` e coleÃ§Ã£o `Niveis` (owned) â€” validaÃ§Ã£o de 5 nÃ­veis fica na US3.
- [x] T033 [P] [US1] Criar `Armadura` em `src/DarkestDungeon.Domain/Itens/Armadura.cs`.
- [x] T034 [P] [US1] Criar `Acessorio` em `src/DarkestDungeon.Domain/Itens/Acessorio.cs`.
- [x] T035 [P] [US1] Criar `Personagem` em `src/DarkestDungeon.Domain/Seres/Personagem.cs` herdando `Ser` com atributos e owned types conforme data-model.md. Individualidade e DoenÃ§as estÃ£o fora do escopo desta feature e NÃƒO devem ser modeladas.
- [x] T036 [P] [US1] Criar `Inimigo` em `src/DarkestDungeon.Domain/Seres/Inimigo.cs` herdando `Ser` com `TipoDeInimigo` e lista de `HabilidadeDeInimigo.Id`.

**Checkpoint**: Hierarquia de tipos existe no domÃ­nio e Ã© validada por testes de arquitetura. Nada persiste ainda.

---

## Phase 4: User Story 2 â€” Popular catÃ¡logo com as 20 classes e suas habilidades (Priority: P2)

**Goal**: Persistir `Classe`, `HabilidadeDeHeroi`, `HabilidadeDeInimigo` e a associaÃ§Ã£o `ClasseHabilidade`; expor endpoints de leitura/criaÃ§Ã£o; carregar as 20 classes oficiais no banco via seed.

**Independent Test**: `GET /classes` retorna 20 classes; `GET /classes/{id}/habilidades` retorna habilidades associadas; `POST /habilidades/combate` cria habilidade e recusa nome duplicado.

### Tests for User Story 2

- [x] T037 [P] [US2] Criar `ClasseTests` em `tests/DarkestDungeon.Domain.Tests/ClasseTests.cs` validando associaÃ§Ã£o de habilidades e unicidade de par (Classe, Habilidade).
- [x] T038 [P] [US2] Criar `ClassesEndpointsTests` em `tests/DarkestDungeon.Api.Tests/ClassesEndpointsTests.cs` cobrindo `GET /classes` (200 com 20 itens), `GET /classes/{id}` (200 + 404), `GET /classes/{id}/habilidades` (200 + 404).
- [x] T039 [P] [US2] Criar `HabilidadesEndpointsTests` em `tests/DarkestDungeon.Api.Tests/HabilidadesEndpointsTests.cs` cobrindo POSTs de combate, acampamento e inimigo, e `GET /habilidades/{id}`.
- [x] T040 [P] [US2] Criar `HabilidadesValidationTests` em `tests/DarkestDungeon.Api.Tests/HabilidadesValidationTests.cs` cobrindo 400 para nome duplicado e 400 para habilidade de acampamento vinculada a Inimigo.

### Implementation for User Story 2

- [x] T041 [P] [US2] Criar entidade `Classe` em `src/DarkestDungeon.Domain/Classes/Classe.cs` com `ClasseDeHeroi`, `NomeExibicao`, `NomeOriginal`, `ResistenciasBase`.
- [x] T042 [P] [US2] Criar associaÃ§Ã£o `ClasseHabilidade` em `src/DarkestDungeon.Domain/Classes/ClasseHabilidade.cs` (PK composta).
- [x] T043 [US2] Atualizar `DarkestDungeonDbContext` em `src/DarkestDungeon.Infrastructure/Data/DarkestDungeonDbContext.cs` adicionando `DbSet<Classe>`, `DbSet<Habilidade>`, `DbSet<ClasseHabilidade>`; configurar TPH para `Habilidade` (discriminator `Combate`/`Acampamento`/`Inimigo`); mapear owned types de habilidade.
- [x] T043a [US2] Configurar `OnDelete(DeleteBehavior.Restrict)` em `DarkestDungeonDbContext.OnModelCreating` para todas as FKs que apontam para `Classe`, `Habilidade` e `Item` (a partir de `Personagem`, `ClasseHabilidade`, `Arma`, `Armadura`, `Acessorio`, `EntradaDoMapaDeCobertura` e referÃªncias de equipamento em `Personagem`) â€” alinhado a FR-023.
- [x] T044 [US2] Configurar Ã­ndice Ãºnico global `Habilidade.NomeExibicao` no `OnModelCreating` do DbContext.
- [x] T045 [US2] Criar migraÃ§Ã£o `AddCatalogoHabilidadesEClasses` em `src/DarkestDungeon.Infrastructure/Data/Migrations/` via `dotnet ef migrations add AddCatalogoHabilidadesEClasses`. (consolidada em T115 `AddCatalogoHeroisEEntidades`)
- [x] T046 [P] [US2] Criar `IClasseRepository` em `src/DarkestDungeon.Application/Abstractions/IClasseRepository.cs` e `IClasseService` em `src/DarkestDungeon.Application/Abstractions/IClasseService.cs`.
- [x] T047 [P] [US2] Criar `IHabilidadeRepository` e `IHabilidadeService` em `src/DarkestDungeon.Application/Abstractions/`.
- [x] T048 [P] [US2] Criar DTOs `ClasseResumoDto`, `ClasseDetalheDto`, `HabilidadeResumoDto`, `HabilidadeDetalheDto` em `src/DarkestDungeon.Application/Classes/` e `src/DarkestDungeon.Application/Habilidades/`.
- [x] T049 [P] [US2] Criar comandos `CriarHabilidadeDeCombateCommand`, `CriarHabilidadeDeAcampamentoCommand`, `CriarHabilidadeDeInimigoCommand` em `src/DarkestDungeon.Application/Habilidades/Commands/`.
- [x] T050 [US2] Implementar `ClasseService` em `src/DarkestDungeon.Application/Classes/ClasseService.cs` (ObterCatalogo, ObterPorId, ObterHabilidadesDaClasse).
- [x] T051 [US2] Implementar `HabilidadeService` em `src/DarkestDungeon.Application/Habilidades/HabilidadeService.cs` com validaÃ§Ã£o de nome Ãºnico global e regra que impede vincular `HabilidadeDeAcampamento` a Inimigo.
- [x] T052 [P] [US2] Implementar `ClasseRepository` em `src/DarkestDungeon.Infrastructure/Repositories/ClasseRepository.cs`.
- [x] T053 [P] [US2] Implementar `HabilidadeRepository` em `src/DarkestDungeon.Infrastructure/Repositories/HabilidadeRepository.cs`.
- [x] T054 [US2] Registrar `IClasseService`, `IClasseRepository`, `IHabilidadeService`, `IHabilidadeRepository` em `AdicionarServicosDoCatalogo` e `AdicionarRepositoriosDoCatalogo` (Phase 2).
- [x] T055 [P] [US2] Criar `ClassesController` em `src/DarkestDungeon.Api/Controllers/Catalogo/ClassesController.cs` com rotas `GET /classes`, `GET /classes/{id}`, `GET /classes/{id}/habilidades`.
- [x] T056 [P] [US2] Criar `HabilidadesController` em `src/DarkestDungeon.Api/Controllers/Catalogo/HabilidadesController.cs` com rotas `GET /habilidades`, `GET /habilidades/{id}`, `POST /habilidades/combate`, `POST /habilidades/acampamento`, `POST /habilidades/inimigo`.
- [x] T057 [P] [US2] Criar contratos HTTP em `src/DarkestDungeon.Api/Contracts/Catalogo/` refletindo os schemas do `contracts/openapi.yaml`.
- [x] T058 [US2] Implementar seed inicial das 20 classes em `src/DarkestDungeon.Infrastructure/Data/Seeds/ClassesSeed.cs`, chamado por `DarkestDungeonDbContext.OnModelCreating` (`HasData`) â€” sem resistÃªncias (US4) e sem habilidades (mineraÃ§Ã£o fica na US2 durante `/speckit-implement`).
- [x] T059 [US2] Documentar em `specs/003-hierarquia-catalogo-herois/dados-minerados.md` a lista das 20 classes (PT-BR + original) e o resumo por classe da mineraÃ§Ã£o da wiki oficial (`darkestdungeon.wiki.gg`), servindo como Ã­ndice das habilidades cadastradas.
- [x] T059a [US2] Para cada uma das 20 classes, criar as entradas de `HabilidadeDeCombate` (via `POST /habilidades/combate`) transcrevendo fielmente os dados oficiais da wiki; atributos ainda nÃ£o mineraveis ficam como `Pendente` no Mapa de Cobertura (T106). (implementado via seed C# — 140 habilidades de combate)
- [x] T059b [US2] Para cada uma das 20 classes, criar as entradas de `HabilidadeDeAcampamento` (via `POST /habilidades/acampamento`) transcrevendo fielmente os dados oficiais; habilidades compartilhadas (por exemplo, `Gallows Humor`) MUST reutilizar o mesmo registro com mÃºltiplas linhas em `ClasseHabilidade`. (implementado via seed C# — 79 habilidades de acampamento, com Gallows Humor, Field Dressing, Marching Plan, Triage, Encourage, Wound Care, Pep Talk compartilhadas)
- [x] T059c [US2] Materializar todas as associaÃ§Ãµes Classe Ã— Habilidade em `ClasseHabilidade` a partir das entradas de T059a e T059b, garantindo unicidade de par (Classe, Habilidade) e sem duplicar habilidades compartilhadas. (implementado via `HabilidadesSeed.MaterializarAssociacoes` — 277 associaÃ§Ãµes)
- [x] T060 [US2] Atualizar `Program.cs` para expor os novos endpoints no Swagger/OpenAPI e adicionar tags PT-BR (`Classes`, `Habilidades`).

**Checkpoint**: `GET /classes` retorna 20 classes; POSTs de habilidade retornam 201 e 400 conforme contrato; habilidades ficam associadas Ã s classes elegÃ­veis via `ClasseHabilidade`.

---

## Phase 5: User Story 3 â€” Refinar Item para Arma, Armadura e AcessÃ³rio (Priority: P3)

**Goal**: Persistir `Item` (TPH), aplicar as regras de 5 nÃ­veis fixos, expor endpoints e validar equipamento por classe elegÃ­vel.

**Independent Test**: `POST /armas`, `POST /armaduras`, `POST /acessorios` criam itens; `POST /personagens/{id}/equipar` rejeita item de classe diferente com 400.

### Tests for User Story 3

- [x] T061 [P] [US3] Criar `ArmaTests` em `tests/DarkestDungeon.Domain.Tests/ArmaTests.cs` cobrindo lista fixa de 5 nÃ­veis e rejeiÃ§Ã£o de nÃ­veis fora de 1..5 ou duplicados.
- [x] T062 [P] [US3] Criar `ArmaduraTests` em `tests/DarkestDungeon.Domain.Tests/ArmaduraTests.cs`.
- [x] T063 [P] [US3] Criar `AcessorioTests` em `tests/DarkestDungeon.Domain.Tests/AcessorioTests.cs` cobrindo raridade obrigatÃ³ria (conjunto fechado dos sete valores: `Comum`, `Incomum`, `Rara`, `MuitoRara`, `CrimsonCourt`, `Crystalline`, `Set`), classe exclusiva opcional e `ConjuntoId` opcional (metadata).
- [x] T064 [P] [US3] Criar `ItensEndpointsTests` em `tests/DarkestDungeon.Api.Tests/ItensEndpointsTests.cs` cobrindo POSTs por tipo, GET `/itens/{id}`, e 400 para arma sem 5 nÃ­veis.
- [x] T065 [P] [US3] Criar `PersonagensEquipamentoTests` em `tests/DarkestDungeon.Api.Tests/PersonagensEquipamentoTests.cs` cobrindo `POST /personagens/{id}/equipar` com sucesso e 400 para classe divergente e acessÃ³rio exclusivo de outra classe.

### Implementation for User Story 3

- [x] T066 [US3] Ampliar `Arma` (T032) com validaÃ§Ã£o estrita de exatamente 5 nÃ­veis (1..5, sem duplicados) no construtor/factory.
- [x] T067 [US3] Ampliar `Armadura` (T033) com validaÃ§Ã£o idÃªntica de 5 nÃ­veis.
- [x] T068 [US3] Ampliar `Acessorio` (T034) com `Raridade` obrigatÃ³ria (enum fechado de sete valores), `ClasseExclusiva?`, `ConjuntoId?` (metadata; sem cÃ¡lculo de bÃ´nus nesta feature) e coleÃ§Ã£o owned `Efeitos`.
- [x] T069 [US3] Atualizar `DarkestDungeonDbContext` mapeando TPH para `Item` (discriminator `Arma`/`Armadura`/`Acessorio`) e Owned collections dos nÃ­veis e efeitos.
- [x] T070 [US3] Criar migraÃ§Ã£o `AddCatalogoItens` em `src/DarkestDungeon.Infrastructure/Data/Migrations/`. (consolidada em T115 `AddCatalogoHeroisEEntidades`)
- [x] T071 [P] [US3] Criar `IItemRepository` e `IItemService` em `src/DarkestDungeon.Application/Abstractions/`.
- [x] T072 [P] [US3] Criar DTOs `ArmaDto`, `ArmaduraDto`, `AcessorioDto`, `ItemDetalheDto` em `src/DarkestDungeon.Application/Itens/`.
- [x] T073 [P] [US3] Criar comandos `CriarArmaCommand`, `CriarArmaduraCommand`, `CriarAcessorioCommand`.
- [x] T074 [US3] Implementar `ItemService` em `src/DarkestDungeon.Application/Itens/ItemService.cs` com validaÃ§Ã£o de nÃ­veis e raridade.
- [x] T075 [US3] Implementar `ItemRepository` em `src/DarkestDungeon.Infrastructure/Repositories/ItemRepository.cs` (leitura polimÃ³rfica de `Item`).
- [x] T076 [US3] Registrar `IItemService` e `IItemRepository` nos mÃ³dulos de DI da Phase 2.
- [x] T077 [P] [US3] Criar `ItensController` em `src/DarkestDungeon.Api/Controllers/Catalogo/ItensController.cs` com `GET /itens/{id}`.
- [x] T078 [P] [US3] Criar `ArmasController`, `ArmadurasController`, `AcessoriosController` em `src/DarkestDungeon.Api/Controllers/Catalogo/` com respectivos POSTs.
- [x] T079 [P] [US3] Criar contratos HTTP em `src/DarkestDungeon.Api/Contracts/Catalogo/` para Arma, Armadura, AcessÃ³rio e Item.

**Checkpoint**: Itens sÃ£o criados e consultados; validaÃ§Ãµes de 5 nÃ­veis e raridade retornam 400 em PT-BR.

---

## Phase 6: User Story 4 â€” Alinhar Classe e resistÃªncias Ã s oito resistÃªncias (Priority: P4)

**Goal**: Persistir `ResistenciasDeClasse` nas 20 classes; ao criar `Personagem`, copiar as resistÃªncias; expor endpoint `POST /personagens` que respeita FR-016 e FR-022.

**Independent Test**: `POST /personagens` cria personagem cujas resistÃªncias correspondem exatamente Ã s da classe; `POST /personagens/{id}/equipar` jÃ¡ cobre a validaÃ§Ã£o por classe (US3).

### Tests for User Story 4

- [x] T080 [P] [US4] Criar `ResistenciasDeClasseTests` em `tests/DarkestDungeon.Domain.Tests/ResistenciasDeClasseTests.cs` cobrindo range 0â€“100 e igualdade estrutural.
- [x] T081 [P] [US4] Criar `PersonagemTests` em `tests/DarkestDungeon.Domain.Tests/PersonagemTests.cs` cobrindo cÃ³pia de resistÃªncias da classe, invariante `Aflicao XOR Virtude` e limite 6+6 na coleÃ§Ã£o `Habilidades`.
- [x] T082 [P] [US4] Criar `PersonagensEndpointsTests` em `tests/DarkestDungeon.Api.Tests/PersonagensEndpointsTests.cs` cobrindo `POST /personagens`, `GET /personagens/{id}` e 400 para classe inexistente.
- [x] T082a [P] [US4] Adicionar em `PersonagensEndpointsTests` cenÃ¡rios cobrindo FR-009 (400 quando `POST /personagens` recebe mais de 6 habilidades de Combate ou mais de 6 de Acampamento) e FR-009a (400 quando alguma habilidade nÃ£o pertence Ã  Classe do Personagem), afirmando o schema `ErroResponse` PT-BR.
- [x] T083 [P] [US4] Criar `InimigosEndpointsTests` em `tests/DarkestDungeon.Api.Tests/InimigosEndpointsTests.cs` cobrindo `POST /inimigos` e `GET /inimigos/{id}`.

### Implementation for User Story 4

- [x] T084 [US4] Atualizar `Classe` (T041) para exigir `ResistenciasBase` no construtor; ajustar seed T058 para atribuir as 8 resistÃªncias oficiais mineradas por classe (marcar `Pendente` no Mapa de Cobertura quando o valor oficial for desconhecido).
- [x] T085 [US4] Atualizar `DarkestDungeonDbContext` mapeando `Personagem` e `Inimigo` como TPH sobre `Ser` (discriminator), com Owned Types de `ResistenciasExtrasDePersonagem`, `Inventario` e `HabilidadeDePersonagem`.
- [x] T086 [US4] Criar migraÃ§Ã£o `AddPersonagensEInimigos` em `src/DarkestDungeon.Infrastructure/Data/Migrations/`. (consolidada em T115 `AddCatalogoHeroisEEntidades`)
- [x] T087 [P] [US4] Criar `IPersonagemRepository`, `IPersonagemService`, `IInimigoRepository`, `IInimigoService` em `src/DarkestDungeon.Application/Abstractions/`.
- [x] T088 [P] [US4] Criar DTOs `PersonagemDetalheDto`, `InimigoDetalheDto` e comandos `CriarPersonagemCommand`, `EquiparPersonagemCommand`, `CriarInimigoCommand`.
- [x] T089 [US4] Implementar `PersonagemService` em `src/DarkestDungeon.Application/Personagens/PersonagemService.cs` (copia resistÃªncias da Classe na criaÃ§Ã£o; valida classe elegÃ­vel em `EquiparPersonagem`).
- [x] T089a [US4] Implementar em `PersonagemService` a validaÃ§Ã£o FR-009 (mÃ¡ximo 6 habilidades de Combate + 6 de Acampamento por Personagem) e FR-009a (toda `HabilidadeId` atribuÃ­da MUST pertencer Ã  associaÃ§Ã£o Classe Ã— Habilidade da Classe do Personagem), rejeitando com `ErroResponse` PT-BR indicando o `campo` responsÃ¡vel.
- [x] T090 [US4] Implementar `InimigoService` em `src/DarkestDungeon.Application/Inimigos/InimigoService.cs`.
- [x] T091 [P] [US4] Implementar `PersonagemRepository` e `InimigoRepository` em `src/DarkestDungeon.Infrastructure/Repositories/`.
- [x] T092 [US4] Registrar `IPersonagemService`, `IPersonagemRepository`, `IInimigoService`, `IInimigoRepository` nos mÃ³dulos de DI da Phase 2.
- [x] T093 [P] [US4] Criar `PersonagensController` em `src/DarkestDungeon.Api/Controllers/Catalogo/PersonagensController.cs` (`POST /personagens`, `GET /personagens/{id}`, `POST /personagens/{id}/equipar`).
- [x] T094 [P] [US4] Criar `InimigosController` em `src/DarkestDungeon.Api/Controllers/Catalogo/InimigosController.cs` (`POST /inimigos`, `GET /inimigos/{id}`).

**Checkpoint**: Personagem herda resistÃªncias da Classe; equipamentos sÃ£o validados; Inimigo persistido separadamente.

---

## Phase 7: User Story 5 â€” Mapa de Cobertura consultÃ¡vel (Priority: P5)

**Goal**: Persistir `EntradaDoMapaDeCobertura`, expor endpoints de leitura e permitir atualizaÃ§Ã£o de estado (Coletado / Pendente / NaoAplicavel).

**Independent Test**: `GET /mapa-de-cobertura` retorna cobertura por classe; `GET /mapa-de-cobertura/{classeId}` retorna a cobertura filtrada.

### Tests for User Story 5

- [x] T095 [P] [US5] Criar `MapaDeCoberturaTests` em `tests/DarkestDungeon.Domain.Tests/MapaDeCoberturaTests.cs` cobrindo unicidade por (Classe, Categoria, Chave) e transiÃ§Ãµes permitidas.
- [x] T096 [P] [US5] Criar `MapaDeCoberturaEndpointsTests` em `tests/DarkestDungeon.Api.Tests/MapaDeCoberturaEndpointsTests.cs` cobrindo GET completo, GET por classe e 404 para classe inexistente.

### Implementation for User Story 5

- [x] T097 [P] [US5] Criar `EntradaDoMapaDeCobertura` em `src/DarkestDungeon.Domain/Cobertura/EntradaDoMapaDeCobertura.cs` herdando `EntidadeIdentificavel`.
- [x] T098 [US5] Atualizar `DarkestDungeonDbContext` mapeando `DbSet<EntradaDoMapaDeCobertura>` e Ã­ndice Ãºnico lÃ³gico (ClasseDeHeroi, Categoria, ChaveDoAtributo).
- [x] T099 [US5] Criar migraÃ§Ã£o `AddMapaDeCobertura` em `src/DarkestDungeon.Infrastructure/Data/Migrations/`. (consolidada em T115 `AddCatalogoHeroisEEntidades`)
- [x] T100 [P] [US5] Criar `IMapaDeCoberturaRepository` e `IMapaDeCoberturaService` em `src/DarkestDungeon.Application/Abstractions/`.
- [x] T101 [P] [US5] Criar DTO `MapaDeCoberturaPorClasseDto` e `EntradaCoberturaDto`.
- [x] T102 [US5] Implementar `MapaDeCoberturaService` em `src/DarkestDungeon.Application/Cobertura/MapaDeCoberturaService.cs` (ObterMapaCompleto, ObterMapaPorClasse, RegistrarEstado).
- [x] T103 [US5] Implementar `MapaDeCoberturaRepository` em `src/DarkestDungeon.Infrastructure/Repositories/MapaDeCoberturaRepository.cs`.
- [x] T104 [US5] Registrar contratos de Mapa de Cobertura nos mÃ³dulos de DI da Phase 2.
- [x] T105 [P] [US5] Criar `MapaDeCoberturaController` em `src/DarkestDungeon.Api/Controllers/Catalogo/MapaDeCoberturaController.cs` (`GET /mapa-de-cobertura`, `GET /mapa-de-cobertura/{classeId}`).
- [x] T106 [US5] Popular Mapa de Cobertura inicial em `src/DarkestDungeon.Infrastructure/Data/Seeds/MapaDeCoberturaSeed.cs`: para cada classe, criar entradas `Pendente` para cada resistÃªncia ainda nÃ£o minerada e para cada habilidade oficial ainda nÃ£o catalogada em T059.

**Checkpoint**: Mapa de Cobertura consultÃ¡vel reflete estado atual da mineraÃ§Ã£o e das resistÃªncias.

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Testes de contrato OpenAPI, ajustes finais, documentaÃ§Ã£o executÃ¡vel.

- [x] T107 [P] Criar `ContratoOpenApiTests` em `tests/DarkestDungeon.Api.Tests/ContratoOpenApiTests.cs` validando: cada `path` e cada `schema` referenciado por `contracts/openapi.yaml` estÃ¡ presente no Swagger gerado pelo runtime.
- [x] T107a [P] Criar `ContratoErroResponseTests` em `tests/DarkestDungeon.Api.Tests/ContratoErroResponseTests.cs` percorrendo todas as respostas 400 e 404 dos novos endpoints e afirmando que o body segue o schema `ErroResponse` (`mensagem` obrigatÃ³rio em PT-BR, `campo` opcional) â€” cobrindo FR-025 e SC-009.
- [x] T107b [P] Criar `IntegridadeReferencialTests` em `tests/DarkestDungeon.Api.Tests/IntegridadeReferencialTests.cs` usando Testcontainers SQL Server para validar que tentativas de excluir uma `Classe`, `Habilidade` ou `Item` referenciados falham com `DbUpdateException` provocado por `OnDelete(Restrict)` â€” cobrindo FR-023.
- [x] T108 [P] Atualizar `DependencyInjectionTests` em `tests/DarkestDungeon.Architecture.Tests/DependencyInjectionTests.cs` para resolver todos os novos `I*Service` e `I*Repository` via container real e afirmar que nenhum contrato ficou sem implementaÃ§Ã£o.
- [x] T109 [P] Adicionar teste `HabilidadesCompartilhadasTests` em `tests/DarkestDungeon.Api.Tests/HabilidadesCompartilhadasTests.cs` validando que a mesma `HabilidadeDeAcampamento` aparece nas duas classes que a compartilham (por exemplo, `Gallows Humor` em Bandido e LadrÃ£o de Cova) com o mesmo `Id`.
- [x] T110 Rodar `dotnet build DarkestDungeon.sln -c Release` e resolver eventuais warnings virados em erro pelo `TreatWarningsAsErrors`.
- [x] T111 Rodar `dotnet test` completo e garantir todos os testes verdes (Domain + Api + Architecture).
- [x] T112 Executar `dotnet ef database update --startup-project src/DarkestDungeon.Api --project src/DarkestDungeon.Infrastructure` contra SQL Server real e confirmar aplicaÃ§Ã£o das migraÃ§Ãµes `AddCatalogoHabilidadesEClasses`, `AddCatalogoItens`, `AddPersonagensEInimigos`, `AddMapaDeCobertura`. (executado 2026-09-08 contra SQL Server 2022 em Docker â€” migraÃ§Ãµes consolidadas em `SchemaCompleto`; 16 tabelas + seeds populados: 20 classes, 219 habilidades, 277 associaÃ§Ãµes, 160 entradas do mapa)
- [x] T113 Executar cenÃ¡rios do `quickstart.md` (curl PowerShell) contra a API rodando localmente e anexar evidÃªncia em `specs/003-hierarquia-catalogo-herois/dados-minerados.md` (contagem 20, exemplos de habilidade, cobertura). (executado 2026-09-07 — seÃ§Ã£o 6 do dados-minerados.md com 8/8 cenÃ¡rios validados via InMemory)
- [x] T114 Atualizar `.specify/feature.json` apontando `status` para `implemented` quando os checkpoints das cinco US estiverem verdes. (criado `.specify/features/003-hierarquia-catalogo-herois.json` com status=implemented e checkpoints das 5 US)

---

## Dependencies

- Phase 1 (Setup) â€” sem dependÃªncias.
- Phase 2 (Foundational) â€” depende de Phase 1. **Bloqueia** todas as US.
- **US1 (P1)** â€” depende de Phase 2. Ã‰ prÃ©-requisito conceitual para US2, US3, US4 e US5 (define os tipos de domÃ­nio).
- **US2 (P2)** â€” depende de US1. Introduz `Classe`, `Habilidade` persistidos + endpoints. Bloqueia parcialmente US4 (que depende de `Classe` persistida com resistÃªncias).
- **US3 (P3)** â€” depende de US1. Independente de US2 no domÃ­nio, mas usa DI compartilhada. Pode rodar em paralelo com US2 se o time se dividir.
- **US4 (P4)** â€” depende de US1, US2 (para consultar `Classe`) e US3 (para validar equipamento).
- **US5 (P5)** â€” depende de US2 (Classe e Habilidade catalogadas) e da migraÃ§Ã£o de Personagem (US4) apenas se o cÃ¡lculo de cobertura referenciar dados de Personagem; caso contrÃ¡rio depende sÃ³ de US2.
- Phase 8 (Polish) â€” depende de todas as US.

## Parallel Execution Examples

### Phase 2 â€” Enums e Owned Types em paralelo

```text
T005 ClasseDeHeroi              (independente)
T006 TipoDeInimigo              (independente)
T007 Enums de Habilidade e Item (independente)
T008 Enums de Cobertura         (independente)
T009 EfeitoDeHabilidade          (depende T007)
T010 LimitePorUso                (depende T007)
T011 ResistenciasDeClasse        (independente)
T012 ResistenciasExtras          (independente)
T013 NivelDeArma                 (independente)
T014 NivelDeArmadura             (independente)
T015 EfeitoDeAcessorio           (depende T007)
T016 Inventario                  (independente)
T017 HabilidadeDePersonagem      (independente)
```

Grupos T005/T006/T007/T008/T011/T012/T013/T014/T016/T017 podem rodar em paralelo; T009/T010/T015 aguardam T007.

### US1 â€” Tipos de domÃ­nio em paralelo

```text
T026, T027, T028, T029, T030   (hierarquia de Habilidade)
T031, T032, T033, T034         (hierarquia de Item)
T035, T036                     (Personagem e Inimigo)
```

Cada bloco pode ser feito por uma pessoa em paralelo, pois toca arquivos diferentes.

### US2 â€” ServiÃ§os, repositÃ³rios e controllers em paralelo

```text
T046, T047                     (interfaces de Application)
T048, T049                     (DTOs e comandos)
T052, T053                     (repositÃ³rios)
T055, T056, T057               (controllers + contratos HTTP)
```

## Implementation Strategy

1. **MVP (US1)**: Introduzir a hierarquia no domÃ­nio com testes de arquitetura. Nada persiste, nada expÃµe endpoint. Entrega o alinhamento conceitual exigido pela clarificaÃ§Ã£o Q1.
2. **Incremento 1 (US2)**: Persistir `Classe` + `Habilidade` e expor consulta/criaÃ§Ã£o. Permite popular o catÃ¡logo real (mineraÃ§Ã£o parcial da wiki).
3. **Incremento 2 (US3)**: Persistir `Item` (TPH) e validar equipamento por classe. Habilita o uso real dos itens minerados.
4. **Incremento 3 (US4)**: Persistir `Personagem` e `Inimigo` com cÃ³pia de resistÃªncias. Fecha o loop de criaÃ§Ã£o de herÃ³is a partir do catÃ¡logo.
5. **Incremento 4 (US5)**: Ativar o Mapa de Cobertura, com estados explÃ­citos, para acompanhar o progresso da mineraÃ§Ã£o.
6. **Polish**: Testes de contrato OpenAPI, teste completo de DI e execuÃ§Ã£o ponta a ponta do quickstart.








---

## Phase 9: Convergence

- [x] T115 [HIGH] Gerar migração EF Core consolidada `AddCatalogoHeroisEEntidades` em `src/DarkestDungeon.Infrastructure/Data/Migrations/` cobrindo todas as tabelas novas (Classes, Habilidades TPH, ClassesHabilidades, Itens TPH, Personagens/Inimigos TPT sobre Ser, MapaDeCobertura) e todos os Owned Types (ResistenciasDeClasse, EfeitosHabilidadeCombate/Acampamento/Inimigo, LimitePorUso, NiveisArma, NiveisArmadura, EfeitosAcessorio, ResistenciasExtrasDePersonagem, HabilidadesDePersonagem), preservando OnDelete(Restrict) nas FKs de Classe/Habilidade/Item; validar aplicação via `dotnet ef database update` em ambiente SQL Server local. per plan.md Storage + Constitution II (partial)
- [x] T116 [HIGH] Popular `ClassesSeed.Materializar()` em `src/DarkestDungeon.Infrastructure/Data/Seeds/ClassesSeed.cs` com os oito valores oficiais de `ResistenciasDeClasse` (Atordoamento, Sangramento, Envenenamento, Debuff, Movimento, Doença, Golpe Mortal, Armadilha) para cada uma das 20 classes, extraídos da wiki `darkestdungeon.wiki.gg`; atualizar `MapaDeCoberturaSeed` para marcar as entradas correspondentes como `Coletado` em vez de `Pendente` quando o valor for real. per FR-014, SC-002, SC-007 (partial)
- [x] T117 [HIGH] Executar a mineração da wiki oficial e cadastrar via `POST /habilidades/{combate|acampamento}` (ou via seed dedicado) as ~140 habilidades das 20 classes, incluindo habilidades compartilhadas de acampamento como `Gallows Humor` (um único registro referenciado por Bandido e Ladrão de Cova); documentar cobertura resultante em `specs/003-hierarquia-catalogo-herois/dados-minerados.md` e atualizar Mapa de Cobertura marcando cada atributo minerado como `Coletado`. per US2/AC5, FR-009 (missing)
- [x] T118 [MEDIUM] Persistir `Inimigo.HabilidadesIds` em `src/DarkestDungeon.Infrastructure/Data/DarkestDungeonDbContext.cs` (remover `entity.Ignore(i => i.HabilidadesIds)` e adicionar `HasConversion` para JSON ou tabela dedicada `InimigoHabilidades`), garantindo que `POST /inimigos` com `HabilidadesIds` populado retenha os IDs após restart e que `InimigosEndpointsTests` valide a leitura round-trip. per data-model.md Inimigo (partial)
- [x] T119 [MEDIUM] Persistir `Personagem.Inventario` como Owned Type em `src/DarkestDungeon.Infrastructure/Data/DarkestDungeonDbContext.cs` (remover `entity.Ignore(p => p.Inventario)` e usar `OwnsOne` com `OwnsMany`/tabela `InventarioSlots`), garantindo 4 slots persistidos por Personagem. per data-model.md Personagem (partial)
- [x] T120 [MEDIUM] Refatorar `HabilidadeService.ObterAssociacoesAsync` em `src/DarkestDungeon.Application/Habilidades/HabilidadeService.cs` para substituir o loop N+1 (`ListarPorClasseAsync` por classe) por uma única consulta a `ClassesHabilidades` filtrada por `HabilidadeId`, honrando a meta de <200 ms do plan. per plan.md Performance Goals (partial)
- [x] T121 [MEDIUM] Estender `PersonagensEquipamentoTests` em `tests/DarkestDungeon.Api.Tests/PersonagensEquipamentoTests.cs` com cenário de armadura: (a) equipar armadura da mesma classe retorna 200; (b) equipar armadura de outra classe retorna 400 com `ErroResponse` PT-BR. per US3/AC2 (partial)
- [x] T122 [LOW] Estender `ContratoErroResponseTests` em `tests/DarkestDungeon.Api.Tests/ContratoErroResponseTests.cs` com um caso 400 para cada rota ainda descoberta (`POST /armaduras`, `POST /acessorios`, `POST /personagens`, `POST /inimigos`), afirmando `ErroResponse.mensagem` obrigatória em PT-BR. per FR-025 (partial)
- [x] T123 [LOW] Executar o quickstart end-to-end contra a API rodando localmente (SQL Server ou InMemory) e anexar a saÃ­da dos 8 cenÃ¡rios (contagem 20 classes, habilidade compartilhada, 6+6, habilidade fora de classe, criaÃ§Ã£o de Personagem, criaÃ§Ã£o de Arma, equipar, Mapa de Cobertura) em `specs/003-hierarquia-catalogo-herois/dados-minerados.md`. per plan.md Quickstart validaÃ§Ã£o (missing) (executado 2026-09-07 — idem T113; seÃ§Ã£o 6 do dados-minerados.md)

