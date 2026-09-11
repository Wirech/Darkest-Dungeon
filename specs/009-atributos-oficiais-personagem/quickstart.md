# Quickstart: Atributos Oficiais do Personagem

## Pré-requisitos

- SDK .NET 10.
- SQL Server conforme o README (`ConnectionStrings__DarkestDungeonDb` se necessário).
- Snapshots em `specs/009-atributos-oficiais-personagem/wiki-snapshots/` **completos** (20 classes). Se faltar arquivo ou campo, **parar** e decidir caso a caso — não inventar.
- Migração desta feature aplicada (passos no personagem; campos extras na classe; seed de arma/armadura oficial).
- Seeds de classes e habilidades já existentes.

## Cobertura local (antes de criar)

Confirmar 20 JSON com 5 níveis de arma, 5 de armadura, deslocamento frente/atrás, religiosa, provisão e bônus ao crítico. Abominação somente forma humana.

Reexecução do coletor (curadoria, **não** em runtime da API):

```powershell
dotnet run --project tools/DarkestDungeon.WikiCatalogCollector -- --saida specs/009-atributos-oficiais-personagem/wiki-snapshots
```

Esperado: saída 0 e 20 arquivos. Qualquer lacuna: saída ≠ 0 e relatório PT-BR.

## Subir a API

```powershell
dotnet restore
dotnet run --project src/DarkestDungeon.Api --launch-profile http
```

Abrir `/personagens/index.html`.

## Cenário válido (criação fiel)

1. Escolher classe conhecida (ex.: Cruzado), nome, nível do herói `3`, arma `2`, armadura `3`, aparência `B`.
2. `POST /personagens` só com esses seis campos (ver [contracts/personagens-atributos.md](contracts/personagens-atributos.md)).
3. Esperar `201`.
4. Conferir contra o snapshot local da classe (não contra este guia):
   - HP = MAX HP da armadura nível 3
   - esquiva da armadura 3; SPD/CRIT/DMG da arma 2
   - precisão 0, proteção 0, stress 0, virtude 25
   - passos frente/atrás iguais ao snapshot
   - seis resistências = base + 30 p.p. (nível 3), teto 100; golpe mortal e armadilha = base
5. No card da lista: seis categorias; habilidades de combate e acampamento **todas** com nível; nível 0 visível; cabeçalho com níveis/aparência; religiosa, provisão e bônus ao crítico da **classe**.

## Abominação

Criar Abominação com os mesmos seis campos. HP/SPD/DMG/CRIT/DODGE devem bater com o bloco **humano** do snapshot, nunca com o bloco besta.

## Cenários inválidos

- Snapshot/seed sem um nível: criação fiel recusa; nenhum personagem novo.
- Nível de herói fora de 0..6 ou equipamento fora de 1..5: `400`.
- Sem rede após o dump: criação e card continuam iguais (só catálogo local).

## Validação automatizada

```powershell
dotnet test tests/DarkestDungeon.Domain.Tests/DarkestDungeon.Domain.Tests.csproj --no-restore
dotnet test tests/DarkestDungeon.Architecture.Tests/DarkestDungeon.Architecture.Tests.csproj --no-restore
dotnet test tests/DarkestDungeon.Api.Tests/DarkestDungeon.Api.Tests.csproj --no-restore
dotnet build DarkestDungeon.sln --no-restore --configuration Release
```

Cobrir pelo menos: cobertura 20×5 dos snapshots commitados; Cruzado (ou amostra) HP/arma/armadura; 3 classes × 3 níveis de resolução nas resistências; precisão/proteção/stress 0; Abominação humana; lista/card com categorias e extras da classe; recusa se catálogo incompleto; personagens antigos sem recálculo.

## Compatibilidade

Ler um personagem criado antes desta feature. A leitura não pode falhar. O card mostra o gravado; HP/resistências antigos **não** são reescritos no GET.
