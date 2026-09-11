# Implementation Plan: Imagens no Card do Personagem

**Branch**: `010-imagens-card-personagem` | **Date**: 2026-09-11 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from [spec.md](spec.md)

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

Mostrar no card da listagem o retrato e o corpo inteiro estáticos da aparência A–D, os ícones de arma e armadura no nível cadastrado (1–5) e o ícone único de cada habilidade já listada (inclusive Nv. 0). Sem importar mídia nova: resolver o acervo 004/006 e os vínculos 005/006 na Application, expor URLs `/acervo/...` no `GET /personagens` e renderizar no HTML/JS existente, com espaço reservado e texto **sem imagem** quando faltar arquivo.

## Technical Context

**Language/Version**: C# / .NET 10 (LangVersion 14)

**Primary Dependencies**: ASP.NET Core, EF Core, SQL Server, xUnit, FluentAssertions; UI estática HTML/CSS/JS em `wwwroot/personagens`

**Storage**: SQL Server existente (sem migração). Arquivos em `assets/herois/` e `assets/equipamentos-itens/` servidos como estáticos. InMemory nos testes de API.

**Testing**: API (contrato da lista/detalhe + estático `/acervo`) e regressão 009 do card; Architecture se novos tipos cruzarem camadas. Sem teste de recorte de sprite.

**Target Platform**: API .NET hospedada; navegador moderno em `/personagens/index.html`

**Project Type**: Serviço web em camadas + frontend estático (sem CLI nova)

**Performance Goals**: Um `GET /personagens` monta o card (sem N+1); lista em até 3 s no volume atual de heróis; arquivos estáticos fora do JSON

**Constraints**: Constituição 1.0.0; PT-BR **sem imagem**; FR-009 sem import/publicação; sem Spine no card; sem recálculo de personagens; equipamento pelo nível cadastrado, não pela resolução; ícone de habilidade independente do nível

**Scale/Scope**: 20 classes × 4 paletas (retrato + idle) + 20 × 5 arma + 20 × 5 armadura + ícones de combate do manifesto; 1 card; 2 prefixos `/acervo`

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Princípio | Verificação | Status |
|---|---|---|
| I. Arquitetura em camadas | Domain inalterado (sem path de disco). Application: resolvedor + DTOs. Infrastructure: leitura de inventário/manifesto já existente. API: static files + contratos. UI: apresentação. Sem regra no controller. | PASS |
| II. SQL Server | Sem migração; leitura de `Personagem`, `Classe.Assets`, `MidiaDeItem`. Arquivos não substituem o banco. | PASS |
| III. Contratos verificáveis | `GET /personagens`, `GET /personagens/{id}`, `GET /acervo/...` com sucesso, Pendente e 404 de arquivo. | PASS |
| IV. PT-BR | Texto canônico **sem imagem**; rótulos Retrato / Corpo inteiro / Arma / Armadura. | PASS |
| V. Operação remota e simplicidade | Stateless; reuso de acervo; static files em vez de streamer/base64; sem CLI. | PASS |

**Gate status**: PASS.

## Project Structure

### Documentation (this feature)

```text
specs/010-imagens-card-personagem/
├── plan.md
├── spec.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── personagens-card-midias.md
├── checklists/
│   └── requirements.md
└── tasks.md                 # /speckit-tasks — NÃO criado aqui
```

### Source Code (repository root)

```text
src/
├── DarkestDungeon.Application/
│   ├── Personagens/         # DTO/mapper/lista: bloco midias + habilidades[].midia
│   └── Midias/              # resolvedor de slot (retrato, idle, eqp, ability)
├── DarkestDungeon.Infrastructure/
│   └── Midias/              # reuso LeitorDeInventario; manifesto 004 se necessário
├── DarkestDungeon.Api/
│   ├── Program.cs           # UseStaticFiles /acervo/herois e /acervo/equipamentos-itens
│   └── wwwroot/personagens/ # card: slots + onerror “sem imagem”
└── DarkestDungeon.Domain/   # sem schema novo

tests/
├── DarkestDungeon.Api.Tests/Feature010/
└── DarkestDungeon.Architecture.Tests/
```

**Structure Decision**: Quatro projetos da solução. Sem quinto runtime e sem estender o MediaCollector. Acervo permanece em `assets/`.

## Phase 0: Research Summary

Decisões em [research.md](research.md): lista autossuficiente; retrato = `portrait_roster` (`AssetsDeClasse`); corpo = `sprite.idle.png` da paleta; arma/armadura = nível do personagem (índice arquivo = N−1); combate via manifesto; acampamento Pendente se não importado; URLs `/acervo/`; sem migração.

Não restam itens NEEDS CLARIFICATION.

## Phase 1: Design Summary

- Composição do card: [data-model.md](data-model.md)
- Lista, detalhe e estático: [contracts/personagens-card-midias.md](contracts/personagens-card-midias.md)
- Validação: [quickstart.md](quickstart.md)

## Implementation Phases

### Phase A: Contrato da lista e resolvedor (US1 + US2)

Estender `PersonagemResumoDto` / mapper / `ListarAsync` com retrato, corpo, arma e armadura. Resolvedor lê `AssetsDeClasse` + inventário 004 + `MidiaDeItem` no **nível cadastrado**. Alinhar detalhe 006 para não usar `NivelDeMidia(resolução)`. Mapear static files `/acervo/*`.

### Phase B: Ícones de habilidade (US3)

Acrescentar `midia` em cada habilidade listada (combate via manifesto; acampamento Pendente se ausente). Mesmo URL em qualquer `numeroDoNivel`.

### Phase C: Card UI

Slots rotulados em `personagens.js` / CSS; `onerror` e `Pendente` → **sem imagem**; não cobrir ações nem texto 009.

### Phase D: Testes

Contratos Feature010 + regressão 009 + 404 `/acervo`. Quickstart no host `http://localhost:5140`.

## Constitution Check (Post-Design)

- **Camadas**: PASS. Inventário só na Infrastructure; Application recebe ids/URLs relativas.
- **SQL Server**: PASS. Sem bytes no banco; sem migração injustificada.
- **Contratos**: PASS. Sucesso, Pendente e arquivo 404 documentados.
- **PT-BR**: PASS.
- **Simplicidade**: PASS. Static files + lookup; sem publicador, sem CLI, sem Spine.

**Post-design gate status**: PASS.

## Complexity Tracking

Nenhuma violação constitucional.
