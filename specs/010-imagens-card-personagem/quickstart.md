# Quickstart: Imagens no Card do Personagem

**Feature**: 010-imagens-card-personagem  
**Date**: 2026-09-11

Validação executável após a implementação. Sem código de serviço, sem suíte completa — isso fica em `tasks.md`.

Contratos: [contracts/personagens-card-midias.md](contracts/personagens-card-midias.md)  
Modelo: [data-model.md](data-model.md)

---

## Pré-requisitos

- SQL Server local (`Server=localhost,1433;Database=DarkestDungeon`) e acervo já importado:
  - `assets/herois/` (004) com `inventario.json`, `manifesto-habilidades.json` e PNGs
  - `assets/equipamentos-itens/` (006), se a publicação de vínculos tiver sido usada
- API no perfil `http`: `http://localhost:5140`
- Pelo menos um personagem cadastrado (criação 008/009). Personagens antigos **não** precisam ser recriados.

---

## Subir a API

```powershell
dotnet test tests/DarkestDungeon.Api.Tests/DarkestDungeon.Api.Tests.csproj --filter "FullyQualifiedName~Feature010"
dotnet run --project src/DarkestDungeon.Api --launch-profile http
```

Esperado: testes 010 verdes; health `GET /health` → `{"status":"saudável"}`.

---

## Cenário 1 — Lista autossuficiente (SC-001, SC-002, SC-003)

1. Criar dois Cruzados (ou usar existentes) com aparências diferentes e níveis de arma/armadura distintos, via `POST /personagens` (seis campos 008).
2. `GET /personagens` **uma vez**.

Esperado:

- Cada item tem `midias.retrato`, `midias.corpoInteiro`, `midias.arma`, `midias.armadura`.
- Retrato/corpo da aparência B usam pasta `*_B`, não `*_A`.
- Arma nível N usa ícone índice `N-1` (`eqp_weapon_0` para nível 1).
- Campos 009 (HP, resistências, passos, extras) ainda vêm no mesmo JSON.
- Não é necessário `GET /personagens/{id}` para montar o card.

---

## Cenário 2 — Ícone de habilidade único (SC-004)

1. Na lista, localizar a mesma habilidade de combate em Nv. 0 e em Nv. ≥1 (dois personagens ou o mesmo card com mistas).
2. Comparar `habilidades[].midia.url`.

Esperado: URLs iguais (ou ambas `null` se Pendente). O texto `numeroDoNivel` muda; o ícone não.

---

## Cenário 3 — Ausência (SC-005)

1. Personagem sem `nivelDaArma` **ou** URL de um slot 404 (arquivo apagado / classe sem idle).
2. Abrir `/personagens/index.html`.

Esperado:

- Espaço do slot visível com o texto **sem imagem**.
- Nome, classe, números e demais imagens do card continuam.
- Excluir ainda funciona.

---

## Cenário 4 — Arquivo estático

```http
GET /acervo/herois/arquivos/cruzado/crusader_A/crusader_portrait_roster.png
```

Esperado: `200` e PNG. Caminho inventado sob `/acervo/herois/` → `404` (o card trata como “sem imagem”).

---

## Cenário 5 — UI no browser

1. Abrir `http://localhost:5140/personagens/index.html`.
2. Conferir, sem instrução extra (SC-006): retrato, corpo inteiro, arma, armadura, ícones nas linhas de habilidade.
3. Viewport estreita: textos e ações (excluir) ainda legíveis.

---

## Não fazer neste quickstart

- Rodar o MediaCollector ou republicar vínculos.
- Recriar heróis só para “atualizar imagem”.
- Esperar ícone de acampamento se o PNG não estiver no acervo 004 — o correto é “sem imagem”.
