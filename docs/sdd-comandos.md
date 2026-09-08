# Comandos SDD do Spec Kit

Este guia documenta os comandos instalados pelo GitHub Spec Kit para este
repositorio. Eles implementam um fluxo de Spec-Driven Development (SDD):
primeiro descrevem o que deve ser construido, depois definem como construir,
quebram o trabalho em tarefas e so entao executam a implementacao.

## Visao geral do fluxo

Para uma funcionalidade nova, a ordem recomendada e:

```text
/speckit-constitution   (uma vez por projeto)
        |
/speckit-specify
        |
/speckit-clarify        (opcional, mas recomendado quando houver ambiguidades)
        |
/speckit-plan
        |
/speckit-checklist      (opcional, para revisar a qualidade dos requisitos)
        |
/speckit-tasks
        |
/speckit-analyze        (opcional, antes da implementacao)
        |
/speckit-implement
        |
/speckit-converge       (se ainda houver trabalho pendente)
```

Cada comando e usado como uma instrucao do GitHub Copilot no workspace. Por
exemplo:

```text
/speckit-specify Adicionar um inventario de herois com filtros por classe
```

Os comandos trabalham principalmente dentro de `specs/`, usando a pasta da
funcionalidade registrada em `.specify/feature.json`.

## 1. `/speckit-constitution`

### Para que serve

Cria ou atualiza a constituicao do projeto: principios obrigatorios de
qualidade, testes, experiencia, seguranca e governanca. A constituicao orienta
as etapas seguintes e deve refletir regras estaveis do projeto, nao detalhes de
uma unica funcionalidade.

### Quando usar

- No inicio do projeto, para preencher a constituicao inicial.
- Quando uma regra de engenharia do projeto mudar.
- Antes de especificar funcionalidades importantes, se a constituicao ainda
  estiver incompleta.

### O que informar

Passe os principios desejados depois do comando. Exemplo:

```text
/speckit-constitution Definir principios de qualidade de codigo, testes automatizados, acessibilidade e seguranca
```

### O que produz

- Atualiza `.specify/memory/constitution.md`.
- Mantem um relatorio de impacto de versao dentro do arquivo.

### Limites

Este comando altera somente a constituicao. Ele nao deve criar funcionalidades,
alterar codigo, gerar testes ou preparar um plano.

## 2. `/speckit-specify`

### Para que serve

Transforma uma descricao de produto em uma especificacao funcional focada no
que o usuario precisa e por que isso importa. A especificacao evita decidir a
implementacao cedo demais.

### Quando usar

Use para iniciar uma funcionalidade ou atualizar a especificacao de uma
funcionalidade existente.

### O que informar

Descreva o resultado desejado, os usuarios envolvidos e as regras conhecidas.
Foque no comportamento, nao em framework ou estrutura de arquivos.

```text
/speckit-specify Permitir que o jogador organize herois por esquadrao e veja os espacos disponiveis
```

### O que produz

- Cria uma pasta numerada em `specs/`, por exemplo `specs/001-organizar-herois/`.
- Cria `spec.md` com cenarios de usuario, requisitos funcionais, casos de
  borda e criterios de sucesso.
- Cria `checklists/requirements.md` para validar a qualidade da especificacao.
- Atualiza `.specify/feature.json` com a funcionalidade ativa.

### Limites

Pode deixar no maximo tres marcadores `[NEEDS CLARIFICATION]` quando uma
decisao realmente muda o escopo ou o comportamento. Nao deve escolher a
arquitetura nem implementar a funcionalidade.

## 3. `/speckit-clarify`

### Para que serve

Encontra ambiguidades importantes na especificacao ativa e faz perguntas
objetivas para resolve-las antes do planejamento.

### Quando usar

Use depois de `specify` e antes de `plan`, especialmente quando houver mais de
uma interpretacao razoavel para uma regra, fluxo, dado ou criterio de sucesso.

### O que informar

Normalmente nenhum argumento e necessario:

```text
/speckit-clarify
```

O comando faz ate cinco perguntas, uma por vez. As respostas sao gravadas na
propria especificacao.

### O que produz

- Atualiza o `spec.md` da funcionalidade ativa.
- Adiciona uma secao de clarificacoes com as decisoes tomadas.
- Revalida `checklists/requirements.md`, quando esse arquivo existe.

### Limites

Ele nao cria um plano, nao altera a arquitetura e nao implementa codigo. Se a
especificacao nao existir, execute `speckit-specify` primeiro.

## 4. `/speckit-plan`

### Para que serve

Converte a especificacao em um plano tecnico: tecnologias, arquitetura,
modelo de dados, contratos e estrategia de validacao.

### Quando usar

Use depois de `specify` e, de preferencia, depois de `clarify`.

### O que informar

O contexto tecnico pode ser passado no argumento, por exemplo:

```text
/speckit-plan Usar Python para a logica de dominio e persistir os dados em arquivos JSON locais
```

Se a tecnologia ja estiver definida no repositorio, o comando deve aproveita-la
em vez de inventar outra.

### O que produz

Dentro da pasta da funcionalidade ativa, normalmente cria:

- `plan.md`: decisoes tecnicas, estrutura e verificacoes de conformidade.
- `research.md`: decisoes de pesquisa e alternativas consideradas.
- `data-model.md`: entidades, campos, relacoes e validacoes, quando aplicavel.
- `contracts/`: contratos de interfaces externas, quando aplicavel.
- `quickstart.md`: cenarios executaveis de validacao ponta a ponta.

### Limites

O comando termina na fase de design. Ele nao cria a lista final de tarefas e
nao deve implementar a funcionalidade.

## 5. `/speckit-checklist`

### Para que serve

Cria uma checklist de qualidade dos requisitos. A checklist funciona como
"testes para o texto": verifica se os requisitos sao claros, completos,
consistentes e mensuraveis.

### Quando usar

Use depois de `specify`, ou depois de `plan` quando quiser uma revisao focada
em um risco especifico, como seguranca, acessibilidade ou UX.

### O que informar

Indique o foco desejado:

```text
/speckit-checklist Criar uma checklist de seguranca e tratamento de falhas
```

### O que produz

- Cria ou amplia um arquivo em `specs/<feature>/checklists/`.
- Mantem os itens novos desmarcados (`[ ]`) para revisao humana.

### Limites

Os itens avaliam a qualidade da especificacao, nao se o codigo funciona. Nao
marque automaticamente um item como concluido apenas porque a implementacao
existe.

## 6. `/speckit-tasks`

### Para que serve

Quebra o plano em tarefas pequenas, ordenadas por dependencia e agrupadas por
historia de usuario. Cada tarefa deve ser executavel sem exigir nova
interpretacao do escopo.

### Quando usar

Use depois que `spec.md` e `plan.md` estiverem prontos.

### O que informar

Normalmente nenhum argumento e necessario:

```text
/speckit-tasks
```

### O que produz

- Cria `tasks.md` na pasta da funcionalidade ativa.
- Organiza fases de setup, fundacao, historias de usuario e acabamento.
- Atribui IDs como `T001` e marca tarefas paralelizaveis com `[P]`.
- Inclui caminhos de arquivos, dependencias, oportunidades de paralelismo e
  estrategia de MVP.

### Formato das tarefas

As tarefas seguem este formato:

```text
- [ ] T012 [P] [US1] Criar o modelo de heroi em src/models/hero.py
```

O checkbox, o ID e o caminho do arquivo sao obrigatorios. O marcador da
historia (`[US1]`, `[US2]` etc.) e obrigatorio nas fases de historia de usuario.

## 7. `/speckit-analyze`

### Para que serve

Faz uma analise somente leitura de `spec.md`, `plan.md` e `tasks.md` para
encontrar requisitos sem cobertura, conflitos, duplicacoes, ambiguidades e
desalinhamento com a constituicao.

### Quando usar

Use depois de `tasks` e antes de `implement`.

### O que informar

Nenhum argumento e necessario:

```text
/speckit-analyze
```

### O que produz

Produz um relatorio na conversa com:

- achados classificados por severidade;
- tabela de cobertura de requisitos e tarefas;
- problemas de alinhamento com a constituicao;
- tarefas sem requisito correspondente;
- metricas de cobertura e ambiguidades.

### Limites

E estritamente somente leitura. Ele nao corrige automaticamente os documentos.
As correcoes devem ser feitas com `specify`, `plan` ou edicao orientada das
tarefas, conforme o problema encontrado.

## 8. `/speckit-implement`

### Para que serve

Executa as tarefas de `tasks.md`, respeitando a ordem, as dependencias e as
marcacoes de paralelismo. Tambem valida o estado das checklists antes de
comecar.

### Quando usar

Use somente depois de gerar e revisar `tasks.md`. O comando e a etapa que
altera o codigo da aplicacao.

### O que informar

Normalmente nenhum argumento e necessario:

```text
/speckit-implement
```

### O que faz

- Le `tasks.md`, `plan.md`, `spec.md` e documentos de design disponiveis.
- Verifica arquivos de exclusao necessarios para a tecnologia detectada.
- Executa as tarefas por fase.
- Marca tarefas concluidas como `[X]`.
- Executa validacoes e testes previstos no plano.

### Limites

Se alguma checklist tiver itens desmarcados, o comando para e pede confirmacao
antes de continuar. Se uma tarefa sequencial falhar, ele interrompe para evitar
construir em cima de uma base incompleta.

## 9. `/speckit-converge`

### Para que serve

Compara o estado atual do codigo com a especificacao, o plano e as tarefas e
identifica o que ainda falta depois de uma rodada de implementacao.

### Quando usar

Use depois de `implement` quando ainda houver duvida sobre cobertura ou quando
uma rodada de implementacao nao tiver concluido tudo.

### O que informar

Nenhum argumento e necessario:

```text
/speckit-converge
```

### O que produz

- Se houver lacunas, adiciona uma nova secao `Phase N: Convergence` ao final de
  `tasks.md`, com novos IDs de tarefa.
- Se tudo estiver atendido, deixa `tasks.md` byte a byte sem alteracao e
  informa que a implementacao convergiu.

### Limites

O comando e append-only: nao reescreve, renumera ou remove tarefas existentes.
Ele tambem nao altera codigo, `spec.md` ou `plan.md`. Depois de novas tarefas,
execute `speckit-implement` novamente.

## 10. `/speckit-taskstoissues`

### Para que serve

Converte as tarefas de `tasks.md` em issues do GitHub, preservando o ID de cada
tarefa para rastreabilidade.

### Quando usar

Use quando o projeto ja tiver tarefas prontas e o time quiser acompanha-las no
GitHub.

### O que informar

Normalmente nenhum argumento e necessario:

```text
/speckit-taskstoissues
```

### O que faz

- Confere o remote Git configurado.
- Procura issues existentes pelos IDs (`T001`, `T002` etc.) para evitar
  duplicacao.
- Cria issues com titulos como `T001: Criar o modelo de heroi`.
- Ignora tarefas que ja tenham uma issue correspondente.

### Limites e pre-requisitos

- O remote precisa apontar para um repositorio GitHub.
- E necessario ter acesso autenticado para criar issues.
- O comando nao deve criar issues em outro repositorio que nao seja o remote
  configurado.

## Fluxo pratico recomendado

1. Rode `/speckit-constitution` uma vez e estabeleca as regras do projeto.
2. Rode `/speckit-specify` descrevendo uma unica funcionalidade.
3. Rode `/speckit-clarify` se houver decisoes importantes em aberto.
4. Rode `/speckit-plan` para definir a solucao tecnica.
5. Rode `/speckit-checklist` se a funcionalidade tiver riscos relevantes.
6. Rode `/speckit-tasks` para gerar o trabalho executavel.
7. Rode `/speckit-analyze` para conferir a consistencia dos artefatos.
8. Rode `/speckit-implement` para implementar as tarefas.
9. Rode `/speckit-converge` se ainda existirem lacunas.
10. Repita implementacao e convergencia ate o relatorio indicar convergencia.
11. Use `/speckit-taskstoissues` quando quiser sincronizar as tarefas com o
    GitHub.

## Arquivos principais

| Arquivo | Finalidade |
|---|---|
| `.specify/memory/constitution.md` | Principios e regras do projeto |
| `.specify/feature.json` | Funcionalidade ativa |
| `specs/<feature>/spec.md` | O que deve ser construido |
| `specs/<feature>/plan.md` | Como sera construido |
| `specs/<feature>/tasks.md` | Trabalho executavel |
| `specs/<feature>/research.md` | Decisoes e alternativas pesquisadas |
| `specs/<feature>/data-model.md` | Entidades e relacionamentos |
| `specs/<feature>/quickstart.md` | Validacao ponta a ponta |
| `specs/<feature>/checklists/` | Checklists de qualidade dos requisitos |
