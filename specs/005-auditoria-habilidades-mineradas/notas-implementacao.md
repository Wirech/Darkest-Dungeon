# Feature 005 — Notas de Implementação

**Feature**: 005-auditoria-habilidades-mineradas
**Início**: 2026-09-08

## Estado Inicial (baseline pré-Feature 005)

- **Testes verdes**: 141 (T001 verificado)
- **Contagens no SQL Server**:
  - Classes: 20
  - Habilidades: 219
  - Associações Classe × Habilidade: 277
- **Migration base**: `20260908131601_SchemaCompleto`
- **Docker**: `mssql-dd` Up (T002 verificado)

### Checkpoint 2 — 2026-09-08 (fim da sessão de implementação completa)

**Concluído adicional (após Checkpoint 1)**:
- Phase 3 US1 código + contract tests: T021–T035 ✓ (10 tarefas — DTOs, service, controller, endpoints, tests)
- Phase 4 US4 testes Domain: T083–T101 ✓ (13 tarefas com cobertura — 42 novos testes)
- Phase 6 US3 Publisher + tests: T111–T132 ✓ (18 tarefas — LogDePublicacao, PublicadorAtomicoEfCore, DetectorDeSessoesAtivasSqlServer, controller, contract tests)
- Phase 7 Polish parcial: T140 ✓ (`.specify/features/005*.json` atualizado)
- Stubs de wiki-snapshots: 20/20 arquivos vazios prontos para curador preencher

**Contagens finais SQL Server**:
- Classes: 20 (preservado)
- Habilidades: 219 (preservado)
- ClassesHabilidades: 277 (preservado)
- MapaDeCobertura: 160 (preservado)
- **AssetsDeClasse: 80 — 80/80 Coletado vinculadas ao inventário Feature 004**
- **LogsDePublicacao: 0 (tabela criada; pronto para uso)**
- NiveisDeHabilidade: 0 (aguarda mineração wiki + seed refactor T060–T081)

**Testes**: **191 verdes** (**141 baseline + 50 novos Feature 005**).
- Domain.Tests: 88 (baseline 46 + **42 novos**: NivelDeResolucao, TabelaDeExperiencia, NivelDeHabilidade, Personagem Aparencia/Experiencia, HabilidadeDePersonagem, AssetsDeClasse)
- Architecture.Tests: 25 (preservado)
- Api.Tests: 78 (baseline 70 + **8 novos**: 4 Auditoria + 4 Publicação)

**Migrations aplicadas**:
- `20260908131601_SchemaCompleto` (Feature 003 baseline)
- `20260909002441_NiveisEProgressao` (Feature 005 — Aparencia, Experiencia, NumeroDoNivel, NiveisDeHabilidade, ValoresDeEfeitoDeNivel, AssetsDeClasse)
- `20260909004439_LogsDePublicacao` (Feature 005 — tabela de log estruturado)

**Endpoints operacionais**:
- `GET /api/auditoria/relatorio` — relatório completo (funciona com snapshots vazios; marca tudo como Faltando)
- `GET /api/auditoria/relatorio/classes/{classe}` — relatório filtrado
- `GET /api/auditoria/cobertura` — resumo com contagem de assets (80/80 coletados)
- `POST /api/publicacao` — publicação atômica
- `GET /api/publicacao/{id}/status`
- `GET /api/publicacao/{id}/logs?nivel=&campo=`

**Pendências (para futura sessão do curador)**:
- **Mineração wiki** (T013–T020): preencher os 20 stubs em `wiki-snapshots/*.json` com dados Level 1..5 oficiais
- **Refatoração de 22 seeds** (T060–T081): popular `Habilidade.Niveis` com dados dos JSONs mineirados
- **Correções seletivas US2** (T104–T110): após relatório real
- **Integration tests com SQL real** (T095–T097, T125–T134): opcional, cobertura estrutural presente

## Progresso

Ver `tasks.md` para checklist detalhado.

### Checkpoint 1 — 2026-09-08 (fim da sessão de implementação inicial)

**Concluído**:
- Phase 1 Setup: T001–T006 ✓
- Phase 2 Foundational: T007–T012 ✓ (enums + tipos compartilhados)
- Phase 4 US4 Domain: T040–T052 ✓ (value objects, NivelDeHabilidade, NivelDeResolucao PT-BR+EN, TabelaDeExperiencia modo único, AssetsDeClasse vinculado 004, refactor Personagem/HabilidadeDePersonagem/Classe)
- Phase 4 US4 Infrastructure: T053–T059 ✓ (mapeamentos EF inline, migration `20260909002441_NiveisEProgressao` gerada e aplicada, contagens 003 preservadas)
- Phase 4 US4 Seed Assets: T082 ✓ (**80/80 combinações Classe × Aparência marcadas `Coletado` vinculadas ao inventário Feature 004**)

**Contagens SQL Server pós-checkpoint 1**:
- Classes: 20 (preservado)
- Habilidades: 219 (preservado)
- ClassesHabilidades: 277 (preservado)
- AssetsDeClasse: **80 novas linhas — todas Coletado**
- NiveisDeHabilidade: 0 (aguarda mineração wiki + seed refactor)

**Testes**: 141/141 verdes preservados (0 regressão).

**Pendências (manuais / bloqueadas por mineração wiki)**:
- T013–T020 mineração da wiki oficial (20 arquivos JSON de snapshot)
- T060–T081 refatoração dos 22 seeds de habilidade para 5 níveis
- T104, T110 correções seletivas US2 (depende do relatório de auditoria)

**Pendências estruturais opcionais (sessão futura)**:
- T021–T038 Auditoria service + controller (US1) — pode ser feito com wiki-snapshots vazios/mock
- T083–T101 testes US4 (podem ser criados agora)
- T111–T135 Publicador atômico US3 (pode ser feito estruturalmente)

### Checkpoint 2 — 2026-09-09 (mineração wiki 100%)

**Concluído neste checkpoint**:
- T013–T020 **mineração 100%** — 20/20 arquivos preenchidos em `wiki-snapshots/*.json`
  - 220 habilidades documentadas (11 por classe: 7 combate + 4 acampamento)
  - **1 100 níveis extraídos** (5 níveis × 220 habilidades) via templates MediaWiki `{{heroability|valuelvN=|accuracylvN=|critlvN=|effectlvN=|selflvN=}}` e `{{CampSkills|timecostN=|targetN=|descriptionN=}}`
  - Fonte: `https://darkestdungeon.wiki.gg/wiki/{Classe}_(Darkest_Dungeon)?action=raw` (wikitext raw, não HTML renderizado)
- Nomes canônicos PT-BR corrigidos em `tasks.md` (salteador→bandido, homem-de-armas→veterano, monja→flagelante, jesuita→rompedor, arauto→abominacao, anticristo→antiquario, cao-de-caca→mestre-de-caca, aviador→bobo-da-corte, xama→fugitivo)

**Testes**: 191/191 verdes preservados (88 Domain + 25 Architecture + 78 API).

**Desbloqueado**:
- T060–T081 refatoração dos 22 seeds — pode agora consultar os JSONs para preencher `Habilidade.Niveis[1..5]`
- T137 rodar os 9 cenários do quickstart (assim que Docker Desktop + SQL Server estiverem ativos)
- T138 atualizar `dados-minerados.md` com seção "Auditoria da Feature 005"
- T139 documentar FR-013 (chance base > 100% capada) — chances 110/120/130/140/150 documentadas em vários efeitos (Bleed/Blight/Stun/Debuff) confirmam a mecânica

**Bloqueado por infraestrutura**:
- Auditoria real via `GET /api/auditoria/relatorio` requer Docker Desktop + container `mssql-dd` rodando

### Checkpoint 3 — 2026-09-09 (aplicador de níveis + T060-T081)

**Concluído neste checkpoint**:
- T060–T081 **substituídas** por um único componente `AplicadorDeNiveisDoWikiSnapshot`
  - `src/DarkestDungeon.Infrastructure/Data/Seeds/AplicadorDeNiveisDoWikiSnapshot.cs`
  - Lê os 20 wiki-snapshots preenchidos e chama `Habilidade.DefinirNiveis()` em cada uma
  - **Combate**: extrai `campos.modificadorDano[1..5]`, `campos.modificadorAcerto[1..5]`, `campos.modificadorCritico[1..5]` + `efeitos[].valorPorNivel[1..5]` + `efeitos[].chancePorNivel[1..5]`
  - **Acampamento**: replica `custoDeDescanso` × 5 níveis idênticos + `efeitos[].valor` como `ValorDeEfeito`
  - **Fallback**: para as 3 shared globais (Encourage/Wound Care/Pep Talk) que não aparecem nos snapshots, gera 5 níveis idênticos derivados do próprio objeto (`ModificadorDano/Acerto/Critico` de `HabilidadeDeCombate`, `CustoDeDescanso` de `HabilidadeDeAcampamento`)
  - Chamado em `src/DarkestDungeon.Api/Program.cs` após `HabilidadesSeed.Materializar()` e antes de salvar habilidades novas — logga `[Feature 005] Níveis Wiki: aplicadas=X, fallback=Y, ignoradas=Z`
- T095 concluída: `AplicadorDeNiveisDoWikiSnapshotTests` com 4 casos verdes (5 níveis 1..5 em todas as habilidades, progressão real do Smite, valores de efeito do Punish, custo de descanso replicado do Zealous Vigil)

**Testes**: 195/195 verdes (+4 novos casos do aplicador).

**Rationale**: Substituir a refatoração de 22 arquivos de seed manuais (~2000+ linhas) por um componente único (~370 linhas) que consome a fonte de verdade (JSONs) diretamente. Vantagens:
- Fonte única de verdade: mudar um valor no snapshot já reflete no seed via runtime
- Não duplica lógica de mineração vs seed
- Testes exercitam o parser (não os literais dos seeds)
- Ainda respeita a arquitetura em camadas — o componente vive em Infrastructure

**Pendente para completude 100% da feature**:
- T137–T139 polish (bloqueado por Docker Desktop offline neste checkpoint)
- T030 `AuditoriaJobRunner` CLI

## Convenções PT-BR das mensagens (fonte única de verdade)

Strings padronizadas usadas nas rejeições 400 e mensagens de erro (referenciadas por FR-007c/d/m, SC-013, publicação):

| Situação | Mensagem PT-BR |
|---|---|
| Aparência fora do enum (SC-013) | `"Aparência inválida. Valores aceitos: A, B, C, D."` |
| 4ª habilidade equipada acampamento (FR-007c) | `"Apenas 3 habilidades de acampamento podem estar equipadas simultaneamente. Desequipe uma antes."` |
| Equipar habilidade não treinada (FR-007d) | `"A habilidade '{nome}' ainda não foi treinada (Nível 0). Treine-a antes de equipá-la."` |
| Usar habilidade bloqueada (Nível 0) | `"A habilidade '{nome}' ainda não foi treinada (Nível 0). Treine-a antes de usá-la."` |
| Subir de nível com XP insuficiente (FR-007m) | `"Experiência insuficiente para subir para o Nível {n}. Faltam {x} pontos de experiência."` |
| Habilidade fora da associação Classe × Habilidade (FR-007e) | `"A habilidade '{nome}' não pertence à classe '{classe}'."` |
| Publicação — janela ativa (FR-009c) | `"Publicação bloqueada: existem {n} sessões ativas. Aguarde janela de manutenção."` |
| Publicação — confirmação ausente | `"Confirmação obrigatória: 'confirmacaoJanelaManutencao' deve ser true."` |
| Publicação — outra em curso | `"Publicação em andamento (PublicacaoId={id}). Aguarde a conclusão."` |
| Publicação — rollback | `"Publicação falhou; rollback aplicado. Consulte /api/publicacao/{id}/logs para detalhes."` |

## Dependência da Feature 004 (inventário Spine)

- **Verificação (T006)**: **Inventário disponível** em `assets/herois/inventario.json` (1.7MB, 34k linhas).
- **Estrutura**: cada arquivo é `{ Classe (PT-BR), CaminhoOrigem, CaminhoDestino, Sha256, TamanhoBytes, Reutilizado }`.
- **Aparências**: pastas `{classe}_A`, `{classe}_B`, `{classe}_C`, `{classe}_D` (mapeamento 1:1 com `AparenciaDePersonagem`).
- **Chave de vínculo escolhida**: para cada Classe × Aparência, o `AssetsDeClasse` referenciará o `CaminhoDestino` do arquivo `{classe}_{X}/{classe}_portrait_roster.png` como identificador do "conjunto principal" daquela aparência, e o `Sha256` correspondente como `HashArquivo`. Detalhes de textura/atlas/skel completos permanecem consultáveis via inventário.
