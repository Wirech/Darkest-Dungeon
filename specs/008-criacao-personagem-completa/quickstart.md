# Quickstart: Criação Completa de Personagem

## Pré-requisitos

- SDK .NET 10 instalado.
- SQL Server configurado conforme o README.
- Migrações aplicadas, incluindo a alteração de atributos do personagem.
- Seeds de classes, habilidades, armas e armaduras carregados.
- `ConnectionStrings__DarkestDungeonDb` configurada quando necessário.

## Executar

```powershell
dotnet restore
dotnet run --project src/DarkestDungeon.Api --launch-profile http
```

Abrir o Swagger ou a interface de personagens publicada pela API.

## Cenário válido

1. Consultar `/classes` e escolher uma classe existente.
2. Escolher nível do herói `3`, nível de arma `2`, nível de armadura `3` e aparência `B`.
3. Enviar `POST /personagens` somente com nome e essas cinco escolhas; enums usam os valores numéricos atuais do contrato (`classe` e `aparencia`).
4. Confirmar `201 Created`.
5. Consultar o personagem pelo `Location` retornado.
6. Confirmar classe, nível, aparência, IDs dos equipamentos e níveis selecionados.
7. Separar as habilidades por categoria e confirmar exatamente quatro combate e quatro acampamento com nível 1.
8. Confirmar que todas as habilidades restantes dessas categorias estão no nível 0 e `Treinada=false`.

## Cenários inválidos

- Nível do herói `-1` ou `7`: esperar `400` e nenhum registro novo.
- Nível da arma/armadura `0` ou `6`: esperar `400` e nenhum registro novo.
- Aparência fora de `A`, `B`, `C`, `D`: esperar `400`.
- Classe com menos de quatro habilidades de combate/acampamento: esperar `400`.
- Catálogo sem arma ou armadura elegível para a classe: esperar `404` ou erro de catálogo documentado.
- Falha durante persistência: confirmar que não existe personagem parcial.

## Validação automatizada

```powershell
dotnet test tests/DarkestDungeon.Domain.Tests/DarkestDungeon.Domain.Tests.csproj --no-restore
dotnet test tests/DarkestDungeon.Architecture.Tests/DarkestDungeon.Architecture.Tests.csproj --no-restore
dotnet test tests/DarkestDungeon.Api.Tests/DarkestDungeon.Api.Tests.csproj --no-restore
dotnet build DarkestDungeon.sln --no-restore --configuration Release
```

Os testes devem cobrir criação válida, regra 4+4, bloqueio das habilidades restantes, aparência, níveis de equipamento, validações de faixa, catálogo insuficiente e ausência de persistência parcial.

## Compatibilidade

Consultar pelo menos um personagem criado antes da feature. Confirmar que a leitura não falha quando os novos campos não estavam preenchidos e que o sistema aplica os defaults documentados sem reescrever o registro automaticamente.
