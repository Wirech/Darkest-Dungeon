# Contrato: Mídias no card da listagem

**Idioma**: PT-BR  
Estende [specs/009-atributos-oficiais-personagem/contracts/personagens-atributos.md](../../009-atributos-oficiais-personagem/contracts/personagens-atributos.md) e [specs/007-frontend-personagens/contracts/personagens-api.md](../../007-frontend-personagens/contracts/personagens-api.md).

Campos novos são **acrescentados**; HTTP de sucesso da lista/detalhe **não muda** por ausência de arquivo. Esta feature **não** altera `POST /personagens` nem `DELETE /personagens/{id}`.

---

## GET /personagens

A lista MUST ser autossuficiente para o card (sem segundo GET por personagem).

Campos 009 preservados. Acrescentar bloco `midias` (ou campos equivalentes no DTO, serializados em camelCase).

### Response 200 — personagem com acervo completo (formato)

```json
{
  "id": "...",
  "nome": "Reynauld",
  "classe": 4,
  "nomeClasse": "Cruzado",
  "aparencia": 1,
  "nivelDaArma": 2,
  "nivelDaArmadura": 3,
  "habilidades": [
    {
      "habilidadeId": "...",
      "nome": "Golpe Sagrado",
      "categoria": "Combate",
      "numeroDoNivel": 1,
      "midia": {
        "tipo": "habilidade",
        "status": "OK",
        "arquivoInventarioId": "arquivos/cruzado/crusader.ability.one.png",
        "hashArquivo": "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef",
        "url": "/acervo/herois/arquivos/cruzado/crusader.ability.one.png"
      }
    },
    {
      "habilidadeId": "...",
      "nome": "Encorajar",
      "categoria": "Acampamento",
      "numeroDoNivel": 0,
      "midia": {
        "tipo": "habilidade",
        "status": "Pendente",
        "arquivoInventarioId": null,
        "hashArquivo": null,
        "url": null
      }
    }
  ],
  "midias": {
    "retrato": {
      "tipo": "retrato",
      "rotulo": "Retrato",
      "status": "OK",
      "arquivoInventarioId": "arquivos/cruzado/crusader_B/crusader_portrait_roster.png",
      "hashArquivo": "...",
      "url": "/acervo/herois/arquivos/cruzado/crusader_B/crusader_portrait_roster.png"
    },
    "corpoInteiro": {
      "tipo": "corpoInteiro",
      "rotulo": "Corpo inteiro",
      "status": "OK",
      "arquivoInventarioId": "arquivos/cruzado/crusader_B/anim/crusader.sprite.idle.png",
      "hashArquivo": "...",
      "url": "/acervo/herois/arquivos/cruzado/crusader_B/anim/crusader.sprite.idle.png"
    },
    "arma": {
      "tipo": "arma",
      "rotulo": "Arma",
      "status": "OK",
      "nivel": 2,
      "arquivoInventarioId": "arquivos/cruzado/icons_equip/eqp_weapon_1.png",
      "url": "/acervo/herois/arquivos/cruzado/icons_equip/eqp_weapon_1.png"
    },
    "armadura": {
      "tipo": "armadura",
      "rotulo": "Armadura",
      "status": "OK",
      "nivel": 3,
      "arquivoInventarioId": "arquivos/cruzado/icons_equip/eqp_armour_2.png",
      "url": "/acervo/herois/arquivos/cruzado/icons_equip/eqp_armour_2.png"
    }
  }
}
```

Os hashes/URLs do exemplo ilustram **formato**. Retrato e corpo MUST usar a pasta da aparência do item (`aparencia` 1 = B). Arma nível 2 MUST usar `eqp_weapon_1` (índice 0-based). Armadura nível 3 MUST usar `eqp_armour_2`.

Habilidade Nv. 0 MUST aparecer com o **mesmo** `url` que Nv. ≥1 da mesma habilidade, quando o ícone existir.

### Response 200 — ausência

Lista vazia: `[]` (inalterado).

Personagem sem nível de arma:

```json
"arma": {
  "tipo": "arma",
  "rotulo": "Arma",
  "status": "Pendente",
  "nivel": null,
  "arquivoInventarioId": null,
  "url": null
}
```

Arquivo faltando no acervo: mesmo shape `Pendente`, HTTP 200. **Não** 404 na lista.

Dois personagens da mesma classe e aparências distintas MUST ter `url` de retrato/corpo diferentes (pastas `_A` vs `_B` etc.).

### Semântica de URL

- Prefixo `/acervo/herois/` ou `/acervo/equipamentos-itens/`.
- Sem `file://`, sem caminho de disco.
- Cliente: `<img src="{url}">`; `onerror` → texto **sem imagem**.

---

## GET /personagens/{id}

Mesmos slots de retrato, corpo, arma, armadura e `habilidades[].midia` que a lista.

`midiaArmaEquipada` / `midiaArmaduraEquipada` (006) MUST refletir o **nível cadastrado** (`nivelDaArma` / `nivelDaArmadura`), não o nível de resolução. Campos 006 (`status`, `arquivoInventarioId`, `conjuntoSpineId`, `hashArquivo`) permanecem; o card da lista usa o bloco `midias` acima.

Acessórios: inalterados; fora do card.

404 de personagem inexistente: inalterado.

---

## Arquivos estáticos

| URL | Origem no repositório |
|---|---|
| `/acervo/herois/{*caminho}` | `assets/herois/` |
| `/acervo/equipamentos-itens/{*caminho}` | `assets/equipamentos-itens/` |

Arquivo existente: `200` + `image/png` (ou tipo do arquivo).  
Arquivo inexistente: `404` (o card trata como “sem imagem”; a lista HTTP continua 200).  
Caminho fora da raiz mapeada: recusado (não vazar disco).

Não há `POST` de mídia nesta feature.

---

## UI do card (contrato de apresentação)

Consumidor: [wwwroot/personagens](../../../src/DarkestDungeon.Api/wwwroot/personagens/).

O card MUST:

1. Mostrar retrato e corpo inteiro juntos, com rótulos distinguíveis.
2. Mostrar arma e armadura junto dos níveis já exibidos.
3. Mostrar ícone + nome + `Nv. {n}` em cada linha de combate e acampamento listada.
4. Reservar o espaço e escrever **sem imagem** quando `status !== "OK"`, `url` nula ou a imagem falhar.
5. Preservar categorias textuais da 009 e as ações criar/excluir.

---

## Testes de contrato exigidos

1. `GET /personagens` após criar dois Cruzados com aparências distintas: retrato/corpo com pastas `_A` vs outra letra; HTTP 200.
2. Dois personagens mesma classe, `nivelDaArma` 1 e 5: `eqp_weapon_0` vs `eqp_weapon_4` (ou ids equivalentes); não cruzar.
3. Habilidade Nv. 0 e Nv. 3 da mesma `habilidadeId`: mesmo `url` de ícone (ou ambos Pendente).
4. Nível de arma nulo ou mídia ausente: slot `Pendente`, `url` null, lista 200; demais slots intactos.
5. `GET /personagens/{id}`: arma/armadura no nível cadastrado (não na resolução).
6. `GET /acervo/herois/arquivos/.../portrait_roster.png` existente → 200; caminho inexistente → 404.
7. Payload da lista continua incluindo HP, resistências, passos, extras da classe (regressão 009).

Erros de criação/exclusão: inalterados (não retestar além da regressão de que a lista com `Pendente` não quebra o fluxo).
