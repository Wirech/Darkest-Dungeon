# Feature Specification: Auditoria de Mídias de Armas, Armaduras e Itens

**Feature Branch**: `006-midias-armas-armaduras-itens`

**Created**: 2026-09-08

**Status**: Skeleton (não iniciada)

**Input**: Registro do escopo excluído explicitamente da Feature 005 (Q2 da 3ª clarify).

> **⚠️ Este arquivo é apenas um esqueleto** criado durante a Feature 005 para não esquecer o escopo excluído. NÃO iniciar `/speckit-clarify` ou `/speckit-plan` aqui até que a Feature 005 esteja implementada e o operador confirme que Feature 006 deve entrar na fila. Use este documento como ponto de partida rodando `/speckit-specify` sobre a descrição abaixo.

## Contexto de origem

Durante a 3ª sessão de `/speckit-clarify` da Feature 005 (2026-09-08), o operador respondeu **Q2 = A**: mídias/ícones de `Arma`, `Armadura` e `Item` ficam **fora do escopo da 005** e serão tratadas por uma feature dedicada. A Feature 004 (`004-minerar-midias-herois`) explicitamente exclui esses assets em seu FR-012 ("O escopo DEVE se limitar aos assets de heróis instalados localmente, sem baixar conteúdo de sites externos e sem incluir inimigos ou itens.").

Portanto:

- Nenhuma das features 003/004/005 audita ou modela mídias de equipamento e itens.
- Personagens semeados até a Feature 005 não têm ícones de arma/armadura/item — o campo de referência de asset simplesmente inexiste nessas entidades.
- Feature 003 já semeou 20 `Arma`s e 20 `Armadura`s com 5 níveis cada (owned type), mas sem qualquer referência a imagem.

## Escopo proposto (a ser refinado por /speckit-specify)

Categorias que devem ter mídias auditadas/importadas nesta feature:

1. **Armas** (20 classes × 5 níveis = 100 sprites de arma) — ícones e/ou sprites de combate por nível.
2. **Armaduras** (20 classes × 5 níveis = 100 sprites de armadura) — idem.
3. **Itens de acampamento / provisões** — bandagem, ervas medicinais, sangue-santo, brasa, chave-mestra, comida, tocha, água benta, etc.
4. **Troféus (Trinkets)** — todos os curiosos oficiais da wiki (colar do velho, argila corrompida, pé de coelho, olho fumegante, etc.).
5. **Consumíveis genéricos** — pergaminhos, poções, dinamite.

Categorias **fora do escopo** da Feature 006 (adiadas para futuras features 007+):

- Mídias de inimigos e chefes.
- Mídias de ambientes/tiles.
- Ícones de status/buffs/debuffs.

## Pré-requisitos técnicos prováveis

- **Reabrir Feature 004** ou criar Feature 004b para ampliar o pipeline de mineração — os arquivos de equipamentos ficam em pastas do jogo diferentes de `heroes/` (ex.: `panels/`, `items/`, `trinkets/`).
- Definir se as mídias vão como **Conjunto Spine** (para armas/armaduras animadas), **PNG estático** (para ícones), ou ambos.
- Definir se `Arma`/`Armadura`/`Item` ganham owned type de referência à mídia (análogo ao `AssetsDeClasse` da Feature 005) ou tabela separada.

## Ligação com a Feature 005

A Feature 005 estabeleceu o padrão de vincular assets ao inventário da Feature 004 via `ConjuntoSpineId` + `HashArquivo` (Q1). A Feature 006 deve seguir o mesmo padrão para consistência arquitetural.

## Comandos futuros

1. Após implementação da Feature 005, rodar `/speckit-specify` com descrição resumida: "importar e vincular mídias/ícones de armas, armaduras, itens de acampamento, troféus e consumíveis — expandindo o modelo da Feature 003 e reutilizando o inventário da Feature 004".
2. Depois `/speckit-clarify` (pelo menos 3-5 perguntas: origem dos assets no jogo, granularidade por nível de arma, categorias de item incluídas, padrão de aparência de arma, decisão sobre reabertura da 004).
3. Depois `/speckit-plan` → `/speckit-tasks` → `/speckit-analyze` → `/speckit-implement`.

## Referência cruzada

- Origem desta anotação: [specs/005-auditoria-habilidades-mineradas/spec.md § Out of Scope](../005-auditoria-habilidades-mineradas/spec.md)
- Sessão de clarify que originou: Q2 da 3ª rodada (2026-09-08) em [specs/005-auditoria-habilidades-mineradas/spec.md § Clarifications](../005-auditoria-habilidades-mineradas/spec.md)
