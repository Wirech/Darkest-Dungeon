# Quickstart — Mídias de Armas, Armaduras e Itens (Feature 006)

**Feature**: 006-midias-armas-armaduras-itens  
**Date**: 2026-09-10  
**Prerequisites**: [plan.md](./plan.md), [data-model.md](./data-model.md), [contracts/](./contracts)

Validação end-to-end. Sem código de implementação neste arquivo.

---

## Pré-requisitos

- .NET SDK 10 (`dotnet --version` → 10.x).
- Docker Desktop; container `mssql-dd` **Up (healthy)** na porta 1433.
- Baseline Features 003–005 no banco (`dotnet ef database update --project src/DarkestDungeon.Infrastructure --startup-project src/DarkestDungeon.Api`).
- Testes atuais verdes (`dotnet test --nologo --verbosity minimal`).
- Instalação local licenciada de Darkest Dungeon **ou** fixture em `tests/DarkestDungeon.Api.Tests/ColetaMidias/Fixtures/Equipamentos/` (PNG mínimos nas pastas mapeadas).
- Caminho da instalação **não** commitado; passar via `--origem`.

---

## Cenário 1 — Inventariar (US1 / P1)

```powershell
dotnet run --project tools/DarkestDungeon.MediaCollector -- --origem ".\tests\DarkestDungeon.Api.Tests\ColetaMidias\Fixtures\Equipamentos" --saida .\artifacts\midias-006 --simular
```

Esperado: listagem com categorias; nenhum arquivo criado.

```powershell
dotnet run --project tools/DarkestDungeon.MediaCollector -- --origem ".\tests\DarkestDungeon.Api.Tests\ColetaMidias\Fixtures\Equipamentos" --saida .\artifacts\midias-006 --continuar
```

Esperado:

- Cópia byte a byte; SHA-256 origem = destino.
- `artifacts/midias-006/inventario.json` com `declaracaoDeUso`, `categoria` por arquivo.
- Pasta mapeada ausente → `lacunas[]` com caminho e motivo PT-BR; processo não aborta.

---

## Cenário 2 — Deduplicação com Feature 004 (FR-003)

1. Garantir `assets/herois/inventario.json` de uma importação 004 (ou copiar um hash conhecido para o fixture).
2. Colocar na fixture 006 um PNG com os **mesmos bytes**.
3. Importar 006.

Esperado: entrada `reutilizado: true`; sem segundo blob; associação nova no inventário 006.

---

## Cenário 3 — Publicar armas e consultar (US2)

Após migration desta feature:

```powershell
dotnet ef database update --project src/DarkestDungeon.Infrastructure --startup-project src/DarkestDungeon.Api
```

API em `http://localhost:5140`.

```http
POST /api/midias/publicacao
{ "categoria": "arma" }
```

Esperado: 202. Em seguida:

```http
GET /itens/{id-arma-cruzado}
GET /api/midias/cobertura?categoria=arma
```

Esperado:

- 20 armas; cada uma com 5 níveis numéricos intactos (SC-003).
- Cada nível com `midia.status` `OK` ou `Pendente`.
- Arma com 5 ícones → cobertura `OK`; com parte → `Parcial`.

SQL de checagem:

```sql
SELECT COUNT(*) FROM NiveisArma; -- 100
SELECT COUNT(*) FROM NiveisArma WHERE Midia_Status = 'OK' OR Midia_Status = 'Pendente'; -- 100
```

---

## Cenário 4 — Personagem não perde equipamento (FR-009 / SC-006)

1. GET personagem existente; anotar `armaEquipadaId`, `armaduraEquipadaId`, acessórios.
2. POST publicação `arma` (e opcionalmente `armadura`) **com a API em uso** (não precisa zerar sessões).
3. GET o mesmo personagem.

Esperado: mesmos IDs; `midiaArmaEquipada` preenchida ou `Pendente`; HTTP 200.

Contraste com 005: **não** chamar `POST /api/publicacao` da auditoria; aquele fluxo ainda exige janela.

---

## Cenário 5 — Troféus, acampamento, consumível (US3)

```http
POST /api/midias/publicacao
{ "categoria": "acessorio" }
```

Esperado:

- Acessórios 003: Id/raridade/efeitos iguais; só `midia` nova.
- Trinket só na instalação: novo `Acessorio` `Comum`, `efeitosAcessorio: []`, sem classe exclusiva.

```http
POST /api/midias/publicacao
{ "categoria": "acampamento" }
```

Arquivo em `inventory/provision` → `tipo: ItemDeAcampamento`.

```http
POST /api/midias/publicacao
{ "categoria": "consumivel" }
```

Arquivo em `inventory/quest` → `tipo: Consumivel`, nunca acampamento.

Órfão (pasta não mapeada): inventário `NaoAssociado`; aparece em `orfaos` da cobertura; não some.

---

## Cenário 6 — Relatório (US4 / SC-008)

```http
GET /api/midias/cobertura
```

Esperado em ≤ 2 minutos: cinco categorias; `ok + parcial + pendente = esperados`; textos `OK` / `Parcial` / `Pendente` em PT-BR (SC-010).

---

## Cenário 7 — Publicação concorrente e rollback

- Dois POST `categoria=arma` sobrepostos → segundo 409.
- POST `categoria=armadura` enquanto arma publica → permitido (não 409 cruzado).
- Falha injetada (teste) → 500; cobertura da categoria igual à anterior.

---

## Fora deste guia

- Não minerar wiki.
- Não importar `enemies/` nem tiles.
- Não reauditar `AssetsDeClasse` (005).
- Não criar `tasks.md` neste comando.
