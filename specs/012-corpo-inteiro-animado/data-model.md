# Modelo de Dados: Corpo Inteiro Composto e Animado

**Feature**: 012-corpo-inteiro-animado  
**Date**: 2026-09-11

Esta feature **não** cria tabelas nem altera o cadastro do personagem. Compõe conjuntos Spine de leitura a partir do inventário 004 e enriquece o slot `corpoInteiro` já definido na 010. Persistência oficial continua SQL Server; arquivos continuam fora do banco. A versão escolhida no card **não** é entidade persistida.

---

## Entidades de leitura

### ConjuntoDeCorpo (idle | walk)

Trio Spine necessário para uma versão animada/composta. Não é persistido.

| Campo | Tipo | Regra |
|---|---|---|
| Ciclo | `idle` \| `walk` | Idle cobre “Em espera” e “Animado”; walk cobre “Caminhada” |
| Atlas | caminho relativo | `arquivos/{pastaClasse}/anim/{prefixo}.sprite.{ciclo}.atlas` (pasta da **classe**, sem paleta) |
| Esqueleto | caminho relativo | mesmo stem `.skel` |
| Textura | caminho relativo | `arquivos/{pastaClasse}/{prefixo}_{A\|B\|C\|D}/anim/{prefixo}.sprite.{ciclo}.png` (**paleta** do personagem) |
| UrlAtlas | string | `/acervo/herois/` + atlas |
| UrlEsqueleto | string | `/acervo/herois/` + esqueleto |
| UrlTextura | string | `/acervo/herois/` + textura |
| Completo | bool | Os três arquivos existem no inventário **e** são servíveis |

Relacionamento: 1 personagem × 1 aparência → 0..1 conjunto idle e 0..1 conjunto walk. Não cruzar classe, paleta nem ciclo.

### VersaoDoCorpo (opção do seletor)

| Campo | Tipo | Regra |
|---|---|---|
| Id | `emEspera` \| `animado` \| `caminhada` | Estável no contrato (camelCase) |
| Rotulo | string PT-BR | `Em espera` \| `Animado` \| `Caminhada` |
| Disponivel | bool | Idle completo para espera/animado; walk completo para caminhada |
| Conjunto | `idle` \| `walk` | Qual trio o player usa |

Padrão de apresentação: `emEspera`. Não gravar no personagem.

### SlotDeCorpoInteiro (extensão do slot 010)

O slot `midias.corpoInteiro` permanece um `SlotDeMidiaDoCard` e **acrescenta** os conjuntos. Campos 010 preservados.

| Campo | Tipo | Regra |
|---|---|---|
| Tipo / Rotulo | iguais à 010 | `corpoInteiro` / `Corpo inteiro` |
| Status | `OK` \| `Pendente` | `OK` só com conjunto **idle** completo |
| ArquivoInventarioId / HashArquivo / Url | iguais à 010 | Quando OK: PNG idle da paleta (`*.sprite.idle.png` na pasta `_A`…`_D`). Quando Pendente: nulos — **não** apontar à folha como substituto visual |
| ConjuntoIdle | `ConjuntoDeCorpo?` | Presente se os três arquivos idle existirem |
| ConjuntoWalk | `ConjuntoDeCorpo?` | Presente se os três arquivos walk existirem |
| Versoes | lista de 3 | Sempre as três opções; `disponivel` reflete o conjunto |

Invariantes:

- `Status == OK` ⇒ `ConjuntoIdle` completo e `Url` = textura idle da paleta.
- `Status == Pendente` ⇒ `Url` nula; UI mostra **sem imagem**; seletor sem opções acionáveis (ou só as que tiverem conjunto — walk sozinho **não** habilita espera).
- Retrato, arma, armadura e habilidades **não** ganham conjuntos.
- Versão selecionada **não** faz parte deste modelo (estado de UI).

---

## Entidades persistidas reutilizadas (sem schema)

### Personagem

`Classe`, `Aparencia`. Sem coluna de versão de corpo.

### Inventário 004 (`ArquivoImportado`)

Chave: `CaminhoDestino`. Leitura por predicado de classe + stem `sprite.idle` / `sprite.walk` e paleta no path (`_{A-D}/`).

Prefixo inglês e pasta PT-BR: os mesmos mapas de [ResolvedorDeMidiasDoCard](../../src/DarkestDungeon.Infrastructure/Midias/ResolvedorDeMidiasDoCard.cs).

---

## Convenções de arquivo (somente leitura)

Exemplo Cruzado, aparência B:

| Peça | CaminhoDestino |
|---|---|
| Atlas idle | `arquivos/cruzado/anim/crusader.sprite.idle.atlas` |
| Skel idle | `arquivos/cruzado/anim/crusader.sprite.idle.skel` |
| PNG idle B | `arquivos/cruzado/crusader_B/anim/crusader.sprite.idle.png` |
| Atlas walk | `arquivos/cruzado/anim/crusader.sprite.walk.atlas` |
| Skel walk | `arquivos/cruzado/anim/crusader.sprite.walk.skel` |
| PNG walk B | `arquivos/cruzado/crusader_B/anim/crusader.sprite.walk.png` |

URL pública: `/acervo/herois/{CaminhoDestino}`.

A primeira linha do atlas (nome do PNG) MUST ser resolvida para `UrlTextura` da paleta, não para um PNG na pasta da classe.

---

## Estados do slot de corpo

```text
Conjunto idle completo? ──não──► Pendente ("sem imagem"); versões espera/animado indisponíveis
        │sim
        ▼
OK — visual padrão "Em espera" (esqueleto idle, pausado/pose 0)
        │
        ├─ consultor escolhe Animado → loop idle (só na página aberta)
        ├─ walk completo e escolhe Caminhada → loop walk no lugar
        └─ recarrega listagem → volta a Em espera (nada persistido)
```

Walk incompleto: opção “Caminhada” indisponível; idle intacto.

Falha do player (skel ilegível, runtime ausente, 404 de atlas): aquele card cai em **sem imagem** ou na última versão válida **do mesmo personagem**; demais cards intactos.

---

## Validação

- Não aceitar `file://` nem caminho de disco no JSON.
- Não cruzar paleta (`_B` nunca usa PNG `_A`).
- Não expor `sprite.attack_*`, `camp`, `combat`, etc. no seletor.
- Texto de ausência exatamente `sem imagem`.
- Rótulos exatamente `Em espera`, `Animado`, `Caminhada`.
- Sem recálculo de personagens antigos.

---

## Fora deste modelo

- Migração EF.
- Forma bestial da Abominação.
- Persistência da versão (localStorage, SQL, cookie).
- Importação / republicação de acervo.
- Recorte de atlas sem esqueleto.
