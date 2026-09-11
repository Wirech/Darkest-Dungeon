# Feature Specification: Criação Completa de Personagem

**Feature Branch**: `008-criacao-personagem-completa`

**Created**: 2026-09-10

**Status**: Draft

**Input**: User description: "a criação de personagem não está funcionando do jeito que eu imaginei. quero que funcione assim: Você seleciona: Nome, classe, nível do herói, nível da arma. nível da armadura e variação da aparência. o sistema então deve criar o personagem, uma entidade que tenha todas suas informações adicionais automaticamente criadas a partir destas escolhas. repare que pra isso, a entidade personagem não está bem configurada, pois faltam os atributos que identificam alguns elementos. faça os ajustes necessários e arrume. o personagem deve ser criado com 4 habilidades de combate e 4 habilidades de acampamento no nível 1 e o resto no nível 0 (bloqueado)."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Criar personagem a partir das escolhas essenciais (Priority: P1)

Como usuário do catálogo, quero informar apenas nome, classe, nível do herói, nível da arma, nível da armadura e aparência para criar um personagem completo sem montar manualmente suas entidades relacionadas.

**Why this priority**: Esse é o fluxo principal e corrige a diferença entre o comportamento esperado e o modelo atual de criação.

**Independent Test**: Escolher uma classe, níveis válidos e uma aparência, criar o personagem e consultar o resultado para confirmar que todos os identificadores e dados derivados foram persistidos.

**Acceptance Scenarios**:

1. **Given** uma classe existente e escolhas válidas, **When** o usuário confirma a criação, **Then** o sistema cria um personagem com nome, classe, nível do herói e aparência selecionados.
2. **Given** uma classe com arma e armadura catalogadas, **When** o usuário escolhe os níveis desses equipamentos, **Then** o personagem referencia a arma e a armadura elegíveis da classe e preserva os níveis escolhidos.
3. **Given** uma escolha de aparência válida, **When** o personagem é criado, **Then** a aparência persistida corresponde à escolha e não volta automaticamente para `A`.
4. **Given** que o personagem foi criado, **When** o usuário consulta seus detalhes, **Then** os dados derivados da classe, arma, armadura, habilidades e aparência estão disponíveis por identificadores estáveis e não dependem de reconstrução manual pelo cliente.

---

### User Story 2 - Inicializar progressão de habilidades (Priority: P1)

Como usuário do catálogo, quero que um personagem recém-criado receba automaticamente suas habilidades iniciais para começar com uma progressão consistente.

**Why this priority**: A regra 4+4 define o estado inicial de jogo e precisa ser aplicada junto da criação, sem permitir personagens parcialmente configurados.

**Independent Test**: Criar um personagem de uma classe com habilidades suficientes e verificar que exatamente quatro habilidades de combate e quatro de acampamento estão no nível 1, enquanto todas as outras habilidades dessas categorias estão no nível 0.

**Acceptance Scenarios**:

1. **Given** uma classe com habilidades de combate e acampamento catalogadas, **When** o personagem é criado, **Then** exatamente quatro habilidades de combate e quatro de acampamento ficam com `NumeroDoNivel = 1`.
2. **Given** que a classe possui mais de quatro habilidades em uma categoria, **When** o personagem é criado, **Then** as habilidades restantes da categoria ficam com `NumeroDoNivel = 0` e permanecem bloqueadas.
3. **Given** que uma categoria possui menos de quatro habilidades válidas para a classe, **When** o usuário tenta criar o personagem, **Then** a operação é rejeitada com uma mensagem clara informando que a classe não atende à configuração inicial obrigatória.
4. **Given** as habilidades inicializadas, **When** o personagem é consultado, **Then** cada associação informa o ID da habilidade, a categoria, o nível e o estado de treinamento de forma consistente.

---

### User Story 3 - Rejeitar escolhas incompatíveis (Priority: P1)

Como usuário do catálogo, quero receber uma mensagem clara quando minhas escolhas não puderem gerar um personagem válido, sem criar um registro incompleto.

**Why this priority**: A validação evita personagens impossíveis de equipar ou com dados de progressão incoerentes.

**Independent Test**: Tentar criar personagens com níveis fora dos limites, aparência inválida, equipamento de classe incompatível ou dados ausentes e confirmar que nenhum registro parcial é persistido.

**Acceptance Scenarios**:

1. **Given** nível de herói menor que 0 ou maior que 6, **When** o usuário confirma a criação, **Then** a operação falha com erro associado ao nível e nenhum personagem é criado.
2. **Given** nível de arma ou armadura menor que 1 ou maior que 5, **When** o usuário confirma a criação, **Then** a operação falha com erro associado ao equipamento.
3. **Given** que arma ou armadura não pertence à classe escolhida, **When** o usuário confirma a criação, **Then** a operação falha informando a incompatibilidade e nenhum personagem é criado.
4. **Given** uma aparência fora de A, B, C ou D, **When** o usuário confirma a criação, **Then** a operação falha com erro de aparência.
5. **Given** uma falha durante a criação de dados relacionados, **When** a operação termina, **Then** o personagem não fica persistido pela metade.

### Edge Cases

- Uma classe pode não possuir exatamente cinco níveis válidos de arma ou armadura; a criação deve falhar sem escolher um nível aproximado silenciosamente.
- Uma classe pode possuir habilidades compartilhadas ou ordenadas de forma não determinística; a seleção das quatro iniciais deve ser determinística e documentada pelo catálogo.
- O cliente pode enviar IDs de habilidades, arma ou armadura para tentar contornar as escolhas; a criação deve derivar e validar os relacionamentos a partir da classe e dos níveis informados.
- A aparência escolhida pode não ter asset cadastrado; a aparência ainda deve ser persistida e o estado de cobertura visual deve permanecer observável.
- Repetir a requisição não deve criar entidades relacionadas duplicadas dentro do mesmo personagem.
- Dados antigos de personagens sem nível de equipamento ou aparência explícitos devem continuar legíveis com valores de compatibilidade documentados.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O sistema MUST permitir criar um personagem informando nome, classe, nível do herói, nível da arma, nível da armadura e aparência.
- **FR-002**: O sistema MUST persistir no personagem a classe escolhida, o nível do herói, a aparência escolhida e identificadores dos equipamentos selecionados.
- **FR-003**: O sistema MUST persistir o nível selecionado da arma e da armadura no contexto do personagem, para que o equipamento atualmente usado seja identificável sem inferência frágil.
- **FR-004**: O sistema MUST localizar a arma e a armadura elegíveis para a classe e os níveis escolhidos antes de persistir o personagem.
- **FR-005**: O sistema MUST derivar as resistências, atributos e demais informações iniciais dependentes da classe conforme os dados oficiais já catalogados.
- **FR-006**: O sistema MUST associar automaticamente ao personagem todas as habilidades de combate e acampamento disponíveis para sua classe, sem exigir que o cliente envie a lista completa.
- **FR-007**: O sistema MUST inicializar exatamente quatro habilidades de combate no nível 1 e exatamente quatro habilidades de acampamento no nível 1 quando a classe possuir quantidade suficiente.
- **FR-008**: O sistema MUST inicializar no nível 0, bloqueado, todas as habilidades de combate e acampamento da classe que não estiverem entre as quatro iniciais de sua categoria.
- **FR-009**: O sistema MUST manter `NumeroDoNivel`, `Treinada` e o estado de bloqueio consistentes em cada habilidade de personagem.
- **FR-010**: O sistema MUST rejeitar a criação quando a classe não possuir pelo menos quatro habilidades de combate e quatro de acampamento válidas.
- **FR-011**: O sistema MUST rejeitar níveis de herói fora de 0 a 6, níveis de arma/armadura fora de 1 a 5 e aparências fora de A, B, C e D.
- **FR-012**: O sistema MUST rejeitar equipamentos inexistentes ou incompatíveis com a classe escolhida.
- **FR-013**: O sistema MUST impedir persistência parcial quando qualquer validação ou criação relacionada falhar.
- **FR-014**: O sistema MUST expor no detalhe do personagem a aparência, os níveis de equipamento e as habilidades com seus níveis e categorias.
- **FR-015**: O contrato de criação MUST deixar de depender de uma lista manual de habilidades para executar a inicialização padrão 4+4.
- **FR-016**: O sistema MUST manter compatibilidade de leitura com personagens existentes que não possuam explicitamente os novos atributos, aplicando defaults documentados sem alterar registros silenciosamente.
- **FR-017**: As mensagens de validação e erro destinadas ao usuário MUST estar em Português do Brasil.

### Key Entities

- **Personagem**: Herói persistido com classe, nível, aparência, IDs de arma/armadura, níveis selecionados dos equipamentos, resistências, estado de progressão e associações de habilidades.
- **Classe de herói**: Catálogo que fornece resistências, habilidades permitidas, arma elegível e armadura elegível.
- **Habilidade de personagem**: Associação entre personagem e habilidade catalogada, incluindo categoria, nível de desbloqueio/treinamento e estado de equipamento.
- **Arma e armadura**: Itens elegíveis por classe, cada um com cinco níveis catalogados; o personagem referencia o item e o nível escolhido.
- **Aparência**: Variação visual A, B, C ou D associada ao personagem e aos assets da classe.
- **Configuração de criação**: Escolhas mínimas fornecidas pelo usuário e usadas para construir automaticamente o estado inicial completo.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% das criações válidas de teste persistem classe, nível do herói, aparência, arma, armadura e níveis escolhidos corretamente.
- **SC-002**: 100% dos personagens criados com classe elegível possuem exatamente 4 habilidades de combate e 4 de acampamento no nível 1.
- **SC-003**: 100% das habilidades restantes de combate e acampamento ficam no nível 0 após a criação inicial.
- **SC-004**: 100% das tentativas com escolha inválida são rejeitadas sem personagem parcial persistido.
- **SC-005**: Um usuário consegue criar um personagem informando somente os seis campos essenciais, sem selecionar manualmente habilidades individuais.
- **SC-006**: Uma consulta posterior ao personagem reproduz todas as escolhas originais e o estado inicial de suas habilidades sem depender do estado da tela de criação.

## Assumptions

- O nível do herói usa a faixa atual do domínio, de 0 a 6, e representa o campo `Nivel` do personagem.
- Os níveis de arma e armadura usam a faixa catalogada de 1 a 5.
- As quatro habilidades iniciais de cada categoria serão escolhidas por uma ordenação determinística do catálogo, preferencialmente a ordem oficial/seed existente; a regra exata de desempate será registrada no plano técnico.
- O personagem receberá todas as habilidades de combate e acampamento da classe; habilidades de inimigo não serão associadas a personagens.
- As resistências e demais atributos derivados da classe continuarão vindo do catálogo oficial existente.
- A criação não incluirá seleção inicial de acessórios, equipamentos de acampamento ou progressão posterior; esses fluxos permanecem separados.
- A leitura de personagens antigos usará aparência `A` e níveis de equipamento compatíveis com os dados disponíveis quando esses campos não existirem, sem reescrever automaticamente o banco.
- A criação deve preservar as regras de arquitetura em camadas, persistência oficial em SQL Server e contratos backend testáveis.