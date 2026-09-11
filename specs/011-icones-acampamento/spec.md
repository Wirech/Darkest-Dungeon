# Feature Specification: Ícones de Habilidades de Acampamento

**Feature Branch**: `011-icones-acampamento`

**Created**: 2026-09-11

**Status**: Draft

**Input**: User description: "faça isso" — após o mapa de lacunas: coletar `raid/camping/skill_icons` e equivalentes DLC, manifesto de camping e resolver no card. Sem coletar `panels/icons_equip` nesta feature.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Inventariar ícones de acampamento da instalação (Priority: P1)

Como curador do acervo, quero importar da instalação local licenciada os PNGs `camp_skill_*.png` (vanilla em `raid/camping/skill_icons` e DLCs equivalentes) para o acervo de heróis, sem alterar bytes e sem apagar os arquivos já importados na Feature 004.

**Why this priority**: Sem os arquivos no acervo o card continua Pendente. É o MVP: mesmo antes do manifesto, o curador já tem os ícones rastreáveis.

**Independent Test**: Com uma instalação de teste contendo pelo menos um `camp_skill_encourage.png` vanilla e um PNG DLC, executar a coleta e confirmar cópia byte a byte, inventário com origem/hash e preservação dos arquivos 004 já existentes.

**Acceptance Scenarios**:

1. **Given** a instalação local com PNGs de camping vanilla e DLC, **When** o curador executa a coleta de acampamento, **Then** cada `camp_skill_*.png` encontrado é copiado sem alteração de bytes para o acervo de heróis e consta no inventário com caminho-fonte, destino, hash e tamanho.
2. **Given** o acervo 004 já preenchido, **When** a coleta de acampamento termina, **Then** os arquivos anteriores de heróis permanecem e o inventário passa a listar também os ícones de camping (mescla, não substituição cega).
3. **Given** um PNG idêntico já presente no acervo, **When** a coleta encontra o mesmo conteúdo, **Then** evita cópia redundante e registra a associação.
4. **Given** pasta de camping ausente na instalação, **When** a coleta é executada, **Then** a lacuna é registrada com o caminho consultado e o restante da importação continua.

---

### User Story 2 - Manifesto liga NomeOriginal ao PNG (Priority: P1)

Como curador do catálogo, quero um manifesto versionado que associe cada habilidade de acampamento do seed (NomeOriginal wiki) ao arquivo `camp_skill_{id}.png` correspondente, incluindo aliases (Wound Care → `first_aid`, Snuff Box → `forage`, skills DLC `lash_*` / `way_of_*`) e as três skills compartilhadas.

**Why this priority**: O card resolve por NomeOriginal; sem manifesto os PNGs importados não aparecem nas habilidades certas.

**Independent Test**: Consultar o manifesto e confirmar que as 79 skills do catálogo têm associação; skills compartilhadas apontam para o mesmo arquivo; leftovers (`bandage`, `hobby`, `wrap`, etc.) não entram como habilidades do catálogo.

**Acceptance Scenarios**:

1. **Given** o catálogo com 79 habilidades de acampamento, **When** o manifesto de camping é gerado/atualizado, **Then** cada NomeOriginal aponta para um `camp_skill_*.png` existente no acervo.
2. **Given** Encourage, Wound Care e Pep Talk, **When** o manifesto é lido, **Then** as três compartilham os arquivos `camp_skill_encourage.png`, `camp_skill_first_aid.png` e `camp_skill_pep_talk.png` para todas as classes elegíveis (exceto Flagelante, que não as usa).
3. **Given** aliases wiki (Unshakable Leader, Lash's Anger, Snake Eyes, Pick Pocket, Again!, Resupply, The Cure), **When** o manifesto é consultado, **Then** cada um aponta para o id de arquivo do jogo, não para um slug ingênuo da wiki.
4. **Given** PNGs leftover (`bandage`, `bear_traps`, `hobby`, `perimeter_alarms`, `wrap`), **When** o manifesto é gerado, **Then** eles não são associados a skills do catálogo.

---

### User Story 3 - Card mostra ícone de acampamento (Priority: P2)

Como consultor do catálogo, quero ver no card o ícone de cada habilidade de acampamento listada (inclusive Nv. 0), no mesmo espaço já previsto pela Feature 010, em vez do texto “sem imagem”.

**Why this priority**: Entrega o valor visível; depende do acervo e do manifesto, mas não muda o layout do card.

**Independent Test**: Abrir um personagem fiel (ex. Cruzado) e confirmar que as skills de acampamento têm URL `/acervo/herois/.../camp_skill_*.png` com status OK; combate continua igual; habilidade sem arquivo permanece “sem imagem”.

**Acceptance Scenarios**:

1. **Given** um personagem com habilidades de acampamento no card e PNGs no acervo, **When** a listagem é consultada, **Then** cada skill de acampamento exibe ícone OK (URL relativa), nome e nível.
2. **Given** a mesma skill de acampamento em dois personagens com níveis diferentes, **When** os cards são exibidos, **Then** o ícone é o mesmo; só o texto do nível muda.
3. **Given** skills de combate, **When** o card é exibido após esta feature, **Then** os ícones de combate permanecem os do manifesto 004, sem regressão.
4. **Given** uma skill de acampamento ainda sem arquivo, **When** o card é renderizado, **Then** o espaço permanece com “sem imagem” (comportamento 010).

---

### Edge Cases

- Flagelante não recebe as três skills compartilhadas; seus quatro `lash_*` vêm da DLC Crimson Court.
- Gallows Humor é compartilhada entre Bandido e Ladrão de Cova: um PNG, duas classes.
- Field Dressing / Marching Plan / Triage são compartilhadas entre Besteiro e Musqueteiro: um PNG cada.
- PNG em pasta DLC (não em `raid/camping/skill_icons`) deve ser encontrado sem caminho hardcoded por DLC.
- Coleta com `--continuar` não apaga inventário 004.
- Leftovers da instalação não viram skills fantasma no card.
- Trinkets e `panels/icons_equip` ficam fora desta feature.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O coletor MUST descobrir `camp_skill_*.png` em `raid/camping/skill_icons` e em pastas equivalentes sob `dlc/**/raid/camping/`, sem URL externa.
- **FR-002**: O coletor MUST copiar os PNGs sem alterar bytes, nomes de origem ou extensão, para o acervo de heróis (`assets/herois`), em pasta dedicada de camping (não misturar com Spine da classe).
- **FR-003**: A coleta MUST mesclar o inventário existente da Feature 004: arquivos anteriores permanecem; novos ícones de camping são acrescentados.
- **FR-004**: O coletor MUST registrar lacunas (pasta ausente, arquivo do manifesto não encontrado) com caminho consultado, motivo e data UTC.
- **FR-005**: O manifesto MUST associar cada NomeOriginal de acampamento do seed ao arquivo `camp_skill_{idJogo}.png`, usando a tabela de aliases wiki→id do jogo.
- **FR-006**: Skills compartilhadas MUST reutilizar o mesmo arquivo; classes DLC MUST usar os PNGs das respectivas DLCs quando a vanilla não os tiver.
- **FR-007**: Leftovers (`bandage`, `bear_traps`, `hobby`, `perimeter_alarms`, `wrap`) MUST NÃO ser associados a habilidades do catálogo.
- **FR-008**: O resolvedor do card MUST localizar ícones de acampamento pelo manifesto mesmo quando o arquivo não estiver na pasta da classe (camping é compartilhado).
- **FR-009**: Combate MUST continuar resolvendo pelo manifesto 004 (`*.ability.*.png`); esta feature MUST NÃO reescrever associações de combate.
- **FR-010**: O card MUST continuar usando só acervo local; textos de ausência permanecem “sem imagem” em PT-BR.
- **FR-011**: Esta feature MUST NÃO coletar `panels/icons_equip` nem `inventory/**` de itens/trinkets.
- **FR-012**: O inventário MUST declarar origem em instalação local licenciada e que não há redistribuição autorizada.

### Key Entities

- **Ícone de acampamento**: PNG `camp_skill_{id}.png` da instalação, copiado ao acervo.
- **Manifesto de habilidades**: documento versionado que liga Classe + NomeOriginal (combate e acampamento) ao nome de arquivo.
- **Alias wiki→jogo**: mapeamento NomeOriginal do seed para o id usado no JSON/PNG do jogo.
- **Inventário de heróis**: lista mesclada de arquivos 004 + camping, com hash e proveniência.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% das 79 habilidades de acampamento do catálogo têm associação no manifesto para um PNG existente no acervo após a coleta numa instalação completa (vanilla + DLCs do seed).
- **SC-002**: Em 100% dos cards com skills de acampamento e mídia disponível, o ícone deixa de ser Pendente e a URL aponta para `camp_skill_*.png` sob `/acervo/herois/`.
- **SC-003**: Após a coleta, o inventário de heróis contém todos os arquivos 004 anteriores mais os ícones de camping (nenhuma perda líquida dos 2645 arquivos já importados, salvo reutilização por hash).
- **SC-004**: 100% dos ícones de combate nos cards de regressão mantêm a mesma URL de antes desta feature.
- **SC-005**: Uma coleta com pasta de camping ausente termina com lacuna registrada e código de saída de sucesso da CLI (não aborta o restante).

## Assumptions

- A instalação Steam local do curador é a fonte licenciada; DLCs Flagellant, Shieldbreaker, Runaway e Duelist estão presentes quando o seed as exige.
- Feature 010 já reserva o espaço no card; esta feature só preenche o acervo e o resolvedor.
- `panels/icons_equip` (trinkets, supply, etc.) fica para feature futura.
- Leftovers da instalação podem ser copiados se o glob `camp_skill_*.png` os incluir, mas não entram no manifesto do catálogo.
- Geração do manifesto de camping é determinística a partir da tabela de aliases + ids do JSON do jogo, não por ordem de arquivo na pasta da classe.

## Out of Scope

- Coleta de ícones de inventário/trinket em `panels/`.
- Recorte, conversão ou redimensionamento de PNG.
- Alterar seed de habilidades, custos ou efeitos.
- Novo endpoint HTTP de importação (continua CLI).
- Animação Spine de camping.
