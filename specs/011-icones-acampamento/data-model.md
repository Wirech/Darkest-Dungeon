# Modelo de Dados: Ícones de Acampamento

**Feature**: 011-icones-acampamento  
**Date**: 2026-09-11

Sem tabelas SQL novas. Estende inventário e manifesto em disco.

## ArquivoImportado (inventário heróis)

Campos existentes (004). Novos registros:

| Campo | Valor camping |
|---|---|
| Classe | `"Acampamento"` (compartilhado) ou nome PT-BR da classe quando o PNG é exclusivo DLC — **canônico: `"Acampamento"`** para todos os camp_skill, pois o manifesto liga por NomeOriginal |
| CaminhoOrigem | absoluto na instalação Steam |
| CaminhoDestino | `arquivos/acampamento/camp_skill_{id}.png` |
| Sha256 / TamanhoBytes / Reutilizado | iguais à 004 |
| Categoria | `HabilidadeDeAcampamento` (opcional) |

Invariante: mescla com arquivos 004; chave de identidade = `CaminhoDestino` normalizado.

## AssociacaoDeHabilidade (manifesto)

| Campo | Combate (004) | Acampamento (011) |
|---|---|---|
| Classe | NomeExibicao PT-BR | NomeExibicao da classe (ou `"Compartilhadas"` para Encourage/Wound Care/Pep Talk) |
| Arquivo | `{prefixo}.ability.{n}.png` | `camp_skill_{id}.png` |
| Habilidades | lista NomeOriginal combate | lista NomeOriginal acampamento |

Uma associação por (Classe, Arquivo). Skills compartilhadas entre duas classes (Gallows Humor, Field Dressing) podem repetir o mesmo `Arquivo` em duas linhas de Classe, ou uma linha com várias habilidades — o resolvedor indexa por NomeOriginal e por `prefixo\|NomeOriginal`. Para camping, a chave `NomeOriginal` sozinha basta (arquivo único).

## AliasDeCamping

Tabela estática no coletor: NomeOriginal → stem. Não persistida no banco.

## SlotDeMidiaDoCard

Inalterado (010). Status passa de Pendente → OK quando manifesto+arquivo existem.
