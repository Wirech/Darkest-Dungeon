# Implementation Plan: Importação e Vínculo de Mídias de Armas, Armaduras e Itens

**Branch**: `006-midias-armas-armaduras-itens` | **Date**: 2026-09-10 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `specs/006-midias-armas-armaduras-itens/spec.md`

## Summary

Descobrir na instalação local licenciada as mídias de armas, armaduras, acessórios (troféus/trinkets), itens de acampamento/provisão e consumíveis; copiá-las sem alterar bytes para `assets/equipamentos-itens/`; reutilizar o inventário da Feature 004 quando o SHA-256 já existir; e vincular o catálogo da Feature 003 (20 armas × 5 níveis, 20 armaduras × 5 níveis, acessórios existentes) mais dois tipos novos de `Item` (`ItemDeAcampamento`, `Consumivel`). Referência de mídia no domínio segue o padrão 005 (`ArquivoInventarioId` / `ConjuntoSpineId` + `HashArquivo`), sem paths livres. Publicação **atômica por categoria a qualquer momento**, **sem** janela de manutenção e **sem** reutilizar `IPublicadorAtomicoService` da 005. Relatório de cobertura PT-BR (`OK` / `Parcial` / `Pendente`) via API. Abordagem: estender `tools/DarkestDungeon.MediaCollector`, migration incremental EF, novos endpoints `/api/midias/*`, estender GET `/itens/{id}` e detalhe de Personagem.

## Technical Context

**Language/Version**: C# 14 / .NET SDK 10.0.400 (herdado das Features 003–005)

**Primary Dependencies**: ASP.NET Core 10, EF Core 10 (SQL Server), xUnit, FluentAssertions, NetArchTest.Rules, Microsoft.AspNetCore.Mvc.Testing; CLI usa apenas BCL (`System.IO`, `System.Security.Cryptography`, `System.Text.Json`)

**Storage**: SQL Server 2022 Developer via Docker (`mssql-dd`, porta 1433). Inventário de arquivos em `assets/equipamentos-itens/` + merge de hash com `assets/herois/inventario.json`. Sem bytes de mídia no banco.

**Testing**: xUnit + FluentAssertions + NetArchTest + `WebApplicationFactory`. Baseline na abertura desta feature: 206 testes verdes (pós-005). Novos testes em `tests/DarkestDungeon.Domain.Tests`, `tests/DarkestDungeon.Api.Tests` (contratos + `ColetaMidias/Fixtures/Equipamentos/`). Architecture tests devem continuar proibindo Domain → Infrastructure.

**Target Platform**: Backend ASP.NET Core hospedado (internet + VPN); CLI de importação no Windows/Linux do curador com instalação local do jogo.

**Project Type**: web-service em camadas (Domain, Application, Infrastructure, Api) **+** extensão da CLI existente em `tools/DarkestDungeon.MediaCollector`

**Performance Goals**:

- Relatório de cobertura em ≤ 2 minutos (SC-008) no volume desta feature (centenas de vínculos, não mineração wiki).
- Publicação por categoria em uma transação SQL Server (timeout padrão 30s).
- Importação CLI sem rede; reexecução sem recopiar hashes existentes.

**Constraints**:

- Constituição 1.0.0: camadas, SQL Server, contratos testáveis, PT-BR, simplicidade / env fora do repo.
- Não alterar IDs nem os cinco níveis numéricos de armas/armaduras 003.
- Não alterar equipamento já atribuído a Personagens.
- Não reusar o publicador 005 (bloqueio por sessão / janela).
- Sem download remoto; sem inimigos, tiles, ícones de status; sem reauditoria de `AssetsDeClasse`.
- Caminho da instalação via `--origem` / ambiente, nunca secret no git.

**Scale/Scope**: 20 classes; 20 armas × 5 + 20 armaduras × 5 = 200 vínculos de nível; N acessórios (seed 003 + novos da instalação); provisões e consumíveis conforme pastas mapeadas. Cinco categorias de cobertura. Fora: combate, loja, bônus de conjunto, curadoria posterior de raridade dos trinkets novos.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Princípio | Verificação | Status |
|---|---|---|
| I. Arquitetura em Camadas | Domain: `MidiaDeItem`, `ItemDeAcampamento`, `Consumivel`, extensão de `NivelDeArma`/`NivelDeArmadura`/`Acessorio`. Application: `IPublicadorDeVinculosDeMidia`, `ICoberturaDeMidiasService`, `ILeitorDeInventarioDeMidias`, DTOs (sem `System.IO`). Infrastructure: migration + `LeitorDeInventarioDeMidias` (JSON) + EF. Api: controllers `/api/midias/*` e extensão de GET itens/personagem. CLI em `tools/` (borda). Nenhuma regra de vínculo no controller. | PASS |
| II. SQL Server como Persistência Oficial | Vínculos e tipos novos em SQL Server via EF; transação `BeginTransactionAsync` por categoria. Inventário de arquivos é artefato de importação (mesmo papel da 004), não substitui o banco de negócio. | PASS |
| III. Contratos de Backend Verificáveis | Contratos em `contracts/cobertura-midias.md`, `publicacao-vinculos.md`, `itens-e-personagem.md` listam sucesso + 400/409/500 relevantes. CLI em `coletor-cli.md`. | PASS |
| IV. Português do Brasil | Relatório, lacunas, estados `OK`/`Parcial`/`Pendente`, mensagens de API e CLI em PT-BR. Nomes de arquivo originais só como rastreio. | PASS |
| V. Operação Remota e Simplicidade Proporcional | API stateless; publicação sem fila; sem detector de sessão; reuso do CLI 004; TPH em vez de TPT; caminho da instalação fora do código. Publicador 006 **não** exige 0 sessões (Q5). | PASS |

**Resultado**: PASS — Complexity Tracking vazio.

## Project Structure

### Documentation (this feature)

```text
specs/006-midias-armas-armaduras-itens/
├── plan.md              # Este arquivo
├── spec.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── cobertura-midias.md
│   ├── publicacao-vinculos.md
│   ├── itens-e-personagem.md
│   └── coletor-cli.md
├── checklists/
│   └── requirements.md
└── tasks.md             # Phase 2 (/speckit-tasks) — NÃO criado aqui
```

### Source Code (repository root)

```text
src/
├── DarkestDungeon.Domain/
│   └── Itens/
│       ├── Item.cs                      # comentário TPH: + ItemDeAcampamento, Consumivel
│       ├── Arma.cs                      # inalterado (Id / 5 níveis)
│       ├── Armadura.cs
│       ├── Acessorio.cs                 # + Midia; criação de novos na Application
│       ├── NivelDeArma.cs               # + MidiaDeItem
│       ├── NivelDeArmadura.cs           # + MidiaDeItem
│       ├── MidiaDeItem.cs               # NOVO
│       ├── StatusDeMidia.cs             # NOVO
│       ├── ItemDeAcampamento.cs         # NOVO
│       └── Consumivel.cs                # NOVO
├── DarkestDungeon.Application/
│   ├── Midias/
│   │   ├── IPublicadorDeVinculosDeMidia.cs    # NOVO (não é IPublicadorAtomicoService)
│   │   ├── ICoberturaDeMidiasService.cs       # NOVO
│   │   ├── ILeitorDeInventarioDeMidias.cs     # NOVO (porta; sem System.IO)
│   │   ├── RelatorioDeCoberturaDeMidias.cs    # NOVO
│   │   └── StatusDeCoberturaDeItem.cs         # NOVO
│   ├── Itens/
│   │   ├── ItemDtos.cs                  # + midia; TipoDeItemDto estendido
│   │   └── ItemService.cs               # criar acampamento/consumível
│   └── Personagens/
│       └── PersonagemDtos.cs            # + midia do equipamento (projeção)
├── DarkestDungeon.Infrastructure/
│   ├── Data/
│   │   ├── DarkestDungeonDbContext.cs   # TPH + owned Midia
│   │   └── Migrations/
│   │       └── {timestamp}_MidiasDeEquipamentoEItens.cs
│   └── ├── LeitorDeInventarioDeMidias.cs      # NOVO (JSON compartilhado)
│       Midias/
│       └── PublicadorDeVinculosDeMidia.cs
├── DarkestDungeon.Api/
│   └── Controllers/
│       ├── Catalogo/ItensController.cs  # GET já existe; + POST acampamento/consumível
│       └── Midias/                      # NOVO cobertura + publicação
└── tools/DarkestDungeon.MediaCollector/
    ├── Program.cs                       # ramo --categoria
    ├── Configuracao/OpcoesDoColetor.cs  # --categoria, --mapeamento, --inventario-herois
    ├── Inventario/ModelosDeInventario.cs
    └── Importacao/                      # descoberta por mapeamento de pastas

tests/
├── DarkestDungeon.Domain.Tests/         # MidiaDeItem, ItemDeAcampamento, Consumivel
├── DarkestDungeon.Api.Tests/
│   ├── ColetaMidias/                    # fixtures 004 + Equipamentos/
│   ├── ItensEndpointsTests.cs           # campos midia
│   └── Feature006/                      # cobertura + publicação
└── DarkestDungeon.Architecture.Tests/

assets/
├── herois/                              # 004 (somente leitura / merge de hash)
└── equipamentos-itens/                  # 006: arquivos + inventario.json
```

**Structure Decision**: Mesmo web-service em camadas das Features 003–005, mais extensão da CLI 004. Sem frontend. Sem terceiro executável.

## Complexity Tracking

*Vazio — Constitution Check passou sem violações.*

## Phase 0 — Research

Artefato: [research.md](./research.md)

Decisões fechadas (zero `NEEDS CLARIFICATION`):

1. Reusar `MediaCollector`; `--categoria`; saída `assets/equipamentos-itens/`.
2. Mapeamento pasta→tipo (FR-004): tabela em research (trinkets, provision, quest/raid items, heroes weapon/armour + DLC recursivo).
3. PNG obrigatório para `OK`; Spine opcional.
4. Vínculo = inventário + hash (padrão `AssetsDeClasse`).
5. TPH: `ItemDeAcampamento` e `Consumivel`.
6. Trinkets novos: `Acessorio` Guid novo, `Comum`, efeitos vazios.
7. `IPublicadorDeVinculosDeMidia` sem sessões/janela; atômico por categoria; 409 só na mesma categoria.
8. Merge de hash com inventário 004.
9. Relatório síncrono na API; PT-BR.
10. GET item/personagem devolve mídia; pendente não é erro.

## Phase 1 — Design

- [data-model.md](./data-model.md)
- [contracts/cobertura-midias.md](./contracts/cobertura-midias.md)
- [contracts/publicacao-vinculos.md](./contracts/publicacao-vinculos.md)
- [contracts/itens-e-personagem.md](./contracts/itens-e-personagem.md)
- [contracts/coletor-cli.md](./contracts/coletor-cli.md)
- [quickstart.md](./quickstart.md)

## Post-Design Constitution Re-check

| Princípio | Verificação pós-design | Status |
|---|---|---|
| I. Arquitetura em Camadas | data-model coloca `MidiaDeItem` e tipos novos em Domain; publicação/cobertura em Application; EF/migration/leitura de JSON em Infrastructure; controllers só HTTP. CLI isolada em `tools/`. | PASS |
| II. SQL Server oficial | Vínculos owned no SQL; tabela própria de publicação 006 (não herda invariantes 005). Inventário JSON continua artefato de disco. | PASS |
| III. Contratos verificáveis | Quatro contratos com testes de sucesso e erro (400 categoria, 409 em curso / inventário ausente, 500 rollback, GET pendente = 200). | PASS |
| IV. PT-BR | Contratos e relatório usam `OK`/`Parcial`/`Pendente`; lacunas e títulos de erro em português. | PASS |
| V. Simplicidade | Sem segundo CLI, sem TPT, sem janela 005, sem fila, sem bytes no banco, sem scraper. | PASS |

**Resultado pós-design**: PASS.

## Notas explícitas (clarificação 2026-09-10)

| Decisão | Impacto no plano |
|---|---|
| Q1 dois tipos novos | `ItemDeAcampamento` + `Consumivel` no TPH |
| Q2 vincular 003 **e** criar acessórios novos | publicação `acessorio`; seed 003 intocado |
| Q3 raridade `Comum`, efeitos vazios | construtor dos novos `Acessorio` |
| Q4 pasta/origem | tabela em research.md; `--mapeamento` |
| Q5 publicação a qualquer momento | **não** chamar `IPublicadorAtomicoService`; **não** 400 por sessão |

## Next command

`/speckit-tasks` — gerar `tasks.md`. Não implementar nesta etapa.
