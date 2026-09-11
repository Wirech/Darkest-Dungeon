# Data Model: Equipar Trinkets no Personagem

## Visão geral

Reusa `Acessorio` + `EfeitoDeAcessorio` e os dois `Guid?` do `Personagem`. A coleta preenche efeitos oficiais (upsert por `NomeOriginal`). O card grava **só** os espaços. A ficha **base** continua nas colunas de `Ser`/`Personagem`. A ficha **efetiva** é derivada na consulta.

Não existe tipo `Trinket`. `ConjuntoId` permanece metadado (FR-018). `IPublicadorAtomicoService` não entra neste modelo.

## Snapshot Wiki (curadoria)

Arquivo em `specs/013-equipar-trinkets/wiki-snapshots/{slug}.json`. Não é tabela SQL.

| Campo | Tipo | Regra |
|---|---|---|
| `nomeOriginal` | texto | Chave de upsert; rastreio EN da wiki |
| `nomeExibicao` | texto | PT-BR quando houver; senão original |
| `descricao` | texto | Pode ser vazio se a wiki não publicar |
| `raridade` | enum fechado | `Comum`, `Incomum`, `Rara`, `MuitoRara`, `CrimsonCourt`, `Crystalline`, `Set`. Ausente/ilegível → lacuna, não chute |
| `classeExclusiva` | enum de herói ou nulo | Só se a wiki restringir |
| `conjuntoIdWiki` | texto opcional | Só rastreio; **não** vira bônus |
| `fonteUrl` | URL | Página de origem |
| `efeitos[]` | lista | `nome`, `valor` decimal, `unidade` (`Percentual` \| `Pontos`), `sinal` (`Positivo` \| `Negativo`) |
| `lacunas[]` | lista opcional | Campo, motivo observável |

DLC listado na wiki entra no mesmo formato. Reexecução sobrescreve o JSON do mesmo slug.

## Lacuna de coleta

Registro de curadoria (arquivo de relatório e/ou lista no índice da coleta), não necessariamente entidade de domínio persistida no SQL da API.

| Campo | Regra |
|---|---|
| Trinket ou página | Identificador observável |
| Campo ausente | Ex.: raridade, efeito, valor |
| Motivo | Texto PT-BR |
| Momento | Timestamp da execução |

A coleta **continua** para os demais itens (FR-004).

## Acessório (existente, estendido)

Entidade `Acessorio` : `Item`.

| Campo | Persistido | Mutação nesta feature |
|---|---|---|
| `Id` | sim | **Nunca** na coleta |
| `NomeExibicao`, `NomeOriginal`, `Descricao` | sim | Atualizar no upsert se a wiki informar |
| `Raridade` | sim | Atualizar se a wiki informar valor do enum |
| `ClasseExclusiva` | sim | Atualizar (pode limpar se a wiki não restringir mais) |
| `ConjuntoId` | sim | **Não** recalcular; preservar |
| `Efeitos` | `OwnsMany` → `EfeitosAcessorio` | Substituir a lista no upsert |
| `Midia` | sim (006) | **Não** tocar |

Regras:

- Construtor atual permanece para criação 003/006/`POST /acessorios`.
- Novo mutator de catálogo (efeitos + metadados oficiais) no domínio, sem expor lista mutável crua.
- Índice único filtrado `NomeOriginal` + discriminador `Acessorio` (migração), se o SQL Server da solução permitir filtered index no TPH.

## Efeito de acessório (existente)

Record `EfeitoDeAcessorio`: `Nome` ≤ 80, `Valor` decimal, `Unidade`, `Sinal`.

O **nome** continua livre na persistência. O calculador usa o mapa canônico abaixo; nome fora do mapa não entra na ficha efetiva.

## Mapa canônico (efeito → ficha)

Aplicação no runtime (e validação na coleta quando o rótulo for reconhecido):

| Família de nomes (wiki / catálogo) | Campo da ficha | Unidade de aplicação |
|---|---|---|
| MAX HP, HP | HP máximo | % da base |
| DMG, Damage | dano mín **e** máx | % da base em cada um |
| PROT, Protection | proteção | pontos percentuais |
| CRIT, Critical | crítico | pontos percentuais |
| Stun / Bleed / Blight / Debuff / Move / Disease / Death Blow / Trap (+ equivalentes PT já usados) | resistência correspondente | pontos percentuais |
| Virtue Chance | chance de virtude | pontos percentuais |
| ACC, Accuracy | precisão | pontos |
| DODGE, Dodge | esquiva | pontos |
| SPD, Speed | velocidade | pontos |

Sinal negativo inverte o delta. Efeito reconhecido com unidade incompatível na coleta → lacuna. Efeito não reconhecido → só lista do item (FR-014).

## Personagem (espaços existentes)

| Campo | Tipo | Uso |
|---|---|---|
| `AcessorioEquipado1Id` | `Guid?` | Espaço 1 |
| `AcessorioEquipado2Id` | `Guid?` | Espaço 2 |

`EquiparAcessorios` já recusa o mesmo Id nos dois espaços. Esta feature:

- Grava **um** espaço por operação (o outro permanece).
- Não valida classe no domínio (continua na aplicação, com o catálogo).
- Identificador órfão (item apagado): consulta trata o espaço como vazio, sem erro fatal.
- Criação: ambos `null`.

Sem FK SQL para `Itens` (já é assim); IDs soltos permanecem.

## Ficha base vs ficha efetiva

**Ficha base** = colunas persistidas cobertas por FR-013 (criação 009 e fluxos que não sejam trinket):

- HP máximo, precisão, proteção, esquiva, velocidade, crítico, dano mín/máx, chance de virtude, oito resistências.

**HP atual** e stress **não** entram no somatório de trinket. HP atual exibido = persistido, limitado a `[1, HP máximo efetivo]`.

**Ficha efetiva** (não persistida):

```
para cada atributo mapeado:
  delta = Σ (efeito_i aplicado sobre a BASE, nunca sobre o resultado do outro)
  bruto = base + delta
  se atributo inteiro: teto matemático (Ceiling) se houver fração
  se resistência: clamp 0..100
  se HP máximo: máximo(1, valor)
```

Exemplos de aceite:

- 30 HP + 10% + 10% → 36 (não 36,3).
- 23 HP + 10% → 25,3 → **26**.
- 23 HP + 10% + 10% → 27,6 → **28**.
- 0 PROT + 10% (pontos percentuais) → 10.

Espaços vazios: efetiva = base (ainda ambas visíveis).

Payload JSON (lista e detalhe, camelCase): `espacoTrinket1`, `espacoTrinket2`, `fichaBase`, `fichaEfetiva`. Topo de combate = efetiva. `fichaEfetiva` não é coluna SQL.

## Estados do espaço

```text
vazio ──selecionar permitido──► ocupado
ocupado ──remover──► vazio
ocupado ──trocar (A→B)──► ocupado'   (uma transação: remove A e aplica B, ou nada muda)
```

Transições recusadas (estado inalterado): classe exclusiva, inexistente, duplicata, personagem inexistente, espaço ≠ {1,2}.

## Validação

| Regra | Onde |
|---|---|
| Mesmo acessório nos dois espaços | Domínio + API |
| Classe exclusiva | Aplicação |
| Trinket/personagem inexistente | Aplicação |
| Lista só permitidos + omitir o Id do outro espaço | Aplicação / GET |
| Atomicidade por personagem | Aplicação + transação EF |
| Não regravar ficha base | Calculador só na leitura |
| Teto / clamps | Calculador |

## Migração

- Mutação de `OwnsMany` Efeitos: sem tabela nova; replace da coleção.
- Índice único filtrado opcional em `NomeOriginal` de acessório.
- **Nenhuma** coluna nova de ficha efetiva.
- Personagens antigos: espaços `null`; leitura compatível.
---
