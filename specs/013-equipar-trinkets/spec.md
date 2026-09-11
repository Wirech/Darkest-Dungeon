# Feature Specification: Equipar Trinkets no Personagem

**Feature Branch**: `013-equipar-trinkets`

**Created**: 2026-09-11

**Status**: Draft

**Input**: User description: "agora quero um foco nos trinkets. um personagem deve, depois de ser criado, poder equipar trinkets. para isso, preciso que primeiro vc minere da wiki todos os trinkets que conseguir, ajuste a entidade "acessórios" pra conseguir comportar os bônus e a entidade personagem para conseguir equipar até 2 trinkets. no card do personagem deve ter dois espaços pra selecionar um trinket. escolher um trinket em qualquer espaço adiciona á ficha base os bõnus dele, enquanto remover faz o oposto. para não haver confusão, essa mudança deve ser feita de forma segura."

## Contexto de origem

O catálogo já trata **trinket** como **Acessório** (Feature 003): raridade fechada, efeitos estruturados (nome, valor, unidade, sinal), classe exclusiva opcional e `ConjuntoId` só como metadado. O Personagem já reserva **dois identificadores** de acessório equipado, mas a **criação fiel** (Features 008/009) nasce **sem** trinkets e o card **não** oferece seleção nem aplica bônus.

A Feature 006 importa mídias e pode criar acessórios novos a partir da instalação (`Comum`, efeitos vazios). Esta feature **não** reabre ícones nem Spine: o valor é o **catálogo de bônus oficiais** e o **equipar/desequipar seguro no card**, depois que o herói já existe.

A Feature 009 deixou trinkets **fora** do cálculo de criação. Esta feature **não** recalcula a criação: a ficha **base** permanece a da criação; os trinkets entram como **modificadores reversíveis** em cima dessa base.

## Clarifications

### Session 2026-09-11

- Q: Nos dois espaços do card, a lista de trinkets deve mostrar só o que aquele herói pode usar, ou todos os do catálogo (recusando os inválidos só na hora de confirmar)? → A: Só trinkets permitidos para a classe daquele herói (sem restrição de classe, ou exclusivos da classe dele). Recusa em PT-BR continua para repetir o mesmo nos dois espaços ou dado inválido.
- Q: Quando a wiki descreve um bônus em percentual (por exemplo +10% de HP ou +10% de proteção), como isso entra na ficha efetiva? → A: Depende do atributo: HP e dano = % da ficha base; proteção, crítico, resistências e chance de virtude = pontos percentuais somados; precisão, esquiva e velocidade seguem pontos da wiki.
- Q: A coleta da wiki deve incluir trinkets de DLC (Crimson Court, Color of Madness / Crystalline e similares), ou só os do jogo base? → A: Todos os trinkets que a wiki listar, inclusive DLC; página incompleta vira lacuna, sem inventar efeito.
- Q: No card, o usuário deve ver só a ficha efetiva (já com trinkets), ou também os valores da ficha base da criação? → A: Ficha efetiva em destaque e ficha base visível no mesmo card (para comparar).
- Q: Quando um % da ficha base gera número quebrado (ex.: 23 HP + 10% = 2,3), o valor efetivo deve ser arredondado como? → A: Sempre para cima (teto) nos atributos inteiros da ficha, depois da soma exata.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Completar o catálogo de trinkets a partir da wiki (Priority: P1)

Como responsável pelo catálogo, quero baixar da wiki oficial o maior conjunto possível de trinkets (nome, raridade, restrição de classe quando houver, e cada bônus/penalidade com valor) e gravá-los como Acessórios identificáveis, para que o card tenha o que selecionar sem inventar efeito.

**Why this priority**: Sem catálogo com bônus reais, os dois espaços do card ficam vazios ou com acessórios sem efeito (como os criados só pela instalação). A coleta é o MVP independente: o curador já consulta o inventário de trinkets mesmo antes de equipar.

**Independent Test**: Executar a coleta, conferir que dezenas de trinkets oficiais entram com nome rastreável até a wiki, raridade válida e pelo menos um efeito estruturado quando a wiki publica bônus; lacunas (página incompleta) ficam registradas e nenhum valor é inventado.

**Acceptance Scenarios**:

1. **Given** a wiki oficial de trinkets, **When** a coleta é executada, **Then** cada trinket obtido vira um Acessório com nome de exibição, nome original, descrição, raridade suportada e lista de efeitos (positivos e negativos) extraída da fonte.
2. **Given** um trinket já existente no catálogo (seed 003 ou criação 006) com o mesmo nome original, **When** a coleta encontra a página correspondente, **Then** o registro existente é atualizado com os bônus oficiais **sem** trocar o identificador.
3. **Given** um trinket da wiki ainda ausente no catálogo, **When** a coleta termina, **Then** um Acessório novo é criado com identificador próprio, sem recriar nem fundir registros 003 por engano.
4. **Given** uma página sem bônus legível ou com campo obrigatório ausente, **When** a coleta processa esse item, **Then** a lacuna é registrada (trinket, campo, motivo observável) e o item **não** recebe valor inventado.
5. **Given** uma segunda execução da coleta com as mesmas páginas, **When** ela termina, **Then** não há duplicata de nome original e os identificadores anteriores permanecem.
6. **Given** o catálogo após a coleta, **When** o responsável consulta um trinket, **Then** consegue rastrear a origem (wiki) e distinguir efeito positivo de negativo.
7. **Given** trinkets de DLC listados na wiki (Crimson Court, Color of Madness / Crystalline e similares), **When** a coleta é executada, **Then** esses itens entram no catálogo com a raridade suportada correspondente; página incompleta é lacuna, não exclusão silenciosa nem valor inventado.

---

### User Story 2 - Equipar e desequipar até dois trinkets no card (Priority: P1)

Como usuário do catálogo, quero, no card de um personagem **já criado**, dois espaços para escolher trinkets. Escolher em qualquer espaço aplica os bônus na ficha **visível**; remover desfaz exatamente esses bônus, sem bagunçar a ficha da criação.

**Why this priority**: É o valor jogável. Depende do catálogo (P1 da US1) para ter opções reais, mas pode ser demonstrado com dois Acessórios de teste se a coleta ainda estiver parcial.

**Independent Test**: Abrir o card de um herói sem trinket, selecionar um em cada espaço, conferir que os atributos efetivos mudam pela soma dos bônus; esvaziar um espaço e conferir que só os bônus daquele trinket saem; recarregar o card e ver as mesmas escolhas.

**Acceptance Scenarios**:

1. **Given** um personagem já criado, **When** o usuário abre o card, **Then** há exatamente dois espaços de trinket, inicialmente vazios se nada estiver equipado, independentes um do outro.
2. **Given** um espaço vazio, **When** o usuário abre a lista daquele espaço, **Then** aparecem somente trinkets que o herói pode usar (sem restrição de classe, ou exclusivos da classe dele); trinkets exclusivos de outra classe **não** entram na lista.
3. **Given** um espaço vazio e um trinket permitido para a classe do herói, **When** o usuário seleciona esse trinket naquele espaço, **Then** a ficha **efetiva** passa a incluir todos os bônus e penalidades daquele trinket somados à ficha base, e a escolha fica gravada nesse espaço.
4. **Given** um trinket já escolhido em um espaço, **When** o usuário remove a seleção (espaço vazio), **Then** os bônus daquele trinket deixam de aparecer na ficha efetiva e a ficha base da criação permanece intacta.
5. **Given** os dois espaços, **When** o usuário escolhe trinkets diferentes em cada um, **Then** a ficha efetiva reflete a **soma** dos efeitos dos dois, sem que um espaço substitua o outro.
6. **Given** uma seleção válida, **When** o usuário recarrega o card, **Then** os dois espaços, a ficha efetiva em destaque e a ficha base visível continuam iguais ao último estado gravado.
7. **Given** um trinket exclusivo de outra classe (fora da lista), **When** uma tentativa inválida chega mesmo assim (dado adulterado), **Then** a operação é recusada com mensagem em PT-BR e nenhum espaço nem atributo muda.
8. **Given** o mesmo trinket já ocupando um espaço, **When** o usuário tenta colocá-lo no outro, **Then** a operação é recusada com mensagem em PT-BR (os dois espaços não repetem o mesmo acessório); esse trinket **não** reaparece como opção no espaço ainda vazio.

---

### User Story 3 - Aplicar e reverter bônus de forma segura (Priority: P1)

Como usuário, quero que equipar e remover trinket nunca “queime” a ficha base nem deixe atributo pela metade se a operação falhar, para não haver confusão entre o herói nascido na criação e o herói com trinkets.

**Why this priority**: O pedido exige mudança segura. Sem essa regra, somar bônus em cima de valores já persistidos tornaria o desequipar irreversível ou cumulativo.

**Independent Test**: Anotar HP, precisão, proteção, esquiva, velocidade, crítico, dano e resistências **antes** de qualquer trinket; equipar dois; desequipar os dois; conferir que os valores **base** voltaram ao registro da criação. Forçar uma falha no meio da troca e conferir que não ficou um espaço com trinket e a ficha sem (ou o contrário).

**Acceptance Scenarios**:

1. **Given** a ficha gravada na criação (sem trinkets), **When** trinkets são equipados ou removidos, **Then** os valores **base** persistidos da criação não são reescritos; o card mostra a ficha **efetiva** em destaque (base + efeitos dos espaços ocupados) e a ficha **base** visível para comparar.
2. **Given** um herói com um ou dois trinkets, **When** todos os espaços são esvaziados, **Then** a ficha efetiva coincide de novo com a ficha base da criação, atributo a atributo cobertos por esta feature.
3. **Given** uma tentativa de troca que falha (trinket inválido, personagem inexistente, conflito), **When** a resposta chega, **Then** nenhum espaço é alterado e a ficha efetiva permanece a de antes da tentativa.
4. **Given** um efeito cujo atributo existe na ficha (por exemplo precisão, proteção, HP máximo, resistência), **When** o trinket é equipado, **Then** o card mostra o valor efetivo já com esse efeito aplicado, com a regra de unidade do atributo (HP/dano em % da base; proteção/crítico/resistências/virtude em pontos percentuais; precisão/esquiva/velocidade em pontos), sinal correto (bônus soma, penalidade subtrai) e, se a soma gerar fração em atributo inteiro, arredondamento sempre para cima (teto).
5. **Given** um efeito da wiki que **não** corresponde a nenhum campo da ficha exibida, **When** o trinket é equipado, **Then** o efeito continua visível na ficha do acessório, **não** é inventado como atributo novo e **não** altera em silêncio um campo errado.
6. **Given** dois usuários ou dois cliques rápidos no mesmo card, **When** uma operação ainda está em andamento, **Then** a interface impede envio duplicado e o estado final é o da última operação **concluída** com sucesso.

---

### Edge Cases

- Personagem criado antes desta feature, sem trinkets: os dois espaços nascem vazios; a ficha base permanece a já gravada (mesmo que a 009 não a tenha recalculado).
- Personagem que já tenha identificadores de acessório preenchidos por fluxo antigo: o card mostra esses trinkets se ainda existirem no catálogo; se o acessório foi apagado, o espaço aparece vazio e a ficha efetiva ignora o identificador órfão, sem erro fatal.
- Trinket com classe exclusiva igual à do herói: permitido e **aparece** na lista dos dois espaços. Sem classe exclusiva: permitido a qualquer classe e **aparece** na lista. Exclusivo de outra classe: **não** aparece na lista.
- Dois trinkets do mesmo conjunto: cada um aplica só os próprios efeitos; bônus de conjunto (duas peças) **não** entra nesta feature.
- Dois efeitos percentuais no mesmo atributo (ex.: dois +10% de HP) aplicam-se **cada um sobre a ficha base** e depois somam; não há juros sobre juros (30 HP + 10% + 10% = 36, não 36,3).
- Fração no valor efetivo de atributo inteiro (HP máximo, dano mín/máx e demais campos inteiros da ficha): soma-se em exato e **arredonda sempre para cima** (teto). Ex.: 23 HP + 10% = 25,3 → **26**. Inteiro permanece inteiro. O teto aplica-se ao valor efetivo **depois** da soma, não a cada trinket isolado (23 HP + 10% + 10% = 27,6 → **28**).
- HP efetivo abaixo de 1 ou resistência efetiva fora de 0–100: a exibição e a regra de negócio **limitam** resistência a 0–100; HP máximo efetivo não pode ficar abaixo de 1; HP atual não sobe automaticamente ao equipar HP extra, nem é reduzido abaixo de 1 ao remover (HP atual permanece o persistido, limitado ao HP máximo **efetivo** na exibição).
- Trocar o trinket de um espaço (A → B) é uma única operação segura: remove A e aplica B, ou nada muda.
- Lista de seleção vazia (catálogo sem trinkets ainda, ou nenhum permitido para a classe): os espaços existem, informam que não há trinkets disponíveis, e não quebram o card.
- Nome original da wiki em inglês permanece para rastreio; textos do card, rótulos dos espaços e erros em PT-BR.
- Coleta parcial da wiki: o usuário só pode selecionar trinkets **já** no catálogo; a ausência de um trinket famoso não inventa placeholder.
- Criação de personagem **não** ganha seleção inicial de trinket (continua o fluxo 008/009).
- Ícones, Spine e mídia de trinket (Features 006/011) não são reabertos: a seleção funciona com nome/raridade mesmo sem ícone.
- Espaços vazios: ficha efetiva e ficha base coincidem atributo a atributo; as duas continuam visíveis no card, com a efetiva em destaque.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O sistema MUST coletar da wiki oficial o maior conjunto possível de trinkets, **inclusive DLC** (Crimson Court, Color of Madness / Crystalline e similares listados na mesma fonte), gravando para cada item obtido: nome de exibição, nome original, descrição, raridade do conjunto fechado já adotado (Comum, Incomum, Rara, MuitoRara, CrimsonCourt, Crystalline, Set), classe exclusiva quando a wiki restringir, e a lista de efeitos. MUST NOT restringir a coleta ao jogo base.
- **FR-002**: Cada efeito MUST ter nome identificável, valor numérico, unidade (percentual ou pontos, conforme a wiki) e sinal (bônus ou penalidade). O sistema MUST NOT inventar valor, raridade ou restrição quando a wiki não informar.
- **FR-003**: A coleta MUST ser idempotente: reexecução não duplica trinket com o mesmo nome original; registros já existentes MUST ser atualizados no lugar, preservando identificador.
- **FR-004**: Lacunas de coleta MUST ser registradas com trinket ou página, campo ausente, motivo observável e momento; a coleta MUST continuar para os demais itens.
- **FR-005**: Acessório MUST comportar a lista completa de bônus e penalidades oficiais de um trinket (não apenas metadado vazio). MUST NOT alterar identificadores, raridade já curada além da atualização oficial, nem reclassificar Arma/Armadura.
- **FR-006**: Personagem MUST permitir no máximo **dois** trinkets equipados, em dois espaços distintos, ambos opcionais. A criação de personagem MUST continuar sem exigir trinket.
- **FR-007**: O card do personagem MUST exibir dois espaços de seleção de trinket, utilizáveis depois que o herói existe. Qualquer um dos dois espaços MUST poder receber um trinket permitido, independentemente do outro estar vazio ou ocupado.
- **FR-007a**: A lista de cada espaço MUST mostrar somente trinkets que aquele herói pode usar: sem restrição de classe, ou exclusivos da classe do personagem. MUST NOT listar trinkets exclusivos de outra classe. O acessório já ocupando o outro espaço MUST NOT reaparecer como opção. Recusa em PT-BR permanece para repetir o mesmo acessório, trinket inexistente, personagem inexistente ou dado adulterado.
- **FR-008**: Selecionar um trinket em um espaço MUST persistir essa escolha nesse espaço e MUST refletir na ficha **efetiva** a soma dos efeitos daquele trinket sobre a ficha **base**.
- **FR-009**: Remover o trinket de um espaço MUST persistir o espaço vazio e MUST retirar da ficha efetiva exatamente os efeitos daquele trinket.
- **FR-010**: A ficha **base** (valores gravados na criação e nas atualizações que não sejam trinket) MUST permanecer inalterada ao equipar ou remover trinket. A ficha **efetiva** MUST ser sempre base + efeitos dos espaços ocupados no momento da consulta.
- **FR-011**: A operação de equipar, trocar ou remover MUST ser atômica por personagem: ou os dois espaços e a consulta efetiva ficam consistentes, ou o estado anterior é preservado. MUST NOT aplicar bônus sem gravar o espaço, nem gravar o espaço sem os bônus visíveis na consulta.
- **FR-012**: O sistema MUST recusar, com mensagem em PT-BR, trinket exclusivo de outra classe, trinket inexistente, personagem inexistente e a repetição do mesmo acessório nos dois espaços.
- **FR-013**: Efeitos cujo atributo existe na ficha (ao menos: HP máximo, precisão, proteção, esquiva, velocidade, crítico, dano mínimo, dano máximo, chance de virtude, e as oito resistências) MUST entrar no valor efetivo correspondente, respeitando o sinal. Resistências efetivas MUST permanecer entre 0 e 100. HP máximo efetivo MUST ser no mínimo 1.
- **FR-013a**: A unidade de um efeito MUST seguir o atributo, não um único significado de “%”: HP máximo, dano mínimo e dano máximo usam percentual **da ficha base**; proteção, crítico, as oito resistências e chance de virtude somam **pontos percentuais** (0 PROT + 10% = 10); precisão, esquiva e velocidade usam **pontos** quando a wiki lista pontos. Vários efeitos no mesmo atributo MUST ser calculados cada um contra a ficha base e depois somados (sem aplicar % em cima do resultado do outro trinket).
- **FR-013b**: Se a soma exata de um atributo **inteiro** da ficha (ao menos HP máximo, dano mínimo e dano máximo) não for inteira, o valor efetivo exibido MUST ser o teto matemático (sempre para cima; qualquer fração sobe 1). O arredondamento MUST ocorrer **depois** da soma de todos os efeitos daquele atributo, não por trinket. Inteiro permanece inteiro. Limites de FR-013 (HP máximo efetivo ≥ 1; resistências 0–100) aplicam-se ao valor já arredondado.
- **FR-014**: Efeitos sem campo correspondente na ficha MUST permanecer listados no trinket e MUST NOT ser mapeados para um atributo errado.
- **FR-015**: O card MUST exibir a ficha **efetiva** em destaque (a ficha que o usuário lê ao escolher) e MUST exibir no mesmo card a ficha **base** da criação, visível para comparar. MUST NOT ocultar a base depois de equipar. MUST NOT apresentar sucesso se a gravação falhou.
- **FR-016**: Textos de interface, rótulos dos dois espaços, estados vazio/ocupado e mensagens de erro MUST estar em Português do Brasil. Nomes originais de trinket podem permanecer para rastreio.
- **FR-017**: Esta feature MUST NOT alterar equipamento de arma/armadura, habilidades, aparência, criação fiel, publicação 005, nem exigir janela de manutenção para uma troca de trinket no card.
- **FR-018**: Bônus de conjunto (duas peças do mesmo conjunto) MUST permanecer fora do cálculo. `ConjuntoId` existente MAY ser preservado como metadado, sem efeito automático.

### Key Entities

- **Trinket / Acessório**: Item de catálogo (troféu) com raridade, restrição opcional de classe, conjunto opcional (metadado) e lista de efeitos oficiais. É o que aparece na seleção dos dois espaços.
- **Efeito de acessório**: Um bônus ou penalidade com nome, valor, unidade e sinal, proveniente da wiki, aplicável à ficha efetiva quando houver atributo correspondente.
- **Espaço de trinket**: Um dos dois encaixes do personagem (primeiro e segundo). Vazio ou ocupado por no máximo um acessório. Os espaços são equivalentes em regra (qualquer um aplica o trinket escolhido).
- **Ficha base**: Atributos persistidos do personagem sem trinkets (criação 009 e demais fluxos que não sejam esta feature).
- **Ficha efetiva**: Ficha base mais a soma dos efeitos dos trinkets atualmente nos dois espaços; é o valor em destaque no card após selecionar. A ficha base permanece visível no mesmo card.
- **Personagem**: Herói já criado; esta feature só acrescenta o uso dos dois espaços e a consulta efetiva. Não nasce com trinket.
- **Lacuna de coleta**: Registro de trinket ou campo que a wiki não entregou de forma utilizável.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Após a coleta, uma amostra de 20 trinkets da wiki (mistura de comuns e raros, com e sem restrição de classe, **incluindo pelo menos um de DLC** quando a wiki o listar) está no catálogo com nome rastreável, raridade válida e efeitos estruturados quando a wiki os publica; 0 de 20 têm valor inventado.
- **SC-002**: Reexecutar a coleta sobre o mesmo conjunto não aumenta o número de acessórios com o mesmo nome original (0 duplicatas).
- **SC-003**: 100% dos personagens consultados no card exibem exatamente dois espaços de trinket.
- **SC-004**: Em 10 tentativas de selecionar um trinket permitido em um espaço vazio, 10 atualizam a ficha efetiva em destaque na mesma consulta e mantêm a ficha base visível e inalterada (o usuário vê o bônus e a comparação sem recálculo manual).
- **SC-005**: Após equipar um trinket e em seguida removê-lo, 100% dos atributos da ficha efetiva voltam aos valores da ficha base medidos antes da seleção (tolerância zero em campos cobertos por FR-013). Um caso amostral de HP +10% e outro de PROT +10% (base 0) confirmam percentual-da-base vs pontos percentuais. Um caso com fração (23 HP + 10% → 26) confirma o teto.
- **SC-006**: Equipar dois trinkets distintos e depois esvaziar os dois espaços devolve a ficha efetiva à base em 100% dos atributos cobertos.
- **SC-007**: 100% das tentativas inválidas (classe exclusiva errada via dado adulterado, repetir o mesmo acessório nos dois espaços, trinket inexistente) são recusadas em PT-BR e deixam espaços e ficha efetiva inalterados. Em uso normal, 100% das listas dos espaços omitem trinkets exclusivos de outra classe.
- **SC-008**: Uma falha simulada no meio da troca não deixa estado pela metade: ou o par de espaços anterior permanece, ou o novo par completo; 0 casos híbridos.
- **SC-009**: O usuário completa “abrir card → escolher trinket no espaço 1 → ver ficha efetiva” em até 1 minuto no volume atual do catálogo.
- **SC-010**: 100% dos rótulos dos espaços, estados vazios e mensagens de recusa estão em Português do Brasil.

## Assumptions

- A fonte oficial de texto e bônus é a wiki já usada nas features anteriores (`darkestdungeon.wiki.gg`); a coleta **não** ocorre em tempo real no uso diário do card — o card lê só o catálogo local.
- “Todos os trinkets que conseguir” significa melhor esforço **incluindo DLC** listado na wiki (Crimson Court, Color of Madness / Crystalline e similares): cobertura máxima com lacunas registradas, **não** o bloqueio total da 009 (que parava a criação se faltasse uma classe). O card funciona com o subconjunto já gravado. Trinket de DLC incompleto é lacuna, não fica de fora só por ser DLC.
- Trinket e Acessório são o mesmo conceito de produto; não se cria um terceiro tipo de item.
- Os dois espaços já previstos no Personagem (003) são os únicos; não há terceiro trinket nem trinket de acampamento.
- A ficha base **não** é regravada ao equipar (evita confusão e torna o desequipar reversível). No card a efetiva fica em destaque e a base permanece visível para comparar. HP atual persistido não é “curado” por trinket de HP.
- Criação de personagem permanece sem seletor de trinket.
- Bônus de conjunto, doenças, individualidades, luz da tocha e distrito continuam fora.
- Mídias de ícone de trinket (006) são opcionais na seleção; ausência de ícone não impede escolher pelo nome.
- Restrição de classe segue a 003: só quando o acessório é exclusivo.
- Não se pode repetir o **mesmo** acessório nos dois espaços; dois acessórios diferentes são permitidos mesmo que a wiki descreva sinergia de conjunto (sinergia não calculada).
- Unidade percentual vs pontos segue o atributo (FR-013a) e o que a wiki indica; na dúvida o item entra como lacuna, não como chute. “%” em PROT/crítico/resistência/virtude **não** multiplica um zero da base. Fração em atributo inteiro da ficha efetiva usa teto depois da soma (FR-013b).
- Mensagens e rótulos em PT-BR; nomes de trinket da wiki podem ficar bilingues (exibição + original), no padrão do catálogo.

## Out of Scope

- Recalcular a criação do personagem com trinkets pré-selecionados.
- Bônus de conjunto, loja, custo em ouro, desbloqueio por área ou raridade “ainda não dropada”.
- Combate, inimigos, individualidades, doenças, distrito, luz da tocha.
- Reabrir coleta de ícones/`panels/icons_equip` ou Spine de trinket.
- Alterar arma, armadura, habilidades ou aparência pelo seletor de trinket.
- Publicador atômico 005 / janela de manutenção.
- Inventar trinkets que a wiki não listou.
- Excluir da coleta trinkets de DLC que a wiki listar (eles entram no mesmo melhor esforço; bônus de conjunto continua fora do cálculo).

## Referência cruzada

- Feature 003: Acessório, dois espaços no Personagem, recusa de classe exclusiva, conjunto só metadado.
- Feature 006: mídias e acessórios novos da instalação (efeitos vazios até esta curadoria).
- Feature 008: criação sem seleção inicial de acessório.
- Feature 009: ficha base oficial **sem** trinkets; card por categorias; esta feature só soma modificadores reversíveis.
- Feature 011: ícones de acampamento; trinkets de `icons_equip` continuam fora.
