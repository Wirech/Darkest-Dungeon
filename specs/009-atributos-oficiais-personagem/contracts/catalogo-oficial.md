# Contrato: Catálogo oficial local (coleta e cobertura)

A wiki **não** é consultada por endpoints de personagem. Este contrato cobre a ferramenta de curadoria e a garantia de cobertura local.

## Coletor CLI

Projeto: `tools/DarkestDungeon.WikiCatalogCollector`

```powershell
dotnet run --project tools/DarkestDungeon.WikiCatalogCollector -- --saida specs/009-atributos-oficiais-personagem/wiki-snapshots
```

### Sucesso

- Gera 20 arquivos `{slug}.json` (um por `ClasseDeHeroi`).
- Cada arquivo contém arma 1..5, armadura 1..5, `passosAFrente`, `passosAtras`, `religiosa`, `provisaoInicial`, `bonusAoCritico`, `forma: humana`.
- Código de saída `0`.
- Mensagens em PT-BR.

### Falha (lacuna)

- Código de saída ≠ 0.
- Relatório lista classe, campo e motivo (ausente, ambíguo, não numérico, forma besta detectada sem bloco humano).
- **Não** grava snapshot parcial como “completo”.
- **Não** inventa nem interpola valores.

O coletor é de curadoria: roda fora da API. A criação **nunca** o invoca.

## Seed / cobertura local

Após snapshots válidos, o seed de infraestrutura materializa:

- 20 `Arma` (5 níveis oficiais)
- 20 `Armadura` (5 níveis oficiais; `HpAdicional` = MAX HP wiki)
- Campos novos em `Classe`

Reexecução é idempotente (IDs determinísticos). Musqueteiro tem registro próprio mesmo que os números coincidam com o Besteiro.

## Consulta de classe (join do card)

`GET /classes/{id}` deve passar a expor, além das resistências base:

```json
{
  "id": "...",
  "classe": 4,
  "nomeExibicao": "Cruzado",
  "nomeOriginal": "Crusader",
  "religiosa": true,
  "provisaoInicial": "...",
  "bonusAoCritico": "+15% PROT",
  "passosAFrente": 1,
  "passosAtras": 1,
  "resistenciasBase": { }
}
```

O card **pode** usar esses campos via join no servidor da lista; o cliente não precisa montar o perfil extra sozinho.

### Erros

Não há endpoint de “sincronizar wiki”. Tentativa de criação fiel com cobertura incompleta está em [personagens-atributos.md](personagens-atributos.md).
