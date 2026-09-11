# Data Model: Frontend Interativo de Personagens

## PersonagemResumo

Projeção usada para uma linha ou cartão da lista de personagens.

| Campo | Tipo | Obrigatório | Regra |
|---|---|---:|---|
| `id` | identificador | sim | Identifica o personagem para consulta e exclusão. |
| `nome` | texto | sim | Nome válido segundo as regras do domínio. |
| `classe` | enum | sim | Classe de herói existente no catálogo. |
| `nomeClasse` | texto | sim | Nome amigável da classe para apresentação. |
| `hpAtual` | inteiro | sim | Maior ou igual a zero e menor ou igual a `hpMaximo`. |
| `hpMaximo` | inteiro | sim | Maior ou igual a zero. |
| `stress` | inteiro | sim | Entre 0 e 200. |
| `nivel` | inteiro | sim | Nível atual do personagem. |
| `habilidades` | coleção | sim | Habilidades atribuídas, com estado disponível para apresentação. |

## HabilidadeDePersonagemResumo

Representa uma habilidade associada a um personagem.

| Campo | Tipo | Obrigatório | Regra |
|---|---|---:|---|
| `id` | identificador | sim | Identifica a habilidade. |
| `nome` | texto | sim | Nome amigável da habilidade. |
| `categoria` | enum/texto | sim | Combate, acampamento ou outra categoria válida. |
| `habilitada` | booleano | sim | Estado de habilitação. |
| `treinada` | booleano | sim | Estado legado de treinamento. |
| `equipada` | booleano | sim | Indica se está equipada. |
| `numeroDoNivel` | inteiro | sim | 0 indica bloqueada; 1 a 5 indica nível treinado, quando disponível. |

## CriarPersonagem

Dados enviados pelo formulário para criar um personagem: nome, classe, atributos básicos, nível, stress, chance de virtude e IDs das habilidades selecionadas.

As regras de faixa são as do domínio atual, incluindo HP consistente, percentuais não negativos e nível entre 0 e 6. As habilidades devem ser válidas para a classe escolhida e respeitar os limites existentes.

## EstadoDaTela

Estado transitório da interface, não persistido:

```text
Carregando -> ProntaComDados
Carregando -> Vazia
Carregando -> ErroConsulta
Vazia/ProntaComDados -> FormularioCriacao
FormularioCriacao -> Salvando -> ProntaComDados ou ErroCriacao
ProntaComDados -> ConfirmandoExclusao -> Excluindo -> ProntaComDados ou ErroExclusao
```

Nenhuma transição de erro deve apagar dados que o usuário ainda pode corrigir ou repetir.

## Relacionamentos

- Um `PersonagemResumo` referencia uma classe de herói do catálogo.
- Um `PersonagemResumo` possui zero ou mais `HabilidadeDePersonagemResumo`.
- Uma exclusão usa o ID do personagem e não remove classes, habilidades ou itens compartilhados.
- O formulário carrega classes e habilidades do catálogo antes de permitir referências selecionáveis.