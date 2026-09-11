# Contrato: Criação Completa de Personagem

## Criar personagem

`POST /personagens`

### Request

```json
{
  "nome": "Cruzado da Expedição",
  "classe": 4,
  "nivel": 3,
  "nivelDaArma": 2,
  "nivelDaArmadura": 3,
  "aparencia": 1
}
```

O cliente não envia IDs de habilidades, arma ou armadura para o fluxo padrão. O serviço deriva essas referências a partir da classe e dos níveis escolhidos.

### Sucesso `201 Created`

O retorno deve conter, no mínimo:

```json
{
  "id": "00000000-0000-0000-0000-000000000001",
  "nome": "Cruzado da Expedição",
  "classe": 4,
  "nivel": 3,
  "aparencia": 1,
  "armaEquipadaId": "00000000-0000-0000-0000-000000000002",
  "nivelDaArma": 2,
  "armaduraEquipadaId": "00000000-0000-0000-0000-000000000003",
  "nivelDaArmadura": 3,
  "habilidades": [
    {
      "habilidadeId": "00000000-0000-0000-0000-000000000010",
      "nome": "Golpe",
      "categoria": "Combate",
      "numeroDoNivel": 1,
      "treinada": true,
      "habilitada": true,
      "equipada": false
    },
    {
      "habilidadeId": "00000000-0000-0000-0000-000000000011",
      "nome": "Habilidade bloqueada",
      "categoria": "Combate",
      "numeroDoNivel": 0,
      "treinada": false,
      "habilitada": true,
      "equipada": false
    }
  ]
}
```

O retorno real deve incluir todas as habilidades de combate e acampamento da classe, não somente as duas ilustradas.

### Erros

- `400 Bad Request`: nome/classe ausente, nível de herói fora de 0..6, nível de arma/armadura fora de 1..5, aparência inválida ou classe sem quatro habilidades em alguma categoria.
- `404 Not Found`: classe, arma ou armadura elegível não encontrada.
- `409 Conflict`: catálogo inconsistente, como mais de uma arma/armadura elegível ambígua para a mesma classe/nível.
- `500 Internal Server Error`: falha inesperada de persistência; nenhum personagem parcial deve ser considerado criado.

Os erros devem usar `ErroResponse` e mensagens em PT-BR.

## Consultar personagem

`GET /personagens/{id}`

O detalhe deve expor `aparencia`, `armaEquipadaId`, `nivelDaArma`, `armaduraEquipadaId`, `nivelDaArmadura` e as habilidades com categoria, `numeroDoNivel`, `treinada`, `habilitada` e `equipada`.

## Catálogos usados pela tela

- `GET /classes`: classes disponíveis.
- `GET /classes/{id}/habilidades`: consulta informativa das habilidades da classe; a criação continua sendo responsável pela regra automática 4+4.
- Catálogo de itens: a tela pode apresentar os níveis disponíveis, mas não deve substituir a derivação/validação do serviço.
