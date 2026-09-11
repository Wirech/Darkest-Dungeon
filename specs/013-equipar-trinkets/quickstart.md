# Quickstart: Equipar Trinkets no Personagem

## Pré-requisitos

- SDK .NET 10.
- SQL Server conforme o README (`ConnectionStrings__DarkestDungeonDb` se necessário).
- Feature 009 aplicada (criação fiel e card).
- Esta feature **não** exige snapshots 013 completos para o card funcionar: a UI usa o subconjunto já no catálogo.

## Coleta (curadoria, não runtime)

```powershell
dotnet run --project tools/DarkestDungeon.WikiCatalogCollector -- --trinkets --saida specs/013-equipar-trinkets/wiki-snapshots
```

Esperado: saída `0` mesmo com lacunas parciais; JSON por trinket; relatório PT-BR das lacunas. 429/rede → saída ≠ 0. Nenhum valor inventado.

Amostra SC-001: 20 trinkets (comuns e raros, com e sem classe, **pelo menos um DLC** se a wiki listar) com nome rastreável, raridade válida e efeitos quando a wiki os publica.

Reexecução sobre os mesmos snapshots: 0 duplicatas de `NomeOriginal` (SC-002). Upsert **não** troca `Id` de acessório 003/006.

Testes do parser **sem rede**: wikitext de fixture no projeto de testes.

## Subir a API

```powershell
dotnet restore
dotnet run --project src/DarkestDungeon.Api --launch-profile http
```

Abrir `/personagens/index.html`.

## Cenário válido (card)

1. Criar herói fiel (seis campos da 008/009). Conferir espaços vazios; `fichaEfetiva` = `fichaBase`.
2. `GET /acessorios?classe={classeDoHeroi}`: só itens permitidos; exclusivos de outra classe ausentes.
3. `PUT /personagens/{id}/acessorios/1` com um id permitido.
4. Esperar `200`. Topo do card e `fichaEfetiva` incluem o bônus; `fichaBase` igual à criação.
5. `GET /acessorios?classe={classe}&excluirId={idDoEspaco1}`: o item do espaço 1 **não** reaparece.
6. Equipar outro no espaço 2; efetiva = soma dos dois sobre a **base** (sem juros).
7. Esvaziar espaço 1 (`acessorioId: null`): só os efeitos daquele trinket saem; base intacta.
8. Recarregar o card: mesmos espaços e fichas (SC-004).

Casos amostrais da ficha (SC-005):

- HP +10% sobre a base (percentual da base).
- PROT +10% com base 0 → 10 (pontos percentuais).
- 23 HP + 10% → efetiva **26** (teto).

## Cenários inválidos

- PUT com trinket exclusivo de outra classe (mesmo fora da lista): `400` PT-BR; nada muda.
- Mesmo id nos dois espaços: `400` PT-BR.
- Personagem inexistente: `404`.
- Trinket inexistente: `400`.
- Falha simulada no meio da troca: 0 estado híbrido (SC-008).

`POST /personagens/{id}/equipar` **sem** `acessoriosIds` não apaga trinkets já gravados.

Criação **não** ganha seletor de trinket.

## Validação automatizada

```powershell
dotnet test tests/DarkestDungeon.Domain.Tests/DarkestDungeon.Domain.Tests.csproj --no-restore
dotnet test tests/DarkestDungeon.Architecture.Tests/DarkestDungeon.Architecture.Tests.csproj --no-restore
dotnet test tests/DarkestDungeon.Api.Tests/DarkestDungeon.Api.Tests.csproj --no-restore
dotnet build DarkestDungeon.sln --no-restore --configuration Release
```

Cobrir pelo menos: upsert idempotente e preservação de Id; amostra 20 sem valor inventado (fixture); GET lista filtrada; PUT por espaço; duplicata/classe/inexistente; ficha efetiva vs base (HP %, PROT p.p., teto 23+10%); desequipar devolve a base; `DerivacaoDeAtributosOficiais` sem acessórios; arquitetura sem `IPublicadorAtomicoService` neste fluxo.

## Compatibilidade

Personagem criado antes desta feature: dois espaços vazios (ou IDs antigos se existirem); leitura não falha; órfão = espaço vazio. HP/resistências persistidos **não** são reescritos no GET.
---
