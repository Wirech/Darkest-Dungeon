# Feature Specification: Frontend Interativo de Personagens

**Feature Branch**: `007-frontend-personagens`

**Created**: 2026-09-10

**Status**: Draft

**Input**: User description: "crie um frontend em que eu consiga ver os personagens cadastrados no banco, criar e excluir personagens de forma interativa."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Consultar personagens cadastrados (Priority: P1)

Como usuário do catálogo, quero visualizar os personagens cadastrados para compreender rapidamente quais heróis existem e acessar seus dados principais.

**Why this priority**: A consulta é o fluxo central e entrega valor mesmo antes de qualquer novo cadastro ou exclusão.

**Independent Test**: Com personagens cadastrados e sem personagens cadastrados, abrir a tela e confirmar respectivamente a exibição dos registros e do estado vazio apropriado.

**Acceptance Scenarios**:

1. **Given** que existem personagens cadastrados, **When** o usuário abre a tela de personagens, **Then** os personagens são exibidos em uma lista identificável com nome, classe, HP atual/máximo, stress, nível e habilidades.
2. **Given** que não existem personagens cadastrados, **When** o usuário abre a tela, **Then** a interface informa que não há personagens e oferece uma ação clara para criar o primeiro.
3. **Given** que a consulta está em andamento, **When** o usuário visualiza a tela, **Then** a interface apresenta um estado de carregamento sem sugerir que a lista está vazia.
4. **Given** que a consulta falha, **When** o usuário visualiza a tela, **Then** a interface apresenta uma mensagem em PT-BR e permite tentar novamente.

---

### User Story 2 - Criar um personagem (Priority: P1)

Como usuário do catálogo, quero preencher um formulário de personagem e salvá-lo para que ele apareça imediatamente na lista.

**Why this priority**: O cadastro torna a tela operacional e permite alimentar o catálogo sem intervenção manual no banco.

**Independent Test**: Preencher todos os campos obrigatórios com dados válidos, salvar e confirmar a criação do personagem na lista; repetir com dados inválidos e confirmar as mensagens de validação.

**Acceptance Scenarios**:

1. **Given** que o usuário está na tela de personagens, **When** seleciona a ação de criar, **Then** o frontend apresenta um formulário com nome, classe, atributos básicos, stress, chance de virtude e habilidades selecionáveis.
2. **Given** que o formulário contém dados válidos, **When** o usuário confirma o cadastro, **Then** o personagem é persistido e aparece na lista sem exigir recarregamento manual da página.
3. **Given** que o formulário contém campos obrigatórios ausentes ou valores inválidos, **When** o usuário tenta salvar, **Then** o cadastro não é enviado e cada problema é indicado em PT-BR junto ao campo correspondente.
4. **Given** que o salvamento falha, **When** o usuário recebe a resposta, **Then** a interface preserva os dados preenchidos, informa o problema e permite corrigir ou tentar novamente.
5. **Given** que o usuário iniciou o formulário, **When** cancela a operação, **Then** o frontend retorna à lista sem criar um personagem.

---

### User Story 3 - Excluir um personagem (Priority: P2)

Como usuário do catálogo, quero excluir um personagem que não deve mais existir para manter a lista correta.

**Why this priority**: A exclusão é necessária para manutenção do catálogo, mas não impede a consulta e o cadastro inicial.

**Independent Test**: Selecionar um personagem, confirmar a exclusão e verificar que ele desaparece da lista; cancelar a confirmação e verificar que ele permanece.

**Acceptance Scenarios**:

1. **Given** que um personagem está listado, **When** o usuário escolhe excluir, **Then** o frontend solicita confirmação explícita antes de qualquer alteração.
2. **Given** que a confirmação de exclusão está aberta, **When** o usuário cancela, **Then** o personagem permanece cadastrado e a lista não muda.
3. **Given** que o usuário confirma a exclusão, **When** a operação é concluída, **Then** o personagem deixa de aparecer na lista e a interface informa o sucesso.
4. **Given** que a exclusão falha, **When** a resposta é recebida, **Then** o personagem permanece visível e a interface informa o erro sem apresentar sucesso falso.

### Edge Cases

- A lista pode estar vazia após a exclusão do último personagem; a interface deve retornar ao estado vazio com ação de criação.
- Dois usuários podem alterar o catálogo entre a consulta e uma exclusão; uma exclusão de registro inexistente deve ser tratada como erro informativo e não como sucesso silencioso.
- O nome pode conter acentos, espaços no início/fim e caracteres especiais aceitos pelo domínio; a apresentação deve preservar o texto válido sem quebrar o layout.
- Uma classe ou habilidade pode não estar disponível para seleção; o formulário deve informar a indisponibilidade e impedir o envio de uma referência inválida.
- O usuário pode submeter o formulário mais de uma vez rapidamente; a interface deve impedir duplicidade de envio enquanto a operação estiver em andamento.
- A tela deve permanecer utilizável em viewport estreita, sem sobreposição de campos, ações ou mensagens.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O frontend MUST oferecer uma tela dedicada para consultar personagens cadastrados.
- **FR-002**: O frontend MUST exibir, para cada personagem listado, no mínimo nome, classe, HP atual e máximo, stress, nível e habilidades associadas.
- **FR-003**: O frontend MUST diferenciar visualmente os estados de carregamento, lista preenchida, lista vazia e erro de consulta.
- **FR-004**: O frontend MUST oferecer uma ação visível para iniciar o cadastro de um personagem tanto na lista quanto no estado vazio.
- **FR-005**: O formulário MUST permitir informar todos os dados necessários para criar um personagem compatível com o catálogo atual, incluindo nome, classe, atributos de combate, stress, chance de virtude e habilidades.
- **FR-006**: O frontend MUST carregar e apresentar apenas classes e habilidades válidas para seleção no cadastro.
- **FR-007**: O frontend MUST validar campos obrigatórios e valores fora dos limites antes do envio e apresentar mensagens em PT-BR associadas aos campos.
- **FR-008**: Após uma criação bem-sucedida, o frontend MUST atualizar a lista e indicar qual personagem foi criado.
- **FR-009**: O frontend MUST oferecer uma ação de exclusão para cada personagem listado.
- **FR-010**: O frontend MUST exigir confirmação explícita antes de excluir um personagem.
- **FR-011**: Após uma exclusão bem-sucedida, o frontend MUST remover o personagem da lista e apresentar confirmação da operação.
- **FR-012**: Em falhas de consulta, criação ou exclusão, o frontend MUST apresentar uma mensagem compreensível em PT-BR, preservar o contexto necessário e permitir nova tentativa quando aplicável.
- **FR-013**: O frontend MUST impedir envios duplicados enquanto uma criação ou exclusão estiver em andamento.
- **FR-014**: O frontend MUST manter as ações e os dados legíveis e utilizáveis em telas estreitas e largas, sem sobreposição ou perda de conteúdo.
- **FR-015**: A exclusão MUST respeitar a resposta do serviço de personagens e não apresentar sucesso quando o registro não tiver sido removido.

### Key Entities

- **Personagem**: Herói cadastrado no catálogo, identificado por nome, classe, atributos de combate, resistências, stress, chance de virtude, habilidades e equipamentos existentes.
- **Classe de herói**: Opção de classe válida para associar ao personagem durante o cadastro.
- **Habilidade de personagem**: Habilidade associada ao personagem, com estado de habilitação, treinamento, equipamento e nível de progressão quando disponível.
- **Operação de personagem**: Resultado de consulta, criação ou exclusão, incluindo estado de sucesso, carregamento ou erro apresentado ao usuário.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Um usuário consegue encontrar a tela de personagens e visualizar a lista ou o estado vazio em até 30 segundos após abrir o produto.
- **SC-002**: Pelo menos 90% dos cadastros válidos realizados em teste manual aparecem na lista em até 3 segundos após a confirmação.
- **SC-003**: 100% das tentativas de exclusão exigem confirmação explícita antes da alteração.
- **SC-004**: 100% dos estados de carregamento, vazio, sucesso e erro previstos têm uma mensagem ou indicação visual compreensível em PT-BR.
- **SC-005**: Pelo menos 90% dos usuários de teste conseguem criar um personagem válido sem consultar documentação externa.
- **SC-006**: Nenhum teste de viewport estreita apresenta sobreposição de conteúdo ou impede o acesso às ações principais.

## Assumptions

- O serviço existente de personagens continuará sendo a fonte oficial para consultar, criar e excluir registros.
- As regras de domínio e os limites atuais do personagem permanecem válidos; o frontend não substitui a validação do serviço.
- O usuário possui acesso ao ambiente do produto e não será introduzido um fluxo novo de autenticação nesta feature.
- A exclusão é permanente no catálogo e, por isso, exige confirmação; não será criado mecanismo de restauração nesta versão.
- Classes e habilidades necessárias ao formulário serão disponibilizadas pelo catálogo existente.
- O escopo desta feature é a experiência de consulta, criação e exclusão; edição de personagens, equipagem de itens e progressão de habilidades ficam fora do escopo.
- O frontend deve usar os contratos existentes do serviço de personagens; qualquer capacidade de exclusão ausente no serviço será tratada como dependência para implementação e planejamento.