---
description: "Task list for Ícones de Habilidades de Acampamento"
---

# Tasks: Ícones de Habilidades de Acampamento

**Input**: Design documents from `specs/011-icones-acampamento/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/coleta-camping.md`, `quickstart.md`

## Phase 1: Setup

- [x] T001 Add `--camping` option parsing in `tools/DarkestDungeon.MediaCollector/Configuracao/OpcoesDoColetor.cs` (reject combo with `--categoria`).
- [x] T002 [P] Add CLI contract tests in `tests/DarkestDungeon.Api.Tests/ColetaMidias/ContratoImportadorCliTests.cs`.

## Phase 2: Foundational

- [x] T003 Create alias table `tools/DarkestDungeon.MediaCollector/Inventario/AliasesDeCamping.cs`.
- [x] T004 Extend `ArmazenamentoDeInventario` to merge camping files into existing 004 JSON without dropping prior `Arquivos`.
- [x] T005 Wire `--camping` in `tools/DarkestDungeon.MediaCollector/Program.cs`.

## Phase 3: US1 — Importar PNGs

- [x] T006 [US1] Implement `tools/DarkestDungeon.MediaCollector/Importacao/ImportadorDeCamping.cs` (vanilla + dlc glob, copy to `arquivos/acampamento/`, lacunas).
- [x] T007 [US1] Tests in `tests/DarkestDungeon.Api.Tests/ColetaMidias/ImportadorDeCampingTests.cs` (copy bytes, merge 004, missing folder lacuna, simulate).

## Phase 4: US2 — Manifesto

- [x] T008 [US2] Implement manifesto append in `tools/DarkestDungeon.MediaCollector/Inventario/GeradorDoManifestoDeCamping.cs`.
- [x] T009 [US2] Tests covering 79 associations, aliases, leftovers excluded, combat associations preserved.

## Phase 5: US3 — Card

- [x] T010 [US3] Relax `TentarArquivoDeHabilidade` for `camp_skill_*.png` in `src/DarkestDungeon.Infrastructure/Midias/ResolvedorDeMidiasDoCard.cs`.
- [x] T011 [US3] Update `tests/DarkestDungeon.Api.Tests/Feature010/PersonagensCardHabilidadesMidiaTests.cs` and add `tests/DarkestDungeon.Api.Tests/Feature011/PersonagensCardAcampamentoMidiaTests.cs`.

## Phase 6: Polish

- [x] T012 Run collector against Steam install into `assets/herois` with `--camping --continuar`.
- [x] T013 [P] Update `specs/011-icones-acampamento/quickstart.md` with executed results.
- [x] T014 Run `dotnet test` for ColetaMidias + Feature010 + Feature011; fix regressions.

---

## Dependencies

T001–T005 before US1. T006 before T012. T008 before T010. T012 before T011 if tests need real assets (tests should use fixtures; live collect is polish).
