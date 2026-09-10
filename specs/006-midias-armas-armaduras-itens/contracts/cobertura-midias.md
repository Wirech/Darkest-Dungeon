# Contract — Cobertura de Mídias de Equipamento e Itens (Feature 006)

**Base URL**: `/api/midias`
**Content-Type**: `application/json; charset=utf-8`
**Idioma**: PT-BR em mensagens e em `status` (`OK`, `Parcial`, `Pendente`)

Endpoints de leitura. Não alteram estado. A importação de arquivos permanece na CLI (`tools/DarkestDungeon.MediaCollector`). A publicação de vínculos é outro contrato.

---

## GET /api/midias/cobertura

Retorna o relatório de cobertura das cinco categorias oficiais.

### Request

Query opcional:

- `categoria` (`arma` | `armadura` | `acessorio` | `acampamento` | `consumivel`): filtra `categorias` a um item; `orfaos` e `lacunas` continuam globais.

### Response 200 OK

```json
{
  "geradoEm": "2026-09-10T18:00:00Z",
  "categorias": [
    {
      "categoria": "Arma",
      "esperados": 20,
      "ok": 12,
      "parcial": 5,
      "pendente": 3,
      "linhas": [
        {
          "itemId": "8a3c0000-0000-0000-0000-000000000001",
          "nomeExibicao": "Espada Longa",
          "nomeOriginal": "Long Sword",
          "status": "Parcial",
          "niveisOk": [1, 2, 3],
          "niveisPendentes": [4, 5]
        }
      ]
    },
    {
      "categoria": "Armadura",
      "esperados": 20,
      "ok": 10,
      "parcial": 6,
      "pendente": 4,
      "linhas": []
    },
    {
      "categoria": "Acessório",
      "esperados": 42,
      "ok": 30,
      "parcial": 0,
      "pendente": 12,
      "linhas": []
    },
    {
      "categoria": "Item de acampamento/provisão",
      "esperados": 8,
      "ok": 8,
      "parcial": 0,
      "pendente": 0,
      "linhas": []
    },
    {
      "categoria": "Consumível",
      "esperados": 5,
      "ok": 4,
      "parcial": 0,
      "pendente": 1,
      "linhas": []
    }
  ],
  "orfaos": [
    {
      "caminhoOrigem": "C:\\Jogos\\DarkestDungeon\\inventory\\misc\\unknown.png",
      "sha256": "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
      "motivo": "Arquivo fora das origens mapeadas; permanece no inventário sem categoria de catálogo."
    }
  ],
  "lacunas": [
    {
      "categoria": "Acessório",
      "caminhoConsultado": "C:\\Jogos\\DarkestDungeon\\inventory\\trinkets",
      "motivo": "Pasta esperada não encontrada na instalação.",
      "tentadoEm": "2026-09-10T17:55:01Z"
    }
  ]
}
```

Notas:

- Para arma/armadura, `esperados` conta **itens** (20), não os 100 vínculos; o status do item segue FR-013. `niveisOk` / `niveisPendentes` detalham os cinco níveis.
- Para acessório/acampamento/consumível não há níveis: `parcial` permanece 0; status do item é `OK` ou `Pendente`.
- `esperados` de acessório = registros 003 + acessórios criados nesta feature.
- Soma `ok + parcial + pendente` MUST igualar `esperados` em cada categoria (SC-005).

### Response 400 Bad Request

`categoria` query inválida.

```json
{ "titulo": "Categoria inválida", "mensagem": "Informe arma, armadura, acessorio, acampamento ou consumivel." }
```

---

## GET /api/midias/cobertura/categorias/{categoria}

Atalho equivalente a `GET /api/midias/cobertura?categoria=`.

### Response 200 OK

Mesmo envelope, com `categorias` de um elemento.

### Response 404 Not Found

Somente se a categoria path não for uma das cinco oficiais (não usar 404 para “ainda não publicado”: devolver zeros e `pendente = esperados`).

---

## Contract tests exigidos (Constituição III)

1. Sucesso: cinco categorias presentes; totais somam `esperados`.
2. Filtro `categoria=arma` reduz `categorias` a um elemento.
3. Query inválida → 400 com mensagem PT-BR.
4. Arma com 5 níveis OK → linha `status: "OK"` e `niveisPendentes` vazio.
5. Arma com 2 níveis OK → `status: "Parcial"`.
6. Item sem mídia → `status: "Pendente"` (não 404).
7. Órfão aparece em `orfaos`, não numa sexta categoria.
