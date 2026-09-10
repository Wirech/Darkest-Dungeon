# Specification Quality Checklist: Auditoria das Habilidades Mineradas da Wiki

**Purpose**: Validate specification completeness and quality before proceeding to planning
**Created**: 2026-09-08
**Feature**: [spec.md](../spec.md)

## Content Quality

- [x] No implementation details (languages, frameworks, APIs)
- [x] Focused on user value and business needs
- [x] Written for non-technical stakeholders
- [x] All mandatory sections completed

## Requirement Completeness

- [x] No [NEEDS CLARIFICATION] markers remain
- [x] Requirements are testable and unambiguous
- [x] Success criteria are measurable
- [x] Success criteria are technology-agnostic (no implementation details)
- [x] All acceptance scenarios are defined
- [x] Edge cases are identified
- [x] Scope is clearly bounded
- [x] Dependencies and assumptions identified

## Feature Readiness

- [x] All functional requirements have clear acceptance criteria
- [x] User scenarios cover primary flows
- [x] Feature meets measurable outcomes defined in Success Criteria
- [x] No implementation details leak into specification

## Notes

- Items marked incomplete require spec updates before `/speckit-clarify` or `/speckit-plan`
- Baseline validation: Cruzado / Vestal / Ocultista / Médico da Peste tratadas como referência de "OK" no relatório.
- Restrição herdada da Feature 003 aceita: chance base > 100% permanece capada em 100% no modelo — não será reaberta.
- Skills compartilhadas (7 no total) auditadas uma única vez cada, evitando retrabalho por classe.
- Sessão de clarificação 2026-09-08 registrou 4 decisões: (1) modelagem de 5 níveis por habilidade como owned type; (2) semântica Treinada (Nivel≥1) vs Equipada (limite 3 apenas para acampamento); (3) `Aparencia` como enum A/B/C/D com mapa de assets por Classe; (4) `Experiencia` como XP acumulado + `Nivel` (Resolve Level 0-6) com nomes oficiais + tabela de limiares por `ModoDeCampanha` (Radiant/Darkest/Stygian) + bônus de +10% em 5 resistências e trap disarm por nível.
- Segunda rodada de clarificação (2026-09-08) registrou +2 decisões: (5) baseline mantém `OK` mesmo depois de Levels 2..5 preenchidos — mineração de níveis do baseline é "preenchimento inicial", não "correção"; (6) publicação da versão auditada é atômica dentro de uma única transação SQL, com log estruturado por habilidade/campo em caso de falha e janela de manutenção fora do horário das sessões live.
- Terceira rodada de clarificação (2026-09-08) registrou +5 decisões, focadas em consistência cross-feature identificada pelo `/speckit-analyze`: (7) `AssetsDeClasse` referencia o inventário da Feature 004 (Conjunto Spine + hash), sem duplicar paths ou bytes; (8) mídias de Arma/Armadura/Item ficam **fora de escopo** — nova seção "Out of Scope" adicionada, criando espaço para Feature 006 dedicada; (9) 4 classes baseline são **reminadas da wiki** e comparadas com o seed 003, sem privilégio — SC-002 revisto para meta ≥ 90% de OK; (10) nomes de Resolve Level traduzidos para PT-BR (`Curioso`/`Aprendiz`/`Aventureiro`/`Veterano`/`Mestre`/`Campeão`/`Lenda`) com `NomeOriginal` inglês para rastreabilidade, decisão análoga ao padrão `NomeExibicao`/`NomeOriginal` de `Habilidade`; (11) **modo de campanha único** — `ModoDeCampanha` (Radiant/Darkest/Stygian) removido do modelo; sistema usa apenas tabela única `{2, 8, 14, 24, 36, 48}` para o modo mais difícil.
- Nova US4 introduzida para agrupar a modelagem de níveis + campos novos de Personagem antes da publicação (US3).
- 17 SC totais (SC-001 a SC-017) — 8 novos derivados das 6 clarificações da 1ª/2ª rodada; SC-002 e SC-014 revistos na 3ª rodada; nenhum SC adicional criado (as decisões da 3ª rodada simplificaram/ajustaram os existentes).
- ~28 FRs totais (FR-001 a FR-014 + FR-007a-m + FR-009a-c subdivisões). Na 3ª rodada FR-007i, FR-007j, FR-007l, FR-007m e FR-012 foram revistos; nenhum FR novo adicionado — a decisão de modo único **simplificou** FR-007m em vez de expandir.
