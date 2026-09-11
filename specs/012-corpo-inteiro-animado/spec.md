# Feature Specification: Corpo Inteiro Composto e Animado

**Feature Branch**: `012-corpo-inteiro-animado`

**Created**: 2026-09-11

**Status**: Draft

**Input**: User description: "perfeito! agora quero que, onde aparece o \"corpo inteiro\", o usuário possa selecionar qual versão aparece. atualmente o corpo inteiro é uma imagem das partes do corpo do personagem separados. eles são usados pra compor o corpo como aparece no jogo e também pra fazer as animações. faça isso funcionar visualmente"

## Clarifications

### Session 2026-09-11

- Q: Além da pose de espera, o seletor do corpo inteiro deve oferecer outros movimentos do jogo (ataque, caminhada, etc.)? → A: Espera, animado e caminhada (sem demais ciclos nesta entrega).
- Q: Depois que o consultor recarrega a listagem, a versão escolhida no card deve continuar a mesma? → A: Só enquanto a listagem está aberta; recarregar volta a “Em espera”. Não grava no cadastro.
- Q: Quando o consultor escolhe “Caminhada”, o herói deve permanecer dentro do espaço “Corpo inteiro” ou pode sair andando para fora do card? → A: Caminha no lugar, sempre visível dentro do espaço “Corpo inteiro”.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Ver o herói montado como no jogo (Priority: P1)

Como consultor do catálogo, quero que o espaço “Corpo inteiro” mostre o herói já composto (partes do corpo encaixadas na pose de espera), na classe e na aparência cadastrada, em vez da folha com membros separados, para reconhecer o personagem como ele aparece no jogo.

**Why this priority**: Sem a composição, o espaço atual não cumpre o papel de “corpo inteiro”; é o MVP visual.

**Independent Test**: Abrir a listagem com um personagem cuja mídia de corpo esteja disponível; confirmar que o espaço rotulado “Corpo inteiro” mostra a silhueta montada (cabeça, tronco e membros no lugar), não a grade de partes soltas; o retrato e o restante do card continuam iguais.

**Acceptance Scenarios**:

1. **Given** um personagem com classe, aparência e conjunto de corpo disponíveis, **When** o consultor visualiza o card, **Then** o espaço “Corpo inteiro” exibe o herói composto na pose de espera daquela classe e aparência (A–D).
2. **Given** dois personagens da mesma classe com aparências distintas, **When** ambos aparecem na listagem, **Then** cada corpo composto usa a paleta da própria aparência, sem misturar partes da outra.
3. **Given** que o conjunto necessário para montar o corpo não está disponível, **When** o card é renderizado, **Then** o espaço “Corpo inteiro” permanece visível com o texto “sem imagem”, sem mostrar a folha de partes soltas como substituto e sem afetar retrato, arma, armadura ou habilidades.

---

### User Story 2 - Escolher a versão visual do corpo (Priority: P1)

Como consultor do catálogo, quero escolher, no próprio card, qual versão do corpo inteiro aparece — **Em espera**, **Animado** (ciclo de espera) e **Caminhada**, quando existirem — para comparar o herói parado, em espera e andando sem sair da listagem.

**Why this priority**: É o pedido explícito do usuário; a composição sozinha não cobre “selecionar qual versão aparece”.

**Independent Test**: No card de um personagem com conjunto completo, alternar entre as versões oferecidas e ver o espaço “Corpo inteiro” atualizar na hora; a escolha de um card não altera o corpo dos outros.

**Acceptance Scenarios**:

1. **Given** um card com corpo disponível, **When** o consultor abre o seletor no espaço “Corpo inteiro”, **Then** vê as versões disponíveis em rótulos PT-BR: “Em espera”, “Animado” e “Caminhada” (cada uma só como opção funcional se o conjunto correspondente existir).
2. **Given** a versão “Em espera” selecionada (padrão), **When** o card é exibido, **Then** o espaço mostra o herói composto parado, reconhecível como a forma humana da classe na aparência cadastrada.
3. **Given** a versão “Animado” disponível, **When** o consultor a seleciona, **Then** o mesmo espaço reproduz o ciclo de espera do jogo (partes do corpo em movimento), na mesma classe e aparência, sem recarregar a página.
4. **Given** a versão “Caminhada” disponível, **When** o consultor a seleciona, **Then** o mesmo espaço reproduz o ciclo de caminhada do jogo, na mesma classe e aparência, sem recarregar a página; o herói permanece inteiro e visível dentro do espaço “Corpo inteiro” (caminha no lugar).
5. **Given** dois cards lado a lado, **When** o consultor muda a versão em um deles, **Then** o outro card mantém a versão que já tinha.
6. **Given** que só a pose composta está disponível, **When** o consultor tenta usar o seletor, **Then** “Animado” e “Caminhada” não são oferecidas como opção funcional (ou aparecem indisponíveis) e o espaço continua na pose composta.
7. **Given** um card em “Animado” ou “Caminhada”, **When** o consultor recarrega a listagem, **Then** todos os cards voltam a “Em espera”; a escolha anterior não reaparece.
8. **Given** a versão “Caminhada” em reprodução, **When** o ciclo avança, **Then** o herói não sai do espaço “Corpo inteiro” nem cobre retrato, texto ou o card vizinho.

---

### User Story 3 - Manter o card utilizável enquanto o corpo anima (Priority: P2)

Como consultor do catálogo, quero que retrato, equipamento, habilidades e ações do card continuem legíveis e clicáveis enquanto o corpo inteiro está composto ou animado, para consultar o personagem sem o visual atrapalhar o restante da ficha.

**Why this priority**: Completa o uso real da listagem; pode ser validado depois do MVP de composição e seleção.

**Independent Test**: Com o corpo em “Animado”, ler nome, classe, HP e uma habilidade e acionar uma ação já existente do card (quando houver); nada deve ficar coberto ou travado pela animação.

**Acceptance Scenarios**:

1. **Given** o corpo inteiro em qualquer versão selecionada, **When** o consultor lê o card, **Then** retrato, arma, armadura, habilidades, nome e números permanecem visíveis e distinguíveis do corpo.
2. **Given** o corpo em “Animado”, **When** o consultor interage com o restante do card, **Then** a animação não bloqueia cliques nem esconde rótulos.
3. **Given** viewport estreita, **When** o corpo composto ou animado é exibido, **Then** o espaço de corpo permanece identificável e o card não sobrepõe informações essenciais.

---

### Edge Cases

- Conjunto de corpo incompleto (faltam partes, mapa de encaixe ou ciclo de espera): o espaço mostra “sem imagem”; não monta um herói deformado nem cai de volta na folha de partes soltas.
- “Animado” ou “Caminhada” indisponível para a classe ou aparência: o seletor não força essa versão; o padrão permanece “Em espera” se a pose composta existir.
- Falha ao apresentar a versão escolhida: aquele espaço volta a “sem imagem” ou à última versão válida do mesmo personagem; os demais cards não são afetados.
- Troca rápida entre versões no mesmo card: o espaço mostra só a versão atual, sem sobrepor espera e animação.
- Personagem sem aparência definida: aplica-se a mesma regra já usada no card (aparência A); o corpo composto, se existir, segue essa paleta.
- Recarregar a listagem (ou reabrir a tela): todas as versões escolhidas são descartadas; cada card volta a “Em espera”.
- Caminhada (e demais versões em movimento): o herói permanece no espaço “Corpo inteiro”, visível por inteiro; não atravessa retrato, rótulos ou o card ao lado.
- Muitos cards na listagem com animação ligada: cada corpo continua correspondente ao próprio personagem; a listagem permanece rolável e utilizável.
- Abominação e demais classes com forma humana no card: o corpo composto e animado desta feature é a forma humana já usada no retrato/corpo da 010; outras formas ficam fora de escopo.
- Esta feature não importa arquivos novos: usa o acervo de heróis já coletado (conjuntos de partes, mapas e ciclos da Feature 004).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O espaço rotulado “Corpo inteiro” MUST apresentar o herói **composto** (partes do corpo encaixadas na pose de espera), e MUST NÃO usar como visual principal a folha com partes separadas.
- **FR-002**: O corpo composto MUST corresponder à classe e à aparência cadastrada (A–D) daquele personagem, na forma humana já adotada pelo card.
- **FR-003**: O usuário MUST poder selecionar, no próprio card, qual versão ocupa o espaço “Corpo inteiro”, entre as versões disponíveis daquele personagem.
- **FR-004**: O seletor MUST oferecer no máximo três versões nesta entrega: “Em espera” (pose composta parada, padrão), “Animado” (ciclo de espera do jogo) e “Caminhada” (ciclo de caminhada do jogo). MUST NÃO expor ataque nem demais ciclos. Cada versão animada, quando disponível, MUST usar as mesmas partes e paleta da aparência cadastrada.
- **FR-005**: A seleção de versão MUST ser por card: alterar um personagem MUST NÃO alterar o corpo dos demais na listagem. A escolha MUST durar só enquanto a listagem permanece aberta; recarregar ou reabrir a tela MUST voltar todos os cards a “Em espera”. MUST NÃO gravar a versão no cadastro do personagem.
- **FR-006**: Quando o conjunto para compor ou animar não existir, estiver incompleto ou falhar ao ser apresentado, o espaço MUST permanecer visível com o texto “sem imagem” em PT-BR e MUST NÃO substituir por folha de partes soltas, outra classe, outra aparência ou outro personagem.
- **FR-007**: Retrato, arma, armadura, habilidades e dados textuais do card MUST permanecer como na Feature 010/011; esta feature altera somente o conteúdo e o controle do espaço “Corpo inteiro”.
- **FR-008**: Rótulos do seletor e estados de ausência MUST estar em PT-BR.
- **FR-009**: Esta feature MUST NÃO importar mídia nova nem alterar o cadastro do personagem (classe, aparência, equipamentos, habilidades).
- **FR-010**: Composição e animação MUST usar somente o acervo local já associado à classe e à aparência; MUST NÃO baixar conteúdo de wiki ou de outra origem em tempo de apresentação.
- **FR-011**: A animação, quando ligada, MUST NÃO impedir leitura nem ações já existentes da listagem.
- **FR-012**: Em qualquer versão, o herói MUST permanecer visível dentro do espaço “Corpo inteiro”. Na “Caminhada”, o movimento MUST ser no lugar (sem sair da área). MUST NÃO invadir retrato, texto, controles do card nem o card vizinho.

### Key Entities

- **Espaço Corpo inteiro**: área já existente no card, agora com seletor de versão e visual composto ou animado no lugar da folha de partes.
- **Versão do corpo**: opção escolhida pelo consultor para aquele card; nesta entrega: “Em espera” (composto parado), “Animado” (ciclo de espera) e “Caminhada” (ciclo de caminhada), cada uma só se o conjunto permitir.
- **Conjunto de corpo**: material de acervo da classe e paleta (partes, mapa de encaixe, ciclo de espera e, quando houver, ciclo de caminhada) necessário para montar e animar o herói.
- **Corpo composto**: apresentação em que as partes estão encaixadas na pose de espera, reconhecível como o herói no jogo.
- **Corpo animado**: a mesma montagem em movimento no ciclo de espera (“Animado”) ou no ciclo de caminhada (“Caminhada”).
- **Ausência de mídia**: estado visível “sem imagem” quando não for possível compor ou animar de forma fiel.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Em uma listagem com pelo menos 5 personagens de classes e aparências variadas e conjunto disponível, um consultor reconhece o herói no espaço “Corpo inteiro” (silhueta montada, não folha de partes) em até 5 segundos por card, sem instrução extra.
- **SC-002**: Em 100% dos cards com conjunto completo, a versão padrão “Em espera” mostra o herói composto na paleta da aparência exibida no card (A–D).
- **SC-003**: Em 100% dos cards com o ciclo correspondente disponível, ao escolher “Animado” ou “Caminhada” o espaço passa a mostrar o movimento contínuo do mesmo herói (espera ou caminhada, conforme a opção) em até 3 segundos, sem recarregar a listagem.
- **SC-004**: Em um teste informal, pelo menos 90% dos consultores conseguem alternar a versão do corpo no primeiro card que tentarem, sem ajuda.
- **SC-005**: Quando o conjunto está ausente ou incompleto, 100% desses cards mostram “sem imagem” no espaço de corpo e continuam exibindo retrato e demais dados.
- **SC-006**: Alterar a versão em um card não muda o corpo de nenhum outro card na mesma listagem (verificado em pelo menos 3 cards simultâneos).
- **SC-007**: Após recarregar a listagem, 100% dos cards com conjunto disponível reaparecem em “Em espera”, mesmo que antes estivessem em “Animado” ou “Caminhada”.
- **SC-008**: Com “Caminhada” ligada, em 100% dos cards observados o herói permanece inteiro dentro do espaço “Corpo inteiro” durante o ciclo; retrato e texto do mesmo card continuam descobertos.

## Assumptions

- O acervo da Feature 004 já contém, por classe e paleta, os arquivos usados no jogo para montar o herói (partes, mapa de encaixe e ciclo de espera); esta feature só os apresenta.
- “Versão” no pedido do usuário significa o modo de apresentação do mesmo herói/aparência (espera composta vs. animado), não a letra de aparência A–D (essa continua sendo a aparência cadastrada).
- Nesta entrega o seletor tem só três versões: “Em espera”, “Animado” (ciclo de espera) e “Caminhada”; ataque e demais ciclos ficam fora de escopo.
- Na “Caminhada” o herói anda no lugar, sempre dentro do espaço “Corpo inteiro”; o deslocamento do ciclo do jogo não tira o personagem da área nem atravessa o restante do card.
- Folha de partes soltas deixa de ser o visual principal do espaço; não precisa permanecer como terceira opção no seletor.
- A escolha de versão é temporária enquanto a listagem permanece aberta; recarregar volta a “Em espera”. Não é gravada no cadastro nem lembrada entre visitas.
- Retrato permanece imagem estática de busto; apenas o espaço “Corpo inteiro” ganha composição, animação e seletor.
- Forma humana do card (incluindo Abominação) permanece a mesma da Feature 010; outras formas e skins fora A–D ficam fora de escopo.
- Não há importação nova nem consumo de wiki em tempo de apresentação (constituição e Features 004/010).
