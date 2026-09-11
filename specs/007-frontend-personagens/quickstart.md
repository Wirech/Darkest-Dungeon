# Quickstart: Frontend Interativo de Personagens

## Pré-requisitos

- SDK .NET 10 instalado.
- SQL Server configurado conforme o README.
- Banco com migrações e seeds aplicados.
- `ConnectionStrings__DarkestDungeonDb` configurada quando necessário.

## Executar

```powershell
dotnet restore
dotnet run --project src/DarkestDungeon.Api --launch-profile http
```

Abrir a página de personagens em `http://localhost:5140/personagens/index.html`.
O endereço `GET /personagens` permanece reservado à lista JSON usada pela interface.

## Validação manual principal

1. Abrir a tela e confirmar lista ou estado vazio.
2. Confirmar a indicação de carregamento durante a consulta.
3. Selecionar `Criar personagem`.
4. Tentar salvar sem nome e confirmar o erro junto ao campo.
5. Selecionar uma classe, preencher atributos válidos e salvar.
6. Confirmar que o novo personagem aparece sem recarregar manualmente.
7. Selecionar `Excluir`, cancelar e confirmar que o personagem permanece.
8. Repetir a exclusão, confirmar e verificar que o personagem desaparece.
9. Excluir o último personagem e confirmar o retorno ao estado vazio.
10. Simular indisponibilidade do serviço e confirmar mensagem PT-BR e nova tentativa.

## Validação automatizada do backend

```powershell
dotnet test tests/DarkestDungeon.Api.Tests/DarkestDungeon.Api.Tests.csproj --no-restore
dotnet test tests/DarkestDungeon.Domain.Tests/DarkestDungeon.Domain.Tests.csproj --no-restore
dotnet test tests/DarkestDungeon.Architecture.Tests/DarkestDungeon.Architecture.Tests.csproj --no-restore
dotnet build DarkestDungeon.sln --no-restore --configuration Release
```

Os testes de API devem cobrir `GET /personagens`, `POST /personagens` e `DELETE /personagens/{id}`, incluindo sucesso, lista vazia, validação, ID inexistente e falha de exclusão.

## Validação responsiva

Repetir os passos principais em viewport estreita e larga. Nome, classe, métricas, mensagens e ações devem permanecer legíveis e acessíveis sem sobreposição ou rolagem horizontal obrigatória.

## Smoke test confirmado em 2026-09-10

- `GET /personagens/index.html`: `200 OK`, documento HTML servido.
- `GET /personagens/personagens.css`: `200 OK`.
- `GET /personagens/personagens.js`: `200 OK`.
- `GET /personagens`: `200 OK`, lista JSON.
- `node --check src/DarkestDungeon.Api/wwwroot/personagens/personagens.js`: passou.