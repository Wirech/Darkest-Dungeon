# Contratos Internos entre Camadas

Este arquivo estende os contratos internos já estabelecidos pela feature 001 com as novas
abstrações desta feature. As regras de dependência entre camadas permanecem exatamente as
mesmas.

## Layer Dependency Rules

- `DarkestDungeon.Api` depende de `DarkestDungeon.Application` e de `DarkestDungeon.Domain`
  (nunca de `Infrastructure`).
- `DarkestDungeon.Application` depende apenas de `DarkestDungeon.Domain`.
- `DarkestDungeon.Domain` não referencia nenhuma outra camada do projeto.
- `DarkestDungeon.Infrastructure` depende de `DarkestDungeon.Application` (para implementar
  contratos) e de `DarkestDungeon.Domain`.
- `Api` e `Application` NÃO conhecem `DbContext`, `DbSet`, `Microsoft.EntityFrameworkCore` ou
  qualquer tipo específico do provedor SQL Server.

Um teste dedicado em `DarkestDungeon.Architecture.Tests` (`HierarquiaExtensibilidadeTests`)
valida essas regras estaticamente via NetArchTest para as novas classes/interfaces desta
feature.

## Required Interfaces (Application/Abstractions)

Adiciona as seguintes abstrações em `DarkestDungeon.Application/Abstractions`:

- `IClasseRepository` — CRUD leitura das 20 classes por `Id` e por `ClasseDeHeroi`.
- `IClasseService` — orquestra `ObterCatalogo`, `ObterPorId`, `ObterHabilidadesDaClasse`.
- `IHabilidadeRepository` — CRUD com filtros por categoria e por classe; garante unicidade
  global do nome.
- `IHabilidadeService` — orquestra criação por categoria (`CriarDeCombate`,
  `CriarDeAcampamento`, `CriarDeInimigo`), consultas por id e listagens filtradas.
- `IPersonagemRepository` — persistência de `Personagem`, inclusive owned types e
  habilidades associadas.
- `IPersonagemService` — orquestra `CriarPersonagem` (copiando resistências da Classe,
  validando que cada `HabilidadeId` atribuída pertence à associação Classe × Habilidade da
  Classe do Personagem, e que o total respeita 6 habilidades de Combate + 6 de Acampamento) e
  `EquiparPersonagem` (validando classe elegível de Arma/Armadura/Acessório).
- `IInimigoRepository` — persistência de `Inimigo` e associação com `HabilidadeDeInimigo`.
- `IInimigoService` — orquestra `CriarInimigo` e `ObterInimigo`.
- `IItemRepository` — persistência TPH de `Arma`, `Armadura` e `Acessorio`.
- `IItemService` — orquestra `CriarArma`, `CriarArmadura`, `CriarAcessorio`, `ObterItem`.
- `IMapaDeCoberturaRepository` — leitura e escrita de `EntradaDoMapaDeCobertura`.
- `IMapaDeCoberturaService` — orquestra `ObterMapaCompleto`, `ObterMapaPorClasse`,
  `RegistrarEstado`.

Cada contrato expõe apenas tipos de `DarkestDungeon.Domain` (entidades, enums e Owned Types) e
tipos primitivos ou DTOs definidos em `DarkestDungeon.Application` — nunca tipos de EF Core ou
do provedor.

## DI Registration Checklist

- `AddApplicationServices` (novo módulo `AdicionarServicosDoCatalogo`) registra: `IClasseService`,
  `IHabilidadeService`, `IPersonagemService`, `IInimigoService`, `IItemService`,
  `IMapaDeCoberturaService`.
- `AddInfrastructureServices` (novo módulo `AdicionarRepositoriosDoCatalogo`) registra:
  `IClasseRepository`, `IHabilidadeRepository`, `IPersonagemRepository`,
  `IInimigoRepository`, `IItemRepository`, `IMapaDeCoberturaRepository`.
- `Program.cs` continua chamando os dois módulos existentes e os dois novos.
- Um teste em `DarkestDungeon.Architecture.Tests` (`DependencyInjectionTests`) valida
  resolução dos novos contratos via `IServiceProvider.BuildServiceProvider().GetRequiredService`.

## Validation Contract

- Regras de validação de entrada (comprimento, ranges, enums) ficam em
  `DarkestDungeon.Application/Validation` e são compartilhadas entre serviços e controllers.
- Regras de invariantes de domínio (por exemplo: `Habilidade Equipada => Treinada` para
  `HabilidadeDePersonagem`) ficam encapsuladas nos próprios agregados de domínio.
- Regras de integridade referencial (por exemplo: `Arma.ClasseElegivel = Personagem.Classe`)
  são conferidas em `PersonagemService` antes de persistir; `IPersonagemRepository` NÃO
  duplica essa verificação.
- Regras específicas do Personagem (limite 6 habilidades de Combate + 6 de Acampamento e
  pertenência à associação Classe × Habilidade) ficam em `PersonagemService`; falhas são
  reportadas como 400 seguindo o schema `ErroResponse` (`mensagem` obrigatória em PT-BR e
  `campo` opcional).
- Todas as respostas 400/404 dos novos endpoints MUST usar o mesmo contrato `ErroResponse`
  herdado da feature 001; um teste dedicado (`ContratoErroResponseTests`) valida o schema.
- Exclusão de `Classe`, `Habilidade` e `Item` está fora do escopo desta feature. O
  `DarkestDungeonDbContext` MUST configurar todas as FKs desses catálogos com
  `OnDelete(DeleteBehavior.Restrict)`. Um teste dedicado (`IntegridadeReferencialTests`)
  valida que uma tentativa de excluir uma `Classe`/`Habilidade`/`Item` referenciado falha via
  banco.

## Contract Test Surface

- `HabilidadesEndpointsTests` — cria por categoria, retorna 400 quando nome global já existe,
  retorna 404 quando id inexistente.
- `ClassesEndpointsTests` — retorna 20 classes; retorna 200 para uma classe específica;
  retorna 404 para id desconhecido.
- `PersonagensEndpointsTests` — cria Personagem, valida cópia das resistências da Classe,
  rejeita 400 quando habilidades atribuídas não pertencem à Classe do Personagem, rejeita
  400 quando o limite de 6 habilidades de Combate ou 6 de Acampamento é ultrapassado;
  `PersonagensEquipamentoTests` cobre 200 ao equipar item da mesma classe e 400 quando classe
  não bate.
- `ItensEndpointsTests` — cobre POSTs por tipo, valida que Armas/Armaduras exigem exatamente 5
  níveis.
- `InimigosEndpointsTests` — POST + GET com associação a habilidades de inimigo.
- `MapaDeCoberturaEndpointsTests` — cobre leitura completa e por classe, validando três
  estados possíveis.
- `ContratoOpenApiTests` — valida presença dos schemas e paths listados em
  [openapi.yaml](openapi.yaml).
- `ContratoErroResponseTests` — valida que toda resposta 400 e 404 dos endpoints desta
  feature devolve o schema `ErroResponse` (`mensagem` string em PT-BR obrigatória, `campo`
  opcional).
- `IntegridadeReferencialTests` — valida via Testcontainers SQL Server que qualquer tentativa
  de excluir uma `Classe`, `Habilidade` ou `Item` referenciados falha com `DbUpdateException`
  causado por `OnDelete(Restrict)`.
