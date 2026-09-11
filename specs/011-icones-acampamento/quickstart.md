# Guia de Validação: Ícones de Acampamento

## Pré-requisitos

- .NET SDK 10.
- Instalação Steam de Darkest Dungeon (vanilla + DLCs do seed).
- Acervo 004 já em `assets/herois`.

## 1. Testar

```powershell
dotnet test DarkestDungeon.sln --filter "FullyQualifiedName~ColetaMidias|FullyQualifiedName~Feature010|FullyQualifiedName~Feature011"
```

## 2. Simular

```powershell
dotnet run --project tools/DarkestDungeon.MediaCollector -- --origem "C:\Program Files (x86)\Steam\steamapps\common\DarkestDungeon" --saida .\assets\herois --camping --simular
```

Esperado: conta arquivos, não grava.

## 3. Importar e mesclar

```powershell
dotnet run --project tools/DarkestDungeon.MediaCollector -- --origem "C:\Program Files (x86)\Steam\steamapps\common\DarkestDungeon" --saida .\assets\herois --camping --continuar
```

Esperado: `arquivos/acampamento/camp_skill_*.png`; inventário 004 preservado; manifesto com 79 associações camping.

## 4. Card

Com a API em `http://localhost:5140`, `GET /personagens`: skills de acampamento com `midia.status=OK`. Combate inalterado.

## Resultado

Executado em 2026-09-11 contra `C:\Program Files (x86)\Steam\steamapps\common\DarkestDungeon`:

```
79 arquivos de acampamento inventariados; 0 lacunas.
```

- `assets/herois/arquivos/acampamento/camp_skill_*.png`: 79 arquivos.
- Inventário: 2724 entradas (2645 da 004 + 79 camping). Leftovers (`bandage`, `bear_traps`, `hobby`, `perimeter_alarms`, `wrap`) não copiados.
- Manifesto: 219 associações (140 combate + 79 camping).
