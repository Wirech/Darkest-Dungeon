# Contract — CLI do coletor (Feature 006)

Ferramenta: `tools/DarkestDungeon.MediaCollector` (mesmo projeto da Feature 004).

Não sobe servidor HTTP. Não grava SQL. Saída: arquivos + `inventario.json`.

---

## Argumentos

Herdados da 004:

| Flag | Obrigatório | Significado |
|---|---|---|
| `--origem` | sim | Raiz da instalação local licenciada |
| `--saida` | sim | Destino do acervo (006: `.\assets\equipamentos-itens`) |
| `--simular` | não | Lista descoberta sem copiar |
| `--continuar` | não | Segue após lacuna |
| `--classe` | não | **Ignorado** para categorias de item; ainda vale no modo herói 004 |
| `--manifesto` / `--gerar-manifesto` | não | Somente modo herói 004; 006 não gera manifesto de habilidades |

Novos:

| Flag | Obrigatório | Significado |
|---|---|---|
| `--categoria` | não, repetível | `arma` `armadura` `acessorio` `acampamento` `consumivel`. Omissão = as cinco |
| `--mapeamento` | não | JSON da tabela pasta→categoria (default embutido, ver research.md) |
| `--inventario-herois` | não | Caminho de `assets/herois/inventario.json` para reuso de hash (default relativo `.\assets\herois\inventario.json` se existir) |

Modo herói 004 permanece o default **quando nenhuma `--categoria` de item é passada e o fluxo antigo é detectado** — implementação: se `--categoria` presente, executa só o pipeline 006; se ausente, comportamento 004 inalterado (regressão 004).

---

## Códigos de saída

| Código | Quando |
|---|---|
| 0 | Importação (ou simulação) concluída; lacunas não zeram o processo se `--continuar` ou se lacunas são esperadas (006 sempre continua como FR-005) |
| 1 | `--origem` inexistente, opção desconhecida, categoria inválida |

Mensagens de erro em PT-BR (ex.: `Informe um diretório de instalação existente com --origem.`).

---

## Inventário de saída

`{saida}/inventario.json` inclui `declaracaoDeUso` (FR-015), `arquivos[]` com `categoria`, `sha256`, `reutilizado`, `caminhoOrigem`, `caminhoDestino`, `lacunas[]`.

Reuso: se `sha256` já está no inventário de heróis, `reutilizado: true` e não há segundo blob.

---

## Exemplos

```powershell
dotnet run --project tools/DarkestDungeon.MediaCollector -- --origem "C:\Program Files (x86)\Steam\steamapps\common\DarkestDungeon" --saida .\assets\equipamentos-itens --categoria arma --simular

dotnet run --project tools/DarkestDungeon.MediaCollector -- --origem "C:\Program Files (x86)\Steam\steamapps\common\DarkestDungeon" --saida .\assets\equipamentos-itens --continuar
```

---

## Testes de CLI / fixtures

1. Fixture com um PNG de arma, um de armadura, um trinket: três entradas, hashes iguais à origem.
2. Dois arquivos byte-idênticos: uma cópia, duas associações.
3. Pasta `inventory/trinkets` ausente: lacuna, exit 0, demais categorias importadas.
4. PNG já no inventário 004: `reutilizado: true`.
5. Arquivo em pasta não mapeada: `categoria: NaoAssociado`.
6. Arquivo em `inventory/provision`: `ItemDeAcampamento`, nunca `Consumivel`.
7. Regressão: invocação 004 (`--classe Antiquarian` sem `--categoria`) continua importando heróis.
