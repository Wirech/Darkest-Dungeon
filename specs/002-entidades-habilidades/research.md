# Research: Entidades e Catálogo de Habilidades

## Decisão: Herança TPT para Ser, Personagem e Inimigo

**Rationale**: `Ser` já concentra os atributos comuns e validações. TPT mantém esses dados em uma tabela base e coloca os campos específicos de Personagem e Inimigo em tabelas próprias, permitindo adicionar novos derivados com mudanças localizadas e sem colunas nulas na tabela base.

**Alternativas consideradas**:
- TPH: seria mais simples em quantidade de tabelas, mas concentraria campos de tipos diferentes em uma tabela larga.
- Composição sem herança: reduziria o acoplamento de persistência, mas contrariaria o contrato de domínio já definido para tipos derivados de Ser.

## Decisão: Tabelas de associação para habilidades

**Rationale**: habilidades de classe, personagem e inimigo possuem cardinalidades e estados diferentes. Relações explícitas preservam integridade referencial, permitem consultas por categoria e evitam serializar listas em colunas de texto.

**Alternativas consideradas**:
- JSON em uma coluna: simplificaria a primeira migration, mas dificultaria unicidade, filtros, validação de disponibilidade e evolução dos estados treinada/equipada.
- IDs em uma lista delimitada: rejeitado por perda de integridade referencial.

## Decisão: Habilidade como entidade independente

**Rationale**: a mesma habilidade pode ser referenciada por classes, Personagens e Inimigos, e pode ser exclusiva de um público ou compartilhada. O cadastro inicial terá nome, descrição, categoria e disponibilidade; efeitos detalhados ficam para evolução posterior.

**Alternativas consideradas**:
- Habilidade embutida em cada entidade: duplicaria conteúdo e impediria o catálogo global solicitado.
- Enum fixo: não permitiria popular detalhes depois sem nova versão do código.

## Decisão: Fotografia de habilidades no Personagem

**Rationale**: na criação, o serviço copia a configuração vigente da classe para vínculos do Personagem. Isso torna explícito quais habilidades o personagem possui e evita alterações silenciosas em personagens existentes quando uma classe for reconfigurada.

**Alternativas consideradas**:
- Resolver a configuração da classe a cada consulta: manteria dados menores, mas faria personagens antigos mudarem de comportamento sem uma ação explícita.

## Decisão: Estados de habilidades separados por categoria

**Rationale**: vínculos de Combate terão `Habilitada`; vínculos de Acampamento terão `Treinada` e `Equipada`. O domínio impedirá equipar habilidade não treinada e limitará três equipadas por aventura. Uma posição vazia é representada pela ausência de vínculo, portanto o padrão aceita até seis habilidades de Combate.

**Alternativas consideradas**:
- Um estado único para todas as categorias: não representa o ciclo próprio de treinamento de Acampamento.

## Decisão: Resistências de Inimigo em duas parcelas

**Rationale**: o Tipo de Inimigo fornece base 5 para cada resistência nesta fase; o Inimigo armazena sua parcela própria, validada entre 0 e 100. A resistência efetiva é base + própria e pode superar 100, conforme a clarificação.

**Alternativas consideradas**:
- Persistir apenas o total: perderia a origem da base e dificultaria alteração futura de tipos.
- Limitar o total a 100: contrariaria a decisão de domínio registrada na especificação.

## Decisão: Item independente de Ser

**Rationale**: Item é equipamento e pode ser usado por Armadura, Arma, Acessório e Inventário, mas não possui atributos de criatura. O contrato inicial será mínimo e extensível.

**Alternativas consideradas**:
- Herdar de Ser: adicionaria HP e atributos de combate sem significado para um item.

## Decisão: Persistência atômica e serviços manuais

**Rationale**: o projeto já usa serviços, DTOs e mapeamento manual em Application, repositórios por interface e EF Core em Infrastructure. A criação de Personagem deve validar tudo antes de salvar e usar a unidade de trabalho do DbContext para evitar registros parciais.

**Alternativas consideradas**:
- AutoMapper ou novo mediator: não há benefício demonstrado para o escopo e aumentaria dependências.

## Decisão: Escopo operacional

**Rationale**: manter listagem de habilidades sem paginação inicialmente, pois o catálogo esperado é pequeno. Contratos devem continuar documentados em OpenAPI e todas as mensagens destinadas ao usuário devem ser PT-BR.

**Alternativas consideradas**:
- Paginação obrigatória desde a primeira versão: pode ser adicionada quando o volume real justificar, sem bloquear o catálogo inicial.
