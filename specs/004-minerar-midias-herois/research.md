# Pesquisa: Importação Local de Assets Spine

## Decisões

### Usar a instalação local como única fonte

- **Decisão**: Ler exclusivamente o diretório de instalação indicado pelo operador.
- **Justificativa**: Preserva assets originais e elimina thumbnails, bloqueios de automação e dependência de rede.
- **Alternativas consideradas**: Coleta web foi rejeitada por não assegurar resolução original.

### Preservar conjuntos Spine sem conversão

- **Decisão**: Copiar `.png`, `.atlas` e `.skel` byte a byte como um conjunto Spine.
- **Justificativa**: O atlas referencia regiões da textura; o `.skel` contém poses e animações. GIF não preserva essa informação.
- **Alternativas consideradas**: Recorte manual e exportação GIF foram rejeitados por perda de qualidade e metadados.

### Descobrir heróis base e DLC recursivamente

- **Decisão**: Procurar `heroes/` na raiz e em subdiretórios DLC, associando pelo nome da pasta.
- **Justificativa**: A instalação contém os 15 heróis base em `heroes/` e heróis como `flagellant`, `musketeer`, `shieldbreaker`, `duelist` e `runaway` em DLCs.
- **Alternativas consideradas**: Caminhos DLC codificados foram rejeitados por fragilidade entre instalações.

### Usar manifesto para habilidade e região

- **Decisão**: Um manifesto JSON versionado declara associações entre classe, conjunto ou região de atlas e habilidade do catálogo.
- **Justificativa**: Nomes internos não são sempre equivalentes ao nome oficial da habilidade.
- **Alternativas consideradas**: Associação automática exclusiva pelo nome foi rejeitada por ambiguidade.

### Deduplicar por hash, nomear de modo legível

- **Decisão**: SHA-256 identifica conteúdo idêntico; o destino usa `classe/conjunto/nome-original.ext`.
- **Justificativa**: Mantém rastreabilidade e navegação humana sem copiar o mesmo binário duas vezes.
