# Modelo de Dados: Inventário de Assets Spine

## InventarioDeMidias

| Campo | Regra |
|---|---|
| versao | Versão do formato. |
| geradoEmUtc | Instante UTC de importação. |
| instalacaoOrigem | Raiz local lida somente para referência. |
| classes | Uma entrada para cada classe do catálogo. |
| arquivos | Assets únicos por SHA-256. |
| conjuntosSpine | Relações entre textura, atlas e esqueleto. |
| lacunas | Classe, conjunto ou habilidade ausente. |

## ArquivoImportado

| Campo | Regra |
|---|---|
| sha256 | Identifica conteúdo e suporta deduplicação. |
| caminhoOrigem | Caminho local na instalação. |
| caminhoDestino | Caminho relativo legível no acervo. |
| extensao | `.png`, `.atlas` ou `.skel`. |
| tamanhoBytes | Igual na origem e no destino. |
| classe | Classe associada. |

## ConjuntoSpine

| Campo | Regra |
|---|---|
| classe | Classe dona do conjunto. |
| nomeOriginal | Prefixo ou identificador do conjunto. |
| texturaSha256 | Referência obrigatória para PNG. |
| atlasSha256 | Referência obrigatória para atlas. |
| esqueletoSha256 | Referência opcional; ausência vira lacuna. |
| estado | Estado inferido do nome do conjunto. |
| paleta | Variante inferida do nome ou caminho. |
| regioes | Nomes lidos do atlas. |
| habilidades | Associações declaradas pelo manifesto. |

## ManifestoDeHabilidades

Cada entrada contém `classe`, `conjuntoOuRegiao` e uma ou mais habilidades originais do catálogo. Uma entrada é opcional: ausência gera lacuna, não associação especulativa.

## Regras de Integridade

- Um arquivo de destino tem o mesmo SHA-256 da origem.
- Um conjunto completo contém PNG e atlas; `.skel` é preservado quando localizado.
- Uma habilidade possui associação declarada ou lacuna verificável.
- Caminhos de destino são relativos e não derivados de forma insegura de texto externo.
- O inventário é salvo atomicamente após a importação.
