# Quickstart: Entidades e Catálogo de Habilidades

Este guia valida a feature depois da implementação. O contrato HTTP completo está em [contracts/openapi.yaml](contracts/openapi.yaml), as interfaces estão em [contracts/interfaces.md](contracts/interfaces.md) e o modelo está em [data-model.md](data-model.md).

## Pré-requisitos

- .NET SDK 10 instalado.
- SQL Server acessível para integração ou ambiente de testes configurado.
- String de conexão fornecida por variável de ambiente, sem segredo versionado.

## Configuração

```powershell
$env:ConnectionStrings__DarkestDungeonDb = "Server=localhost;Database=DarkestDungeon;User Id=sa;Password=<senha>;TrustServerCertificate=True"
```

## Comandos

```powershell
dotnet restore
dotnet ef database update --project src/DarkestDungeon.Infrastructure --startup-project src/DarkestDungeon.Api
dotnet test
dotnet run --project src/DarkestDungeon.Api
```

## Cenários de validação

### 1. Catálogo de Habilidade

- Criar uma Habilidade de Combate acessível a Personagem.
- Criar uma Habilidade de Acampamento compartilhada.
- Consultar `GET /habilidades` e confirmar as duas categorias.
- Consultar `GET /habilidades?categoria=Combate` e confirmar que nenhuma habilidade de Acampamento aparece.
- Tentar criar habilidade sem nome ou categoria inválida e confirmar `400` em PT-BR.

### 2. Configuração de classe

- Associar até seis habilidades de Combate e até seis de Acampamento a uma classe.
- Consultar `GET /classes-personagens/{classe}/habilidades`.
- Tentar adicionar uma sétima habilidade padrão ou habilidade exclusiva de Inimigo.
- Confirmar `400` e preservação da configuração anterior.

### 3. Criação de Personagem

- Criar um Personagem com uma classe configurada.
- Confirmar `201`, `Location` e atribuição automática das habilidades.
- Confirmar que habilidades de Acampamento iniciam com `treinada=false` e `equipada=false`.
- Treinar habilidades e tentar equipar quatro; a quarta deve ser rejeitada.
- Criar/consultar Personagem com Stress entre 0 e 200 e Chance de Virtude entre 0 e 100.
- Confirmar que Aflição e Virtude não podem existir simultaneamente.
- Confirmar Inventário padrão com quatro espaços e referências de Item válidas.

### 4. Criação de Inimigo

- Criar Inimigo do tipo Humano com zero habilidades.
- Criar Inimigo com quantidade arbitrária de habilidades, incluindo uma exclusiva de Inimigo.
- Confirmar que a resistência efetiva é base 5 mais a resistência própria.
- Confirmar que a resistência própria acima de 100 é rejeitada e que o total efetivo pode superar 100.

### 5. Item e equipamentos

- Criar Item com nome válido e consultar pelo ID.
- Usar Item como Armadura, Arma, Acessório ou entrada do Inventário.
- Tentar criar Item sem nome e confirmar `400` sem registro parcial.

### 6. Persistência e arquitetura

- Criar Habilidade, configuração, Personagem, Inimigo e Item.
- Reiniciar a aplicação e consultar cada registro novamente.
- Executar testes de arquitetura e confirmar que Domain não referencia API/Application/Infrastructure e Application não referencia API/Infrastructure.
- Executar testes de resolução de DI e confirmar que todos os serviços da feature iniciam corretamente.
