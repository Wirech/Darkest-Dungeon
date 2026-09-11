# Feature Specification: Atributos Oficiais do Personagem

**Feature Branch**: `009-atributos-oficiais-personagem`

**Created**: 2026-09-10

**Status**: Draft

**Input**: User description: "quero que o personagem tenha todos os atributos atribuídos na criação pela classe e nível, usando dados oficiais da wiki. verifique se essas informações já foram mineradas, se não, projete um plano pra adquirí-las de forma automática antes de começar a feature. verifique: como o hp, vel, acc etc são calculados no jogo original. por enquanto desconsidere trinkets. isso deve ser integrado na criação do personagem de forma fiel. no card do personagem (no frontend), deve aparecer todas as suas informações, separadas por categoria: hp, stress, atributos (com acc, prot, esquiva etc), resistencias, habilidades de combate (sinalizadas com o nível), habilidades de acampamento (sinalizadas com o nível)"

## Levantamento de dados existentes

Antes desta feature, o catálogo oficial já cobre:

- As 20 classes canônicas, com nomes PT-BR e originais.
- As 8 resistências **base** de cada classe, mineradas da wiki.
- As habilidades de combate e de acampamento por classe, mineradas da wiki.
- A estrutura de arma e armadura com cinco níveis cada (dano, crítico e velocidade na arma; HP e esquiva na armadura).

Ainda **não** estão minerados de forma oficial e completa:

- Os valores numéricos oficiais dos 5 níveis de arma de cada classe (dano mínimo/máximo, crítico, velocidade).
- Os valores numéricos oficiais dos 5 níveis de armadura de cada classe (HP máximo, esquiva).
- Dados de classe usados no cálculo fiel e ainda ausentes do catálogo de criação, em especial o deslocamento típico do herói (passos à frente/atrás).
- A regra de crescimento de resistências pelo nível de resolução, que hoje não entra na criação.

Conclusão: a criação fiel **não pode começar** até as tabelas oficiais de equipamento (e o deslocamento por classe) estarem baixadas, validadas e gravadas no catálogo **local**. Resistências base e habilidades já podem ser reutilizadas. A criação **nunca** consulta a wiki em tempo real.

## Clarifications

### Session 2026-09-10

- Q: Se a coleta da wiki completar 19 classes e falhar em 1, o usuário ainda pode criar heróis das classes com tabela completa? → A: Não há consulta em tempo real à wiki. As informações oficiais são baixadas e ficam só no catálogo local. “Falha pontual na wiki” não é um modo de operação do produto. Se faltar qualquer informação, a coleta/implementação para e o responsável decide caso a caso; não se inventa valor nem se libera criação parcial.
- Q: O deslocamento do herói deve ser gravado como passos à frente e passos atrás, iguais à wiki, ou como um único número de movimento? → A: Dois valores: passos à frente e passos atrás.
- Q: Na criação de uma Abominação, qual bloco oficial de atributos deve ser gravado no personagem? → A: Só a forma humana.
- Q: Além das seis categorias pedidas, o card deve mostrar também as escolhas da criação (nível do herói, nível da arma, nível da armadura e aparência)? → A: As seis categorias, mais nível do herói, da arma, da armadura e aparência, e também os demais dados oficiais da classe na wiki (ao menos: religioso, provisão inicial e bônus ao crítico).
- Q: Os dados extras da classe (se é religiosa, provisão inicial e bônus ao crítico) devem ser copiados para o personagem na criação ou só lidos do catálogo da classe na hora de montar o card? → A: Manter só na classe; o card busca de lá.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Completar o catálogo oficial de atributos (Priority: P1)

Como responsável pelo catálogo, quero que os números oficiais de arma, armadura e deslocamento de cada classe sejam baixados da wiki **uma vez** e gravados no catálogo local antes de qualquer criação fiel, para que o herói nunca nasça com valores inventados nem dependa da wiki no uso diário.

**Why this priority**: Sem esses dados locais, HP, velocidade, dano, crítico e esquiva não podem ser fiéis. A coleta é pré-requisito das demais histórias.

**Independent Test**: Executar o download das 20 classes, gravar o resultado localmente e conferir que cada uma possui 5 níveis de arma e 5 de armadura com todos os campos oficiais preenchidos, além dos dois valores de deslocamento (passos à frente e passos atrás). A criação usa somente esse catálogo local.

**Acceptance Scenarios**:

1. **Given** as 20 classes canônicas, **When** o download oficial é executado, **Then** cada classe fica com tabela local completa de arma (níveis 1 a 5: dano mínimo, dano máximo, crítico, velocidade) e de armadura (níveis 1 a 5: HP máximo, esquiva).
2. **Given** que o download encontrou lacuna em qualquer classe ou nível, **When** a coleta termina, **Then** o trabalho para e o responsável é questionado caso a caso; nenhum valor é inventado e a criação fiel não é considerada pronta.
3. **Given** um catálogo local já completo, **When** o usuário cria um personagem, **Then** a operação lê somente os dados locais e não acessa a wiki.
4. **Given** uma nova execução do download com os mesmos valores, **Then** o catálogo local permanece idempotente (sem duplicar equipamentos da mesma classe).
5. **Given** um valor gravado, **When** o responsável consulta a origem, **Then** é possível identificar a classe, o tipo de equipamento, o nível e que a fonte original foi a wiki oficial.

---

### User Story 2 - Criar personagem com atributos fiéis ao jogo original (Priority: P1)

Como usuário do catálogo, quero informar apenas as escolhas já usadas na criação (nome, classe, nível do herói, nível da arma, nível da armadura e aparência) e receber um personagem cujos atributos batem com o Darkest Dungeon original, sem acessórios.

**Why this priority**: Corrige a criação atual, que preenche HP, velocidade e precisão de forma incompleta ou inventada.

**Independent Test**: Criar um herói de classe conhecida (por exemplo Cruzado) com níveis explícitos de herói, arma e armadura e conferir HP, esquiva, velocidade, dano, crítico, precisão, proteção, passos à frente, passos atrás, stress, chance de virtude e resistências contra a tabela oficial correspondente.

**Acceptance Scenarios**:

1. **Given** classe, nível do herói (resolução), nível da arma e nível da armadura válidos e catálogo oficial completo, **When** o usuário confirma a criação, **Then** o personagem nasce com HP máximo igual ao HP oficial da armadura da classe naquele nível, HP atual igual ao HP máximo e stress 0.
2. **Given** o mesmo conjunto de escolhas, **When** o personagem é criado, **Then** velocidade, crítico e faixa de dano vêm exclusivamente da arma oficial da classe no nível escolhido, e a esquiva vem exclusivamente da armadura oficial no nível escolhido.
3. **Given** a criação sem acessórios, **When** os atributos são gravados, **Then** o modificador de precisão do herói é 0 e a proteção é 0, porque no jogo original esses valores não vêm da arma/armadura nem da classe base.
4. **Given** o nível de resolução escolhido, **When** as resistências são aplicadas, **Then** atordoamento, sangramento, envenenamento, debuff, movimento e doença usam a base da classe acrescida de 10 pontos percentuais por nível de resolução (limitado a 100), enquanto golpe mortal e armadilha permanecem iguais à base da classe.
5. **Given** a classe escolhida, **When** o personagem é criado, **Then** os passos à frente e os passos atrás persistidos correspondem ao deslocamento oficial da classe, não a um único número genérico.
6. **Given** as habilidades da classe, **When** o personagem é criado, **Then** permanece a inicialização já definida (quatro habilidades de combate e quatro de acampamento desbloqueadas no nível 1; demais no nível 0), sem o usuário informar atributos de combate.
7. **Given** a classe Abominação, **When** o personagem é criado, **Then** HP, esquiva, velocidade, dano e crítico vêm das tabelas oficiais da forma humana no nível de arma/armadura escolhido, não da forma besta.

---

### User Story 3 - Ver o card completo do personagem por categoria (Priority: P1)

Como usuário do catálogo, quero ver no card de cada personagem todas as informações relevantes, agrupadas por categoria, para entender o herói sem abrir outra tela.

**Why this priority**: A consulta atual omite atributos, resistências e o nível das habilidades; o card precisa refletir o personagem fiel recém-criado.

**Independent Test**: Abrir a lista com pelo menos um personagem criado pelo fluxo fiel e conferir que o card mostra as seis categorias pedidas, as escolhas de criação (nível do herói, da arma, da armadura e aparência) e os dados oficiais extras da classe (religioso, provisão inicial e bônus ao crítico), com habilidades de combate e acampamento separadas e cada uma com o nível.

**Acceptance Scenarios**:

1. **Given** um personagem cadastrado, **When** o usuário vê o card, **Then** as informações aparecem separadas nas categorias: HP; Stress; Atributos; Resistências; Habilidades de combate; Habilidades de acampamento.
2. **Given** a categoria HP, **When** o card é exibido, **Then** aparecem HP atual e HP máximo.
3. **Given** a categoria Atributos, **When** o card é exibido, **Then** aparecem pelo menos precisão, proteção, esquiva, velocidade, crítico, faixa de dano, passos à frente e passos atrás, com os valores persistidos do personagem.
4. **Given** a categoria Resistências, **When** o card é exibido, **Then** aparecem as oito resistências do herói (atordoamento, sangramento, envenenamento, debuff, movimento, doença, golpe mortal e armadilha).
5. **Given** as categorias de habilidades, **When** o card é exibido, **Then** todas as habilidades de combate da classe aparecem com o nível, todas as de acampamento aparecem com o nível, e as duas listas não se misturam.
6. **Given** uma habilidade no nível 0, **When** o card é exibido, **Then** ela continua visível e sinalizada como nível 0 (bloqueada), em vez de ser omitida.
7. **Given** o cabeçalho ou a área de identificação do card, **When** o card é exibido, **Then** aparecem nível do herói, nível da arma, nível da armadura e aparência.
8. **Given** os dados oficiais extras no catálogo da classe, **When** o card é exibido, **Then** aparecem ao menos se a classe é religiosa, a provisão inicial e o bônus ao crítico, lidos da classe e não copiados para o personagem.

### Edge Cases

- Se o download local encontrar classe, nível ou campo incompleto, a coleta para e o responsável decide caso a caso; não há preenchimento automático nem criação com catálogo parcial.
- A criação e a consulta do card nunca acessam a wiki; ausência de rede após o download não altera os atributos.
- Musqueteiro é reskin funcional do Besteiro: os números oficiais podem coincidir, mas cada classe deve ter o próprio registro de equipamento no catálogo.
- O usuário pode escolher nível de arma/armadura 5 com herói no nível 0. Esta feature respeita as escolhas informadas e aplica a tabela do equipamento escolhido; não simula a restrição da ferraria do jogo original.
- Acessórios, individualidades, doenças, luz da tocha, distrito e bônus de acampamento estão fora do cálculo. Se um personagem antigo tiver acessório, o card desta feature ainda mostra os atributos persistidos, sem recalcular trinkets.
- Personagens criados antes desta feature podem ter HP/velocidade/precisão incompatíveis com a wiki. O card mostra o que está gravado; não há recálculo silencioso de registros antigos.
- No jogo original, habilidades de acampamento não sobem de nível. O card ainda deve mostrar o nível persistido (0 bloqueada, 1 desbloqueada na criação).
- Uma classe sem os dois valores oficiais de deslocamento (frente e atrás) no catálogo local não pode ser usada na criação fiel.
- Resistências após o bônus de resolução não podem ultrapassar 100%.
- A Abominação possui dois blocos oficiais (humana e besta). A criação e o card usam somente a forma humana; a forma besta (transformação em combate) fica fora desta feature.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O sistema MUST baixar da wiki oficial, para as 20 classes, as tabelas de 5 níveis de arma e 5 níveis de armadura com os campos oficiais usados no cálculo (arma: dano mínimo, dano máximo, crítico, velocidade; armadura: HP máximo, esquiva) e MUST persistir esses valores no catálogo local.
- **FR-001a**: O download local MUST incluir também os demais dados oficiais da infobox da classe usados no card: se a classe é religiosa, a provisão inicial e o bônus ao crítico. Esses dados MUST permanecer no perfil da classe; MUST NOT ser copiados para o personagem na criação.
- **FR-002**: O sistema MUST baixar o deslocamento oficial de cada classe como dois valores (passos à frente e passos atrás), gravá-los localmente e atribuí-los ao personagem na criação. MUST NOT condensar o deslocamento em um único número.
- **FR-003**: O download MUST ser concluído e validado (cobertura completa das 20 classes no catálogo local) antes de a criação fiel ser considerada disponível.
- **FR-004**: A criação e a consulta MUST usar somente o catálogo local; MUST NOT consultar a wiki em tempo real.
- **FR-004a**: Se faltar qualquer informação oficial durante o download ou a validação, o trabalho MUST parar e o responsável MUST ser questionado caso a caso; MUST NOT inventar, interpolar ou completar valores automaticamente.
- **FR-005**: Na criação, o usuário MUST continuar informando somente nome, classe, nível do herói, nível da arma, nível da armadura e aparência.
- **FR-006**: O sistema MUST atribuir HP máximo ao valor oficial de HP da armadura da classe no nível escolhido, e HP atual igual ao HP máximo.
- **FR-007**: O sistema MUST atribuir esquiva ao valor oficial de esquiva da armadura no nível escolhido.
- **FR-008**: O sistema MUST atribuir velocidade, crítico, dano mínimo e dano máximo aos valores oficiais da arma da classe no nível escolhido.
- **FR-009**: O sistema MUST gravar o modificador de precisão do herói como 0 e a proteção como 0 na criação sem acessórios.
- **FR-010**: O sistema MUST calcular resistências efetivas assim: base oficial da classe para as seis resistências escaláveis, somando 10 pontos percentuais por nível de resolução do herói, com teto 100; golpe mortal e armadilha permanecem na base da classe, sem esse bônus.
- **FR-011**: O sistema MUST iniciar stress em 0 e chance de virtude em 25 na criação, salvo regra oficial já catalogada em contrário para a classe.
- **FR-012**: O sistema MUST NÃO aplicar acessórios, individualidades, doenças, distrito ou outros modificadores externos no cálculo desta feature.
- **FR-012a**: Para a Abominação, o download e a criação MUST usar exclusivamente os atributos oficiais da forma humana. MUST NOT gravar nem exibir no card os atributos da forma besta.
- **FR-013**: O sistema MUST manter a inicialização de habilidades da criação completa já especificada (quatro de combate e quatro de acampamento no nível 1; demais no nível 0).
- **FR-014**: A consulta usada pelo card MUST expor HP atual e máximo, stress, atributos de combate persistidos, as oito resistências e as habilidades com categoria e nível.
- **FR-015a**: O card MUST mostrar também nível do herói, nível da arma, nível da armadura e aparência.
- **FR-015b**: O card MUST mostrar os dados oficiais extras da classe (se é religiosa, provisão inicial e bônus ao crítico) lendo o catálogo da classe do personagem, sem exigir que esses campos existam no próprio personagem.
- **FR-015**: O card de personagem MUST apresentar as informações nas categorias: HP; Stress; Atributos; Resistências; Habilidades de combate; Habilidades de acampamento.
- **FR-016**: Na categoria Atributos, o card MUST mostrar precisão, proteção, esquiva, velocidade, crítico, dano mínimo–máximo, passos à frente e passos atrás.
- **FR-017**: O card MUST listar todas as habilidades de combate e todas as de acampamento do personagem, cada uma com o nível, inclusive as de nível 0.
- **FR-018**: Textos do card, do formulário e das mensagens de erro MUST estar em Português do Brasil.
- **FR-019**: O sistema MUST impedir persistência parcial se a derivação de atributos ou a validação do catálogo oficial falhar.

### Key Entities

- **Tabela oficial de arma da classe**: Cinco níveis com dano mínimo, dano máximo, crítico e velocidade, obtidos da wiki.
- **Tabela oficial de armadura da classe**: Cinco níveis com HP máximo e esquiva, obtidos da wiki.
- **Perfil de classe**: Resistências base, deslocamento (passos à frente e atrás), se é religiosa, provisão inicial, bônus ao crítico e habilidades já catalogadas; passa a incluir as tabelas de equipamento oficiais.
- **Personagem**: Herói persistido cujos atributos de combate, HP, passos à frente, passos atrás e resistências efetivas nascem das escolhas de classe e níveis, sem o usuário digitá-los.
- **Nível de resolução**: Nível do herói (0 a 6) usado no bônus de resistências; não substitui os níveis de arma e armadura.
- **Card de personagem**: Apresentação agrupada do herói na lista, com as seis categorias, as escolhas de criação e os dados oficiais extras da classe.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% das 20 classes possuem tabelas oficiais **locais** completas de arma e armadura (5 níveis cada) antes de a criação fiel ser oferecida ao usuário.
- **SC-002**: 100% das criações válidas de teste geram HP, esquiva, velocidade, crítico e dano iguais aos valores oficiais da arma/armadura escolhidas, sem o usuário informar esses campos.
- **SC-003**: Em uma amostra de pelo menos 3 classes e 3 combinações de nível de resolução, 100% das resistências escaláveis seguem base + 10 pontos percentuais por nível, com teto 100, e golpe mortal/armadilha permanecem na base.
- **SC-004**: 100% das criações fiéis gravam precisão 0, proteção 0 e stress 0 quando não há acessórios.
- **SC-005**: Nenhuma criação fiel é oferecida enquanto o catálogo local das 20 classes estiver incompleto; lacunas descobertas no download são interrompidas para decisão caso a caso, sem personagem parcial nem valor inventado.
- **SC-006**: Em 100% dos cards da lista, um avaliador encontra as seis categorias pedidas, o nível do herói, da arma, da armadura, a aparência, se a classe é religiosa, a provisão inicial, o bônus ao crítico e o nível de cada habilidade de combate e de acampamento, sem ação extra.
- **SC-007**: Um usuário completa a criação informando somente os seis campos essenciais e, no card resultante, reconhece HP, atributos e resistências coerentes com a classe e os níveis escolhidos na primeira visualização.

## Assumptions

- A fonte original é a wiki `darkestdungeon.wiki.gg`, nas páginas de cada classe (infobox de stats e seção de equipamento). Os números são baixados e passam a viver só no catálogo local; a criação não volta à wiki. Não há seed oficial de arma/armadura no catálogo atual.
- No Darkest Dungeon original, **sem acessórios**:
  - HP máximo e esquiva vêm da **armadura** no nível equipado, não de um HP base separado da classe.
  - Dano, crítico e velocidade vêm da **arma** no nível equipado.
  - O modificador de precisão do herói (ACC MOD) é 0; a precisão de cada ataque pertence à habilidade, não ao personagem.
  - Proteção base é 0.
  - O nível de resolução **não** aumenta HP, velocidade, dano, crítico nem esquiva; ele libera upgrades na ferraria/guilda e concede **+10%** (10 pontos percentuais) em atordoamento, sangramento, envenenamento, debuff, movimento e doença por nível. Golpe mortal e armadilha não recebem esse bônus.
- O “nível do herói” da criação continua sendo o nível de resolução (0 a 6). Os níveis de arma e armadura continuam independentes (1 a 5), como na criação completa já especificada.
- Tamanho 1 e 1 ação por turno permanecem os padrões de herói jogável.
- Chance de virtude inicial é 25%, padrão do jogo sem individualidades.
- Trinkets, individualidades, doenças, distrito, relíquias de conjunto, bônus temporários e a forma besta da Abominação estão fora do escopo. A Abominação é criada e exibida na forma humana (estado ao recrutar).
- Habilidades de acampamento no jogo original não têm upgrade; o nível no card reflete o estado persistido (bloqueada 0 / desbloqueada 1 na criação).
- Personagens antigos não são recalculados automaticamente.
- O download local completo (20 classes × 5 níveis de arma e armadura, mais passos à frente e atrás, se é religiosa, provisão inicial e bônus ao crítico de cada classe) é pré-requisito desta feature, não uma etapa em tempo de uso: sem cobertura 20×5 local, a criação fiel não é entregue.
- Religioso, provisão inicial e bônus ao crítico pertencem à classe. O card os lê do catálogo da classe; a criação não os duplica no personagem.
- Continua valendo a constituição do produto: regras de negócio fora da interface, persistência oficial, contratos verificáveis e textos em PT-BR.
