# Implementation Plan: Auditoria das Habilidades Mineradas da Wiki

**Branch**: `005-auditoria-habilidades-mineradas` | **Date**: 2026-09-08 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `specs/005-auditoria-habilidades-mineradas/spec.md`

## Summary

Auditar as 219 habilidades já semeadas na Feature 003 contra a wiki oficial `darkestdungeon.wiki.gg`, expandindo o modelo para conter os 5 níveis oficiais por habilidade, revisar regras de "treinada vs equipada" no Personagem e introduzir os novos campos `Aparencia` (A/B/C/D), `Experiencia` (XP) e `NivelDeResolucao` (Curioso→Lenda com bônus de +10% por nível). Sistema adota **modo único de campanha** (equivalente a Darkest/Stygian) com tabela XP fixa `{2, 8, 14, 24, 36, 48}`. Mapa de assets Classe × Aparência consome o **inventário da Feature 004** (Conjunto Spine + hash), sem duplicar paths. Publicação atomística em SQL Server 2022 em janela de manutenção. Abordagem técnica: adicionar owned type `NivelDeHabilidade` (5 linhas fixas) via nova migration EF Core, criar mineradores especializados por classe (incluindo remineção das 4 baselines) que geram um relatório Markdown de auditoria, e substituir o pipeline de seed atual por um publisher transacional que faz upsert por GUID em uma única `IDbContextTransaction`.

## Technical Context

**Language/Version**: C# 14 / .NET SDK 10.0.400 (herdado da Feature 003)

**Primary Dependencies**: ASP.NET Core 10 (Minimal API), EF Core 10 (SQL Server provider), Newtonsoft.Json (owned type de resistências), FluentAssertions 8.10.0, NetArchTest.Rules 1.3.2, Microsoft.AspNetCore.Mvc.Testing 10, xUnit

**Storage**: SQL Server 2022 Developer via Docker (container `mssql-dd`, imagem `mcr.microsoft.com/mssql/server:2022-latest`, porta 1433, volume `mssql-dd-data`). Connection string em `appsettings.json`: `Server=localhost,1433;Database=DarkestDungeon;User Id=sa;Password=Devlocal!2024;TrustServerCertificate=True`. Migration consolidada base: `20260908131601_SchemaCompleto`.

**Testing**: xUnit + FluentAssertions + NetArchTest.Rules (regras de camadas) + Microsoft.AspNetCore.Mvc.Testing (contract tests via `WebApplicationFactory`). Baseline atual: 141 testes verdes.

**Target Platform**: Backend ASP.NET Core hospedado remotamente (VPN + internet), consumido por até 6 jogadores concorrentes numa sessão live.

**Project Type**: web-service (Backend em camadas — Domain, Application, Infrastructure, Api — sem frontend).

**Performance Goals**:

- Regerar relatório de auditoria em < 5 min (SC-008), sem hits à internet (cache local dos snapshots wiki).
- Publicação atômica (~1.100 upserts = 219 habilidades × 5 níveis) deve caber em uma transação SQL Server dentro do timeout configurável (padrão 30s; parametrizado até 300s se necessário — ver research.md).
- Consulta ao log de falha de publicação < 10s (SC-017).

**Constraints**:

- Constitucional: SQL Server oficial, PT-BR obrigatório em mensagens/logs voltados ao usuário, arquitetura em camadas, contratos com testes de sucesso + erro relevante.
- Compatibilidade: 141 testes existentes MUST permanecer verdes; 219 habilidades / 277 associações / 20 classes MUST manter contagem; GUIDs determinísticos preservados (SC-003).
- Zero downtime lógico durante publicação: rollback total em falha (FR-009a); apenas em janela de manutenção com 0 conexões ativas (FR-009c).
- Fonte única: `darkestdungeon.wiki.gg` (Assumption principal).

**Scale/Scope**: 20 classes; 219 habilidades × 5 níveis = **1.095 linhas** de `NivelDeHabilidade`; 80 combinações Classe × Aparência (**vinculadas ao inventário 004**, não duplicadas); **modo único** de campanha; ~7 níveis de Resolve (0..6). Impacta ~22 arquivos partial de seed em `src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.*.cs`. Fora do escopo: mídias de Arma/Armadura/Item, inimigos e ambientes (ver seção "Out of Scope" da spec).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-checked after Phase 1 design.*

| Princípio | Verificação | Status |
|---|---|---|
| I. Arquitetura em Camadas | Novos artefatos localizados: `NivelDeHabilidade` (Domain), `IAuditoriaWikiService` + `IPublicadorAtomicoService` (Application), migration + seed publisher (Infrastructure), endpoints `/api/auditoria/*` e `/api/publicacao/*` (Api). Nenhuma regra de negócio em controller. | PASS |
| II. SQL Server como Persistência Oficial | Publicação transacional é feita via `DbContext.Database.BeginTransactionAsync()` no provider SQL Server; owned type `NivelDeHabilidade` mapeado como tabela filha; sem SQL cru desnecessário. | PASS |
| III. Contratos de Backend Verificáveis | Cada novo endpoint (relatório de auditoria, iniciar publicação, consultar log) terá contract test com fluxo de sucesso + erro relevante (400 janela ativa, 409 outra publicação em curso, 500 rollback executado). | PASS |
| IV. Português do Brasil | Mensagens de erro de publicação, rejeição de 4ª habilidade equipada (FR-007c), XP insuficiente (FR-007m) e aparência inválida (SC-013) em PT-BR. Nomes de entidades PT-BR (`NivelDeHabilidade`, `AparenciaDePersonagem`, `NivelDeResolucao`, `TabelaDeExperiencia`). | PASS |
| V. Operação Remota e Simplicidade Proporcional | Publicação stateless por request; janela de manutenção validada por contagem de sessões ativas (mecanismo simples). Credenciais SQL continuam em `appsettings.json`/env vars (nunca commitadas em produção). Nenhuma abstração nova além do estritamente necessário. | PASS |

**Resultado**: PASS — nenhuma violação a justificar. Complexity Tracking vazio.

## Project Structure

### Documentation (this feature)

```text
specs/005-auditoria-habilidades-mineradas/
├── plan.md              # Este arquivo
├── spec.md              # Especificação da feature (já existente)
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
│   ├── auditoria.md
│   └── publicacao.md
├── checklists/
│   └── requirements.md  # já existente (16/16)
└── tasks.md             # Phase 2 (/speckit-tasks)
```

### Source Code (repository root)

```text
src/
├── DarkestDungeon.Domain/
│   ├── Seres/
│   │   ├── Personagem.cs                     # + Aparencia, + Experiencia, revisar Limite*
│   │   └── HabilidadeDePersonagem.cs         # ALTERADO: remove Treinada/Habilitada persistidos, + NumeroDoNivel (0..5)
│   ├── Habilidades/
│   │   ├── HabilidadeDeCombate.cs            # + coleção Niveis (5 linhas)
│   │   ├── HabilidadeDeAcampamento.cs        # + coleção Niveis (5 linhas)
│   │   └── NivelDeHabilidade.cs              # NOVO owned type
│   ├── Classes/
│   │   ├── ClasseDeHeroi.cs                  # (sem mudança)
│   │   └── AssetsDeClasse.cs                 # NOVO — vincula Aparência → ConjuntoSpineId (Feature 004)
│   ├── Personagens/
│   │   ├── AparenciaDePersonagem.cs          # NOVO enum A/B/C/D
│   │   ├── NivelDeResolucao.cs               # NOVO — Nome PT-BR (Curioso..Lenda) + NomeOriginal (Seeker..Legend)
│   │   └── TabelaDeExperiencia.cs            # NOVO — limiares únicos {2,8,14,24,36,48}
│   └── Comum/                                # (existente)
│
├── DarkestDungeon.Application/
│   ├── Auditoria/
│   │   ├── IAuditoriaWikiService.cs          # NOVO
│   │   ├── AuditoriaWikiService.cs           # NOVO — gera relatorio.md
│   │   ├── StatusDeAuditoria.cs              # NOVO enum OK/Parcial/Faltando
│   │   └── RelatorioDeAuditoria.cs           # NOVO — DTO agregado
│   ├── Publicacao/
│   │   ├── IPublicadorAtomicoService.cs      # NOVO
│   │   ├── PublicadorAtomicoService.cs       # NOVO — transação + rollback
│   │   └── LogDePublicacao.cs                # NOVO — entrada estruturada
│   └── Personagens/                          # ajustes de regras de treinada/equipada
│
├── DarkestDungeon.Infrastructure/
│   ├── Data/
│   │   ├── Migrations/
│   │   │   └── {timestamp}_NiveisEProgressao.cs  # NOVO — +NivelDeHabilidade, +Aparencia, +Experiencia
│   │   ├── Configurations/
│   │   │   ├── HabilidadeDeCombateConfiguration.cs   # OwnsMany(Niveis)
│   │   │   ├── HabilidadeDeAcampamentoConfiguration.cs
│   │   │   ├── PersonagemConfiguration.cs            # HasConversion Aparencia; Experiencia int (sem ModoDeCampanha)
│   │   │   └── ClasseDeHeroiConfiguration.cs         # OwnsMany AssetsDeClasse com FK lógica pro inventário 004
│   │   └── Seeds/
│   │       ├── HabilidadesSeed.*.cs                  # 22 arquivos — refatorar p/ 5 níveis
│   │       └── AssetsDeClasseSeed.cs                 # NOVO
│   └── Publicacao/
│       └── PublicadorAtomicoEfCore.cs        # NOVO impl da interface Application
│
├── DarkestDungeon.Api/
│   ├── Endpoints/
│   │   ├── AuditoriaEndpoints.cs             # NOVO — GET /api/auditoria/classes/{id}
│   │   └── PublicacaoEndpoints.cs            # NOVO — POST /api/publicacao, GET /api/publicacao/logs
│   └── Program.cs                            # + DI dos novos services
│
tests/
└── DarkestDungeon.Tests/
    ├── Domain/
    │   ├── NivelDeHabilidadeTests.cs         # NOVO
    │   ├── PersonagemAparenciaTests.cs       # NOVO
    │   ├── PersonagemExperienciaTests.cs     # NOVO (subir nivel + bônus 10%)
    │   └── HabilidadeDePersonagemNivelTests.cs   # NOVO (Nivel=0 bloqueada)
    ├── Application/
    │   ├── AuditoriaWikiServiceTests.cs      # NOVO
    │   └── PublicadorAtomicoTests.cs         # NOVO (rollback simulado)
    ├── Infrastructure/
    │   └── SeedNiveisTests.cs                # NOVO (219 × 5 = 1095 linhas)
    └── Api/
        ├── AuditoriaEndpointsTests.cs        # NOVO contract test
        └── PublicacaoEndpointsTests.cs       # NOVO contract test (400/409/500)
```

**Structure Decision**: Web-service em camadas C# — **mesma estrutura da Feature 003**. Reutiliza:

- Domain: `src/DarkestDungeon.Domain/` — expande `Seres`, `Habilidades`, `Classes` e cria `Personagens/`.
- Application: `src/DarkestDungeon.Application/` — dois novos módulos (`Auditoria/`, `Publicacao/`).
- Infrastructure: `src/DarkestDungeon.Infrastructure/` — nova migration incremental (**não consolidar** — a Feature 003 já consolidou; esta feature adiciona por cima), refatoração dos 22 seed partials e uma implementação do publicador atômico.
- Api: `src/DarkestDungeon.Api/` — dois novos arquivos de endpoints com Minimal API.
- Tests: `tests/DarkestDungeon.Tests/` — segue divisão por camada existente.

## Complexity Tracking

*Vazio — Constitution Check passou sem violações.*

## Post-Design Constitution Re-check

Após gerar `research.md`, `data-model.md`, `contracts/auditoria.md`, `contracts/publicacao.md` e `quickstart.md`, re-avaliei os 5 princípios:

| Princípio | Verificação pós-design | Status |
|---|---|---|
| I. Arquitetura em Camadas | data-model.md confirma que `NivelDeHabilidade`/`AparenciaDePersonagem`/`NivelDeResolucao`/`TabelaDeExperiencia` vivem em Domain; `AuditoriaWikiService`/`PublicadorAtomicoService`/`LogDePublicacao` em Application; publicador EF + migration em Infrastructure; endpoints só orquestram. Nenhum vazamento. | PASS |
| II. SQL Server oficial | research.md R2 usa `IDbContextTransaction` do provider SQL Server; R3 usa DMV `sys.dm_exec_sessions`; R4 persiste log em tabela SQL Server. Migration incremental (não consolidada). | PASS |
| III. Contratos verificáveis | contracts/auditoria.md e contracts/publicacao.md especificam todos os testes contract exigidos: 5 cenários de auditoria + 9 cenários de publicação (feliz + 400 x2 + 409 + 500 + status + logs). | PASS |
| IV. PT-BR | Todas as mensagens de erro nos contracts em PT-BR; nomes de entidades no data-model em PT-BR (`NivelDeHabilidade`, `AparenciaDePersonagem`, `NivelDeResolucao`, `TabelaDeExperiencia`, `AssetsDeClasse`, `LogDePublicacao`, `RelatorioDeAuditoria`). | PASS |
| V. Simplicidade | Nenhuma nova dependência externa; publicação síncrona sem fila; log em tabela do mesmo banco; assets como paths (sem bytes no DB); mineração wiki manual assistida (não scraping). | PASS |

**Resultado pós-design**: PASS — nenhuma violação nova. Design consistente com plano.

## 3ª Clarify — Impacto no Plano (2026-09-08)

A 3ª sessão de `/speckit-clarify` (5 decisões após `/speckit-analyze`) exigiu ajustes localizados propagados neste plano e nos artefatos Phase 0/1:

| Decisão | Impacto neste plan |
|---|---|
| Q1 · `AssetsDeClasse` vincula inventário 004 (ConjuntoSpineId + hash) | Estrutura de código atualizada — `AssetsDeClasse.cs` agora referencia inventário 004, não armazena paths livres. R7 do `research.md` reescrito. Seção 10 do `data-model.md` reescrita. |
| Q2 · Armas/armaduras/itens **fora de escopo** | Adicionada nota em Scale/Scope confirmando exclusão. Nova seção "Out of Scope" na spec cita Feature 006 futura. |
| Q3 · Remineirar 4 baselines da wiki | Nenhum impacto estrutural neste plan; afeta apenas tarefas de mineração (tasks.md) e SC-002 (spec). |
| Q4 · Nomes de Resolve Level em PT-BR + `NomeOriginal` inglês | `NivelDeResolucao` (seção 8 data-model) agora expõe `Nome` PT-BR e `NomeOriginal` inglês. `research.md` R9 reescrito. |
| Q5 · **Modo único de campanha** | `ModoDeCampanha.cs` **removido** da estrutura de código. `TabelaDeExperiencia` simplificada para lista única `{2, 8, 14, 24, 36, 48}`. `Personagem` perde campo de modo. `PersonagemConfiguration.cs` não mapeia mais o modo. Seções 5 e 9 do `data-model.md` reescritas; seção 7 marcada como REMOVIDO. |

Constitution Check **permanece PASS** após essas mudanças — todas simplificam o design (menos código, menos edge cases, mais consistência PT-BR).

