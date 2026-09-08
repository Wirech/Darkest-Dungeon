# Quickstart: Hierarquia e Catálogo de Heróis

Este quickstart complementa o quickstart da feature 001 e adiciona os cenários necessários
para validar a hierarquia de entidades e o catálogo oficial das 20 classes.

## Prerequisites

- .NET SDK 10.0.400 instalado (`dotnet --version` retorna `10.0.400`).
- SQL Server acessível via variável de ambiente `DARKEST_DUNGEON_SQL` (mesma variável de 001).
- Docker Desktop em execução para os testes de integração via Testcontainers.
- Repositório clonado com a branch `003-hierarquia-catalogo-herois` ativa.

## Setup Commands

```powershell
# 1. Restaurar dependências.
dotnet restore

# 2. Aplicar a migração nova ao banco SQL Server.
dotnet ef migrations add AddCatalogoHeroisEEntidades --project src/DarkestDungeon.Infrastructure --startup-project src/DarkestDungeon.Api
dotnet ef database update --project src/DarkestDungeon.Infrastructure --startup-project src/DarkestDungeon.Api

# 3. Rodar toda a suíte de testes (Domain + Api + Architecture).
dotnet test

# 4. Subir a API localmente (fica ouvindo na porta configurada em launchSettings.json).
dotnet run --project src/DarkestDungeon.Api
```

## Endpoint Validation Scenarios

### 1. Consultar as 20 classes cadastradas

```powershell
curl -s http://localhost:5000/classes | ConvertFrom-Json | Measure-Object
```

Esperado: contagem exatamente igual a 20.

### 2. Consultar as habilidades de uma classe (Highwayman → Bandido)

```powershell
$bandido = curl -s http://localhost:5000/classes | ConvertFrom-Json | Where-Object { $_.classe -eq 'Bandido' }
curl -s "http://localhost:5000/classes/$($bandido.id)/habilidades" | ConvertFrom-Json
```

Esperado: lista contendo habilidades de combate + habilidades de acampamento; `Gallows Humor`
aparece com a mesma id que na resposta de Ladrão de Cova.

### 3. Criar uma nova habilidade de combate

```powershell
$corpo = @{
    nomeExibicao = 'Corte de Sabre'
    nomeOriginal = 'Sabre Slash'
    descricao = 'Golpe rápido em posição avançada.'
    classes = @('Bandido')
    posicoesValidas = @(1, 2)
    posicoesQueAtinge = @(1, 2)
    alvoEmArea = $false
    modificadorDano = 10
    modificadorAcerto = 5
    modificadorCritico = 3
    efeitos = @(
        @{ nomeDoEfeito = 'Sangramento'; alvo = 'Inimigo'; valor = 3; unidade = 'Pontos'; duracaoEmRodadas = 3; chanceBase = 100 }
    )
} | ConvertTo-Json -Depth 4

curl -s -X POST http://localhost:5000/habilidades/combate -H 'Content-Type: application/json' -d $corpo
```

Esperado: 201 com corpo contendo `id` gerado; um `POST` subsequente com o mesmo `nomeExibicao`
retorna 400 com mensagem em PT-BR sobre nome duplicado.

### 4. Criar um Personagem e verificar cópia de resistências

```powershell
$corpo = @{ nome = 'Reynauld'; classe = 'Cruzado' } | ConvertTo-Json
$personagem = curl -s -X POST http://localhost:5000/personagens -H 'Content-Type: application/json' -d $corpo | ConvertFrom-Json
$personagem.resistencias
```

Esperado: bloco `resistencias` contém as 8 resistências e é idêntico ao `resistenciasBase`
da Classe Cruzado.

### 5. Criar uma Arma com cinco níveis e equipar no Personagem certo

```powershell
$niveis = 1..5 | ForEach-Object {
    @{ nivel = $_; danoMinimo = 5 + $_; danoMaximo = 8 + $_; critico = 4 + $_; velocidade = 3 + $_ }
}
$arma = curl -s -X POST http://localhost:5000/armas -H 'Content-Type: application/json' -d (@{
    nomeExibicao = 'Espada Cruzada'
    nomeOriginal = 'Crusader Sword'
    descricao = 'Espada padrão do Cruzado.'
    classeElegivel = 'Cruzado'
    niveis = $niveis
} | ConvertTo-Json -Depth 4) | ConvertFrom-Json

curl -s -X POST "http://localhost:5000/personagens/$($personagem.id)/equipar" -H 'Content-Type: application/json' -d (@{ armaId = $arma.id } | ConvertTo-Json)
```

Esperado: 200 com Personagem atualizado; se `classeElegivel` da Arma fosse `Bandido`, o mesmo
POST retornaria 400 com mensagem em PT-BR explicando a incompatibilidade.

### 6. Consultar o Mapa de Cobertura por classe

```powershell
curl -s "http://localhost:5000/mapa-de-cobertura/$($bandido.id)" | ConvertFrom-Json
```

Esperado: entradas com `estado` em `Coletado`, `Pendente` ou `NaoAplicavel` para as habilidades
de combate, acampamento e para as resistências base.

### 7. Rejeitar criação de Personagem com habilidade de outra Classe (FR-009a)

```powershell
$habilidadeDoOcultista = curl -s 'http://localhost:5000/habilidades?classe=Ocultista' | ConvertFrom-Json | Select-Object -First 1
$corpo = @{ nome = 'Reynauld'; classe = 'Cruzado'; habilidadesEquipadas = @($habilidadeDoOcultista.id) } | ConvertTo-Json
curl -i -X POST http://localhost:5000/personagens -H 'Content-Type: application/json' -d $corpo
```

Esperado: HTTP 400 com body `{ "mensagem": "...", "campo": "habilidadesEquipadas" }` em PT-BR.

### 8. Rejeitar criação de Personagem que ultrapasse o limite 6 + 6 (FR-009)

```powershell
$sete = 1..7 | ForEach-Object { "11111111-1111-1111-1111-11111111000$_" }
$corpo = @{ nome = 'Reynauld'; classe = 'Cruzado'; habilidadesEquipadas = $sete } | ConvertTo-Json
curl -i -X POST http://localhost:5000/personagens -H 'Content-Type: application/json' -d $corpo
```

Esperado: HTTP 400 com body `ErroResponse` em PT-BR citando o limite estourado (Combate ou
Acampamento).

## Persistence Validation

- Rodar `dotnet test tests/DarkestDungeon.Api.Tests` — os testes com `WebApplicationFactory`
  usam o provedor In-Memory no ambiente `Testing`.
- Rodar `dotnet test tests/DarkestDungeon.Api.Tests --filter Category=Integracao` — a suíte
  Testcontainers sobe um SQL Server real e valida a migração `AddCatalogoHeroisEEntidades`.
- Rodar `dotnet test --filter FullyQualifiedName~IntegridadeReferencialTests` — valida que
  tentativas de excluir `Classe`, `Habilidade` ou `Item` referenciados falham por
  `OnDelete(DeleteBehavior.Restrict)` sem endpoint HTTP para exclusão.

## Concurrency Validation

- O contrato dos endpoints é stateless; nenhum novo cenário de concorrência é introduzido.
- Testes com múltiplas chamadas paralelas de `POST /habilidades/*` cobrem a unicidade global de
  `nomeExibicao`.

## DI Validation

- `DependencyInjectionTests` resolve `IClasseService`, `IHabilidadeService`,
  `IPersonagemService`, `IInimigoService`, `IItemService`, `IMapaDeCoberturaService` a partir
  do container real, garantindo que nenhum contrato ficou sem implementação registrada.

## Architecture Validation

- `HierarquiaExtensibilidadeTests` valida via NetArchTest que:
  - `DarkestDungeon.Domain` não referencia `Microsoft.EntityFrameworkCore.*`.
  - `DarkestDungeon.Application` não referencia `Microsoft.EntityFrameworkCore.*`.
  - `DarkestDungeon.Api` não referencia `DarkestDungeon.Infrastructure` diretamente.
  - Novos agregados (`Personagem`, `Inimigo`, `Habilidade*`, `Item*`, `Classe`,
    `EntradaDoMapaDeCobertura`) residem em `DarkestDungeon.Domain`.
- `ContratoErroResponseTests` valida que toda resposta 400/404 dos novos endpoints devolve
  o schema `ErroResponse` (`mensagem` obrigatória em PT-BR, `campo` opcional).
