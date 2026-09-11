# Implementation Plan: Corpo Inteiro Composto e Animado

**Branch**: `012-corpo-inteiro-animado` | **Date**: 2026-09-11 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from [spec.md](spec.md)

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

Substituir a folha de partes no espaço “Corpo inteiro” do card pelo herói **composto** via runtime Spine 2.1, com seletor **Em espera** / **Animado** / **Caminhada**. Sem importar mídia: atlas+skel da classe (004) + PNG da paleta A–D. Escolha só na listagem aberta; caminhada no lugar. Estender `GET /personagens` (`midias.corpoInteiro`) e o HTML/JS existente; MIME `.atlas`/`.skel` no static `/acervo`.

## Technical Context

**Language/Version**: C# / .NET 10 (LangVersion 14); JavaScript no `wwwroot/personagens`

**Primary Dependencies**: ASP.NET Core, EF Core, SQL Server, xUnit, FluentAssertions; runtime Spine **2.1** vendido em `wwwroot/personagens/vendor/` (skel `2.1.27`)

**Storage**: SQL Server existente (sem migração). Arquivos 004 em `assets/herois/` servidos em `/acervo/herois`. InMemory nos testes de API.

**Testing**: API (contrato do conjunto idle/walk + MIME atlas/skel + regressão 010/011); Architecture se novos tipos cruzarem camadas. Roteiro de navegador no quickstart (composição, seletor, recarregar, recorte). Sem teste automatizado de pixels do WebGL.

**Target Platform**: API .NET hospedada; navegador moderno em `/personagens/index.html`

**Project Type**: Serviço web em camadas + frontend estático (sem CLI nova)

**Performance Goals**: Um `GET /personagens` basta para o card; troca de versão no card em até 3 s (SC-003); listagem rolável com vários players (idle pausado por padrão)

**Constraints**: Constituição 1.0.0; FR-009 sem import; FR-010 sem wiki; PT-BR **sem imagem** / rótulos do seletor; versão não persistida; ataque fora de escopo; forma humana apenas

**Scale/Scope**: 20 classes × 4 paletas; 2 ciclos (idle, walk) × (atlas+skel compartilhados + PNG paleta); 1 card; 1 runtime Spine 2.1

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Princípio | Verificação | Status |
|---|---|---|
| I. Arquitetura em camadas | Domain inalterado. Application: DTO do conjunto + mapper. Infrastructure: lookup atlas/skel/PNG no inventário. API: MIME estático. UI: player + seletor. Sem regra no controller. | PASS |
| II. SQL Server | Sem migração; leitura de `Personagem.Classe`/`Aparencia`. Arquivos não substituem o banco. | PASS |
| III. Contratos verificáveis | `GET /personagens` (conjunto, versões, Pendente), `GET /acervo/...atlas|skel|png` 200/404. Regressão 010/011. | PASS |
| IV. PT-BR | Rótulos **Em espera**, **Animado**, **Caminhada**; **sem imagem**. | PASS |
| V. Operação remota e simplicidade | Stateless; acervo local; static files; vendor Spine 2.1 justificado pelo skel nativo (sem CDN, sem 5º projeto). | PASS |

**Gate status**: PASS.

## Project Structure

### Documentation (this feature)

```text
specs/012-corpo-inteiro-animado/
├── plan.md
├── spec.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── personagens-corpo-inteiro.md
├── checklists/
│   └── requirements.md
└── tasks.md                 # /speckit-tasks — NÃO criado aqui
```

### Source Code (repository root)

```text
src/
├── DarkestDungeon.Application/
│   └── Midias/              # DTO conjunto + versões no slot corpo
├── DarkestDungeon.Infrastructure/
│   └── Midias/              # ResolvedorDeMidiasDoCard: idle/walk atlas+skel+PNG paleta
├── DarkestDungeon.Api/
│   ├── Program.cs           # MIME .atlas / .skel no acervo
│   └── wwwroot/personagens/
│       ├── personagens.js   # seletor + player por card
│       ├── personagens.css  # recorte, overflow, seletor
│       └── vendor/spine-2.1/  # runtime vendido
└── DarkestDungeon.Domain/   # sem schema novo

tests/
├── DarkestDungeon.Api.Tests/Feature012/
└── DarkestDungeon.Architecture.Tests/   # se novos tipos
```

**Structure Decision**: Quatro projetos da solução. Sem quinto runtime e sem estender o MediaCollector. Acervo permanece em `assets/`. Runtime Spine só no frontend estático.

## Phase 0: Research Summary

Decisões em [research.md](research.md): Spine 2.1 no card; atlas/skel na pasta da classe e PNG na paleta; idle = espera+animado, walk = caminhada; contrato enriquece o slot 010; MIME estático; estado do seletor só no cliente; walk no lugar.

Não restam itens NEEDS CLARIFICATION.

## Phase 1: Design Summary

- Conjuntos e versões: [data-model.md](data-model.md)
- Lista, detalhe e estático: [contracts/personagens-corpo-inteiro.md](contracts/personagens-corpo-inteiro.md)
- Validação: [quickstart.md](quickstart.md)

## Implementation Phases

### Phase A: Contrato do conjunto (US1)

Estender `SlotDeMidiaDoCardDto` / mapper / `ResolverCorpoInteiro` com `conjuntoIdle`, `conjuntoWalk` e `versoes`. `status` OK só com idle completo. Mapear MIME `.atlas`/`.skel`. Testes Feature012 + regressão 010 (`url` ainda `sprite.idle` + paleta).

### Phase B: Player e seletor (US2)

Vender Spine 2.1. No card: canvas no espaço “Corpo inteiro”; seletor PT-BR; padrão Em espera (skel idle pausado); Animado = loop idle; Caminhada = loop walk no lugar. Estado por `id` em memória; recarregar zera.

### Phase C: Card utilizável (US3)

Overflow hidden; canvas não cobre retrato/texto/ações; viewport estreita. Falha do player → **sem imagem**.

### Phase D: Testes e quickstart

Contratos Feature012 + estático atlas/skel + regressão 009–011. Quickstart no host `http://localhost:5140`.

## Constitution Check (Post-Design)

- **Camadas**: PASS. Inventário só na Infrastructure; Application recebe URLs relativas; player só na UI.
- **SQL Server**: PASS. Sem bytes no banco; sem migração.
- **Contratos**: PASS. Sucesso, Pendente, MIME e 404 documentados.
- **PT-BR**: PASS.
- **Simplicidade**: PASS. Vendor 2.1 + lookup; sem CLI, sem CDN, sem GIF.

**Post-design gate status**: PASS.

## Complexity Tracking

Nenhuma violação constitucional. O runtime Spine 2.1 não é um 5º projeto: é um script estático no `wwwroot` exigido pelo formato nativo do acervo 004.
