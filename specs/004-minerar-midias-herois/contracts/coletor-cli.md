# Contrato: Importador Local de Mídias

## Comando

```powershell
dotnet run --project tools/DarkestDungeon.MediaCollector -- --origem <instalacao-do-jogo> --saida <diretorio> [opcoes]
```

## Entradas

| Opção | Obrigatória | Descrição |
|---|---:|---|
| `--origem <diretorio>` | Sim | Raiz da instalação local do jogo, somente leitura. |
| `--saida <diretorio>` | Sim | Raiz do acervo e do inventário. |
| `--classe <nome-original>` | Não | Restringe a importação; repetível. |
| `--manifesto <arquivo>` | Não | Mapeamento JSON de regiões/conjuntos para habilidades. |
| `--simular` | Não | Descobre e valida sem copiar ou gravar inventário. |
| `--continuar` | Não | Reutiliza arquivos já validados pelo inventário. |

## Saída

- Código `0`: importação ou simulação concluída, inclusive com lacunas documentadas.
- Código `1`: argumentos, instalação ou manifesto inválido.
- Código `4`: falha de leitura da origem ou escrita da saída; inventário anterior é preservado.
- Arquivos em `<saida>/arquivos/<classe>/<conjunto>/` com nome e extensão originais.
- `<saida>/inventario.json` com hashes, caminhos, conjuntos Spine, associações e lacunas.

## Garantias

- Nenhuma requisição de rede é feita.
- A instalação de origem não é modificada.
- Os bytes de cada arquivo copiado são preservados.
- GIF e recorte de sprites não fazem parte da importação.
