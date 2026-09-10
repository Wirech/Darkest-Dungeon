# Data Model — Mídias de Armas, Armaduras e Itens

**Feature**: 006-midias-armas-armaduras-itens  
**Date**: 2026-09-10  
**Prerequisites**: [research.md](./research.md)

Modelo focado nas mudanças desta feature sobre o catálogo 003, o inventário 004 e o padrão de vínculo 005. Entidades pré-existentes só aparecem onde mudam.

---

## Hierarquia de `Item` (TPH existente + 2 tipos)

```text
Item (abstrato, tabela Itens, Discriminador)
├── Arma                 (003, inalterado em Id/níveis numéricos)
├── Armadura             (003, inalterado em Id/níveis numéricos)
├── Acessorio            (003 + criação de novos a partir de trinkets)
├── ItemDeAcampamento    (NOVO)
└── Consumivel           (NOVO)
```

Discriminadores EF: `Arma`, `Armadura`, `Acessorio`, `ItemDeAcampamento`, `Consumivel`.

---

## 1. MidiaDeItem (owned / value object reutilizável)

Padrão copiado de `AssetsDeClasse` (Feature 005). Não armazena bytes nem caminho livre.

**Localização**: `src/DarkestDungeon.Domain/Itens/MidiaDeItem.cs`

| Campo | Tipo | Constraint | Descrição |
|---|---|---|---|
| `ArquivoInventarioId` | `string?` | máx. 200 | Chave do arquivo no inventário (004 e/ou 006) |
| `ConjuntoSpineId` | `string?` | máx. 200 | Conjunto animado opcional |
| `HashArquivo` | `string?` | SHA-256 hex, 64 chars quando presente | Hash do ícone estático |
| `Status` | `StatusDeMidia` | `OK` ou `Pendente` | Ícone resolvido vs ausente |

### Invariantes

- `Status == OK` ⇒ `HashArquivo` tem 64 caracteres e `ArquivoInventarioId` não é vazio.
- `Status == Pendente` ⇒ hashes/ids podem ser nulos.
- `ConjuntoSpineId` sozinho **não** eleva o status para `OK`.
- Construtor valida comprimentos (mesmo estilo de `AssetsDeClasse`).

### Transições

| De | Para | Trigger |
|---|---|---|
| (ausente) | `Pendente` | Publicação da categoria sem arquivo correspondente |
| `Pendente` | `OK` | Publicação encontra PNG no inventário |
| `OK` | `Pendente` | Hash do inventário desapareceu (substituição/remoção) na próxima publicação da categoria |

---

## 2. StatusDeMidia e CategoriaDeMidiaDeItem

**Localização**: `src/DarkestDungeon.Domain/Itens/`

```text
StatusDeMidia: OK | Pendente
StatusDeCoberturaDeItem: OK | Parcial | Pendente   // relatório (FR-012/013); API em PT-BR
CategoriaDeMidiaDeItem: Arma | Armadura | Acessorio | ItemDeAcampamento | Consumivel
```

Não reutilizar `EstadoDeAtributo` (`Coletado`/`Pendente`/`NaoAplicavel`) nem `StatusDeAuditoria` da wiki: vocabulário de produto desta feature é o trio de cobertura.

---

## 3. NivelDeArma / NivelDeArmadura (owned existentes) — adição de mídia

**Localização**: `src/DarkestDungeon.Domain/Itens/NivelDeArma.cs`, `NivelDeArmadura.cs`

| Campo novo | Tipo | Descrição |
|---|---|---|
| `Midia` | `MidiaDeItem` (owned) | Vínculo visual do nível; default `Pendente` |

### Invariantes preservados (003)

- Continua exatamente 5 níveis, `Nivel` 1–5 únicos.
- `DanoMinimo`/`DanoMaximo`/`Critico`/`Velocidade` (arma) e `HpAdicional`/`Esquiva` (armadura) **não** mudam nesta feature.
- Publicação de mídia **não** recria o `Arma`/`Armadura` nem troca o `Id`.

### Persistência

- Colunas em `NiveisArma` / `NiveisArmadura`: `Midia_ArquivoInventarioId`, `Midia_ConjuntoSpineId`, `Midia_HashArquivo`, `Midia_Status`.
- Migration incremental (não consolidar o schema 003/005).

### Cobertura da arma/armadura (FR-013)

| Situação | Status do item no relatório |
|---|---|
| 5 níveis com `Midia.Status == OK` | `OK` |
| 1–4 níveis `OK` e o restante `Pendente` | `Parcial` |
| 0 níveis `OK` | `Pendente` |

---

## 4. Acessorio — mídia + criação de novos

**Localização**: `src/DarkestDungeon.Domain/Itens/Acessorio.cs`

| Campo novo | Tipo |
|---|---|
| `Midia` | `MidiaDeItem` |

Registros 003: Id, `Raridade`, `Efeitos`, `ClasseExclusiva`, `ConjuntoId` imutáveis nesta feature.

Registros novos (trinket sem match de `NomeOriginal`):

| Campo | Valor na criação |
|---|---|
| `Id` | `Guid.NewGuid()` (não determinístico de seed 003) |
| `NomeExibicao` / `NomeOriginal` | derivados do stem do arquivo (máx. 80) |
| `Descricao` | texto curto a partir do nome (máx. 400); pode repetir o original |
| `Raridade` | `Comum` |
| `Efeitos` | lista vazia |
| `ClasseExclusiva` | `null` |
| `ConjuntoId` | `null` |
| `Midia` | vínculo do PNG encontrado |

Match: `NomeOriginal` vs nome de arquivo sem extensão, ordinal ignore case, após trim. Colisão de dois arquivos no mesmo nome: um `Acessorio`; segunda associação reutiliza hash (FR-003).

---

## 5. ItemDeAcampamento (novo)

**Localização**: `src/DarkestDungeon.Domain/Itens/ItemDeAcampamento.cs`

Especialização de `Item` sem campos numéricos extra.

| Campo | Tipo | Origem |
|---|---|---|
| (base) `NomeExibicao`, `NomeOriginal`, `Descricao`, `Id` | iguais a `Item` | stem da origem `inventory/provision/**` |
| `Midia` | `MidiaDeItem` | inventário |

Não tem classe elegível. Não é `Acessorio`. Classificação **somente** pela pasta mapeada (research).

---

## 6. Consumivel (novo)

**Localização**: `src/DarkestDungeon.Domain/Itens/Consumivel.cs`

Idêntico em forma a `ItemDeAcampamento`, discriminador distinto, origens `inventory/quest/**` e `inventory/raid/items/**`.

---

## 7. Inventário de arquivo (artefato CLI, não tabela de negócio)

Estende o shape 004 em `assets/equipamentos-itens/inventario.json`.

### ArquivoImportado (campos extra)

| Campo | Regra |
|---|---|
| `Categoria` | Uma de: `Arma`, `Armadura`, `Acessorio`, `ItemDeAcampamento`, `Consumivel`, `NaoAssociado` |
| `Sha256`, `CaminhoOrigem`, `CaminhoDestino`, `TamanhoBytes`, `Reutilizado` | iguais à 004 |
| `Classe` | preenchido para arma/armadura quando a pasta de herói for identificável; senão vazio |

### LacunaDeImportacao

Igual à 004 + `Categoria` da origem consultada.

Arquivos `NaoAssociado` permanecem no inventário e entram no relatório como órfãos (não apagar).

---

## 8. RelatorioDeCoberturaDeMidias (DTO de aplicação / projeção)

**Localização**: `src/DarkestDungeon.Application/Midias/`

| Campo | Tipo |
|---|---|
| `GeradoEm` | `DateTimeOffset` UTC |
| `Categorias` | lista de `ResumoDeCategoriaDeMidia` (exatamente as 5 oficiais) |
| `Orfaos` | arquivos inventariados `NaoAssociado` |
| `Lacunas` | pastas/arquivos ausentes |

### ResumoDeCategoriaDeMidia

| Campo | Tipo |
|---|---|
| `Categoria` | enum / string PT-BR na API |
| `Esperados` | int — conta **itens** da categoria, **não** os 5× vínculos. Arma = 20; Armadura = 20; acessórios = count do catálogo (003 + novos desta feature); acampamento/consumível = count de itens criados a partir da instalação. Os cinco níveis entram só em `niveisOk` / `niveisPendentes` de cada linha. |
| `Ok`, `Parcial`, `Pendente` | int; soma = `Esperados` para a categoria |
| `Linhas` | cada item com status e, para arma/armadura, `niveisOk` e `niveisPendentes` (1–5) |

SC-005: as cinco categorias sempre presentes; nenhuma categoria órfã “não classificada” no resumo oficial (órfãos vão em `Orfaos`).

---

## 9. PublicacaoDeVinculosDeMidia (rastreio, não o log 005)

**Localização**: Domain ou Application conforme implementação simples.

| Campo | Tipo |
|---|---|
| `Id` | Guid |
| `Categoria` | `CategoriaDeMidiaDeItem` |
| `IniciadaEm` / `ConcluidaEm` | UTC |
| `Estado` | `EmCurso` \| `Concluida` \| `RollbackAplicado` |

Lock: no máximo uma `EmCurso` **por categoria**. Não há campo de janela de manutenção. Não há detector de sessões.

Efeito colateral **proibido**: UPDATE em `Personagens` (equipamento).

---

## 10. Personagem (sem mudança de equipamento)

Nenhum campo novo em `Personagem` (sem colunas extras). A consulta de detalhe **projeta** mídia no mapper.

Mapeamento único do nível visual (`PersonagemMapper`): `nivelMidia = Clamp(map NivelDeResolucao → 1..5)`:

| `NivelDeResolucao` (Feature 005) | Nível de arma/armadura (1–5) |
|---|---|
| 0 ou 1 | 1 |
| 2 | 2 |
| 3 | 3 |
| 4 | 4 |
| 5 ou 6 | 5 |

- Arma equipada → `Midia` do `NivelDeArma` cujo `Nivel` iguala `nivelMidia`.
- Armadura: mesma regra sobre `NivelDeArmadura`.
- Acessórios equipados: `Midia` de cada `Acessorio`.

Se o vínculo estiver `Pendente`, o DTO devolve `status: "Pendente"` e hashes nulos; HTTP 200 (não é erro de negócio). IDs de equipamento inalterados.

---

## 11. Regras de validação cruzadas

- 20 armas × 5 vínculos; 20 armaduras × 5 vínculos (SC-003, SC-004). Esta feature **não** cria arma/armadura extra.
- Publicação atômica: ou todos os owned `Midia` da categoria no `SaveChanges` da transação, ou rollback.
- `OnDelete` de `Classe` permanece `Restrict` (003). Itens novos não FK para classe.
- Architecture: Domain sem `System.IO` da instalação. Application consome `ILeitorDeInventarioDeMidias` (porta em `Application/Midias/`) e **não** lê disco. Infrastructure implementa o leitor JSON (`LeitorDeInventarioDeMidias`) compartilhado pela publicação e pela cobertura.

---

## 12. Migration

Nome sugerido: `{timestamp}_MidiasDeEquipamentoEItens`.

- Novos valores de discriminador em `Itens`.
- Colunas owned de mídia em `NiveisArma`, `NiveisArmadura`.
- Colunas owned de mídia em `Itens` para `Acessorio` / `ItemDeAcampamento` / `Consumivel` (prefixo `Midia_`).
- Tabela opcional `PublicacoesDeVinculosDeMidia` (simples) se o status 409 precisar de persistência; alternativa aceitável: lock em memória + registro de log mínimo na mesma tabela de publicação 005 **somente se** não misturar o fluxo de janela — **preferir tabela própria** para não herdar invariantes 005.

Seed: **não** reexecutar seed 003 de armas/armaduras/acessórios. Inserir apenas `ItemDeAcampamento`/`Consumivel`/`Acessorio` novos na publicação, não no `HasData` estático (dependem da instalação).
