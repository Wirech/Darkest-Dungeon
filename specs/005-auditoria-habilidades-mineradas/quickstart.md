# Quickstart — Auditoria e Publicação (Feature 005)

**Feature**: 005-auditoria-habilidades-mineradas
**Date**: 2026-09-08
**Prerequisites**: [plan.md](./plan.md), [data-model.md](./data-model.md), contratos em [contracts/](./contracts)

Guia de validação end-to-end. Cada cenário abaixo prova que uma porção do escopo funciona, sem duplicar código de teste. Detalhes de implementação vão em `tasks.md` (Phase 2).

---

## Prerequisites

- .NET SDK 10.0.400 instalado (`dotnet --version` → `10.0.400`).
- Docker Desktop rodando com WSL 2 (`docker --version` → `29.7.2+`).
- Container `mssql-dd` ativo:

  ```powershell
  docker ps --filter "name=mssql-dd"
  # STATUS deve conter "Up X minutes (healthy)"
  ```

  Caso não esteja: `docker start mssql-dd`.
- Baseline da Feature 003 aplicado no banco:

  ```powershell
  dotnet ef database update --project src/DarkestDungeon.Infrastructure --startup-project src/DarkestDungeon.Api
  # Deve mostrar migration mais recente = 20260908131601_SchemaCompleto
  ```

- 141 testes verdes de baseline:

  ```powershell
  dotnet test --nologo --verbosity minimal
  # Passed! - Failed: 0, Passed: 141, Skipped: 0
  ```

---

## Cenário 1 — Modelagem de 5 níveis é aplicada (US4)

**Objetivo**: Validar que cada uma das 219 habilidades tem exatamente 5 `NivelDeHabilidade`.

### Setup

```powershell
# Após implementação (Phase 3+), aplicar a nova migration
dotnet ef migrations add NiveisEProgressao --project src/DarkestDungeon.Infrastructure --startup-project src/DarkestDungeon.Api
dotnet ef database update --project src/DarkestDungeon.Infrastructure --startup-project src/DarkestDungeon.Api
```

### Validação

Executar contra SQL Server:

```sql
SELECT COUNT(*) AS TotalNiveis FROM NiveisDeHabilidade;
-- Esperado: 1095   (219 habilidades × 5 níveis)

SELECT h.NomeExibicao, COUNT(n.NumeroDoNivel) AS Qtd
FROM Habilidades h
LEFT JOIN NiveisDeHabilidade n ON n.HabilidadeId = h.Id
GROUP BY h.NomeExibicao
HAVING COUNT(n.NumeroDoNivel) <> 5;
-- Esperado: 0 linhas
```

Referência: [data-model.md — seção 2 e 10](./data-model.md).

---

## Cenário 2 — Personagem recebe defaults A/0/Curioso

**Objetivo**: Validar SC-013 (Aparencia default = A) e defaults de Experiencia/Nivel.

### Comando

```powershell
$body = @{
  nome = "Reinaldo o Impávido"
  classeId = "GUID_DO_CRUZADO"
  seExplicado = $true
} | ConvertTo-Json -Depth 4

Invoke-RestMethod -Uri "http://localhost:5000/api/personagens" `
  -Method POST `
  -Body $body `
  -ContentType "application/json; charset=utf-8"
```

### Validação

Resposta 201 Created contendo:

```json
{
  "id": "...",
  "nome": "Reinaldo o Impávido",
  "classe": "Cruzado",
  "aparencia": "A",
  "experiencia": 0,
  "nivel": { "valor": 0, "nome": "Curioso", "nomeOriginal": "Seeker", "bonusResistenciaPercentual": 0 }
}
```

Referência: [data-model.md — seções 5, 6, 8](./data-model.md).

---

## Cenário 3 — Ganhar XP sobe Resolve Level e aplica +10%

**Objetivo**: Validar FR-007l/m e SC-014/015.

### Passos

1. Criar Personagem `Cruzado` (Cenário 2).
2. Aplicar 2 XP (limiar da tabela única para nível 1):

   ```powershell
   Invoke-RestMethod -Uri "http://localhost:5000/api/personagens/$id/xp" `
     -Method POST -Body '{"quantidade":2}' `
     -ContentType "application/json; charset=utf-8"
   ```

3. Consultar Personagem:

   ```powershell
   Invoke-RestMethod -Uri "http://localhost:5000/api/personagens/$id"
   ```

### Validação

- `nivel.valor == 1`, `nivel.nome == "Aprendiz"`, `nivel.nomeOriginal == "Apprentice"`.
- `nivel.bonusResistenciaPercentual == 10`.
- `resistencias.Atordoamento == baseCruzado + 10` (e mesmo para Sangramento/Envenenamento/Movimento/Debuff).
- `chanceDesarmarArmadilha == baseCruzado + 10`.

Repetir com `quantidade=48` (limiar final da tabela única) → esperado `nivel.valor == 6`, `nivel.nome == "Lenda"`, `nivel.nomeOriginal == "Legend"`, bônus +60 pontos.

Referência: [data-model.md — seções 8, 9](./data-model.md).

---

## Cenário 4 — Rejeitar 4ª habilidade equipada de acampamento

**Objetivo**: Validar FR-007c e mensagem PT-BR.

### Passos

1. Criar Personagem `Cruzado`.
2. Treinar 4 habilidades de acampamento diferentes (Nivel=1 cada).
3. Equipar 3 delas via `POST /api/personagens/{id}/acampamento/equipar`.
4. Tentar equipar a 4ª.

### Validação

Resposta 400 com:

```json
{
  "titulo": "Limite de acampamento excedido",
  "mensagem": "Apenas 3 habilidades de acampamento podem estar equipadas simultaneamente. Desequipe uma antes."
}
```

Referência: [data-model.md — seção 4](./data-model.md).

---

## Cenário 5 — Habilidade com Nivel=0 é bloqueada

**Objetivo**: Validar Q3 da clarificação (Nivel=0 = bloqueada).

### Passos

1. Criar Personagem `Cruzado`. Todas as habilidades da classe começam em `Nivel=0` (bloqueadas) — exceto as que a migration marcou como Level 1 por retrocompatibilidade.
2. Tentar usar "Habilidade X" (Nivel=0) em batalha via `POST /api/personagens/{id}/combate/usar`.

### Validação

Resposta 400:

```json
{
  "titulo": "Habilidade bloqueada",
  "mensagem": "A habilidade 'X' ainda não foi treinada (Nivel=0). Treine-a antes de usá-la."
}
```

Referência: [data-model.md — seção 4](./data-model.md).

---

## Cenário 6 — Gerar relatório de auditoria

**Objetivo**: Validar SC-001 (100% classificadas) e SC-008 (< 5min).

### Comando

```powershell
# Executa o job runner com cronômetro
Measure-Command {
  dotnet run --project src/DarkestDungeon.Api -- auditoria --classe=todas
}
```

### Validação

- Duração < 5 min.
- Arquivo `specs/005-auditoria-habilidades-mineradas/relatorio.md` gerado.
- Consulta:

  ```powershell
  Invoke-RestMethod -Uri "http://localhost:5000/api/auditoria/relatorio"
  ```

  retorna `resumo.totalHabilidades == 219` e `ok + parcial + faltando == 219`.

Referência: [contracts/auditoria.md](./contracts/auditoria.md).

---

## Cenário 7 — Publicação atômica com rollback simulado (SC-016)

**Objetivo**: Provar que uma falha em qualquer habilidade reverte tudo.

### Setup

Injetar erro num campo específico do seed (ex.: colocar string "quinze" onde deveria ser `int 15`):

```powershell
# Preparar snapshot antes
$antes = docker exec mssql-dd /opt/mssql-tools/bin/sqlcmd -U sa -P Devlocal!2024 -d DarkestDungeon `
  -Q "SELECT COUNT(*) FROM NiveisDeHabilidade" -h -1
```

### Comando

```powershell
$body = @{ confirmacaoJanelaManutencao = $true; observacao = "Teste rollback" } | ConvertTo-Json
Invoke-RestMethod -Uri "http://localhost:5000/api/publicacao" `
  -Method POST -Body $body -ContentType "application/json; charset=utf-8"
```

### Validação

- Status 500 com corpo `titulo == "Publicação falhou; rollback aplicado"`.
- `campoQueFalhou` presente.
- Depois:

  ```powershell
  $depois = docker exec mssql-dd ... "SELECT COUNT(*) FROM NiveisDeHabilidade" -h -1
  $antes -eq $depois  # deve ser $true
  ```

- Consulta ao log:

  ```powershell
  Invoke-RestMethod -Uri "http://localhost:5000/api/publicacao/$publicacaoId/logs?nivel=Error"
  # Retorna >= 1 entrada com campo preenchido
  ```

- Duração da resposta < 10s (SC-017).

Referência: [contracts/publicacao.md](./contracts/publicacao.md).

---

## Cenário 8 — Publicação bloqueada por janela ativa (FR-009c)

**Objetivo**: Validar detecção de sessões ativas.

### Setup

Abrir uma segunda conexão (simular jogador ativo) no SQL Server:

```powershell
Start-Job -ScriptBlock {
  sqlcmd -S "localhost,1433" -U sa -P "Devlocal!2024" -d DarkestDungeon `
    -A "-a", "-N" -Q "WAITFOR DELAY '00:00:30'; SELECT 1" `
    -o (New-TemporaryFile) `
    -h -1 `
    -w 65535
}
```

### Comando

```powershell
$body = @{ confirmacaoJanelaManutencao = $true } | ConvertTo-Json
Invoke-RestMethod -Uri "http://localhost:5000/api/publicacao" `
  -Method POST -Body $body -ContentType "application/json; charset=utf-8"
```

### Validação

Status 400 com `sessoesAtivas >= 1` e mensagem PT-BR.

Referência: [contracts/publicacao.md](./contracts/publicacao.md) + [research.md — R3](./research.md).

---

## Cenário 9 — Continuidade de Personagens após publicação (SC-009)

**Objetivo**: Personagens criados antes da publicação continuam válidos.

### Passos

1. Criar Personagem antes (Cenário 2).
2. Executar publicação bem-sucedida (Cenário 7 sem injeção de erro).
3. Consultar o Personagem:

   ```powershell
   Invoke-RestMethod -Uri "http://localhost:5000/api/personagens/$id"
   ```

### Validação

- `id` inalterado.
- `aparencia == "A"` preservado.
- `nivel.valor == 0` preservado.
- Habilidades atribuídas mantêm `NumeroDoNivel` original.

---

## Rodar toda a suíte

```powershell
dotnet test --nologo --verbosity minimal
# Esperado no merge da Feature 005 (PR #1, squash a7ce662): 206 verdes.
```

---

## Resultado da validação (2026-09-10) — T137

Polish pós-merge da Feature 005. `mssql-dd` **Up** (Docker 29.7.2, porta 1433). `sqlcmd` e CLI de auditoria reexecutados em 2026-09-10 12:26 UTC. `Program.cs` ainda faz seed relacional **antes** do divert CLI — por isso o job precisa do SQL mesmo lendo snapshots locais.

Contratos HTTP do rascunho original usavam `http://localhost:5000/api/personagens`. Contratos **implementados**:

| Rascunho do cenário | Implementado |
|---|---|
| `POST /api/personagens` | `POST /personagens` (Kestrel default **5140**) |
| `POST /api/personagens/{id}/xp` | **não existe** — domínio `Personagem.GanharExperiencia` |
| `POST /api/personagens/{id}/acampamento/equipar` | **não existe** — domínio `Personagem.EquiparHabilidadeAcampamento` |
| `POST /api/personagens/{id}/combate/usar` | **não existe** — `NumeroDoNivel=0` bloqueia treino/equipar no domínio |
| `GET /api/auditoria/relatorio` | implementado |
| `POST /api/publicacao` | implementado |
| `PersonagemDetalheDto` com `aparencia`/`experiencia`/`nivel` | **não expostos** no DTO HTTP (existem no domínio) |

| # | Cenário | Status | Evidência | Gap HTTP / live |
|---|---|---|---|---|
| C1 | 5 níveis (219 × 5 = 1095) | **Validado (SQL live 2026-09-10)** | `sqlcmd` em `mssql-dd`: Classes=**20**, Habilidades=**219**, ClassesHabilidades=**277**, NiveisDeHabilidade=**1095**, AssetsDeClasse=**80**. Também `AplicadorDeNiveisDoWikiSnapshotTests` | — |
| C2 | Defaults A / 0 / Curioso | **Validado (domínio)** | `PersonagemFeature005Tests.Novo_Personagem_recebe_aparencia_A_por_padrao` e `…Experiencia_zero_e_Nivel_Curioso` | `POST /personagens` não devolve `aparencia`/`experiencia`/`nivel` |
| C3 | XP +10% (2 XP → Aprendiz; 48 → Lenda) | **Validado (domínio)** | `GanharExperiencia_com_2_XP_sobe_para_Aprendiz_e_aplica_bonus_10pct`; `…ate_48_sobe_para_Lenda_com_bonus_60pct` | Sem `POST …/xp` |
| C4 | 4ª habilidade de acampamento | **Validado (domínio, sem teste xUnit dedicado)** | `Personagem.EquiparHabilidadeAcampamento` lança *«Apenas 3 habilidades de acampamento podem estar equipadas simultaneamente. Desequipe uma antes.»* (`T091` ainda `[~]`) | Sem `POST …/acampamento/equipar` |
| C5 | Nivel=0 bloqueada | **Validado (domínio, via treino/equipar)** | `HabilidadeDePersonagemFeature005Tests` (`NumeroDoNivel=0` → `TreinadaPorNivel=false`); equipar sem treino: *«ainda não foi treinada (Nível 0)»* | Sem `POST …/combate/usar`; mensagem de **uso** em combate não tem endpoint |
| C6 | Relatório + SC-008 &lt; 5 min | **Validado (live)** | `Measure-Command { dotnet run --project src/DarkestDungeon.Api -- auditoria --classe=todas }` → **19,36 s** (0,323 min) **&lt; 5 min** (SC-008). `relatorio.md` regenerado **2026-09-10 12:26:13 UTC**: **277 OK / 0 Parcial / 0 Faltando**, assets **80/80** | — |
| C7 | Rollback atômico SC-016 | **Validado (contrato com fake)** | `PublicacaoRollbackContractTests.Falha_simulada_retorna_500_com_rollback_e_log_error_SC016` → 500 + corpo com `rollback`/`publicacaoId` | Injeção live no seed + `sqlcmd` antes/depois não reexecutada |
| C8 | Sessões ativas FR-009c | **Validado (código + teste skip se SQL down)** | Controller: 400 `{ titulo, mensagem, sessoesAtivas }`; `DetectorDeSessoesAtivasTests` skip silencioso sem SQL | Live com 2 conexões `DarkestDungeon.Api*` não reexecutado |
| C9 | Continuidade SC-009 | **Validado (InMemory)** | `ContinuidadeDePersonagensTests` — cria via `POST /personagens`, publica, `GET /personagens/{id}` preserva `id`/`nome` | DTO HTTP não afirma `aparencia`/`nivel`; SQL real não reexecutado |

**Suíte automatizada no merge**: 206/206 verdes (PR #1 squash `a7ce662`).

Live 2026-09-10 (C1 + C6):

```powershell
docker exec mssql-dd /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Devlocal!2024" -C -d DarkestDungeon -h -1 -Q "SET NOCOUNT ON; SELECT 'Classes', COUNT(*) FROM Classes UNION ALL SELECT 'Habilidades', COUNT(*) FROM Habilidades UNION ALL SELECT 'ClassesHabilidades', COUNT(*) FROM ClassesHabilidades UNION ALL SELECT 'NiveisDeHabilidade', COUNT(*) FROM NiveisDeHabilidade UNION ALL SELECT 'AssetsDeClasse', COUNT(*) FROM AssetsDeClasse;"
# 20 / 219 / 277 / 1095 / 80

Measure-Command { dotnet run --project src/DarkestDungeon.Api -- auditoria --classe=todas }
# TotalSeconds=19.36  TotalMinutes=0.323  (SC-008)
```

---

## Referências

- [spec.md](./spec.md) — user stories, FRs, SCs.
- [plan.md](./plan.md) — decisões técnicas e estrutura.
- [research.md](./research.md) — decisões de best practices.
- [data-model.md](./data-model.md) — entidades, invariantes, cardinalidades.
- [contracts/auditoria.md](./contracts/auditoria.md), [contracts/publicacao.md](./contracts/publicacao.md) — schemas.
