# Pesquisa: Importação e Vínculo de Mídias de Equipamento e Itens

**Feature**: 006-midias-armas-armaduras-itens  
**Date**: 2026-09-10  
**Prerequisites**: [spec.md](./spec.md), Feature 003 (catálogo), Feature 004 (inventário/CLI), Feature 005 (padrão de vínculo por hash)

Todas as ambiguidades da spec (incluindo caminhos de pasta adiados na clarificação Q4) ficam resolvidas aqui. Não resta `NEEDS CLARIFICATION`.

---

## Decisões

### Reutilizar o CLI `tools/DarkestDungeon.MediaCollector`

- **Decisão**: Estender o coletor da Feature 004 no mesmo projeto (`tools/DarkestDungeon.MediaCollector`), sem criar um segundo executável. Novas opções: `--categoria` (repetível) e destino padrão `assets/equipamentos-itens/`. A descoberta deixa de ser exclusiva de `heroes/` quando a categoria não for herói.
- **Justificativa**: FR-003 pede reuso do inventário 004 (hash SHA-256, cópia byte a byte, lacunas, declaração de uso local). Um segundo CLI duplicaria parser de atlas, deduplicação e contrato de `inventario.json`. Constituição V exige a solução mais simples.
- **Alternativas consideradas**: Novo `tools/DarkestDungeon.EquipmentCollector` (rejeitado: duplicação). Job HTTP de importação (rejeitado: Feature 004 já isolou I/O de disco na CLI; a API continua stateless).

### Mapeamento pasta/origem → categoria (FR-004)

- **Decisão**: Classificar **somente** pela origem relativa na instalação (raiz do jogo e cada DLC), via tabela versionada no coletor (`mapeamento-pastas.json`). Arquivo fora das origens mapeadas **não** é classificado por nome avulso; permanece órfão no inventário (US3 cenário 5).

Tabela padrão (caminhos relativos à raiz da instalação e a cada pasta `dlc/*/`):

| Categoria (PT-BR) | Origens relativas consultadas | Extensões |
|---|---|---|
| Arma | `heroes/**` com nome de arquivo contendo `weapon`; equivalentes em `dlc/**/heroes/**` | `.png` obrigatório para `OK`; `.atlas` + `.skel` opcionais (conjunto) |
| Armadura | `heroes/**` com nome contendo `armour` **ou** `armor`; equivalentes DLC | idem |
| Acessório / troféu | `inventory/trinkets/**` e `dlc/**/inventory/trinkets/**` | `.png` (ícone); trio Spine se existir no mesmo prefixo |
| Item de acampamento / provisão | `inventory/provision/**` e `dlc/**/inventory/provision/**` | `.png` |
| Consumível | `inventory/quest/**` e `inventory/raid/items/**`; equivalentes DLC | `.png` |

Regras:

- Cada origem da tabela liga-se a **exatamente um** tipo. `inventory/provision/**` nunca vira consumível; `inventory/quest/**` e `inventory/raid/items/**` nunca viram acampamento.
- Padrões `weapon` / `armour` / `armor` só se aplicam **dentro** das origens já mapeadas como arma/armadura (`heroes/**`). Isso não classifica provisão vs consumível por nome.
- Se a instalação do curador usar árvore diferente, o operador ajusta `mapeamento-pastas.json` (caminho via `--mapeamento` ou arquivo ao lado do coletor). Não hardcodar caminhos absolutos no Domain.
- Descoberta DLC: recursiva como na Feature 004 (não listar DLC por nome).
- Pastas típicas **fora de escopo** (não entram no mapeamento): `enemies/`, `dungeons/`, `shared/effects/`, `raid/camping/skills/`, ícones de status. SC-009.

- **Justificativa**: A clarificação Q4 exigiu pasta/origem, não heurística de filename solto. A instalação vanilla concentra trinkets em `inventory/trinkets` e sprites de arma/armadura nas pastas de herói; provisões e quest items vivem em subárvores distintas de `inventory/`.
- **Alternativas consideradas**: Classificar por prefixo de arquivo em `inventory/` plano (rejeitado: viola FR-004). Codificar caminhos DLC (`dlc/crimson_court/...`) (rejeitado: frágil, Feature 004 já recusou).

### Ícone PNG obrigatório para `OK`; conjunto Spine opcional

- **Decisão**: Status de um vínculo individual é `OK` só com ícone estático (PNG) resolvido no inventário (hash 64 hex). Trio `.png` + `.atlas` + `.skel` é registrado em `ConjuntoSpineId` quando os três (ou png+atlas) existirem; ausência do animado **não** impede `OK`.
- **Justificativa**: Spec Assumptions e FR-006/007. Armas/armaduras de herói frequentemente têm Spine; trinkets e provisões em geral só ícone.
- **Alternativas consideradas**: Exigir Spine para `OK` (rejeitado: a maioria dos itens de inventário não tem skel). Aceitar qualquer extensão de imagem (rejeitado: política 004 é png/atlas/skel).

### Referência de mídia no catálogo = inventário + hash (padrão 005)

- **Decisão**: Domínio **não** guarda caminho de disco nem bytes. Cada vínculo persiste `ArquivoInventarioId` (chave estável do inventário, máx. 200) e/ou `ConjuntoSpineId` (máx. 200), mais `HashArquivo` SHA-256 (64 chars) e `Status` (`OK` | `Pendente`). Mesmo padrão de `AssetsDeClasse`.
- **Justificativa**: FR-008; Feature 005 Q1. Detecta substituição silenciosa do arquivo. Reuso por hash cumpre FR-003 mesmo se o PNG já estiver em `assets/herois/`.
- **Alternativas consideradas**: Coluna `CaminhoRelativo` no item (rejeitado: duplica inventário e quebra quando o acervo muda de pasta). Embed de bytes no SQL (rejeitado: escala e licença).

### Tipos novos no TPH de `Item`: `ItemDeAcampamento` e `Consumivel`

- **Decisão**: Duas classes irmãs de `Arma` / `Armadura` / `Acessorio`, herdando `Item`. Discriminador EF: `"ItemDeAcampamento"` e `"Consumivel"`. Sem atributos numéricos de combate nesta feature (nome, descrição, mídia). Não reclassificar registros 003.
- **Justificativa**: Clarificação Q1; FR-011. TPH já usado em `Itens` (`HasDiscriminator`). Constituição V: não criar TPT só para dois tipos magros.
- **Alternativas consideradas**: Um único tipo `Suprimento` com enum (rejeitado: Q1 pediu dois tipos). Tabela separada fora de `Item` (rejeitado: quebra `GET /itens/{id}` e o inventário de Personagem futuro).

### Acessórios novos a partir de trinkets ausentes no catálogo 003

- **Decisão**: Match de trinket existente por `NomeOriginal` normalizado (arquivo sem extensão vs `Acessorio.NomeOriginal`, case-insensitive). Se houver match: só preencher mídia; **não** alterar Id, raridade, efeitos, classe exclusiva, conjunto. Se não houver: `new Acessorio(...)` com Guid **novo**, `RaridadeDeAcessorio.Comum`, efeitos vazios, `classeExclusiva: null`, `conjuntoId: null`, nome/descrição derivados do stem do arquivo (PT-BR na exibição pode repetir o original até curadoria). Não reseedar 003.
- **Justificativa**: Clarificações Q2 e Q3; FR-010.
- **Alternativas consideradas**: Recriar seed 003 (proibido). Inferir raridade pela pasta DLC (rejeitado: Q3 fixou `Comum`).

### Publicador de vínculos **distinto** do `IPublicadorAtomicoService` da 005

- **Decisão**: Novo contrato `IPublicadorDeVinculosDeMidia` (Application) + implementação Infrastructure. Transação SQL Server **por categoria**. Pode rodar a qualquer momento. **Não** consulta `IDetectorDeSessoesAtivas`. **Não** exige `confirmacaoJanelaManutencao`. **Não** altera `Personagem.ArmaEquipadaId`, `ArmaduraEquipadaId` nem slots de acessório. Concorrência: 409 se a **mesma categoria** já estiver em publicação; categorias distintas podem serializar no mesmo serviço com lock por categoria (não global).
- **Justificativa**: Clarificação Q5 / FR-009. Reusar o publicador 005 puxaria janela de manutenção e bloqueio por sessão — incompatível.
- **Alternativas consideradas**: Flag opcional no publicador 005 (rejeitado: mistura invariantes e complica testes 005). Publicar vínculos no seed EF estático (rejeitado: mídia depende da instalação local do curador).

### Inventário: merge por hash com o acervo 004

- **Decisão**: O coletor 006 grava `assets/equipamentos-itens/inventario.json` (mesmo shape estendido com `Categoria`). Antes de copiar, consulta hashes já presentes em `assets/herois/inventario.json` **e** no inventário 006. Hit: `Reutilizado = true`, sem segunda cópia física; associação nova aponta para `CaminhoDestino` existente (pode viver sob `assets/herois/`).
- **Justificativa**: FR-003, SC-001. Sprites de arma frequentemente já foram copiados com o herói.
- **Alternativas consideradas**: Sempre copiar para `equipamentos-itens/` (rejeitado: duplica gigabytes). Um único `inventario.json` na raiz `assets/` nesta feature (adiado: mudaria o contrato 004; merge na leitura basta).

### Relatório de cobertura no espírito 005, sem job wiki

- **Decisão**: Relatório **em banco + DTO da API** (geração síncrona na consulta ou no POST de publicação), mensagens PT-BR `OK` / `Parcial` / `Pendente`. Não há mineração wiki. Meta SC-008: ≤ 2 minutos no volume desta feature (centenas de vínculos, não 1095 níveis de habilidade). Job CLI opcional só para o coletor, não para o relatório.
- **Justificativa**: US4; Feature 005 isolou job pesado porque batia wiki. Aqui a fonte já está no SQL + inventário local.
- **Alternativas consideradas**: Só `relatorio.md` em disco (rejeitado: curador remoto precisa da API). Reusar `IAuditoriaWikiService` (rejeitado: acoplamento falso).

### Consultas de item e Personagem devolvem mídia resolvida

- **Decisão**: Estender `ItemDetalheDto` e `PersonagemDetalheDto` com objetos de mídia opcionais. Personagem: mídia do **nível atual** da arma/armadura equipada (nível de resolução do herói mapeado para slot 1–5 já existente na 005) e dos acessórios equipados. Vínculo `Pendente` → campos de mídia nulos + `status: "Pendente"`; HTTP 200, sem erro de negócio (FR-014, FR-018).
- **Justificativa**: US2 cenário 4; SC-006.
- **Alternativas consideradas**: Endpoint só de mídia separado (rejeitado: o jogador já consulta o personagem). 404 quando pendente (rejeitado pela spec).

### Destino de arquivos e declaração de licença

- **Decisão**: Cópia para `assets/equipamentos-itens/arquivos/{categoria}/{nome-original.ext}` com nomes relativos seguros (sanitizar `..`). `inventario.json` inclui `DeclaracaoDeUso` idêntica em espírito à 004: origem = instalação local licenciada; redistribuição não autorizada (FR-015).
- **Justificativa**: Política 004 + FR-002/015.
- **Alternativas consideradas**: Misturar em `assets/herois/` (rejeitado: dificulta SC-009).

---

## Pesquisa de instalação (vanilla + DLC)

Fontes: estrutura já usada pela Feature 004 (`heroes/` recursivo, DLC como subárvore), layout público da instalação Steam `DarkestDungeon/`, e a restrição FR-016.

Conclusão operacional: o coletor **não** precisa da wiki; se uma origem da tabela não existir, registra `LacunaDeImportacao` (caminho consultado, motivo, UTC) e segue (FR-005, SC-007).

---

## Impacto em testes existentes

- 206 testes da linha 005 permanecem verdes: publicador 005 **intocado**.
- `ItensEndpointsTests` / `PersonagensEquipamentoTests`: contratos GET ganham campos opcionais de mídia (compatível se o JSON ignora extras; atualizar asserts que usam igualdade estrita de DTO).
- Architecture tests: novos tipos em Domain; Application não referencia `System.IO` de instalação.

---

## Itens em aberto proibidos neste documento

Nenhum. Caminhos de pasta, publisher, TPH, raridade default e reuso de CLI estão fechados.
