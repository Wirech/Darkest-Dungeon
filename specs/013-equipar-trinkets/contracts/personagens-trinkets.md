# Contrato: Trinkets no personagem e no card

Estende [specs/009-atributos-oficiais-personagem/contracts/personagens-atributos.md](../../009-atributos-oficiais-personagem/contracts/personagens-atributos.md). `POST /personagens` **não muda**. `IPublicadorAtomicoService` **não** é chamado.

Campos JSON em camelCase. Erros: `ErroResponse` existente, mensagens PT-BR.

## Ficha no payload

Objeto reutilizado em lista e detalhe:

```json
{
  "hpMaximo": 33,
  "hpAtual": 33,
  "precisao": 0,
  "protecao": 0,
  "esquiva": 5,
  "velocidade": 1,
  "critico": 3.0,
  "danoBaseMinimo": 6,
  "danoBaseMaximo": 12,
  "chanceDeVirtude": 25,
  "resistencias": {
    "atordoamento": 50,
    "sangramento": 40,
    "envenenamento": 30,
    "debuff": 30,
    "movimento": 40,
    "doenca": 30,
    "golpeMortal": 67,
    "armadilha": 10
  }
}
```

`hpAtual` na **base** é o persistido. Na **efetiva**, é o persistido limitado a `[1, hpMaximo efetivo]`. Stress, passos, habilidades, arma/armadura, extras da classe **não** entram neste objeto.

Atributos de topo do card (HP, precisão, etc. já existentes no resumo) **MUST** coincidir com a ficha **efetiva**.

## Espaços posicionais

```json
{
  "espacoTrinket1": { "acessorioId": null, "nomeExibicao": null, "raridade": null },
  "espacoTrinket2": { "acessorioId": null, "nomeExibicao": null, "raridade": null }
}
```

Id órfão (acessório apagado): o espaço serializa como vazio. Nomes JSON estáveis: `espacoTrinket1`, `espacoTrinket2`, `fichaBase`, `fichaEfetiva`.

## Listar para o card

`GET /personagens`

Cada item inclui, além do contrato 009:

- `espacoTrinket1`, `espacoTrinket2` (formato acima)
- `fichaBase`
- `fichaEfetiva`

Campos numéricos de combate já presentes no resumo = valores **efetivos**.

A lista **não** chama a wiki.

## Detalhe

`GET /personagens/{id}`

Mesmos `espacoTrinket1` / `espacoTrinket2`, `fichaBase`, `fichaEfetiva`.  
`acessoriosEquipadosIds` legado MAY permanecer (IDs não nulos), mas o card usa os dois espaços posicionais.

`404` se o personagem não existir.

## Lista de trinkets utilizáveis

`GET /acessorios?classe={ClasseDeHeroi}&excluirId={guid opcional}`

- `classe` obrigatória (enum já usado no catálogo).
- Resposta: lista de acessórios **sem** restrição de classe **ou** exclusivos daquela classe.
- Se `excluirId` informado, esse item **não** aparece (o outro espaço).
- Cada item: `id`, `nomeExibicao`, `nomeOriginal`, `raridade`, `classeExclusiva`, `efeitos[]` (nome, valor, unidade, sinal). Ícone 006 opcional; ausência não omite o item.
- Lista vazia: `200` com `[]` (não é erro).
- `400` se `classe` inválida.

Não listar arma/armadura/consumível.

## Equipar / esvaziar um espaço

`PUT /personagens/{id}/acessorios/{espaco}`

`espaco` ∈ {1, 2}.

### Request

```json
{ "acessorioId": "3fa85f64-5717-4562-b3fc-2c963f66afa6" }
```

`acessorioId: null` esvazia o espaço.

### Sucesso `200 OK`

Corpo = detalhe atualizado (`fichaBase` inalterada atributo a atributo cobertos; `fichaEfetiva` recalculada; só o espaço alvo muda).

### Erros (estado anterior preservado)

| Status | Quando |
|---|---|
| `400` | Espaço ≠ 1 ou 2; trinket inexistente; exclusivo de outra classe; mesmo Id já no outro espaço; corpo inválido |
| `404` | Personagem inexistente |

Trocar A→B no mesmo espaço é **uma** transação.

## `POST /personagens/{id}/equipar` (legado)

Continua para arma/armadura. Se `acessoriosIds` for **omitido**, os dois espaços de trinket **MUST NOT** ser zerados. Se enviado, aplica o par completo (0–2 ids) com as mesmas recusas de classe/duplicata.

O card **não** usa este POST para trinket.

## `POST /personagens`

Inalterado: sem seletor de trinket; espaços nascem vazios; ficha efetiva = ficha base.

## UI (card)

- Dois espaços com rótulos PT-BR; estado vazio explícito.
- Abertura da lista: `GET /acessorios` com a classe do herói e `excluirId` do outro espaço.
- Seleção: `PUT` do espaço; desabilitar o controle até a resposta (FR US3.6).
- Mostrar **ficha efetiva em destaque** e **ficha base visível**.
- Lista vazia: informar que não há trinkets disponíveis; card não quebra.
- Sem sucesso visual se o PUT falhou.
---
