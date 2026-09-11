# Contrato: coleta de ícones de acampamento

## CLI

```
dotnet run --project tools/DarkestDungeon.MediaCollector -- --origem <instalacao> --saida assets/herois --camping [--simular] [--continuar]
```

- `--camping` não combina com `--categoria`.
- `--gerar-manifesto` permanece combate-only.
- `--camping` importa PNGs **e** atualiza o manifesto (append camping) no mesmo `saida`.

Códigos: `0` sucesso (mesmo com lacunas parciais); `1` origem inválida / opções.

Saída stdout (PT-BR): `N arquivos de acampamento inventariados; M lacunas.`

## Inventário

`assets/herois/inventario.json` após corrida completa numa instalação com DLCs do seed:

- Mantém os arquivos 004 (heróis/Spine/ability).
- Acrescenta entradas `CaminhoDestino` em `arquivos/acampamento/camp_skill_*.png`.
- Declaração de uso local inalterada.

## Manifesto

`assets/herois/manifesto-habilidades.json`:

- Associações 004 (`*.ability.*.png`) intactas.
- Novas associações com `Arquivo` = `camp_skill_*.png` cobrindo as 79 skills do seed.

## HTTP (regressão 010)

`GET /personagens` — cada `habilidades[]` com `categoria: "Acampamento"`:

- `midia.status`: `OK` quando o PNG está no acervo.
- `midia.url`: `/acervo/herois/arquivos/acampamento/camp_skill_*.png`
- Combate inalterado.

`GET /acervo/herois/arquivos/acampamento/camp_skill_encourage.png` → 200 `image/png`.
