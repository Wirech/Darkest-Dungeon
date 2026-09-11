# Contrato de Integração: Personagens

Os contratos abaixo são consumidos pela interface de personagens e usam o formato de erro PT-BR do backend.

## Listar personagens

`GET /personagens`

### Sucesso `200 OK`

```json
[
  {
    "id": "00000000-0000-0000-0000-000000000001",
    "nome": "Cruzado",
    "classe": "Cruzado",
    "nomeClasse": "Cruzado",
    "hpAtual": 33,
    "hpMaximo": 33,
    "stress": 0,
    "nivel": 0,
    "habilidades": [
      {
        "id": "00000000-0000-0000-0000-000000000002",
        "nome": "Golpe",
        "categoria": "Combate",
        "habilitada": true,
        "treinada": false,
        "equipada": false,
        "numeroDoNivel": 0
      }
    ]
  }
]
```

Lista vazia retorna `200 OK` com `[]`, não `404`.

## Criar personagem

`POST /personagens`

O corpo usa os campos de `CriarPersonagemRequest`, incluindo dados básicos, classe, stress, chance de virtude e habilidades selecionadas.

### Sucesso `201 Created`

Retorna o personagem criado e o endereço do recurso no cabeçalho `Location`.

### Erros

- `400 Bad Request`: corpo inválido, campo fora de faixa, classe inexistente ou habilidade não pertencente à classe.
- `500 Internal Server Error`: falha inesperada; a interface mostra mensagem genérica sem perder os dados preenchidos.

## Excluir personagem

`DELETE /personagens/{id}`

### Sucesso `204 No Content`

O personagem deixa de existir e não há corpo de resposta.

### Erros

- `404 Not Found`: ID não corresponde a personagem; a interface informa o conflito.
- `400 Bad Request`: ID inválido.
- `409 Conflict`: exclusão impedida por dependência ou regra de negócio, quando aplicável.

## Catálogos usados pelo formulário

- `GET /classes`: carrega classes selecionáveis.
- `GET /classes/{id}/habilidades`: carrega habilidades válidas para a classe escolhida.

Falhas nesses catálogos deixam o formulário em erro e impedem o envio de referências incompletas.

## Formato de erro

Quando disponível, a interface apresenta `mensagem` e erros por campo do contrato `ErroResponse`, sem exibir stack trace.