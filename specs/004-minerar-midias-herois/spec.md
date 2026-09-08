# Feature Specification: Importação de Mídias Originais dos Heróis

**Feature Branch**: `004-minerar-midias-herois`

**Created**: 2026-09-08

**Status**: Draft

**Input**: User description: "Importar as imagens e animações originais dos heróis a partir dos arquivos locais instalados do jogo."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Importar imagens das habilidades (Priority: P1)

Como mantenedor do catálogo, quero importar os assets originais instalados das habilidades de cada classe para relacioná-los ao catálogo sem perda de qualidade.

**Why this priority**: As imagens originais preservam o conteúdo licenciado instalado pelo usuário, sem perda de qualidade.

**Independent Test**: Com um diretório de assets de teste contendo uma classe, executar a importação e confirmar que os PNGs originais são copiados sem alteração e que cada associação ou lacuna consta no inventário.

**Acceptance Scenarios**:

1. **Given** uma classe e seus assets instalados, **When** a importação for concluída, **Then** os PNGs originais são copiados integralmente para o acervo e vinculados às habilidades identificadas.
2. **Given** uma mesma textura usada por múltiplas habilidades, **When** ela for importada, **Then** é mantida uma cópia e todas as associações são registradas.
3. **Given** uma habilidade sem região ou asset identificável, **When** a importação terminar, **Then** a lacuna é registrada com o caminho local consultado.

---

### User Story 2 - Preservar animações Spine e paletas (Priority: P2)

Como mantenedor do acervo, quero importar o conjunto original de atlas, texturas e esqueletos de cada herói para preservar estados, animações e paletas sem reconstrução manual.

**Why this priority**: As animações do jogo dependem do trio Spine `.png`, `.atlas` e `.skel`; converter ou recortar esses arquivos perderia informação.

**Independent Test**: Com assets de uma classe contendo `.png`, `.atlas` e `.skel`, confirmar que os três arquivos são copiados sem modificação, associados como conjunto e classificados pela paleta ou estado identificado.

**Acceptance Scenarios**:

1. **Given** um conjunto Spine instalado, **When** ele for importado, **Then** a textura, o atlas e o esqueleto são preservados e relacionados no inventário.
2. **Given** variantes de paleta ou estado, **When** forem importadas, **Then** o inventário distingue cada variante pelo caminho e nome originais.
3. **Given** uma classe DLC instalada, **When** a importação completa for executada, **Then** seus assets são descobertos nos diretórios DLC sem exigir caminho codificado por classe.

---

### User Story 3 - Auditar a origem do acervo (Priority: P3)

Como responsável pelo projeto, quero consultar um inventário rastreável para saber de qual instalação, classe, conjunto Spine e caminho local veio cada arquivo importado.

**Why this priority**: A rastreabilidade permite repetir a importação, identificar lacunas e provar que o conteúdo não foi baixado, convertido ou alterado.

**Independent Test**: Selecionar um arquivo importado e localizar no inventário sua classe, caminho-fonte local, hash, tamanho, tipo e conjunto associado.

**Acceptance Scenarios**:

1. **Given** um arquivo importado, **When** ele for consultado no inventário, **Then** sua origem local, seu hash e seu caminho no acervo são exibidos.
2. **Given** uma importação das 20 classes, **When** o resumo for gerado, **Then** ele informa conjuntos encontrados, arquivos copiados, reutilizados e lacunas por classe.

### Edge Cases

- A instalação indicada pode não existir, não ser um diretório de Darkest Dungeon ou não ter permissão de leitura.
- Uma classe pode não estar instalada porque sua DLC não está presente.
- Um conjunto pode ter PNG e atlas sem `.skel`, ou conter referência a textura inexistente.
- Texturas, atlas ou esqueletos podem ser usados em mais de uma variante.
- O manifesto pode não identificar automaticamente uma habilidade a partir do nome de uma região.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O importador DEVE usar como escopo inicial as 20 classes canônicas listadas em `dados-minerados.md`.
- **FR-002**: O importador DEVE descobrir diretórios de heróis na instalação local base e em DLCs instaladas, sem depender de URLs ou páginas externas.
- **FR-003**: O importador DEVE copiar PNGs de textura, arquivos `.atlas` e arquivos `.skel` sem alterar seus bytes, nomes de origem ou extensão.
- **FR-004**: O importador DEVE identificar cada conjunto Spine e associar no inventário sua textura, atlas, esqueleto, classe, estado e paleta quando identificáveis.
- **FR-005**: O importador DEVE usar os nomes de regiões do atlas e um manifesto versionado para associar conjuntos ou regiões às habilidades catalogadas.
- **FR-006**: O importador DEVE preservar e distinguir variantes por paleta, estado e caminho original, quando presentes.
- **FR-007**: O importador DEVE manter um inventário que associe cada arquivo a classe, habilidade quando aplicável, conjunto Spine, caminho-fonte local, caminho de destino, hash e tamanho.
- **FR-008**: O importador DEVE evitar cópias redundantes de conteúdo idêntico e registrar todas as associações que compartilham um arquivo.
- **FR-009**: O importador DEVE registrar separadamente cada classe, conjunto ou habilidade não localizado, com caminho consultado, motivo observável e data da tentativa.
- **FR-010**: O importador DEVE produzir um resumo de cobertura por classe com conjuntos encontrados, arquivos copiados, reutilizados e lacunas.
- **FR-011**: O inventário DEVE declarar que os arquivos foram importados de uma instalação local licenciada do jogo e que não há redistribuição autorizada pela feature.
- **FR-012**: O escopo DEVE se limitar aos assets de heróis instalados localmente, sem baixar conteúdo de sites externos e sem incluir inimigos ou itens.

### Key Entities

- **Instalação local**: Diretório do jogo fornecido pelo operador, usado apenas como fonte de leitura.
- **Classe de herói**: Uma das 20 classes canônicas que agrupa diretórios e associações de assets.
- **Conjunto Spine**: Grupo coerente formado por textura PNG, atlas e esqueleto `.skel`.
- **Região do atlas**: Região nomeada dentro de um atlas, usada para associação opcional a uma habilidade.
- **Manifesto de associação**: Arquivo versionado que relaciona nomes de conjuntos ou regiões a habilidades do catálogo.
- **Arquivo importado**: Cópia byte a byte de asset local, identificada por hash e caminhos de origem/destino.
- **Registro de lacuna**: Evidência de classe, conjunto ou habilidade que não pôde ser localizado ou associado.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% das 20 classes possuem registro de assets importados ou uma lacuna verificável que informa DLC ou diretório ausente.
- **SC-002**: 100% dos arquivos PNG, `.atlas` e `.skel` descobertos nos diretórios de heróis selecionados são copiados com hash de destino idêntico ao hash de origem.
- **SC-003**: 100% dos conjuntos Spine completos identificados possuem relações verificáveis entre textura, atlas e esqueleto no inventário.
- **SC-004**: Em uma amostra de 20 arquivos importados, 20 de 20 podem ser rastreados do destino até o caminho-fonte local, classe e hash original.
- **SC-005**: O resumo de cobertura permite identificar por classe toda DLC ausente, conjunto incompleto ou habilidade sem associação sem inspeção manual da árvore de arquivos.

## Assumptions

- A instalação local licenciada do usuário está em `C:\Program Files (x86)\Steam\steamapps\common\DarkestDungeon` ou será fornecida por argumento.
- Heróis base ficam em `heroes/`; DLCs podem conter diretórios adicionais de heróis e são descobertas recursivamente.
- O formato de animação é Spine e a preservação do trio `.png`, `.atlas` e `.skel` atende ao requisito de qualidade original; exportação para GIF está fora de escopo.
- O manifesto inicial poderá conter associações manuais quando o nome de uma região do atlas não corresponder de forma inequívoca a uma habilidade.
- Os assets são copiados para uso local do projeto, respeitando a licença do jogo; a feature não autoriza publicação ou redistribuição.
