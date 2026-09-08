# Implementation Plan: Hierarquia de Entidades e Catálogo Oficial de Heróis

**Branch**: `003-hierarquia-catalogo-herois` | **Date**: 2026-09-07 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/003-hierarquia-catalogo-herois/spec.md`

## Summary

Consolidar a hierarquia de entidades solicitada (`Identificavel → Ser → Personagem/Inimigo`,
`Identificavel → Habilidade → Habilidade de Herói/Habilidade de Inimigo`,
`Identificavel → Item → Arma/Armadura/Acessório`) e materializar o catálogo controlado das 20
classes oficiais de heróis com suas habilidades de Combate e de Acampamento, seus efeitos
estruturados, suas resistências base e o Mapa de Cobertura que registra, por atributo,
`Coletado`, `Pendente` ou `NaoAplicavel`. Como a feature 002 ficou apenas em nível de spec,
esta feature também implementa Personagem, Inimigo, Item e Habilidade, unindo 002 e 003 em uma
única entrega compatível com as regras já estabelecidas em 001 e com a constituição.

Refinamentos derivados de `/speckit-clarify` (Session 2026-09-07): (a) Individualidades e
Doenças de Personagem estão fora do escopo desta feature; (b) cada Personagem MUST respeitar
limite de até 6 habilidades de Combate e até 6 habilidades de Acampamento (FR-009); (c) toda
habilidade atribuída a um Personagem MUST pertencer à associação Classe × Habilidade da Classe
do Personagem (FR-009a); (d) `ConjuntoId` de Acessório é apenas metadata nesta feature, o
cálculo do bônus fica para feature futura (FR-013); (e) todas as respostas de erro
(400/404) MUST reutilizar o contrato `ErroResponse` da feature 001 (FR-025); (f)
`HabilidadeDePersonagem` mantém referência viva a `Habilidade` — esta feature não expõe
`PUT/PATCH` de habilidade; (g) as sete raridades de Acessório formam conjunto fechado
(FR-021); (h) exclusão de Classe/Habilidade/Item está fora de escopo, com todas as FKs
protegidas por `OnDelete(Restrict)` (FR-023); (i) a inclusão de uma 21ª classe segue
procedimento documentado (novo valor no enum + migração + seed + Mapa de Cobertura).

## Technical Context

**Language/Version**: C# 14 com .NET SDK 10.0.400 (mesma linha da feature 001).

**Primary Dependencies**: ASP.NET Core 10 (controllers já em uso); Entity Framework Core 10 com
`Microsoft.EntityFrameworkCore.SqlServer` para persistência e `Owned Types` para componentes
(Resistências, Efeito de Habilidade, Nível de Arma, Nível de Armadura); `Swashbuckle.AspNetCore`
para OpenAPI; `Microsoft.AspNetCore.Mvc.Testing` + xUnit + FluentAssertions para testes de
endpoint; `Testcontainers.MsSql` para o caminho SQL Server real; `NetArchTest.Rules` para
testes de dependência entre camadas; `Microsoft.EntityFrameworkCore.InMemory` apenas no
ambiente de teste `Testing` (padrão já estabelecido em 001).

**Storage**: SQL Server como banco oficial. Hierarquia de `Habilidade` e de `Item` mapeada como
`TPH` (Table-Per-Hierarchy) com coluna discriminadora, para manter integridade referencial
simples entre Classe × Habilidade e Personagem × Item equipado. Componentes (Nível de Arma,
Nível de Armadura, Efeito de Habilidade, Resistências de Classe) mapeados como `OwnsOne` ou
`OwnsMany`, sem chave própria exposta.

**Testing**: xUnit para domínio; xUnit + WebApplicationFactory para endpoints; xUnit +
Testcontainers para persistência real; xUnit + NetArchTest para dependências entre camadas;
xUnit para DI (validação de resolução via `IServiceProvider.BuildServiceProvider`).

**Target Platform**: Servidor Windows ou Linux com .NET 10, acessível via internet ou VPN,
mesma constraint operacional das features 001 e 002.

**Project Type**: Web service backend em camadas (Api, Application, Domain, Infrastructure) e
projetos de teste (Api.Tests, Architecture.Tests, Domain.Tests).

**Performance Goals**: consulta do catálogo por classe (habilidades + resistências) responde em
até 500 ms com o catálogo completo (20 classes) carregado em memória; consulta pontual de uma
habilidade por nome responde em até 200 ms; criação de Personagem que copia resistências base
da Classe responde em até 500 ms.

**Constraints**: mensagens de usuário em PT-BR seguindo o contrato `ErroResponse` de 001
(`mensagem` obrigatória, `campo` opcional) em todas as respostas 400/404; segredos fora do
repositorio; endpoints stateless; validação de entrada obrigatória; contratos por interface
entre camadas; DI validada na inicialização; nenhuma alteração em `Ser` (feature 001) — as
três resistências novas ficam apenas em Personagem e em Classe; toda `Arma`/`Armadura` tem
exatamente uma classe elegível; toda habilidade tem nome único global; cada Personagem MUST
respeitar limite de 6 habilidades de Combate + 6 de Acampamento e MUST usar apenas
habilidades da própria Classe via associação Classe × Habilidade; `ConjuntoId` de Acessório
tratado somente como metadata (sem cálculo de bônus); Individualidades e Doenças fora do
escopo; `HabilidadeDePersonagem` mantém referência viva a `Habilidade` (sem endpoint
`PUT/PATCH` nesta feature); raridades de Acessório formam conjunto fechado de exatamente 7
valores; exclusão de Classe/Habilidade/Item está fora de escopo e todas as FKs entre esses
catálogos MUST usar `OnDelete(Restrict)`; `Coletado`, `Pendente` e `NaoAplicavel` são estados
distintos.

**Scale/Scope**: 20 classes, aproximadamente 7 habilidades de Combate por classe e 4
habilidades de Acampamento únicas por classe + 3 compartilhadas (~220 habilidades de herói),
mais habilidades de Inimigo cadastradas conforme necessidade. Cada Arma e cada Armadura tem
lista fixa de cinco níveis. Cada Efeito de Habilidade é uma linha estruturada; uma habilidade
pode ter várias linhas.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Evidence |
|-----------|--------|----------|
| I. Arquitetura em Camadas | PASS | Novas entidades adicionadas em `DarkestDungeon.Domain`; casos de uso em `DarkestDungeon.Application`; persistência e EF Core em `DarkestDungeon.Infrastructure`; endpoints em `DarkestDungeon.Api`; sem herança lateral entre camadas. |
| II. SQL Server como Persistência Oficial | PASS | Todas as novas tabelas mapeadas por EF Core no `DarkestDungeonDbContext`; migrações versionadas; testes de integração via Testcontainers SQL Server; todas as FKs entre Classe/Habilidade/Item usam `OnDelete(Restrict)` para bloquear exclusão (fora de escopo). |
| III. Contratos de Backend Verificáveis | PASS | Todos os novos endpoints têm testes de contrato (status + body) em `DarkestDungeon.Api.Tests`; contratos internos documentados em [contracts/interfaces.md](contracts/interfaces.md); teste `ContratoErroResponseTests` garante que todas as respostas 400/404 seguem o schema `ErroResponse`. |
| IV. Português do Brasil como Idioma do Produto | PASS | Nomes de negócio e mensagens de erro em PT-BR via `ErroResponse` da feature 001; nomes originais em inglês mantidos como campo de rastreabilidade. |
| V. Operação Remota e Simplicidade Proporcional | PASS | Endpoints stateless; configuração por ambiente; nenhuma nova dependência externa obrigatória além do SQL Server; abstrações mantidas proporcionais ao domínio real. |
| Evolução e DI (constituição pós-design 001) | PASS | Novas abstrações adicionadas em `DarkestDungeon.Application/Abstractions`; registro por camada mantido; teste de DI validado antes de exercitar endpoints. |

Sem violação de gate.

## Project Structure

### Documentation (this feature)

```text
specs/003-hierarquia-catalogo-herois/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── dados-minerados.md
├── contracts/
│   ├── interfaces.md
│   └── openapi.yaml
└── tasks.md
```

### Source Code (repository root)

```text
src/
├── DarkestDungeon.Api/
│   ├── Controllers/ (adiciona ClassesController, HabilidadesController, PersonagensController, InimigosController, ItensController, MapaDeCoberturaController)
│   ├── Contracts/ (adiciona contratos HTTP das novas entidades)
│   ├── Extensions/ (atualiza ServiceCollectionExtensions com os novos registros)
│   └── Program.cs (adiciona metadata OpenAPI dos novos endpoints)
├── DarkestDungeon.Application/
│   ├── Abstractions/ (adiciona IClasseService/Repository, IHabilidadeService/Repository, IPersonagemService/Repository, IInimigoService/Repository, IItemService/Repository, IMapaDeCoberturaService/Repository)
│   ├── Classes/, Habilidades/, Personagens/, Inimigos/, Itens/, Cobertura/ (novos DTOs, comandos e serviços)
│   ├── Seres/ (permanece)
│   └── Validation/ (permanece)
├── DarkestDungeon.Domain/
│   ├── Seres/ (Ser inalterado; adiciona Personagem, Inimigo)
│   ├── Classes/ (Classe, ClasseDeHeroi enum, ResistenciasDeClasse, TipoDeInimigo)
│   ├── Habilidades/ (Habilidade base abstrata, HabilidadeDeHeroi abstrata, HabilidadeDeCombate, HabilidadeDeAcampamento, HabilidadeDeInimigo, EfeitoDeHabilidade owned, ClasseHabilidade associação)
│   ├── Itens/ (Item abstrato, Arma, Armadura, Acessorio, NivelDeArma/Armadura owned, EfeitoDeAcessorio owned, RaridadeDeAcessorio enum)
│   ├── Personagens/ (ResistenciasExtrasDePersonagem, HabilidadeDePersonagem, Inventario)
│   └── Cobertura/ (EstadoDeAtributo enum, EntradaDoMapaDeCobertura)
└── DarkestDungeon.Infrastructure/
    ├── Data/DarkestDungeonDbContext.cs (adiciona DbSets e mapeamentos TPH/Owned)
    ├── Data/Migrations/ (nova migração AddCatalogoHeroisEEntidades)
    └── Repositories/ (adiciona ClasseRepository, HabilidadeRepository, PersonagemRepository, InimigoRepository, ItemRepository, MapaDeCoberturaRepository)

tests/
├── DarkestDungeon.Api.Tests/ (adiciona testes de endpoint para Classes, Habilidades, Personagens, Inimigos, Itens, MapaDeCobertura, teste de contrato OpenAPI e teste ContratoErroResponseTests para o schema PT-BR)
├── DarkestDungeon.Architecture.Tests/ (atualiza DI e adiciona HierarquiaExtensibilidadeTests)
└── DarkestDungeon.Domain.Tests/ (adiciona testes para Classe, Habilidade, EfeitoDeHabilidade, Personagem — incluindo limite 6+6 e validação habilidade ⊆ Classe —, Arma, Armadura, Acessorio e MapaDeCobertura)
```

**Structure Decision**: Ampliar a solução já existente em quatro projetos principais adicionando
novos arquivos por camada; sem introduzir projetos novos e sem alterar `Ser` ou os artefatos já
implementados na feature 001. A composição de DI segue a mesma divisão por camada
(`AddApplicationServices` / `AddInfrastructureServices`), com validação de resolução mantida.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| N/A | Sem violação de gate constitucional identificada. | N/A |

## Phase 0: Research

See [research.md](research.md).

## Phase 1: Design & Contracts

See [data-model.md](data-model.md), [contracts/openapi.yaml](contracts/openapi.yaml),
[contracts/interfaces.md](contracts/interfaces.md), and [quickstart.md](quickstart.md).

## Post-Design Constitution Check

| Principle | Status | Evidence |
|-----------|--------|----------|
| Arquitetura em Camadas | PASS | Data model mantém Domain isolado; EF Core e mapeamento TPH ficam em Infrastructure; contratos HTTP em Api; nenhuma referência lateral. |
| SQL Server | PASS | `data-model.md` descreve mapeamento SQL Server via EF Core (TPH + Owned Types); quickstart descreve `dotnet ef database update` para aplicar a migração. |
| Testes de Backend | PASS | `openapi.yaml` documenta contratos com casos de sucesso, validação e não encontrado; quickstart e tasks exigem testes de endpoint para cada rota. |
| PT-BR | PASS | Contratos e exemplos usam PT-BR; nomes de negócio em PT-BR com rastreabilidade para nomes originais. |
| Operação Remota | PASS | Endpoints stateless; nenhuma nova credencial obrigatória; DI validada na inicialização; SQL Server mantido como persistência oficial. |
| Evolução e DI | PASS | Novas abstrações adicionadas em `DarkestDungeon.Application/Abstractions`; registros por camada; testes de DI e arquitetura cobrem os novos contratos. |
</newString>
</invoke>
