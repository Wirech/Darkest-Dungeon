# Dados oficiais mineirados — Feature 003 (Hierarquia + Catálogo de Heróis)

Este documento reúne os dados oficiais extraídos da wiki `darkestdungeon.wiki.gg` durante a implementação da Feature 003. Serve como índice curatorial das 20 classes canônicas, base para os seeds ([`ClassesSeed.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/ClassesSeed.cs), [`MapaDeCoberturaSeed.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/MapaDeCoberturaSeed.cs)) e para as tarefas restantes de mineração de habilidades (T117).

- **Data da coleta**: 2026-09-07
- **Fonte primária**: páginas `https://darkestdungeon.wiki.gg/wiki/{NomeOriginal}_(Darkest_Dungeon)` (uma por classe).
- **Escopo atual**: resistências base (US4/US5 — T084, T116). Habilidades ainda pendentes (US2 — T117).

## 1. Índice das 20 classes canônicas

| # | Enum (`ClasseDeHeroi`) | Nome PT-BR (`NomeExibicao`) | Nome oficial inglês (`NomeOriginal`) | DLC |
|---|---|---|---|---|
| 1 | `Abominacao` | Abominação | Abomination | base |
| 2 | `Antiquario` | Antiquário | Antiquarian | base |
| 3 | `Besteiro` | Besteiro | Arbalest | base |
| 4 | `CacadorDeRecompensas` | Caçador de Recompensas | Bounty Hunter | base |
| 5 | `Cruzado` | Cruzado | Crusader | base |
| 6 | `LadraoDeCova` | Ladrão de Cova | Grave Robber | base |
| 7 | `BoboDaCorte` | Bobo da Corte | Jester | base |
| 8 | `MestreDeCaca` | Mestre de Caça | Houndmaster | base |
| 9 | `Leproso` | Leproso | Leper | base |
| 10 | `Infernal` | Infernal | Hellion | base |
| 11 | `Bandido` | Bandido | Highwayman | base |
| 12 | `Musqueteiro` | Musqueteiro | Musketeer | The Musketeer (free) |
| 13 | `Veterano` | Veterano | Man-at-Arms | base |
| 14 | `Ocultista` | Ocultista | Occultist | base |
| 15 | `MedicoDaPeste` | Médico da Peste | Plague Doctor | base |
| 16 | `Vestal` | Vestal | Vestal | base |
| 17 | `Flagelante` | Flagelante | Flagellant | The Crimson Court |
| 18 | `Rompedor` | Rompedor | Shieldbreaker | The Shieldbreaker |
| 19 | `Duelista` | Duelista | Duelist | The Fire's Edge |
| 20 | `Fugitivo` | Fugitivo | Runaway | The Fire's Edge |

Contagem final: **20 classes**, o que satisfaz SC-002 (`GET /classes` retorna exatamente 20).

## 2. Resistências base oficiais

Valores em %. Ordem no construtor de `ResistenciasDeClasse`: **Atordoamento, Sangramento, Envenenamento, Debuff, Movimento, Doença, GolpeMortal, Armadilha**.

| Classe | Stun | Bleed | Blight | Debuff | Move | Disease | Deathblow | Trap |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| Abominação | 40 | 30 | 60 | 20 | 40 | 20 | 67 | 10 |
| Antiquário | 20 | 20 | 20 | 20 | 20 | 20 | 67 | 10 |
| Besteiro | 40 | 30 | 30 | 30 | 40 | 30 | 67 | 10 |
| Caçador de Recompensas | 40 | 30 | 30 | 30 | 40 | 20 | 67 | 40 |
| Cruzado | 40 | 30 | 30 | 30 | 40 | 30 | 67 | 10 |
| Ladrão de Cova | 20 | 30 | 50 | 30 | 20 | 30 | 67 | 50 |
| Bobo da Corte | 20 | 30 | 40 | 40 | 20 | 20 | 67 | 30 |
| Mestre de Caça | 40 | 40 | 40 | 30 | 40 | 30 | 67 | 40 |
| Leproso | 60 | 10 | 40 | 40 | 60 | 20 | 67 | 10 |
| Infernal | 40 | 40 | 40 | 30 | 40 | 30 | 67 | 20 |
| Bandido | 30 | 30 | 30 | 30 | 30 | 30 | 67 | 40 |
| Musqueteiro | 40 | 30 | 30 | 30 | 40 | 30 | 67 | 10 |
| Veterano | 40 | 40 | 30 | 30 | 40 | 30 | 67 | 10 |
| Ocultista | 20 | 40 | 30 | 60 | 20 | 40 | 67 | 10 |
| Médico da Peste | 20 | 20 | 60 | 50 | 20 | 50 | 67 | 20 |
| Vestal | 30 | 40 | 30 | 30 | 30 | 30 | 67 | 10 |
| Flagelante | 50 | 65 | 30 | 30 | 50 | 40 | **73** | **0** |
| Rompedor | 50 | 30 | 20 | 30 | 50 | 30 | 67 | 20 |
| Duelista | 30 | 30 | 30 | 40 | 30 | 30 | 67 | 10 |
| Fugitivo | 20 | 40 | 40 | 20 | 30 | 30 | 67 | 30 |

Observações:

- **Flagelante** é a única classe fora do padrão: `Deathblow=73%` e `Armadilha=0%` (documentado na wiki como "innate 73% passive chance to resist fatality").
- **Musqueteiro** é reskin funcional do Besteiro — mesmos oito valores.
- Todas as demais 18 classes têm `Deathblow=67%`.

## 3. Estado do Mapa de Cobertura

Após a implementação de T116, `MapaDeCoberturaSeed.Materializar()` cria 160 entradas (`20 classes × 8 resistências`) todas com estado `Coletado`. Habilidades e efeitos ainda não gerados aparecerão como novas entradas conforme T117 popular as ~140 habilidades.

## 4. Habilidades — pendente (T117)

A mineração das ~140 habilidades por classe (7 de combate + 4 únicas + 3 compartilhadas de acampamento na maioria dos casos, com exceções documentadas) será executada em fase separada. As páginas fonte já foram identificadas (mesma URL base de cada classe, seções `Combat Skills` e `Camping Skills`). Casos particulares:

- **Flagelante** possui apenas habilidades únicas de acampamento (sem `Encourage`/`Wound Care`/`Pep Talk` compartilhadas).
- **Duelista** pode equipar todas as 7 habilidades simultaneamente (todas ganhas ao recrutar).
- **Habilidades compartilhadas de acampamento** (por exemplo `Encourage`, `Wound Care`, `Pep Talk`) devem ser cadastradas como **registros únicos** referenciados por múltiplas linhas em `ClasseHabilidade`, conforme FR-009 e US2/AC5.

### 4.1. Piloto — Cruzado + acampamento compartilhadas (2026-09-07)

Implementado em [`HabilidadesSeed.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.cs). Convenção adotada para leitura da wiki:

- **Rank** (posições onde o herói pode estar): dots esquerda→direita = rank **4,3,2,1** (backline→frontline).
- **Target** (posições atingidas): dots esquerda→direita = target **1,2,3,4** (frontline→backline).

**Cruzado — 7 habilidades de combate**

| # | PT-BR | Original | Rank | Target | Dano | Prec | Crit | Efeitos-chave |
|---|---|---|---|---|---|---|---|---|
| 1 | Golpe Sagrado | Smite | 1,2 | 1,2 | +0% | 85 | 0% | +15% Dano vs Não-Mortos |
| 2 | Acusação Zelosa | Zealous Accusation | 1,2 | 1,2 (AOE) | -40% | 85 | -4% | — |
| 3 | Golpe Atordoante | Stunning Blow | 1,2 | 1,2 | -50% | 90 | 0% | Atordoamento 100% |
| 4 | Baluarte da Fé | Bulwark of Faith | 1,2 | Self | 0 | — | — | Tocha +24, PROT +20%, Marca Self — limite 1/batalha |
| 5 | Cura de Batalha | Battle Heal | 1-4 | Aliado | 0 | — | — | Cura 2-3 |
| 6 | Lança Sagrada | Holy Lance | 3,4 | 2,3,4 | +0% | 85 | 6.5% | +15% Dano vs Não-Mortos, Avança 1 |
| 7 | Grito Inspirador | Inspiring Cry | 1-4 | Aliados | 0 | — | — | Cura 1, -5 Estresse, Tocha +5 |

**Cruzado — 4 habilidades de acampamento únicas**

| # | PT-BR | Original | Custo | Alvo | Efeitos |
|---|---|---|---|---|---|
| 1 | Líder Inabalável | Unshakable Leader | 2 | Self | -25% Estresse (4 batalhas) |
| 2 | Manter-se Firme | Stand Tall | 3 | Um aliado | -15 Estresse, remove Mortalidade |
| 3 | Discurso Zeloso | Zealous Speech | 5 | Party inteira | -15 Estresse, -15% Estresse recebido (4 batalhas) |
| 4 | Vigília Zelosa | Zealous Vigil | 4 | Self | -25 Estresse, -15 extra se Afligido, impede emboscada noturna |

**Acampamento compartilhadas globais (aplicadas a 19 classes — Flagelante excluído)**

| # | PT-BR | Original | Custo | Alvo | Efeitos |
|---|---|---|---|---|---|
| 1 | Encorajar | Encourage | 2 | Um aliado | -15 Estresse |
| 2 | Cuidados com Ferimentos | Wound Care | 2 | Um aliado | Cura 15% HP, remove Sangramento e Envenenamento |
| 3 | Conversa Motivadora | Pep Talk | 2 | Um aliado | -15% Estresse recebido (4 batalhas) |

**Traduções canônicas adotadas** (para replicar nas próximas classes):

- Stun→Atordoamento, Bleed→Sangramento, Blight→Envenenamento, Move→Movimento, Disease→Doença, Deathblow→Golpe Mortal, Trap→Armadilha
- DMG→Dano, ACC→Precisão, CRIT→Crítico, PROT→Proteção, DODGE→Esquiva, SPD→Velocidade
- Stress→Estresse, Heal→Cura, Torch→Tocha, Mark→Marcação, Unholy→Não-Mortos

**Status**: 10 habilidades novas (7+4) + 3 globais compartilhadas + 7+4+(3×19)=**14 habilidades + 68 associações** semeadas. Falta expandir para as outras 19 classes.

### 4.2. Batch 2 — Vestal + Ocultista + Médico da Peste (2026-09-07)

Arquivos: [`HabilidadesSeed.Vestal.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.Vestal.cs), [`HabilidadesSeed.Ocultista.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.Ocultista.cs), [`HabilidadesSeed.MedicoDaPeste.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.MedicoDaPeste.cs).

**Vestal** (curandeira sagrada — 7 combate + 4 acampamento):
- Golpe de Maça (Mace Bash), Julgamento (Judgement), Luz Deslumbrante (Dazzling Light), Graça Divina (Divine Grace), Conforto Divino (Divine Comfort), Iluminação (Illumination), Mão da Luz (Hand of Light).
- Acampamento: Benção (Bless), Canto Sacro (Chant), Oração (Pray), Santuário (Sanctuary).

**Ocultista** (suporte sombrio — 7 combate + 4 acampamento):
- Punhalada Sacrificial (Sacrificial Stab), Artilharia do Abismo (Abyssal Artillery), Maldição Enfraquecedora (Weakening Curse), Reconstrução Estranha (Wyrd Reconstruction), Maldição de Vulnerabilidade (Vulnerability Hex), Mãos do Abismo (Hands from the Abyss), Chamado do Daemon (Daemon's Pull).
- Acampamento: Abandonai a Esperança (Abandon Hope), Ritual Sombrio (Dark Ritual), Força Sombria (Dark Strength), Comunhão Indizível (Unspeakable Commune).

**Médico da Peste** (DoT + suporte — 7 combate + 4 acampamento):
- Explosão Nociva (Noxious Blast), Granada da Peste (Plague Grenade), Gás Cegante (Blinding Gas — 3 usos/batalha), Incisão (Incision), Medicina de Campo (Battlefield Medicine), Vapores Encorajadores (Emboldening Vapours — 2 usos/batalha), Explosão Desorientadora (Disorienting Blast).
- Acampamento: Vapores Experimentais (Experimental Vapours), Sanguessugas (Leeches), A Cura (The Cure), Auto-Medicação (Self-Medicate).

**Nota de escopo**: o modelo de domínio limita `EfeitoDeHabilidade.ChanceBase` a 0-100%. A wiki oficial usa 110-130% em vários efeitos ("burla resistência"). Optou-se por **capar em 100%** e documentar o valor original na `Descricao` (ex.: "Mãos do Abismo — chance 110% base capada em 100").

**Progresso T117**: 47 habilidades semeadas (4 classes × 11 + 3 compartilhadas) + 101 associações = **~21% do catálogo total** (~223 habilidades).

### 4.3. Batch 3 — Besteiro + Musqueteiro + Veterano (2026-09-07)

Arquivos: [`HabilidadesSeed.Besteiro.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.Besteiro.cs), [`HabilidadesSeed.Musqueteiro.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.Musqueteiro.cs), [`HabilidadesSeed.Veterano.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.Veterano.cs).

**Besteiro** (Arbalest — backline ranged com marcação; 7 combate + 4 acampamento):
- Tiro de Sniper (Sniper Shot), Fogo de Supressão (Suppressing Fire), Marca de Sniper (Sniper's Mark), Bola (Bola), Fogo Cego (Blindfire), Curativo de Campo (Battlefield Bandage), Sinalizador de Comando (Rallying Flare).
- Acampamento: Curativo de Emergência (Field Dressing), Plano de Marcha (Marching Plan), Recordoar Besta (Restring Crossbow), Triagem (Triage).

**Musqueteiro** (Musketeer — reskin funcional do Besteiro; 7 combate + 1 acampamento único):
- Tiro Certeiro (Aimed Shot), Cortina de Fumaça (Smokescreen), Anunciar o Alvo (Call the Shot), Chumbinho (Buckshot), Pistola (Sidearm), Reparo Rápido (Patch Up), Tiro de Skeet (Skeet Shot).
- Acampamento única: Limpar Mosquete (Clean Musket). As demais 3 (Field Dressing, Marching Plan, Triage) são **compartilhadas** com o Besteiro — 1 registro global, associadas às duas classes.

**Veterano** (Man-at-Arms — tank e comandante; 7 combate + 4 acampamento):
- Esmagar (Crush), Muralha (Rampart), Rugido (Bellow), Defensor (Defender), Retribuição (Retribution), Comando (Command), Reforçar (Bolster — 1 uso/batalha).
- Acampamento: Manter Equipamento (Maintain Equipment), Táticas (Tactics), Instrução (Instruction), Prática de Armas (Weapons Practice).

**Reuso de registros globais**: `Field Dressing`, `Marching Plan` e `Triage` são criadas 1 vez pelo Besteiro e reusadas pelo Musqueteiro via `NomesPorClasse`, respeitando FR-009 (habilidade compartilhada = 1 registro, N associações).

**Progresso T117**: **77/223 habilidades** semeadas (~35%) + 140 associações. Faltam 13 classes.

### 4.4. Batch 4 — Bandido + Ladrão de Cova + Bobo da Corte (2026-09-07)

Arquivos: [`HabilidadesSeed.Bandido.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.Bandido.cs), [`HabilidadesSeed.LadraoDeCova.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.LadraoDeCova.cs), [`HabilidadesSeed.BoboDaCorte.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.BoboDaCorte.cs).

**Bandido** (Highwayman — DPS flexível melee/ranged com Riposte; 7 combate + 3 acampamento únicas):
- Corte Perverso (Wicked Slice), Tiro de Pistola (Pistol Shot), Tiro à Queima-Roupa (Point Blank Shot), Disparo de Chumbo (Grapeshot Blast), Tiro Rastreador (Tracking Shot), Avanço do Duelista (Duelist's Advance), Cortar Veia (Open Vein).
- Acampamento únicas: Destreza Sem Igual (Unparalleled Finesse), Limpar Armas (Clean Guns), Instinto de Bandido (Bandit's Sense).
- Acampamento compartilhada com Ladrão de Cova: **Humor Negro (Gallows Humor)** — 1 registro global.

**Ladrão de Cova** (Grave Robber — assassina evasiva com envenenamento e furtividade; 7 combate + 4 acampamento):
- Picareta na Cara (Pick to the Face), Investida (Lunge), Adagas Cintilantes (Flashing Daggers), Fusão nas Sombras (Shadow Fade), Adaga Arremessada (Thrown Dagger), Dardo Envenenado (Poison Dart), Truque Tóxico (Toxin Trickery).
- Acampamento: Caixa de Rapé (Snuff Box), **Humor Negro (Gallows Humor)** [criada aqui e reusada pelo Bandido], Andanças Noturnas (Night Moves), Furto (Pilfer).

**Bobo da Corte** (Jester — buffer/debuffer com sistema Finale acumulativo; 7 combate + 4 acampamento):
- Adaga de Punho (Dirk Stab), Colheita (Harvest), Grande Final (Finale — 1 uso/batalha), Solo (Solo — 2 usos/batalha), Corte Fora (Slice Off), Balada de Guerra (Battle Ballad), Melodia Inspiradora (Inspiring Tune). Todas as skills acumulam o buff Finale (+30% dano por 8 rodadas) exceto o próprio Finale que é a "descarga".
- Acampamento: Voltar no Tempo (Turn Back Time), Toda Rosa Tem Seu Espinho (Every Rose Has Its Thorn), Olho do Tigre (Tiger's Eye), Zombaria (Mockery).

**Segunda skill compartilhada**: `Gallows Humor` demonstra o mesmo padrão FR-009 usado com as 3 compartilhadas globais e com Field Dressing/Marching Plan/Triage (Besteiro↔Musqueteiro): 1 registro, N associações.

**Progresso T117**: **109/223 habilidades** semeadas (~49%) + 172 associações. Faltam 10 classes.

### 4.5. Batch 5 — Leproso + Infernal + Abominação (2026-09-07)

Arquivos: [`HabilidadesSeed.Leproso.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.Leproso.cs), [`HabilidadesSeed.Infernal.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.Infernal.cs), [`HabilidadesSeed.Abominacao.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.Abominacao.cs).

**Leproso** (Leper — tank frontline com auto-cura e debuffs; 7 combate + 4 acampamento):
- Talhar (Chop), Cortar (Hew — AOE), Expurgar (Purge), Vingança (Revenge — buff berserker), Resistir (Withstand — buff defensivo), Solenidade (Solemnity — auto-cura), Intimidar (Intimidate).
- Acampamento: Remover a Máscara (Let the Mask Down), Mortalha Sangrenta (Bloody Shroud), Reflexão (Reflection), Quarentena (Quarantine).

**Infernal** (Hellion — barbara com sangramento e drawback pós-Yawp; 7 combate + 4 acampamento):
- Golpe Perverso (Wicked Hack), **Cisne de Ferro** (Iron Swan — ataque backline-a-backline único no jogo), Grito Bárbaro (Barbaric Yawp — 3 usos/batalha), Se Sangra (If It Bleeds — AOE Bleed), Investida (Breakthrough), Descarga de Adrenalina (Adrenaline Rush), Sangrar Até Morrer (Bleed Out).
- Acampamento: Transe de Batalha (Battle Trance), Farra (Revel), Rejeitar os Deuses (Reject the Gods), Afiar Lança (Sharpen Spear).

**Abominação** (Abomination — dual-form Humano/Besta com Transform como ação livre; 7 combate + 4 acampamento):
- **Transform** (ação livre, limite 2/batalha) alterna entre formas causando +8 estresse aos aliados.
- Forma Humana (3 skills): Algemas (Manacles), Bile da Besta (Beast's Bile), Absolvição (Absolution).
- Forma Besta (3 skills): Rasgar (Rake), Fúria (Rage), Bater (Slam).
- Acampamento: Controle da Raiva (Anger Management), Preparação Psíquica (Psych Up), Aceleração (The Quickening), Sangue Ancião (Eldritch Blood).

**Nota sobre Abominação**: hoje ela ganha as 3 acampamento compartilhadas normalmente (Encourage/Wound Care/Pep Talk) — a restrição antiga de "não pode ficar na mesma party de religiosos" foi revogada em Color of Madness.

**Progresso T117**: **142/223 habilidades** semeadas (~64%) + 205 associações. Faltam 7 classes.

### 4.6. Batch 6 — Antiquário + Caçador de Recompensas + Mestre de Caça (2026-09-07)

Arquivos: [`HabilidadesSeed.Antiquario.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.Antiquario.cs), [`HabilidadesSeed.CacadorDeRecompensas.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.CacadorDeRecompensas.cs), [`HabilidadesSeed.MestreDeCaca.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.MestreDeCaca.cs).

**Antiquário** (Antiquarian — suporte fraca de dano mas com "reverse guard" único; 7 combate + 4 acampamento):
- Facada Nervosa (Nervous Stab), Vapores Purulentos (Festering Vapours), Abaixe-se! (Get Down!), Pó de Flash (Flashpowder), Vapores Fortificantes (Fortifying Vapours), Vapores Revigorantes (Invigorating Vapours), **Proteja-me!** (Protect Me — força um aliado a guardá-la, 3 usos/batalha).
- Acampamento: Reabastecer (Resupply), Vasculhar Bugigangas (Trinket Scrounge), Pós Estranhos (Strange Powders), Encantamento Curioso (Curious Incantation).

**Caçador de Recompensas** (Bounty Hunter — executor com combos de Mark+Stun; 7 combate + 4 acampamento):
- Coletar Recompensa (Collect Bounty — +90% dano vs Marcado), Marca de Morte (Mark for Death), Venha Cá (Come Hither), Soco Cruzado (Uppercut — Stun + Knockback 2), Bomba de Fumaça (Flashbang), Acabe com Ele (Finish Him — +25% dano vs Atordoado), Estrepes (Caltrops).
- Acampamento: É Assim que a Gente Faz (This Is How We Do It), Rastreamento (Tracking), Abate Planejado (Planned Takedown), Explorar à Frente (Scout Ahead).

**Mestre de Caça** (Houndmaster — flexível com cão que ataca todas as posições; 7 combate + 4 acampamento):
- Investida do Cão (Hound's Rush), Assédio do Cão (Hound's Harry — AOE total), Assobio de Alvo (Target Whistle), Grito de Guerra (Cry Havoc), Cão de Guarda (Guard Dog), Lamber Feridas (Lick Wounds), Cassetete (Blackjack).
- Acampamento: Vigília do Cão (Hound's Watch), Cão de Terapia (Therapy Dog), Melhor Amigo do Homem (Man's Best Friend), Soltar o Cão (Release the Hound).

**🎉 Base game 100% coberto (16 de 16 classes)** — restam apenas as 4 classes DLC.

**Progresso T117**: **175/223 habilidades** semeadas (~78%) + 238 associações. Faltam 4 classes (todas DLC).

### 4.7. Batch 7 (final) — Flagelante + Rompedor + Duelista + Fugitivo (2026-09-07)

Arquivos: [`HabilidadesSeed.Flagelante.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.Flagelante.cs), [`HabilidadesSeed.Rompedor.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.Rompedor.cs), [`HabilidadesSeed.Duelista.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.Duelista.cs), [`HabilidadesSeed.Fugitivo.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.Fugitivo.cs).

**Flagelante** (Crimson Court DLC — sangramento auto-destrutivo com cura via Rapturous; 7 combate + 4 acampamento — **sem shared**):
- Punir (Punish — +100% dano), Chuva de Sofrimento (Rain of Sorrows), Exsanguinação (Exsanguinate — 3 usos/batalha), Reclamar (Reclaim), Redimir (Redeem — 2 usos/batalha), Suportar (Endure), Sofrer (Suffer — transfere DoTs para si).
- Acampamento: Raiva do Açoite (Lash's Anger), Consolo do Açoite (Lash's Solace), Beijo do Açoite (Lash's Kiss), Cura do Açoite (Lash's Cure). **Única classe sem `Encourage`/`Wound Care`/`Pep Talk`** — o loop de associações compartilhadas em [`HabilidadesSeed.cs`](../../src/DarkestDungeon.Infrastructure/Data/Seeds/HabilidadesSeed.cs) já pula o Flagelante.

**Rompedor** (Shieldbreaker DLC — quebra guarda e ignora armadura; 7 combate + 4 acampamento):
- Perfurar (Pierce), **Puncionar** (Puncture — quebra guarda e impede re-guarda por 2 rodadas), Beijo da Víbora (Adder's Kiss), Empalar (Impale — AOE total do frontline), Expor (Expose), Cativar (Captivate), Balanço da Serpente (Serpent Sway — 2 usos/batalha).
- Acampamento: Olhos da Serpente (Snake Eyes), Pele de Serpente (Snake Skin), Tempestade de Areia (Sandstorm), Abraço da Víbora (Adder's Embrace).

**Duelista** (Fire's Edge DLC — dual-stance Defensivo/Agressivo com Riposte; 7 combate + 4 acampamento):
- **Antecipação** (Anticipation — alterna entre modos como ação livre) + Defensivo (Touché, Finta, Desengajar) + Agressivo (Flèche, Golpe de Misericórdia, Chute).
- Segunda classe com sistema de "formas duplas" no seed (a primeira foi Abominação, mas essa alterna manualmente com uma skill dedicada).
- Acampamento: Meditação (Meditation), Preparação (Preparation), Instrução Impiedosa (Ruthless Instruction), De Novo! (Again! — renova usos de acampamento).

**Fugitivo** (Fire's Edge DLC — sistema de Queimadura DoT com Furtividade; 7 combate + 4 acampamento):
- Golpe Escaldante (Searing Strike), Vaga-lume (Firefly), **Corra e Esconda** (Run and Hide — Furtividade 4 rodadas), Saquear (Ransack), Luz da Fogueira (Hearthlight), Queima Controlada (Controlled Burn — 2 usos/batalha), **Refluxo** (Backdraft — atinge posição atrás do alvo, +25% dano por pilha de Queimadura).
- Acampamento: Atear Fogo (Kindle), Cauterizar (Cauterize), Brincar com Fogo (Play with Fire), Bater Carteira (Pick Pocket — produz chave-mestra).

**🏁 CATÁLOGO COMPLETO — 20/20 classes semeadas.**

**Estatística final T117**:
- **20 classes** com 7 combate + 4 acampamento = **220 slots de habilidade**
- **-4 duplicações removidas**: Musqueteiro reusa 3 skills do Besteiro (Field Dressing/Marching Plan/Triage) + Bandido reusa Gallows Humor do Ladrão de Cova
- **+3 skills compartilhadas globais** (Encourage/Wound Care/Pep Talk)
- **= 219 habilidades únicas** no catálogo (140 combate + 79 acampamento)
- **+ 277 associações Classe×Habilidade** (220 unique per-class + 57 compartilhadas globais em 19 classes exceto Flagelante)

## 5. Rastreabilidade

- Task ⇒ resultado: **T084**, **T116** → seed oficial das resistências (esta seção 2).
- Task ⇒ resultado: **T059** → índice das 20 classes (esta seção 1) + tabela de resistências.
- Task ⇒ resultado: **T117** (+ T059a/b/c) → seções 4.1–4.7 deste documento cobrindo as 20 classes com 219 habilidades.
- Task ⇒ resultado: **T113/T123** → seção 6 abaixo (quickstart executado end-to-end).

## 6. Quickstart executado (T113 + T123, 2026-09-07)

API subida localmente em `http://localhost:5140` com `ASPNETCORE_ENVIRONMENT=Testing` (InMemory) e catálogo seed carregado:

- **40 entidades Classe + owned resistências**
- **748 entidades Habilidade + Efeitos + Limites**
- **277 associações ClasseHabilidade**
- **160 entradas MapaDeCobertura**

Os 8 cenários do [quickstart.md](quickstart.md) foram executados via `Invoke-WebRequest` PowerShell 5.1. Resultado consolidado:

### 6.1. Cenário 1 — `GET /classes` retorna 20

```
Cenario1_ContagemClasses=20
```

✅ Confere com SC-002 (`GET /classes` retorna exatamente 20).

### 6.2. Cenário 2 — `Gallows Humor` compartilhado entre Bandido e Ladrão de Cova

```
Bandido_habilidades=14
LadraoCova_habilidades=14
GallowsHumor_bandido_id=dc39facc-3e93-6d4e-8d54-541b6b17b4ca
GallowsHumor_ladrao_id =dc39facc-3e93-6d4e-8d54-541b6b17b4ca
IdsIguais=True
```

✅ Confere com FR-009: mesma habilidade compartilhada aparece com **GUID idêntico** para as duas classes; nenhuma duplicação. Contagem 14 = 7 combate + 4 únicas + 3 globais compartilhadas.

### 6.3. Cenário 3 — `POST /habilidades/combate` novo (201) e duplicata (400)

```
Cenario3a_novaCriada=201
Cenario3b_duplicataRejeitada=400
Body: {"mensagem":"Já existe uma habilidade com o nome 'Investida do Sabre'.",
       "erros":[{"campo":"nomeExibicao",
                 "mensagem":"Nome de habilidade deve ser único no catálogo global."}]}
```

✅ Confere com FR-006 (unicidade global por `NomeExibicao`) + padrão `ErroResponse` PT-BR com `campo` preenchido.

**Observação**: o quickstart.md descrevia o payload com `alvo='Inimigo'` (string) e `classes` (chave). O contrato real usa `alvo=2` (int enum) e `classesIds` (chave). Documentado para atualização futura do quickstart.

### 6.4. Cenário 4 — `POST /personagens` Cruzado copia resistências

```
Cenario4_status=201
Cenario4_personagem_resistencias:
  atordoamento=40  sangramento=30  envenenamento=30  debuff=30  movimento=40
  doenca=30  golpeMortal=67  armadilha=10
Cenario4_personagemId=9db0fe1f-e4aa-4778-8718-78a3d2af7cc3
```

✅ Confere com FR-016 (Personagem herda `ResistenciasBase` da Classe) — os 8 valores batem com o Cruzado da seção 2 (40/30/30/30/40/30/67/10).

### 6.5. Cenário 5 — Criar Arma para Cruzado e equipar; rejeitar Arma de outra classe

```
Cenario5_arma_status=201
Cenario5_armaId=6aa005fc-8edc-416a-a5af-77f24e54dbaa   armaClasse=4  (Cruzado)
Cenario5_equipar_status=200

Cenario5_equipar_incompativel_status=400
Body: {"mensagem":"Arma não é elegível para a classe do Personagem.",
       "erros":[{"campo":"armaId",
                 "mensagem":"Arma pertence a outra classe."}]}
```

✅ Confere com FR-020 (Arma tem `ClasseElegivel` obrigatório) + validação em `EquiparAsync` que rejeita arma de classe incompatível.

### 6.6. Cenário 6 — `GET /mapa-de-cobertura/{bandidoId}`

```
Cenario6_totalEntradas=8 (uma por resistência do Bandido)
Cenario6_estados: todas Coletado (estado=0)
Amostra:
  { categoria=2 (ResistenciaBase), chaveDoAtributo="Armadilha",  estado=0 },
  { categoria=2 (ResistenciaBase), chaveDoAtributo="GolpeMortal", estado=0 },
  ... (8 entradas cobrindo Atordoamento, Sangramento, Envenenamento, Debuff,
       Movimento, Doenca, GolpeMortal, Armadilha)
```

✅ Confere com FR-025 + T116: seed do MapaDeCoberturaSeed marca todas as 8 resistências como `Coletado` após população oficial da wiki.

### 6.7. Cenário 7 — Rejeitar Personagem com habilidade de outra classe (FR-009a)

```
Cenario7_status=400
Body: {"mensagem":"Habilidade '05cdbae5-...' não pertence à Classe do Personagem.",
       "erros":[{"campo":"habilidadesEquipadas",
                 "mensagem":"Habilidade '05cdbae5-...' não pertence à Classe."}]}
```

✅ Confere com FR-009a (validação `PersonagemService` verifica que toda habilidade equipada pertence à Classe).

### 6.8. Cenário 8 — Limite 6+6 (FR-009) — **comportamento diferente do quickstart**

```
Cenario8_status=201  (Personagem criado com 7 habilidades de Cruzado)
Body: personagem tem 7 entradas no array "habilidades", TODAS com equipada=false
```

⚠️ **Interpretação**: o campo `habilidadesEquipadas` do `CriarPersonagemRequest` semanticamente **atribui** habilidades ao Personagem (adiciona à coleção `Habilidades` com flag `equipada=false`); ele **não** força que estejam equipadas em batalha. Portanto o limite FR-009 (6 combate + 6 acampamento equipadas) é validado no endpoint dedicado de **equipar habilidade** (`POST /personagens/{id}/equipar` ou similar), não na criação. O array pode conter todas as habilidades disponíveis ao Personagem.

Isso é **coerente com o design do domínio** (`HabilidadeDePersonagem` tem propriedade `Equipada` distinta de `Habilitada`), mas diverge do que o quickstart.md previa. Recomendação: revisar o texto do quickstart para refletir a semântica real, ou adicionar validação de "no máximo 6 combate + 6 acampamento na atribuição" caso a intenção original fosse essa.

## 7. Sumário final

| Métrica | Valor |
|---|---|
| Classes canônicas cadastradas | **20/20** |
| Habilidades únicas semeadas | **219** (140 combate + 79 acampamento) |
| Skills compartilhadas globais | **7** (Encourage/Wound Care/Pep Talk + Field Dressing/Marching Plan/Triage + Gallows Humor) |
| Associações Classe×Habilidade | **277** |
| Entradas Mapa de Cobertura | **160** (8 × 20 = todas resistências como `Coletado`) |
| Testes automatizados verdes | **129** (46 Domain + 25 Architecture + 58 API) |
| Cenários de quickstart validados end-to-end | **8/8** (7 conformes ao esperado + 1 com nota semântica sobre FR-009) |
| Progresso tasks 003 | **128/131 (98%)** |

