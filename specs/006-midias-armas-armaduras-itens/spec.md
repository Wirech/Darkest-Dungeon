# Feature Specification: Importação e Vínculo de Mídias de Armas, Armaduras e Itens

**Feature Branch**: `006-midias-armas-armaduras-itens`

**Created**: 2026-09-10

**Status**: Draft

**Input**: User description: "importar e vincular mídias/ícones de armas, armaduras, itens de acampamento, troféus e consumíveis — expandindo o modelo da Feature 003 e reutilizando o inventário da Feature 004"

## Contexto de origem

A Feature 005 (Q2 da 3ª sessão de clarificação, 2026-09-08) excluiu explicitamente mídias de `Arma`, `Armadura` e `Item` do seu escopo e registrou este trabalho como Feature 006. A Feature 004 limita-se a assets de heróis e exclui inimigos e itens. A Feature 003 já cataloga 20 armas e 20 armaduras com cinco níveis cada, além de acessórios (troféus), mas sem qualquer referência visual.

Esta feature preenche essa lacuna: descobrir, inventariar e vincular as mídias locais de equipamento e itens ao catálogo já existente, sem duplicar arquivos nem alterar os identificadores dos registros da Feature 003.

## Clarifications

### Session 2026-09-10

- Q: Como o catálogo deve representar itens de acampamento/provisões e consumíveis que ainda não existem como tipo próprio na Feature 003? → A: Dois tipos novos no catálogo: item de acampamento/provisão e consumível, cada um identificável, sem misturar com acessório.
- Q: O catálogo de acessórios (troféus) desta feature deve se limitar aos já semeados na Feature 003, ou também criar registros novos para troféus encontrados na instalação e ainda ausentes no catálogo? → A: Vincular os existentes e criar acessórios novos para troféus encontrados na instalação e ainda ausentes no catálogo.
- Q: Quando um acessório novo for criado a partir de um trinket encontrado só na instalação, quais dados de catálogo ele deve ter além da mídia? → A: Nome, descrição e mídia; raridade padrão `Comum`; efeitos, classe exclusiva e conjunto vazios até curadoria futura.
- Q: Como o sistema deve decidir se um arquivo descoberto é item de acampamento/provisão ou consumível? → A: Pela pasta/origem na instalação local (cada origem mapeia a um tipo).
- Q: A publicação dos vínculos precisa ocorrer fora do horário das sessões ao vivo, como na Feature 005, ou pode rodar a qualquer momento desde que seja atômica por categoria? → A: A qualquer momento, atômica por categoria, sem mexer no equipamento já atribuído a Personagens.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Inventariar mídias locais de equipamento e itens (Priority: P1)

Como curador do acervo, quero descobrir na instalação local licenciada do jogo os arquivos de mídia de armas, armaduras, itens de acampamento/provisões, troféus (acessórios) e consumíveis, e registrá-los em um inventário rastreável — origem, destino no acervo, hash, tamanho e tipo — sem baixar conteúdo externo e sem alterar os bytes originais.

**Why this priority**: Sem inventário não há o que vincular. É o MVP independente: mesmo sem associação ao catálogo 003, o curador já sabe quais arquivos existem, quais faltam e de onde vieram.

**Independent Test**: Com um diretório de teste contendo pelo menos um ícone de arma, um de armadura e um de item, executar a descoberta e confirmar que cada arquivo copiado aparece no inventário com origem local, hash idêntico ao original e categoria (arma, armadura, acampamento, troféu ou consumível).

**Acceptance Scenarios**:

1. **Given** uma instalação local com arquivos de equipamento e itens, **When** o curador executa a descoberta, **Then** cada arquivo encontrado é copiado sem alteração de bytes e consta no inventário com caminho-fonte, hash, tamanho e categoria.
2. **Given** dois arquivos com conteúdo idêntico em pastas distintas, **When** a descoberta é executada, **Then** o inventário evita cópia redundante e registra as duas associações sobre o mesmo conteúdo.
3. **Given** uma pasta esperada ausente na instalação, **When** a descoberta é executada, **Then** a lacuna é registrada com o caminho consultado, o motivo observável e a data da tentativa, sem interromper o restante da importação.
4. **Given** o inventário gerado, **When** o curador consulta um arquivo importado, **Then** consegue rastrear a origem local licenciada, o hash e o destino no acervo.

---

### User Story 2 - Vincular armas e armaduras do catálogo às mídias por nível (Priority: P2)

Como curador do catálogo, quero que cada uma das 20 armas e 20 armaduras já existentes (Feature 003) receba um vínculo visual para cada um dos cinco níveis (1 a 5), apontando para o inventário desta feature (ou reutilizando o inventário da Feature 004 quando o arquivo já existir lá), sem duplicar bytes e sem alterar os identificadores das armas e armaduras.

**Why this priority**: Armas e armaduras já têm modelo e seed estáveis; o valor imediato para o jogador é ver o ícone/sprite do equipamento do Personagem. Depende do inventário (P1), mas pode ser demonstrada só com as 20 classes sem tocar em troféus ou consumíveis.

**Independent Test**: Escolher uma classe (por exemplo Cruzado), aplicar os vínculos dos 5 níveis de arma e 5 de armadura, consultar a arma e a armadura dessa classe e verificar que cada nível aponta para um registro de mídia com hash, sem mudança de identificador nem da lista de cinco níveis numéricos.

**Acceptance Scenarios**:

1. **Given** as 20 armas e 20 armaduras já catalogadas, **When** o curador publica os vínculos, **Then** cada arma e cada armadura mantém exatamente cinco níveis numéricos e passa a expor, por nível, a referência de mídia correspondente (ou o estado `Pendente` se o arquivo não foi encontrado).
2. **Given** um nível de arma cujo arquivo existe no inventário, **When** um usuário consulta essa arma, **Then** o nível apresenta a mídia resolvida (ícone estático e, se houver, conjunto animado associado) sem copiar o arquivo de novo.
3. **Given** um nível de armadura sem arquivo correspondente, **When** o curador consulta o mapa de cobertura, **Then** aquela combinação classe × nível aparece como `Pendente` e os demais níveis da mesma armadura não são afetados.
4. **Given** Personagens já criados apontando para armas e armaduras existentes, **When** os vínculos de mídia são publicados, **Then** os Personagens continuam existindo com os mesmos identificadores de equipamento e passam a poder resolver a mídia do nível atual.

---

### User Story 3 - Vincular troféus, itens de acampamento e consumíveis (Priority: P3)

Como curador do catálogo, quero que os acessórios (troféus) já modelados e os itens de acampamento/provisões e consumíveis do jogo recebam vínculos de mídia no mesmo inventário. Provisões e consumíveis entram como **dois tipos novos** do catálogo (irmãos de Arma, Armadura e Acessório). Troféus encontrados na instalação e ainda ausentes no catálogo 003 entram como **acessórios novos**, sem recriar nem alterar os já semeados.

**Why this priority**: Completa o conjunto de equipamentos e suprimentos visíveis ao jogador, mas não bloqueia o MVP de arma/armadura. Troféus já têm tipo (`Acessório`); provisões e consumíveis passam a ter tipos próprios, sem reabrir combate, inimigos ou status.

**Independent Test**: Selecionar um acessório já catalogado, um item de acampamento típico (por exemplo tocha ou bandagem) e um consumível, vincular cada um a um arquivo do inventário e confirmar que a consulta do item devolve a mídia e que itens sem arquivo ficam `Pendente` no relatório.

**Acceptance Scenarios**:

1. **Given** acessórios já catalogados na Feature 003, **When** o curador publica os vínculos, **Then** cada acessório com arquivo encontrado recebe uma referência de mídia; os sem arquivo ficam `Pendente`; identificadores, raridade, efeitos e classe exclusiva dos registros 003 permanecem inalterados.
2. **Given** um arquivo numa origem da instalação mapeada como acampamento/provisão, **When** ele é inventariado, **Then** o sistema o reconhece como `Item de acampamento/provisão` identificável (nome, descrição e mídia), distinto de arma, armadura, acessório e consumível.
3. **Given** um arquivo numa origem da instalação mapeada como consumível, **When** o vínculo é publicado, **Then** o sistema o reconhece como `Consumível` identificável, a consulta devolve a mídia e o estado de cobertura (`OK` ou `Pendente`).
4. **Given** um arquivo de troféu (trinket) na instalação sem acessório correspondente no catálogo 003, **When** a descoberta e a publicação terminam, **Then** o sistema cria um acessório novo com nome e descrição derivados do arquivo, raridade `Comum`, efeitos vazios, sem classe exclusiva nem conjunto, vincula a mídia e o inclui na cobertura da categoria acessório/troféu.
5. **Given** um arquivo de item que não pode ser classificado como arma, armadura, acessório, acampamento/provisão ou consumível, **When** a descoberta termina, **Then** o arquivo permanece no inventário como não associado e aparece no relatório de cobertura, sem ser descartado em silêncio.

---

### User Story 4 - Relatório de cobertura das mídias de equipamento e itens (Priority: P2)

Como curador, quero um relatório por categoria (arma, armadura, acessório/troféu, acampamento/provisão, consumível) que marque cada item esperado como `OK`, `Parcial` ou `Pendente`, no mesmo espírito da auditoria visual da Feature 005, para saber o que ainda falta sem inspecionar pasta por pasta.

**Why this priority**: Sem cobertura mensurável o curador não fecha a feature nem prioriza lacunas. É útil assim que P1 existir (inventário + lacunas); fica completo depois de US2 (arma/armadura) e US3 (acessório/acampamento/consumível).

**Independent Test**: Gerar o relatório após uma importação parcial (algumas armas com ícone, outras sem) e conferir totais: quantidade esperada por categoria, quantidade `OK`, quantidade `Pendente`, e que a soma bate com o esperado.

**Acceptance Scenarios**:

1. **Given** inventário e vínculos publicados, **When** o curador solicita o relatório de cobertura, **Then** cada categoria lista totais `OK`, `Parcial` e `Pendente` em português do Brasil.
2. **Given** uma arma com os cinco níveis vinculados a arquivos existentes, **When** o relatório é gerado, **Then** essa arma aparece como `OK`.
3. **Given** uma arma com apenas parte dos níveis vinculados, **When** o relatório é gerado, **Then** essa arma aparece como `Parcial` e o relatório indica quais níveis faltam.
4. **Given** um item de catálogo sem nenhum arquivo associado, **When** o relatório é gerado, **Then** o item aparece como `Pendente` e o caminho esperado consultado é registrado quando conhecido.

---

### Edge Cases

- Arquivo esperado ausente na instalação: o catálogo permanece válido; o vínculo fica `Pendente`; a descoberta continua para os demais arquivos.
- Arquivo presente no inventário da Feature 004 (pasta de heróis) que na verdade é ícone de equipamento: reutilizar a entrada existente por hash; não duplicar bytes.
- Conteúdo idêntico com nomes diferentes: uma única cópia no acervo e múltiplas associações no inventário.
- Instalação local incompleta (DLC ausente): registrar lacunas das categorias afetadas; não falhar a importação inteira.
- Tentativa de redistribuir ou baixar mídias de sites externos: fora de política; a feature só opera sobre instalação local licenciada.
- Publicação durante sessão ao vivo: permitida a qualquer momento; ou todos os vínculos de uma categoria são aplicados, ou nenhum; Personagens existentes não perdem nem trocam equipamento. Não há janela de manutenção obrigatória.
- Arma ou armadura cujo seed 003 não muda de identificador: vínculos novos não recriam o item nem alteram os cinco níveis numéricos.
- Troféu (trinket) encontrado na instalação sem acessório no catálogo 003: cria-se um acessório novo com nome, descrição, mídia, raridade `Comum` e efeitos/classe exclusiva/conjunto vazios; os acessórios já semeados não são substituídos nem têm raridade alterada.
- Arquivo de inventário que não se classifica em nenhuma das cinco categorias: permanece órfão no inventário e no relatório; não é apagado.
- Consulta de Personagem cujo nível de arma/armadura está `Pendente`: o Personagem e os atributos numéricos são devolvidos normalmente; a mídia vem vazia ou marcada como pendente, sem erro de negócio.
- Escopo visual de heróis (Classe × Aparência) já coberto pela Feature 005: esta feature não reaudita nem altera esses vínculos.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: O sistema MUST descobrir arquivos de mídia de armas, armaduras, itens de acampamento/provisões, troféus (acessórios) e consumíveis a partir de uma instalação local licenciada do jogo, sem baixar conteúdo de origens externas.
- **FR-002**: O sistema MUST copiar cada arquivo descoberto sem alterar bytes, nome de origem ou extensão, e MUST registrar origem, destino no acervo, hash, tamanho, data e categoria no inventário.
- **FR-003**: O sistema MUST reutilizar o inventário já existente de mídias locais (Feature 004) quando o mesmo conteúdo (mesmo hash) já estiver catalogado, evitando cópia redundante.
- **FR-004**: O inventário MUST distinguir as categorias: arma, armadura, acessório/troféu, item de acampamento/provisão e consumível. A distinção entre item de acampamento/provisão e consumível MUST seguir o mapeamento pasta/origem na instalação local (cada origem consultada corresponde a exatamente um desses dois tipos); arquivos fora das origens mapeadas NÃO MUST ser classificados por nome avulso.
- **FR-005**: O sistema MUST registrar lacunas (pasta ou arquivo não encontrado) com caminho consultado, motivo observável e data, sem abortar o restante da descoberta.
- **FR-006**: O sistema MUST vincular cada um dos cinco níveis de cada arma catalogada a no máximo uma mídia de ícone estático e, quando existir na instalação, a um conjunto animado associado; ausência MUST ser `Pendente`.
- **FR-007**: O sistema MUST vincular cada um dos cinco níveis de cada armadura catalogada com a mesma regra de FR-006.
- **FR-008**: O vínculo de mídia MUST ser referência ao inventário (identificador do arquivo ou conjunto + hash), sem duplicar o arquivo no catálogo de itens.
- **FR-009**: A publicação dos vínculos MUST poder ocorrer a qualquer momento (inclusive durante sessões ao vivo), sem janela de manutenção obrigatória. MUST ser atômica por categoria (tudo da categoria aplicado ou nada) e MUST preservar identificadores, atributos numéricos e equipamento já atribuído a Personagens (Feature 003).
- **FR-010**: O sistema MUST vincular cada acessório já catalogado (Feature 003) a uma mídia de ícone quando o arquivo existir; caso contrário o estado é `Pendente`. O sistema MUST criar um acessório novo identificável para cada troféu (trinket) encontrado na instalação que ainda não exista no catálogo, com nome e descrição, raridade `Comum`, lista de efeitos vazia, sem classe exclusiva e sem identificador de conjunto, e com o vínculo de mídia. O sistema NÃO MUST inventar efeitos oficiais nem alterar identificadores, raridade, efeitos ou classe exclusiva dos acessórios já semeados, e NÃO MUST recriar o catálogo 003.
- **FR-011**: O sistema MUST adicionar dois tipos novos de catálogo, ambos especializações identificáveis de `Item` com nome, descrição e vínculo de mídia: `Item de acampamento/provisão` e `Consumível`. Esses tipos MUST ser distintos entre si e de `Arma`, `Armadura` e `Acessório`; o sistema NÃO MUST reclassificar arma, armadura ou acessório existente.
- **FR-012**: O sistema MUST produzir um relatório de cobertura por categoria com status `OK`, `Parcial` e `Pendente` em português do Brasil, incluindo totais e a lista das lacunas.
- **FR-013**: Uma arma ou armadura MUST ser `OK` somente se os cinco níveis tiverem mídia de ícone resolvida; `Parcial` se ao menos um nível estiver resolvido e ao menos um pendente; `Pendente` se nenhum nível estiver resolvido.
- **FR-014**: A consulta de um item, arma, armadura ou personagem MUST devolver a mídia resolvida quando o vínculo existir e MUST permanecer utilizável quando o vínculo estiver pendente (sem exigir arquivo físico naquele momento).
- **FR-015**: O inventário MUST declarar que os arquivos provêm de instalação local licenciada e que esta feature não autoriza redistribuição.
- **FR-016**: O escopo MUST excluir mídias de inimigos, chefes, ambientes/tiles e ícones de status, bem como reauditoria dos assets visuais de Classe × Aparência já cobertos pela Feature 005.
- **FR-017**: Mensagens voltadas ao curador ou ao jogador (relatório, lacunas, estados de cobertura) MUST estar em português do Brasil; nomes originais de arquivo podem permanecer como rastreabilidade.
- **FR-018**: O sistema MUST impedir que a ausência de mídia apague ou altere equipamento já atribuído a Personagens.

### Key Entities

- **Inventário de mídia local**: Acervo rastreável de arquivos importados de instalação licenciada; cada entrada tem origem, destino, hash, tamanho, categoria e, quando aplicável, conjunto animado (textura + atlas + esqueleto).
- **Arquivo importado**: Cópia byte a byte identificada por hash; pode ser compartilhada por várias associações.
- **Mídia de equipamento**: Vínculo entre um item do catálogo e uma ou mais entradas do inventário (ícone estático obrigatório para status `OK`; conjunto animado opcional).
- **Vínculo de nível de arma**: Associação entre um dos cinco níveis de uma arma e sua mídia; não altera dano, crítico nem velocidade.
- **Vínculo de nível de armadura**: Associação entre um dos cinco níveis de uma armadura e sua mídia; não altera HP adicional nem esquiva.
- **Vínculo de item**: Associação entre um acessório, item de acampamento/provisão ou consumível e sua mídia de ícone.
- **Relatório de cobertura**: Visão por categoria com contagens `OK` / `Parcial` / `Pendente` e lista de lacunas.
- **Arma, Armadura, Acessório, Item**: Entidades já definidas na Feature 003; esta feature acrescenta referências de mídia sem alterar identificadores nem atributos numéricos/raridade já existentes. **Troféu** é o nome de produto para `Acessório` (trinket no jogo: anel, colar, ídolo e equivalentes equipáveis), não cabeça de chefe nem item de acampamento. Acessórios novos criados a partir da instalação têm raridade `Comum` e efeitos vazios até curadoria futura; os registros 003 não são substituídos.
- **Item de acampamento/provisão**: Especialização identificável de `Item` (tipo novo nesta feature) para suprimentos de expedição provenientes das origens de pasta mapeadas como acampamento/provisão (por exemplo tocha, bandagem, comida); não é acessório nem consumível.
- **Consumível**: Especialização identificável de `Item` (tipo novo nesta feature) para usos pontuais provenientes das origens de pasta mapeadas como consumível (por exemplo pergaminho, poção, dinamite); não é item de acampamento/provisão nem acessório.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% dos arquivos de mídia descobertos nas pastas de equipamento e itens da instalação local selecionada são copiados com hash de destino idêntico ao de origem, ou reutilizados quando o hash já existe no inventário.
- **SC-002**: Em uma amostra de 20 arquivos importados (mistura de arma, armadura e item), 20 de 20 podem ser rastreados do destino até o caminho-fonte local, a categoria e o hash original.
- **SC-003**: 100% das 20 armas catalogadas expõem cinco vínculos de mídia (um por nível), cada um `OK` ou `Pendente`; nenhuma arma perde níveis numéricos ou identificador.
- **SC-004**: 100% das 20 armaduras catalogadas satisfazem o mesmo critério de SC-003.
- **SC-005**: O relatório de cobertura lista as cinco categorias (arma, armadura, acessório/troféu, acampamento/provisão, consumível) com totais que somam o número de **itens** esperados (Arma = 20, Armadura = 20; não 100 vínculos), sem categoria órfã não classificada. Na categoria acessório/troféu, o esperado inclui os registros 003 mais os acessórios novos criados a partir da instalação.
- **SC-006**: Após uma publicação feita a qualquer momento (incluindo durante sessão ao vivo), a consulta de um Personagem existente continua devolvendo o mesmo equipamento; quando o nível atual tem mídia `OK`, a referência visual é resolvida na mesma consulta. Se a publicação da categoria falhar, nenhum vínculo daquela categoria permanece pela metade.
- **SC-007**: Uma importação com pelo menos uma pasta ausente ainda produz inventário e relatório; 100% das pastas ausentes aparecem como lacuna registrada (não como falha silenciosa).
- **SC-008**: O curador consegue obter o relatório de cobertura em até 2 minutos após solicitar a geração, para o volume esperado desta feature (armas, armaduras e itens da instalação local).
- **SC-009**: Nenhuma mídia de inimigo, tile ou ícone de status entra no inventário desta feature; uma verificação amostral de 10 arquivos do inventário 006 confirma categoria exclusiva de equipamento ou item.
- **SC-010**: 100% das mensagens de status do relatório (`OK`, `Parcial`, `Pendente`) e das descrições de lacuna estão em português do Brasil.

## Out of Scope

- Mídias de inimigos e chefes.
- Mídias de ambientes, corredores e tiles.
- Ícones de status, buffs e debuffs.
- Reauditoria ou alteração dos assets visuais de Classe × Aparência já cobertos pela Feature 005.
- Download ou mineração a partir da wiki ou de qualquer origem remota de arquivos de jogo.
- Redistribuição dos arquivos importados.
- Regras novas de combate, acampamento, XP ou aparência de Personagem.
- Recálculo de bônus de conjunto de acessórios (já fora da Feature 003).
- Curadoria posterior de raridade, efeitos, classe exclusiva e conjunto dos acessórios novos criados só com mídia (permanecem `Comum` e sem efeitos nesta feature).
- Exclusão de itens do catálogo.

## Assumptions

- A origem dos arquivos é a mesma política da Feature 004: instalação local licenciada do jogo (base e DLCs presentes na máquina do curador), nunca a wiki nem origens remotas.
- Granularidade visual de arma e armadura é **por nível 1 a 5** (até 100 vínculos de arma e 100 de armadura), alinhada aos cinco níveis da Feature 003; não há um único ícone genérico por classe no lugar dos cinco níveis.
- Ícone estático (imagem) é o mínimo para marcar `OK`; conjunto animado (quando a instalação tiver os três arquivos relacionados) é registrado e vinculado, mas a ausência do conjunto animado sozinha não impede `OK` se o ícone existir.
- A Feature 003 já cobre `Arma`, `Armadura` e `Acessório` (troféu). `Item de acampamento/provisão` e `Consumível` são tipos novos desta feature, irmãos de Arma/Armadura/Acessório. A lista concreta de registros segue o que for encontrado nas origens de pasta mapeadas (bandagem, ervas, sangue-santo, brasa, chave, comida, tocha, água benta, pergaminhos, poções, dinamite e equivalentes), sem inventar itens ausentes no jogo e sem promover provisão ou consumível a acessório.
- A classificação acampamento/provisão versus consumível é determinada pelo mapeamento pasta/origem na instalação (cada origem consultada liga-se a um único tipo). Os caminhos exatos das pastas ficam para o plano técnico.
- “Troféus” no pedido do operador correspondem a `Acessório` (trinkets) da Feature 003 — itens equipáveis com raridade e efeitos, não loot de chefes. Acessórios já semeados são apenas vinculados. Troféus encontrados na instalação e ainda ausentes no catálogo viram acessórios novos com raridade `Comum`, efeitos vazios, sem classe exclusiva e sem conjunto; o seed 003 de acessórios não é recriado nem substituído.
- O padrão de referência é o da Feature 005: identificar no inventário por chave de conjunto/arquivo + hash, sem guardar caminhos duplicados no catálogo de domínio.
- Contagens da Feature 003 permanecem a âncora para armas e armaduras: 20 armas, 20 armaduras, 20 classes; esta feature não cria armas ou armaduras extras. O total de acessórios pode crescer além do seed 003.
- Vocabulário de produto desta feature é o trio `OK` / `Parcial` / `Pendente`. A string `Faltando` pertence à Feature 005 (auditoria wiki) e **não** é status de cobertura 006.
- Textos de produto, relatório e estados de cobertura em PT-BR; nomes de arquivo originais em inglês ficam só para rastreabilidade.
- Persistência oficial continua sendo o banco do projeto; configurações de caminho da instalação local ficam fora do código-fonte (ambiente), sem segredos no repositório.
- A solução deve permanecer a mais simples que atenda aos requisitos: inventariar, referenciar, relatar cobertura — sem novos fluxos de combate ou loja.
- Publicação de vínculos não exige a janela de manutenção da Feature 005: pode rodar a qualquer momento, desde que atômica por categoria e sem alterar equipamento já atribuído.

## Referência cruzada

- Escopo excluído da Feature 005: [specs/005-auditoria-habilidades-mineradas/spec.md](../005-auditoria-habilidades-mineradas/spec.md) (Q2 da 3ª clarificação; seção Out of Scope).
- Inventário e política local: [specs/004-minerar-midias-herois/spec.md](../004-minerar-midias-herois/spec.md) (FR-012 exclui itens).
- Catálogo de equipamentos: [specs/003-hierarquia-catalogo-herois/spec.md](../003-hierarquia-catalogo-herois/spec.md).

