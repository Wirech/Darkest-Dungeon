# Feature Specification: Hierarquia de Entidades e Catálogo Oficial de Heróis

**Feature Branch**: `003-hierarquia-catalogo-herois`

**Created**: 2026-09-07

**Status**: Draft

**Input**: User description: "Minerar dados das 20 classes de heróis de Darkest Dungeon a partir da wiki oficial, com foco em habilidades de combate e de acampamento. Mapear o que já foi especificado nas features anteriores e o que ainda precisa. Refinar a hierarquia de entidades para que Personagem e Inimigo herdem de Ser (que herda de Identificável); habilidades de personagem herdem de Habilidade de Herói, que herda de Habilidade, que herda de Identificável; itens (Arma, Armadura, Acessório) herdem de Item, que herda de Identificável; Classe forneça as resistências base do Personagem. Validar continuamente as regras de herança e minerar o máximo de dados relevantes da wiki."

## Clarifications

### Session 2026-09-07

- Q: Como estruturar habilidades acessíveis a Inimigos, dado que a hierarquia proposta cobre apenas heróis? → A: Manter uma hierarquia única com `Habilidade` como base identificável comum; `Habilidade de Herói` (subdividida em `Habilidade de Combate` e `Habilidade de Acampamento`) e `Habilidade de Inimigo` são subtipos irmãos de `Habilidade`, garantindo que Inimigo nunca receba habilidade de Acampamento e permitindo atributos próprios para habilidades de Inimigo.
- Q: Como modelar Arma, Armadura e Acessório em relação a `Item`? → A: `Item` é uma base abstrata identificável com Id, Nome e Descrição; `Arma`, `Armadura` e `Acessório` herdam de `Item` com atributos próprios; efeitos de `Acessório` são registrados como lista estruturada com nome do efeito, valor e sinal.
- Q: Como tratar as resistências de Doença, Golpe Mortal e Armadilha? → A: `Ser` permanece com as cinco resistências atuais (`Atordoamento`, `Sangramento`, `Envenenamento`, `Debuff`, `Movimento`); as três novas (`Doença`, `Golpe Mortal`, `Armadilha`) são modeladas apenas em `Personagem` e nas resistências base de `Classe`. `Inimigo` não recebe as três novas resistências nesta feature.
- Q: Como representar armas e armaduras que evoluem por níveis? → A: Cada `Arma` e cada `Armadura` é um único registro identificável que carrega uma lista fixa de cinco níveis; cada nível guarda os atributos numéricos daquele nível (para Arma: dano mínimo, dano máximo, crítico e velocidade; para Armadura: HP adicional e esquiva).
- Q: Habilidades homônimas entre classes devem ser um registro só ou instanciadas por classe? → A: Cada habilidade é um registro único no catálogo, identificado por nome global; classes se associam à habilidade por meio de uma associação Classe × Habilidade. Habilidades exclusivas de uma classe são aquelas associadas a uma única classe elegível; habilidades compartilhadas são o mesmo registro referenciado por várias classes.
- Q: Arma e Armadura devem ter classe elegível obrigatória ou opcional? → A: Toda `Arma` e toda `Armadura` MUST ter exatamente uma classe elegível; Personagens só podem equipar Arma ou Armadura da própria classe, e a validação é automática com base nesse vínculo.
- Q: Como marcar dados não coletados no Mapa de Cobertura? → A: Cada atributo catalogaável do Mapa de Cobertura tem exatamente um dos três estados: `Coletado`, `Pendente` ou `NaoAplicavel`. `Coletado` significa que o valor foi minerado e está disponível; `Pendente` significa que o dado deveria existir e ainda não foi minerado; `NaoAplicavel` significa que aquele atributo não faz sentido para aquela entidade e não deve ser cobrado como pendência.
- Q: Como representar os efeitos de uma Habilidade de Combate ou de Acampamento? → A: Cada efeito é uma linha estruturada com nome do efeito, alvo do efeito (self, aliado ou inimigo), valor numérico, unidade (percentual, pontos ou rodadas), duração (em rodadas quando aplicável) e chance base; uma Habilidade guarda uma lista dessas linhas, permitindo consulta e validação exatas.
- Q: Individualidades e Doenças de Personagem devem ser modeladas nesta feature ou adiadas? → A: Remover Individualidades e Doenças do escopo desta feature; permanecem apenas como intenção documentada para uma feature futura. Personagem nesta feature não carrega essas coleções.
- Q: Qual limite de habilidades atribuídas a Personagem esta feature deve aplicar? → A: Limite fixo de até 6 habilidades de Combate e até 6 habilidades de Acampamento por Personagem, validado no serviço com rejeição em PT-BR e teste correspondente.
- Q: Como validar habilidades atribuídas a um Personagem em relação à Classe? → A: O serviço de Personagem MUST validar que todas as habilidades atribuídas pertencem à associação Classe × Habilidade da Classe do Personagem; qualquer habilidade fora da Classe MUST resultar em rejeição 400 em PT-BR, coberto por teste de endpoint.
- Q: Como esta feature deve tratar bonus de conjunto de Acessórios? → A: Modelar apenas `ConjuntoId` como metadata nesta feature; o cálculo e a aplicação do bônus quando duas peças do conjunto são equipadas ficam para uma feature futura, fora deste escopo.
- Q: Qual o formato padrão das mensagens de erro em PT-BR nos novos endpoints? → A: Reutilizar o contrato `ErroResponse` da feature 001 (`mensagem` obrigatório, `campo` opcional) em todos os novos endpoints, com teste de contrato validando que as respostas 400 e 404 seguem esse schema.
- Q: Como tratar alterações futuras em uma Habilidade já catalogada em relação aos Personagens que a referenciam? → A: Manter referência viva: `HabilidadeDePersonagem` continua guardando apenas `HabilidadeId`. Esta feature NÃO expõe endpoint de atualização de `Habilidade` (`PUT`/`PATCH`); quando uma feature futura introduzir edição, ela MUST exigir confirmação explícita e registrar a mudança como evento auditado. Assim não há propagação silenciosa dentro do escopo desta feature.
- Q: As raridades suportadas para Acessórios devem formar um conjunto fechado ou permanecer abertas? → A: Conjunto fechado com exatamente sete valores: `Comum`, `Incomum`, `Rara`, `MuitoRara`, `CrimsonCourt`, `Crystalline`, `Set`. Novas raridades exigem atualização explícita do enum e migração em feature futura.
- Q: Esta feature deve suportar exclusão de Classe, Habilidade ou Item? → A: Não. Exclusão está fora do escopo desta feature; nenhum endpoint DELETE será exposto e o banco MUST proteger todas as FKs de Classe/Habilidade/Item com `OnDelete(Restrict)`, garantido por teste de integridade.
- Q: A spec deve declarar explicitamente que `Inimigo` não possui as três novas resistências? → A: Sim. A Key Entity `Inimigo` MUST afirmar de forma explícita e positiva que as resistências de `Inimigo` são exatamente as cinco de `Ser` (Atordoamento, Sangramento, Envenenamento, Debuff, Movimento), sem `Doença`, `Golpe Mortal` ou `Armadilha`.
- Q: Como acomodar uma futura 21ª classe de herói (DLC novo) sem quebrar os 20 nomes atuais? → A: Manter enum fechado com 20 valores nesta feature. Adicionar uma classe futura envolve procedimento documentado: (1) novo valor no enum `ClasseDeHeroi` com atributo `Description` para o nome oficial; (2) migração dedicada inserindo a linha em `Classe` com `ResistenciasBase`; (3) atualização do seed e do Mapa de Cobertura.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Refinar a hierarquia de entidades para refletir o domínio (Priority: P1)

Como responsável pela modelagem do domínio, quero que a hierarquia de entidades reflita as regras reais do jogo, para que Personagem, Inimigo, habilidades e itens compartilhem regras comuns sem duplicação e permitam extensão futura sem retrabalho.

**Why this priority**: Sem a hierarquia correta, o catálogo minerado e as regras de personagem/inimigo ficam inconsistentes ou duplicadas.

**Independent Test**: Pode ser validado consultando o mapa de entidades (contratos e conceitos) e confirmando que Personagem e Inimigo compartilham `Ser`; que habilidades de herói (Combate e Acampamento) compartilham as regras comuns de `Habilidade`; e que Arma, Armadura e Acessório compartilham as regras comuns de `Item`.

**Acceptance Scenarios**:

1. **Given** o modelo refinado, **When** um responsável consulta a definição de Personagem, **Then** o sistema apresenta os atributos herdados de `Ser` e os próprios de Personagem sem duplicar regras já definidas em `Ser`.
2. **Given** o modelo refinado, **When** um responsável consulta a definição de Habilidade de Combate ou Habilidade de Acampamento, **Then** o sistema mostra os atributos comuns de `Habilidade de Herói` e os atributos próprios de cada tipo, sem repetir campos base.
3. **Given** o modelo refinado, **When** um responsável consulta a definição de Arma, Armadura ou Acessório, **Then** o sistema apresenta os atributos comuns de `Item` e os atributos próprios de cada tipo.
4. **Given** a hierarquia refinada, **When** um mantenedor tenta usar uma regra específica de Personagem em Inimigo, ou vice-versa, **Then** o sistema rejeita porque cada especialização mantém suas regras próprias, sem herança lateral indevida.

---

### User Story 2 - Popular o catálogo de heróis e habilidades com as 20 classes oficiais (Priority: P2)

Como responsável pelo conteúdo do jogo, quero que o sistema conheça as 20 classes de herói do jogo base e seus DLCs, com suas habilidades de Combate e Acampamento, para que o catálogo espelhe o domínio real e possa ser consultado por outras funcionalidades.

**Why this priority**: O catálogo real do jogo é o insumo necessário para todos os fluxos de Personagem, sem ele o sistema permanece vazio.

**Independent Test**: Pode ser testado listando as 20 classes suportadas, listando as habilidades de Combate e de Acampamento de cada classe, e confirmando que cada habilidade traz seus atributos observáveis (posições de execução, alvos, modificadores, efeitos e limites).

**Acceptance Scenarios**:

1. **Given** o catálogo popular, **When** um usuário lista as classes suportadas, **Then** o sistema retorna exatamente 20 classes correspondendo às classes oficiais do jogo (15 do jogo base e 5 dos DLCs), sem duplicidade.
2. **Given** uma classe existente, **When** um usuário consulta suas habilidades de Combate, **Then** o sistema retorna todas as habilidades de Combate conhecidas para aquela classe, cada uma com nome, posições em que pode ser usada, posições que atinge, modificador de dano, modificador de acerto, modificador de crítico, efeitos e eventuais limites por batalha.
3. **Given** uma classe existente, **When** um usuário consulta suas habilidades de Acampamento, **Then** o sistema retorna as habilidades de Acampamento conhecidas para aquela classe, cada uma com custo de descanso, alvo, descrição de efeitos e indicação de quantas vezes pode ser usada por acampamento.
4. **Given** o catálogo popular, **When** um usuário consulta as habilidades de Acampamento compartilhadas entre classes, **Then** o sistema mostra quais classes têm acesso a essas habilidades, evitando duplicidade de cadastro.
5. **Given** o catálogo popular, **When** um usuário compara o resultado com as classes descritas na fonte oficial, **Then** todas as 20 classes e suas habilidades conhecidas estão representadas ou explicitamente marcadas como pendentes de coleta.

---

### User Story 3 - Refinar Item para incluir Arma, Armadura e Acessório com regras próprias (Priority: P3)

Como responsável pelo conteúdo, quero que o sistema represente Arma, Armadura e Acessório como especializações de Item, para que equipamentos de personagens tenham atributos e limites próprios do domínio.

**Why this priority**: O detalhamento de Item permite atribuir equipamentos a personagens e é pré-requisito para regras futuras de combate.

**Independent Test**: Pode ser testado cadastrando exemplos de Arma, Armadura e Acessório do jogo, consultando seus atributos e verificando que o sistema rejeita atributos inválidos para cada tipo.

**Acceptance Scenarios**:

1. **Given** o refinamento de Item, **When** um responsável cadastra uma Arma com um nível permitido e valores de dano, crítico e velocidade, **Then** o sistema aceita e mantém esses atributos consultáveis por Item ou por Arma.
2. **Given** o refinamento de Item, **When** um responsável cadastra uma Armadura com um nível permitido e valores de HP e esquiva, **Then** o sistema aceita e mantém esses atributos consultáveis.
3. **Given** o refinamento de Item, **When** um responsável cadastra um Acessório com raridade e efeitos livres, **Then** o sistema aceita e permite marcar o Acessório como exclusivo de uma ou mais classes.
4. **Given** um Personagem com sua classe, **When** o usuário tenta equipar um Acessório exclusivo de outra classe, **Then** o sistema rejeita a operação com mensagem em PT-BR.
5. **Given** os equipamentos definidos, **When** um usuário consulta um Personagem, **Then** os slots de Arma, Armadura e Acessório referenciam Item ou seus herdeiros, sem duplicar seus atributos.

---

### User Story 4 - Alinhar Classe e resistências às oito resistências do jogo (Priority: P4)

Como responsável pela modelagem, quero que Classe forneça as resistências base do Personagem e que o conjunto de resistências reflita o jogo real (`Atordoamento`, `Sangramento`, `Envenenamento`, `Debuff`, `Movimento`, `Doença`, `Golpe Mortal`, `Armadilha`), para que os dados minerados possam ser gravados sem perda.

**Why this priority**: Sem os campos corretos, dados oficiais do jogo (por exemplo, `Deathblow 67%`, `Trap 40%`) ficariam sem lugar para ser armazenados.

**Independent Test**: Pode ser testado consultando as resistências base de uma Classe do jogo e comparando com o valor oficial, e criando um Personagem de uma classe para confirmar que as resistências iniciais correspondem às da classe.

**Acceptance Scenarios**:

1. **Given** o catálogo de Classes, **When** um usuário consulta as resistências base de uma classe do jogo, **Then** o sistema retorna as oito resistências suportadas por essa classe com seus valores oficiais.
2. **Given** o refinamento de Classe, **When** um usuário cria um Personagem para uma classe existente, **Then** as resistências iniciais do Personagem correspondem às resistências base da classe.
3. **Given** o refinamento de Classe, **When** um usuário consulta uma classe cujas resistências não puderam ser mineradas, **Then** o sistema indica explicitamente essa pendência sem inventar valores.

---

### User Story 5 - Consultar e atualizar o mapa de cobertura entre spec e conteúdo (Priority: P5)

Como responsável pela feature, quero um mapa que mostre quais dados oficiais das 20 classes já estão cobertos por 002 e o que 003 precisa adicionar, para acompanhar a evolução sem perder rastreabilidade.

**Why this priority**: A consulta do mapa evita que dados minerados fiquem inconsistentes com as decisões anteriores.

**Independent Test**: Pode ser testado consultando o mapa antes e depois de adicionar uma classe ou uma habilidade minerada, confirmando que a cobertura reflete o estado atual.

**Acceptance Scenarios**:

1. **Given** o mapa de cobertura, **When** um responsável consulta a situação de uma classe, **Then** o sistema mostra quais habilidades de Combate, de Acampamento e quais atributos de Classe já estão registrados e quais faltam.
2. **Given** o mapa de cobertura, **When** um responsável adiciona uma habilidade minerada, **Then** o mapa passa a considerar essa habilidade coberta para a classe correspondente.
3. **Given** o mapa de cobertura, **When** um responsável remove ou marca uma habilidade como não confirmada, **Then** o sistema não a considera coberta.

---

### Edge Cases

- O sistema deve rejeitar habilidade duplicada no catálogo global e associação duplicada da mesma habilidade à mesma classe.
- O sistema deve rejeitar Arma ou Armadura que não tenha exatamente cinco níveis definidos (1 a 5) ou que tenha níveis duplicados ou fora do intervalo.
- O sistema deve rejeitar Acessório sem raridade válida.
- O sistema deve rejeitar tentativa de vincular uma habilidade de acampamento a um Inimigo, porque acampamento é conceito de herói.
- O sistema deve rejeitar Arma ou Armadura sem exatamente uma classe elegível definida.
- O sistema deve rejeitar tentativa de equipar em um Personagem uma Arma ou Armadura de classe diferente da classe do Personagem.
- O sistema deve rejeitar tentativa de equipar Acessório exclusivo de outra classe em um Personagem.
- O sistema deve tratar habilidades compartilhadas de acampamento como uma única definição referenciada por várias classes, sem duplicá-la.
- O sistema deve permitir marcar habilidade ou classe como `Pendente` ou `NaoAplicavel` no Mapa de Cobertura, sem que isso represente dado falso; `Pendente` e `NaoAplicavel` são distintos e não podem ser tratados como sinônimos.
- Se a fonte minerada não fornecer um atributo específico de uma habilidade (por exemplo, chance base de um efeito), o sistema deve gravar essa lacuna como `Pendente` e não presumir um valor arbitrário.
- Alterações posteriores em uma Habilidade catalogada não podem alterar silenciosamente dados de Personagens já criados: esta feature NÃO expõe endpoint de atualização de `Habilidade`; `HabilidadeDePersonagem` mantém apenas `HabilidadeId` (referência viva) e qualquer futuro endpoint de edição MUST exigir confirmação explícita e auditoria.
- Personagens de uma classe não podem receber habilidade de Combate ou Acampamento que não pertença àquela classe, ou àquelas classes compartilhadas do acampamento.
- O sistema MUST manter integridade referencial entre Classe, Habilidade e Personagem: exclusão está fora do escopo desta feature e o banco MUST rejeitar tentativas via `OnDelete(Restrict)`.
- Se o catálogo receber uma classe adicional futura (por exemplo, um DLC novo), a expansão segue procedimento documentado: adicionar novo valor no enum `ClasseDeHeroi` com nome oficial em inglês via `Description`, criar migração dedicada inserindo a linha em `Classe` com `ResistenciasBase` e atualizar seed e Mapa de Cobertura. Os 20 nomes atuais permanecem inalterados.
- Personagem, Inimigo, Item e todas as habilidades devem preservar as regras comuns de `Identificavel` e das camadas superiores, sem quebrar as restrições existentes das features 001 e 002.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O sistema MUST manter Personagem e Inimigo como especializações de `Ser`, com `Ser` continuando como especialização de `Identificavel`, sem alterar as regras já estabelecidas em 001.
- **FR-002**: O sistema MUST definir `Habilidade` como conceito identificável comum, com uma especialização `Habilidade de Herói` que se divide em `Habilidade de Combate` e `Habilidade de Acampamento`.
- **FR-003**: O sistema MUST manter habilidades acessíveis a Personagens organizadas por categoria (Combate ou Acampamento) e MUST tratar cada categoria com seus próprios atributos.
- **FR-004**: O sistema MUST tratar habilidades usadas por Inimigos como `Habilidade de Inimigo`, subtipo irmão de `Habilidade de Herói` sob a base comum `Habilidade`, garantindo que Inimigo nunca receba uma `Habilidade de Acampamento` e que `Habilidade de Inimigo` possa ter atributos próprios (por exemplo, condição de aparecer, chance de execução) que não existem em `Habilidade de Herói`.
- **FR-005**: O sistema MUST manter para cada `Habilidade de Combate` os atributos: nome, descrição, posições válidas de execução, posições que atinge (com indicação de alvo único ou área), modificador de dano, modificador de acerto, modificador de crítico, lista estruturada de `Efeito de Habilidade` e eventuais limites por batalha.
- **FR-006**: O sistema MUST manter para cada `Habilidade de Acampamento` os atributos: nome, descrição, custo de descanso, alvo, lista estruturada de `Efeito de Habilidade` e eventuais limites por acampamento.
- **FR-006a**: O sistema MUST manter cada `Efeito de Habilidade` como uma linha estruturada com nome do efeito, alvo do efeito (self, aliado ou inimigo), valor numérico, unidade (percentual, pontos ou rodadas), duração (em rodadas quando aplicável) e chance base; qualquer atributo do efeito ainda não minerado MUST ser marcado como `Pendente` no Mapa de Cobertura.
- **FR-007**: O sistema MUST manter cada habilidade como um registro único no catálogo, identificado por nome global, e MUST permitir que uma mesma habilidade seja referenciada por uma ou mais classes por meio de uma associação Classe × Habilidade.
- **FR-008**: O sistema MUST manter a lista fechada das 20 classes oficiais do jogo, com nomes controlados que possam ser expostos em PT-BR sem perder rastreabilidade para os nomes originais.
- **FR-009**: O sistema MUST permitir associar, para cada classe, as habilidades de Combate e de Acampamento conhecidas do catálogo, sem duplicar o registro da habilidade, e MUST limitar cada Personagem a no máximo 6 habilidades de Combate e no máximo 6 habilidades de Acampamento atribuídas simultaneamente.
- **FR-009a**: O sistema MUST validar, na criação ou alteração de um Personagem, que toda habilidade atribuída pertence à associação Classe × Habilidade da Classe do Personagem; qualquer habilidade fora dessa associação MUST resultar em rejeição 400 em PT-BR.
- **FR-010**: O sistema MUST manter `Item` como especialização abstrata de `Identificavel` com Id, Nome e Descrição comuns; `Arma`, `Armadura` e `Acessório` MUST herdar de `Item` com atributos próprios, sem duplicar campos base.
- **FR-011**: O sistema MUST manter cada Arma como um único registro identificável com nome, descrição, exatamente uma classe elegível e uma lista fixa de cinco níveis; cada nível MUST carregar dano mínimo, dano máximo, crítico e velocidade daquele nível.
- **FR-012**: O sistema MUST manter cada Armadura como um único registro identificável com nome, descrição, exatamente uma classe elegível e uma lista fixa de cinco níveis; cada nível MUST carregar HP adicional e esquiva daquele nível.
- **FR-013**: O sistema MUST manter para cada Acessório pelo menos: raridade, efeitos (positivos e negativos), classe exclusiva quando aplicável e um identificador opcional de conjunto (`ConjuntoId`) como metadata para agrupamento futuro. O cálculo do bônus quando duas peças do mesmo conjunto são equipadas está fora do escopo desta feature.
- **FR-014**: O sistema MUST modelar `Classe` como entidade com resistências base para as oito resistências suportadas por heróis (`Atordoamento`, `Sangramento`, `Envenenamento`, `Debuff`, `Movimento`, `Doença`, `Golpe Mortal` e `Armadilha`).
- **FR-015**: O sistema MUST manter as cinco resistências atuais de `Ser` inalteradas; as resistências `Doença`, `Golpe Mortal` e `Armadilha` MUST ser modeladas apenas em `Personagem` e nas resistências base de `Classe`, sem afetar `Ser` nem `Inimigo`.
- **FR-016**: O sistema MUST permitir que a criação de Personagem consulte a Classe para preencher as resistências base do Personagem no momento da criação.
- **FR-017**: O sistema MUST manter um Mapa de Cobertura consultável que indique, para cada classe e para cada atributo catalogaável (habilidades de Combate, habilidades de Acampamento e resistências base), exatamente um dos estados `Coletado`, `Pendente` ou `NaoAplicavel`.
- **FR-018**: O sistema MUST permitir marcar atributos como `Pendente` (dado deveria existir e ainda não foi minerado) ou `NaoAplicavel` (o atributo não faz sentido para aquela entidade), e MUST tratar esses dois estados de forma distinta entre si e distinta de `Coletado`.
- **FR-019**: O sistema MUST rejeitar duplicidade de nome de habilidade no catálogo global e MUST rejeitar associação duplicada da mesma habilidade à mesma classe.
- **FR-020**: O sistema MUST rejeitar cadastro de Arma ou Armadura que não tenha exatamente cinco níveis definidos ou que tenha níveis fora do intervalo suportado pelo jogo (1 a 5).
- **FR-021**: O sistema MUST rejeitar cadastro de Acessório sem raridade suportada. O conjunto fechado de raridades suportadas nesta feature é: `Comum`, `Incomum`, `Rara`, `MuitoRara`, `CrimsonCourt`, `Crystalline` e `Set`.
- **FR-022**: O sistema MUST impedir que um Personagem equipe uma Arma, Armadura ou Acessório cuja classe elegível não corresponda à classe do Personagem; para Acessórios, essa regra vale apenas quando o Acessório for exclusivo de uma classe.
- **FR-023**: O sistema MUST manter integridade referencial entre Classe, Habilidade, Personagem e Item. Exclusão de Classe, Habilidade ou Item está fora do escopo desta feature: nenhum endpoint DELETE será exposto e todas as FKs desses catálogos MUST ser configuradas com `OnDelete(Restrict)` no banco, garantido por teste de integridade.
- **FR-024**: O sistema MUST reutilizar validações e regras comuns de `Ser` para Personagem e Inimigo e de `Item` para suas especializações, sem duplicação.
- **FR-025**: O sistema MUST expor os catálogos e as consultas em PT-BR e MUST reutilizar o contrato `ErroResponse` da feature 001 (`mensagem` obrigatória em PT-BR, `campo` opcional) em todas as respostas de erro (400/404) dos novos endpoints.
- **FR-026**: O sistema MUST manter contratos por interface entre camadas para as novas entidades e MUST registrar suas dependências no contêiner de injeção de dependência, alinhado à decisão de arquitetura das features anteriores.

### Key Entities

- **Identificavel**: Base comum já definida em 001; permanece inalterada.
- **Ser**: Base comum de criaturas do jogo, já definida em 001; permanece inalterada nesta feature, mantendo as cinco resistências atuais.
- **Personagem**: Especialização de `Ser` que representa um herói jogável; carrega Classe, Stress, Chance de Virtude, habilidades, equipamentos, aflição ou virtude e as resistências próprias de `Doença`, `Golpe Mortal` e `Armadilha`, herdadas da Classe no momento da criação. Individualidades e Doenças de Personagem estão fora do escopo desta feature e serão tratadas em feature futura.
- **Inimigo**: Especialização de `Ser` que representa um oponente controlado pelo sistema; carrega Tipo de Inimigo, resistências próprias e lista arbitrária de `Habilidade de Inimigo`. As resistências de `Inimigo` são exatamente as cinco de `Ser` (`Atordoamento`, `Sangramento`, `Envenenamento`, `Debuff`, `Movimento`); `Inimigo` intencionalmente NÃO possui `Doença`, `Golpe Mortal` nem `Armadilha` nesta feature.
- **Classe**: Catálogo controlado das 20 classes oficiais; fornece as habilidades disponíveis para a classe e as resistências base para as oito resistências suportadas por heróis, aplicadas ao Personagem no momento da criação.
- **Habilidade**: Base comum identificável para todas as habilidades do jogo, com nome único global, descrição e categoria; classes se associam à habilidade por meio da associação Classe × Habilidade, e uma mesma habilidade pode ser referenciada por várias classes. Uma habilidade referenciada por mais de uma classe (por exemplo, uma habilidade de acampamento compartilhada como `Gallows Humor`) permanece como um único registro no catálogo com múltiplas linhas em Classe × Habilidade, sem duplicação.
- **Habilidade de Herói**: Especialização de `Habilidade` que agrupa as habilidades acessíveis a Personagens; se subdivide em `Habilidade de Combate` e `Habilidade de Acampamento`.
- **Habilidade de Combate**: Habilidade usada em batalha, com posições válidas de execução, posições que atinge, modificadores de dano, acerto e crítico, lista estruturada de `Efeito de Habilidade` e limites por batalha.
- **Habilidade de Acampamento**: Habilidade usada em acampamentos, com custo de descanso, alvo, lista estruturada de `Efeito de Habilidade` e limites por acampamento.
- **Efeito de Habilidade**: Linha estruturada usada por Habilidade de Combate ou Habilidade de Acampamento; carrega nome do efeito, alvo (self, aliado ou inimigo), valor numérico, unidade (percentual, pontos ou rodadas), duração em rodadas quando aplicável e chance base.
- **Habilidade de Inimigo**: Subtipo irmão de `Habilidade de Herói` sob a base comum `Habilidade`; representa habilidades acessíveis apenas a Inimigos, com atributos próprios distintos das habilidades de heróis e sem categoria de acampamento.
- **Item**: Especialização abstrata de `Identificavel` que representa qualquer objeto do jogo, com Id, Nome e Descrição comuns; não é instanciado diretamente.
- **Arma**: Especialização de `Item` com exatamente uma classe elegível e uma lista fixa de cinco `Nível de Arma` (1 a 5); cada Nível de Arma carrega dano mínimo, dano máximo, crítico e velocidade daquele nível.
- **Armadura**: Especialização de `Item` com exatamente uma classe elegível e uma lista fixa de cinco `Nível de Armadura` (1 a 5); cada Nível de Armadura carrega HP adicional e esquiva daquele nível.
- **Nível de Arma**: Componente interno de Arma que representa os atributos numéricos de um dos cinco níveis de progressão.
- **Nível de Armadura**: Componente interno de Armadura que representa os atributos numéricos de um dos cinco níveis de progressão.
- **Acessório**: Especialização de `Item` (trinket) com raridade, efeitos positivos e negativos, classe exclusiva quando aplicável e possibilidade de fazer parte de um conjunto.
- **Mapa de Cobertura**: Registro consultável que indica, por classe e por atributo catalogaável (habilidades de Combate, habilidades de Acampamento e resistências base), qual dos três estados se aplica: `Coletado`, `Pendente` ou `NaoAplicavel`.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% das definições de Personagem, Inimigo, Arma, Armadura, Acessório e habilidades de herói reutilizam as regras comuns das entidades superiores, sem duplicação de campos base.
- **SC-002**: O catálogo de classes contém exatamente 20 classes correspondentes às classes oficiais do jogo, com nomes controlados em PT-BR e rastreabilidade para o nome original.
- **SC-003**: 100% das habilidades de Combate registradas contêm nome, posições válidas, posições que atinge, modificadores e efeitos observáveis; qualquer atributo faltante é marcado como `Pendente` ou `NaoAplicavel` no Mapa de Cobertura.
- **SC-004**: 100% das habilidades de Acampamento registradas contêm custo de descanso, alvo, efeitos e limite por acampamento; qualquer atributo faltante é marcado como `Pendente` ou `NaoAplicavel` no Mapa de Cobertura.
- **SC-005**: 100% das Armas e Armaduras registradas contêm exatamente cinco níveis (1 a 5) com atributos numéricos observáveis em cada nível.
- **SC-006**: 100% dos Acessórios registrados contêm raridade suportada e, quando exclusivos de uma classe, o vínculo com essa classe.
- **SC-007**: 100% das classes registradas fornecem resistências base para as oito resistências suportadas por heróis, com estado `Pendente` explicitamente registrado no Mapa de Cobertura quando o dado oficial for desconhecido.
- **SC-008**: O mapa de cobertura reflete corretamente, em 100% das consultas, o estado atual do catálogo de cada classe.
- **SC-009**: 100% das tentativas inválidas (habilidade duplicada, nível fora do intervalo, raridade inválida, habilidade de Acampamento em Inimigo, Acessório exclusivo em classe errada) são rejeitadas com mensagem em PT-BR.
- **SC-010**: Personagens criados por classe recebem as resistências base da classe em 100% dos casos, mantendo compatibilidade com as regras já definidas em 001 e 002.

## Assumptions

- A fonte oficial minerada é a wiki `darkestdungeon.wiki.gg`; nomes originais em inglês serão mantidos como referência mesmo quando os nomes de negócio forem expostos em PT-BR.
- A mineração completa dos dados (leitura e transcrição fiel das 20 páginas de herói) é atividade da fase de implementação; a especificação garante que o modelo aguente todos os atributos observados na fonte.
- Os limites de habilidades atribuídas a Personagens (até 6 de Combate e até 6 de Acampamento) são definidos e validados por esta feature (FR-009), sem depender de 002.
- Os níveis de Arma e Armadura seguem o jogo original (níveis 1 a 5 obrigatórios), e cada equipamento é modelado como um único registro com essa lista fixa de níveis.
- As raridades de Acessório formam um conjunto fechado de exatamente sete valores: `Comum`, `Incomum`, `Rara`, `MuitoRara`, `CrimsonCourt`, `Crystalline` e `Set`. Novas raridades exigem atualização explícita do enum em feature futura.
- Personagens criados antes da atualização de um dado catalogado permanecem inalterados, conforme regra já estabelecida em 002.
- O sistema não modela cálculos de combate nem simulação de batalha; ele apenas mantém os dados e as regras estruturais para futuras funcionalidades.
- O escopo desta feature termina no mapa de cobertura e nas definições estruturais das entidades; regras de progressão de nível, treinamento de habilidades específicas ou economia de recursos ficam fora deste escopo.
- A decisão sobre estrutura de armazenamento (por exemplo, tabelas físicas por especialização ou herança) fica para o plano; a spec descreve apenas WHAT e as regras observáveis.


