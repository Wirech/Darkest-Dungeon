# Pesquisa: Imagens no Card do Personagem

**Feature**: 010-imagens-card-personagem  
**Date**: 2026-09-11  
**Prerequisites**: [spec.md](./spec.md), Features 004 (acervo de heróis), 005 (`AssetsDeClasse` × aparência), 006 (mídia de arma/armadura), 007/009 (card da lista)

Não resta `NEEDS CLARIFICATION`. As quatro decisões da sessão de clarificação (retrato + corpo inteiro; todas as habilidades listadas inclusive Nv. 0; ícone único por habilidade; espaço com “sem imagem”) entram como restrições de desenho, não como perguntas abertas.

---

## Decisões

### O card da lista ainda não recebe mídia

- **Decisão**: Estender `GET /personagens` (`PersonagemResumoDto`) para incluir retrato, corpo inteiro, arma, armadura e ícone por habilidade no mesmo payload. A lista MUST continuar bastando para o card, sem N+1 de `GET /personagens/{id}` nem de catálogo.
- **Justificativa**: O card em [personagens.js](../../src/DarkestDungeon.Api/wwwroot/personagens/personagens.js) consome só a lista. `PersonagemDetalheDto` já tem `midiaArmaEquipada` / `midiaArmaduraEquipada` (006), mas o resumo da 009 não. SC-001 e o contrato 009 exigem um único GET.
- **Alternativas consideradas**: A UI chamar o detalhe por card (rejeitado: N+1 e quebra o contrato da lista). Servir só URLs hardcoded no JS (rejeitado: foge das camadas e do acervo).

### Retrato = `portrait_roster` da paleta; já vinculado em `AssetsDeClasse`

- **Decisão**: Retrato do card = PNG `*_portrait_roster.png` da pasta `{classe}_{A|B|C|D}` no inventário 004. O seed 005 já grava isso em `Classe.Assets` (`ConjuntoSpineId` = `CaminhoDestino`, `HashArquivo` = SHA-256, `Status` Coletado/Pendente). O resolvedor do card lê o asset da aparência do personagem; não reimporta.
- **Justificativa**: Clarificação: retrato na paleta A–D. Seed documentado em [AssetsDeClasseSeed.cs](../../src/DarkestDungeon.Infrastructure/Data/Seeds/AssetsDeClasseSeed.cs). FR-009.
- **Alternativas consideradas**: Usar `*_guild_header.png` (rejeitado: não varia A–D). Runtime Spine do conjunto de retrato (rejeitado: fora de escopo; o campo 005 guarda o PNG, não um trio atlas/skel).

### Corpo inteiro = PNG estático `sprite.idle` da mesma paleta

- **Decisão**: Corpo inteiro do card = PNG `*.sprite.idle.png` na pasta da aparência (`arquivos/{classe}/{prefixo}_{A|B|C|D}/anim/`). Sem player Spine, sem recorte de região do atlas, sem importar arquivo novo. Se o PNG for folha de atlas, o card ainda mostra esse arquivo estático oficial da paleta.
- **Justificativa**: Spec FR-001 pede corpo inteiro estático junto do retrato. O acervo 004 já tem o PNG por paleta (ex.: `arquivos/abominacao/abomination_B/anim/abomination.sprite.idle.png`). `AssetsDeClasse` hoje só indexa o retrato; o idle resolve-se no inventário na leitura. Constituição V: sem pipeline de recorte.
- **Alternativas consideradas**: Animar Spine no card (rejeitado: Out of Scope). `guild_header` (rejeitado: não é por aparência). `sprite.camp.png` (rejeitado: pose de acampamento, não o corpo de combate). Estender `AssetsDeClasse` com segundo arquivo nesta feature (adiado: lookup no inventário evita migração).

### Arma e armadura seguem `NivelDaArma` / `NivelDaArmadura` (1–5), não o nível de resolução

- **Decisão**: Imagem de equipamento = ícone do nível cadastrado no personagem. No disco o jogo usa índice 0–4 (`eqp_weapon_0.png` … `eqp_weapon_4.png`, idem `eqp_armour_*`). Nível 1 → `*_0`, nível 5 → `*_4`. Preferir o vínculo 006 (`NivelDeArma.Midia` / `NivelDeArmadura.Midia`) quando `Status = OK`; senão, lookup somente leitura no acervo pela convenção de nome. **Não** usar `MidiaDeItemDtoMapper.NivelDeMidia(NivelDeResolucao)` no card.
- **Justificativa**: FR-002/003/005 e clarificação: equipamento no nível certo. O mapper de detalhe atual escolhe o nível pela resolução — isso diverge do card 009 (que já mostra `nivelDaArma` / `nivelDaArmadura`) e MUST ser alinhado ao expor as imagens.
- **Alternativas consideradas**: Continuar mapeando pela resolução (rejeitado: viola FR-005). Inventar nível 1 quando o campo for nulo (rejeitado: edge case da spec).

### Ícone de habilidade = um PNG por habilidade de combate; acampamento só se já estiver no acervo

- **Decisão**: Combate: `assets/herois/manifesto-habilidades.json` associa `NomeOriginal` → `{classe}.ability.{one|two|…|seven}.png`; o inventário 004 tem o `CaminhoDestino`. O ícone **não** muda com `numeroDoNivel` (inclusive Nv. 0). Acampamento: a 006 deixou `raid/camping/skills/` fora do mapeamento e o manifesto 004 não lista camping; o card mostra “sem imagem” se não houver PNG já importado. Sem minerar nesta feature.
- **Justificativa**: Clarificação: um ícone por habilidade. FR-004/009. Constituição V.
- **Alternativas consideradas**: Importar `raid/camping/skills/` agora (rejeitado: Out of Scope / FR-009). Persistência `MidiaDeHabilidade` no domínio (adiado: o manifesto já é a associação; migração não é necessária para o card). Ícone diferente por nível (rejeitado na clarificação).

### Resolução somente leitura; sem migração de cadastro

- **Decisão**: Um resolvedor na Application monta o bloco de mídia do card a partir de: (1) vínculos SQL existentes (`AssetsDeClasse`, `MidiaDeItem` no nível); (2) inventários locais 004/006 por convenção de caminho; (3) manifesto de habilidades. Não grava personagem, não publica vínculos, não copia bytes. Ausência → `status: "Pendente"`, URL nula.
- **Justificativa**: FR-009; personagens já criados não precisam ser refeitos (Assumptions). Constituição V.
- **Alternativas consideradas**: Nova tabela `MidiaDoCard` (rejeitado: duplica 004–006). Job de publicação só para o card (rejeitado: a 006 já publica equipamento).

### Entrega do arquivo: estático sob `/acervo/`, não bytes no JSON

- **Decisão**: Mapear os diretórios `assets/herois/` e `assets/equipamentos-itens/` como arquivos estáticos em `/acervo/herois/` e `/acervo/equipamentos-itens/`. O DTO leva `arquivoInventarioId` (chave estável) + `url` relativa (`/acervo/herois/` + `CaminhoDestino`). A API não embute base64. Path traversal fora dessas raízes é recusado pelo provedor estático (raiz fixa).
- **Justificativa**: `UseStaticFiles` hoje só cobre `wwwroot`. Copiar o acervo para `wwwroot` duplicaria gigabytes. Endpoint `GET /midias/arquivos/{id}` exigiria I/O na Application. Constituição V + FR-009.
- **Alternativas consideradas**: Base64 no DTO (rejeitado: payload da lista). Copiar para `wwwroot` (rejeitado: duplicação). Novo controller de stream (adiado: estático basta).

### Ausência visível na UI: espaço + “sem imagem”

- **Decisão**: Todo slot (retrato, corpo, arma, armadura, cada habilidade listada) permanece no layout. Sem URL, `status != OK` ou `onerror` do `<img>` → texto canônico **sem imagem**. Sem silhueta, sem outra paleta, sem outro nível. Habilidade sem mídia continua com nome e Nv.
- **Justificativa**: Clarificação Q4; FR-006; SC-005; PT-BR (constituição IV).
- **Alternativas consideradas**: Omitir o slot (rejeitado). Placeholder de outra classe (rejeitado). Inglês “missing” (rejeitado).

### Detalhe `GET /personagens/{id}` alinha equipamento ao nível cadastrado

- **Decisão**: Ao montar mídia de arma/armadura no detalhe, usar `NivelDaArma` / `NivelDaArmadura` (com o mesmo resolvedor do card). Acessórios permanecem como na 006 (fora do card).
- **Justificativa**: Um personagem não pode ter ícone de resolução 3 e texto de arma 1. Contrato único de “nível certo”.
- **Alternativas consideradas**: Corrigir só a lista e deixar o detalhe 006 (rejeitado: duas semânticas no mesmo recurso).

---

## Integrações e padrões reutilizados

| Origem | O que o card reusa |
|---|---|
| Feature 004 | `assets/herois/inventario.json`, PNGs de retrato/idle/ability, manifesto |
| Feature 005 | `AssetsDeClasse` × `AparenciaDePersonagem` |
| Feature 006 | `MidiaDeItem` por nível; inventário `assets/equipamentos-itens/` (merge de hash) |
| Feature 007/009 | Card estático, `GET /personagens` autossuficiente, PT-BR |

Lista `ListarAsync` hoje não carrega `IItemRepository`. Passa a resolver arma/armadura da classe no nível do personagem (não só `ArmaEquipadaId`), para o card funcionar mesmo se o FK de item não estiver preenchido.

---

## Itens resolvidos (antes NEEDS CLARIFICATION)

| Tema | Resolução |
|---|---|
| Qual PNG é retrato vs corpo | `portrait_roster` vs `sprite.idle` da mesma pasta A–D |
| Índice arma 0–4 vs nível 1–5 | `arquivo = nível - 1` |
| Habilidades de acampamento sem ícone no acervo | Slot + “sem imagem” |
| Como o browser acha o arquivo | URL `/acervo/...` via static files |
| Lista vs detalhe | Mesmo resolvedor; lista não faz segundo GET |
