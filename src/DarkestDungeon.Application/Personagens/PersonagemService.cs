using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Application.Habilidades;
using DarkestDungeon.Application.Midias;
using DarkestDungeon.Application.Personagens.Commands;
using DarkestDungeon.Application.Validation;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Habilidades;
using DarkestDungeon.Domain.Itens;
using DarkestDungeon.Domain.Personagens;
using DarkestDungeon.Domain.Seres;

namespace DarkestDungeon.Application.Personagens;

public sealed class PersonagemService : IPersonagemService
{
    private readonly IPersonagemRepository personagens;
    private readonly IClasseRepository classes;
    private readonly IHabilidadeRepository habilidades;
    private readonly IItemRepository itens;
    private readonly IResolvedorDeMidiasDoCard resolvedorDeMidias;

    public PersonagemService(
        IPersonagemRepository personagens,
        IClasseRepository classes,
        IHabilidadeRepository habilidades,
        IItemRepository itens,
        IResolvedorDeMidiasDoCard resolvedorDeMidias)
    {
        this.personagens = personagens;
        this.classes = classes;
        this.habilidades = habilidades;
        this.itens = itens;
        this.resolvedorDeMidias = resolvedorDeMidias;
    }

    public async Task<ResultadoOperacao<IReadOnlyCollection<PersonagemResumoDto>>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var personagensCadastrados = await personagens.ListarAsync(cancellationToken).ConfigureAwait(false);
        var classesCatalogo = await classes.ListarAsync(cancellationToken).ConfigureAwait(false);
        var habilidadesCatalogo = await habilidades.ListarAsync(null, null, cancellationToken).ConfigureAwait(false);
        var itensCatalogo = await itens.ListarTodosAsync(cancellationToken).ConfigureAwait(false);
        var classesPorEnum = classesCatalogo.ToDictionary(c => c.ClasseDeHeroi);
        var habilidadesPorId = habilidadesCatalogo.ToDictionary(h => h.Id);
        var itensPorId = itensCatalogo.ToDictionary(i => i.Id);

        var resumo = personagensCadastrados
            .Select(personagem =>
            {
                classesPorEnum.TryGetValue(personagem.Classe, out var classe);
                return PersonagemMapper.ParaResumo(personagem, classe, habilidadesPorId, resolvedorDeMidias, itensPorId);
            })
            .ToArray();

        return ResultadoOperacao<IReadOnlyCollection<PersonagemResumoDto>>.Ok(resumo);
    }

    public async Task<ResultadoOperacao<PersonagemDetalheDto>> ObterAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var personagem = await personagens.ObterPorIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (personagem is null)
        {
            return ResultadoOperacao<PersonagemDetalheDto>.NaoEncontrado($"Personagem '{id}' não encontrado.");
        }

        return ResultadoOperacao<PersonagemDetalheDto>.Ok(PersonagemMapper.ParaDto(
            personagem,
            await CarregarItensParaCardAsync(personagem, cancellationToken),
            await CarregarHabilidadesAsync(cancellationToken),
            await classes.ObterPorEnumAsync(personagem.Classe, cancellationToken).ConfigureAwait(false),
            resolvedorDeMidias));
    }

    public async Task<ResultadoOperacao<PersonagemDetalheDto>> CriarAsync(CriarPersonagemCommand command, CancellationToken cancellationToken = default)
    {
        var classe = await classes.ObterPorEnumAsync(command.Classe, cancellationToken).ConfigureAwait(false);
        if (classe is null)
        {
            return ResultadoOperacao<PersonagemDetalheDto>.Invalido(
                $"Classe '{command.Classe}' não encontrada.",
                new ErroOperacao("classe", "Classe não encontrada."));
        }

        var habilidadesDaClasse = await habilidades.ListarPorClasseAsync(classe.Id, cancellationToken).ConfigureAwait(false);
        var modoCompleto = command.NivelDaArma.HasValue || command.NivelDaArmadura.HasValue;
        if (modoCompleto && (!command.NivelDaArma.HasValue || !command.NivelDaArmadura.HasValue))
        {
            return ResultadoOperacao<PersonagemDetalheDto>.Invalido(
                "Nível da arma e da armadura devem ser informados juntos.",
                new ErroOperacao("equipamento", "Informe os níveis da arma e da armadura."));
        }

        Arma? armaSelecionada = null;
        Armadura? armaduraSelecionada = null;
        DerivacaoDeAtributosOficiais.AtributosDerivados? derivados = null;
        if (modoCompleto)
        {
            if (command.NivelDaArma is < 1 or > 5 || command.NivelDaArmadura is < 1 or > 5)
            {
                return ResultadoOperacao<PersonagemDetalheDto>.Invalido(
                    "Níveis de equipamento inválidos.",
                    new ErroOperacao("equipamento", "Os níveis de arma e armadura devem estar entre 1 e 5."));
            }

            var itensCatalogo = await itens.ListarTodosAsync(cancellationToken).ConfigureAwait(false);
            var armas = itensCatalogo.OfType<Arma>().Where(item => item.ClasseElegivel == command.Classe).ToArray();
            var armaduras = itensCatalogo.OfType<Armadura>().Where(item => item.ClasseElegivel == command.Classe).ToArray();
            armaSelecionada = EscolherEquipamentoOficial(armas);
            armaduraSelecionada = EscolherEquipamentoOficial(armaduras);
            if (armaSelecionada is null || armaduraSelecionada is null)
            {
                return ResultadoOperacao<PersonagemDetalheDto>.TabelaOficialAusente(
                    $"Não foi possível localizar arma e armadura oficiais para a classe '{command.Classe}'.");
            }

            if (!DerivacaoDeAtributosOficiais.CatalogoOficialCompleto(classe, armaSelecionada, armaduraSelecionada))
            {
                return ResultadoOperacao<PersonagemDetalheDto>.CatalogoIncompleto(
                    $"O catálogo oficial está incompleto para a classe '{command.Classe}'.",
                    new ErroOperacao("catalogo", "Faltam tabela oficial de arma, armadura ou deslocamento."));
            }

            var nivelArma = armaSelecionada.Niveis.SingleOrDefault(nivel => nivel.Nivel == command.NivelDaArma);
            var nivelArmadura = armaduraSelecionada.Niveis.SingleOrDefault(nivel => nivel.Nivel == command.NivelDaArmadura);
            if (nivelArma is null || nivelArmadura is null)
            {
                return ResultadoOperacao<PersonagemDetalheDto>.TabelaOficialAusente(
                    "Nível de equipamento não catalogado na tabela oficial.");
            }

            derivados = DerivacaoDeAtributosOficiais.Derivar(classe, nivelArma, nivelArmadura, command.Nivel);
            command = command with
            {
                HpMaximo = derivados.HpMaximo,
                HpAtual = derivados.HpAtual,
                Velocidade = derivados.Velocidade,
                Critico = derivados.Critico,
                DanoBaseMinimo = derivados.DanoBaseMinimo,
                DanoBaseMaximo = derivados.DanoBaseMaximo,
                Esquiva = derivados.Esquiva,
                Movimento = 0,
                BonusDeCritico = derivados.BonusDeCritico,
                Tamanho = derivados.Tamanho,
                AcoesPorTurno = derivados.AcoesPorTurno,
                Precisao = derivados.Precisao,
                Protecao = derivados.Protecao,
                Stress = derivados.Stress,
                ChanceDeVirtude = derivados.ChanceDeVirtude
            };
        }

        var habilidadesEquipadas = (command.HabilidadesEquipadas ?? Array.Empty<Guid>()).Distinct().ToArray();
        var habilidadesIniciais = new HashSet<Guid>();
        if (modoCompleto)
        {
            var combate = habilidadesDaClasse.OfType<HabilidadeDeCombate>().ToArray();
            var acampamento = habilidadesDaClasse.OfType<HabilidadeDeAcampamento>().ToArray();
            if (combate.Length < 4 || acampamento.Length < 4)
            {
                return ResultadoOperacao<PersonagemDetalheDto>.Invalido(
                    "A classe não possui habilidades suficientes para a configuração inicial.",
                    new ErroOperacao("habilidades", "A classe precisa possuir pelo menos 4 habilidades de Combate e 4 de Acampamento."));
            }

            var combateIniciais = command.Classe is ClasseDeHeroi.Abominacao or ClasseDeHeroi.Duelista
                ? combate.AsEnumerable()
                : EscolherAleatorias(combate, 4);
            foreach (var habilidade in combateIniciais.Cast<Habilidade>().Concat(EscolherAleatorias(acampamento, 4)))
            {
                habilidadesIniciais.Add(habilidade.Id);
            }

            habilidadesEquipadas = habilidadesDaClasse
                .OfType<HabilidadeDeHeroi>()
                .OrderBy(habilidade => habilidade.NomeExibicao)
                .ThenBy(habilidade => habilidade.Id)
                .Select(habilidade => habilidade.Id)
                .ToArray();
        }
        else if (habilidadesEquipadas.Length > 0)
        {
            var validacao = await ValidarHabilidadesAsync(classe.Id, habilidadesEquipadas, cancellationToken).ConfigureAwait(false);
            if (validacao is { } erro)
            {
                return erro;
            }
        }

        try
        {
            var resistencias = derivados?.Resistencias ?? new Resistencias(
                classe.ResistenciasBase.Atordoamento,
                classe.ResistenciasBase.Sangramento,
                classe.ResistenciasBase.Envenenamento,
                classe.ResistenciasBase.Debuff,
                classe.ResistenciasBase.Movimento);
            var extras = derivados?.ResistenciasExtras ?? new ResistenciasExtrasDePersonagem(
                classe.ResistenciasBase.Doenca,
                classe.ResistenciasBase.GolpeMortal,
                classe.ResistenciasBase.Armadilha);

            var personagem = new Personagem(
                command.Nome,
                command.Classe,
                command.HpMaximo,
                command.HpAtual,
                command.Velocidade,
                command.Critico,
                command.DanoBaseMinimo,
                command.DanoBaseMaximo,
                command.Movimento,
                command.BonusDeCritico,
                command.Tamanho,
                command.AcoesPorTurno,
                command.Esquiva,
                command.Precisao,
                command.Protecao,
                command.Nivel,
                resistencias,
                extras,
                command.Stress,
                command.ChanceDeVirtude,
                command.NivelDaArma,
                command.NivelDaArmadura,
                command.Aparencia,
                passosAFrente: derivados?.PassosAFrente,
                passosAtras: derivados?.PassosAtras);

            foreach (var habilidadeId in habilidadesEquipadas)
            {
                var nivel = modoCompleto && habilidadesIniciais.Contains(habilidadeId)
                    ? 1
                    : 0;
                personagem.AdicionarHabilidade(new HabilidadeDePersonagem(habilidadeId, numeroDoNivel: nivel));
            }

            if (armaSelecionada is not null) personagem.EquiparArma(armaSelecionada.Id);
            if (armaduraSelecionada is not null) personagem.EquiparArmadura(armaduraSelecionada.Id);

            await personagens.AdicionarAsync(personagem, cancellationToken).ConfigureAwait(false);
            return ResultadoOperacao<PersonagemDetalheDto>.Ok(PersonagemMapper.ParaDto(
                personagem,
                await CarregarItensParaCardAsync(personagem, cancellationToken),
                await CarregarHabilidadesAsync(cancellationToken),
                classe,
                resolvedorDeMidias));
        }
        catch (ArgumentException ex)
        {
            return ResultadoOperacao<PersonagemDetalheDto>.Invalido(ex.Message, new ErroOperacao(ex.ParamName ?? "requisicao", ex.Message));
        }
        catch (Exception ex)
        {
            _ = ex;
            return ResultadoOperacao<PersonagemDetalheDto>.FalhaPersistencia("Não foi possível criar o Personagem.");
        }
    }

    public async Task<ResultadoOperacao<PersonagemDetalheDto>> EquiparAsync(EquiparPersonagemCommand command, CancellationToken cancellationToken = default)
    {
        var personagem = await personagens.ObterPorIdAsync(command.PersonagemId, cancellationToken).ConfigureAwait(false);
        if (personagem is null)
        {
            return ResultadoOperacao<PersonagemDetalheDto>.NaoEncontrado($"Personagem '{command.PersonagemId}' não encontrado.");
        }

        if (command.ArmaId is { } armaId)
        {
            var arma = await itens.ObterPorIdAsync(armaId, cancellationToken).ConfigureAwait(false) as DarkestDungeon.Domain.Itens.Arma;
            if (arma is null)
            {
                return ResultadoOperacao<PersonagemDetalheDto>.Invalido("Arma não encontrada.", new ErroOperacao("armaId", "Arma não encontrada."));
            }
            if (arma.ClasseElegivel != personagem.Classe)
            {
                return ResultadoOperacao<PersonagemDetalheDto>.Invalido(
                    "Arma não é elegível para a classe do Personagem.",
                    new ErroOperacao("armaId", "Arma pertence a outra classe."));
            }
            personagem.EquiparArma(armaId);
        }

        if (command.ArmaduraId is { } armaduraId)
        {
            var armadura = await itens.ObterPorIdAsync(armaduraId, cancellationToken).ConfigureAwait(false) as DarkestDungeon.Domain.Itens.Armadura;
            if (armadura is null)
            {
                return ResultadoOperacao<PersonagemDetalheDto>.Invalido("Armadura não encontrada.", new ErroOperacao("armaduraId", "Armadura não encontrada."));
            }
            if (armadura.ClasseElegivel != personagem.Classe)
            {
                return ResultadoOperacao<PersonagemDetalheDto>.Invalido(
                    "Armadura não é elegível para a classe do Personagem.",
                    new ErroOperacao("armaduraId", "Armadura pertence a outra classe."));
            }
            personagem.EquiparArmadura(armaduraId);
        }

        var acessorios = (command.AcessoriosIds ?? Array.Empty<Guid>()).Take(2).ToArray();
        foreach (var acId in acessorios)
        {
            var acessorio = await itens.ObterPorIdAsync(acId, cancellationToken).ConfigureAwait(false) as DarkestDungeon.Domain.Itens.Acessorio;
            if (acessorio is null)
            {
                return ResultadoOperacao<PersonagemDetalheDto>.Invalido("Acessório não encontrado.", new ErroOperacao("acessoriosIds", $"Acessório '{acId}' não encontrado."));
            }
            if (acessorio.ClasseExclusiva is { } exclusiva && exclusiva != personagem.Classe)
            {
                return ResultadoOperacao<PersonagemDetalheDto>.Invalido(
                    "Acessório é exclusivo de outra classe.",
                    new ErroOperacao("acessoriosIds", $"Acessório '{acId}' é exclusivo de outra classe."));
            }
        }

        try
        {
            personagem.EquiparAcessorios(
                acessorios.ElementAtOrDefault(0) is var a1 && a1 != Guid.Empty ? a1 : null,
                acessorios.ElementAtOrDefault(1) is var a2 && a2 != Guid.Empty ? a2 : null);

            await personagens.AtualizarAsync(personagem, cancellationToken).ConfigureAwait(false);
            return ResultadoOperacao<PersonagemDetalheDto>.Ok(PersonagemMapper.ParaDto(
                personagem,
                await CarregarItensParaCardAsync(personagem, cancellationToken),
                await CarregarHabilidadesAsync(cancellationToken),
                await classes.ObterPorEnumAsync(personagem.Classe, cancellationToken).ConfigureAwait(false),
                resolvedorDeMidias));
        }
        catch (ArgumentException ex)
        {
            return ResultadoOperacao<PersonagemDetalheDto>.Invalido(ex.Message, new ErroOperacao(ex.ParamName ?? "requisicao", ex.Message));
        }
    }

    public async Task<ResultadoOperacao<bool>> RemoverAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var removido = await personagens.RemoverAsync(id, cancellationToken).ConfigureAwait(false);
        return removido
            ? ResultadoOperacao<bool>.Ok(true)
            : ResultadoOperacao<bool>.NaoEncontrado($"Personagem '{id}' não encontrado.");
    }

    private async Task<ResultadoOperacao<PersonagemDetalheDto>?> ValidarHabilidadesAsync(
        Guid classeId,
        IReadOnlyList<Guid> habilidadesIds,
        CancellationToken cancellationToken)
    {
        var habilidadesDaClasse = await habilidades.ListarPorClasseAsync(classeId, cancellationToken).ConfigureAwait(false);
        var idsPermitidos = habilidadesDaClasse.Select(h => h.Id).ToHashSet();

        int combate = 0, acampamento = 0;
        foreach (var id in habilidadesIds)
        {
            if (!idsPermitidos.Contains(id))
            {
                return ResultadoOperacao<PersonagemDetalheDto>.Invalido(
                    $"Habilidade '{id}' não pertence à Classe do Personagem.",
                    new ErroOperacao("habilidadesEquipadas", $"Habilidade '{id}' não pertence à Classe."));
            }

            var habilidade = habilidadesDaClasse.First(h => h.Id == id);
            if (habilidade is HabilidadeDeCombate)
            {
                combate++;
            }
            else if (habilidade is HabilidadeDeAcampamento)
            {
                acampamento++;
            }
        }

        if (combate > Personagem.LimiteHabilidadesDeCombate)
        {
            return ResultadoOperacao<PersonagemDetalheDto>.Invalido(
                $"Personagem não pode ter mais de {Personagem.LimiteHabilidadesDeCombate} habilidades de Combate.",
                new ErroOperacao("habilidadesEquipadas", $"Limite de {Personagem.LimiteHabilidadesDeCombate} habilidades de Combate ultrapassado."));
        }

        if (acampamento > Personagem.LimiteHabilidadesDeAcampamento)
        {
            return ResultadoOperacao<PersonagemDetalheDto>.Invalido(
                $"Personagem não pode ter mais de {Personagem.LimiteHabilidadesDeAcampamento} habilidades de Acampamento.",
                new ErroOperacao("habilidadesEquipadas", $"Limite de {Personagem.LimiteHabilidadesDeAcampamento} habilidades de Acampamento ultrapassado."));
        }

        return null;
    }

    private async Task<IReadOnlyDictionary<Guid, Item>> CarregarItensParaCardAsync(Personagem personagem, CancellationToken cancellationToken)
    {
        var todos = await itens.ListarTodosAsync(cancellationToken).ConfigureAwait(false);
        return todos
            .Where(item => item.Id == personagem.ArmaEquipadaId
                || item.Id == personagem.ArmaduraEquipadaId
                || item.Id == personagem.AcessorioEquipado1Id
                || item.Id == personagem.AcessorioEquipado2Id
                || (item is Arma arma && arma.ClasseElegivel == personagem.Classe)
                || (item is Armadura armadura && armadura.ClasseElegivel == personagem.Classe))
            .DistinctBy(item => item.Id)
            .ToDictionary(item => item.Id);
    }

    private async Task<IReadOnlyDictionary<Guid, Habilidade>> CarregarHabilidadesAsync(CancellationToken cancellationToken)
    {
        var lista = await habilidades.ListarAsync(null, null, cancellationToken).ConfigureAwait(false);
        return lista.ToDictionary(habilidade => habilidade.Id);
    }

    private static IReadOnlyList<T> EscolherAleatorias<T>(IReadOnlyList<T> origem, int quantidade) =>
        origem.OrderBy(_ => Random.Shared.Next()).Take(quantidade).ToArray();

    private static T? EscolherEquipamentoOficial<T>(IReadOnlyList<T> itens) where T : Item
    {
        if (itens.Count == 0)
        {
            return null;
        }

        var oficial = itens.FirstOrDefault(item =>
            item.NomeExibicao.StartsWith("Arma oficial", StringComparison.Ordinal)
            || item.NomeExibicao.StartsWith("Armadura oficial", StringComparison.Ordinal));
        return oficial ?? (itens.Count == 1 ? itens[0] : itens.OrderBy(item => item.Id).First());
    }
}
