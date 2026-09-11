# Data Model: Criação Completa de Personagem

## Personagem

Entidade jogável persistida com estado próprio e referências ao catálogo.

| Campo | Tipo | Obrigatório para novos registros | Regra |
|---|---|---:|---|
| `id` | identificador | sim | Único e estável. |
| `nome` | texto | sim | Não vazio; regras atuais do domínio. |
| `classe` | enum | sim | Classe existente no catálogo. |
| `nivel` | inteiro | sim | 0 a 6; nível do herói. |
| `aparencia` | enum | sim | `A`, `B`, `C` ou `D`; default de leitura legado `A`. |
| `armaEquipadaId` | identificador nulo | sim após derivação | Arma elegível para a classe. |
| `nivelDaArma` | inteiro nulo | sim após derivação | 1 a 5; nível escolhido da arma. Pode ser nulo somente para dados legados sem informação. |
| `armaduraEquipadaId` | identificador nulo | sim após derivação | Armadura elegível para a classe. |
| `nivelDaArmadura` | inteiro nulo | sim após derivação | 1 a 5; nível escolhido da armadura. Pode ser nulo somente para dados legados sem informação. |
| `resistencias` | objeto de valor | sim | Copiadas da classe no momento da criação, conforme regra existente. |
| `resistenciasExtras` | objeto de valor | sim | Derivadas das resistências da classe. |
| `habilidades` | coleção de associações | sim | Todas as habilidades de combate/acampamento da classe. |

Os atributos gerais não escolhidos pelo usuário (HP, dano, velocidade, movimento, precisão etc.) são preenchidos pelo perfil inicial explícito definido no design, preservando os defaults do fluxo atual até existir catálogo oficial por classe.

## HabilidadeDePersonagem

Associação viva entre o personagem e uma habilidade catalogada.

| Campo | Tipo | Regra inicial |
|---|---|---|
| `habilidadeId` | identificador | Deve pertencer à classe do personagem. |
| `numeroDoNivel` | inteiro | 1 para as quatro primeiras de cada categoria; 0 para o restante. |
| `treinada` | booleano | `true` quando `numeroDoNivel >= 1`; `false` quando 0. |
| `habilitada` | booleano | `true` para associações iniciais válidas. |
| `equipada` | booleano | `false` na criação; equipagem posterior segue regras próprias. |

A categoria é obtida da entidade `Habilidade` associada e não deve ser enviada como autoridade pelo cliente.

## ConfiguracaoDeCriacaoPersonagem

Entrada mínima do novo fluxo:

```text
Nome: texto
Classe: ClasseDeHeroi
Nivel: 0..6
NivelDaArma: 1..5
NivelDaArmadura: 1..5
Aparencia: A|B|C|D
```

Não contém IDs manuais de habilidades, arma ou armadura.

## Derivação de equipamentos

1. Buscar a arma catalogada com `ClasseElegivel == Classe`.
2. Confirmar que existem exatamente os níveis 1..5.
3. Selecionar o nível informado.
4. Repetir o processo para a armadura.
5. Gravar o ID do item e o nível escolhido no personagem.

## Derivação de habilidades

1. Buscar todas as habilidades associadas à classe.
2. Filtrar combate e acampamento; ignorar habilidades de inimigo.
3. Ordenar cada categoria por `NomeExibicao` e um desempate estável pelo ID.
4. Rejeitar se qualquer categoria tiver menos de quatro habilidades.
5. Criar todas as associações, marcando as quatro primeiras de cada categoria com nível 1 e as demais com nível 0.

## Compatibilidade e estados

- Registro novo: todos os campos derivados devem estar completos antes da persistência.
- Registro antigo: `Aparencia` ausente é lida como `A`; níveis de equipamento ausentes são representados como desconhecidos/compatíveis, sem atualização silenciosa.
- Falha de validação: nenhum `Personagem` novo ou associação parcial fica persistido.
