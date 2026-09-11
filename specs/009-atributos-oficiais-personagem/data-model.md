# Data Model: Atributos Oficiais do Personagem

## Visão geral

O catálogo local passa a ter números oficiais de equipamento e perfil de classe (deslocamento + extras da infobox). A criação fiel deriva atributos e resistências efetivas a partir desses dados e das seis escolhas do usuário. O card lê o personagem persistido e os extras **somente** da classe.

Nenhum número da wiki é inventado. Sem cobertura 20×5 local, a criação fiel não é entregue.

## Snapshot Wiki (artefato de curadoria)

Arquivo por classe em `specs/009-atributos-oficiais-personagem/wiki-snapshots/{slug}.json`. Não é tabela SQL; é a fonte do seed.

| Campo | Tipo | Regra |
|---|---|---|
| `classeDeHeroiEnum` | texto | Um valor de `ClasseDeHeroi`. |
| `fonteUrl` | URL | Página oficial da classe. |
| `forma` | texto | Sempre `humana`. Abominação besta **não** entra. |
| `passosAFrente` | inteiro ≥ 0 | Obrigatório. |
| `passosAtras` | inteiro ≥ 0 | Obrigatório. |
| `religiosa` | booleano | Infobox Religious. |
| `provisaoInicial` | texto | Infobox Provisions (pode ser vazio/`None`). |
| `bonusAoCritico` | texto | Infobox Crit Buff Bonus **como efeito** (ex.: `+15% PROT`). Não converter para CRIT 0..100. |
| `arma.niveis[1..5]` | objeto | `danoMinimo`, `danoMaximo`, `critico`, `velocidade`. |
| `armadura.niveis[1..5]` | objeto | `hpMaximo`, `esquiva`. |

Validação do coletor: 20 arquivos, 5 níveis cada, nenhum campo nulo. Falha = parar (FR-004a).

## Classe (perfil)

Entidade existente `Classe`. Novos campos de catálogo (não copiados ao personagem, salvo deslocamento na criação):

| Campo | Tipo | Persistido em | Copiado ao Personagem? |
|---|---|---|---|
| `ResistenciasBase` | 8 percentuais | já existe | sim, com bônus de resolução nas 6 escaláveis |
| `PassosAFrente` | inteiro ≥ 0 | novo | sim |
| `PassosAtras` | inteiro ≥ 0 | novo | sim |
| `Religiosa` | booleano | novo | **não** |
| `ProvisaoInicial` | texto | novo | **não** |
| `BonusAoCriticoDaClasse` | texto (efeito oficial) | novo | **não** |

Relação: 1 classe → 1 arma elegível e 1 armadura elegível (já implícito em `ClasseElegivel`).

## Tabela oficial de arma

Reusa `Arma` + `NivelDeArma` (5 níveis). Seed preenche valores oficiais.

| Campo no nível | Origem wiki | Uso na criação |
|---|---|---|
| `DanoMinimo` / `DanoMaximo` | Weapons DMG | `Personagem.DanoBaseMinimo/Maximo` |
| `Critico` | Weapons CRIT | `Personagem.Critico` |
| `Velocidade` | Weapons SPD | `Personagem.Velocidade` |

IDs determinísticos por `ClasseDeHeroi`. Idempotente: reexecução não duplica.

## Tabela oficial de armadura

Reusa `Armadura` + `NivelDeArmadura`.

| Campo no nível | Origem wiki | Uso na criação |
|---|---|---|
| `HpAdicional` | Armor MAX HP (valor absoluto) | `Personagem.HpMaximo` e `HpAtual` |
| `Esquiva` | Armor DODGE | `Personagem.Esquiva` |

Nome `HpAdicional` permanece no código por compatibilidade; semanticamente é HP máximo oficial daquele nível.

## Personagem

Campos de combate continuam em `Ser`/`Personagem`. Alterações desta feature:

| Campo | Origem na criação fiel |
|---|---|
| `HpMaximo`, `HpAtual` | armadura nível escolhido |
| `Esquiva` | armadura |
| `Velocidade`, `Critico`, `DanoBaseMinimo`, `DanoBaseMaximo` | arma |
| `Precisao` | 0 |
| `Protecao` | 0 |
| `Stress` | 0 |
| `ChanceDeVirtude` | 25 |
| `Tamanho` | 1 |
| `AcoesPorTurno` | 1 |
| `BonusDeCritico` (herói) | 0 (não é o Crit Buff da classe) |
| `PassosAFrente`, `PassosAtras` | **novos**, copiados da classe |
| `Resistencias` (5 de `Ser`) | base + 10 p.p. × `Nivel` (resolução), teto 100 |
| `ResistenciasExtras.Doenca` | base + 10 p.p. × `Nivel`, teto 100 |
| `ResistenciasExtras.GolpeMortal`, `Armadilha` | base da classe, sem bônus |
| `Nivel` | escolha 0..6 |
| `NivelDaArma`, `NivelDaArmadura` | escolhas 1..5 |
| `Aparencia` | escolha A..D |
| habilidades | 4+4 nível 1, demais 0 |

`Ser.Movimento` permanece no modelo legado; criação fiel **não** o usa como deslocamento de herói. Legado pode continuar com `2`.

Não persistir `Religiosa`, `ProvisaoInicial` nem `BonusAoCriticoDaClasse` no personagem.

## Nível de resolução

`Personagem.Nivel` (0..6) é o nível de resolução da criação. Fórmula:

```text
escalavel(base) = min(100, base + 10 * Nivel)
```

Escaláveis: atordoamento, sangramento, envenenamento, debuff, movimento, doença.  
Não escaláveis: golpe mortal, armadilha.

## Card (visão, não entidade persistida)

Composição na consulta de lista/detalhe:

1. Dados do personagem (HP, stress, atributos persistidos, resistências efetivas, habilidades com nível, níveis, aparência).
2. Join `Classe` do enum do herói → `Religiosa`, `ProvisaoInicial`, `BonusAoCriticoDaClasse`.

Categorias de apresentação (UI):

1. HP (atual / máximo)
2. Stress
3. Atributos (precisão, proteção, esquiva, velocidade, crítico, dano mín–máx, passos à frente, passos atrás)
4. Resistências (8)
5. Habilidades de combate (todas, com nível)
6. Habilidades de acampamento (todas, com nível)

Cabeçalho: nome, classe, nível do herói, nível da arma, nível da armadura, aparência.  
Bloco de classe: religiosa, provisão inicial, bônus ao crítico.

## Validação

- Criação fiel exige exatamente 1 arma e 1 armadura da classe, 5 níveis oficiais preenchidos, passos frente/atrás catalogados, ≥4 habilidades de combate e ≥4 de acampamento.
- Catálogo incompleto: erro, sem persistência parcial (FR-019).
- Coleta incompleta: o seed/coleta falha; criação fiel não é considerada disponível (FR-003).
- Abominação: snapshots e seed usam só forma humana.

## Estados e compatibilidade

| Estado | Comportamento |
|---|---|
| Catálogo local incompleto | Coleta/implementação parada; criação fiel recusa. |
| Criação fiel válida | Atributos oficiais persistidos atomicamente. |
| Personagem legado | Leitura com defaults; card mostra persistido; sem recálculo. |
| Reexecução do seed | Idempotente; não duplica arma/armadura. |

## Relacionamentos

```text
Classe 1 ──< ClasseHabilidade >── Habilidade
Classe 1 ── 1 Arma (ClasseElegivel)
Classe 1 ── 1 Armadura (ClasseElegivel)
Personagem N ── 1 ClasseDeHeroi (enum)
Personagem N ── 1 Arma (id) + NivelDaArma
Personagem N ── 1 Armadura (id) + NivelDaArmadura
Card ── Personagem + join Classe (extras)
```
