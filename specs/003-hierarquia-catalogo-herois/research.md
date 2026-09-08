# Research: Hierarquia de Entidades e Catálogo Oficial de Heróis

## Decision: Absorver 002 dentro de 003

**Rationale**: A feature 002 ficou apenas em nível de spec e não teve implementação; várias
decisões de 002 (hierarquia flat de Habilidade, Item genérico) são revisadas por 003. Consolidar
Personagem, Inimigo, Item, Habilidade e Classe em uma única entrega evita retrabalho, evita
divergência entre 002 e 003 e mantém a base de código coerente com as decisões finais aqui
registradas (`Habilidade` como base identificável comum, `Habilidade de Herói`/`Habilidade de
Inimigo` como subtipos irmãos, `Item` abstrato, `Arma`/`Armadura`/`Acessório` herdando).

**Alternatives considered**: Executar 002 primeiro em uma iteração separada; foi rejeitado
porque 002 sozinha entregaria contratos que 003 sobrescreveria (por exemplo, `Habilidade` sem
hierarquia; `Item` genérico sem herança), gerando migrações e endpoints descartáveis.

## Decision: Herança TPH para Habilidade e Item, Owned Types para componentes

**Rationale**: A hierarquia tem tipos concretos com atributos próprios (`Habilidade de
Combate`, `Habilidade de Acampamento`, `Habilidade de Inimigo`; `Arma`, `Armadura`,
`Acessório`) e não há necessidade prática de consultas por ancestral abstrato. `TPH`
(Table-Per-Hierarchy) mantém queries e integridade referencial simples com uma única tabela
por hierarquia e uma coluna discriminadora, atendendo bem à escala esperada (~220 habilidades,
20 Armas + 20 Armaduras + N Acessórios). Componentes como `Nível de Arma`, `Nível de
Armadura`, `Efeito de Habilidade` e `Resistências de Classe` são representados como `Owned
Types` do EF Core — sem identidade própria e sempre consultados junto com o dono.

**Alternatives considered**: `TPT` (Table-Per-Type) foi rejeitado por adicionar `JOIN`s a cada
consulta sem ganho perceptível dado o volume. `TPC` (Table-Per-Concrete-Type) foi rejeitado
porque dificulta associações genéricas (Classe × Habilidade referenciando qualquer subtipo de
Habilidade de Herói). Colunas soltas em `Habilidade` (sem herança) foram rejeitadas por
conflitar diretamente com a decisão registrada nas clarificações.

## Decision: Nome único global de Habilidade + associação Classe × Habilidade

**Rationale**: Alinhado à decisão Q5 registrada na spec. Cada `Habilidade` é um registro único
identificado por nome global; a associação Classe × Habilidade materializa as classes elegíveis
por habilidade. Isso simplifica `Habilidade Compartilhada de Acampamento` (mesmo registro,
várias associações) e evita duplicação entre Highwayman e Grave Robber para skills como
`Gallows Humor`.

**Alternatives considered**: Duplicar habilidades por classe (par nome+classe como chave); foi
rejeitado porque cria linhas repetidas com dados idênticos e obriga sincronização manual.

## Decision: Toda Arma e toda Armadura exigem exatamente uma classe elegível

**Rationale**: Alinhado à decisão Q6. Toda `Arma`/`Armadura` no jogo pertence a uma única
classe; a validação de equipamento passa a ser uma simples comparação entre a classe do
Personagem e a classe elegível do Item. Isso simplifica endpoints e reduz risco de
inconsistência.

**Alternatives considered**: Permitir `Arma` genérica (zero classes) ou multi-classe; ambas
rejeitadas porque não refletem o jogo real e enfraquecem a validação automática.

## Decision: Mapa de Cobertura com três estados por atributo

**Rationale**: Alinhado à decisão Q7. Três estados (`Coletado`, `Pendente`, `NaoAplicavel`)
distinguem o que ainda falta minerar (`Pendente`) do que é intencionalmente ausente
(`NaoAplicavel`), suportando testes objetivos para SC-003, SC-004 e SC-007 e permitindo relatórios
por classe. O estado é registrado em uma `EntradaDoMapaDeCobertura` por (Classe, Categoria,
Atributo).

**Alternatives considered**: Bool `pendente` (2 estados) foi rejeitado porque força representar
ausência legítima como valor especial e confunde consultas. Cobertura por entidade completa
(sem granularidade por atributo) foi rejeitada por não capturar campos parcialmente coletados
(por exemplo, chance base ausente em uma habilidade).

## Decision: Efeito de Habilidade como Owned Type estruturado

**Rationale**: Alinhado à decisão Q8. Cada `Efeito de Habilidade` carrega nome do efeito
(string), alvo (enum `Self`/`Aliado`/`Inimigo`), valor numérico, unidade (enum
`Percentual`/`Pontos`/`Rodadas`), duração em rodadas (nullable) e chance base (0–100). Isso
permite consultas exatas como "todas as habilidades com Bleed no inimigo" e mantém
`Pendente`/`NaoAplicavel` no nível do atributo individual, quando necessário.

**Alternatives considered**: Texto livre foi rejeitado pela impossibilidade de consulta
estruturada. Modelo híbrido (campos estruturados só para efeitos comuns) foi rejeitado por
adicionar complexidade sem eliminar o texto livre.

## Decision: Resistências extras (Doença, Golpe Mortal, Armadilha) apenas em Personagem

**Rationale**: Alinhado à decisão Q3. `Ser` não muda; `Personagem` recebe um Owned Type
`ResistenciasExtrasDePersonagem` com as três resistências novas. `Classe` fornece os valores
base para as oito resistências (as cinco de `Ser` + as três novas), e `PersonagemService`
copia as bases da Classe no momento da criação. `Inimigo` não recebe as três novas resistências
nesta feature.

**Alternatives considered**: Estender `Ser` foi rejeitado porque quebra a compatibilidade da
feature 001 e propaga campos que não fazem sentido para `Inimigo`. Uma entidade separada
`ResistenciasDeHeroi` foi rejeitada por adicionar tabela sem ganho — o Owned Type resolve.

## Decision: Enum forte para ClasseDeHeroi e TipoDeInimigo

**Rationale**: 20 classes de herói e 7 tipos de inimigo são conjuntos fechados. Enums em C#
fornecem validação de compilação, evitam strings mágicas e mapeiam bem para colunas
discriminadoras. Nomes originais em inglês ficam como metadata do enum (`Description`
attribute) para rastreabilidade.

**Alternatives considered**: Tabelas de referência (`ClasseDeHeroiRef`) foram consideradas para
DLCs futuros, mas rejeitadas porque tornam o modelo mais dinâmico do que precisa e conflitam com
o SC-002 (lista fechada de 20 classes).

## Decision: Mineração como responsabilidade das tasks/implement, não do plan

**Rationale**: A spec explicita que a mineração das 20 páginas é parte da implementação. O plan
descreve o modelo e os contratos; o `tasks.md` e a fase `/speckit-implement` executam a
transcrição fiel dos dados oficiais, marcando estados no Mapa de Cobertura. Isso mantém o
plano estável enquanto os dados evoluem.

**Alternatives considered**: Definir seed data completo no plan foi rejeitado porque
duplicaria a fonte oficial e engessaria o plano para pequenas correções de dado.

## Decision: Reutilizar `EntidadeControllerBase` e o padrão de `ResultadoOperacao`

**Rationale**: Os novos controllers seguem a mesma base já validada em 001 (`Ok`/`NotFound`/
`BadRequest` + `ErroResponse` em PT-BR). Isso mantém contratos consistentes entre features e
elimina redecisão de mapping em cada endpoint.

**Alternatives considered**: Introduzir um `ProblemDetails` genérico foi rejeitado por sair do
padrão PT-BR já validado. Handlers de exceção globais foram rejeitados como fora de escopo
desta feature (o plan foca em contratos, não em middleware novo).

## Decision: Limite fixo de 6 habilidades de Combate + 6 de Acampamento por Personagem

**Rationale**: Alinhado à decisão Q2 do `/speckit-clarify`. É o limite oficial do jogo e evita
dependência da feature 002 (que não foi implementada). A validação mora em
`PersonagemService` como pré-condição antes de persistir; 400 em PT-BR via `ErroResponse` cita
o atributo responsável (`habilidadesCombate` ou `habilidadesAcampamento`).

**Alternatives considered**: Parametrizar via `appsettings` foi rejeitado por adicionar
configuração sem ganho real (o valor 6+6 é fixo no jogo). Remover o limite foi rejeitado
porque quebra o edge case da spec.

## Decision: Habilidades atribuídas a Personagem MUST pertencer à Classe do Personagem

**Rationale**: Alinhado à decisão Q3. A associação `ClasseHabilidade` é a fonte da verdade;
`PersonagemService.CriarPersonagem` e `AlterarHabilidades` MUST consultar a associação antes
de persistir. Erros são retornados como 400 com `ErroResponse` em PT-BR indicando qual
habilidade está fora da Classe.

**Alternatives considered**: Delegar validação ao cliente foi rejeitado porque quebra o
edge case da spec e viola o Princípio III (contratos de backend verificáveis). Validar
apenas na hora de "equipar em batalha" foi rejeitado porque essa endpoint não existe nesta
feature.

## Decision: `ConjuntoId` de Acessório como metadata; cálculo de bônus adiado

**Rationale**: Alinhado à decisão Q4. O `Acessório` guarda `ConjuntoId` opcional apenas para
agrupar peças; nenhum efeito adicional é calculado quando duas peças são equipadas. Isso
mantém a persistência preparada para uma feature futura sem inflar o escopo atual (Princípio V
— simplicidade proporcional).

**Alternatives considered**: Modelar tabela `BonusDeConjunto` + regra em `PersonagemService`
foi rejeitado porque exige tabela de bônus ainda não minerada. Remover `ConjuntoId` por
completo foi rejeitado porque impede um upgrade incremental futuro.

## Decision: Formato único `ErroResponse` em PT-BR para todas as respostas 400/404

**Rationale**: Alinhado à decisão Q5. Reutilizar o contrato `ErroResponse` de 001 mantém
consistência entre features, satisfaz o Princípio IV e permite validar automaticamente (via
`ContratoErroResponseTests`) que toda resposta 400 e 404 dos novos endpoints segue o schema
`{ mensagem: string, campo?: string }`.

**Alternatives considered**: `ProblemDetails` (RFC 7807) foi rejeitado por divergir do
padrão existente. Formato livre por controller foi rejeitado por impossibilitar teste de
contrato.

## Decision: Individualidades e Doenças de Personagem fora do escopo desta feature

**Rationale**: Alinhado à decisão Q1. Ambas as coleções são funcionalidades ricas (progressão
de individualidade, cura de doenças, imunidades) que exigem regras próprias e não cabem no
escopo desta feature. Modelo, contratos e testes desta feature ignoram esses conceitos; eles
ficam registrados como intenção para uma feature futura (documentado em spec e research).

**Alternatives considered**: Modelar como owned collections vazias foi rejeitado porque
criaria contratos que sugerem funcionalidade inexistente. Adicionar endpoints "stub" foi
rejeitado por violar o Princípio V.

## Decision: `HabilidadeDePersonagem` mantém referência viva a `Habilidade`

**Rationale**: Alinhado à decisão Q6 da rodada 3. `HabilidadeDePersonagem` guarda apenas
`HabilidadeId`, sem snapshot de atributos; esta feature NÃO expõe endpoint `PUT/PATCH` de
`Habilidade`, então não há risco de propagação silenciosa dentro do escopo. Quando uma
feature futura introduzir edição, ela MUST exigir confirmação explícita e registrar a
mudança como evento auditado (fora de escopo aqui).

**Alternatives considered**: Snapshot completo na `HabilidadeDePersonagem` foi rejeitado por
duplicar dados e complicar correções de dados oficiais. Versionamento de `Habilidade` (nova
tabela `HabilidadeVersao`) foi rejeitado por infra desnecessária nesta feature. Imutabilidade
após uso foi rejeitada por travar correções tipográficas simples.

## Decision: Sete raridades fechadas para Acessório

**Rationale**: Alinhado à decisão Q7 da rodada 3. O enum `RaridadeDeAcessorio` fica fechado
com exatamente `Comum`, `Incomum`, `Rara`, `MuitoRara`, `CrimsonCourt`, `Crystalline`, `Set`,
cobrindo todas as raridades observadas na wiki oficial (jogo base + Crimson Court + Color of
Madness + Shieldbreaker). Novas raridades exigem atualização explícita do enum e migração em
feature futura.

**Alternatives considered**: Tabela de referência editável foi rejeitada por complicar
validação em tempo de compilação. Valor `Outra` como escape foi rejeitado por diluir a
validação e permitir dados oficiais mal categorizados.

## Decision: Exclusão fora de escopo + `OnDelete(DeleteBehavior.Restrict)` em todas as FKs

**Rationale**: Alinhado à decisão Q8 da rodada 3. Nenhum endpoint `DELETE` é exposto para
Classe, Habilidade ou Item; todas as FKs entre esses catálogos (Personagem → Classe,
ClasseHabilidade → Classe/Habilidade, Personagem → Arma/Armadura/Acessório equipados,
EntradaDoMapaDeCobertura → Classe) MUST usar `OnDelete(DeleteBehavior.Restrict)`. Tentativas
de exclusão direta via banco falham com erro de FK, garantindo a integridade referencial
requerida por FR-023 sem escrever middleware específico.

**Alternatives considered**: Suportar `DELETE` com regra 409 quando houver dependência foi
rejeitado por inflar escopo. Soft delete (`Ativo: bool`) foi rejeitado por adicionar campo
sem uso imediato. Comportamento default do EF Core foi rejeitado porque provocaria cascata em
owned types e `SetNull` inconsistente em FKs entre agregados.

## Decision: Procedimento documentado para adicionar uma 21ª classe futura

**Rationale**: Alinhado à decisão Q10 da rodada 3. O enum `ClasseDeHeroi` permanece fechado
com 20 valores. Adicionar uma classe futura envolve três passos padronizados: (1) adicionar
novo valor no enum `ClasseDeHeroi` com atributo `Description` para o nome oficial em inglês;
(2) criar migração dedicada (`AddClasse<Nome>`) inserindo a linha em `Classe` com
`ResistenciasBase` e nome PT-BR; (3) atualizar o seed inicial (`ClassesSeed`) e registrar
entradas correspondentes no Mapa de Cobertura (`Pendente` até que os dados sejam minerados).
Os 20 nomes atuais permanecem inalterados.

**Alternatives considered**: Tabela de referência editável em runtime foi rejeitada porque
torna o modelo mais dinâmico do que precisa e conflita com o SC-002 (lista fechada de 20
classes). Reservar placeholders `ReservadoFuturo1..5` no enum foi rejeitado por poluir a API.
