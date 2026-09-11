# Pesquisa: Corpo Inteiro Composto e Animado

**Feature**: 012-corpo-inteiro-animado  
**Date**: 2026-09-11  
**Prerequisites**: [spec.md](./spec.md), Features 004 (acervo Spine), 007 (UI estática), 010 (slot `corpoInteiro`)

Não resta `NEEDS CLARIFICATION`. As três decisões da sessão de clarificação (versões espera/animado/caminhada; escolha só na listagem aberta; caminhada no lugar) entram como restrições de desenho.

---

## Decisões

### Player Spine 2.1 no navegador, sem importar mídia nova

- **Decisão**: Compor e animar o herói no card com um runtime Spine **2.1** (WebGL/canvas) vendido em `wwwroot/personagens/vendor/`, alimentado pelos `.atlas` + `.skel` da classe e pelo `.png` da paleta A–D já existentes no acervo 004. Sem MediaCollector, sem GIF, sem wiki, sem CDN em tempo de apresentação.
- **Justificativa**: O PNG `*.sprite.idle.png` da 010 é a **folha de partes** (atlas). O `.skel` do Cruzado contém a marca `2.1.27` (binário Spine 2.1). Sem esqueleto não há pose montada nem ciclo fiel. FR-001/004/009/010; constituição V (ferramenta que o asset já exige).
- **Alternativas consideradas**: Continuar com `<img>` da folha (rejeitado: viola FR-001). Recortar regiões do atlas sem ossos (rejeitado: pose deformada). Rasterizar GIF/WebP no servidor (rejeitado: mídia nova, FR-009). Spine Player 4.x / `@esotericsoftware/spine-player` atual (rejeitado: incompatível com skel 2.1). CDN (rejeitado: FR-010 e constituição V).

### Um esqueleto por ciclo, textura por paleta

- **Decisão**: Idle e walk **não** estão no mesmo `.skel`. Cada ciclo é um trio: atlas + skel **compartilhados na pasta da classe** (`arquivos/{classe}/anim/{prefixo}.sprite.{idle|walk}.{atlas,skel}`) + PNG **na pasta da aparência** (`arquivos/{classe}/{prefixo}_{A|B|C|D}/anim/{prefixo}.sprite.{idle|walk}.png`). O player MUST carregar a textura pelo URL da paleta, mesmo que a primeira linha do atlas cite só o nome do PNG.
- **Justificativa**: Inventário 004: atlas/skel de idle/walk sem sufixo `_A`; PNGs repetidos em `_A`…`_D`. Atlas do Cruzado começa com `crusader.sprite.idle.png`. FR-002 (paleta do personagem).
- **Alternativas consideradas**: Um único skel com várias animações (não é o layout do jogo). PNG da pasta `anim/` da classe (rejeitado: sem paleta). Duplicar atlas/skel por paleta (rejeitado: importação nova).

### “Em espera” e “Animado” usam o conjunto idle; “Caminhada” usa o walk

- **Decisão**: Conjunto **idle** completo (atlas + skel + PNG da paleta) habilita “Em espera” (pose composta, tempo 0 / pausado) e “Animado” (ciclo idle em loop). Conjunto **walk** completo habilita “Caminhada” (ciclo walk em loop, no lugar). Ataque e demais `sprite.*` ficam fora. Nomes de animação esperados: o último segmento do stem (`idle`, `walk`); se o skel usar outro, o player usa a primeira animação do esqueleto.
- **Justificativa**: Clarificação Q1; FR-004. Arquivos `sprite.attack_*` existem e MUST NÃO ser expostos.
- **Alternativas consideradas**: “Em espera” = PNG estático da folha (rejeitado: FR-001). “Animado” = outro arquivo (camp/combat) (rejeitado: fora do pedido).

### Contrato: enriquecer `midias.corpoInteiro`, sem migração

- **Decisão**: Estender o slot de corpo no `GET /personagens` (e detalhe) com conjunto idle, conjunto walk e lista de versões (id + rótulo PT-BR + disponível). Manter `url` / `arquivoInventarioId` apontando ao PNG idle da paleta (regressão 010). `status` do slot passa a `OK` só com conjunto idle **completo** (composto possível); incompleto → `Pendente`, `url` nula no sentido de apresentação (folha não é substituto). Sem tabela nova, sem gravar versão no personagem.
- **Justificativa**: Lista autossuficiente (010/009). FR-005/009. Testes 010 leem `CorpoInteiro.Url` com `sprite.idle` e `_A`/`_B` — o PNG da paleta continua sendo essa URL quando o conjunto idle está OK.
- **Alternativas consideradas**: Segundo GET de conjunto (rejeitado: N+1). Campo só no JS com paths hardcoded (rejeitado: foge das camadas). Persistência da versão escolhida (rejeitado: clarificação Q2).

### MIME de `.atlas` e `.skel` no static `/acervo`

- **Decisão**: Mapear no `UseStaticFiles` do acervo: `.atlas` → `text/plain; charset=utf-8`, `.skel` → `application/octet-stream`. PNG inalterado. Sem endpoint de stream.
- **Justificativa**: O provedor estático padrão do ASP.NET Core não serve extensões desconhecidas (404), o que quebraria o player. Constituição V.
- **Alternativas consideradas**: Renomear para `.txt`/`.bin` (rejeitado: muda acervo). Controller que lê disco (rejeitado: I/O na API além do estático).

### Seletor e estado só no cliente

- **Decisão**: O HTML/JS do card ganha um controle PT-BR por card (`Em espera`, `Animado`, `Caminhada`). Padrão “Em espera”. Estado em memória da página; `carregarLista` / F5 descarta. Independente por `personagem.id`. Opções indisponíveis não são acionáveis.
- **Justificativa**: Clarificação Q2; FR-003/005/008. Feature 007: UI estática na mesma origem, sem framework novo.
- **Alternativas consideradas**: `localStorage` (rejeitado: Q2). Query string (rejeitado: compartilha entre cards). Controlo no backend (rejeitado: stateless).

### Caminhada no lugar, recorte no espaço

- **Decisão**: O canvas/WebGL do corpo vive dentro do `figure` “Corpo inteiro” com recorte (`overflow: hidden`). Não aplicar (ou anular) a translação de raiz do ciclo walk. `pointer-events` do canvas não cobrem retrato, texto nem ações; o seletor permanece clicável.
- **Justificativa**: Clarificação Q3; FR-011/012; US3.
- **Alternativas consideradas**: Deixar o root bone andar e recortar (Option B da Q3 — rejeitado). Permitir sair do card (rejeitado).

### Dependência frontend vendida, sem novo projeto

- **Decisão**: Copiar o runtime Spine 2.1 (JS + licença Esoteric) para `wwwroot/personagens/vendor/spine-2.1/`. Sem npm no build da API, sem quinto projeto, sem TypeScript extra. Falha ao carregar o runtime ou o conjunto → “sem imagem” naquele card.
- **Justificativa**: 007 evitou framework; aqui o benefício é o requisito. Constituição I/V.
- **Alternativas consideradas**: Pacote npm e bundler (rejeitado: pipeline nova). Projeto Blazor/WASM (rejeitado: complexidade). PixiJS + pixi-spine (rejeitado: stack maior).

---

## Integrações reutilizadas

| Origem | Uso nesta feature |
|---|---|
| Feature 004 | `.atlas` / `.skel` em `arquivos/{classe}/anim/`; PNG por paleta; `inventario.json` |
| Feature 007 | `wwwroot/personagens` HTML/CSS/JS |
| Feature 010 | `GET /personagens` + `midias.corpoInteiro` + `/acervo/herois` |
| Feature 011 | Sem mudança de camping |

---

## Itens resolvidos (antes abertos no contexto técnico)

| Tema | Resolução |
|---|---|
| Folha vs herói montado | Player Spine; folha nunca é o visual principal |
| Versão do Spine | 2.1.x (skel `2.1.27`); runtime 2.1, não 4.x |
| Onde estão atlas/skel vs PNG | Classe `anim/` vs paleta `{prefixo}_{A-D}/anim/` |
| Idle vs walk | Dois trios; ataque fora |
| Persistência da escolha | Só na sessão da listagem |
| Walk com deslocamento | No lugar, dentro do espaço |
| MIME `.skel`/`.atlas` | Mapear no static files |
| Importação | Nenhuma |
