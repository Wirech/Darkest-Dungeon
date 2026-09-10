# Phase 0 Research — Auditoria das Habilidades Mineradas

**Feature**: 005-auditoria-habilidades-mineradas
**Date**: 2026-09-08
**Objetivo**: Resolver todas as pendências técnicas do Technical Context antes de gerar `data-model.md` e contratos.

Nenhuma pendência restou marcada `NEEDS CLARIFICATION` no plan.md — a spec já teve 7 clarificações. Este documento consolida as decisões técnicas de suporte (best practices) que serão referenciadas em `tasks.md`.

---

## R1 — Owned Types com Cardinalidade Fixa (5 níveis) em EF Core 10

**Decision**: Mapear `NivelDeHabilidade` como Owned Type via `OwnsMany` na `Habilidade` (raiz TPH), com `HasKey({ HabilidadeId, NumeroDoNivel })` composto e `NumeroDoNivel` restrito a 1..5 via `HasCheckConstraint` na configuração EF.

**Rationale**:

- Já usamos padrão idêntico em `Arma` e `Armadura` da Feature 003 (owned type de níveis de equipamento) — mantém consistência arquitetural.
- Cardinalidade fixa é reforçada em 3 camadas: (a) Domain, no construtor de `Habilidade`, popula 5 `NivelDeHabilidade` obrigatórios; (b) Application, `AuditoriaWikiService` valida `Niveis.Count == 5`; (c) Infrastructure, `HasCheckConstraint("CK_NivelDeHabilidade_Numero", "[NumeroDoNivel] BETWEEN 1 AND 5")` no SQL Server.
- `OwnsMany` gera tabela filha `NiveisDeHabilidade` com FK cascade — combinada com `OnDelete(Restrict)` do agregado `Habilidade` protege contra apagamento acidental.

**Alternatives considered**:

- Tabela separada com entidade `NivelDeHabilidade` de primeira classe → **rejeitada** porque `NivelDeHabilidade` não tem identidade de negócio própria: só existe como parte de uma habilidade específica.
- JSON coluna `Niveis` (owned como JSON) → **rejeitada** porque impede queries SQL agregadas (SC-011 exige `SELECT` de agregação, e a auditoria pode filtrar por "habilidades cujo Level 3 não tem efeito").
- 5 colunas por habilidade (`Nivel1DanoMod`, `Nivel2DanoMod` etc) → **rejeitada** por explodir schema e violar 1NF.

---

## R2 — Transação SQL Atômica cobrindo ~1.100 upserts

**Decision**: Publicador usa `IDbContextTransaction transaction = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted)`, itera todas as habilidades em memória, chama `db.Habilidades.Update(...)` (ou `Add` se GUID novo), e finaliza com `db.SaveChangesAsync()` + `transaction.CommitAsync()`. Em `catch`, `transaction.RollbackAsync()` + log estruturado. Command timeout configurável via `DbContextOptionsBuilder.UseSqlServer(_, opts => opts.CommandTimeout(180))` — padrão 180s (margem confortável para 1.100 upserts em rede local).

**Rationale**:

- `SaveChanges` do EF Core já faz batch (`MARS` desligado / `Server` com `Multiple Active Result Sets` off): tipicamente < 5s para 1.100 upserts em SQL Server local, e < 30s remoto — cabe no timeout padrão. Escolhi 180s como salvaguarda para ambientes com rede degradada.
- `ReadCommitted` é o mínimo suficiente: durante janela de manutenção não há concorrência de leitura significativa (FR-009c garante 0 conexões ativas de jogadores).
- Rollback via `IDbContextTransaction` é atômico no SQL Server 2022 e deixa o banco 100% no estado pré-publicação (SC-016).

**Alternatives considered**:

- `TransactionScope` implícito com `System.Transactions` → **rejeitada** porque exige MSDTC quando cruza connection strings; aqui é sempre single-connection.
- `BulkInsert` de terceiros (EF Extensions) → **rejeitada** — dependência externa paga, e o volume de 1.100 registros não justifica.
- Batch de 100 em 100 commits parciais → **rejeitada** por violar FR-009a (atomicidade absoluta).

---

## R3 — Detecção de "Conexões Ativas de Jogadores"

**Decision**: Consultar `sys.dm_exec_sessions` filtrando `program_name` contendo `DarkestDungeon.Api` e `login_name != 'sa'` (jogadores usam login dedicado configurável — inicialmente apenas contar sessões != da própria conexão de publicação). SQL: `SELECT COUNT(*) FROM sys.dm_exec_sessions WHERE program_name LIKE 'DarkestDungeon.Api%' AND session_id <> @@SPID`. Se retorno > 0 → 400 PT-BR: "Publicação bloqueada: existem N sessões ativas. Aguarde janela de manutenção."

**Rationale**:

- `sys.dm_exec_sessions` é DMV pública com `VIEW SERVER STATE` — permissão default do usuário `sa` já usado. Simples e sem infra adicional (constitucional V).
- Enche o critério "declaração de janela de manutenção" (FR-009c) com verificação concreta em vez de flag manual (que poderia ficar desatualizada).
- `program_name` é setado automaticamente pelo driver .NET SQL Client como `.Net SqlClient Data Provider` por padrão — **decisão de operação**: configurar `Application Name=DarkestDungeon.Api` na connection string da API (não na do publicador). Publicador usa CS distinta com `Application Name=DarkestDungeon.Publisher` para não se contar.

**Alternatives considered**:

- Flag em cache (Redis / memória) atualizada por health check → **rejeitada** por adicionar dependência (Redis) e ficar dessincronizada em falhas.
- Health check HTTP consultado antes → **rejeitada** porque a API poderia estar offline e ainda haver conexões TCP zumbis.
- Manual: exigir que o operador confirme "sim, publicar" → **rejeitada** por não ser verificável em teste automatizado (SC-016/017 exigem teste de rollback e log).

---

## R4 — Log Estruturado de Publicação

**Decision**: Persistir `LogDePublicacao` como tabela `LogsDePublicacao` no mesmo banco com colunas: `Id GUID PK`, `PublicacaoId GUID (agrupa a corrida)`, `Timestamp DATETIME2`, `Nivel VARCHAR(16) (Info/Warn/Error)`, `HabilidadeId GUID? (nullable — nem toda linha refere a uma habilidade)`, `NomeExibicao NVARCHAR(200)?`, `Campo VARCHAR(64)? (ex.: 'modificadorDano', 'niveis[3].efeito', 'connectionActive')`, `Mensagem NVARCHAR(1000)`, `StackTrace NVARCHAR(MAX)?`. Índice `IX_LogsDePublicacao_PublicacaoId_Timestamp` para consulta rápida por corrida.

**Rationale**:

- Consulta por `PublicacaoId` retorna em < 10s (SC-017) com índice B-tree em (PublicacaoId, Timestamp).
- Persistir no mesmo banco simplifica a operação (constitucional V, "simplicidade proporcional") — não precisa de Serilog sink separado nem Application Insights nesta fase.
- Log estruturado em tabela permite queries do relatório: `SELECT Campo, COUNT(*) FROM LogsDePublicacao WHERE PublicacaoId=@id AND Nivel='Error' GROUP BY Campo` para retry-priorização.
- **Importante**: o log **NÃO** entra na transação da publicação — usa uma segunda `DbContext` (ou `NoTracking + IsolationLevel.ReadUncommitted`) para garantir que o log persiste mesmo após rollback. Padrão "outbox invertido".

**Alternatives considered**:

- Serilog + arquivo → **rejeitada** por não ser consultável via SQL (SC-017 mensura por resposta de query).
- Log só em memória com dump em falha → **rejeitada** por perder histórico entre publicações.
- Tabela em banco separado → **rejeitada** por complexidade desnecessária.

---

## R5 — Mineração dos 5 Níveis da Wiki

**Decision**: A mineração da wiki é **atividade manual assistida** — o agente lê a página da classe em `https://darkestdungeon.wiki.gg/wiki/{ClasseNome}` e transcreve as colunas Level 1..5 de cada tabela de habilidade em um bloco JSON por classe. Esse JSON alimenta o refatorador dos partial seed files. A auditoria (`AuditoriaWikiService`) compara o JSON minerado com o seed atual e produz o relatório.

**Rationale**:

- A Feature 003 já validou este padrão (o `dados-minerados.md` documenta 8 batches manuais bem-sucedidos).
- Não há API oficial da wiki; scraping automatizado é frágil (o HTML pode mudar) e viola o princípio de simplicidade.
- 20 classes × ~1-2 min/classe = ~30-40 min de mineração total, dentro do orçamento pessoal do curador.
- O JSON intermediário fica em `specs/005-auditoria-habilidades-mineradas/wiki-snapshots/{classe}.json` — versionável e auditável.

**Alternatives considered**:

- Scraping automatizado com HtmlAgilityPack → **rejeitada** por fragilidade + esforço maior do que ganho.
- API da MediaWiki (`api.php?action=parse`) → **investigada** — funciona mas retorna HTML raw que ainda precisa ser parseado; overhead não compensa para 20 classes.
- Aceitar dados abaixo do padrão wiki → **rejeitada** — contradiz o objetivo da feature.

---

## R6 — Migração de Personagens Existentes

**Decision**: A migration nova `NiveisEProgressao` adiciona:

1. Coluna `Aparencia INT NOT NULL DEFAULT 0` (0 = A) na tabela `Personagens`.
2. Coluna `Experiencia INT NOT NULL DEFAULT 0` na tabela `Personagens`.
3. Coluna `NumeroDoNivel INT NOT NULL` na tabela `HabilidadesDePersonagem` (0..5), **com default 1** para linhas existentes (as 44 habilidades das 4 classes baseline já estavam "aprendidas" no Level 1 conforme mineração original).
4. Tabela nova `NiveisDeHabilidade` (owned) — populada pelo seed refatorado.
5. Tabela nova `AssetsDeClasse` (owned) — populada por `AssetsDeClasseSeed`.
6. Tabela nova `LogsDePublicacao`.
7. Remove colunas `Treinada` e `Habilitada` de `HabilidadesDePersonagem` se existirem (derivadas de `NumeroDoNivel >= 1`).

**Rationale**:

- Compatibilidade retroativa com Personagens existentes (SC-009): defaults preservam semântica ("aparência A, 0 XP, Level 1 nas habilidades já aprendidas").
- EF Core Migrations trata `AddColumn` com default como operação segura em SQL Server 2022 (metadata-only para tipos fixos).
- Remoção de `Treinada`/`Habilitada` só ocorre se essas colunas foram introduzidas em algum ponto — o schema atual (Feature 003) não as tem; o SQL da migration usa `IF COL_LENGTH(...) IS NOT NULL` para idempotência.

**Alternatives considered**:

- Nova migration destrutiva (`DROP TABLE HabilidadesDePersonagem; CREATE ...`) → **rejeitada** por invalidar Personagens existentes (viola SC-009).
- Consolidar tudo numa migration única sobrescrevendo `SchemaCompleto` → **rejeitada** — a Feature 003 já foi publicada em produção; consolidação retrógrada quebra `__EFMigrationsHistory`.

---

## R7 — Assets Visuais das 4 Aparências (Vinculados ao Inventário 004)

**Decision**: `AssetsDeClasse` é modelado como owned collection na `ClasseDeHeroi`, com chave composta (`ClasseId`, `Aparencia`) e duas colunas de referência ao inventário da Feature 004: `ConjuntoSpineId NVARCHAR(64)` (identificador do Conjunto Spine no inventário 004) e `HashArquivo NVARCHAR(64)` (SHA-256 do arquivo principal para validação de integridade). **Não guarda paths livres nem bytes**. A auditoria (FR-007j) apenas verifica que o vínculo existe — a resolução do caminho renderizável final (textura + atlas + esqueleto) ocorre por lookup no inventário 004, que permanece o dono do arquivo físico. Se algum vínculo estiver faltando ou o inventário 004 não estiver disponível no ambiente, todas as 80 combinações Classe × Aparência ficam `Pendente` no Mapa de Cobertura na categoria `AssetsVisuais`.

**Rationale** (revisado na 3ª clarify):

- Alinhamento cross-feature (I1 do `/speckit-analyze`): Feature 004 já é a fonte-de-verdade dos Conjuntos Spine; duplicar caminhos ou bytes em 005 viola simplicidade e cria duas fontes divergentes.
- Simplicidade (constitucional V): 005 registra apenas o **vínculo** (ConjuntoSpineId + hash); 004 continua responsável pelos arquivos e paths.
- Auditoria independente: se o inventário 004 ainda não estiver populado, 005 marca todas as combinações como `Pendente` sem falhar — mantém a auditoria das habilidades (categorias `ResistenciasBase`, `NiveisDeHabilidade`) sempre operacional.
- Rastreabilidade: `HashArquivo` permite detectar substituição silenciosa de asset no inventário 004 sem re-registro em 005 (audit trail).

**Alternatives considered**:

- Guardar bytes no banco (`VARBINARY(MAX)`) → **rejeitada** por inflar backup e violar simplicidade.
- `CaminhoRelativo` livre por Aparência (versão anterior desta pesquisa) → **rejeitada na 3ª clarify** por criar segunda fonte de paths e ignorar a estrutura Spine minerada em 004.
- Enum estático com paths hardcoded → **rejeitada** por impedir novos assets sem recompilar.

---

## R8 — Regras de "Treinada vs Equipada" no Domain

**Decision**: `HabilidadeDePersonagem` fica com apenas 2 campos persistidos: `NumeroDoNivel INT (0..5)` e `Equipada BIT`. Regras derivadas ficam em propriedades computed:

```csharp
public bool Treinada => NumeroDoNivel >= 1;
public bool Habilitada => Treinada; // alias
```

Validação no agregado `Personagem`:

- `TreinarHabilidade(hab, nivel)` — exige `nivel BETWEEN 1 AND 5`; exige que `hab` pertença à associação Classe×Habilidade da Classe do Personagem (FR-007e).
- `EquiparHabilidadeAcampamento(hab)` — exige `hab.Tipo == Acampamento`; exige `Treinada`; exige contagem atual de equipadas < 3; senão lança `RegrasDePersonagemException` PT-BR.
- Combate sem restrição de "equipar" — combate consulta `Habilidades.Where(h => h.Tipo == Combate && h.Treinada)` diretamente.

**Rationale**:

- Elimina flags redundantes (FR-007g), reduz superfície de bug (impossível ter `Treinada=false` mas `Equipada=true`).
- Regras ficam no agregado (constitucional I) e são testáveis unitariamente sem SQL.
- Mensagens em PT-BR na exception (constitucional IV).

**Alternatives considered**:

- Manter `Treinada` como coluna persistida → **rejeitada** por redundância e risco de dessincronização.
- Regras em `Application` (serviço) em vez de agregado → **rejeitada** por vazar invariantes para camada de aplicação.

---

## R9 — Tabela de XP e Subida de Nível (Modo Único)

**Decision**: `TabelaDeExperiencia` é uma classe estática do Domain com uma **única** `IReadOnlyList<int>` contendo os limiares oficiais do modo mais difícil:

```csharp
public static readonly IReadOnlyList<int> Limiares = new[] { 2, 8, 14, 24, 36, 48 };
// atinge Nivel 1, 2, 3, 4, 5, 6 respectivamente
```

Método `NivelDeResolucao Resolver(int experiencia)` retorna o `NivelDeResolucao` correspondente (sem parâmetro de modo). `Personagem.GanharExperiencia(int)` atualiza `Nivel` derivadamente e aplica delta de +10% nas 5 resistências + trap disarm por nível ganho. **A entidade `NivelDeResolucao` expõe `Nome` PT-BR (`Curioso`/`Aprendiz`/`Aventureiro`/`Veterano`/`Mestre`/`Campão`/`Lenda`) e `NomeOriginal` inglês (`Seeker`/`Apprentice`/`Adventurer`/`Veteran`/`Master`/`Champion`/`Legend`)** para rastreabilidade com a wiki, seguindo o padrão `NomeExibicao`/`NomeOriginal` da Feature 003.

**Rationale**:

- Modo único (decisão Q5 da 3ª clarify) elimina complexidade de parâmetro em toda a API sem perder gameplay — Darkest e Stygian têm limiares idênticos oficialmente, e Radiant é rejeitado como modo suportado.
- Tabela como dado imutável do Domain evita magia numérica espalhada.
- Bônus aplicado incrementalmente (delta por nível) simplifica reverter em teste (SC-015 comparando `Nivel=0` vs `Nivel=6`).
- Nomes bilíngues respeitam princípio IV (PT-BR na API) e preservam a fonte da wiki para diff/rastreabilidade.

**Alternatives considered**:

- Persistir tabela no banco como `LimiaresDeXp` → **rejeitada** por ser dado imutável do domínio (nunca vai ser editado pelo jogador).
- Guardar bônus acumulado como coluna `BonusResistenciaAcumulado` → **rejeitada** por violar single source of truth (`Nivel` já implica bônus).
- Manter enum `ModoDeCampanha` com 3 valores → **rejeitada na 3ª clarify** — introduzia edge cases (rebaixamento ao trocar modo) sem valor de gameplay compensatório.

---

## Resumo — Todos os NEEDS CLARIFICATION Resolvidos

| Item | Status |
|---|---|
| Modelagem 5 níveis (owned type) | ✅ R1 |
| Transação atômica ~1.100 upserts | ✅ R2 |
| Detecção conexões ativas | ✅ R3 |
| Log estruturado de publicação | ✅ R4 |
| Mineração da wiki | ✅ R5 |
| Migração de Personagens existentes | ✅ R6 |
| Assets Classe × Aparência | ✅ R7 (revisto 3ª clarify — vincula inventário 004) |
| Regras treinada/equipada | ✅ R8 |
| Tabela XP / subida de Nivel | ✅ R9 (revisto 3ª clarify — modo único + nomes PT-BR) |

**Output**: Todos itens do plan.md prontos para Phase 1.
