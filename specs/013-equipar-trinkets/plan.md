# Implementation Plan: Equipar Trinkets no Personagem

**Branch**: `013-equipar-trinkets` | **Date**: 2026-09-11 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from [spec.md](spec.md)

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

Completar o catálogo de **Acessório** (trinket) com bônus oficiais da wiki (jogo base + DLC, melhor esforço, lacunas sem inventar) e, no card de um herói **já criado**, oferecer dois espaços independentes. Equipar/remover **não regrava** a ficha da criação: a consulta devolve **ficha base** persistida (visível) e **ficha efetiva** em destaque (base + efeitos dos espaços, unidade por atributo, teto após a soma). Reusa `Acessorio`/`EfeitoDeAcessorio` e os dois `Guid?` do Personagem. Não toca criação 008/009, mídia 006/011, nem `IPublicadorAtomicoService`.

## Technical Context

**Language/Version**: C# / .NET 10

**Primary Dependencies**: ASP.NET Core, EF Core, SQL Server, xUnit, FluentAssertions; CLI de curadoria com BCL (`HttpClient`, parse de wikitext); UI estática HTML/CSS/JS em `/personagens/`

**Storage**: SQL Server oficial (upsert de `Acessorio` + `EfeitosAcessorio`; slots `AcessorioEquipado1Id`/`AcessorioEquipado2Id` já existem, sem FK). Snapshots JSON em `specs/013-equipar-trinkets/wiki-snapshots/`. InMemory nos testes. Ficha efetiva **não** é coluna.

**Testing**: Domain (mapeamento de efeito, teto, empilhamento sobre a base, limites 0–100 / HP ≥ 1); Application (upsert idempotente, filtrar lista por classe); API (GET lista, PUT por espaço, GET personagem com base+efetiva, recusas PT-BR, atomicidade); UI do card; coletor **sem rede** a partir de wikitext de fixture. Arquitetura: Application/API **não** referenciam `IPublicadorAtomicoService` nesta feature.

**Target Platform**: API .NET hospedada; navegador moderno em `/personagens/index.html`; coletor CLI no ambiente do curador

**Project Type**: Serviço web em camadas + frontend estático + ferramenta de curadoria em `tools/`

**Performance Goals**: Abrir card → escolher trinket no espaço 1 → ver ficha efetiva em até 1 minuto (SC-009); consulta de lista/card sem chamar a wiki; troca atômica em uma transação por personagem

**Constraints**: Constituição 1.0.0; PT-BR; regras fora do controller; wiki só na curadoria; ficha base imutável; sem bônus de conjunto; sem recalcular criação; sem reabrir ícones/Spine; **não** alterar `IPublicadorAtomicoService` nem `DerivacaoDeAtributosOficiais`

**Scale/Scope**: Dezenas a ~200 trinkets (base + DLC); 2 espaços por personagem; 1 card na lista; 1 coletor de curadoria; mapeamento canônico dos atributos da ficha (HP, dano, PROT, CRIT, ACC, DODGE, SPD, virtude, 8 resistências)

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Princípio | Verificação | Status |
|---|---|---|
| I. Arquitetura em camadas | Domínio: mutação de efeitos do Acessório, invariante dos dois espaços. Application: upsert da coleta, lista filtrada, calculador da ficha efetiva, comando atômico por espaço. Infrastructure: repositório, seed/upsert, migração se índice. API: contratos HTTP. UI: dois seletores + base/efetiva. Coletor em `tools/` (borda). Sem regra no controller. | PASS |
| II. SQL Server | Catálogo e slots via EF. Snapshots são curadoria (padrão 009), não substituto do banco. Ficha efetiva calculada na leitura. | PASS |
| III. Contratos verificáveis | Novos/alterados: `GET /acessorios`, `PUT /personagens/{id}/acessorios/{espaco}`, `GET /personagens` e `GET /personagens/{id}` com base+efetiva e dois slots posicionais. Testes de sucesso + recusas (classe, duplicata, inexistente, personagem inexistente) + atomicidade. | PASS |
| IV. PT-BR | Rótulos dos espaços, vazio, lista vazia e erros em Português do Brasil. Nomes originais da wiki para rastreio. | PASS |
| V. Operação remota e simplicidade | API stateless; sem cliente wiki no runtime; reuso de Acessório e slots existentes; sem quinto projeto; sem publicador 005. | PASS |

**Gate status**: PASS.

## Project Structure

### Documentation (this feature)

```text
specs/013-equipar-trinkets/
├── plan.md
├── spec.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── coleta-trinkets.md
│   └── personagens-trinkets.md
├── wiki-snapshots/          # gerado pelo coletor; commitado após validação amostral
├── checklists/
│   └── requirements.md
└── tasks.md                 # /speckit-tasks — NÃO criado aqui
```

### Source Code (repository root)

```text
src/
├── DarkestDungeon.Domain/
│   ├── Itens/Acessorio.cs
│   ├── Itens/EfeitoDeAcessorio.cs
│   └── Seres/Personagem.cs
├── DarkestDungeon.Application/
│   ├── Itens/
│   └── Personagens/          # calculador efetiva; comando por espaço
├── DarkestDungeon.Infrastructure/
│   ├── Data/
│   └── Data/Seeds/           # upsert a partir dos snapshots 013
├── DarkestDungeon.Api/
│   ├── Contracts/Catalogo/
│   ├── Controllers/Catalogo/
│   └── wwwroot/personagens/
└── tools/
    └── DarkestDungeon.WikiCatalogCollector/   # modo trinkets (best-effort)

tests/
├── DarkestDungeon.Domain.Tests/
├── DarkestDungeon.Api.Tests/
└── DarkestDungeon.Architecture.Tests/
```

**Structure Decision**: Quatro projetos da solução + o CLI de curadoria já existente. Sem quinto projeto de runtime. Snapshots versionados na pasta da feature, como na 009.

## Phase 0: Research Summary

Decisões em [research.md](research.md): coletor best-effort no CLI 009 (modo novo, não fail-closed); upsert por `NomeOriginal` preservando Id; ficha efetiva só na leitura; endpoint por espaço (não zerar o outro); lista filtrada por classe; mapeamento canônico + lacuna se o nome não casar; teto depois da soma; `POST /equipar` deixa de apagar slots se `AcessoriosIds` omitido; `IPublicadorAtomicoService` intocado.

Não restam itens NEEDS CLARIFICATION.

## Phase 1: Design Summary

- Entidades, mapeamento e fórmulas: [data-model.md](data-model.md)
- Coletor e upsert: [contracts/coleta-trinkets.md](contracts/coleta-trinkets.md)
- Card, lista e troca atômica: [contracts/personagens-trinkets.md](contracts/personagens-trinkets.md)
- Validação executável: [quickstart.md](quickstart.md)

## Implementation Phases

### Phase A: Coleta e catálogo oficial (US1)

Modo trinkets no WikiCatalogCollector (DLC incluso, lacuna registrada, continua). Snapshots JSON. Upsert de `Acessorio` por nome original: atualiza raridade/efeitos/classe exclusiva/descrição **sem** trocar Id; cria Guid novo se ausente. Não inventa valor. Não reabre mídia.

### Phase B: Dois espaços no card (US2)

`GET /acessorios` filtrado pela classe do herói e excluindo o ID do outro espaço. `PUT` por espaço. Card: dois seletores, lista só permitidos, persistência independente. `GET` lista/detalhe com slots posicionais.

### Phase C: Ficha efetiva segura (US3)

Calculador na Application (sem regravar `Ser`). Unidade por atributo (FR-013a), teto (FR-013b), limites. Card mostra efetiva em destaque e base visível. Falha = estado anterior. `POST /personagens/{id}/equipar` **não** zera trinkets se `AcessoriosIds` omitido.

## Constitution Check (pós-design)

Reavaliado após data-model e contratos: camadas intactas; SQL Server permanece a persistência; contratos HTTP novos/estendidos são testáveis; PT-BR nos textos de produto; operação remota sem wiki em runtime. **PASS**. Sem violações — Complexity Tracking vazio.

## Complexity Tracking

> Preenchido somente se o Constitution Check tiver violações justificadas.

Nenhuma.
