# Research: Frontend Interativo de Personagens

**Feature**: `007-frontend-personagens`
**Data**: 2026-09-10

## Decisão 1: Hospedar a interface na mesma aplicação da API

**Decision**: A interface será composta por arquivos web estáticos servidos pela própria aplicação da API, em uma página dedicada de personagens.

**Rationale**: O repositório não possui projeto frontend, `package.json`, `wwwroot` ou framework web. Servir a interface na mesma origem reduz configuração, evita CORS e preserva a solução proporcional ao escopo.

**Alternatives considered**:

- Criar um SPA separado: rejeitado porque adicionaria um segundo projeto, pipeline e conjunto de dependências sem benefício demonstrado para três fluxos simples.
- Adicionar Blazor/Razor Pages: rejeitado porque introduziria uma nova camada de UI no backend e não existe padrão correspondente no projeto atual.

## Decisão 2: Completar os contratos de personagem antes da UI

**Decision**: Adicionar operações de listagem e exclusão no serviço/repositório de personagens e expor `GET /personagens` e `DELETE /personagens/{id}`.

**Rationale**: A API atual possui somente consulta individual, criação e equipagem. O frontend não consegue listar o catálogo nem remover registros sem esses contratos.

**Alternatives considered**:

- Fazer a lista por chamadas conhecidas a IDs: rejeitado porque não representa o catálogo completo e cria comportamento frágil.
- Ocultar a exclusão no frontend: rejeitado porque a especificação exige exclusão persistida e confirmação real do serviço.

## Decisão 3: Separar resumo de listagem do detalhe

**Decision**: Criar um DTO de resumo para a lista, mantendo o DTO de detalhe para consulta individual e retorno de criação.

**Rationale**: A lista precisa de nome, classe, HP, stress, nível e habilidades sem carregar dados que não são necessários para cada linha. O detalhe atual não contém todos os campos de apresentação, então o resumo define um contrato explícito para a tela.

**Alternatives considered**:

- Reutilizar o DTO de detalhe: rejeitado porque mistura necessidades de tela e pode ampliar payload sem necessidade.
- Retornar entidades de domínio: rejeitado por violar a separação de camadas e expor estrutura interna.

## Decisão 4: Validar em duas camadas

**Decision**: O frontend fará validações imediatas de formato e faixa para orientar o usuário, enquanto o backend continuará sendo a autoridade das regras de domínio.

**Rationale**: A validação local melhora a experiência, mas não é confiável para consistência do catálogo, especialmente em uso remoto ou concorrente.

**Alternatives considered**:

- Validar somente no frontend: rejeitado por permitir dados inválidos por outros consumidores.
- Validar somente após envio: rejeitado porque produz feedback tardio e piora o preenchimento.

## Decisão 5: Testar contrato no backend e fluxo principal no navegador

**Decision**: Cobrir listagem, criação e exclusão, incluindo erros, nos testes da API; validar a interface com um roteiro de navegador em estados preenchido, vazio, erro, criação e exclusão.

**Rationale**: A constituição exige testes de contratos backend, enquanto a responsividade e os estados visuais só podem ser confirmados na experiência renderizada.

**Alternatives considered**:

- Confiar apenas em testes de unidade de domínio: rejeitado porque não cobre HTTP, persistência nem estados da interface.
- Adicionar um framework de testes frontend agora: adiado porque não existe infraestrutura frontend e o fluxo manual automatizável pelo navegador cobre o MVP com menor custo.