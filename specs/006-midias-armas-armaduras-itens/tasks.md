---
description: "Tasks para Feature 006 — Importação e Vínculo de Mídias de Armas, Armaduras e Itens"
---

# Tasks: Importação e Vínculo de Mídias de Armas, Armaduras e Itens

**Input**: Design documents from `/specs/006-midias-armas-armaduras-itens/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Tests**: Obrigatórios pelo princípio constitucional III (contratos verificáveis) e pelos contract tests listados em `contracts/`. Incluídos em cada fase de user story.

**Organization**: Tarefas agrupadas por user story. US1 (P1) é o MVP. US2 e US4 são P2; US3 é P3.

**Clarificações 2026-09-10 (não negociar nas tasks)**:
- Q1: TPH com `ItemDeAcampamento` e `Consumivel`.
- Q2/Q3: vincular acessórios 003 **e** criar novos `Acessorio` (`Comum`, efeitos vazios) para trinkets só na instalação.
- Q4: classificar acampamento vs consumível **somente** por pasta (`mapeamento-pastas.json`).
- Q5: publicação a qualquer momento; **não** reutilizar `IPublicadorAtomicoService`; **não** 400 por sessão ativa.

## Format: `[ID] [P?] [Story?] Descrição`

- **[P]**: Pode rodar em paralelo (arquivos diferentes, sem dependências incompletas)
- **[Story]**: US1, US2, US3, US4 — só nas fases de user story
- Cada descrição contém o caminho exato do arquivo

## Path Conventions (Web-Service em camadas C# + CLI 004)

- Domain: `src/DarkestDungeon.Domain/`
- Application: `src/DarkestDungeon.Application/`
- Infrastructure: `src/DarkestDungeon.Infrastructure/`
- Api: `src/DarkestDungeon.Api/`
- CLI: `tools/DarkestDungeon.MediaCollector/`
- Tests: `tests/DarkestDungeon.Domain.Tests/`, `tests/DarkestDungeon.Api.Tests/`, `tests/DarkestDungeon.Architecture.Tests/`
- Specs: `specs/006-midias-armas-armaduras-itens/`
- Acervo: `assets/equipamentos-itens/` (006) e `assets/herois/` (004, só merge de hash)

---

## Phase 1: Setup (Infraestrutura Compartilhada)

**Purpose**: Ambiente e pastas da Feature 006 sem alterar regras de produção ainda.

- [X] T003 Criar `specs/006-midias-armas-armaduras-itens/notas-implementacao.md` com estado inicial (20 armas, 20 armaduras, seed 003 intocado, publicador 005 fora de escopo) — **primeiro**; T001/T002 escrevem neste arquivo
- [X] T001 [P] Verificar baseline: rodar `dotnet test --nologo --verbosity minimal` e anotar a contagem verde em `specs/006-midias-armas-armaduras-itens/notas-implementacao.md` (depois de T003)
- [X] T002 [P] Verificar Docker `mssql-dd` Up e `dotnet ef migrations list --project src/DarkestDungeon.Infrastructure --startup-project src/DarkestDungeon.Api`; registrar a migration head em `specs/006-midias-armas-armaduras-itens/notas-implementacao.md` (depois de T003)
- [X] T004 [P] Criar diretórios `assets/equipamentos-itens/arquivos/` e `tests/DarkestDungeon.Api.Tests/ColetaMidias/Fixtures/Equipamentos/` (subpastas `heroes/`, `inventory/trinkets/`, `inventory/provision/`, `inventory/quest/`, `inventory/raid/items/`, `inventory/misc/`)
- [X] T005 [P] Criar mapeamento default `tools/DarkestDungeon.MediaCollector/Configuracao/mapeamento-pastas.json` conforme tabela de `specs/006-midias-armas-armaduras-itens/research.md` (arma/armadura em `heroes/**`, trinkets, provision, quest, raid/items; DLC recursivo)
- [X] T006 Confirmar em `specs/006-midias-armas-armaduras-itens/notas-implementacao.md` que `src/DarkestDungeon.Application/Publicacao/IPublicadorAtomicoService.cs` **não** será alterado nem chamado pelo fluxo 006

**Checkpoint**: Baseline documentada, pastas e mapeamento prontos, sem mudança de domínio.

---

## Phase 2: Foundational (Bloqueadores para todas as user stories)

**Purpose**: Tipos, TPH e migration compartilhados. Nenhuma US de vínculo/API começa sem esta fase. US1 (CLI) pode usar só T005+T004, mas o checkpoint desta fase MUST completar antes de US2/US3/US4.

**⚠️ CRITICAL**: Completar esta fase antes de US2, US3 e US4.

- [X] T007 [P] Criar enum `StatusDeMidia` (`OK`, `Pendente`) em `src/DarkestDungeon.Domain/Itens/StatusDeMidia.cs`
- [X] T008 [P] Criar enum `CategoriaDeMidiaDeItem` (`Arma`, `Armadura`, `Acessorio`, `ItemDeAcampamento`, `Consumivel`) em `src/DarkestDungeon.Domain/Itens/CategoriaDeMidiaDeItem.cs`
- [X] T009 [P] Criar enum `StatusDeCoberturaDeItem` (`OK`, `Parcial`, `Pendente`) em `src/DarkestDungeon.Domain/Itens/StatusDeCoberturaDeItem.cs`
- [X] T010 Criar value object `MidiaDeItem` em `src/DarkestDungeon.Domain/Itens/MidiaDeItem.cs` (ArquivoInventarioId máx. 200, ConjuntoSpineId máx. 200, HashArquivo 64 hex quando presente, Status; invariante OK exige hash + arquivo)
- [X] T011 [P] Criar `ItemDeAcampamento` em `src/DarkestDungeon.Domain/Itens/ItemDeAcampamento.cs` herdando `Item` com `MidiaDeItem`
- [X] T012 [P] Criar `Consumivel` em `src/DarkestDungeon.Domain/Itens/Consumivel.cs` herdando `Item` com `MidiaDeItem`
- [X] T013 Estender `NivelDeArma` em `src/DarkestDungeon.Domain/Itens/NivelDeArma.cs` com owned `Midia` default `Pendente`, sem alterar dano/crítico/velocidade
- [X] T014 Estender `NivelDeArmadura` em `src/DarkestDungeon.Domain/Itens/NivelDeArmadura.cs` com owned `Midia` default `Pendente`, sem alterar HP/esquiva
- [X] T015 Estender `Acessorio` em `src/DarkestDungeon.Domain/Itens/Acessorio.cs` com `MidiaDeItem` default `Pendente`, sem mudar raridade/efeitos no construtor 003
- [X] T016 Atualizar comentário TPH em `src/DarkestDungeon.Domain/Itens/Item.cs` listando `ItemDeAcampamento` e `Consumivel`
- [X] T017 Mapear TPH + owned `Midia` em `src/DarkestDungeon.Infrastructure/Data/DarkestDungeonDbContext.cs` (`HasValue<ItemDeAcampamento>("ItemDeAcampamento")`, `HasValue<Consumivel>("Consumivel")`; colunas `Midia_*` em `NiveisArma`, `NiveisArmadura` e `Itens`)
- [X] T018 Criar entidade de rastreio `PublicacaoDeVinculosDeMidia` em `src/DarkestDungeon.Domain/Itens/PublicacaoDeVinculosDeMidia.cs` (Id, Categoria, Estado EmCurso/Concluida/RollbackAplicado, datas) e mapear tabela própria em `src/DarkestDungeon.Infrastructure/Data/DarkestDungeonDbContext.cs` — **não** reusar tabela/fluxo de `IPublicadorAtomicoService`
- [X] T019 Gerar migration incremental `{timestamp}_MidiasDeEquipamentoEItens` via `dotnet ef migrations add MidiasDeEquipamentoEItens --project src/DarkestDungeon.Infrastructure --startup-project src/DarkestDungeon.Api` (não consolidar schema 003/005; não `HasData` de armas)
- [X] T020 Definir `IPublicadorDeVinculosDeMidia` em `src/DarkestDungeon.Application/Midias/IPublicadorDeVinculosDeMidia.cs` (PublicarAsync por categoria, ObterStatusAsync; **sem** detector de sessões e **sem** `confirmacaoJanelaManutencao`)
- [X] T021 Definir `ICoberturaDeMidiasService` em `src/DarkestDungeon.Application/Midias/ICoberturaDeMidiasService.cs` e a porta `ILeitorDeInventarioDeMidias` em `src/DarkestDungeon.Application/Midias/ILeitorDeInventarioDeMidias.cs` (Application **não** usa `System.IO` no inventário)
- [X] T022 [P] Unit tests de invariantes `MidiaDeItem` em `tests/DarkestDungeon.Domain.Tests/Feature006/MidiaDeItemTests.cs` (hash ≠ 64 rejeita; OK sem arquivo rejeita; Pendente permite nulos)
- [X] T023 [P] Unit tests `tests/DarkestDungeon.Domain.Tests/Feature006/ItemDeAcampamentoEConsumivelTests.cs` (nome 80, descrição 400, tipos distintos)
- [X] T024 Aplicar migration no `mssql-dd` com `dotnet ef database update --project src/DarkestDungeon.Infrastructure --startup-project src/DarkestDungeon.Api` e confirmar `SELECT COUNT(*) FROM NiveisArma` = 100

**Checkpoint**: Domain + SQL prontos; publicador 005 intocado; user stories de vínculo podem começar.

---

## Phase 3: User Story 1 — Inventariar mídias locais (Priority: P1) 🎯 MVP

**Goal**: Descobrir, copiar byte a byte e registrar no inventário 006 (origem, destino, hash, tamanho, categoria, lacunas), reusando hash da 004, sem SQL e sem wiki.

**Independent Test**: Fixture com um PNG de arma, um de armadura e um de item → `inventario.json` com três entradas, hash idêntico à origem e categoria correta; pasta ausente vira lacuna e o restante continua.

### Testes US1 (contrato CLI)

- [X] T025 [P] [US1] Criar fixture PNG mínimos em `tests/DarkestDungeon.Api.Tests/ColetaMidias/Fixtures/Equipamentos/` (arma em `heroes/crusader/`, armadura, `inventory/trinkets/`, `inventory/provision/`, `inventory/quest/`, órfão em `inventory/misc/`)
- [X] T026 [P] [US1] Escrever testes de coletor em `tests/DarkestDungeon.Api.Tests/ColetaMidias/ImportadorDeEquipamentosTests.cs` cobrindo: cópia hash, dedupe, lacuna de pasta, `NaoAssociado`, provision≠consumivel, regressão `--classe` sem `--categoria`

### CLI e inventário

- [X] T027 [US1] Estender `OpcoesDoColetor` em `tools/DarkestDungeon.MediaCollector/Configuracao/OpcoesDoColetor.cs` com `--categoria` (repetível), `--mapeamento`, `--inventario-herois`; mensagens de erro PT-BR
- [X] T028 [US1] Estender `ArquivoImportado` e `LacunaDeImportacao` em `tools/DarkestDungeon.MediaCollector/Inventario/ModelosDeInventario.cs` com `Categoria` (incluir `NaoAssociado`)
- [X] T029 [US1] Implementar leitor da tabela pasta→tipo em `tools/DarkestDungeon.MediaCollector/Configuracao/MapeamentoDePastas.cs` (origens da research; DLC recursivo; arquivo fora do mapa = `NaoAssociado`; **não** classificar provision/consumível por filename)
- [X] T030 [US1] Implementar importação de equipamento em `tools/DarkestDungeon.MediaCollector/Importacao/ImportadorDeEquipamentos.cs` (cópia byte a byte, SHA-256, `--simular`, `--continuar` sempre para lacunas FR-005, destino `assets/equipamentos-itens/arquivos/{categoria}/`)
- [X] T031 [US1] Implementar merge de hash com `assets/herois/inventario.json` em `tools/DarkestDungeon.MediaCollector/Inventario/ReusoDeInventarioHerois.cs` (`Reutilizado=true`, sem segundo blob)
- [X] T032 [US1] Gravar `inventario.json` atômico com `DeclaracaoDeUso` (FR-015) em `tools/DarkestDungeon.MediaCollector/Inventario/ArmazenamentoDeInventario.cs` (estender o writer 004 ou overload 006 sem quebrar heróis)
- [X] T033 [US1] Ramificar `tools/DarkestDungeon.MediaCollector/Program.cs`: se `--categoria` presente, pipeline 006; senão comportamento 004 inalterado
- [X] T034 [US1] Rodar simulação e importação da fixture (`dotnet run --project tools/DarkestDungeon.MediaCollector -- --origem tests/DarkestDungeon.Api.Tests/ColetaMidias/Fixtures/Equipamentos --saida artifacts/midias-006 --continuar`) e anexar totais em `specs/006-midias-armas-armaduras-itens/notas-implementacao.md`

**Checkpoint US1**: MVP — inventário rastreável sem catálogo vinculado.

---

## Phase 4: User Story 2 — Vincular armas e armaduras por nível (Priority: P2)

**Goal**: Publicar vínculos dos 5 níveis das 20 armas e 20 armaduras (ícone PNG = OK; Spine opcional) sem mudar Ids nem números 003 nem equipamento de Personagens.

**Independent Test**: Publicar categoria `arma` para o Cruzado; GET `/itens/{id}` mostra 5 níveis numéricos iguais ao seed e `midia` OK ou Pendente; Personagem existente mantém `armaEquipadaId`.

### Testes US2

- [X] T035 [P] [US2] Contract tests de publicação em `tests/DarkestDungeon.Api.Tests/Feature006/PublicacaoVinculosEndpointsTests.cs` (202 arma; 400 categoria inválida; 409 mesma categoria; 409 inventário ausente; 202 com detector 005 > 0 sessões; Personagem IDs inalterados; 500 rollback)
- [X] T036 [P] [US2] Contract tests GET item/personagem em `tests/DarkestDungeon.Api.Tests/Feature006/ItensEPersonagemMidiaTests.cs` (5 níveis + midia; Pendente = 200; hash do DTO = inventário)

### Publicação e DTOs

- [X] T037 [US2] Criar DTOs de publicação em `src/DarkestDungeon.Application/Midias/PublicacaoDeVinculosDtos.cs` (categoria, observação máx. 500, status, totais)
- [X] T038 [US2] Implementar `LeitorDeInventarioDeMidias` em `src/DarkestDungeon.Infrastructure/Midias/LeitorDeInventarioDeMidias.cs` (`ILeitorDeInventarioDeMidias`) e `PublicadorDeVinculosDeMidia` em `src/DarkestDungeon.Infrastructure/Midias/PublicadorDeVinculosDeMidia.cs` (transação SQL por categoria; lock EmCurso **só** na mesma categoria; inventário **somente** via o leitor compartilhado, sem `System.IO` no publicador; mapeia PNG de arma/armadura ao nível 1–5; **zero** UPDATE em `Personagens`; **não** chama `IDetectorDeSessoesAtivas`)
- [X] T039 [US2] Associar nível de arma/armadura ao arquivo por classe + padrão `weapon`/`armour|armor` + índice de nível em `src/DarkestDungeon.Infrastructure/Midias/ResolvedorDeMidiaDeNivel.cs` (falha de arquivo → `Pendente`, não aborta a categoria)
- [X] T040 [US2] Estender `ArmaNivelDto`/`ArmaduraNivelDto`/`ItemDetalheDto` em `src/DarkestDungeon.Application/Itens/ItemDtos.cs` com objeto `midia`
- [X] T041 [US2] Atualizar `ItemMapper` em `src/DarkestDungeon.Application/Itens/ItemMapper.cs` para projetar `MidiaDeItem`
- [X] T042 [US2] Estender `PersonagemDetalheDto` em `src/DarkestDungeon.Application/Personagens/PersonagemDtos.cs` com `midiaArmaEquipada`, `midiaArmaduraEquipada`, `midiasAcessoriosEquipados`
- [X] T043 [US2] Atualizar `PersonagemMapper` em `src/DarkestDungeon.Application/Personagens/PersonagemMapper.cs` para projetar mídia com `nivelMidia = Clamp(map NivelDeResolucao → 1..5)` (0–1→1, 2→2, 3→3, 4→4, 5–6→5); sem colunas novas em Personagem; Pendente = status no DTO e HTTP 200
- [X] T044 [US2] Criar `src/DarkestDungeon.Api/Controllers/Midias/MidiasPublicacaoController.cs` com `POST /api/midias/publicacao` e `GET /api/midias/publicacao/{id}` conforme `specs/006-midias-armas-armaduras-itens/contracts/publicacao-vinculos.md` (sem campo de janela; 409 não 400 por sessão)
- [X] T045 [US2] Registrar `IPublicadorDeVinculosDeMidia` e `ILeitorDeInventarioDeMidias` em `src/DarkestDungeon.Api/Extensions/InfrastructureServiceCollectionExtensions.cs` **sem** remover o registro 005
- [X] T046 [US2] Garantir GET existente em `src/DarkestDungeon.Api/Controllers/Catalogo/ItensController.cs` devolve mídia via mapper (sem nova rota `/api/itens`)
- [X] T047 [US2] Validar SQL `NiveisArma`/`NiveisArmadura` ainda 100 linhas e Ids de seed 003 em `specs/006-midias-armas-armaduras-itens/notas-implementacao.md` após POST `categoria=arma` e `categoria=armadura`

**Checkpoint US2**: 20×5 vínculos de arma e armadura; Personagens intactos; publicação live permitida.

---

## Phase 5: User Story 4 — Relatório de cobertura (Priority: P2)

**Goal**: GET de cobertura das cinco categorias com `OK`/`Parcial`/`Pendente` em PT-BR e totais que somam `esperados` (armas/armaduras já significativos após US2; acessório/acampamento/consumível podem estar zerados até US3).

**Independent Test**: Importação parcial de armas → relatório com alguns `OK`, alguns `Pendente`/`Parcial`; soma = 20 na categoria Arma; textos em PT-BR.

### Testes US4

- [X] T048 [P] [US4] Contract tests em `tests/DarkestDungeon.Api.Tests/Feature006/CoberturaMidiasEndpointsTests.cs` (cinco categorias; filtro `categoria=arma`; query inválida 400 PT-BR; arma 5 OK; arma parcial; item sem mídia Pendente; órfãos não viram 6ª categoria)

### Relatório

- [X] T049 [US4] Criar records `RelatorioDeCoberturaDeMidias`, `ResumoDeCategoriaDeMidia`, `LinhaDeCoberturaDeItem` em `src/DarkestDungeon.Application/Midias/RelatorioDeCoberturaDeMidias.cs`
- [X] T050 [US4] Implementar `CoberturaDeMidiasService` em `src/DarkestDungeon.Application/Midias/CoberturaDeMidiasService.cs` (FR-013: arma/armadura OK só com 5 ícones; `Esperados` conta itens — Arma/Armadura = 20, não 100 vínculos; acessório/acampamento/consumível sem `Parcial`; órfãos em lista própria; lacunas via `ILeitorDeInventarioDeMidias`; **proibido** `System.IO` neste serviço)
- [X] T051 [US4] Criar `src/DarkestDungeon.Api/Controllers/Midias/MidiasCoberturaController.cs` com `GET /api/midias/cobertura` e `GET /api/midias/cobertura/categorias/{categoria}` conforme `specs/006-midias-armas-armaduras-itens/contracts/cobertura-midias.md`
- [X] T052 [US4] Registrar `ICoberturaDeMidiasService` em `src/DarkestDungeon.Api/Extensions/InfrastructureServiceCollectionExtensions.cs` (reusar `ILeitorDeInventarioDeMidias` já registrado em T045; não duplicar)
- [X] T053 [US4] Garantir strings de status e motivos de lacuna em PT-BR no serviço e nos contratos de erro em `src/DarkestDungeon.Application/Midias/CoberturaDeMidiasService.cs`

**Checkpoint US4**: Curador consulta cobertura sem varrer pastas.

---

## Phase 6: User Story 3 — Troféus, acampamento e consumíveis (Priority: P3)

**Goal**: Vincular acessórios 003; criar acessórios novos para trinkets ausentes (`Comum`, efeitos vazios); criar `ItemDeAcampamento` e `Consumivel` a partir das pastas mapeadas; órfãos permanecem no inventário.

**Independent Test**: Um acessório 003, uma tocha/provisão e um consumível devolvem mídia no GET; trinket sem match vira `Acessorio` novo; arquivo `inventory/misc` aparece em `orfaos`.

### Testes US3

- [X] T054 [P] [US3] Testes de match/criação de acessório em `tests/DarkestDungeon.Domain.Tests/Feature006/AcessorioNovoDeTrinketTests.cs` (Comum, efeitos vazios, sem classe/conjunto; 003 não recriado)
- [X] T055 [P] [US3] Contract tests em `tests/DarkestDungeon.Api.Tests/Feature006/ItensAcampamentoConsumivelEndpointsTests.cs` (POST sucesso; POST nome vazio 400 PT-BR; GET tipo discriminador; publicação acessorio cria extra sem alterar seed)
- [X] T056 [P] [US3] Estender `tests/DarkestDungeon.Api.Tests/Feature006/CoberturaMidiasEndpointsTests.cs` para `esperados` de acessório = 003 + novos e órfão em `orfaos`

### Catálogo e publicação

- [X] T057 [US3] Estender `TipoDeItemDto` em `src/DarkestDungeon.Application/Itens/ItemDtos.cs` com `ItemDeAcampamento` e `Consumivel`
- [X] T058 [US3] Adicionar commands/requests em `src/DarkestDungeon.Application/Itens/Commands/` e `src/DarkestDungeon.Api/Contracts/Catalogo/ItemContracts.cs` para criar acampamento e consumível (nome, descrição)
- [X] T059 [US3] Estender `IItemService`/`ItemService` em `src/DarkestDungeon.Application/Abstractions/IItemService.cs` e `src/DarkestDungeon.Application/Itens/ItemService.cs` com `CriarItemDeAcampamentoAsync` e `CriarConsumivelAsync`
- [X] T060 [US3] Adicionar POST `itens-acampamento` e `consumiveis` em `src/DarkestDungeon.Api/Controllers/Catalogo/ItensController.cs` (mesmo estilo de `ArmasController`, rotas sem `/api`)
- [X] T061 [US3] Implementar match de trinket por `NomeOriginal` (stem do arquivo, ignore case) em `src/DarkestDungeon.Infrastructure/Midias/ResolvedorDeAcessorios.cs`; hit = só `Midia`; miss = `new Acessorio(..., RaridadeDeAcessorio.Comum, efeitos vazios, classeExclusiva: null, conjuntoId: null, id: Guid.NewGuid())`
- [X] T062 [US3] Estender `PublicadorDeVinculosDeMidia` em `src/DarkestDungeon.Infrastructure/Midias/PublicadorDeVinculosDeMidia.cs` para categorias `acessorio`, `acampamento`, `consumivel` (upsert por NomeOriginal; pasta provision vs quest/raid conforme mapeamento; transação atômica; sem UPDATE Personagem)
- [X] T063 [US3] Atualizar `ItemMapper` em `src/DarkestDungeon.Application/Itens/ItemMapper.cs` para os dois tipos novos + `midia` no acessório
- [X] T064 [US3] Incluir órfãos `NaoAssociado` e lacunas de pasta em `CoberturaDeMidiasService` (`src/DarkestDungeon.Application/Midias/CoberturaDeMidiasService.cs`) sem criar 6ª categoria oficial
- [X] T065 [US3] Confirmar seed 003 de acessórios (Ids, raridade, efeitos) inalterado após publicação em `specs/006-midias-armas-armaduras-itens/notas-implementacao.md`

**Checkpoint US3**: Cinco categorias no catálogo visual; órfãos visíveis; 003 preservado.

---

## Phase 7: Polish & Cross-Cutting

**Purpose**: Constituição, regressão 004/005, SC amostrais.

- [X] T066 [P] Atualizar testes de arquitetura em `tests/DarkestDungeon.Architecture.Tests/` para Domain sem `System.IO` de instalação e Application sem EF de publicação 006 (Infrastructure only)
- [X] T067 [P] Ajustar asserts de igualdade estrita em `tests/DarkestDungeon.Api.Tests/ItensEndpointsTests.cs` e `tests/DarkestDungeon.Api.Tests/PersonagensEquipamentoTests.cs` para campos opcionais de mídia
- [X] T068 Garantir regressão CLI 004: `--classe Antiquarian` sem `--categoria` ainda importa heróis (`tools/DarkestDungeon.MediaCollector/Program.cs` + `tests/DarkestDungeon.Api.Tests/ColetaMidias/`)
- [X] T069 Verificar amostral SC-009: 10 arquivos de `assets/equipamentos-itens/inventario.json` só com categorias de equipamento/item; anotar em `specs/006-midias-armas-armaduras-itens/notas-implementacao.md`
- [X] T070 Rodar `dotnet test --nologo --verbosity minimal` e confirmar baseline 005/003 verde + novos Feature006
- [X] T071 Percorrer `specs/006-midias-armas-armaduras-itens/quickstart.md` (cenários 1–7) e marcar resultados em `specs/006-midias-armas-armaduras-itens/notas-implementacao.md`
- [X] T072 Revisar mensagens de API/CLI em PT-BR (`OK`/`Parcial`/`Pendente`) contra SC-010 em controllers `src/DarkestDungeon.Api/Controllers/Midias/`

**Checkpoint final**: Feature 006 implementável e verificável; 005 intocada.

---

## Dependencies

```text
Phase 1 (Setup)
    └── Phase 2 (Foundational: MidiaDeItem, TPH, migration, interfaces)
            ├── Phase 3 US1 P1 CLI inventário  ─── MVP
            │       └── Phase 4 US2 P2 vínculos arma/armadura
            │               ├── Phase 5 US4 P2 cobertura
            │               └── Phase 6 US3 P3 acessório/acampamento/consumível
            │                       └── (US4 já existente ganha as 5 categorias completas)
            └── Phase 7 Polish (após US em entrega; mínimo após MVP+US2)
```

- Phase 1: T003 **antes** de T001/T002 (arquivo de notas); T001 // T002 depois; T004 // T005; T006 por último.
- US1 **não** depende de SQL (só T004–T005 + código CLI); na prática esperar T001–T006.
- US2 depende de Phase 2 + inventário US1.
- US4 depende de US2 para números significativos de arma/armadura; contrato das 5 categorias já responde zeros até US3.
- US3 depende de Phase 2 + US1; pode seguir em paralelo a US4 depois de US2.
- **Proibido**: modificar `IPublicadorAtomicoService` ou exigir janela 005.

## Parallel execution examples

**Phase 2**: T007, T008, T009 em paralelo; T011 // T012; T022 // T023 após T010–T012.

**US1**: T025 // T026 (fixtures + esqueleto de teste); depois T027–T033 sequenciais no CLI.

**Phase 1**: T003 primeiro; depois T001 // T002 e T004 // T005.

**US2**: T035 // T036 (testes contrato); T040 // T042 (DTOs); T038 (leitor + publicador) bloqueia T044/T045.

**US3**: T054 // T055 // T056; T057–T060 depois; T061 antes de T062.

**US4**: T048 em paralelo ao esqueleto T049; T050 bloqueia T051.

## Implementation strategy

1. **MVP**: Phase 1 + US1 — curador já tem inventário e lacunas.
2. **Incremento jogável**: US2 — ícones de arma/armadura no GET de item/personagem.
3. **Visibilidade**: US4 — relatório para priorizar Pendente/Parcial.
4. **Completar catálogo**: US3 — trinkets novos + provisões + consumíveis.
5. **Polish**: regressão 004/005, arquitetura, quickstart.

Não implementar nesta geração de tasks. Próximo comando: `/speckit-implement` (ou `/speckit-analyze` se quiser consistência spec/plan/tasks antes).

---

## Phase 8: Convergence

- [X] T073 Passar o inventário completo da categoria (PNG + `.atlas`/`.skel`) para `ResolvedorDeMidiaDeNivel` em `src/DarkestDungeon.Infrastructure/Midias/PublicadorDeVinculosDeMidia.cs` e preencher `MidiaDeItem.ConjuntoSpineId` quando o conjunto animado existir, sem exigir Spine para `OK` e sem abortar a categoria se o conjunto faltar, per FR-006, FR-007, US2/AC2, plan: PNG obrigatório Spine opcional (`partial`)
