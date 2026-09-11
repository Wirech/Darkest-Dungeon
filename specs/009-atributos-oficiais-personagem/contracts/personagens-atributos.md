# Contrato: Criação fiel e card de personagem

Estende [specs/008-criacao-personagem-completa/contracts/personagens-criacao.md](../../008-criacao-personagem-completa/contracts/personagens-criacao.md). Os seis campos de entrada **não mudam**.

## Criar personagem fiel

`POST /personagens`

### Request (inalterado)

```json
{
  "nome": "Reynauld",
  "classe": 4,
  "nivel": 3,
  "nivelDaArma": 2,
  "nivelDaArmadura": 3,
  "aparencia": 1
}
```

O cliente **não** envia HP, velocidade, precisão, proteção, movimento, resistências, IDs de item nem habilidades.

### Sucesso `201 Created`

O corpo inclui pelo menos:

- HP atual/máximo = MAX HP oficial da armadura no `nivelDaArmadura`
- esquiva da armadura; dano/crítico/velocidade da arma no `nivelDaArma`
- `precisao`: 0
- `protecao`: 0
- `stress`: 0
- `chanceDeVirtude`: 25
- `passosAFrente` e `passosAtras` da classe (dois inteiros; **não** um único `movimento` de herói)
- oito resistências efetivas (seis escaláveis com +10 p.p. × `nivel`, teto 100; golpe mortal e armadilha = base)
- habilidades com `categoria` e `numeroDoNivel` (4+4 no nível 1)
- `nivel`, `nivelDaArma`, `nivelDaArmadura`, `aparencia`
- extras da classe no payload do card/lista (não como campos gravados no personagem): `classeReligiosa`, `provisaoInicial`, `bonusAoCriticoDaClasse`

Abominação usa somente números da forma humana.

### Erros

| Status | Quando |
|---|---|
| `400` | Faixa inválida (nível 0..6, arma/armadura 1..5, aparência, nome). Classe sem 4+4 habilidades. |
| `404` | Classe inexistente. |
| `409` ou `400` de catálogo | Falta arma/armadura oficial, nível, passos frente/atrás ou cobertura 20×5 incompleta para a criação fiel. Mensagem PT-BR; **não** inventar valores. |
| `500` | Falha de persistência; nenhum personagem parcial. |

A operação **não** acessa `wiki.gg`.

## Listar para o card

`GET /personagens`

Cada item da lista deve bastar para o card **sem** segundo request:

```json
{
  "id": "...",
  "nome": "Reynauld",
  "classe": 4,
  "nomeClasse": "Cruzado",
  "nivel": 3,
  "nivelDaArma": 2,
  "nivelDaArmadura": 3,
  "aparencia": 1,
  "hpAtual": 33,
  "hpMaximo": 33,
  "stress": 0,
  "precisao": 0,
  "protecao": 0,
  "esquiva": 5,
  "velocidade": 1,
  "critico": 3.0,
  "danoBaseMinimo": 6,
  "danoBaseMaximo": 12,
  "passosAFrente": 1,
  "passosAtras": 1,
  "resistencias": {
    "atordoamento": 70,
    "sangramento": 60,
    "envenenamento": 60,
    "debuff": 60,
    "movimento": 70,
    "doenca": 60,
    "golpeMortal": 67,
    "armadilha": 10
  },
  "classeReligiosa": true,
  "provisaoInicial": "...",
  "bonusAoCriticoDaClasse": "+15% PROT",
  "habilidades": [
    {
      "habilidadeId": "...",
      "nome": "Golpe Sagrado",
      "categoria": "Combate",
      "numeroDoNivel": 1
    },
    {
      "habilidadeId": "...",
      "nome": "Habilidade bloqueada",
      "categoria": "Combate",
      "numeroDoNivel": 0
    }
  ]
}
```

Os números do exemplo são ilustrativos de **formato**; os valores reais vêm dos snapshots oficiais, nunca deste documento.

Habilidades de nível 0 **entram** na lista. Combate e acampamento vêm misturáveis no JSON; o card separa por `categoria`.

## Consultar detalhe

`GET /personagens/{id}`

Mesmos campos de atributos/resistências/passos/extras da classe que a lista. Personagens legados sem passos oficiais: exibir persistido / ausente, sem recálculo.

## UI do card

Textos em PT-BR. Seis categorias obrigatórias: HP; Stress; Atributos; Resistências; Habilidades de combate; Habilidades de acampamento. Cabeçalho com níveis e aparência. Extras da classe visíveis.
