# Implementation Plan: Atributos Oficiais do Personagem

**Branch**: `009-atributos-oficiais-personagem` | **Date**: 2026-09-10 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from [spec.md](spec.md)

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

Completar o catálogo **local** com tabelas oficiais de arma/armadura (5 níveis), deslocamento frente/atrás e extras da infobox (religiosa, provisão inicial, bônus ao crítico), baixados uma vez da wiki e versionados em snapshots. A criação continua com os seis campos da 008, mas deriva HP, esquiva, dano, crítico, velocidade, precisão 0, proteção 0, passos e resistências (base + 10 p.p. × resolução, teto 100) sem acessórios. O card da lista mostra seis categorias + escolhas de criação + extras **lidos da classe**. Sem cobertura 20×5 local, a criação fiel não é oferecida; lacunas param o trabalho (nada inventado, nada em tempo real).

## Technical Context

**Language/Version**: C# / .NET 10

**Primary Dependencies**: ASP.NET Core, EF Core, SQL Server, xUnit, FluentAssertions; CLI de curadoria com BCL (`HttpClient`, parse de wikitext); UI estática HTML/CSS/JS existente

**Storage**: SQL Server oficial (migração: campos de `Classe`, `PassosAFrente`/`PassosAtras` em `Personagem`, seed de `Arma`/`Armadura`); snapshots JSON em `specs/009-atributos-oficiais-personagem/wiki-snapshots/`; InMemory nos testes

**Testing**: Domain + Architecture + API (contrato HTTP da criação fiel, recusa de catálogo incompleto, amostra de resistências, card/lista); testes de cobertura dos 20 snapshots **sem rede**

**Target Platform**: API .NET hospedada; navegador moderno em `/personagens/index.html`; coletor CLI no ambiente do curador

**Project Type**: Serviço web em camadas + frontend estático + ferramenta de curadoria em `tools/`

**Performance Goals**: Criação e lista do card em até 3 s em condições normais; lista não dispara N+1 de wiki; coletor só na curadoria

**Constraints**: Constituição 1.0.0; PT-BR; regras fora do controller; sem wiki em runtime (FR-004); sem interpolação (FR-004a); sem recálculo de personagens antigos; trinkets e forma besta fora; seis campos de entrada inalterados

**Scale/Scope**: 20 classes × 5 níveis arma + 5 armadura; 1 par de deslocamento e 3 extras por classe; 1 fluxo de criação; 1 card na lista

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Princípio | Verificação | Status |
|---|---|---|
| I. Arquitetura em camadas | Domínio: invariantes de classe/personagem/níveis. Application: derivação fiel e DTO do card. Infrastructure: seed, migração, repositórios. API: contratos HTTP. UI: apresentação. Coletor em `tools/` (borda). Sem regra no controller. | PASS |
| II. SQL Server | Números oficiais e novos campos via EF + migração. Snapshots são artefato de curadoria (como 005), não substituto do banco. | PASS |
| III. Contratos verificáveis | `POST /personagens`, `GET /personagens`, `GET /classes/{id}` cobertos em testes (sucesso + catálogo incompleto + Abominação humana + extras da classe). | PASS |
| IV. PT-BR | Card, erros, relatório do coletor em Português do Brasil. | PASS |
| V. Operação remota e simplicidade | API stateless; sem cliente wiki no runtime; coletor opcional de curadoria; reuso de `Arma`/`Armadura`. | PASS |

**Gate status**: PASS.

## Project Structure

### Documentation (this feature)

```text
specs/009-atributos-oficiais-personagem/
├── plan.md
├── spec.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── catalogo-oficial.md
│   └── personagens-atributos.md
├── wiki-snapshots/          # gerado pelo coletor; commitado após validação
├── checklists/
│   └── requirements.md
└── tasks.md                 # /speckit-tasks — NÃO criado aqui
```

### Source Code (repository root)

```text
src/
├── DarkestDungeon.Domain/
│   ├── Classes/Classe.cs
│   ├── Itens/NivelDeArma.cs
│   ├── Itens/NivelDeArmadura.cs
│   └── Seres/Personagem.cs
├── DarkestDungeon.Application/
│   ├── Classes/
│   └── Personagens/
├── DarkestDungeon.Infrastructure/
│   ├── Data/Seeds/          # EquipamentosSeed + extensão ClassesSeed
│   └── Migrations/
├── DarkestDungeon.Api/
│   ├── Contracts/Catalogo/
│   ├── Controllers/Catalogo/
│   └── wwwroot/personagens/
└── tools/
    └── DarkestDungeon.WikiCatalogCollector/

tests/
├── DarkestDungeon.Domain.Tests/
├── DarkestDungeon.Api.Tests/
└── DarkestDungeon.Architecture.Tests/
```

**Structure Decision**: Quatro projetos da solução + CLI de curadoria no mesmo padrão do MediaCollector. Sem quinto projeto de runtime. Snapshots versionados na pasta da feature, como na 005.

## Phase 0: Research Summary

Decisões em [research.md](research.md): dump local (não runtime); lacuna = parar; seed de `Arma`/`Armadura`; dois eixos de movimento no personagem; extras só na classe; fórmula fiel sem trinkets; coletor na borda; lista enriquecida para o card; sem recálculo legado.

Não restam itens NEEDS CLARIFICATION.

## Phase 1: Design Summary

- Entidades e fórmulas: [data-model.md](data-model.md)
- Coletor e cobertura: [contracts/catalogo-oficial.md](contracts/catalogo-oficial.md)
- Criação e card: [contracts/personagens-atributos.md](contracts/personagens-atributos.md)
- Validação executável: [quickstart.md](quickstart.md)

## Implementation Phases

### Phase A: Coleta local e seed oficial (US1)

Implementar o coletor, gravar 20 snapshots, validar cobertura 20×5 + deslocamento + extras + Abominação humana. Gerar seed idempotente de arma/armadura e campos novos de `Classe`. Falha de campo = interromper (sem interpolar).

### Phase B: Persistência e criação fiel (US2)

Migração (`PassosAFrente`/`PassosAtras` no personagem; extras na classe). Application deriva atributos e resistências da fórmula oficial; recusa catálogo incompleto; mantém 4+4 e os seis campos de entrada.

### Phase C: Card categorizado (US3)

Enriquecer `GET /personagens` (e detalhe) com atributos, oito resistências, habilidades inclusive nível 0, extras da classe via join. UI: seis categorias + cabeçalho de níveis/aparência.

### Phase D: Testes e compatibilidade

Contratos HTTP, amostra 3 classes × 3 resoluções, Abominação humana, lista sem segundo GET, legado sem recálculo, architecture (API/domínio sem `HttpClient` para wiki). Quickstart.

## Constitution Check (Post-Design)

- **Camadas**: PASS. Parser wiki só no `tools/`; criação lê seed/SQL.
- **SQL Server**: PASS. Migração + seed; JSON não é persistência de negócio.
- **Contratos**: PASS. Sucesso e erros de catálogo documentados.
- **PT-BR**: PASS.
- **Simplicidade**: PASS. Reuso de item/classe; sem endpoint de sync wiki.

**Post-design gate status**: PASS.

## Complexity Tracking

Nenhuma violação constitucional. O CLI extra é ferramenta de borda (mesmo papel do MediaCollector), não um quarto runtime.
