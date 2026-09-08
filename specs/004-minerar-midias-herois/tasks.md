# Tasks: Importação de Mídias Originais dos Heróis

**Input**: [spec.md](spec.md), [plan.md](plan.md), [research.md](research.md), [data-model.md](data-model.md), [coletor-cli.md](contracts/coletor-cli.md)

**Tests**: Testes xUnit com fixtures locais são obrigatórios para cópia byte a byte, descoberta, atlas e manifesto.

## Phase 1: Setup

- [X] T001 Remover `AngleSharp` e referências de coleta HTTP do projeto em `tools/DarkestDungeon.MediaCollector/DarkestDungeon.MediaCollector.csproj`
- [X] T002 [P] Remover fixtures HTML e testes de coleta web em `tests/DarkestDungeon.Api.Tests/ColetaMidias/Fixtures/Galerias/` e `tests/DarkestDungeon.Api.Tests/ColetaMidias/ColetorDeGaleriasTests.cs`
- [X] T003 [P] Criar fixtures Spine mínimas PNG, atlas e skel em `tests/DarkestDungeon.Api.Tests/ColetaMidias/Fixtures/Spine/`
- [X] T004 Criar o manifesto inicial de associações em `assets/herois/manifesto-habilidades.json` via `--gerar-manifesto`

## Phase 2: Fundação

- [X] T005 Expor a projeção somente leitura de classes e habilidades em `src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.cs`
- [X] T006 [P] Redefinir argumentos para `--origem`, `--saida`, `--classe`, `--manifesto`, `--simular` e `--continuar` em `tools/DarkestDungeon.MediaCollector/Configuracao/OpcoesDoColetor.cs`
- [X] T007 [P] Redefinir os registros do inventário para arquivo importado, conjunto Spine, região, manifesto e lacuna em `tools/DarkestDungeon.MediaCollector/Inventario/ModelosDeInventario.cs`
- [X] T008 Implementar descoberta recursiva de diretórios `heroes` base e DLC em `tools/DarkestDungeon.MediaCollector/Importacao/ImportadorLocal.cs`
- [X] T009 Implementar cópia byte a byte, hash e destino legível por classe/conjunto em `tools/DarkestDungeon.MediaCollector/Importacao/ImportadorLocal.cs`
- [X] T010 Implementar leitura segura de atlas e extração de regiões em `tools/DarkestDungeon.MediaCollector/Spine/LeitorDeAtlasSpine.cs`
- [X] T011 Implementar escrita atômica do inventário e leitura para `--continuar` em `tools/DarkestDungeon.MediaCollector/Inventario/ArmazenamentoDeInventario.cs`

## Phase 3: User Story 1 - Importar imagens das habilidades (P1)

**Independent Test**: Uma fixture de classe é importada, preservando bytes da textura e registrando associações declaradas ou lacunas de habilidade.

- [X] T012 [P] [US1] Criar testes de cópia byte a byte, hash e deduplicação em `tests/DarkestDungeon.Api.Tests/ColetaMidias/ImportadorDeArquivosTests.cs`
- [X] T013 [P] [US1] Criar testes de carregamento e validação do manifesto em `tests/DarkestDungeon.Api.Tests/ColetaMidias/ManifestoDeHabilidadesTests.cs`
- [X] T014 [US1] Implementar leitura e validação do manifesto de associação em `tools/DarkestDungeon.MediaCollector/Inventario/ManifestoDeHabilidades.cs`
- [X] T015 [US1] Implementar associação de região/conjunto a habilidades do catálogo e lacunas não especulativas em `tools/DarkestDungeon.MediaCollector/Importacao/ImportadorLocal.cs`
- [X] T016 [US1] Integrar a importação de texturas e associações de habilidade em `tools/DarkestDungeon.MediaCollector/Importacao/ImportadorLocal.cs`

## Phase 4: User Story 2 - Preservar animações Spine e paletas (P2)

**Independent Test**: Um trio de fixture PNG/atlas/skel e uma variante de paleta são importados sem mudança de bytes e aparecem como conjunto no inventário.

- [X] T017 [P] [US2] Criar teste de conjunto Spine completo em `tests/DarkestDungeon.Api.Tests/ColetaMidias/ConjuntoSpineTests.cs`
- [X] T018 [P] [US2] Criar testes de descoberta de heróis base e DLC em `tests/DarkestDungeon.Api.Tests/ColetaMidias/DescobridorDeHeroisTests.cs`
- [X] T019 [US2] Implementar agrupamento de PNG, atlas e skel e inferência de estado/paleta em `tools/DarkestDungeon.MediaCollector/Spine/ConstrutorDeConjuntosSpine.cs`
- [X] T020 [US2] Integrar descoberta de DLC e importação de conjuntos no fluxo em `tools/DarkestDungeon.MediaCollector/Importacao/ImportadorLocal.cs`

## Phase 5: User Story 3 - Auditar a origem do acervo (P3)

**Independent Test**: Um arquivo importado pode ser rastreado do destino à origem, classe, conjunto, hash e associações; um DLC ausente aparece como lacuna.

- [X] T021 [P] [US3] Criar testes de inventário, proveniência local, retomada e escrita atômica em `tests/DarkestDungeon.Api.Tests/ColetaMidias/InventarioDeMidiasTests.cs`
- [X] T022 [P] [US3] Criar testes de instalação inválida e simulação em `tests/DarkestDungeon.Api.Tests/ColetaMidias/ContratoImportadorCliTests.cs`
- [X] T023 [US3] Implementar resumo por classe de conjuntos, cópias, reutilizações e lacunas em `tools/DarkestDungeon.MediaCollector/Importacao/ImportadorLocal.cs`
- [X] T024 [US3] Integrar opções, simulação e importação no ponto de entrada em `tools/DarkestDungeon.MediaCollector/Program.cs`

## Phase 6: Polish

- [X] T025 Executar testes da solução e registrar o resultado em `specs/004-minerar-midias-herois/quickstart.md`
- [X] T026 Executar simulação da Antiquarian usando a instalação local e registrar conjuntos descobertos em `specs/004-minerar-midias-herois/quickstart.md`
- [X] T027 Executar importação real da Antiquarian e verificar hashes origem/destino em `specs/004-minerar-midias-herois/quickstart.md`
- [X] T028 Executar importação de toda a instalação, revisar lacunas de DLC/conjunto/habilidade e atualizar a cobertura em `specs/004-minerar-midias-herois/quickstart.md`

## Dependencies & Strategy

- T001-T011 criam a base local e bloqueiam as histórias.
- US1 (T012-T016) é o MVP: importa texturas e associações declaradas.
- US2 (T017-T020) acrescenta conjuntos Spine, paletas e DLC sem reescrever US1.
- US3 (T021-T024) finaliza rastreabilidade e retomada.
- T025-T028 validam primeiro uma classe e depois toda a instalação.

## Phase 7: Convergence

- [X] T029 Vincular `ConjuntoSpine` aos arquivos importados, hashes e classe PT-BR no inventário em `tools/DarkestDungeon.MediaCollector/Spine/ConstrutorDeConjuntosSpine.cs` (FR-004, FR-007; partial)
- [X] T030 Recriar o manifesto PT-BR e validar associações de habilidades ou lacunas no inventário em `assets/herois/manifesto-habilidades.json` e `tools/DarkestDungeon.MediaCollector/Inventario/GeradorDoManifesto.cs` (FR-005; missing)
- [X] T031 Deduplicar cópias por SHA-256 e registrar reutilizações em `tools/DarkestDungeon.MediaCollector/Importacao/ImportadorLocal.cs` (FR-008; missing)
- [X] T032 Produzir resumo estruturado por classe de conjuntos, copiados, reutilizados e lacunas em `tools/DarkestDungeon.MediaCollector/Importacao/ImportadorLocal.cs` (FR-009, FR-010, SC-005; partial)
- [X] T033 Declarar instalação local licenciada e proibição de redistribuição no inventário em `tools/DarkestDungeon.MediaCollector/Importacao/ImportadorLocal.cs` (FR-011; missing)
- [X] T034 Implementar leitura do inventário para `--continuar` em `tools/DarkestDungeon.MediaCollector/Importacao/ImportadorLocal.cs` (FR-007; partial)
- [X] T035 Adicionar testes de manifesto, descoberta DLC, conjunto incompleto, inventário e retomada em `tests/DarkestDungeon.Api.Tests/ColetaMidias/` (plan: Testing; missing)

## Phase 8: Convergence

- [X] T036 Registrar `tentadoEmUtc` em cada lacuna gerada pelo importador em `tools/DarkestDungeon.MediaCollector/Importacao/ImportadorLocal.cs` (FR-009; missing)
