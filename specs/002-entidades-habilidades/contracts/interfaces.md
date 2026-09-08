# Contratos Internos: Entidades e Catálogo de Habilidades

## Regras de dependência

| Camada | Pode referenciar | Não pode referenciar |
|---|---|---|
| API | Application e contratos de domínio necessários ao mapeamento HTTP | Implementações internas de Infrastructure |
| Application | Domain e abstrações próprias | API e implementações concretas de Infrastructure |
| Domain | Apenas abstrações internas e bibliotecas permitidas | API, Application e Infrastructure |
| Infrastructure | Application abstractions e Domain | Controllers e contratos HTTP da API |

## Serviços de Application

### IHabilidadeService

- Criar uma Habilidade.
- Listar todas as Habilidades.
- Listar por categoria e disponibilidade.
- Consultar por `Guid`.

### IConfiguracaoClassePersonagemService

- Consultar a configuração por classe.
- Criar ou substituir a lista de habilidades da classe.
- Rejeitar categorias, duplicidades, disponibilidade inválida e excesso do limite padrão.

### IPersonagemService

- Criar Personagem a partir de Ser, classe e configuração vigente.
- Consultar Personagem por `Guid`.
- Treinar habilidade de Acampamento.
- Equipar ou remover habilidade de Acampamento para aventura.

### IInimigoService

- Criar Inimigo a partir de Ser e tipo válido.
- Associar quantidade arbitrária de habilidades válidas.
- Consultar Inimigo e calcular resistências efetivas.

### IItemService

- Criar Item.
- Consultar Item por `Guid`.

## Repositórios

- `IHabilidadeRepository`: consulta por ID, listagem, filtros e persistência.
- `IConfiguracaoClassePersonagemRepository`: consulta e persistência da configuração por classe.
- `IPersonagemRepository`: consulta Personagem com vínculos, inventário e equipamentos.
- `IInimigoRepository`: consulta Inimigo com vínculos e resistências.
- `IItemRepository`: consulta e persistência de Item e tipo derivado.

Todos devem seguir `IRepositorioIdentificavel<TEntity>` quando aplicável e retornar abstrações de domínio, sem expor `DbSet` à Application.

## Respostas de operação

Os serviços devem usar o padrão existente `ResultadoOperacao<T>` com estados de sucesso, entrada inválida, não encontrado e falha de persistência. Mensagens destinadas ao consumidor devem ser PT-BR e não expor detalhes de banco.

## DI obrigatório

A composição deve registrar e resolver:

- Serviços de Habilidade, configuração de classe, Personagem, Inimigo e Item.
- Repositórios correspondentes.
- `DarkestDungeonDbContext`.
- Implementações genéricas existentes sem duplicar registros conflitantes.

Testes de arquitetura devem confirmar ausência de referência do domínio para camadas externas e ausência de referência da Application para API/Infrastructure.
