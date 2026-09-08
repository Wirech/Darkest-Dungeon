# Data Model: Hierarquia de Entidades e Catálogo Oficial de Heróis

Este documento descreve as entidades novas e os relacionamentos introduzidos por esta feature.
`Ser`, `Resistencias` e `EntidadeIdentificavel` permanecem exatamente como definidos pela
feature 001 e não são reescritos aqui.

## Hierarquia geral

```text
EntidadeIdentificavel (existente)
├── Ser (existente, inalterado)
│   ├── Personagem (novo)
│   └── Inimigo (novo)
├── Habilidade (novo, abstrato)
│   ├── HabilidadeDeHeroi (novo, abstrato)
│   │   ├── HabilidadeDeCombate (novo)
│   │   └── HabilidadeDeAcampamento (novo)
│   └── HabilidadeDeInimigo (novo)
└── Item (novo, abstrato)
    ├── Arma (novo)
    ├── Armadura (novo)
    └── Acessorio (novo)
```

## Classe

Catálogo controlado das 20 classes oficiais.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| Id | Guid | Yes | Herdado de `EntidadeIdentificavel`; gerado pelo sistema. |
| ClasseDeHeroi | enum ClasseDeHeroi | Yes | Um dos 20 valores fechados; único no catálogo. |
| NomeExibicao | string | Yes | Nome em PT-BR; não vazio; máximo 60 caracteres. |
| NomeOriginal | string | Yes | Nome oficial em inglês; máximo 60 caracteres. |
| ResistenciasBase | ResistenciasDeClasse (owned) | Yes | Oito percentuais 0–100 (5 herdados de Ser + 3 novos). |

### Relationships

- Uma `Classe` referencia várias `HabilidadeDeHeroi` via `ClasseHabilidade`.
- Uma `Classe` é referenciada por vários `Personagem` (1:N via `Personagem.ClasseDeHeroi`).
- `Classe` fornece as resistências iniciais copiadas para `Personagem` na criação.
- Todas as FKs de `Classe` (a partir de `Personagem`, `ClasseHabilidade`, `Arma`, `Armadura`,
  `Acessorio` e `EntradaDoMapaDeCobertura`) MUST usar `OnDelete(DeleteBehavior.Restrict)`.
  Exclusão de `Classe` está fora do escopo desta feature.

### Validation Rules

- `ClasseDeHeroi` é único no catálogo (unique index).
- Todas as 8 resistências de `ResistenciasBase` são exigidas e ficam entre 0 e 100.

## ResistenciasDeClasse (Owned)

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| Atordoamento | decimal | Yes | 0 a 100. |
| Sangramento | decimal | Yes | 0 a 100. |
| Envenenamento | decimal | Yes | 0 a 100. |
| Debuff | decimal | Yes | 0 a 100. |
| Movimento | decimal | Yes | 0 a 100. |
| Doenca | decimal | Yes | 0 a 100. |
| GolpeMortal | decimal | Yes | 0 a 100. |
| Armadilha | decimal | Yes | 0 a 100. |

## Habilidade (abstrata)

Base comum identificável de todas as habilidades do jogo.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| Id | Guid | Yes | Herdado de `EntidadeIdentificavel`. |
| NomeExibicao | string | Yes | Nome único global em PT-BR; máximo 80 caracteres. |
| NomeOriginal | string | Yes | Nome oficial em inglês; máximo 80 caracteres. |
| Descricao | string | Yes | Máximo 400 caracteres. |
| Discriminator | string (EF Core) | Yes | Uma das constantes: `Combate`, `Acampamento`, `Inimigo`. |

## HabilidadeDeHeroi (abstrata)

Especialização de `Habilidade`; carrega os campos comuns a `HabilidadeDeCombate` e
`HabilidadeDeAcampamento`.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| Efeitos | IReadOnlyList&lt;EfeitoDeHabilidade&gt; | Yes | Owned collection; pode ser vazia. |
| LimitePorUso | LimitePorUso? (owned) | No | Presente quando a habilidade tem limite. |

## HabilidadeDeCombate

Especialização concreta de `HabilidadeDeHeroi`.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| PosicoesValidas | PosicaoDeExecucao (flags 1..4) | Yes | Pelo menos uma posição. |
| PosicoesQueAtinge | PosicoesQueAtinge (flags 1..4 + AlvoUnicoOuArea) | Yes | Pelo menos uma. |
| ModificadorDano | decimal | Yes | Percentual (pode ser negativo). |
| ModificadorAcerto | decimal | Yes | Percentual (pode ser negativo). |
| ModificadorCritico | decimal | Yes | Percentual (pode ser negativo). |

## HabilidadeDeAcampamento

Especialização concreta de `HabilidadeDeHeroi`.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| CustoDeDescanso | int | Yes | 0 a 20. |
| AlvoDeAcampamento | enum AlvoDeAcampamento | Yes | Um de `Self`, `UmAliado`, `TodosOsAliados`, `PartyInteira`. |

## HabilidadeDeInimigo

Especialização de `Habilidade`, irmã de `HabilidadeDeHeroi`.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| Efeitos | IReadOnlyList&lt;EfeitoDeHabilidade&gt; | Yes | Owned collection; pode ser vazia. |
| CondicaoDeAparecer | string | No | Máximo 200 caracteres. |
| ChanceDeExecucao | decimal? | No | 0 a 100 quando presente. |

## EfeitoDeHabilidade (Owned)

Linha estruturada usada por `HabilidadeDeHeroi` e por `HabilidadeDeInimigo`.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| NomeDoEfeito | string | Yes | Máximo 60 caracteres. |
| Alvo | enum AlvoDeEfeito | Yes | Um de `Self`, `Aliado`, `Inimigo`. |
| Valor | decimal | Yes | Pode ser negativo. |
| Unidade | enum UnidadeDeEfeito | Yes | Um de `Percentual`, `Pontos`, `Rodadas`. |
| DuracaoEmRodadas | int? | No | 0 a 20 quando presente. |
| ChanceBase | decimal | Yes | 0 a 100. |

## LimitePorUso (Owned)

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| Escopo | enum EscopoDeLimite | Yes | Um de `Batalha`, `Acampamento`. |
| MaximoUsos | int | Yes | 1 a 10. |

## ClasseHabilidade (Associação Classe × Habilidade)

Materializa a relação N:M entre `Classe` e `HabilidadeDeHeroi`.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| ClasseId | Guid | Yes | FK obrigatória. |
| HabilidadeId | Guid | Yes | FK obrigatória; MUST referenciar uma `HabilidadeDeHeroi`. |

### Validation Rules

- Chave primária composta (ClasseId, HabilidadeId) impede duplicidade da mesma habilidade na
  mesma classe.
- Uma habilidade de acampamento compartilhada aparece como várias linhas com o mesmo
  `HabilidadeId` e `ClasseId` distintos.

## Item (abstrato)

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| Id | Guid | Yes | Herdado de `EntidadeIdentificavel`. |
| NomeExibicao | string | Yes | Máximo 80 caracteres. |
| NomeOriginal | string | Yes | Máximo 80 caracteres. |
| Descricao | string | Yes | Máximo 400 caracteres. |
| Discriminator | string (EF Core) | Yes | Um de `Arma`, `Armadura`, `Acessorio`. |

## Arma

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| ClasseElegivel | enum ClasseDeHeroi | Yes | Exatamente uma. |
| Niveis | IReadOnlyList&lt;NivelDeArma&gt; | Yes | Exatamente 5 níveis (1..5), sem duplicidade. |

## NivelDeArma (Owned)

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| Nivel | int | Yes | 1 a 5. |
| DanoMinimo | int | Yes | 0 ou mais. |
| DanoMaximo | int | Yes | Maior ou igual a `DanoMinimo`. |
| Critico | decimal | Yes | 0 a 100. |
| Velocidade | int | Yes | 0 ou mais. |

## Armadura

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| ClasseElegivel | enum ClasseDeHeroi | Yes | Exatamente uma. |
| Niveis | IReadOnlyList&lt;NivelDeArmadura&gt; | Yes | Exatamente 5 níveis (1..5), sem duplicidade. |

## NivelDeArmadura (Owned)

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| Nivel | int | Yes | 1 a 5. |
| HpAdicional | int | Yes | 0 ou mais. |
| Esquiva | decimal | Yes | 0 a 100. |

## Acessorio

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| Raridade | enum RaridadeDeAcessorio | Yes | Um de `Comum`, `Incomum`, `Rara`, `MuitoRara`, `CrimsonCourt`, `Crystalline`, `Set`. |
| ClasseExclusiva | enum ClasseDeHeroi? | No | Quando presente, MUST ser respeitada por Personagem. |
| ConjuntoId | Guid? | No | Metadata que agrupa Acessórios do mesmo conjunto; o cálculo do bônus quando 2 peças do conjunto são equipadas está fora do escopo desta feature (FR-013). |
| Efeitos | IReadOnlyList&lt;EfeitoDeAcessorio&gt; | Yes | Owned collection; pode ser vazia. |

## EfeitoDeAcessorio (Owned)

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| Nome | string | Yes | Máximo 80 caracteres. |
| Valor | decimal | Yes | Pode ser positivo ou negativo. |
| Unidade | enum UnidadeDeEfeito | Yes | Reaproveita o enum de EfeitoDeHabilidade. |
| Sinal | enum SinalDeEfeito | Yes | `Positivo` ou `Negativo`. |

## Personagem

Especialização de `Ser`.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| ClasseDeHeroi | enum ClasseDeHeroi | Yes | Um dos 20 valores fechados. |
| Stress | int | Yes | 0 a 200. |
| ChanceDeVirtude | int | Yes | 0 a 100. |
| ResistenciasExtras | ResistenciasExtrasDePersonagem (owned) | Yes | Doença, Golpe Mortal, Armadilha (0..100). |
| Aflicao | string? | No | Máximo 60; mutuamente exclusiva com `Virtude`. |
| Virtude | string? | No | Máximo 60; mutuamente exclusiva com `Aflicao`. |
| EstadoPortasDaMorte | bool | Yes | Default `false`. |
| RecuperouPortasDaMorte | bool | Yes | Default `false`. |
| RecuperouAtaqueCardiaco | bool | Yes | Default `false`. |
| Habilidades | IReadOnlyList&lt;HabilidadeDePersonagem&gt; | Yes | Owned collection; no máximo 6 referências a `HabilidadeDeCombate` + no máximo 6 referências a `HabilidadeDeAcampamento` (FR-009); toda `HabilidadeId` MUST estar em `ClasseHabilidade` da Classe do Personagem (FR-009a). |
| Inventario | Inventario (owned) | Yes | Exatamente 4 slots. |
| ArmaEquipadaId | Guid? | No | FK opcional para `Arma`; ClasseElegivel MUST bater com `ClasseDeHeroi`. |
| ArmaduraEquipadaId | Guid? | No | Idem. |
| AcessoriosEquipadosIds | IReadOnlyList&lt;Guid&gt; | Yes | Até 2; se um Acessório tiver `ClasseExclusiva`, MUST bater. |

## HabilidadeDePersonagem (Owned)

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| HabilidadeId | Guid | Yes | FK para `HabilidadeDeHeroi`. |
| Habilitada | bool | Yes | Para habilidades de Combate. |
| Treinada | bool | Yes | Para habilidades de Acampamento. |
| Equipada | bool | Yes | Para habilidades de Acampamento; MUST ser Treinada para ser Equipada. |

## Inventario (Owned)

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| Slots | IReadOnlyList&lt;Guid?&gt; | Yes | Exatamente 4 entradas; cada uma referencia um `Item.Id` ou é nula. |

## Inimigo

Especialização de `Ser`.

> `Inimigo` intencionalmente **NÃO** possui `ResistenciasExtrasDePersonagem`. Suas resistências
> são exatamente as cinco de `Ser` (`Atordoamento`, `Sangramento`, `Envenenamento`, `Debuff`,
> `Movimento`); `Doença`, `Golpe Mortal` e `Armadilha` ficam de fora nesta feature.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| TipoDeInimigo | enum TipoDeInimigo | Yes | Um de `Humano`, `Besta`, `Profano`, `Sobrenatural`, `Vampirico`, `Casca`, `Rochoso`. |
| ResistenciasProprias | Resistencias (owned de Ser) | Yes | Mantém a definição de 001. |
| Habilidades | IReadOnlyList&lt;Guid&gt; | Yes | Lista de `HabilidadeDeInimigo.Id`; qualquer tamanho ≥ 0. |

## EstadoDeAtributo (enum)

Valores: `Coletado`, `Pendente`, `NaoAplicavel`.

## EntradaDoMapaDeCobertura

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| Id | Guid | Yes | Herdado de `EntidadeIdentificavel`. |
| ClasseDeHeroi | enum ClasseDeHeroi | Yes | Alvo da entrada. |
| Categoria | enum CategoriaDeCobertura | Yes | Um de `HabilidadeCombate`, `HabilidadeAcampamento`, `ResistenciaBase`. |
| ChaveDoAtributo | string | Yes | Nome da habilidade (para categorias de habilidade) ou nome da resistência (para resistência base); máximo 80. |
| Estado | enum EstadoDeAtributo | Yes | Um de `Coletado`, `Pendente`, `NaoAplicavel`. |
| Notas | string? | No | Máximo 200 caracteres. |

### Validation Rules

- Chave lógica (ClasseDeHeroi, Categoria, ChaveDoAtributo) é única — impede duplicidade.

## Enums resumidos

| Enum | Valores |
|------|---------|
| ClasseDeHeroi | Abominacao, Antiquario, Besteiro, CacadorDeRecompensas, Cruzado, LadraoDeCova, BoboDaCorte, MestreDeCaca, Leproso, Infernal, Bandido, Musqueteiro, Veterano, Ocultista, MedicoDaPeste, Vestal, Flagelante, Rompedor, Duelista, Fugitivo |
| TipoDeInimigo | Humano, Besta, Profano, Sobrenatural, Vampirico, Casca, Rochoso |
| AlvoDeEfeito | Self, Aliado, Inimigo |
| UnidadeDeEfeito | Percentual, Pontos, Rodadas |
| AlvoDeAcampamento | Self, UmAliado, TodosOsAliados, PartyInteira |
| RaridadeDeAcessorio | Comum, Incomum, Rara, MuitoRara, CrimsonCourt, Crystalline, Set |
| EscopoDeLimite | Batalha, Acampamento |
| SinalDeEfeito | Positivo, Negativo |
| CategoriaDeCobertura | HabilidadeCombate, HabilidadeAcampamento, ResistenciaBase |
| EstadoDeAtributo | Coletado, Pendente, NaoAplicavel |

## State Transitions

- `Personagem`: após criação, recebe cópia das `ResistenciasBase` da `Classe`; `Aflicao` e
  `Virtude` só podem coexistir como null; alterar de `null` para valor exige regra explícita.
  Individualidades e Doenças estão fora do escopo desta feature.
- `HabilidadeDePersonagem`: `Equipada = true` só quando `Treinada = true`; qualquer
  `HabilidadeId` MUST pertencer à associação Classe × Habilidade da Classe do Personagem
  (FR-009a) e o total por categoria MUST respeitar o limite 6 + 6 (FR-009). `HabilidadeId` é
  referência viva a `Habilidade`; esta feature não expõe endpoint `PUT/PATCH` para edição.
- Exclusão de `Classe`, `Habilidade` e `Item` está fora do escopo desta feature; o
  `DarkestDungeonDbContext` MUST configurar todas as FKs desses catálogos com
  `OnDelete(DeleteBehavior.Restrict)`.
- `EntradaDoMapaDeCobertura`: transições permitidas — de `Pendente` para `Coletado`
  (quando um dado é registrado) ou para `NaoAplicavel` (quando o dado é revisado); de
  `Coletado` para `Pendente` requer tratamento explícito (remoção do dado).
