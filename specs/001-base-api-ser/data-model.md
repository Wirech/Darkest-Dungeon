# Data Model: Base de API e Recurso Ser

## EntidadeIdentificavel

Representa o contrato mínimo para qualquer recurso persistido e consultável por
ID.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| Id | Guid | Yes | Gerado pelo sistema; deve ser único e imutável após criação. |

### Relationships

- Base conceitual para `Ser` e futuros recursos identificáveis.

### State Transitions

- Novo recurso: sem ID público antes da criação.
- Recurso criado: recebe ID único e passa a ser consultável por ID.
- Recurso inexistente: consulta retorna erro controlado em PT-BR.

## Ser

Representa uma criatura, personagem ou unidade base com atributos de combate,
defesa, mobilidade, turno, identificação e resistências. Deve permitir
especializações futuras como Monstro, Personagem e Chefe.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| Id | Guid | Yes | Herdado de `EntidadeIdentificavel`; gerado pelo sistema. |
| Nome | string | Yes | Não pode ser vazio nem conter apenas espaços. |
| Tipo | string | Yes | Não pode ser vazio nem conter apenas espaços. |
| HpMaximo | int | Yes | Deve ser maior ou igual a 0. |
| HpAtual | int | Yes | Deve ser maior ou igual a 0 e menor ou igual a `HpMaximo`. |
| Velocidade | int | Yes | Deve ser maior ou igual a 0. |
| Critico | decimal | Yes | Deve estar entre 0 e 100. |
| DanoBaseMinimo | int | Yes | Deve ser maior ou igual a 0 e menor ou igual a `DanoBaseMaximo`. |
| DanoBaseMaximo | int | Yes | Deve ser maior ou igual a 0 e maior ou igual a `DanoBaseMinimo`. |
| Movimento | int | Yes | Deve ser maior ou igual a 0. |
| BonusDeCritico | decimal | Yes | Deve estar entre 0 e 100. |
| Tamanho | int | Yes | Deve estar entre 0 e 4. |
| AcoesPorTurno | int | Yes | Deve ser maior ou igual a 0. |
| Esquiva | decimal | Yes | Deve ser maior ou igual a 0. |
| Precisao | decimal | Yes | Deve ser maior ou igual a 0. |
| Protecao | decimal | Yes | Deve estar entre 0 e 100. |
| Nivel | int | Yes | Deve estar entre 0 e 6. |
| Resistencias | Resistencias | Yes | Cada resistência deve estar entre 0 e 100. |

### Relationships

- `Ser` é um `EntidadeIdentificavel`.
- `Ser` possui um value object `Resistencias`.
- Monstro, Personagem e Chefe são especializações futuras de `Ser`, fora do escopo inicial.

### Validation Rules

- `Nome` e `Tipo` devem ser normalizados por trim antes da validação.
- `HpAtual` não pode exceder `HpMaximo`.
- Campos numéricos não aceitam valores negativos.
- `DanoBaseMinimo` não pode exceder `DanoBaseMaximo`.
- `Critico`, `BonusDeCritico`, `Protecao` e todas as resistências devem estar entre 0 e 100.
- `Tamanho` deve estar entre 0 e 4.
- `Nivel` deve estar entre 0 e 6.
- Mensagens de validação devem ser retornadas em PT-BR.

## Resistencias

Representa as resistências específicas de um Ser a efeitos comuns do domínio.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| Atordoamento | decimal | Yes | Deve estar entre 0 e 100. |
| Sangramento | decimal | Yes | Deve estar entre 0 e 100. |
| Envenenamento | decimal | Yes | Deve estar entre 0 e 100. |
| Debuff | decimal | Yes | Deve estar entre 0 e 100. |
| Movimento | decimal | Yes | Deve estar entre 0 e 100. |

### Relationships

- Pertence a um único `Ser` no escopo inicial.

### Validation Rules

- Todas as resistências usam a mesma escala percentual de 0 a 100.
- Valores fora da faixa devem rejeitar a criação do Ser inteiro.

## TipoDerivadoDeSer

Representa uma especialização futura de `Ser`, como Monstro, Personagem ou
Chefe. Não é persistido como entidade separada no escopo inicial, mas orienta a
modelagem para evitar retrabalho.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| TipoBase | Ser | Yes | Deve reutilizar ID, atributos base e validações comuns de `Ser`. |
| NomeDoTipo | string | Yes | Deve identificar a especialização planejada, como Monstro, Personagem ou Chefe. |

### Relationships

- Herda ou compõe a base comum de `Ser` em versões futuras.
- Não altera o contrato genérico de consulta por ID.

### Validation Rules

- Regras comuns de HP, dano, limites e resistências devem permanecer centralizadas em `Ser` ou em objetos de valor reutilizáveis.

## ContratoPorInterface

Representa um acordo interno entre camadas para reduzir acoplamento. Não é uma
entidade persistida.

| Field | Type | Required | Validation |
|-------|------|----------|------------|
| Nome | string | Yes | Deve indicar o contrato consumido por outra camada. |
| CamadaConsumidora | string | Yes | Deve ser uma camada superior permitida pela arquitetura. |
| CamadaImplementadora | string | Yes | Deve ser uma camada autorizada a fornecer a implementação. |

### Relationships

- Serviços da Application consomem abstrações de repositório.
- Infrastructure implementa contratos necessários para persistência.
- Api compõe dependências sem referenciar regras internas de persistência.

### Validation Rules

- Domain não pode depender de Api nem Infrastructure.
- Contratos principais devem ser resolvidos pelo contêiner de DI durante testes.

### State Transitions

- Rascunho de entrada: dados recebidos ainda não persistidos.
- Válido: dados passam nas validações e podem ser persistidos.
- Persistido: recebe `Id` e fica disponível para consulta.
- Rejeitado: falha de validação ou persistência retorna erro controlado.
