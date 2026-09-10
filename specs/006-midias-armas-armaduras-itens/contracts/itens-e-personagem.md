# Contract — Consulta de Item e Personagem com Mídia (Feature 006)

**Idioma**: PT-BR  
Extensões **compatíveis** dos GET já existentes. Campos novos são opcionais; ausência de vínculo não muda código HTTP de sucesso.

Rotas atuais (Feature 003):

- `GET /itens/{id}`
- `GET` de personagem já exposto pelo controller de seres/personagens (mesmo DTO `PersonagemDetalheDto`)

Não criar `GET /api/itens` paralelo. Manter `itens` sem prefixo `/api` como hoje.

---

## GET /itens/{id}

### Response 200 — arma

Campos 003 preservados. Acrescentar mídia por nível.

```json
{
  "id": "8a3c0000-0000-0000-0000-000000000001",
  "tipo": "Arma",
  "nomeExibicao": "Espada Longa",
  "nomeOriginal": "Long Sword",
  "descricao": "...",
  "classeElegivel": "Cruzado",
  "niveisArma": [
    {
      "nivel": 1,
      "danoMinimo": 4,
      "danoMaximo": 7,
      "critico": 5.0,
      "velocidade": 1,
      "midia": {
        "status": "OK",
        "arquivoInventarioId": "cruzado/weapon_1.png",
        "conjuntoSpineId": null,
        "hashArquivo": "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef"
      }
    }
  ]
}
```

Nível pendente:

```json
"midia": {
  "status": "Pendente",
  "arquivoInventarioId": null,
  "conjuntoSpineId": null,
  "hashArquivo": null
}
```

Atributos numéricos MUST ser idênticos ao seed 003.

### Response 200 — armadura

`niveisArmadura[]` com o mesmo objeto `midia`.

### Response 200 — acessório / acampamento / consumível

```json
{
  "id": "...",
  "tipo": "Acessorio",
  "raridade": "Comum",
  "classeExclusiva": null,
  "conjuntoId": null,
  "efeitosAcessorio": [],
  "midia": { "status": "OK", "arquivoInventarioId": "...", "conjuntoSpineId": null, "hashArquivo": "..." }
}
```

`tipo` enum estendido: `Arma` | `Armadura` | `Acessorio` | `ItemDeAcampamento` | `Consumivel`.

### Response 404

Inalterado: item inexistente.

Item **existente** com mídia pendente **não** é 404.

---

## GET personagem (detalhe existente)

Acrescentar projeções opcionais; FKs de equipamento inalteradas.

```json
{
  "id": "...",
  "armaEquipadaId": "8a3c...",
  "armaduraEquipadaId": "8a3d...",
  "acessoriosEquipadosIds": ["..."],
  "midiaArmaEquipada": {
    "nivel": 1,
    "status": "Pendente",
    "arquivoInventarioId": null,
    "conjuntoSpineId": null,
    "hashArquivo": null
  },
  "midiaArmaduraEquipada": null,
  "midiasAcessoriosEquipados": [
    {
      "acessorioId": "...",
      "status": "OK",
      "arquivoInventarioId": "...",
      "hashArquivo": "..."
    }
  ]
}
```

- Sem arma equipada: `midiaArmaEquipada` = `null`.
- Arma equipada e vínculo pendente: objeto com `status: "Pendente"`, HTTP 200.
- Publicação 006 **não** altera `armaEquipadaId` / `armaduraEquipadaId` / `acessoriosEquipadosIds`.

---

## POST /armas, /armaduras, /acessorios

Sem mudança de contrato de criação 003. Mídia nasce `Pendente` até `POST /api/midias/publicacao`.

Novos POST (mínimos, mesmo estilo):

- `POST /itens-acampamento`
- `POST /consumiveis`

Body: `nomeExibicao`, `nomeOriginal`, `descricao`. Sem raridade. Usados sobretudo pela publicação; testes cobrem sucesso + validação 400 (nome vazio / > 80).

---

## Contract tests exigidos

1. GET arma seed 003: 5 níveis numéricos + `midia` por nível.
2. GET item pendente: 200, `status: "Pendente"`.
3. GET personagem após publicar arma: mesmos IDs de equipamento; `midiaArmaEquipada` preenchida se OK.
4. GET `tipo` acampamento/consumível após criação.
5. POST item acampamento nome vazio → 400 PT-BR.
6. Igualdade de hash entre inventário e `hashArquivo` do DTO (teste de integração com fixture).
