# Feature Specification: Entidades e Catálogo de Habilidades

**Feature Branch**: `002-entidades-habilidades`

**Created**: 2026-09-07

**Status**: Draft

**Input**: User description: "agora, crie a partir da entidade Ser, as entidades Inimigo, Personagem e Item. eu quero uma forma de listar todas as habilidades que existem. Um personagem tem até no máximo 6 habilidades (definido por sua classe). dessa forma quero manter uma lista dessas habilidades por classe e ao criar uma entidade personagem com aquela classe automaticamente seria atribuido essas habilidades. habilidades podem ser de Combate e de Acampamento. um personagem tem 6 de cada. inimigos também tem suas habilidades, então nem todas as habilidades são acessíveis para os jogadores (personagem). as classes de personagem são: Abominação, Antiquário, Besteiro, Caçador de Recompensas, Cruzado, Ladrão de cova, Bobo da corte, Mestre de caça, Leproso, Infernal, Bandido, Musqueteiro, Veterano, Ocultista, Médico da Peste, Vestal, Flagelante, Rompedor, Duelista, Fugitivo. enquanto isso, inimigos podem ser: Humano, Besta, Profano, Sobrenatural, Vampírico, Casca, Rochoso e ter um número arbitrário de habilidades."

## Clarifications

### Session 2026-09-07

- Q: As seis habilidades de Combate de um Personagem devem ser sempre atribuídas pela classe, ocupando seis posições fixas com uma flag `habilitada`, ou o personagem pode possuir menos de seis habilidades atribuídas? → A: O padrão é até seis habilidades de Combate atribuídas pela classe, cada uma podendo estar habilitada ou desabilitada; casos raros podem permitir mais de seis.
- Q: As seis habilidades de Acampamento atribuídas pela classe devem começar não treinadas e não equipadas, exigindo ações separadas para treinar e escolher até três para levar à aventura? → A: As seis são atribuídas pela classe, começam não treinadas e não equipadas; após treinar, até três podem ser equipadas para a aventura.
- Q: Quando o Personagem chegar a 100 de Stress, ele deve receber uma Aflição ou uma Virtude, mas nunca as duas ao mesmo tempo? → A: Aflição e Virtude são mutuamente exclusivas e persistentes; apenas uma pode existir por vez e não é removida automaticamente quando o Stress diminui.
- Q: A chance de Virtude deve ser um valor percentual inteiro entre 0 e 100? → A: A chance de Virtude é um percentual inteiro entre 0 e 100.
- Q: As resistências finais de um Inimigo devem permanecer limitadas ao intervalo de 0 a 100 após somar a resistência base do tipo e os modificadores próprios? → A: Somente as resistências próprias do Inimigo são limitadas entre 0 e 100; o total pode superar 100 após somar a base do tipo.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Consultar o catálogo de habilidades (Priority: P1)

Como usuário do sistema, quero listar todas as habilidades cadastradas e identificar se são de Combate ou de Acampamento, para conhecer as opções disponíveis no jogo.

**Why this priority**: O catálogo é a fonte comum usada por personagens e inimigos e precisa existir antes das associações específicas.

**Independent Test**: Pode ser testado cadastrando habilidades das duas categorias e consultando a listagem, verificando que todas aparecem com sua categoria e que filtros válidos restringem o resultado corretamente.

**Acceptance Scenarios**:

1. **Given** habilidades de Combate e de Acampamento cadastradas, **When** o usuário solicita a listagem completa, **Then** o sistema retorna todas as habilidades com nome, descrição e categoria.
2. **Given** habilidades cadastradas nas duas categorias, **When** o usuário filtra por uma categoria válida, **Then** o sistema retorna somente habilidades daquela categoria.
3. **Given** uma tentativa de criar uma habilidade sem nome ou sem categoria válida, **When** o usuário envia os dados, **Then** o sistema rejeita a operação com uma mensagem em PT-BR.

---

### User Story 2 - Configurar habilidades por classe de personagem (Priority: P2)

Como responsável pelo conteúdo do jogo, quero manter a lista de habilidades de cada classe para definir quais opções um personagem daquela classe pode usar.

**Why this priority**: A configuração por classe transforma o catálogo geral em regras jogáveis para personagens.

**Independent Test**: Pode ser testado associando habilidades a uma classe, consultando sua configuração e tentando ultrapassar o limite de seis habilidades em cada categoria.

**Acceptance Scenarios**:

1. **Given** uma classe de personagem existente e habilidades válidas, **When** o responsável associa até seis habilidades de Combate e até seis de Acampamento, **Then** a configuração da classe é persistida e pode ser consultada.
2. **Given** uma configuração de classe com seis habilidades de uma categoria, **When** o responsável tenta adicionar uma sétima habilidade dessa categoria, **Then** o sistema rejeita a associação e mantém a configuração anterior.
3. **Given** uma habilidade exclusiva de inimigo, **When** o responsável tenta associá-la a uma classe de personagem, **Then** o sistema rejeita a associação porque ela não é acessível a personagens.

---

### User Story 3 - Criar personagem com habilidades da classe (Priority: P3)

Como usuário do sistema, quero criar um Personagem informando sua classe e receber automaticamente as habilidades configuradas para ela.

**Why this priority**: A atribuição automática reduz configuração manual e garante que personagens respeitem as regras da classe.

**Independent Test**: Pode ser testado criando um Personagem para cada uma das classes suportadas e comparando suas habilidades com a configuração vigente da classe.

**Acceptance Scenarios**:

1. **Given** uma classe com sua configuração de habilidades, **When** o usuário cria um Personagem daquela classe, **Then** o Personagem recebe automaticamente as habilidades de Combate e de Acampamento configuradas para a classe.
2. **Given** uma classe sem configuração válida ou inexistente, **When** o usuário tenta criar um Personagem, **Then** o sistema rejeita a criação e informa que a classe não está disponível para uso.
3. **Given** um Personagem criado, **When** o usuário consulta seus dados, **Then** o retorno identifica a classe, os dados herdados de Ser e as habilidades atribuídas, separadas por categoria.

---

### User Story 4 - Criar inimigo com habilidades próprias (Priority: P4)

Como responsável pelo conteúdo do jogo, quero criar um Inimigo a partir de Ser e atribuir a ele qualquer quantidade de habilidades válidas, mesmo que essas habilidades não estejam disponíveis para Personagem.

**Why this priority**: Inimigos precisam compartilhar a base de Ser, mas não devem ficar limitados às regras de classes jogáveis.

**Independent Test**: Pode ser testado criando inimigos de cada tipo suportado com quantidades diferentes de habilidades e confirmando que suas habilidades não são expostas como opções de personagem.

**Acceptance Scenarios**:

1. **Given** um tipo de inimigo válido e habilidades cadastradas, **When** o usuário cria um Inimigo com qualquer quantidade não negativa de habilidades, **Then** o sistema persiste o Inimigo e suas habilidades.
2. **Given** um Inimigo com habilidades de Combate e de Acampamento, **When** o usuário consulta seus dados, **Then** o sistema retorna todas as habilidades atribuídas e suas categorias.
3. **Given** um Inimigo com uma habilidade não acessível a Personagem, **When** o usuário consulta a configuração de uma classe, **Then** essa habilidade não aparece como opção daquela classe.

---

### User Story 5 - Criar e consultar Item (Priority: P5)

Como usuário do sistema, quero cadastrar e consultar um Item a partir do contrato identificável existente, para que itens possam ser usados por funcionalidades futuras sem depender de habilidades.

**Why this priority**: Item faz parte da expansão solicitada a partir de Ser, mas não controla o catálogo de habilidades e pode ser validado de forma independente.

**Independent Test**: Pode ser testado criando um Item com seus dados obrigatórios, consultando-o pelo identificador e rejeitando dados sem nome.

**Acceptance Scenarios**:

1. **Given** dados válidos de um Item, **When** o usuário solicita sua criação, **Then** o sistema cria o Item com identificador único e dados consultáveis.
2. **Given** um Item existente, **When** o usuário consulta seu identificador, **Then** o sistema retorna seus dados sem incluir habilidades de personagem ou inimigo indevidamente.
3. **Given** uma tentativa de criar um Item sem nome, **When** o usuário envia os dados, **Then** o sistema rejeita a operação com uma mensagem em PT-BR.

### Edge Cases

- O sistema deve rejeitar habilidade duplicada na mesma configuração de classe, personagem ou inimigo.
- O sistema deve rejeitar nomes vazios ou compostos apenas por espaços para habilidade, classe, inimigo, personagem e item.
- Stress deve aceitar somente valores entre 0 e 200.
- Chance de Virtude deve aceitar somente valores inteiros entre 0 e 100.
- Resistências próprias de Inimigo devem aceitar valores entre 0 e 100; o total calculado pode superar 100 após a soma com a resistência base do tipo.
- Inventário deve possuir quatro espaços por padrão e aceitar somente Item ou entidades derivadas de Item.
- Uma habilidade de Combate atribuída a Personagem deve possuir estado habilitada ou desabilitada.
- Uma habilidade de Acampamento deve possuir estados independentes de treinada e equipada; somente treinadas podem ser equipadas e no máximo três podem estar equipadas para uma aventura.
- Aflição e Virtude não podem estar preenchidas simultaneamente e não podem ser removidas automaticamente apenas porque o Stress diminuiu.
- Portas da morte, Recuperou portas da morte e Recuperou de um Ataque Cardíaco devem ser flags independentes e persistidas.
- Categoria de habilidade deve aceitar somente Combate ou Acampamento.
- Uma classe de personagem deve possuir no máximo seis habilidades de Combate e seis de Acampamento na configuração padrão; exceções raras de Personagem podem ultrapassar seis habilidades de Combate mediante regra explícita.
- As seis habilidades de Acampamento atribuídas a um Personagem começam não treinadas e não equipadas; somente habilidades treinadas podem ser equipadas, com limite de três equipadas para a aventura.
- Aflição e Virtude são estados mutuamente exclusivos e persistentes; reduzir o Stress não os remove automaticamente.
- Um Personagem não pode receber habilidades de uma classe diferente da sua configuração atual.
- A alteração posterior da configuração de uma classe não deve atribuir ou remover silenciosamente habilidades de personagens já criados.
- Um Inimigo pode ter zero habilidades ou qualquer quantidade finita de habilidades válidas, sem usar o limite de seis dos personagens.
- Tipos de Inimigo devem aceitar somente Humano, Besta, Profano, Sobrenatural, Vampírico, Casca ou Rochoso.
- Classes de Personagem devem aceitar somente Abominação, Antiquário, Besteiro, Caçador de Recompensas, Cruzado, Ladrão de cova, Bobo da corte, Mestre de caça, Leproso, Infernal, Bandido, Musqueteiro, Veterano, Ocultista, Médico da Peste, Vestal, Flagelante, Rompedor, Duelista ou Fugitivo.
- Uma habilidade pode ser acessível a personagens, exclusiva de inimigos ou compartilhada, e essa disponibilidade deve ser respeitada nas associações.
- Criação de Personagem, Inimigo ou Item deve falhar sem registro parcial quando a entidade Ser ou qualquer associação de habilidade for inválida.
- Entidades derivadas devem manter os atributos e validações comuns de Ser sem duplicação de regras.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O sistema MUST permitir criar, consultar e listar habilidades cadastradas.
- **FR-002**: O sistema MUST registrar cada habilidade com nome, descrição, categoria e disponibilidade para Personagem, Inimigo ou ambos.
- **FR-003**: O sistema MUST permitir filtrar a listagem de habilidades por categoria Combate ou Acampamento e por disponibilidade.
- **FR-004**: O sistema MUST rejeitar habilidades sem nome, com categoria fora das duas categorias suportadas ou com disponibilidade inválida.
- **FR-005**: O sistema MUST disponibilizar as classes de Personagem exatamente como catálogo controlado, incluindo as 20 classes informadas nesta especificação.
- **FR-006**: O sistema MUST permitir manter uma configuração de habilidades para cada classe de Personagem.
- **FR-007**: O sistema MUST limitar a configuração padrão de cada classe a no máximo seis habilidades de Combate e seis de Acampamento, mantendo os limites independentes por categoria.
- **FR-008**: O sistema MUST impedir habilidades duplicadas na configuração de uma classe.
- **FR-009**: O sistema MUST impedir que habilidades não acessíveis a Personagem sejam associadas a uma classe de Personagem.
- **FR-010**: O sistema MUST permitir criar um Personagem a partir de Ser informando uma classe válida.
- **FR-011**: O sistema MUST atribuir automaticamente ao Personagem as habilidades configuradas para sua classe no momento da criação.
- **FR-012**: O sistema MUST manter separadas as habilidades de Combate e de Acampamento atribuídas ao Personagem.
- **FR-012a**: O sistema MUST permitir que cada habilidade de Combate atribuída ao Personagem tenha seu estado de habilitada ou desabilitada.
- **FR-012b**: O sistema MUST permitir exceções explícitas para que Personagens específicos possuam mais de seis habilidades de Combate, sem alterar o limite padrão das classes.
- **FR-012c**: O sistema MUST atribuir ao Personagem as seis habilidades de Acampamento configuradas para sua classe com estados independentes de treinada e equipada, iniciados como falsos.
- **FR-012d**: O sistema MUST impedir que uma habilidade de Acampamento não treinada seja equipada e MUST limitar a três habilidades de Acampamento equipadas para uma aventura.
- **FR-012e**: O sistema MUST registrar a chance de Virtude como atributo próprio do Personagem.
- **FR-012f**: O sistema MUST permitir no máximo uma Aflição ou uma Virtude ativa por Personagem, mantendo o estado até uma regra explícita de alteração ou cura.
- **FR-012g**: O sistema MUST registrar Stress entre 0 e 200 e rejeitar valores fora dessa faixa.
- **FR-012h**: O sistema MUST registrar Chance de Virtude como número inteiro entre 0 e 100.
- **FR-012i**: O sistema MUST registrar Armadura, Arma e Acessório como equipamentos baseados em Item, permitindo que seus atributos sejam obtidos das entidades equipadas.
- **FR-012j**: O sistema MUST manter Inventário com quatro espaços padrão, aceitando Item ou seus herdeiros e rejeitando objetos de outros tipos.
- **FR-012k**: O sistema MUST registrar Individualidade como lista de características positivas ou negativas e Doenças como lista de doenças do Personagem.
- **FR-012l**: O sistema MUST registrar as flags Portas da morte, Recuperou portas da morte e Recuperou de um Ataque Cardíaco.
- **FR-012m**: O sistema MUST permitir que Aflição e Virtude sejam nulas ou contenham uma característica correspondente, sem permitir ambas simultaneamente.
- **FR-013**: O sistema MUST impedir que um Personagem seja criado sem uma configuração válida de classe.
- **FR-014**: O sistema MUST disponibilizar os tipos de Inimigo exatamente como catálogo controlado: Humano, Besta, Profano, Sobrenatural, Vampírico, Casca e Rochoso.
- **FR-015**: O sistema MUST permitir criar um Inimigo a partir de Ser com zero ou mais habilidades válidas, sem aplicar o limite de seis das classes de Personagem.
- **FR-016**: O sistema MUST permitir que um Inimigo possua habilidades exclusivas de inimigos e impedir que elas sejam oferecidas a Personagens.
- **FR-016a**: O sistema MUST calcular cada resistência efetiva de Inimigo pela soma da resistência base de seu Tipo de Inimigo com sua resistência própria.
- **FR-016b**: O sistema MUST validar a resistência própria de Inimigo entre 0 e 100 e MUST permitir que a resistência efetiva total ultrapasse 100.
- **FR-017**: O sistema MUST impedir habilidades duplicadas na lista de um Inimigo.
- **FR-018**: O sistema MUST permitir criar e consultar um Item com identificador único, nome e os demais dados de item definidos no contrato de domínio.
- **FR-019**: O sistema MUST rejeitar Item sem nome ou com dados obrigatórios inválidos.
- **FR-020**: Personagem e Inimigo MUST reutilizar o identificador, os atributos comuns e as validações de Ser sem alterar o contrato base existente.
- **FR-021**: O sistema MUST persistir Personagem, Inimigo, Item, Habilidade, configurações de classe e associações de habilidades sem registros parciais.
- **FR-022**: O sistema MUST retornar respostas de sucesso e erro em PT-BR e formato consistente com os recursos já existentes.
- **FR-023**: O sistema MUST manter contratos por interface entre as camadas e resolver suas dependências pelo contêiner de injeção de dependência.
- **FR-024**: O sistema MUST manter a camada de domínio independente de detalhes de API e persistência, permitindo novos tipos de Ser com mudanças localizadas.

### Key Entities

- **Habilidade**: Ação reutilizável do jogo, com nome, descrição, categoria e indicação de disponibilidade para Personagem, Inimigo ou ambos.
- **Configuração de Habilidades da Classe**: Conjunto de habilidades permitidas para uma classe de Personagem, separado em Combate e Acampamento e limitado a seis em cada categoria.
- **Personagem**: Entidade derivada de Ser, com classe, Stress de 0 a 200, Chance de Virtude de 0 a 100, habilidades de Combate com estado habilitada, habilidades de Acampamento com estados treinada e equipada, Armadura, Arma, Acessório, Inventário padrão de quatro espaços, Dinheiro, Individualidade, Doenças, flags de Portas da morte e recuperação, e Aflição ou Virtude persistente.
- **Inimigo**: Entidade derivada de Ser, com um tipo controlado, quantidade arbitrária de habilidades válidas e resistências compostas pela base do tipo mais modificadores próprios do inimigo.
- **Tipo de Inimigo**: Catálogo que fornece resistências base; nesta fase, todos os tipos possuem valor base 5 para cada resistência aplicável.
- **Item**: Entidade identificável criada a partir da base do domínio para representar um objeto do jogo; seus dados específicos serão definidos pelo contrato de domínio durante o planejamento.
- **Classe de Personagem**: Catálogo fechado com Abominação, Antiquário, Besteiro, Caçador de Recompensas, Cruzado, Ladrão de cova, Bobo da corte, Mestre de caça, Leproso, Infernal, Bandido, Musqueteiro, Veterano, Ocultista, Médico da Peste, Vestal, Flagelante, Rompedor, Duelista e Fugitivo.
- **Tipo de Inimigo**: Catálogo fechado com Humano, Besta, Profano, Sobrenatural, Vampírico, Casca e Rochoso.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% das habilidades válidas cadastradas aparecem na listagem completa com sua categoria e disponibilidade.
- **SC-002**: 100% das consultas filtradas por categoria retornam somente habilidades da categoria solicitada.
- **SC-003**: 100% das configurações válidas de classe respeitam no máximo seis habilidades de Combate e seis de Acampamento, sem duplicidades.
- **SC-004**: 100% dos Personagens criados com classe válida recebem exatamente a configuração vigente da classe no momento da criação, separada por categoria.
- **SC-005**: 100% das tentativas de associar habilidades exclusivas de Inimigo a uma classe de Personagem são rejeitadas sem alterar a configuração existente.
- **SC-006**: 100% dos Inimigos válidos podem ser criados com zero ou mais habilidades, sem rejeição causada pelo limite de Personagem.
- **SC-007**: 100% das combinações inválidas de classe, tipo de Inimigo, categoria, nome, duplicidade ou associação são rejeitadas com mensagem em PT-BR.
- **SC-008**: 100% dos registros válidos de Personagem, Inimigo, Item, Habilidade e configurações permanecem consultáveis após reinício da aplicação.
- **SC-009**: 100% das dependências dos serviços da feature são resolvidas na validação de inicialização, sem dependência direta do domínio para API ou infraestrutura.
- **SC-010**: A criação e consulta de cada entidade da feature reutiliza as regras comuns de Ser, sem duplicar validações comuns em Personagem ou Inimigo.

## Assumptions

- A expressão "um personagem tem 6 de cada" significa até seis habilidades de Combate e até seis habilidades de Acampamento por Personagem.
- O padrão de Personagem usa até seis habilidades de Combate atribuídas pela classe, cada uma com estado de habilitada ou desabilitada; exceções acima de seis são raras e precisam ser explicitamente autorizadas.
- As seis habilidades de Acampamento da classe são atribuídas inicialmente não treinadas e não equipadas; o Personagem pode treinar habilidades e levar no máximo três treinadas para a aventura.
- As listas de habilidades de cada classe não foram fornecidas; esta feature cria o catálogo e a manutenção dessas associações, sem inventar uma lista inicial de habilidades por classe.
- Habilidades não acessíveis a Personagens podem ser usadas por Inimigos e podem ser compartilhadas com Personagens quando o cadastro indicar essa disponibilidade.
- As habilidades atribuídas ao Personagem são uma fotografia da configuração da classe no momento da criação; mudanças futuras na classe não alteram personagens existentes automaticamente.
- O escopo inclui cadastro, consulta, listagem e associações necessárias para validar os fluxos; atualização e remoção detalhadas serão definidas no plano conforme o contrato existente.
- Personagem e Inimigo reutilizam todos os atributos comuns de Ser e acrescentam somente os dados específicos descritos nesta especificação.
- A chance de Virtude é um percentual inteiro de 0 a 100.
- Armadura, Arma e Acessório serão referências a Item ou herdeiros de Item; seus atributos efetivos serão definidos quando o contrato de Item for detalhado.
- Todas as resistências base de Humano, Besta, Profano, Sobrenatural, Vampírico, Casca e Rochoso começam em 5; modificadores próprios do Inimigo podem ser adicionados ao valor base.
- Os dados específicos de Item ainda não foram detalhados; o plano deve propor o menor contrato de Item que permita sua criação e evolução sem bloquear esta feature.
- Persistência, contratos de endpoint, camadas e testes devem seguir a arquitetura e as regras já estabelecidas para Ser e a constituição do projeto.