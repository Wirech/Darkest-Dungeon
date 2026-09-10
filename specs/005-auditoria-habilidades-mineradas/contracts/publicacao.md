# Contract — Publicação Atômica (Feature 005)

**Base URL**: `/api/publicacao`
**Content-Type**: `application/json; charset=utf-8`
**Idioma**: PT-BR em mensagens

Endpoints da publicação transacional. Cobrem FR-009, FR-009a, FR-009b, FR-009c e SC-016, SC-017.

---

## POST /api/publicacao

Inicia uma publicação atômica da versão auditada do seed para o SQL Server.

### Request

```http
POST /api/publicacao
Content-Type: application/json

{
  "confirmacaoJanelaManutencao": true,
  "observacao": "Publicação pós-auditoria wiki 2026-09-08"
}
```

- `confirmacaoJanelaManutencao` (bool, obrigatório): operador confirma que está em janela de manutenção.
- `observacao` (string, opcional): livre, até 500 chars — vai como primeira entrada no log.

### Response 202 Accepted (caminho feliz)

```json
{
  "publicacaoId": "8f27c1a4-...",
  "iniciadaEm": "2026-09-08T22:15:00Z",
  "totalHabilidades": 219,
  "totalNiveis": 1095,
  "linkStatus": "/api/publicacao/8f27c1a4-.../status"
}
```

### Response 400 Bad Request — Janela ativa (FR-009c)

```json
{
  "titulo": "Publicação bloqueada",
  "mensagem": "Existem 3 sessões de jogador ativas. Aguarde a janela de manutenção antes de publicar.",
  "sessoesAtivas": 3
}
```

### Response 400 Bad Request — Confirmação ausente

```json
{
  "titulo": "Confirmação obrigatória",
  "mensagem": "O operador MUST confirmar 'confirmacaoJanelaManutencao=true' para iniciar a publicação."
}
```

### Response 409 Conflict — Outra publicação em curso

```json
{
  "titulo": "Publicação em andamento",
  "mensagem": "Já existe uma publicação em execução (PublicacaoId=...). Aguarde a conclusão."
}
```

### Response 500 Internal Server Error — Rollback executado

```json
{
  "titulo": "Publicação falhou; rollback aplicado",
  "mensagem": "A transação foi revertida. Nenhuma habilidade foi alterada. Consulte /api/publicacao/{id}/logs para detalhes.",
  "publicacaoId": "8f27c1a4-...",
  "campoQueFalhou": "niveis[3].modificadorDano",
  "habilidadeAfetada": "Estocada"
}
```

---

## GET /api/publicacao/{publicacaoId}/status

Consulta status de uma publicação.

### Response 200 OK

```json
{
  "publicacaoId": "8f27c1a4-...",
  "estado": "Concluida",
  "iniciadaEm": "2026-09-08T22:15:00Z",
  "concluidaEm": "2026-09-08T22:15:22Z",
  "habilidadesAtualizadas": 219,
  "niveisAtualizados": 1095,
  "erros": 0
}
```

Estados possíveis: `EmExecucao`, `Concluida`, `RollbackAplicado`.

### Response 404

Publicação desconhecida.

---

## GET /api/publicacao/{publicacaoId}/logs

Retorna o log estruturado da publicação. SC-017 exige resposta em < 10s.

### Query params

- `nivel` (opcional): filtra `Info`|`Warn`|`Error`.
- `campo` (opcional): filtra por nome do campo (ex.: `niveis[3].modificadorDano`).

### Response 200 OK

```json
{
  "publicacaoId": "8f27c1a4-...",
  "total": 5,
  "itens": [
    {
      "timestamp": "2026-09-08T22:15:00Z",
      "nivel": "Info",
      "mensagem": "Publicação iniciada. Observação: 'Publicação pós-auditoria wiki 2026-09-08'"
    },
    {
      "timestamp": "2026-09-08T22:15:12Z",
      "nivel": "Error",
      "habilidadeId": "c2b7...",
      "nomeExibicao": "Estocada",
      "campo": "niveis[3].modificadorDano",
      "mensagem": "Valor 'quinze' inválido — esperado int",
      "stackTrace": "..."
    },
    {
      "timestamp": "2026-09-08T22:15:12Z",
      "nivel": "Warn",
      "mensagem": "Iniciando rollback total (SC-016)"
    }
  ]
}
```

---

## Contract Tests exigidos

| Cenário | Endpoint | Método | Status | Validação |
|---|---|---|---|---|
| Publicação bem-sucedida (0 sessões, dados válidos) | `/api/publicacao` | POST | 202 | `publicacaoId` GUID; contagens preservadas depois |
| Confirmação ausente | `/api/publicacao` | POST | 400 | Mensagem PT-BR "Confirmação obrigatória" |
| Janela ativa (mock retorna 2 sessões) | `/api/publicacao` | POST | 400 | Mensagem PT-BR menciona "sessões ativas" |
| Publicação já em curso | `/api/publicacao` | POST | 409 | Mensagem PT-BR "Publicação em andamento" |
| Rollback em falha simulada de uma habilidade | `/api/publicacao` | POST | 500 | Nenhuma linha alterada; log tem entrada `Error` com `campo` (SC-016) |
| Status de publicação existente | `/api/publicacao/{id}/status` | GET | 200 | Estados válidos |
| Status de publicação desconhecida | `/api/publicacao/00000000-.../status` | GET | 404 | PT-BR |
| Logs por publicação | `/api/publicacao/{id}/logs` | GET | 200 | Ordem cronológica; resposta < 10s (SC-017) |
| Logs filtrados por campo | `/api/publicacao/{id}/logs?campo=niveis[3].modificadorDano` | GET | 200 | Apenas entradas com aquele campo |

---

## Regras Constitucionais Verificadas

| Princípio | Aplicação neste contrato |
|---|---|
| III. Contratos verificáveis | Fluxo de sucesso (202) + erros relevantes (400 x2, 409, 500) todos com contract test. |
| IV. PT-BR | Todas as mensagens `titulo`/`mensagem` em PT-BR. |
| V. Simplicidade | Uma única transação síncrona; sem fila, sem worker externo. Cabe no request HTTP (~15-30s), com timeout configurável. |
