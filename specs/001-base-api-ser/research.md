# Research: Base de API e Recurso Ser

## Decision: Usar .NET 10 para o backend

**Rationale**: O SDK 10.0.400 e o runtime ASP.NET Core 10.0.11 estão instalados
no ambiente. .NET 10 é adequado para uma API backend nova e permite começar com
uma base moderna sem instalar outro SDK.

**Alternatives considered**: .NET 8 seria uma opção estável e instalada, mas o
ambiente já possui .NET 10. Usar outra stack violaria a preferência técnica do
projeto.

## Decision: Organizar a solução em camadas explícitas

**Rationale**: A constituição exige separação entre apresentação/API,
aplicação, domínio e infraestrutura. Projetos separados deixam dependências
mais claras e impedem que controladores acessem diretamente regras de domínio
ou detalhes de SQL Server.

**Alternatives considered**: Um único projeto seria mais rápido para começar,
mas aumentaria o risco de misturar responsabilidades logo na base do projeto.

## Decision: Usar contratos por interface entre camadas

**Rationale**: A especificação exige baixo acoplamento e facilidade de evolução.
A camada Application deve depender de abstrações para repositórios e serviços,
enquanto Infrastructure fornece implementações concretas. Isso permite trocar
persistência, adicionar derivados de Ser e testar regras sem acoplar domínio ou
casos de uso à API.

**Alternatives considered**: Injetar classes concretas diretamente reduziria
arquivos no início, mas aumentaria retrabalho ao adicionar Monstro, Personagem,
Chefe ou novas implementações de persistência.

## Decision: Centralizar registros de DI por camada

**Rationale**: O contêiner de injeção de dependência deve ser o ponto explícito
de composição da aplicação. Cada camada expõe um método de registro próprio, e
a API compõe esses registros no startup. Um teste de inicialização deve validar
que os serviços principais resolvem corretamente.

**Alternatives considered**: Registrar serviços diretamente em `Program.cs` foi
rejeitado porque espalha composição e dificulta revisar dependências por camada.

## Decision: Validar arquitetura com teste automatizado

**Rationale**: A especificação exige 0 dependências diretas do domínio para API
ou infraestrutura. Um teste arquitetural com NetArchTest ou ArchUnitNET torna
essa regra verificável e impede regressões quando novos tipos derivados forem
adicionados.

**Alternatives considered**: Revisão manual seria possível, mas frágil e fácil
de esquecer durante alterações futuras.

## Decision: Usar EF Core com provider SQL Server

**Rationale**: SQL Server é o banco oficial definido pela constituição e EF Core
atende ao pedido técnico do usuário com migrações versionadas, consultas
parametrizadas e isolamento da persistência na camada Infrastructure.

**Alternatives considered**: Acesso direto com ADO.NET seria simples, mas
reduziria produtividade e aumentaria repetição. SQLite em desenvolvimento foi
rejeitado como banco principal porque não representa o contrato oficial de
persistência.

## Decision: Implementar contrato base por ID como abstração reutilizável

**Rationale**: O pedido exige um endpoint genérico com ID que os demais recursos
herdem. Em ASP.NET Core, isso será representado por uma entidade base
identificável, um serviço genérico de consulta e um controller base para o
padrão `GET /{recurso}/{id}`. O recurso `Ser` especializa esse padrão.

**Alternatives considered**: Criar somente `GET /ser/{id}` sem abstração seria
mais curto, mas não entregaria a reutilização solicitada para recursos futuros.

## Decision: Modelar Ser como entidade base herdável

**Rationale**: A especificação define `Ser` como tipo genérico para futuras
especializações como Monstro, Personagem e Chefe. A camada Domain deve manter
`Ser` como entidade base estável, com os atributos compartilhados, para que
especializações futuras adicionem comportamento próprio sem duplicar campos.

**Alternatives considered**: Modelar Monstro, Personagem e Chefe já neste
escopo aumentaria a implementação antes de haver requisitos próprios para cada
tipo. Criar apenas uma entidade final `Ser` impediria a extensão pedida.

## Decision: Representar dano base como range mínimo/máximo

**Rationale**: O domínio usa dano variável como exemplo `11-17`, então o modelo
deve persistir `DanoBaseMinimo` e `DanoBaseMaximo` e validar que o mínimo não
exceda o máximo. Isso torna o contrato testável sem depender de texto livre.

**Alternatives considered**: Um campo único `DanoBase` perdeu precisão após o
refinamento. Uma string como `11-17` foi rejeitada por dificultar validação,
consulta e ordenação.

## Decision: Agrupar resistências como value object

**Rationale**: Atordoamento, Sangramento, Envenenamento, Debuff e Movimento são
resistências específicas com a mesma faixa de 0 a 100. Agrupá-las como
`Resistencias` no domínio mantém coesão e reduz repetição de validação.

**Alternatives considered**: Campos soltos diretamente em `Ser` seriam simples,
mas deixariam regras repetidas e menos explícitas. Uma tabela separada por tipo
de resistência foi rejeitada para o escopo inicial por ser flexível demais para
um conjunto fixo de cinco resistências.

## Decision: Testar endpoints com WebApplicationFactory e SQL Server em container

**Rationale**: A constituição exige testes de backend conferindo endpoints e
retornos. `WebApplicationFactory` permite testar a API em memória, enquanto
Testcontainers com SQL Server valida o caminho real de persistência sem depender
de um banco manual compartilhado.

**Alternatives considered**: Testes unitários puros seriam rápidos, mas não
validariam contratos HTTP. Banco em memória do EF Core foi rejeitado para os
testes principais porque pode mascarar diferenças do SQL Server.

## Decision: Mensagens de resposta em PT-BR

**Rationale**: A constituição define PT-BR como idioma do produto. Respostas de
erro como ID inválido, registro não encontrado e validação de Ser devem ser
estáveis, humanas e em português.

**Alternatives considered**: Mensagens técnicas em inglês foram rejeitadas por
conflitar com a constituição e prejudicar consistência do produto.
