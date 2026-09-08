# Data Model: Entidades e Catálogo de Habilidades

## Entidade base

### Ser

Continua sendo a entidade base existente, identificável por `Guid`, com nome, tipo, vida, combate, defesa, movimento, turno e resistências comuns. Personagem e Inimigo reutilizam suas validações e seu identificador.

## Derivados e entidades

### Personagem

**Identidade**: `Id` herdado de Ser.

**Campos próprios**:
- `Classe`: catálogo fechado com as 20 classes da especificação.
- `Stress`: inteiro de 0 a 200.
- `ChanceDeVirtude`: inteiro de 0 a 100.
- `Dinheiro`: valor não negativo.
- `ArmaduraId`, `ArmaId`, `AcessorioId`: referências opcionais a Item ou herdeiro de Item.
- `PortasDaMorte`: flag.
- `RecuperouPortasDaMorte`: flag.
- `RecuperouDeUmAtaqueCardiaco`: flag.
- `Aflicao`: característica negativa opcional.
- `Virtude`: característica positiva opcional.

**Coleções**:
- vínculos de habilidades de Combate, com `Habilitada`.
- vínculos de habilidades de Acampamento, com `Treinada` e `Equipada`.
- `Inventario`, com até 4 itens por padrão.
- `Individualidades`, positivas ou negativas.
- `Doencas`.

**Regras**:
- Aflição e Virtude são mutuamente exclusivas, persistentes e podem ser nulas.
- Personagem criado recebe fotografia da configuração vigente da classe.
- Habilidade de Acampamento não treinada não pode ser equipada.
- No máximo 3 habilidades de Acampamento podem estar equipadas para uma aventura.
- O limite padrão de Combate é até 6; exceções acima de 6 exigem autorização explícita.

### Inimigo

**Identidade**: `Id` herdado de Ser.

**Campos próprios**:
- `TipoInimigo`: Humano, Besta, Profano, Sobrenatural, Vampírico, Casca ou Rochoso.
- resistências próprias de 0 a 100.

**Coleções**:
- vínculos com qualquer quantidade finita de habilidades.

**Regras**:
- resistência efetiva = resistência base do tipo + resistência própria.
- cada tipo começa com base 5 para cada resistência.
- total efetivo pode superar 100.
- não há limite de seis habilidades.
- habilidade duplicada é rejeitada.

### Item

**Identidade**: `Guid`.

**Campos mínimos**:
- `Nome`, obrigatório e não vazio.
- `Descricao`, opcional.
- tipo discriminado para permitir herdeiros futuros.

**Relações**:
- pode ser equipado em Armadura, Arma ou Acessório.
- pode ocupar uma posição do Inventário.
- não herda de Ser.

### Habilidade

**Campos**:
- `Id`, `Nome`, `Descricao`.
- `Categoria`: Combate ou Acampamento.
- disponibilidade: acessível a Personagem, exclusiva de Inimigo ou compartilhada.

**Regras**:
- nome não vazio.
- categoria válida.
- disponibilidade válida.
- entidade é reutilizada por classes, Personagens e Inimigos.

### ConfiguracaoClassePersonagem

**Campos**:
- `Id`.
- `Classe`.
- coleção de habilidades de Combate.
- coleção de habilidades de Acampamento.

**Regras**:
- no máximo 6 habilidades de cada categoria no padrão.
- nenhuma duplicidade.
- somente habilidades acessíveis a Personagem.
- pode ser alterada sem modificar automaticamente personagens já criados.

### VinculoHabilidadePersonagem

**Campos**:
- `PersonagemId`.
- `HabilidadeId`.
- `Categoria` ou categoria derivada da Habilidade.
- `Habilitada` para Combate.
- `Treinada` e `Equipada` para Acampamento.

**Regras**:
- unicidade por Personagem e Habilidade.
- estados incompatíveis rejeitados pelo domínio.

### VinculoHabilidadeInimigo

**Campos**:
- `InimigoId`.
- `HabilidadeId`.

**Regras**:
- unicidade por Inimigo e Habilidade.
- aceita habilidades exclusivas de Inimigo e compartilhadas.

### Individualidade e Doenca

Listas de características e doenças associadas ao Personagem. Individualidade registra polaridade positiva ou negativa; Doença registra o nome/descrição da condição. A evolução dos efeitos de jogo fica fora deste escopo.

## Relações principais

```text
Ser 1 ── 0..1 Personagem
Ser 1 ── 0..1 Inimigo
ClassePersonagem 1 ── 0..1 ConfiguracaoClassePersonagem
ConfiguracaoClassePersonagem N ── N Habilidade
Personagem N ── N Habilidade (com estado do vínculo)
Inimigo N ── N Habilidade
Personagem 0..1 ── 1 Item (Armadura)
Personagem 0..1 ── 1 Item (Arma)
Personagem 0..1 ── 1 Item (Acessório)
Personagem 1 ── 0..4 Item (Inventário padrão)
```

## Transições relevantes

1. Criação de Personagem: validar classe e configuração, copiar vínculos, inicializar Acampamento como não treinado/não equipado.
2. Treinamento de Acampamento: `Treinada: false -> true`.
3. Equipamento para aventura: somente `Treinada == true`; total equipado <= 3.
4. Stress atingir 100: atribuir uma Aflição ou Virtude, mantendo exclusividade e persistência.
5. Stress atingir 200 fora da Porta da Morte: registrar recuperação de Ataque Cardíaco conforme regra do domínio.
6. Resistência efetiva de Inimigo: calcular base do tipo + valor próprio.
