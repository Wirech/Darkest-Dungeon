# Feature Specification: Auditoria das Habilidades Mineradas da Wiki

**Feature Branch**: `005-auditoria-habilidades-mineradas`

**Created**: 2026-09-08

**Status**: Draft

**Input**: User description: "quero que você revise os dados minerados da wiki. os primeiros foram ok, mas o dos outros heróis não foram minerados com os dados de cada habilidade"

## Clarifications

### Session 2026-09-08

- Q: Como a auditoria deve tratar os 5 níveis (Level 1 a Level 5) de cada habilidade que a wiki oficial publica? → A: Adicionar coleção `Niveis` com 5 linhas obrigatórias por habilidade (owned type, análogo a `Arma`/`Armadura`), guardando os modificadores e os valores de cada efeito por nível; auditar a wiki para preencher os 5 níveis de cada uma das 219 habilidades.
- Q: Como a auditoria deve revisar o limite de habilidades por Personagem, agora que a wiki nos deu os totais reais? → A: Distinguir **habilidades treinadas** (aprendidas, sem limite — o Personagem sempre pode usar tudo o que treinou em combate) de **habilidades equipadas de acampamento** (recursos preparados para a expedição, limite fixo de 3 por Personagem, independente da classe). Combate não tem conceito de "equipar": treinou, usa. Acampamento pode ter várias treinadas, mas apenas 3 vão como equipadas na expedição.
- Q: Como o modelo representa uma habilidade não treinada (bloqueada)? → A: Aproveitar a coleção de 5 níveis definida em Q1 e adicionar `Nivel = 0` como estado semântico de "bloqueada / não treinada". `HabilidadeDePersonagem` guarda o nível atual (0..5) daquela habilidade para aquele Personagem. `Nivel = 0` significa bloqueada (não pode ser usada nem em combate nem em acampamento). `Nivel ≥ 1` significa treinada. Isso elimina flags redundantes: "Treinada" é derivado de `Nivel ≥ 1`, "Habilitada" é sinônimo. Combate: qualquer habilidade com `Nivel ≥ 1` é usável, sem outro limite. Acampamento: entre as com `Nivel ≥ 1`, o jogador marca até 3 como `Equipada = true` para a expedição.
- Q: Como o campo `Aparencia` do Personagem (variante visual) deve ser modelado? → A: Usar `enum AparenciaDePersonagem { A, B, C, D }` (quatro paletas oficiais por classe) com `A` como padrão na criação. Cada `Classe` MUST expor um mapa dos assets visuais correspondentes às 4 aparências (arte da classe naquela paleta), de forma que a consulta do Personagem retorne, além do valor `Aparencia`, a referência dos assets renderizáveis para aquela combinação Classe × Aparência.
- Q: Como o campo `Experiencia` do Personagem se relaciona com o `Nivel` de resolucao já existente? → A: Manter `Nivel` (0..6, "Resolve Level") como a resolucao canônica atual e adicionar `Experiencia` (int ≥ 0) como XP acumulado. Semântica oficial da wiki: níveis nomeados `Seeker` (0), `Apprentice` (1), `Adventurer` (2), `Veteran` (3), `Master` (4), `Champion` (5), `Legend` (6). Cada `Nivel` concede +10% em Atordoamento, Sangramento, Envenenamento, Movimento, Debuff e chance de desarmar Armadilha (5 resistências + trap disarm). Subida de nível ocorre quando `Experiencia` cruza os limiares oficiais. **Nota (revisto na 3ª sessão)**: a decisão original propunha 3 modos de campanha (Radiant/Darkest/Stygian) via `enum ModoDeCampanha`; isso foi revogado — ver última Q&A abaixo. O sistema adota apenas o modo mais difícil com tabela única `{2, 8, 14, 24, 36, 48}` e não expõe `ModoDeCampanha`.
- Q: Precisa minerar os 5 níveis das 44 habilidades da baseline (Cruzado/Vestal/Ocultista/Médico da Peste), ou o `SC-002` continua valendo mesmo com o modelo agora exigindo 5 níveis? → A: Ampliar SC-002 — o baseline permanece `OK` para os campos originais (Level 1), mas os Levels 2..5 dessas 44 habilidades entram na fase de modelagem (US4) como "preenchimento inicial", não como "correção". A habilidade continua marcada `OK` mesmo depois que Levels 2..5 forem preenchidos; a mineração de níveis do baseline não invalida a marcação subjetiva de qualidade da wiki original.
- Q: Como a publicação da versão auditada no SQL Server (US3) deve se comportar se falhar no meio das ~1.100 atualizações (219 habilidades × 5 níveis)? → A: Publicação atômica dentro de uma única transação: ou todas as habilidades são atualizadas ou nenhuma. Falha em qualquer upsert MUST provocar rollback total, deixando o banco no estado pré-publicação. A publicação MUST ocorrer em janela de manutenção fora do horário das sessões live (6 jogadores), e o pipeline MUST registrar log detalhado (habilidade que falhou, motivo) para retry manual após correção.
- Q: Como o modelo `AssetsDeClasse` da Feature 005 deve se relacionar com o inventário de conjuntos Spine (PNG + `.atlas` + `.skel`) já minerado pela Feature 004? → A: `AssetsDeClasse` guarda **referência ao inventário 004** por chave (ID do Conjunto Spine + hash do arquivo) — sem duplicar os arquivos nem os paths. A API do Personagem retorna a resolução dos assets a partir do inventário 004 (paths canônicos e conjunto Spine completo — textura + atlas + esqueleto). A auditoria da Feature 005 (FR-007j) verifica apenas a **existência do vínculo** (Classe × Aparência → ConjuntoSpineId), não a existência física dos arquivos (essa responsabilidade continua com a Feature 004). Combinações Classe × Aparência sem vínculo registrado ficam `Pendente` no Mapa de Cobertura na nova categoria `AssetsVisuais`.
- Q: Armas, armaduras e itens (equipamentos/curiosos/consumíveis já modelados na Feature 003) devem ter suas mídias/ícones auditados nesta Feature 005? → A: **Fora do escopo**. A Feature 005 audita apenas mídias de heróis (Classe × Aparência) via vínculo com o inventário da Feature 004. Assets de `Arma`, `Armadura`, `Item` (troféus, curiosos, consumíveis, provisões) NÃO são auditados nem modelados aqui. Uma Feature 006 dedicada MUST ser criada depois para cobrir esses assets — provavelmente reabrindo/ampliando o pipeline da Feature 004 para incluir ícones de equipamento antes de auditar em 006.
- Q: As 4 classes baseline (Cruzado, Vestal, Ocultista, Médico da Peste) devem ser reminadas da wiki para gerar os `wiki-snapshots/*.json`, ou devemos extrair os snapshots dos seeds 003? → A: **Remineirar tudo da wiki** (Level 1..5 das 4 baselines) e comparar com o seed 003. Divergência entre wiki fresca e seed 003 MUST ser tratada caso a caso pelo curador: a versão da wiki é a fonte-de-verdade final (a wiki é a fonte canônica declarada na Assumption principal), mas o curador MUST registrar qualquer diff detectado no relatório antes de sobrescrever. Isso pode ampliar SC-002: se a re-mineração revelar divergência real, a baseline daquela habilidade deixa de ser `OK` e vira `Parcial` no relatório — a marcação "OK" da baseline não é imune a re-auditoria contra a wiki atual.
- Q: Como os 7 nomes oficiais dos Níveis de Resolução (`Seeker`..`Legend`) devem aparecer no modelo/API/mensagens PT-BR? → A: **Traduzir para PT-BR + guardar `NomeOriginal` em inglês para rastreabilidade com a wiki**. Mapeamento canônico: `Seeker` → `Curioso` (0), `Apprentice` → `Aprendiz` (1), `Adventurer` → `Aventureiro` (2), `Veteran` → `Veterano` (3), `Master` → `Mestre` (4), `Champion` → `Campeão` (5), `Legend` → `Lenda` (6). A entidade `NivelDeResolucao` MUST expor `Nome` (PT-BR canônico, usado em API e mensagens) e `NomeOriginal` (inglês, usado apenas para rastreabilidade e diff com a wiki). Padrão análogo ao que `Habilidade` já usa (`NomeExibicao` PT-BR + `NomeOriginal` inglês) na Feature 003.
- Q: Qual deve ser a política quando o `ModoDeCampanha` de um Personagem existente for alterado e a nova tabela XP rebaixaria o `Nivel` atual? → A: **Não existe alternância de modo.** O sistema MUST oferecer **apenas o modo mais difícil** (equivalente a Darkest/Stygian oficialmente idênticos). Não haverá enum `ModoDeCampanha` nem tabelas paralelas para Radiant/Stygian. O campo, a entidade e as tabelas de limiares MUST ser removidos das features FR-007m, FR-007k, do Key Entity `ModoDeCampanha`, do SC-014 e das Assumptions. Restam apenas os limiares canônicos únicos: `{2, 8, 14, 24, 36, 48}` para subir aos níveis 1..6. Como consequência, o edge case de rebaixamento por troca de modo desaparece — não é possível trocar o que não existe como opção.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Auditoria comparativa das 20 classes já semeadas (Priority: P1)

O curador do catálogo precisa saber, por classe, se as 219 habilidades já semeadas contêm todos os dados oficiais da wiki `darkestdungeon.wiki.gg` (posições, modificadores, efeitos estruturados, limites de uso e descrição fiel), ou se ficaram simplificações que precisam ser expandidas. As classes mineradas nos primeiros lotes (Cruzado, Vestal, Ocultista, Médico da Peste) serviram de baseline de qualidade; as demais 16 classes precisam ser conferidas contra a mesma fonte para garantir paridade.

**Why this priority**: Sem esta auditoria, o catálogo tem qualidade heterogênea — os testes automatizados (141 verdes) confirmam a **integridade referencial** (unicidade, associações, resistências copiadas), mas não a **fidelidade dos efeitos** contra a wiki oficial. Isso invalida o valor curatorial do seed para consultas de jogo real.

**Independent Test**: Pode ser testado de forma independente gerando um **relatório de conformidade por habilidade** que compare os 15 campos exigidos pelo modelo (`nomeExibicao`, `nomeOriginal`, `descricao`, `posicoesValidas`, `posicoesQueAtinge`, `alvoEmArea`, `modificadorDano`, `modificadorAcerto`, `modificadorCritico`, `efeitos[]`, `limitePorUso`, `custoDeDescanso`, `alvo`, `id`) com a fonte da wiki. O curador pode marcar cada habilidade como `OK`, `Parcial` ou `Faltando` sem alterar código.

**Acceptance Scenarios**:

1. **Given** o catálogo de 219 habilidades já semeadas, **When** o curador consulta a auditoria da classe Cruzado, **Then** todas as 11 habilidades aparecem com status `OK` (baseline).
2. **Given** uma classe minerada tardiamente com dados simplificados, **When** o curador consulta a auditoria da classe, **Then** cada habilidade que perdeu detalhes é listada com status `Parcial` e um diff textual explicando o que foi omitido.
3. **Given** uma habilidade cujo modelo exige valor obrigatório e o seed atual usa `Array.Empty<EfeitoDeHabilidade>()` sem justificativa da wiki, **When** a auditoria é executada, **Then** essa habilidade é sinalizada com status `Faltando` na linha de efeitos.

---

### User Story 2 - Correção seletiva das habilidades sinalizadas (Priority: P2)

Após ter o relatório de auditoria, o curador precisa aplicar correções pontuais apenas nas habilidades marcadas como `Parcial` ou `Faltando`, sem retrabalhar as que já estão `OK`. Cada correção deve preservar os GUIDs determinísticos existentes (para não invalidar Personagens já criados apontando para essas habilidades) e não deve introduzir novas duplicatas de `NomeExibicao`.

**Why this priority**: Sem correções seletivas o curador retrabalha todas as 219 habilidades e arrisca introduzir regressões nas partes que já estão OK. Priorizando P2 porque depende do relatório de P1 e as classes P1 (Cruzado etc.) já podem ser consumidas pelos usuários.

**Independent Test**: Escolher uma única classe do relatório com status `Parcial`, aplicar as correções, re-executar o teste de integridade (141 testes) e o teste de conformidade e verificar que o número de habilidades `Parcial` diminuiu apenas naquela classe, sem afetar outras.

**Acceptance Scenarios**:

1. **Given** a classe Musqueteiro com 3 habilidades marcadas `Parcial`, **When** o curador aplica as correções, **Then** as 3 passam para `OK`, as demais 5 continuam `OK`, o total de habilidades permanece 219 e nenhum GUID muda.
2. **Given** uma habilidade `Parcial` cuja descrição PT-BR perdeu valores numéricos originais (ex.: "reduz precisão em 15 pontos"), **When** o curador ajusta a descrição para incluir o valor, **Then** o status passa para `OK`.
3. **Given** uma habilidade `Faltando` que precisa de novo efeito estruturado (ex.: "Redução de Tocha 5 pts" que estava só na descrição), **When** o curador adiciona o efeito e roda os testes, **Then** os 141 testes continuam verdes e a auditoria da classe reflete a mudança.

---

### User Story 3 - Publicação da versão auditada no SQL Server (Priority: P3)

Após todas as habilidades serem sinalizadas `OK`, o curador precisa que a versão corrigida do seed seja aplicada ao banco SQL Server real (Docker) usado para hospedar as sessões ao vivo, substituindo a versão anterior sem perder dados de Personagens já criados por jogadores.

**Why this priority**: Sem republicar, os jogadores que consomem a API continuam vendo dados simplificados. É P3 porque o valor curatorial das USs 1 e 2 já é útil offline para os desenvolvedores; a publicação apenas propaga.

**Independent Test**: Executar o pipeline de atualização e comparar as contagens antes/depois no SQL real: número de classes = 20 permanece, número de habilidades = 219 permanece, número de associações = 277 permanece, e a versão do seed é bumped para refletir a auditoria.

**Acceptance Scenarios**:

1. **Given** SQL Server real com a versão pré-auditoria dos seeds, **When** o pipeline de atualização é executado, **Then** as habilidades corrigidas são atualizadas em lugar (sem novas entradas duplicadas) e as contagens totais permanecem inalteradas.
2. **Given** Personagens já criados por jogadores apontando para habilidades cujos efeitos foram corrigidos, **When** o jogador consulta o Personagem depois da atualização, **Then** o Personagem continua existindo e passa a mostrar os efeitos atualizados.

---

### User Story 4 - Modelar níveis de habilidade, aparência e progressão do Personagem (Priority: P2)

Como responsável pela modelagem, quero que Habilidade (Combate e Acampamento) tenha uma coleção obrigatória de 5 níveis, que Personagem exponha `Aparencia` (A-D), `Experiencia` (int ≥ 0) e regras alinhadas com o `Nivel de Resolucao` (Seeker → Legend) da wiki, e que a Classe carregue as referências aos assets visuais das 4 aparências. Isso corrige a modelagem herdada da Feature 003 antes que a auditoria (US1) tente registrar dados de nível que hoje não têm onde ficar.

**Why this priority**: Sem essa modelagem, a US1 não consegue registrar Level 2..5 das habilidades da wiki e a US2 não consegue corrigir habilidades por nível — os dados existentes viram inconsistências permanentes. É P2 (não P1) porque depende da estrutura da auditoria (US1) para saber onde encaixar cada valor, mas precisa terminar antes da publicação (US3).

**Independent Test**: Pode ser testado criando um Personagem, verificando que `Aparencia = A` é padrão, `Experiencia = 0` é padrão, `Nivel = 0` é o Curioso inicial, e que cada habilidade da Classe do Personagem tem 5 níveis modeláveis; subir `Experiencia` até cruzar o limiar oficial (único: `{2, 8, 14, 24, 36, 48}`) faz `Nivel` avançar e aplica +10% nas 5 resistências e no trap disarm.

**Acceptance Scenarios**:

1. **Given** Personagem criado sem `Aparencia`, **When** o sistema retorna o Personagem, **Then** `Aparencia = A` e o consumo dos assets renderizáveis retorna o mapeamento Classe × A.
2. **Given** Personagem com `Nivel = 0`, **When** aplico +2 XP, **Then** o Personagem sobe para `Nivel = 1` (Aprendiz) e as resistências Atordoamento/Sangramento/Envenenamento/Movimento/Debuff aumentam em +10 pontos percentuais.
3. **Given** Habilidade "Golpe Sagrado" do Cruzado com 5 níveis modelados, **When** o Personagem treinar até `Nivel = 3` naquela habilidade, **Then** os modificadores usados em batalha são os do Level 3 da coleção `Niveis`.
4. **Given** Personagem com 3 habilidades de acampamento equipadas, **When** tenta equipar uma 4ª, **Then** o sistema rejeita com 400 em PT-BR indicando o limite de 3.
5. **Given** Personagem com 5 habilidades de combate treinadas (`Nivel ≥ 1`), **When** entra em batalha, **Then** todas as 5 estão disponíveis para uso (sem conceito de "equipar" em combate).
6. **Given** Habilidade não treinada (`Nivel = 0`) para o Personagem, **When** o jogador tenta usá-la em batalha ou equipá-la em acampamento, **Then** o sistema rejeita porque a habilidade está bloqueada.

---

### Edge Cases

- Como o sistema sinaliza uma habilidade cuja fonte na wiki também mudou entre a mineração original e a auditoria (ex.: patch do jogo)?
- O que acontece quando uma habilidade `Parcial` tem efeitos "combinados" numa única string (ex.: "Bônus de Resistências +30%" cobrindo Sangramento/Envenenamento/Debuff/Movimento simultaneamente) — deve ser expandida em 4 efeitos separados ou preservada?
- Como tratar habilidades da wiki que originalmente têm chance base > 100% (ex.: Yawp 110%, Hands from the Abyss 110%, Blackjack 110%) hoje capadas em 100% no seed com nota na descrição? A auditoria deve considerá-las `OK` ou reabrir a discussão do modelo?
- Como lidar com o Musqueteiro cujo seed reusa 3 habilidades globais do Besteiro? A auditoria dessas 3 habilidades pertence à ficha do Besteiro, do Musqueteiro, ou de ambas?
- Como sinalizar habilidades que não têm efeitos na wiki (ex.: Wicked Hack — golpe simples sem status) versus habilidades que perderam efeitos por omissão do seed?
- O que acontece com Personagens existentes (criados antes desta feature) que hoje não têm campo `Aparencia`, `Experiencia` ou `Nivel` explícito da wiki? Migração deve preencher `Aparencia = A`, `Experiencia = 0`, `Nivel = 0` (Seeker)?
- Quando uma habilidade é `Nivel = 0` (bloqueada) para um Personagem, a resposta da consulta do Personagem deve incluí-la (para o jogador saber que existe e pode ser destreinada/treinada) ou omitir?
- O que acontece se a transação de publicação atômica exceder o timeout padrão do SQL Server (30s) por causa do volume (~1.100 upserts)? O timeout deve ser configurável para essa operação específica?
- Como o pipeline detecta "conexões ativas de jogadores" antes de iniciar a publicação — flag em cache, contagem no SQL Server, ou health check da API?
- Se o log estruturado da publicação crescer indefinidamente, qual é a política de rotação/retenção? (Provavelmente fora do escopo, mas deve ficar registrado)

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Sistema MUST produzir um relatório de auditoria por classe listando cada habilidade e um status entre `OK`, `Parcial`, `Faltando`, com base numa comparação campo a campo contra a wiki oficial.
- **FR-002**: Sistema MUST permitir consulta do relatório sem alterar código-fonte nem persistência (relatório é um artefato consultivo).
- **FR-003**: Auditoria MUST cobrir os 15 campos exigidos pelo modelo de `HabilidadeDeCombate` e `HabilidadeDeAcampamento` (`nomeExibicao`, `nomeOriginal`, `descricao`, `posicoesValidas`, `posicoesQueAtinge`, `alvoEmArea`, `modificadorDano`, `modificadorAcerto`, `modificadorCritico`, `efeitos[]`, `limitePorUso`, `custoDeDescanso`, `alvo`, `id`, tipo).
- **FR-003a**: Modelo de `HabilidadeDeCombate` e `HabilidadeDeAcampamento` MUST expor uma coleção `Niveis` com exatamente 5 linhas obrigatórias (Level 1 a Level 5), modelada como Owned Type análogo aos níveis de `Arma` e `Armadura`; cada linha guarda os modificadores da habilidade e os valores numéricos de cada `EfeitoDeHabilidade` naquele nível.
- **FR-003b**: Auditoria MUST minerar os 5 níveis de cada uma das 219 habilidades a partir da wiki oficial; níveis não publicados na wiki (raros) MUST ficar registrados como `Pendente` no Mapa de Cobertura em vez de valores presumidos.
- **FR-004**: Para cada habilidade `Parcial` ou `Faltando`, o relatório MUST citar o valor esperado (fonte wiki) e o valor atual (seed), permitindo diff visual.
- **FR-005**: Correções de habilidades sinalizadas MUST preservar o GUID determinístico gerado a partir do `NomeOriginal` (nenhuma alteração de identidade).
- **FR-006**: Correções MUST manter unicidade global de `NomeExibicao` (constraint já validada em SQL Server real).
- **FR-007**: Correções MUST manter as 277 associações Classe×Habilidade e as skills compartilhadas (Encourage, Wound Care, Pep Talk, Field Dressing, Marching Plan, Triage, Gallows Humor) reutilizadas por múltiplas classes.
- **FR-007a**: Modelo de `Personagem` MUST substituir os limites globais herdados da Feature 003 (`LimiteHabilidadesDeCombate=6` e `LimiteHabilidadesDeAcampamento=6`) pela distinção entre **habilidade treinada** (o Personagem aprendeu; pode usar em combate sem limite) e **habilidade equipada de acampamento** (recurso preparado para a expedição, limite fixo de 3 por Personagem, aplicável a qualquer classe).
- **FR-007b**: Combate MUST não ter conceito de "equipar": qualquer habilidade de combate treinada pelo Personagem fica disponível para uso em batalha, refletindo a lógica de RPG "se treinou, sabe fazer".
- **FR-007c**: Acampamento MUST validar que no máximo 3 habilidades permaneçam com o marcador de "equipada" simultaneamente por Personagem; tentativa de equipar uma 4ª MUST resultar em rejeição 400 em PT-BR (com mensagem indicando que uma habilidade equipada precisa ser retirada antes).
- **FR-007d**: Toda habilidade equipada de acampamento MUST estar previamente treinada; equipagem de habilidade não treinada MUST ser rejeitada 400 em PT-BR.
- **FR-007e**: Toda habilidade treinada MUST pertencer à associação Classe × Habilidade da Classe do Personagem (mantém FR-009a da Feature 003 aplicado ao conceito de "treinada").
- **FR-007f**: `HabilidadeDePersonagem` MUST guardar um campo `Nivel` (inteiro 0..5) como estado canônico da habilidade para aquele Personagem: `Nivel = 0` significa **bloqueada / não treinada**; `Nivel ≥ 1` significa **treinada** naquele nível. Combate: qualquer habilidade com `Nivel ≥ 1` é usável em batalha, sem outro filtro. Acampamento: entre as com `Nivel ≥ 1`, o subconjunto marcado como `Equipada = true` (máximo 3) é aplicado na expedição.
- **FR-007g**: Modelo MUST derivar (não persistir separadamente) os predicados `Treinada` e `Habilitada` a partir de `Nivel ≥ 1`, eliminando flags redundantes na entidade `HabilidadeDePersonagem`; o campo `Equipada` continua persistido apenas para habilidades de acampamento.
- **FR-007h**: Entidade `Personagem` MUST expor o campo `Aparencia` como `enum AparenciaDePersonagem { A, B, C, D }`, obrigatório, com padrão `A` na criação e validação de faixa fechada (rejeição de valores fora do enum).
- **FR-007i**: Entidade `Classe` MUST expor um mapa de referências a assets visuais indexado por `AparenciaDePersonagem` (A/B/C/D); cada entrada MUST guardar uma **referência ao inventário da Feature 004** (identificador do `ConjuntoSpine` + hash do arquivo principal), **não** um path livre nem cópia dos bytes. A consulta do Personagem MUST resolver o caminho renderizável através desse vínculo (Spine trio: textura + atlas + esqueleto vindos do inventário 004).
- **FR-007j**: Auditoria MUST verificar que existe **vínculo registrado** com o inventário da Feature 004 para cada uma das 80 combinações Classe (20) × Aparencia (4); a existência física dos arquivos permanece responsabilidade da Feature 004 e **não** é reverificada aqui. Combinações Classe × Aparência sem vínculo MUST ser marcadas como `Pendente` no Mapa de Cobertura na nova categoria `AssetsVisuais`. Se o inventário da Feature 004 ainda não estiver disponível no ambiente, todas as 80 combinações ficam `Pendente` sem invalidar as demais categorias da auditoria.
- **FR-007k**: Entidade `Personagem` MUST expor `Experiencia` como inteiro `≥ 0`, distinto de `Nivel`; `Nivel` permanece como o Resolve Level (0..6) canônico da wiki; subir de nível ocorre quando `Experiencia` cruza os limiares oficiais.
- **FR-007l**: Cada valor de `Nivel` MUST estar associado ao nome canônico PT-BR (`Curioso` (0), `Aprendiz` (1), `Aventureiro` (2), `Veterano` (3), `Mestre` (4), `Campeão` (5), `Lenda` (6)) exposto como `Nome` na entidade `NivelDeResolucao` e na API. A entidade MUST também expor `NomeOriginal` em inglês (`Seeker`/`Apprentice`/`Adventurer`/`Veteran`/`Master`/`Champion`/`Legend`) apenas para rastreabilidade com a wiki. Cada nível MUST conceder +10% em `Atordoamento`, `Sangramento`, `Envenenamento`, `Movimento`, `Debuff` e na chance de desarmar `Armadilha` a cada nível ganho (aplicado por cima das resistências base da Classe).
- **FR-007m**: Modelo MUST expor os limiares de XP para subir de nível como tabela única consultável (o sistema oferece apenas o modo mais difícil), com os valores oficiais `{2, 8, 14, 24, 36, 48}` para atingir os níveis 1 a 6 respectivamente (deltas por-nível: +2/+6/+6/+10/+12/+12). Não existe alternância entre Radiant/Darkest/Stygian. Tentativa de subir com `Experiencia` insuficiente MUST ser rejeitada 400 em PT-BR.
- **FR-008**: Correções MUST manter as 141 (ou mais) suítes de testes verdes após cada iteração.
- **FR-009**: Publicação da versão corrigida MUST atualizar habilidades existentes em lugar (upsert por GUID), sem inserir duplicatas nem invalidar Personagens já criados.
- **FR-009a**: Publicação da versão corrigida MUST ocorrer dentro de uma única transação SQL atômica cobrindo todos os upserts das habilidades e seus 5 níveis; falha em qualquer operação MUST provocar rollback total, deixando o banco no estado pré-publicação.
- **FR-009b**: Pipeline de publicação MUST registrar log detalhado (identificando a habilidade e o campo que falhou, com timestamp e mensagem de erro) para permitir retry manual após correção da causa raiz.
- **FR-009c**: Publicação da versão corrigida MUST ser executada apenas em janela de manutenção declarada (fora do horário das sessões live), e o pipeline MUST rejeitar tentativa de publicação enquanto houver conexões ativas de jogadores no banco (contagem > 0 em sessão de aplicação).
- **FR-010**: Auditoria MUST identificar habilidades cujos efeitos foram simplificados em strings genéricas (ex.: "Bônus de Resistências") e propor a expansão em N efeitos individuais, quando a wiki listar valores diferentes por tipo.
- **FR-011**: Auditoria MUST identificar valores numéricos originais da wiki que ficaram apenas na descrição PT-BR sem aparecer nos efeitos estruturados (ex.: "Torch -5" em Hands from the Abyss).
- **FR-012**: Auditoria MUST reminar da wiki oficial (Level 1..5) as 4 classes baseline (Cruzado, Vestal, Ocultista, Médico da Peste) e comparar com o seed 003; divergências MUST ser reportadas no relatório como `Parcial` com diff explícito, e a wiki fresca é a fonte-de-verdade para resolução. A designação histórica de "baseline OK" **não isenta** essas classes de re-auditoria — ela apenas indica que se espera baixo volume de divergência (≥ 90% mantendo `OK` conforme SC-002).
- **FR-013**: Sistema MUST documentar publicamente (em `dados-minerados.md` ou artefato equivalente) que a chance base foi capada em 100% quando a wiki reporta > 100%, incluindo a lista das habilidades afetadas.
- **FR-014**: Publicação da nova versão MUST preservar o Mapa de Cobertura existente (160 entradas com estado `Coletado` para resistências base).

### Key Entities

- **Relatório de Auditoria**: Documento (por classe ou consolidado) que lista cada habilidade da classe, seu status (`OK`/`Parcial`/`Faltando`), os campos com divergência, o valor esperado (wiki) e o valor atual (seed).
- **Correção de Habilidade**: Alteração pontual em uma habilidade já semeada (descrição, efeitos, modificadores) preservando `id` e `nomeExibicao`.
- **Baseline de Qualidade**: Subconjunto de classes (Cruzado, Vestal, Ocultista, Médico da Peste) tratadas como referência do que se espera de "OK".
- **Mapa de Confiança da Wiki**: Registro de quais habilidades tiveram sua fonte re-verificada e em que data (para permitir auditorias futuras quando a wiki mudar).
- **NivelDeHabilidade** (owned type): Uma das 5 linhas obrigatórias de uma `HabilidadeDeCombate`/`HabilidadeDeAcampamento`, guardando modificadores e valores de efeitos daquele nível (1 a 5) conforme publicado na wiki.
- **HabilidadeDePersonagem** (revisada): Guarda o `Nivel` atual (0..5) daquela habilidade para aquele Personagem — `0` = bloqueada / não treinada, `1..5` = treinada no nível correspondente. `Equipada` (bool) só é persistida e validada para habilidades de acampamento (máximo 3 equipadas por Personagem).
- **AparenciaDePersonagem** (enum fechado): `A`, `B`, `C`, `D` — quatro paletas oficiais por classe; `A` é padrão. Cada Classe expõe o mapa de assets renderizáveis para cada aparência.
- **NivelDeResolucao** (value object): Valor 0..6 do Personagem com `Nome` PT-BR canônico (`Curioso`/`Aprendiz`/`Aventureiro`/`Veterano`/`Mestre`/`Campeão`/`Lenda`) usado em API e mensagens, e `NomeOriginal` inglês (`Seeker`/`Apprentice`/`Adventurer`/`Veteran`/`Master`/`Champion`/`Legend`) apenas para rastreabilidade com a wiki. A cada nível ganho, o Personagem recebe +10% em Atordoamento, Sangramento, Envenenamento, Movimento, Debuff e chance de desarmar Armadilha, empilhados sobre as resistências base da Classe.
- **AssetsDeClasse**: Mapa `AparenciaDePersonagem` → **referência ao inventário de mídias da Feature 004** (identificador do `ConjuntoSpine` + hash do arquivo principal). Não guarda paths livres nem bytes de arquivo; a resolução final do caminho renderizável (textura + atlas + esqueleto) ocorre via lookup no inventário 004. Auditado por FR-007j (verifica existência do vínculo, não do arquivo físico).

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% das 219 habilidades semeadas têm status classificado (`OK`, `Parcial` ou `Faltando`) no relatório de auditoria.
- **SC-002**: As 44 habilidades das classes de baseline (Cruzado, Vestal, Ocultista, Médico da Peste — 11 cada) são reminadas da wiki oficial (Level 1..5) e comparadas com o seed 003. A meta-esperada é que ≥ 90% delas continuem com status `OK` para Level 1 após a re-auditoria; divergências detectadas MUST ser registradas no relatório como `Parcial` com diff explícito e resolvidas caso a caso pelo curador (a wiki atual é a fonte-de-verdade). A mineração dos Levels 2..5 dessas 44 habilidades faz parte da modelagem inicial (US4).
- **SC-003**: Nenhum GUID determinístico de habilidade é alterado durante as correções (verificável comparando snapshot antes/depois).
- **SC-004**: Nenhum `NomeExibicao` é duplicado após as correções (validado pelo índice único no SQL Server).
- **SC-005**: Após publicação, o número total de habilidades no SQL Server permanece 219, o de associações permanece 277 e o de classes permanece 20.
- **SC-006**: Após publicação, o número de habilidades com efeitos estruturados vazios (`Array.Empty<EfeitoDeHabilidade>()`) é igual ao número de habilidades cuja fonte na wiki também não tem efeitos (ex.: golpes simples como Chop, Wicked Hack), sem excedentes injustificados.
- **SC-007**: Todos os testes automatizados continuam verdes após cada iteração de correção (mínimo 141 testes).
- **SC-008**: O relatório de auditoria pode ser regenerado em menos de 5 minutos sem depender de acesso à internet uma vez que a wiki foi consultada.
- **SC-009**: Personagens criados antes da auditoria continuam válidos após a publicação (validado por um teste de continuidade que cria um Personagem, aplica a atualização e consulta o Personagem novamente).
- **SC-010**: 100% das 219 habilidades expõem uma coleção de exatamente 5 níveis obrigatórios (Level 1 a 5) após a auditoria; nenhuma habilidade permanece single-level.
- **SC-011**: Nenhum Personagem no banco pode ter mais de 3 habilidades de acampamento com `Equipada=true` simultaneamente (verificável por consulta SQL de agregação).
- **SC-012**: 100% das 80 combinações Classe (20) × `AparenciaDePersonagem` (A/B/C/D) têm status registrado no Mapa de Cobertura (`Coletado` se há asset mapeado; `Pendente` caso contrário).
- **SC-013**: Personagem criado sem informar `Aparencia` recebe `A` por padrão; criar Personagem com valor fora do enum (`E`, `1`, `null`, etc.) MUST retornar 400 em PT-BR.
- **SC-014**: A tabela única de limiares de XP para subir de `Nivel` retorna os valores oficiais `{2, 8, 14, 24, 36, 48}` para os níveis 1..6; consulta de "quanto XP falta para subir" retorna resposta consistente com essa tabela.
- **SC-015**: A cada `Nivel` ganho, as 5 resistências (Atordoamento/Sangramento/Envenenamento/Movimento/Debuff) e a chance de desarmar Armadilha do Personagem aumentam em +10 pontos percentuais empilhados sobre a base da Classe (verificável comparando resistências de Personagens em `Nivel=0` vs `Nivel=6` da mesma Classe).
- **SC-016**: A publicação da versão auditada MUST ser atômica: em caso de falha simulada em qualquer upsert, o banco permanece 100% no estado pré-publicação (validável por teste que injeta falha em uma habilidade específica e verifica que nenhuma outra foi alterada).
- **SC-017**: O pipeline de publicação MUST produzir log estruturado por habilidade/campo que falhou, com timestamp, identificador da habilidade, campo problemático e mensagem de erro; consulta ao log após uma falha simulada retorna a entrada correspondente em menos de 10 segundos.

## Out of Scope

Os itens abaixo são explicitamente **excluídos** desta feature. Tentativas de auditar ou modelar qualquer um deles MUST ser tratadas como fora de escopo e adiadas para features futuras (sugestão: Feature 006):

- Mídias/ícones de `Arma` (armas de qualquer classe) — apesar de `Arma` já ter níveis modelados na Feature 003, sua imagem/ícone não é auditada aqui.
- Mídias/ícones de `Armadura` (armaduras de qualquer classe) — idem.
- Mídias/ícones de `Item` (troféus, curiosos, consumíveis, provisões de acampamento, joias, tesouros).
- Mídias/animações de inimigos e chefes.
- Ícones de efeitos de status/buffs/debuffs.
- Mídias de ambientes/tiles/tela de expedição.
- Redesenho ou produção de arte nova para qualquer categoria acima.
- Ampliação do pipeline da Feature 004 para minerar categorias de assets além de heróis (Spine trio de classes) — se necessário, será feita numa reabertura de 004 antes da Feature 006.

## Assumptions

- A wiki `darkestdungeon.wiki.gg` continua sendo a fonte canônica dos dados oficiais; nenhuma outra fonte concorrente será consultada.
- A auditoria será realizada de forma incremental, uma classe por vez, sem exigir um único mega-batch.
- As classes de baseline (Cruzado, Vestal, Ocultista, Médico da Peste) foram amostradas manualmente pelo usuário como referência subjetiva de qualidade; a auditoria formal pode reclassificá-las se encontrar problemas — porém, isso deve ser exceção, não regra.
- Habilidades cuja chance base na wiki > 100% permanecem capadas em 100% no modelo (decisão prévia da Feature 003) e a auditoria não reabrirá essa decisão.
- Skills compartilhadas globais (Encourage/Wound Care/Pep Talk) são auditadas uma única vez e sua correção reflete automaticamente nas 19 classes que as reutilizam.
- Skills compartilhadas Besteiro↔Musqueteiro (Field Dressing/Marching Plan/Triage) idem — auditadas uma vez, refletem nas 2 classes.
- `Gallows Humor` (compartilhada Bandido↔Ladrão de Cova) idem.
- O usuário aceita que o relatório de auditoria seja apresentado como documento textual (Markdown) em `specs/005-auditoria-habilidades-mineradas/relatorio.md` (ou similar), não como endpoint da API.
- A publicação da versão corrigida usa o mesmo mecanismo de seed idempotente já existente (o código atual em `Program.cs` já faz `AddRange` apenas de entidades cujos IDs não existem; para atualizações in-place o mecanismo pode precisar ser estendido).
- O tempo necessário para minerar novamente uma classe da wiki fica em torno de 1-2 minutos por classe (baseado no histórico da Feature 003).
- Personagens criados antes desta feature são migrados com `Aparencia = A`, `Experiencia = 0` e `Nivel = 0` (Seeker); habilidades já atribuídas assumem `Nivel = 1` (Level 1 = valores originais mineirados na Feature 003).
- O sistema oferece **apenas o modo mais difícil** (equivalente a Darkest/Stygian oficialmente idênticos). Radiant não é suportado como opção de campanha. Personagens não têm campo de "modo" — os limiares de XP são globais.
- Os assets renderizáveis das 4 aparências são **consumidos do inventário da Feature 004** (mineração de mídias Spine — trio PNG + `.atlas` + `.skel`); a Feature 005 apenas registra o vínculo Classe × Aparência → ConjuntoSpine. Combinações sem vínculo mapeado ficam `Pendente`. A produção de arte nova e a importação física dos arquivos estão fora do escopo desta feature.
- A wiki `darkestdungeon.wiki.gg` publica os 5 níveis de habilidade de forma consistente (colunas Level 1 a Level 5) para todas as 219 habilidades; ausência de dado num nível específico é rara e será tratada como `Pendente`.
