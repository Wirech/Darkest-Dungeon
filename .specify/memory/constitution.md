<!--
Sync Impact Report
- Version change: template -> 1.0.0
- Modified principles: none; all principles were defined during initial ratification
- Added sections: Restrições Técnicas e de Operação; Fluxo de Desenvolvimento e Qualidade
- Removed sections: none
- Follow-up TODOs: none
-->

# Darkest Dungeon Constitution

## Core Principles

### I. Arquitetura em Camadas

O sistema MUST separar responsabilidades em camadas bem definidas, no mínimo
apresentação/API, aplicação, domínio e infraestrutura. Cada camada MUST
depender apenas das abstrações permitidas pela arquitetura, e regras de negócio
MUST permanecer fora dos controladores e do acesso direto ao banco. Novas
funcionalidades MUST indicar em quais camadas seus arquivos serão criados ou
alterados.

### II. SQL Server como Persistência Oficial

O SQL Server MUST ser o banco de dados oficial da aplicação. Acesso a dados
MUST ocorrer por uma camada de infraestrutura isolada, com consultas
parametrizadas, migrações versionadas e validação explícita de falhas de
conexão. O código de domínio MUST permanecer independente de detalhes de
driver, ORM ou sintaxe específica do banco sempre que isso for razoável.

### III. Contratos de Backend Verificáveis

Toda funcionalidade que expuser ou alterar um endpoint MUST ter testes no
backend cobrindo método, rota, autenticação ou autorização aplicável, dados de
entrada, códigos de status e formato dos retornos. Os testes MUST incluir pelo
menos o fluxo de sucesso e os erros relevantes definidos na especificação.
Uma alteração de contrato MUST atualizar seus testes antes de ser considerada
concluída.

### IV. Português do Brasil como Idioma do Produto

O produto, suas mensagens de erro, documentação funcional, nomes apresentados
ao usuário e contratos textuais MUST ser escritos em Português do Brasil
(PT-BR). Termos técnicos sem tradução adequada podem permanecer no código,
mas textos de interface, logs voltados ao usuário e respostas de API destinadas
ao consumo humano MUST seguir PT-BR de forma consistente.

### V. Operação Remota e Simplicidade Proporcional

O sistema MUST suportar uso por múltiplas pessoas através da internet e acesso
por VPN, considerando hospedagem controlada pelo responsável pelo projeto.
Endpoints MUST ser stateless quando possível e configurações de ambiente MUST
ser separadas do código para permitir implantação e manutenção remotas. O
projeto MUST adotar a solução mais simples que atenda aos requisitos, sem
introduzir complexidade de segurança, infraestrutura ou abstrações que não
tenham benefício demonstrável. Mesmo com segurança como prioridade secundária,
segredos MUST ficar fora do repositório e a aplicação MUST validar entradas e
controlar acesso básico aos recursos.

## Restrições Técnicas e de Operação

O backend MUST expor endpoints documentados e testáveis. O armazenamento
persistente MUST usar SQL Server. A implantação MUST permitir execução em um
ambiente hospedado pelo responsável, com acesso externo pela internet e acesso
alternativo via VPN. Configurações como conexão do banco, portas, URLs e
credenciais MUST ser fornecidas por variáveis de ambiente ou mecanismo
equivalente, nunca por valores sensíveis versionados.

Requisitos de segurança avançada, alta disponibilidade e observabilidade
corporativa não são prioridades iniciais do projeto, mas falhas básicas de
isolamento de credenciais, validação de entrada e controle de acesso não podem
ser introduzidas deliberadamente.

## Fluxo de Desenvolvimento e Qualidade

Cada funcionalidade MUST seguir o fluxo SDD: especificação, esclarecimento de
ambiguidades quando necessário, plano técnico, tarefas, implementação e
convergência. Antes da implementação, a especificação e o plano MUST estar
consistentes com esta constituição.

Uma tarefa só pode ser marcada como concluída quando o código correspondente,
os testes de backend dos endpoints afetados e a validação prevista no plano
estiverem concluídos. Alterações que afetem persistência MUST incluir revisão
de migração e compatibilidade dos dados. Alterações que afetem implantação
MUST documentar as configurações necessárias para acesso por internet ou VPN.

## Governance

Esta constituição prevalece sobre práticas locais conflitantes. Alterações
devem ser propostas por uma atualização explícita deste arquivo, acompanhadas
de um Sync Impact Report e justificativa objetiva. A revisão MUST verificar
arquitetura em camadas, uso de SQL Server, cobertura dos endpoints, PT-BR e
requisitos de operação remota.

A versão segue versionamento semântico: MAJOR para remoção ou redefinição
incompatível de princípios; MINOR para novos princípios ou requisitos
obrigatórios; PATCH para esclarecimentos sem mudança de obrigação. A
constituição deve ser revisada antes de cada mudança arquitetural relevante e
durante a revisão de cada funcionalidade que altere contratos de backend,
persistência ou implantação.

**Version**: 1.0.0 | **Ratified**: 2026-09-07 | **Last Amended**: 2026-09-07
