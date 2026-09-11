# Modelo de Dados: Imagens no Card do Personagem

**Feature**: 010-imagens-card-personagem  
**Date**: 2026-09-11

Esta feature **não** cria tabelas novas. Compõe um bloco de leitura a partir de vínculos e inventários já existentes. Persistência oficial continua SQL Server (personagem, classe, item, habilidade); arquivos continuam fora do banco.

---

## Entidades de leitura (card)

### MidiaDoCard (bloco no resumo/detalhe)

Agregado projetado por personagem na Application. Não é entidade de domínio persistida.

| Campo | Tipo | Regra |
|---|---|---|
| Retrato | `SlotDeMidiaDoCard` | Classe × `Aparencia` → `portrait_roster` |
| CorpoInteiro | `SlotDeMidiaDoCard` | Classe × `Aparencia` → `sprite.idle.png` da paleta |
| Arma | `SlotDeMidiaDoCard` | Classe × `NivelDaArma` (1–5) → ícone `eqp_weapon_{n-1}` / vínculo 006 |
| Armadura | `SlotDeMidiaDoCard` | Classe × `NivelDaArmadura` (1–5) → ícone `eqp_armour_{n-1}` / vínculo 006 |
| Habilidades | lista alinhada às habilidades já listadas | Um slot por habilidade do card; ícone único |

Relacionamentos: 1 personagem → 1 bloco. Retrato e corpo são irmãos (ambos obrigatórios no layout; cada um pode estar Pendente). Arma e armadura independentes. Cada habilidade listada tem exatamente um slot; personagem sem habilidades → lista vazia, sem slots órfãos.

### SlotDeMidiaDoCard

| Campo | Tipo | Regra |
|---|---|---|
| Tipo | enum textual | `retrato` \| `corpoInteiro` \| `arma` \| `armadura` \| `habilidade` |
| Rotulo | string PT-BR | Distingue o espaço no card (`Retrato`, `Corpo inteiro`, `Arma`, `Armadura`, ou o nome da habilidade) |
| Status | `OK` \| `Pendente` | `OK` só com arquivo identificável e URL montável |
| ArquivoInventarioId | string? máx. 200 | Chave estável (`CaminhoDestino` ou id 006) |
| HashArquivo | string? 64 hex | SHA-256 quando conhecido |
| Url | string? | Relativa, prefixo `/acervo/herois/` ou `/acervo/equipamentos-itens/` + caminho destino. Nunca path absoluto de disco |
| Nivel | int? | Só arma/armadura: o nível **do personagem** (1–5). Nulo se nível de equipamento não definido |
| HabilidadeId | Guid? | Só slot de habilidade |

Invariantes:

- `Status == OK` ⇒ `Url` e `ArquivoInventarioId` não vazios.
- `Status == Pendente` ⇒ `Url` nula; o espaço no card permanece com texto **sem imagem**.
- Retrato/corpo MUST NÃO apontar para outra `Aparencia` nem para a outra pose.
- Arma/armadura MUST NÃO apontar para outro nível nem para outra classe.
- Ícone de habilidade MUST NÃO variar com `NumeroDoNivel`.

---

## Entidades persistidas reutilizadas (sem alteração de schema)

### Personagem

Campos já usados pelo card: `Classe`, `Aparencia`, `NivelDaArma`, `NivelDaArmadura`, coleção `Habilidades`. Não ganha colunas de URL.

Validação de leitura:

- `Aparencia` ∈ {A, B, C, D}.
- Sem `NivelDaArma` ou `NivelDaArmadura` → slot de equipamento Pendente (não inventar nível 1).
- Habilidades listadas (qualquer `NumeroDoNivel`, inclusive 0) recebem slot; habilidades não listadas não entram.

### AssetsDeClasse (Classe)

Já existe: 4 entradas A–D, `ConjuntoSpineId` = caminho do `portrait_roster`, `HashArquivo`, `Status` Coletado/Pendente.

Leitura do retrato: `classe.Assets` da `Aparencia` do personagem. Coletado + caminho → slot OK. Pendente → “sem imagem”.

Corpo inteiro **não** está neste owned type; resolve-se no inventário 004 pela pasta da mesma paleta.

### MidiaDeItem (nível de Arma / Armadura)

Já existe: `ArquivoInventarioId`, `ConjuntoSpineId`, `HashArquivo`, `Status` OK/Pendente.

Leitura do card: nível **igual** a `Personagem.NivelDaArma` / `NivelDaArmadura`. Se o vínculo for OK, usa esse arquivo. Se Pendente ou item ausente, tenta convenção no inventário; se ainda falhar, Pendente.

### Habilidade (catálogo)

Sem coluna de mídia. Associação de combate: `NomeOriginal` + classe → `manifesto-habilidades.json` → PNG no inventário 004. Acampamento: na ausência de PNG importado, slot Pendente.

---

## Convenções de arquivo (somente leitura)

| Slot | Padrão no inventário 004 (exemplos Cruzado / paleta B / nível 2) |
|---|---|
| Retrato | `arquivos/cruzado/crusader_B/crusader_portrait_roster.png` |
| Corpo inteiro | `arquivos/cruzado/crusader_B/anim/crusader.sprite.idle.png` |
| Arma nível N | `arquivos/cruzado/icons_equip/eqp_weapon_{N-1}.png` |
| Armadura nível N | `arquivos/cruzado/icons_equip/eqp_armour_{N-1}.png` |
| Habilidade combate | `{prefixo}.ability.{one\|two\|…}.png` via manifesto |

Prefixo inglês da classe: o mesmo mapa de [AssetsDeClasseSeed](../../src/DarkestDungeon.Infrastructure/Data/Seeds/AssetsDeClasseSeed.cs) (`crusader`, `abomination`, …).

URL pública: `/acervo/herois/{CaminhoDestino}`. Se o hash 006 apontar para cópia sob `assets/equipamentos-itens/`, usar `/acervo/equipamentos-itens/{CaminhoDestino}`.

---

## Estados do slot

```text
Definido no personagem? ──não──► Pendente ("sem imagem")
        │sim
        ▼
Arquivo no vínculo/inventário? ──não──► Pendente
        │sim
        ▼
Arquivo servível (existe no disco mapeado)? ──não──► Pendente (UI: onerror também)
        │sim
        ▼
OK (img src = Url)
```

Não há transição persistida. Falha de uma imagem não altera os outros slots.

---

## Validação

- Não aceitar URL `file://` nem caminho `C:\`.
- Não cruzar paleta (B nunca serve retrato de A).
- Não cruzar índice de equipamento (`eqp_weapon_0` não serve nível 2).
- Não omitir slot esperado no layout.
- Texto de ausência exatamente `sem imagem` (minúsculas, PT-BR).
- Sem recálculo de personagens antigos: o resolvedor usa os campos atuais.

---

## Fora deste modelo

- Migração EF.
- `MidiaDeHabilidade` persistida.
- Acessórios, consumíveis, Spine animado, formulário de criação.
- Importação / republicação de acervo.
