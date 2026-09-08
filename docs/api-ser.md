# API Ser

## Criar Ser

```http
POST /seres HTTP/1.1
Content-Type: application/json

{
  "nome": "Cruzado",
  "tipo": "Herói",
  "hpMaximo": 33,
  "hpAtual": 33,
  "velocidade": 1,
  "critico": 5,
  "danoBaseMinimo": 7,
  "danoBaseMaximo": 13,
  "movimento": 2,
  "bonusDeCritico": 0,
  "tamanho": 1,
  "acoesPorTurno": 1,
  "esquiva": 5,
  "precisao": 85,
  "protecao": 0,
  "nivel": 0,
  "resistencias": {
    "atordoamento": 40,
    "sangramento": 30,
    "envenenamento": 20,
    "debuff": 25,
    "movimento": 35
  }
}
```

Resposta esperada: `201 Created`, header `Location` com `/seres/{id}` e corpo
com todos os atributos enviados, incluindo `id`.

## Consultar Ser

```http
GET /seres/{id} HTTP/1.1
```

Resposta esperada para ID existente: `200 OK` com o Ser correspondente.

Resposta esperada para ID inexistente: `404 Not Found`.

```json
{
  "mensagem": "Ser não encontrado."
}
```

Resposta esperada para ID inválido: `400 Bad Request`.

```json
{
  "mensagem": "Identificador inválido.",
  "erros": [
    {
      "campo": "id",
      "mensagem": "Identificador inválido."
    }
  ]
}
```

## Validações

- `hpAtual` deve ser menor ou igual a `hpMaximo`.
- `danoBaseMinimo` deve ser menor ou igual a `danoBaseMaximo`.
- `critico`, `bonusDeCritico`, `protecao` e cada resistência devem estar entre `0` e `100`.
- `tamanho` deve estar entre `0` e `4`.
- `nivel` deve estar entre `0` e `6`.
- `nome` e `tipo` devem ter conteúdo visível.

## Contratos

- O contrato HTTP implementado deve permanecer alinhado a [specs/001-base-api-ser/contracts/openapi.yaml](../specs/001-base-api-ser/contracts/openapi.yaml).
- Os contratos internos de camadas e DI devem permanecer alinhados a [specs/001-base-api-ser/contracts/interfaces.md](../specs/001-base-api-ser/contracts/interfaces.md).
