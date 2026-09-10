# Contract — Auditoria (Feature 005)

**Base URL**: `/api/auditoria`
**Content-Type**: `application/json; charset=utf-8`
**Idioma**: PT-BR em mensagens

Endpoints de leitura (`GET`) — não alteram estado. Servem para o curador consultar o relatório gerado offline. A **execução** da auditoria (mineração + geração do relatorio.md) roda como job local (CLI/console) fora da API pública para respeitar simplicidade (constitucional V).

---

## GET /api/auditoria/relatorio

Retorna o relatório de auditoria mais recente, consolidado.

### Request

Sem parâmetros.

### Response 200 OK

```json
{
  "geradoEm": "2026-09-08T15:22:31Z",
  "resumo": {
    "totalHabilidades": 219,
    "ok": 44,
    "parcial": 128,
    "faltando": 47,
    "niveisPendentes": 12
  },
  "linhas": [
    {
      "habilidadeId": "8a3c...-...",
      "nomeExibicao": "Golpe Sagrado",
      "classeDoDono": "Cruzado",
      "status": "OK",
      "diffs": []
    },
    {
      "habilidadeId": "c2b7...-...",
      "nomeExibicao": "Estocada",
      "classeDoDono": "LadraoDeCova",
      "status": "Parcial",
      "diffs": [
        {
          "campo": "niveis[3].modificadorDano",
          "valorEsperado": "15",
          "valorAtual": "10",
          "observacao": "wiki lista +15% no Level 4"
        }
      ]
    }
  ]
}
```

### Response 404 Not Found

Quando o relatório ainda não foi gerado (arquivo `specs/005-auditoria-habilidades-mineradas/relatorio.md` não existe).

```json
{ "titulo": "Relatório não gerado", "mensagem": "Execute o job de auditoria antes de consultar o relatório." }
```

---

## GET /api/auditoria/relatorio/classes/{classeId}

Filtra o relatório por classe.

### Request

- `classeId` (path): GUID da classe.

### Response 200 OK

Mesma estrutura de `/relatorio`, mas `linhas` apenas da classe requisitada e `resumo` recalculado só para ela.

### Response 404 Not Found

- Se o `classeId` não existe.
- Se o relatório ainda não foi gerado.

---

## GET /api/auditoria/cobertura

Retorna o Mapa de Cobertura atualizado com os itens da Feature 005 (assets Classe × Aparência + níveis pendentes de habilidades).

### Response 200 OK

```json
{
  "categorias": [
    { "categoria": "ResistenciasBase",   "total": 160, "coletados": 160, "pendentes": 0 },
    { "categoria": "NiveisDeHabilidade", "total": 1095, "coletados": 1083, "pendentes": 12 },
    { "categoria": "AssetsVisuais",      "total": 80,  "coletados": 40,   "pendentes": 40 }
  ]
}
```

---

## Contract Tests exigidos

| Cenário | Endpoint | Método | Status | Validação |
|---|---|---|---|---|
| Relatório existente | `/api/auditoria/relatorio` | GET | 200 | JSON respeita schema, `resumo.totalHabilidades == 219` |
| Relatório inexistente | `/api/auditoria/relatorio` | GET | 404 | PT-BR na mensagem |
| Filtro por classe conhecida | `/api/auditoria/relatorio/classes/{id}` | GET | 200 | Todas as linhas têm `classeDoDono` esperado |
| Filtro por classe desconhecida | `/api/auditoria/relatorio/classes/00000000-0000-0000-0000-000000000000` | GET | 404 | PT-BR |
| Cobertura | `/api/auditoria/cobertura` | GET | 200 | 3 categorias mínimas |

---

## Não-endpoints (execução do job)

A geração do relatório é feita por um **console runner** interno em:

- `src/DarkestDungeon.Api/Auditoria/AuditoriaJobRunner.cs` (ativado por CLI arg `--auditoria`)
- Ou por `dotnet run --project src/DarkestDungeon.Api -- auditoria --classe=Cruzado` (mineração seletiva).

Este runner:

1. Lê os snapshots wiki de `specs/005-auditoria-habilidades-mineradas/wiki-snapshots/*.json`.
2. Compara campo a campo com o estado atual do banco.
3. Grava `specs/005-auditoria-habilidades-mineradas/relatorio.md`.
4. Também popula endpoint `/api/auditoria/relatorio` via arquivo lido em runtime.
