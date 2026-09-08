# Guia de Validação: Importação Local de Mídias

## Pré-requisitos

- .NET SDK 10.
- Instalação local legível de Darkest Dungeon.
- Caminho conhecido da instalação, por exemplo `C:\Program Files (x86)\Steam\steamapps\common\DarkestDungeon`.

## 1. Testar

```powershell
dotnet test DarkestDungeon.sln --no-restore
```

Resultado esperado: testes cobrem cópia byte a byte, atlas, conjunto incompleto, DLC e manifesto usando fixtures locais.

## 2. Simular uma classe

```powershell
dotnet run --project tools/DarkestDungeon.MediaCollector -- --origem "C:\Program Files (x86)\Steam\steamapps\common\DarkestDungeon" --saida .\artifacts\midias-piloto --classe Antiquarian --simular
```

Resultado esperado: lista conjuntos e regiões encontrados, sem criar saída.

## 3. Importar uma classe

```powershell
dotnet run --project tools/DarkestDungeon.MediaCollector -- --origem "C:\Program Files (x86)\Steam\steamapps\common\DarkestDungeon" --saida .\assets\herois --classe Antiquarian
```

Resultado esperado: PNG, atlas e skel da Antiquarian copiados sem alteração em `assets/herois/arquivos/antiquarian/`, com inventário válido.

## 4. Validar originalidade

Compare SHA-256 de uma textura, atlas e esqueleto na origem e no destino. Os valores devem ser iguais. Confirme no inventário a classe, conjunto, caminhos e região de atlas.

## 5. Importar catálogo instalado

```powershell
dotnet run --project tools/DarkestDungeon.MediaCollector -- --origem "C:\Program Files (x86)\Steam\steamapps\common\DarkestDungeon" --saida .\assets\herois --continuar
```

Resultado esperado: cada uma das 20 classes tem assets importados ou lacuna que identifica diretório base/DLC ausente ou conjunto incompleto.

## Resultado Executado (2026-09-08)

- `dotnet test DarkestDungeon.sln --no-restore`: 134 testes aprovados.
- Simulação e importação da Antiquarian: 133 arquivos originais, 24 conjuntos Spine e hashes de origem/destino idênticos.
- Importação completa: 2.645 arquivos, 20 classes e 463 conjuntos Spine.
- A árvore e o inventário usam nomes de classe em PT-BR; conjuntos incompletos permanecem registrados como lacunas.
