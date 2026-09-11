# Feature Specification: Imagens no Card do Personagem

**Feature Branch**: `010-imagens-card-personagem`

**Created**: 2026-09-11

**Status**: Draft

**Input**: User description: "agora, eu quero que no card apareça as imagens daquele personagem. deve aparecer: a imagem da arma e armadura no nível certo, as imagens das habilidades e a imagem do herói em si, com a aparencia certa."

## Clarifications

### Session 2026-09-11

- Q: Qual visual do herói o card deve usar para a aparência A, B, C ou D? → A: Retrato e corpo inteiro juntos no mesmo card, ambos na paleta da aparência cadastrada.
- Q: Quais habilidades do card devem mostrar imagem? → A: Todas as habilidades listadas no card (combate e acampamento, qualquer nível, inclusive Nv. 0).
- Q: A imagem da habilidade muda conforme o nível dela, ou é sempre o mesmo ícone daquela habilidade? → A: Um ícone por habilidade, igual em qualquer nível; o nível permanece só no texto.
- Q: Quando faltar uma imagem (retrato, corpo inteiro, arma, armadura ou habilidade), o que o card deve mostrar nesse espaço? → A: Manter o espaço reservado com texto curto em PT-BR (ex.: “sem imagem”), sem silhueta de outra classe e sem omitir o espaço.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Reconhecer o herói pelo retrato da aparência (Priority: P1)

Como consultor do catálogo, quero ver no card o retrato e o corpo inteiro do herói, ambos na classe e na aparência cadastrada (A, B, C ou D), para identificar o personagem de imediato, sem depender só do nome.

**Why this priority**: Retrato e corpo inteiro são o âncora visual do card; sem eles, arma, armadura e habilidades não têm contexto de “quem” está sendo exibido.

**Independent Test**: Abrir a listagem com pelo menos dois personagens da mesma classe e aparências diferentes; confirmar que cada card mostra retrato e corpo inteiro daquela aparência e que o restante do card (atributos e ações) continua utilizável.

**Acceptance Scenarios**:

1. **Given** um personagem com classe e aparência definidas, **When** o usuário visualiza o card, **Then** o card exibe o retrato e o corpo inteiro do herói daquela classe na aparência cadastrada (A, B, C ou D), distinguíveis entre si.
2. **Given** dois personagens da mesma classe com aparências distintas, **When** ambos aparecem na listagem, **Then** cada card mostra retrato e corpo inteiro correspondentes à própria aparência, sem reutilizar o da outra.
3. **Given** que o retrato ou o corpo inteiro daquela classe e aparência não está disponível no acervo, **When** o card é renderizado, **Then** o espaço correspondente permanece visível com o texto “sem imagem”, não substitui pela outra pose nem por outra aparência e continua exibindo nome, classe e demais dados.

---

### User Story 2 - Ver arma e armadura no nível atual (Priority: P1)

Como consultor do catálogo, quero ver no card as imagens da arma e da armadura no nível em que o personagem as possui para conferir o equipamento sem abrir outra tela.

**Why this priority**: Arma e armadura já existem no card como números de nível; as imagens tornam essa informação reconhecível e alinhada ao jogo.

**Independent Test**: Comparar dois personagens da mesma classe com níveis de arma/armadura diferentes e confirmar que as imagens batem com o nível de cada um; um personagem sem equipamento definido deve mostrar ausência clara, sem imagem de outro nível.

**Acceptance Scenarios**:

1. **Given** um personagem com nível de arma definido, **When** o card é exibido, **Then** a imagem da arma corresponde à arma da classe naquele nível (1 a 5).
2. **Given** um personagem com nível de armadura definido, **When** o card é exibido, **Then** a imagem da armadura corresponde à armadura da classe naquele nível (1 a 5).
3. **Given** dois personagens da mesma classe com níveis de arma ou armadura diferentes, **When** ambos são listados, **Then** cada card mostra o visual do próprio nível, sem cruzar imagens.
4. **Given** que o nível de arma ou de armadura não está definido, ou que a mídia daquele nível está ausente, **When** o card é exibido, **Then** o espaço correspondente permanece visível com o texto “sem imagem” e o restante do card não quebra.

---

### User Story 3 - Ver as imagens das habilidades no card (Priority: P2)

Como consultor do catálogo, quero ver no card a imagem de cada habilidade listada — combate e acampamento, inclusive as ainda no nível 0 — junto do nome e do nível para reconhecer o conjunto de relance.

**Why this priority**: Completa o reconhecimento visual do personagem; depende do card já listar as habilidades (feature de frontend existente) e pode ser validada mesmo se retrato ou equipamento falharem pontualmente.

**Independent Test**: Abrir um card com habilidades de combate e de acampamento em níveis 0 e ≥1; confirmar ícone + nome + nível para cada uma; um personagem com habilidade sem mídia deve mostrar o texto e um marcador de ausência, sem esconder a habilidade.

**Acceptance Scenarios**:

1. **Given** um personagem com habilidades listadas no card, **When** o usuário visualiza as seções de combate e de acampamento, **Then** cada habilidade listada — inclusive Nv. 0 — exibe sua imagem, o nome e o nível.
2. **Given** a mesma habilidade em dois personagens com níveis diferentes (por exemplo Nv. 0 e Nv. 3), **When** os cards são exibidos, **Then** o ícone é o mesmo; só o texto do nível muda.
3. **Given** uma habilidade sem imagem no acervo, **When** o card é renderizado, **Then** a habilidade permanece listada com nome, nível e o espaço do ícone visível com o texto “sem imagem”.
4. **Given** um personagem sem habilidades, **When** o card é exibido, **Then** as seções de habilidade seguem o estado vazio já existente, sem espaços de imagem órfãos.

---

### Edge Cases

- Personagem sem nível de arma ou de armadura: o card não inventa um visual de nível 1; o espaço permanece com o texto “sem imagem”.
- Aparência cadastrada sem retrato ou sem corpo inteiro correspondente no acervo: o card não substitui silenciosamente pela outra pose nem por outra aparência da mesma classe; o espaço faltante mostra “sem imagem”.
- Classe ou habilidade recém-cadastrada ainda sem mídia importada: o card permanece utilizável com o espaço reservado e o texto “sem imagem”.
- Falha ao carregar uma imagem individual: aquele espaço passa a “sem imagem”; as demais imagens do mesmo card continuam visíveis; o dado textual (nome, nível, aparência) permanece.
- Muitas habilidades no card: as imagens não devem tornar o card ilegível nem esconder nome, classe ou ações (criar/excluir permanecem fora desta feature, mas o card não pode cobri-las).
- Nível da habilidade diferente entre personagens: o ícone permanece o mesmo; só o texto do nível muda.
- Viewport estreita: retrato, corpo inteiro, equipamento e habilidades continuam identificáveis, sem sobreposição que impeça leitura.
- Dois cards lado a lado da mesma classe: imagens de aparência, arma, armadura e habilidades não vazam de um personagem para o outro.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O card de personagem MUST exibir, juntos, o retrato e o corpo inteiro estático do herói correspondentes à classe e à aparência cadastrada (A, B, C ou D).
- **FR-002**: O card MUST exibir a imagem da arma da classe no nível de arma do personagem, quando esse nível estiver definido.
- **FR-003**: O card MUST exibir a imagem da armadura da classe no nível de armadura do personagem, quando esse nível estiver definido.
- **FR-004**: O card MUST exibir a imagem de cada habilidade listada (combate e acampamento, qualquer nível, inclusive Nv. 0), junto do nome e do nível já apresentados. O ícone MUST ser o da habilidade no catálogo, igual em qualquer nível; o nível MUST aparecer só no texto.
- **FR-005**: Cada imagem MUST corresponder exatamente ao personagem daquele card: mesma classe, mesma aparência, mesmos níveis de equipamento e mesmas habilidades. Retrato, corpo inteiro, arma e armadura MUST NÃO reutilizar visual de outro personagem, outra aparência ou outro nível de equipamento. O ícone de habilidade MUST ser o daquela habilidade, independentemente do nível.
- **FR-006**: Quando uma imagem esperada não existir no acervo, o nível de equipamento não estiver definido ou a imagem falhar ao ser apresentada, o card MUST manter o espaço reservado daquela imagem com o texto curto “sem imagem” em PT-BR, preservar os dados textuais e MUST NÃO substituir por silhueta, outra pose, outra aparência ou outro nível.
- **FR-007**: A listagem MUST continuar apresentando os dados já exigidos pelo frontend de personagens (nome, classe, HP, stress, nível, atributos visíveis, passos e habilidades); as imagens complementam o card, não substituem o texto.
- **FR-008**: As imagens MUST ser apresentadas de forma que o usuário consiga distinguir retrato, corpo inteiro, arma, armadura e cada habilidade (rótulo ou agrupamento visível).
- **FR-009**: O card MUST usar somente mídias já associadas ao catálogo e ao acervo local; esta feature não importa arquivos novos nem altera o cadastro do personagem.
- **FR-010**: A ausência de imagens MUST NÃO impedir consultar, criar ou excluir personagens nas ações já existentes da tela.

### Key Entities

- **Card de personagem**: representação visual de um herói cadastrado na listagem, agora incluindo retrato, equipamento e ícones de habilidade além dos dados textuais.
- **Retrato do herói**: imagem estática de busto/rosto do herói na forma humana da classe, na paleta da aparência A–D do personagem.
- **Corpo inteiro do herói**: sprite estático de corpo inteiro do herói na forma humana da classe, na mesma paleta de aparência A–D; no card aparece junto do retrato, sem substituí-lo.
- **Imagem de arma**: visual do equipamento de arma da classe no nível atual do personagem (1 a 5).
- **Imagem de armadura**: visual do equipamento de armadura da classe no nível atual do personagem (1 a 5).
- **Imagem de habilidade**: ícone único associado à habilidade de combate ou acampamento no catálogo; não varia com o nível do personagem.
- **Ausência de mídia**: estado visível quando o acervo não tem (ou não entrega) a imagem correspondente, ou quando o nível de arma/armadura não está definido; o espaço permanece e exibe o texto “sem imagem”; o card continua válido.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Em uma listagem com pelo menos 5 personagens de classes e aparências variadas, um consultor identifica corretamente retrato, corpo inteiro, arma, armadura e ao menos uma habilidade de cada card em até 10 segundos por personagem, sem abrir outra tela.
- **SC-002**: Em 100% dos cards com nível de arma e de armadura definidos e mídia disponível, as imagens de equipamento batem com o nível mostrado no próprio card.
- **SC-003**: Em 100% dos cards com aparência definida e mídias disponíveis, retrato e corpo inteiro correspondem à letra de aparência exibida no card (A–D).
- **SC-004**: Em 100% das habilidades listadas com mídia disponível, o ícone apresentado é o daquela habilidade (o mesmo em qualquer nível), não de outra do mesmo card.
- **SC-005**: Quando falta qualquer uma das imagens (retrato, corpo inteiro, arma, armadura ou habilidade), 100% desses cards mostram o espaço correspondente com o texto “sem imagem”, continuam permitindo ler nome, classe e dados numéricos e não bloqueiam as demais imagens presentes.
- **SC-006**: Pelo menos 90% dos consultores em um teste informal reconhecem, no primeiro olhar, que o card mostra “o retrato, o corpo inteiro, a arma, a armadura e as habilidades”, sem instrução extra.

## Assumptions

- O acervo de mídias de heróis (incluindo variantes de aparência) e o de armas/armaduras por nível já foram importados e vinculados em features anteriores; esta feature apenas apresenta o que já existe.
- “Imagem do herói” significa retrato estático e corpo inteiro estático da forma humana da classe na aparência cadastrada, ambos no mesmo card; não é uma animação completa.
- As habilidades visíveis no card são as mesmas já listadas hoje (combate e acampamento); cada uma listada recebe o ícone único daquela habilidade, inclusive Nv. 0. Habilidades não listadas não ganham imagem nesta feature.
- Acessórios, consumíveis, provisões e animações Spine fora do retrato estático ficam fora de escopo.
- Personagens já cadastrados não precisam ser recriados: o card resolve as imagens a partir da classe, aparência, níveis de equipamento e habilidades atuais.
- Textos, rótulos de ausência e mensagens de falha de imagem seguem PT-BR. O texto canônico do espaço vazio é “sem imagem”.
- O formulário de criação não passa a exigir upload de imagens; a aparência A–D continua sendo a única escolha visual do usuário no cadastro.

## Out of Scope

- Importar, minerar ou republicar arquivos de mídia.
- Alterar regras de criação fiel, desbloqueio de habilidades, atributos ou Crit Buff.
- Exibir imagens no formulário de criação (além do que o card da listagem já mostrar após salvar).
- Galeria, zoom, troca interativa de aparência no card ou preview animado.
- Imagens de inimigos ou de classes sem personagem cadastrado.
