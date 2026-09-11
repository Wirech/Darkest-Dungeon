# Contrato: Corpo inteiro composto e animado

**Idioma**: PT-BR  
Estende [specs/010-imagens-card-personagem/contracts/personagens-card-midias.md](../../010-imagens-card-personagem/contracts/personagens-card-midias.md).

Campos novos são **acrescentados** em `midias.corpoInteiro`. HTTP de sucesso da lista/detalhe **não muda** por conjunto incompleto. Esta feature **não** altera `POST /personagens` nem `DELETE /personagens/{id}`. **Não** grava a versão escolhida.

---

## GET /personagens

A lista MUST continuar autossuficiente (sem segundo GET por personagem para montar o corpo).

Campos 009/010/011 preservados. `midias.retrato`, `arma`, `armadura` e `habilidades[].midia` inalterados.

### Response 200 — conjunto idle e walk completos (formato)

```json
{
  "id": "...",
  "aparencia": 1,
  "midias": {
    "corpoInteiro": {
      "tipo": "corpoInteiro",
      "rotulo": "Corpo inteiro",
      "status": "OK",
      "arquivoInventarioId": "arquivos/cruzado/crusader_B/anim/crusader.sprite.idle.png",
      "hashArquivo": "...",
      "url": "/acervo/herois/arquivos/cruzado/crusader_B/anim/crusader.sprite.idle.png",
      "conjuntoIdle": {
        "ciclo": "idle",
        "urlAtlas": "/acervo/herois/arquivos/cruzado/anim/crusader.sprite.idle.atlas",
        "urlEsqueleto": "/acervo/herois/arquivos/cruzado/anim/crusader.sprite.idle.skel",
        "urlTextura": "/acervo/herois/arquivos/cruzado/crusader_B/anim/crusader.sprite.idle.png"
      },
      "conjuntoWalk": {
        "ciclo": "walk",
        "urlAtlas": "/acervo/herois/arquivos/cruzado/anim/crusader.sprite.walk.atlas",
        "urlEsqueleto": "/acervo/herois/arquivos/cruzado/anim/crusader.sprite.walk.skel",
        "urlTextura": "/acervo/herois/arquivos/cruzado/crusader_B/anim/crusader.sprite.walk.png"
      },
      "versoes": [
        { "id": "emEspera", "rotulo": "Em espera", "disponivel": true },
        { "id": "animado", "rotulo": "Animado", "disponivel": true },
        { "id": "caminhada", "rotulo": "Caminhada", "disponivel": true }
      ]
    }
  }
}
```

Os hashes/URLs ilustram **formato**. Aparência 1 = B. Atlas/skel **sem** pasta de paleta; PNGs **com** `_B`.

`url` / `arquivoInventarioId` MUST continuar sendo o PNG idle da paleta (regressão 010: contém `sprite.idle` e `_B`).

### Response 200 — idle incompleto

`status`: `Pendente`; `url`, `arquivoInventarioId`, `hashArquivo`, `conjuntoIdle`: nulos.  
`versoes` com `emEspera` e `animado` `disponivel: false`.  
`conjuntoWalk` pode existir; `caminhada.disponivel` só é true se o walk estiver completo. Sem idle, o espaço visual MUST ser **sem imagem** (walk sozinho não substitui a pose de espera).  
Lista HTTP 200. **Não** 404.

### Response 200 — idle completo, walk ausente

`status`: `OK`; `conjuntoIdle` preenchido; `conjuntoWalk`: nulo; `caminhada.disponivel`: false.

Dois personagens da mesma classe e aparências distintas MUST ter `urlTextura` diferentes (`_A` vs `_B`); `urlAtlas` / `urlEsqueleto` PODEM ser iguais (esqueleto compartilhado).

### Semântica de URL

- Prefixo `/acervo/herois/`.
- Sem `file://`, sem caminho de disco.
- Cliente do player: atlas (texto), skel (binário), PNG (imagem).

---

## GET /personagens/{id}

O mesmo `midias.corpoInteiro` enriquecido da lista. Demais campos do detalhe inalterados.

---

## Arquivos estáticos (extensão 010)

| URL | Origem | Content-Type esperado |
|---|---|---|
| `/acervo/herois/{*caminho}.png` | `assets/herois/` | `image/png` |
| `/acervo/herois/{*caminho}.atlas` | `assets/herois/` | `text/plain` (charset utf-8 permitido) |
| `/acervo/herois/{*caminho}.skel` | `assets/herois/` | `application/octet-stream` |

Arquivo existente: `200`. Inexistente: `404`. Caminho fora da raiz: recusado.

Exemplos Cruzado A:

```http
GET /acervo/herois/arquivos/cruzado/anim/crusader.sprite.idle.atlas
GET /acervo/herois/arquivos/cruzado/anim/crusader.sprite.idle.skel
GET /acervo/herois/arquivos/cruzado/crusader_A/anim/crusader.sprite.idle.png
GET /acervo/herois/arquivos/cruzado/anim/crusader.sprite.walk.atlas
GET /acervo/herois/arquivos/cruzado/crusader_A/anim/crusader.sprite.walk.png
```

Não há `POST` de mídia. Não há endpoint de “versão selecionada”.

---

## UI do card (contrato de apresentação)

Consumidor: [wwwroot/personagens](../../../src/DarkestDungeon.Api/wwwroot/personagens/).

O espaço “Corpo inteiro” MUST:

1. Mostrar o herói **composto** (não a folha de partes) quando `status === "OK"`.
2. Oferecer seletor com rótulos **Em espera**, **Animado**, **Caminhada**; opções com `disponivel !== true` não acionáveis.
3. Iniciar em **Em espera** a cada carga da listagem.
4. Trocar a versão só naquele card (`personagem.id`).
5. Em **Caminhada**, manter o herói inteiro dentro do espaço (andar no lugar); overflow recortado; não cobrir retrato/texto/ações.
6. Reservar o espaço e escrever **sem imagem** quando `status !== "OK"`, conjunto idle falhar ao reproduzir, ou o player não carregar.
7. Preservar retrato, arma, armadura, habilidades 011 e ações criar/excluir.

---

## Testes de contrato exigidos

1. `GET /personagens` Cruzado aparências A e B: `conjuntoIdle.urlTextura` contém `_A` vs `_B`; `urlAtlas` contém `arquivos/cruzado/anim/` **sem** `_A`/`_B`; HTTP 200.
2. Mesmo card: `url` do slot ainda contém `sprite.idle` e a pasta da paleta (regressão 010).
3. Idle completo + walk completo: três versões `disponivel: true`.
4. Walk ausente (filtro de inventário de teste ou classe sem walk): `caminhada.disponivel === false`; `status` OK se idle completo.
5. Idle incompleto: `status` Pendente, `url` null, lista 200; retrato intacto.
6. `GET /acervo/herois/arquivos/cruzado/anim/crusader.sprite.idle.atlas` → 200 texto; `.skel` → 200 octet-stream; PNG paleta → 200 image/png; caminho inventado → 404.
7. Payload da lista continua incluindo HP, resistências, acampamento 011 (regressão 009–011).

Erros de criação/exclusão: inalterados.
