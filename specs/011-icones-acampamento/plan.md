# Implementation Plan: Ícones de Habilidades de Acampamento

**Branch**: `011-icones-acampamento` | **Date**: 2026-09-11 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from [spec.md](spec.md)

## Summary

Estender o MediaCollector para copiar `camp_skill_*.png` (vanilla + DLC) para `assets/herois/arquivos/acampamento/`, mesclar o `inventario.json` da 004, acrescentar associações de acampamento ao `manifesto-habilidades.json` (aliases wiki→id do jogo) e fazer o resolvedor do card achar esses PNGs mesmo fora da pasta da classe. Sem `panels/`. Sem migração SQL.

## Technical Context

**Language/Version**: C# / .NET 10

**Primary Dependencies**: MediaCollector CLI existente; ASP.NET Core (static `/acervo/herois`); xUnit

**Storage**: Arquivos em `assets/herois/` (inventário JSON + PNG). SQL Server inalterado.

**Testing**: ColetaMidias (CLI/import/manifesto) + Feature010/011 (card acampamento OK). Regressão combate.

**Target Platform**: CLI local + API que serve acervo estático

**Project Type**: Ferramenta CLI + serviço web em camadas

**Performance Goals**: Coleta de ~84 PNG em segundos; GET /personagens sem N+1 extra

**Constraints**: Constituição 1.0.0; merge do inventário 004; FR-011 sem panels; PT-BR “sem imagem”; bytes originais

**Scale/Scope**: 79 skills de acampamento; ~68 PNG vanilla + 16 DLC; 5 leftovers não catalogados

## Constitution Check

| Princípio | Verificação | Status |
|---|---|---|
| I. Camadas | Coleta na CLI (tools). Resolvedor na Infrastructure (já lê inventário). Application inalterada além do que o resolvedor já expõe. Domain sem path. | PASS |
| II. SQL Server | Sem migração. | PASS |
| III. Contratos | Testes CLI + GET /personagens com acampamento OK e combate estável. | PASS |
| IV. PT-BR | Mensagens de lacuna/CLI em PT-BR; card “sem imagem”. | PASS |
| V. Simplicidade | Mesmo executável `--camping`; sem segundo CLI nem endpoint de import. | PASS |

**Gate status**: PASS.

## Project Structure

### Documentation (this feature)

```text
specs/011-icones-acampamento/
├── plan.md
├── spec.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── coleta-camping.md
├── checklists/
│   └── requirements.md
└── tasks.md
```

### Source Code

```text
tools/DarkestDungeon.MediaCollector/
  Program.cs
  Configuracao/OpcoesDoColetor.cs
  Importacao/ImportadorDeCamping.cs
  Inventario/AliasesDeCamping.cs
  Inventario/GeradorDoManifestoDeCamping.cs
  Inventario/ArmazenamentoDeInventario.cs   # merge
src/DarkestDungeon.Infrastructure/Midias/ResolvedorDeMidiasDoCard.cs
tests/DarkestDungeon.Api.Tests/ColetaMidias/
tests/DarkestDungeon.Api.Tests/Feature011/
assets/herois/inventario.json
assets/herois/manifesto-habilidades.json
assets/herois/arquivos/acampamento/
```

## Implementation phases

- Phase 0: research (aliases, pastas DLC, merge inventário)
- Phase 1: data-model, contracts, quickstart
- Phase 2: tasks.md
- Implement: CLI `--camping`, merge, manifesto, resolvedor, testes, execução real na Steam
