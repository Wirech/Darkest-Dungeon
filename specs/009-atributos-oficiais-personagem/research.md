# Research: Atributos Oficiais do Personagem

**Feature**: `009-atributos-oficiais-personagem`
**Data**: 2026-09-10

## Decisão 1: Dump local da wiki, nunca consulta em tempo de uso

**Decision**: Um coletor de curadoria baixa o wikitext oficial (`darkestdungeon.wiki.gg`, `?action=raw`) **uma vez**, grava snapshots JSON versionados em `specs/009-atributos-oficiais-personagem/wiki-snapshots/` e o seed/catálogo SQL Server é gerado só a partir desses arquivos. `POST /personagens`, `GET /personagens` e o card **MUST NOT** chamar a wiki.

**Rationale**: A spec proíbe pesquisa em tempo real e trata falha pontual da wiki como inexistente no produto. O padrão da Feature 005 (snapshots JSON + seed) já é auditável e idempotente. Wikitext raw evita HTML instável.

**Alternatives considered**:

- Scraping HTML em cada criação: rejeitado (FR-004) e frágil (429 já observado).
- MediaWiki `api.php?action=parse` em runtime: rejeitado pelo mesmo motivo; ainda devolve HTML.
- Transcrição 100% manual como na 005: rejeitada pelo pedido de aquisição automática **antes** da criação fiel; a transcrição humana só entra se o parser parar com lacuna (FR-004a).

## Decisão 2: Lacuna interrompe; nenhum valor inventado

**Decision**: O coletor valida cobertura 20×5 (arma e armadura), deslocamento frente/atrás, religiosa, provisão inicial, bônus ao crítico e forma humana da Abominação. Qualquer campo ausente, ambíguo ou não numérico **falha a coleta** com relatório PT-BR. Não interpolar, não copiar Besteiro→Musqueteiro, não completar “parecido”.

**Rationale**: FR-004a e SC-005. Musqueteiro pode coincidir com Besteiro, mas cada classe tem registro próprio.

**Alternatives considered**:

- Liberar criação das 19 classes completas: rejeitado na clarificação.
- Copiar números do reskin: rejeitado pelo edge case da spec.

## Decisão 3: Equipamento oficial vira seed local de `Arma`/`Armadura`

**Decision**: Não existe `EquipamentosSeed` hoje; testes criam arma/armadura placeholder (`HpAdicional` = número do nível). Esta feature introduz seed idempotente (IDs determinísticos por classe) com os 5 níveis oficiais. `NivelDeArmadura.HpAdicional` **armazena o HP máximo oficial da wiki** (a wiki não publica HP “adicional” separado). Sem catálogo 20×5 local, a criação fiel responde erro de catálogo incompleto.

**Rationale**: A criação 008 já deriva HP/esquiva/dano/crítico/velocidade do item elegível. Falta só preencher os números oficiais. Reusar `Arma`/`Armadura` evita tabela paralela.

**Alternatives considered**:

- Nova entidade `TabelaOficialDeClasse` desligada de `Item`: rejeitada por duplicar os cinco níveis já modelados.
- Inferir HP do nível do herói: rejeitado (assumptions da spec).

## Decisão 4: Deslocamento em dois eixos no catálogo e no personagem

**Decision**: `Classe` ganha `PassosAFrente` e `PassosAtras`. Na criação, os mesmos dois valores são **copiados** para o `Personagem`. `Ser.Movimento` permanece para `Inimigo`/legado; heróis fiéis não condensam deslocamento nesse inteiro único (o card ignora `Movimento` e mostra os dois passos). Personagens antigos sem o par aparecem no card com o que estiver gravado, sem recálculo.

**Rationale**: FR-002. Snapshot no personagem segue o mesmo padrão de HP/resistências (estado no momento da criação).

**Alternatives considered**:

- Um único `Movimento`: rejeitado na clarificação.
- Só na classe, join no card: rejeitado porque FR-002 exige atribuir ao personagem; extras religiosos/provisão/crítico é que ficam só na classe.

## Decisão 5: Extras da infobox ficam só na classe

**Decision**: `Religiosa`, `ProvisaoInicial` e `BonusAoCriticoDaClasse` (Crit Buff Bonus da wiki **como texto de efeito**, p.ex. `+15% PROT`) vivem no perfil de `Classe`. A criação **não** os copia para `Personagem`. O card lê via join da classe. `Ser.BonusDeCritico` do herói fiel fica `0` (não é o Crit Buff Bonus). CRIT% da arma permanece em `NivelDeArma`. Provisão vazia/`None` é válida. `forward`/`backward` omitidos usam o padrão oficial do template (`1`).

**Rationale**: Clarificação Q5. Evita snapshot que divergiria se o catálogo da classe for corrigido.

**Alternatives considered**:

- Copiar para o personagem: rejeitado pelo usuário.

## Decisão 6: Fórmula fiel sem acessórios

**Decision**: Na criação fiel (os seis campos da 008):

| Atributo | Origem |
|---|---|
| HP máx/atual | armadura nível escolhido (`HpAdicional` = MAX HP wiki) |
| Esquiva | armadura nível escolhido |
| Dano mín/máx, crítico, velocidade | arma nível escolhido |
| Precisão | `0` |
| Proteção | `0` |
| Stress | `0` |
| Chance de virtude | `25` |
| Tamanho / ações | `1` / `1` |
| Passos frente/atrás | classe |
| Resistências escaláveis | base da classe + `10` p.p. × nível de resolução, teto `100` |
| Golpe mortal / armadilha | base da classe, sem bônus de resolução |
| Habilidades | regra 4+4 já especificada |
| Abominação | somente tabelas da forma humana |

Nível de resolução **não** altera HP, SPD, DMG, CRIT nem DODGE. Trinkets fora.

**Rationale**: Assumptions da spec e Combat Mechanics da wiki (infobox Armor/Weapons + resolve resistance).

**Alternatives considered**:

- HP base de classe + HP de armadura: rejeitado; a wiki publica MAX HP na armadura.
- ACC do herói vindo da arma: rejeitado; ACC é da habilidade.

## Decisão 7: Coletor na borda, parser determinístico

**Decision**: Novo projeto `tools/DarkestDungeon.WikiCatalogCollector` (BCL: `HttpClient` + parse de wikitext). Saída: 20 JSON em `wiki-snapshots/`. A API não referencia o coletor. Testes de cobertura leem os JSON commitados, não a rede.

**Rationale**: Constituição V (simplicidade; ferramenta de curadoria como o MediaCollector). Runtime sem dependência de rede.

**Alternatives considered**:

- Endpoint `POST /catalogo/sincronizar-wiki`: rejeitado (wiki no produto).
- HtmlAgilityPack: rejeitado enquanto o wikitext da infobox/equipamento for suficiente.

## Decisão 8: Card usa lista enriquecida + join da classe

**Decision**: `GET /personagens` passa a devolver o necessário para o card: HP, stress, atributos persistidos (incl. passos), oito resistências, habilidades com categoria e nível (inclusive 0), níveis de herói/arma/armadura, aparência, **e** extras da classe (religiosa, provisão, bônus ao crítico) resolvidos no servidor. O frontend agrupa em seis categorias; não esconde nível 0.

**Rationale**: SC-006 pede avaliação sem ação extra. A lista atual omite atributos/resistências/nível 0. Join no servidor evita N+1 e independe de o diálogo de criação ter carregado `/classes`.

**Alternatives considered**:

- Card só com `GET /personagens/{id}` por item: rejeitado (ação extra).
- Frontend busca extras só depois de abrir o formulário: rejeitado (primeira visualização incompleta).

## Decisão 9: Compatibilidade de registros antigos

**Decision**: Sem recálculo silencioso. Migração adiciona colunas novas com defaults seguros (`PassosAFrente`/`PassosAtras` 0 ou nulo distinguível; flags da classe exigem seed). Card mostra o persistido. Criação **nova** exige catálogo completo.

**Rationale**: Edge case da spec e padrão da 008.

**Alternatives considered**:

- Recalcular todos no startup: rejeitado.
