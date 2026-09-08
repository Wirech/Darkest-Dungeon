# Darkest Dungeon API

Backend em .NET 10 para modelar entidades do domínio de Darkest Dungeon. A base
usa arquitetura em camadas, SQL Server como persistência oficial e endpoints em
PT-BR.

## Estrutura

```text
src/
├── DarkestDungeon.Api
├── DarkestDungeon.Application
├── DarkestDungeon.Domain
└── DarkestDungeon.Infrastructure

tests/
├── DarkestDungeon.Api.Tests
├── DarkestDungeon.Architecture.Tests
└── DarkestDungeon.Domain.Tests
```

## Configuração

Configure a connection string por variável de ambiente:

```powershell
$env:ConnectionStrings__DarkestDungeonDb = "Server=localhost;Database=DarkestDungeon;Trusted_Connection=True;TrustServerCertificate=True"
```

Para SQL Server com usuário e senha:

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

## Endpoints

- `GET /health`
- `POST /seres`
- `GET /seres/{id}`

Consulte [docs/api-ser.md](docs/api-ser.md) para exemplos do contrato de `Ser`.
