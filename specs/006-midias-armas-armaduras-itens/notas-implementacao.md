# Notas de implementação — Feature 006

**Branch**: `006-midias-armas-armaduras-itens`  
**Data**: 2026-09-10

## Estado inicial (T003)

- 20 armas e 20 armaduras do seed 003 permanecem intocadas (Ids e cinco níveis numéricos) no código; neste banco de desenvolvimento **não há seed HasData** de armas/armaduras.
- Publicador 005 (`IPublicadorAtomicoService`) **fora de escopo**: não alterar nem chamar no fluxo 006.
- Vocabulário de cobertura: `OK` / `Parcial` / `Pendente`.
- `Esperados` no relatório conta **itens** (Arma = 20, Armadura = 20), não 100 vínculos.

## Baseline de testes (T001)

- Baseline pré-006 (histórico desta sessão): suíte 005/003 verde antes das mudanças de domínio.
- Validação final (T070, 2026-09-10): `dotnet test DarkestDungeon.sln --nologo --verbosity minimal`
  - **total: 246; falhou: 0; bem-sucedido: 246; ignorado: 0**
  - Domain.Tests êxito; Architecture.Tests êxito; Api.Tests êxito (51,7s).

## Migration head (T002)

- Docker: `mssql-dd` **Up** (verificado 2026-09-10).
- `dotnet ef migrations list --project src/DarkestDungeon.Infrastructure --startup-project src/DarkestDungeon.Api`:
  - `20260908131601_SchemaCompleto`
  - `20260909002441_NiveisEProgressao`
  - `20260909004439_LogsDePublicacao`
  - **`20260910142854_MidiasDeEquipamentoEItens`** (head 006, aplicada)
- Head anterior à 006: `20260909004439_LogsDePublicacao`.

## IPublicadorAtomicoService (T006)

- Arquivo `src/DarkestDungeon.Application/Publicacao/IPublicadorAtomicoService.cs` **não foi alterado** nesta feature (`git status` limpo; último commit `a7ce662` Feature 005).
- Fluxo 006 usa `IPublicadorDeVinculosDeMidia` / `PublicadorDeVinculosDeMidia`.
- `IPublicadorAtomicoService` permanece registrado no DI e em `POST /api/publicacao` (005).
- Teste de arquitetura: `PublicadorDeVinculos_nao_depende_do_publicador_005`.

## CLI fixture (T034)

Comando (ModoEquipamentos exige `--categoria`; o texto da task omitia as flags, mas o Program ramifica só com categorias):

```text
dotnet run --project tools/DarkestDungeon.MediaCollector -- --origem tests/DarkestDungeon.Api.Tests/ColetaMidias/Fixtures/Equipamentos --saida artifacts/midias-006 --continuar --categoria arma --categoria armadura --categoria acessorio --categoria acampamento --categoria consumivel
```

- Resultado: **11 arquivos inventariados; 6 lacunas.**
- Simular (`--simular`, saída `artifacts/midias-006-sim`): mesmos totais; **sem pasta `arquivos/`** (nenhuma cópia).
- Categorias no inventário: Arma, Armadura, Acessorio, ItemDeAcampamento, Consumivel, NaoAssociado (órfão `inventory/misc/orphan.png`).
- Lacunas: pastas DLC esperadas ausentes na fixture (`dlc/heroes`, `dlc/inventory/...`); processo não abortou.
- Fixture PNG mínimo: bytes idênticos → SHA-256 compartilhado e `Reutilizado=true` após o primeiro blob.

## SQL NiveisArma / NiveisArmadura (T024 / T047)

Aplicado: `dotnet ef database update` → `20260910142854_MidiasDeEquipamentoEItens`.

```text
SELECT COUNT(*) FROM NiveisArma;        -- 0
SELECT COUNT(*) FROM NiveisArmadura;    -- 0
SELECT COUNT(*) FROM Itens;             -- 0
```

**Não há seed HasData de 20 armas × 5 níveis neste banco.** Research 003 previa seed; o schema 006 não adiciona `HasData`. Contagem 100 **não se aplica** a esta instância vazia. Contratos US2 cobrem níveis + mídia em InMemory com dados de teste.

## Seed de acessórios 003 (T065)

- `COUNT(*) FROM Itens WHERE Discriminador = 'Acessorio'` = **0** neste `DarkestDungeon` local (sem seed).
- Contrato `POST_publicacao_acessorio_cria_extra_sem_alterar_seed`: trinket novo (`Comum`, efeitos vazios) sem recriar o acessório ancestral; suíte 246/246.
- Publicador não faz UPDATE em `Personagens`.

## SC-009 amostral (T069)

Amostra de `artifacts/midias-006/inventario.json` (11 entradas):

| Arquivo | Categoria |
|---------|-----------|
| crusader_weapon_1..5.png | Arma |
| crusader_armour_1.png | Armadura |
| lucky_test_amulet.png | Acessorio |
| torch.png | ItemDeAcampamento |
| quest_item.png | Consumivel |
| raid_consumable.png | Consumivel |
| orphan.png | NaoAssociado |

Nenhuma categoria de herói/habilidade 004 nas entradas de equipamento. `DeclaracaoDeUso` presente.

## Quickstart (T071)

| Cenário | Resultado |
|---------|-----------|
| 1 Inventariar (simular + continuar) | PASS — 11 arquivos / 6 lacunas; simular sem cópia |
| 2 Dedupe 004 | PASS nos testes CLI (`Reutilizado`); fixture 006 usa hash compartilhado |
| 3 Publicar armas + GET | PASS contratos Feature006 (InMemory); SQL local sem 100 linhas de seed |
| 4 Personagem IDs intactos | PASS `ItensEPersonagemMidiaTests` / publicação sem UPDATE Personagens |
| 5 Troféus / acampamento / consumível | PASS `ItensAcampamentoConsumivelEndpointsTests` |
| 6 Relatório cobertura | PASS `CoberturaMidiasEndpointsTests` (OK/Parcial/Pendente PT-BR) |
| 7 409 mesma categoria + 500 rollback | PASS `PublicacaoVinculosEndpointsTests` |

## Observações

- CLI 006 só dispara com `--categoria` (`ModoEquipamentos`). Sem categoria, pipeline 004 permanece.
- Isolation de testes: `ClienteComLeitor` / `ClienteComInventario` usam `Testing:DatabaseName` único para não compartilhar InMemory.
- TPH owned `Midia_*` em `NiveisArma`, `NiveisArmadura` e `Itens`.
