# Implementation Plan: Importação de Mídias Originais dos Heróis

**Branch**: `004-minerar-midias-herois` | **Date**: 2026-09-08 | **Spec**: [spec.md](spec.md)

## Summary

Criar uma ferramenta CLI local que lê a instalação licenciada de Darkest Dungeon, descobre assets de heróis base e DLC, copia os arquivos Spine originais sem recompressão e produz um inventário rastreável. Associações entre regiões de atlas e habilidades vêm de um manifesto versionado, com lacunas explícitas quando não houver mapeamento.

## Technical Context

**Language/Version**: C# 14 com .NET 10.0.

**Primary Dependencies**: `System.IO`, `System.Security.Cryptography` e `System.Text.Json` da plataforma; parser local para texto de atlas Spine.

**Storage**: Arquivos importados em `assets/herois/`; `inventario.json` e `manifesto-habilidades.json` versionados. Não há banco, rede ou alteração de API.

**Testing**: xUnit com fixtures locais contendo PNG, atlas e skel mínimos; validação de cópia byte a byte, descoberta de DLC, conjuntos incompletos, manifesto e deduplicação.

**Target Platform**: Windows ou Linux com .NET 10 e instalação local do jogo legível.

**Project Type**: Ferramenta de linha de comando local isolada em `tools/`.

**Performance Goals**: Importar todos os assets instalados sem requisições externas e sem alterar bytes; permitir reexecução sem cópias redundantes.

**Constraints**: Somente leitura da instalação; sem download, conversão, recompressão ou exportação de GIF; escrita atômica do inventário; caminhos configuráveis; saída legível por classe e conjunto; DLCs descobertas recursivamente.

**Scale/Scope**: 20 classes catalogadas; instalação base mais DLCs presentes; formatos `.png`, `.atlas` e `.skel` dentro dos diretórios de heróis.

## Constitution Check

| Principle | Status | Evidence |
|---|---|---|
| I. Arquitetura em Camadas | PASS | A CLI é uma ferramenta de borda e lê apenas a projeção do catálogo existente. |
| II. SQL Server como Persistência Oficial | PASS | O inventário é um artefato de importação, não uma persistência de negócio. |
| III. Contratos Verificáveis | PASS | A CLI possui contrato e testes de entrada, saída e falha. |
| IV. PT-BR | PASS | Mensagens, inventário e documentação operacional usam PT-BR. |
| V. Simplicidade Proporcional | PASS | Sem servidor, credencial, rede ou dependência externa. |

## Project Structure

```text
tools/DarkestDungeon.MediaCollector/
├── Program.cs
├── Catalogo/
├── Descoberta/
├── Spine/
├── Importacao/
└── Inventario/

tests/DarkestDungeon.Api.Tests/ColetaMidias/
├── Fixtures/Spine/
└── ImportadorDeMidiasTests.cs

assets/herois/
├── arquivos/
├── manifesto-habilidades.json
└── inventario.json
```

**Structure Decision**: Reaproveitar o executável local existente, removendo a camada HTTP/HTML. A ferramenta preserva os arquivos originais em vez de reconstruir sprites ou depender de mídia externa.

## Post-Design Constitution Check

Todos os gates permanecem em PASS: a solução não adiciona endpoint, persistência de negócio, credencial ou dependência externa.
