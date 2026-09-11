# Pesquisa: Ícones de Habilidades de Acampamento

**Feature**: 011-icones-acampamento  
**Date**: 2026-09-11  
**Prerequisites**: [spec.md](./spec.md), mapa [artifacts/lacunas-midia.md](../../artifacts/lacunas-midia.md), Features 004 e 010

Não resta `NEEDS CLARIFICATION`.

## Decisões

### Mesmo CLI, flag `--camping`

- **Decisão**: Estender `DarkestDungeon.MediaCollector` com `--camping` (mutuamente exclusivo de `--categoria` de equipamentos). Destino: `--saida` padrão `assets/herois`.
- **Justificativa**: 004 já copia byte a byte, hash e inventário. Constituição V.
- **Alternativas**: segundo executável (duplicação); endpoint HTTP (API deve permanecer stateless).

### Pasta de destino compartilhada

- **Decisão**: Copiar para `arquivos/acampamento/camp_skill_{id}.png` (um arquivo por stem, vanilla tem prioridade se o mesmo stem existir na DLC).
- **Justificativa**: Ícones são globais (Encourage serve 19 classes). O resolvedor 010 exige `CaminhoDaClasse` hoje — isso **quebra** camping e será relaxado só para manifesto de `camp_skill_`.
- **Alternativas**: copiar o mesmo PNG para cada pasta de classe (desperdício; Gallows Humor / Field Dressing).

### Merge do inventário 004

- **Decisão**: Com `--camping`, carregar `inventario.json` existente, acrescentar/atualizar só entradas cujo destino é `arquivos/acampamento/` ou origem `camp_skill_`, preservar o restante. `--continuar` reutiliza hash.
- **Justificativa**: FR-003 / SC-003. `ImportadorLocal` hoje **reescreve** o JSON só com a classe processada — não pode ser reusado sem merge.
- **Alternativas**: inventário separado `inventario-camping.json` (resolvedor teria duas raízes; rejeitado).

### Manifesto: append, não gerar combate de novo

- **Decisão**: `--gerar-manifesto-camping` (ou integrado ao `--camping`) lê o manifesto atual e **adiciona** associações cuja `Arquivo` começa com `camp_skill_`, sem remover `*.ability.*.png`.
- **Justificativa**: FR-009. `GeradorDoManifesto` atual só mapeia combate por ordem `ability.one..seven`.

### Aliases wiki → id do jogo

Tabela canônica (JSON do jogo + PNG):

| NomeOriginal (seed) | id / stem PNG |
|---|---|
| Wound Care | first_aid |
| This Is How We Do It | how_its_done |
| Unshakable Leader | unshakeable_leader |
| Night Moves | night_steps |
| Unparalleled Finesse | uncatchable |
| The Cure | preventative_medicine |
| Self-Medicate | self_medicate |
| Every Rose Has Its Thorn | every_rose |
| Man's Best Friend | pet_the_hound |
| Resupply | supply |
| Snuff Box | forage |
| Snake Eyes | way_of_serpent |
| Snake Skin | way_of_scales |
| Sandstorm | way_of_sway |
| Adder's Embrace | way_of_adder |
| Lash's Anger | lash_anger |
| Lash's Solace | lash_solace |
| Lash's Kiss | lash_kiss |
| Lash's Cure | lash_cure |
| Pick Pocket | pickpocket |
| Again! | again |

Demais: slug do NomeOriginal (apóstrofo/`!`/`-` removidos). Arquivo: `camp_skill_{id}.png`.

Leftovers **não** catalogados: `bandage`, `bear_traps`, `hobby`, `perimeter_alarms`, `wrap`.

### Resolvedor: camping ignora pasta da classe

- **Decisão**: `TentarArquivoDeHabilidade` localiza por nome de arquivo no inventário; se o arquivo é `camp_skill_*.png`, **não** exige `CaminhoDaClasse`. Combate (`ability`) continua exigindo pasta/prefixo da classe.
- **Justificativa**: FR-008. Prefixo extraído de `camp_skill_encourage.png` seria `camp_skill_encourage`, inútil como chave de classe.

### Fora de escopo confirmado

`panels/icons_equip/**` e `inventory/**` PNG=0. Feature futura.
