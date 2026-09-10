# Contract — Publicação de Vínculos de Mídia (Feature 006)

**Base URL**: `/api/midias/publicacao`
**Content-Type**: `application/json; charset=utf-8`
**Idioma**: PT-BR

Publica vínculos catálogo ↔ inventário **por categoria**, atômica, **a qualquer momento**.  
**Não** reutiliza `POST /api/publicacao` da Feature 005.  
**Não** exige janela de manutenção. **Não** devolve 400 por sessões ativas.

---

## POST /api/midias/publicacao

Aplica todos os vínculos da categoria a partir do inventário local já importado pela CLI.

### Request

```http
POST /api/midias/publicacao
Content-Type: application/json

{
  "categoria": "arma",
  "observacao": "Vínculos após importação 2026-09-10"
}
```

| Campo | Tipo | Obrigatório | Regra |
|---|---|---|---|
| `categoria` | string | sim | `arma` \| `armadura` \| `acessorio` \| `acampamento` \| `consumivel` |
| `observacao` | string | não | máx. 500 |

Não há `confirmacaoJanelaManutencao`.

### Response 202 Accepted

```json
{
  "publicacaoId": "b11e0000-0000-0000-0000-000000000001",
  "categoria": "Arma",
  "iniciadaEm": "2026-09-10T18:10:00Z",
  "linkStatus": "/api/midias/publicacao/b11e0000-0000-0000-0000-000000000001"
}
```

Efeitos:

- Transação única: todos os `Midia` da categoria ou nenhum.
- Identificadores de `Arma`/`Armadura`/`Acessorio` 003 preservados.
- Personagens: nenhum UPDATE de equipamento.
- Categoria `acessorio`: cria acessórios novos só para trinkets sem match; não reseeda 003.
- Categoria `acampamento` / `consumivel`: upsert por `NomeOriginal` dos itens já criados; cria os que ainda não existem.

### Response 400 Bad Request — categoria ausente ou inválida

```json
{
  "titulo": "Categoria inválida",
  "mensagem": "Informe uma categoria: arma, armadura, acessorio, acampamento ou consumivel."
}
```

### Response 409 Conflict — publicação da **mesma** categoria em curso

```json
{
  "titulo": "Publicação em andamento",
  "mensagem": "Já existe uma publicação de vínculos da categoria Arma em execução. Aguarde a conclusão.",
  "publicacaoId": "..."
}
```

Não bloquear outras categorias. **Não** retornar 400 por sessão ao vivo.

### Response 409 Conflict — inventário ausente

```json
{
  "titulo": "Inventário não encontrado",
  "mensagem": "Execute o coletor de mídias de equipamento antes de publicar os vínculos."
}
```

(Alternativa aceitável: 404; o teste de contrato deve fixar **409** para não confundir com item inexistente.)

### Response 500 — rollback aplicado

```json
{
  "titulo": "Publicação falhou; rollback aplicado",
  "mensagem": "A transação foi revertida. Nenhum vínculo desta categoria permaneceu pela metade.",
  "publicacaoId": "...",
  "categoria": "Arma"
}
```

SC-006: após 500, consulta de cobertura da categoria igual ao estado anterior.

---

## GET /api/midias/publicacao/{publicacaoId}

### Response 200 OK

```json
{
  "publicacaoId": "b11e0000-0000-0000-0000-000000000001",
  "categoria": "Arma",
  "estado": "Concluída",
  "iniciadaEm": "2026-09-10T18:10:00Z",
  "concluidaEm": "2026-09-10T18:10:02Z",
  "itensAtualizados": 20,
  "vinculosOk": 87,
  "vinculosPendentes": 13,
  "acessoriosNovos": 0
}
```

`estado`: `Em curso` | `Concluída` | `Rollback aplicado`.

### Response 404 Not Found

```json
{ "titulo": "Publicação não encontrada", "mensagem": "Não há publicação de vínculos com o identificador informado." }
```

---

## Contract tests exigidos

1. POST `categoria=arma` → 202; GET status `Concluída`; 20 armas ainda com 5 níveis numéricos.
2. POST sem categoria → 400 PT-BR.
3. POST paralelo mesma categoria → 409.
4. POST com Personagens existentes → IDs de arma/armadura/acessório do personagem inalterados (comparar GET personagem antes/depois).
5. Falha simulada no meio da transação → 500 e cobertura anterior intacta.
6. POST `acessorio` com trinket novo no inventário → acessório extra `Comum`, efeitos vazios; seed 003 intacto.
7. **Não** existe teste de 400 por sessão ativa neste recurso.
8. POST durante “sessão ao vivo” simulada (detector 005 reportaria > 0) **ainda** retorna 202.

---

## Relação com Feature 005

`POST /api/publicacao` permanece com janela de manutenção. Clientes desta feature usam **somente** `/api/midias/publicacao`.
