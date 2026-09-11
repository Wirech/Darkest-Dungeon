# Research: Criação Completa de Personagem

**Feature**: `008-criacao-personagem-completa`
**Data**: 2026-09-10

## Decisão 1: A criação recebe somente seis escolhas essenciais

**Decision**: O contrato de criação recebe `Nome`, `Classe`, `Nivel`, `NivelDaArma`, `NivelDaArmadura` e `Aparencia`; IDs de habilidades, arma e armadura não são necessários para o fluxo padrão.

**Rationale**: O usuário escolhe a configuração do herói, enquanto o serviço deve montar as associações válidas. Isso evita que o cliente envie referências inconsistentes ou monte manualmente o estado interno.

**Alternatives considered**:

- Manter todos os atributos de combate no formulário: rejeitado porque contradiz o fluxo essencial solicitado e mantém a criação parcialmente manual.
- Aceitar IDs de habilidades/equipamentos do cliente: rejeitado porque permite contornar classe, níveis e seleção 4+4.

## Decisão 2: Persistir o nível selecionado dos equipamentos no Personagem

**Decision**: `Personagem` terá `NivelDaArma` e `NivelDaArmadura`, além dos IDs dos itens e de `Aparencia`.

**Rationale**: Arma e armadura possuem cinco níveis internos, e somente o ID do item não identifica qual nível está sendo usado. Guardar o nível no personagem torna a escolha explícita, consultável e estável para mídias e atributos derivados.

**Alternatives considered**:

- Inferir o nível pelo nível do herói: rejeitado porque o usuário pode escolher níveis diferentes do nível do herói.
- Guardar apenas uma cópia completa do nível do item: rejeitado porque duplica catálogo e dificulta sincronização.

## Decisão 3: Seleção determinística das habilidades iniciais

**Decision**: Listar as habilidades válidas da classe, separar `HabilidadeDeCombate` e `HabilidadeDeAcampamento`, ordenar por `NomeExibicao` e selecionar as quatro primeiras de cada categoria para nível 1. Todas as demais ficam no nível 0.

**Rationale**: O repositório já ordena habilidades por nome, e a ordenação explícita evita resultados diferentes entre execuções ou provedores de banco. Habilidades de inimigo nunca entram na inicialização do personagem.

**Alternatives considered**:

- Selecionar pela ordem de inserção: rejeitado porque depende do provedor e do seed.
- Permitir que o usuário escolha as oito habilidades: rejeitado porque a regra exige inicialização automática.

## Decisão 4: Falhar antes de persistir quando a configuração não é possível

**Decision**: Validar classe, quatro habilidades por categoria, item elegível, nível de item e aparência antes de adicionar o personagem; executar a gravação em uma unidade transacional da operação.

**Rationale**: A criação não pode deixar personagem sem equipamento, com progressão incompleta ou com associações parciais.

**Alternatives considered**:

- Criar primeiro e completar associações depois: rejeitado por permitir estados inválidos observáveis.
- Escolher fallback de nível/item: rejeitado porque esconderia uma escolha inválida do usuário.

## Decisão 5: Compatibilidade de dados antigos

**Decision**: Novos campos persistidos aceitarão defaults de leitura compatíveis: aparência `A` e nível de equipamento `null`/valor compatível quando o registro antigo não os possuir. Nenhum registro antigo será reescrito automaticamente durante a leitura.

**Rationale**: A feature deve ser implantável sem invalidar personagens já existentes; a ausência do dado deve ser distinguível de uma nova escolha explícita quando necessário.

**Alternatives considered**:

- Recalcular e salvar todos os personagens antigos no startup: rejeitado por alterar dados sem ação explícita e aumentar risco de migração.
- Exigir migração manual de todos os registros antes de ler: rejeitado porque bloqueia compatibilidade gradual.

## Decisão 6: Defaults dos atributos não escolhidos

**Decision**: Os atributos gerais que deixam de ser informados pelo formulário serão encapsulados em um perfil inicial explícito, preservando os valores-base usados pelo fluxo atual até que um catálogo oficial de atributos por classe seja introduzido.

**Rationale**: O catálogo atual de `Classe` fornece resistências, mas não fornece todos os atributos de combate. Usar um perfil explícito evita inventar valores diferentes por caminho e mantém compatibilidade com a criação existente.

**Alternatives considered**:

- Derivar HP/dano/velocidade de arma ou classe sem dados oficiais: rejeitado por produzir regras não sustentadas pelo catálogo.
- Manter esses campos obrigatórios na UI: rejeitado porque não atende ao fluxo de seis escolhas solicitado.
