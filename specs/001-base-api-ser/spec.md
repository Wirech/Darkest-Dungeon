# Feature Specification: Base de API e Recurso Ser

**Feature Branch**: `001-base-api-ser`

**Created**: 2026-09-07

**Status**: Draft

**Input**: User description: "crie a arquitetura base do projeto (banco de dados), com .net e EF core com SQL server. crie um endpoint genérico com ID que todos os outros vão herdar. a partir dele, crie um endpoint \"Ser\", que vai ter HP máximo, HP Atual, Velocidade, Crítico, Dano base, Movimento, Bonus de Crítico, Tamanho, Ações por turno, Esquiva, Precisão, Proteção, Nível, Nome, Tipo. Refinamento: Ser é uma entidade genérica para herança futura por Monstro, Personagem, Chefe etc.; Crítico e Bonus de Crítico são valores de 0 a 100; Dano base é um range mínimo/máximo; Tamanho é 0 a 4; Proteção é 0 a 100; Nível é 0 a 6; Ser possui resistências de 0 a 100 para Atordoamento, Sangramento, Envenenamento, Debuff e Movimento. Refinamento de análise: explicitar escalabilidade, facilidade de evolução futura e uso consistente de injeção de dependência por camada; permitir novos tipos derivados de Ser com baixo retrabalho, manter contratos por interface entre camadas e validar que dependências sejam resolvidas pelo contêiner de DI."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Consultar entidade por identificador (Priority: P1)

Como usuário da API, quero consultar qualquer recurso suportado usando um identificador único para obter uma resposta previsível e reutilizável entre endpoints.

**Why this priority**: O contrato por ID é a base que os demais recursos devem seguir, então ele precisa existir antes de recursos específicos.

**Independent Test**: Pode ser testado consultando um recurso existente por ID válido e um ID inexistente, verificando status e formato da resposta.

**Acceptance Scenarios**:

1. **Given** um recurso existente com ID válido, **When** o usuário consulta esse ID, **Then** o sistema retorna o recurso correspondente com o ID informado.
2. **Given** um ID sem recurso correspondente, **When** o usuário consulta esse ID, **Then** o sistema retorna uma resposta em PT-BR informando que o registro não foi encontrado.
3. **Given** um ID inválido, **When** o usuário consulta o recurso, **Then** o sistema retorna uma resposta em PT-BR indicando que o identificador é inválido.

---

### User Story 2 - Cadastrar e consultar Ser (Priority: P2)

Como usuário da API, quero registrar e consultar um Ser com seus atributos de combate para que outras funcionalidades possam reutilizar essa entidade.

**Why this priority**: O recurso Ser é a primeira entidade concreta e valida o padrão genérico de identificação em um caso real do domínio.

**Independent Test**: Pode ser testado criando um Ser com todos os atributos obrigatórios e consultando o mesmo registro pelo ID retornado.

**Acceptance Scenarios**:

1. **Given** dados válidos para todos os atributos obrigatórios de um Ser, **When** o usuário solicita o cadastro, **Then** o sistema cria o Ser e retorna seus dados com um ID único.
2. **Given** um Ser cadastrado, **When** o usuário consulta seu ID, **Then** o sistema retorna Nome, Tipo, Nível, atributos de vida, combate, defesa, turno e resistências.
3. **Given** uma tentativa de cadastro sem Nome ou Tipo, **When** o usuário envia os dados, **Then** o sistema rejeita a solicitação com mensagem em PT-BR.
4. **Given** uma tentativa de cadastro com valores fora das faixas permitidas, **When** o usuário envia os dados, **Then** o sistema rejeita a solicitação indicando os campos inválidos em PT-BR.

---

### User Story 3 - Persistir dados entre usos (Priority: P3)

Como responsável por hospedar a aplicação, quero que os registros criados permaneçam disponíveis após reinícios para permitir uso contínuo por várias pessoas.

**Why this priority**: Persistência é necessária para uso real, mas pode ser validada depois que o contrato base e o recurso Ser estiverem definidos.

**Independent Test**: Pode ser testado criando um Ser, reiniciando a aplicação e consultando o mesmo ID depois do reinício.

**Acceptance Scenarios**:

1. **Given** um Ser cadastrado, **When** o sistema é reiniciado, **Then** o Ser continua disponível pelo mesmo ID.
2. **Given** múltiplos usuários acessando a aplicação pela internet ou VPN, **When** eles consultam registros existentes, **Then** recebem respostas consistentes sem perda de dados.

---

### User Story 4 - Evoluir tipos derivados com baixo retrabalho (Priority: P4)

Como mantenedor do projeto, quero adicionar novos tipos derivados de Ser com mudanças localizadas para que Monstro, Personagem, Chefe e tipos futuros possam evoluir sem reescrever a base da API.

**Why this priority**: A base precisa nascer extensível, mas essa história depende do contrato de ID, do Ser e da persistência já planejados.

**Independent Test**: Pode ser testado por uma verificação arquitetural que confirma contratos por interface entre camadas, resolução das dependências pelo contêiner de DI e ausência de dependências diretas indevidas.

**Acceptance Scenarios**:

1. **Given** um novo tipo derivado de Ser planejado, **When** o mantenedor avalia as mudanças necessárias, **Then** a base permite adicionar o tipo sem alterar o contrato genérico por ID nem duplicar validações comuns de Ser.
2. **Given** os serviços e repositórios registrados, **When** a aplicação inicia, **Then** o contêiner de DI resolve as dependências das camadas sem erro.
3. **Given** uma dependência entre camadas, **When** a arquitetura é validada, **Then** camadas superiores dependem de interfaces e camadas inferiores não referenciam a API.

---

### Edge Cases

- O sistema deve rejeitar valores numéricos negativos para atributos que representam vida, dano, movimento, tamanho, ações por turno, esquiva, precisão, proteção, nível ou resistências.
- HP Atual não pode ser maior que HP máximo.
- Dano base mínimo não pode ser maior que Dano base máximo.
- Crítico, Bonus de Crítico, Proteção e cada resistência não podem ser menores que 0 nem maiores que 100.
- Tamanho não pode ser menor que 0 nem maior que 4.
- Nível não pode ser menor que 0 nem maior que 6.
- Nome e Tipo não podem estar vazios ou conter apenas espaços.
- IDs inexistentes devem retornar uma resposta padronizada em PT-BR, sem expor detalhes internos.
- Falhas de persistência devem gerar resposta de erro controlada em PT-BR e não podem criar registros parciais.
- Falhas de configuração de dependências devem ser detectadas na inicialização ou nos testes, antes de qualquer uso real da API.
- Um novo tipo derivado de Ser não pode exigir duplicação das validações comuns de Ser.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O sistema MUST fornecer um contrato padronizado para recursos identificáveis por ID único.
- **FR-002**: O sistema MUST permitir consultar um recurso identificável por ID e retornar exatamente o recurso correspondente quando ele existir.
- **FR-003**: O sistema MUST retornar uma resposta padronizada em PT-BR quando um ID for inválido ou inexistente.
- **FR-004**: O sistema MUST permitir cadastrar um Ser com os atributos Nome, Tipo, HP máximo, HP Atual, Velocidade, Crítico, Dano base mínimo, Dano base máximo, Movimento, Bonus de Crítico, Tamanho, Ações por turno, Esquiva, Precisão, Proteção, Nível e resistências.
- **FR-005**: O sistema MUST atribuir um ID único a cada Ser criado.
- **FR-006**: O sistema MUST permitir consultar um Ser pelo seu ID.
- **FR-007**: O sistema MUST validar que HP Atual seja menor ou igual ao HP máximo.
- **FR-008**: O sistema MUST validar que atributos numéricos de Ser não aceitem valores negativos.
- **FR-009**: O sistema MUST validar que Nome e Tipo sejam informados com conteúdo visível.
- **FR-010**: O sistema MUST persistir os registros de Ser para que continuem disponíveis após reinícios.
- **FR-011**: O sistema MUST retornar respostas de sucesso e erro em formato consistente entre o contrato genérico por ID e o endpoint de Ser.
- **FR-012**: O sistema MUST suportar acesso simultâneo de múltiplos usuários via internet ou VPN sem duplicar IDs nem perder registros.
- **FR-013**: O sistema MUST manter todas as mensagens destinadas ao usuário em PT-BR.
- **FR-014**: O sistema MUST tratar Ser como entidade genérica herdável por tipos futuros como Monstro, Personagem e Chefe.
- **FR-015**: O sistema MUST validar que Crítico e Bonus de Crítico sejam valores entre 0 e 100.
- **FR-016**: O sistema MUST validar que Dano base mínimo seja menor ou igual ao Dano base máximo.
- **FR-017**: O sistema MUST validar que Tamanho seja um valor entre 0 e 4.
- **FR-018**: O sistema MUST validar que Proteção seja um valor entre 0 e 100.
- **FR-019**: O sistema MUST validar que Nível seja um valor entre 0 e 6.
- **FR-020**: O sistema MUST registrar resistências de Atordoamento, Sangramento, Envenenamento, Debuff e Movimento para cada Ser.
- **FR-021**: O sistema MUST validar que cada resistência seja um valor entre 0 e 100.
- **FR-022**: O sistema MUST permitir adicionar tipos derivados de Ser com mudanças localizadas, sem alterar o contrato genérico de consulta por ID.
- **FR-023**: O sistema MUST manter contratos por interface entre as camadas de aplicação, domínio e infraestrutura para reduzir acoplamento.
- **FR-024**: O sistema MUST registrar dependências por camada em um contêiner de injeção de dependência e validar que os serviços principais sejam resolvidos com sucesso.
- **FR-025**: O sistema MUST impedir dependências diretas da camada de domínio para API ou infraestrutura.
- **FR-026**: O sistema MUST centralizar validações comuns de Ser para que tipos derivados reutilizem as mesmas regras sem duplicação.

### Key Entities *(include if feature involves data)*

- **Recurso Identificável**: Conceito base para registros que possuem ID único e podem ser consultados por esse ID. Serve como contrato comum para recursos atuais e futuros.
- **Ser**: Entidade genérica de domínio que representa uma criatura, personagem ou unidade base. Deve permitir especializações futuras como Monstro, Personagem e Chefe.
  - Identificação: ID, Nome, Tipo.
  - Vida: HP máximo, HP Atual.
  - Combate: Velocidade, Crítico de 0 a 100, Dano base mínimo, Dano base máximo, Bonus de Crítico de 0 a 100, Precisão.
  - Movimento e turno: Movimento, Tamanho, Ações por turno.
  - Defesa e progressão: Esquiva, Proteção de 0 a 100, Nível de 0 a 6.
  - Resistências: Atordoamento, Sangramento, Envenenamento, Debuff e Movimento, todas de 0 a 100.
- **Tipo Derivado de Ser**: Especialização futura de Ser, como Monstro, Personagem ou Chefe, que deve reutilizar ID, atributos base, resistências e validações comuns.
- **Contrato por Interface**: Acordo entre camadas que define operações esperadas sem acoplar a camada consumidora a detalhes de implementação.
- **Registro de Dependência**: Mapeamento de contratos por interface para suas implementações, validado na inicialização ou nos testes de arquitetura.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% das consultas por ID válido retornam o registro correspondente com o mesmo ID solicitado.
- **SC-002**: 100% das consultas por ID inválido ou inexistente retornam uma resposta controlada em PT-BR, sem detalhes internos do sistema.
- **SC-003**: 100% das tentativas de cadastro de Ser com dados válidos criam um registro consultável pelo ID retornado.
- **SC-004**: 100% das tentativas de cadastro de Ser com HP Atual maior que HP máximo, range de dano inválido, valores fora das faixas permitidas ou atributos numéricos negativos são rejeitadas com mensagem em PT-BR.
- **SC-005**: Registros criados permanecem disponíveis após reinício da aplicação em 100% dos cenários de validação previstos.
- **SC-006**: Pelo menos 20 usuários simultâneos conseguem consultar registros existentes sem respostas inconsistentes ou duplicação de IDs.
- **SC-007**: 100% dos serviços principais definidos para a feature são resolvidos pelo contêiner de DI durante a validação de inicialização.
- **SC-008**: A validação arquitetural confirma 0 dependências diretas do domínio para API ou infraestrutura.
- **SC-009**: Um novo tipo derivado de Ser pode reutilizar o contrato por ID e as validações comuns sem duplicar as regras de HP, dano, limites e resistências.

## Assumptions

- Esta especificação define uma única funcionalidade inicial: contrato genérico por ID, recurso Ser herdável e persistência mínima para uso remoto.
- Monstro, Personagem e Chefe são exemplos de especializações futuras e não fazem parte da implementação deste escopo inicial.
- O escopo inicial deve preparar a base para tipos derivados, mas não precisa implementar endpoints específicos de Monstro, Personagem ou Chefe.
- A persistência oficial do projeto é SQL Server, conforme definido na constituição.
- Decisões de estrutura de solução, bibliotecas e implementação serão detalhadas em `/speckit-plan`; o plano deve respeitar as preferências técnicas descritas na entrada original e a constituição do projeto.
- O recurso Ser precisa inicialmente de cadastro e consulta por ID; atualização, remoção, listagem e filtros ficam fora deste escopo.
- Segurança avançada não é foco desta funcionalidade, mas validação de entrada, mensagens controladas e proteção de segredos continuam obrigatórias pela constituição.
- O primeiro uso previsto é caseiro, com acesso de múltiplas pessoas pela internet ou VPN em ambiente hospedado pelo responsável pelo projeto.