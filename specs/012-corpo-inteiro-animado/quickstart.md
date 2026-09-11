# Quickstart: Corpo Inteiro Composto e Animado

**Feature**: 012-corpo-inteiro-animado  
**Date**: 2026-09-11

Validação executável após a implementação. Sem código de serviço, sem suíte completa — isso fica em `tasks.md`.

Contratos: [contracts/personagens-corpo-inteiro.md](contracts/personagens-corpo-inteiro.md)  
Modelo: [data-model.md](data-model.md)

---

## Pré-requisitos

- SQL Server local (`Server=localhost,1433;Database=DarkestDungeon`) e acervo 004 em `assets/herois/` (atlas/skel em `arquivos/{classe}/anim/`, PNG por paleta).
- API no perfil `http`: `http://localhost:5140`
- Pelo menos um personagem cadastrado. Personagens antigos **não** precisam ser recriados.
- Runtime Spine 2.1 vendido em `wwwroot/personagens/vendor/` (após a implementação).

---

## Subir a API

```powershell
dotnet test tests/DarkestDungeon.Api.Tests/DarkestDungeon.Api.Tests.csproj --filter "FullyQualifiedName~Feature010|FullyQualifiedName~Feature011|FullyQualifiedName~Feature012"
dotnet run --project src/DarkestDungeon.Api --launch-profile http
```

Esperado: testes 010–012 verdes; `GET /health` → `{"status":"saudável"}`.

---

## Cenário 1 — Contrato do conjunto (SC-002, regressão 010)

1. Criar (ou usar) dois Cruzados com aparências distintas.
2. `GET /personagens` **uma vez**.

Esperado:

- `midias.corpoInteiro.status` = `OK` quando o idle da paleta existir.
- `url` contém `sprite.idle` e a pasta `_A` ou `_B` (não a folha como único recurso, mas o PNG da paleta).
- `conjuntoIdle.urlAtlas` / `urlEsqueleto` em `arquivos/cruzado/anim/` (sem paleta).
- `conjuntoIdle.urlTextura` na pasta da aparência; A ≠ B.
- `versoes` inclui `emEspera`, `animado`, `caminhada` com rótulos PT-BR.

---

## Cenário 2 — Arquivos estáticos Spine

```http
GET /acervo/herois/arquivos/cruzado/anim/crusader.sprite.idle.atlas
GET /acervo/herois/arquivos/cruzado/anim/crusader.sprite.idle.skel
GET /acervo/herois/arquivos/cruzado/crusader_A/anim/crusader.sprite.idle.png
GET /acervo/herois/arquivos/cruzado/anim/crusader.sprite.walk.skel
GET /acervo/herois/arquivos/inexistente/nao.skel
```

Esperado: atlas 200 texto; skel 200 `application/octet-stream`; PNG 200 `image/png`; inexistente 404.

---

## Cenário 3 — UI composta e seletor (SC-001, SC-003, SC-004)

1. Abrir `http://localhost:5140/personagens/index.html`.
2. No espaço **Corpo inteiro** de um card com conjunto OK: o herói aparece **montado** (não a grade de partes).
3. Seletor: **Em espera** (padrão), **Animado**, **Caminhada**.
4. Alternar **Animado**: movimento de espera em até 3 s, sem recarregar.
5. Alternar **Caminhada**: ciclo de walk **no lugar**, inteiro dentro do espaço; retrato e texto descobertos.
6. Em um segundo card, mudar a versão: o primeiro permanece na que tinha.

---

## Cenário 4 — Recarregar (SC-007)

1. Deixar um card em **Animado** ou **Caminhada**.
2. Recarregar a página.

Esperado: todos os cards em **Em espera**. Nada gravado no `GET /personagens/{id}` além do cadastro já existente.

---

## Cenário 5 — Ausência (SC-005)

1. Personagem cujo idle esteja incompleto **ou** forçar falha do player (skel 404).
2. Abrir a listagem.

Esperado: espaço visível com **sem imagem**; retrato e demais dados intactos; exclusão ainda funciona.

---

## Cenário 6 — Card utilizável (US3, SC-008)

Com **Caminhada** ligada: ler nome, HP e uma habilidade; clicar excluir (cancelar). Animação não cobre controles nem sai do `figure` “Corpo inteiro”.
