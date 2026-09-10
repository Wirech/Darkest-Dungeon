using DarkestDungeon.Application.Abstractions;
using DarkestDungeon.Application.Habilidades;
using DarkestDungeon.Application.Personagens.Commands;
using DarkestDungeon.Application.Validation;
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

    public PersonagemService(
        IPersonagemRepository personagens,
        IClasseRepository classes,
        IHabilidadeRepository habilidades,
        IItemRepository itens)
    {
        this.personagens = personagens;
        this.classes = classes;
        this.habilidades = habilidades;
        this.itens = itens;
    }

    public async Task<ResultadoOperacao<PersonagemDetalheDto>> ObterAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var personagem = await personagens.ObterPorIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (personagem is null)
        {
            return ResultadoOperacao<PersonagemDetalheDto>.NaoEncontrado($"Personagem '{id}' não encontrado.");
        }

        return ResultadoOperacao<PersonagemDetalheDto>.Ok(PersonagemMapper.ParaDto(personagem, await CarregarItensAsync(personagem, cancellationToken)));
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

        var habilidadesEquipadas = (command.HabilidadesEquipadas ?? Array.Empty<Guid>()).Distinct().ToArray();
        if (habilidadesEquipadas.Length > 0)
        {
            var validacao = await ValidarHabilidadesAsync(classe.Id, habilidadesEquipadas, cancellationToken).ConfigureAwait(false);
            if (validacao is { } erro)
            {
                return erro;
            }
        }

        try
        {
            var resistencias = new Resistencias(
                classe.ResistenciasBase.Atordoamento,
                classe.ResistenciasBase.Sangramento,
                classe.ResistenciasBase.Envenenamento,
                classe.ResistenciasBase.Debuff,
                classe.ResistenciasBase.Movimento);
            var extras = new ResistenciasExtrasDePersonagem(
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
                command.ChanceDeVirtude);

            foreach (var habilidadeId in habilidadesEquipadas)
            {
                personagem.AdicionarHabilidade(new HabilidadeDePersonagem(habilidadeId));
            }

            await personagens.AdicionarAsync(personagem, cancellationToken).ConfigureAwait(false);
            return ResultadoOperacao<PersonagemDetalheDto>.Ok(PersonagemMapper.ParaDto(personagem, await CarregarItensAsync(personagem, cancellationToken)));
        }
        catch (ArgumentException ex)
        {
            return ResultadoOperacao<PersonagemDetalheDto>.Invalido(ex.Message, new ErroOperacao(ex.ParamName ?? "requisicao", ex.Message));
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
            return ResultadoOperacao<PersonagemDetalheDto>.Ok(PersonagemMapper.ParaDto(personagem, await CarregarItensAsync(personagem, cancellationToken)));
        }
        catch (ArgumentException ex)
        {
            return ResultadoOperacao<PersonagemDetalheDto>.Invalido(ex.Message, new ErroOperacao(ex.ParamName ?? "requisicao", ex.Message));
        }
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

    private async Task<IReadOnlyDictionary<Guid, Item>> CarregarItensAsync(Personagem personagem, CancellationToken cancellationToken)
    {
        var ids = new[] { personagem.ArmaEquipadaId, personagem.ArmaduraEquipadaId, personagem.AcessorioEquipado1Id, personagem.AcessorioEquipado2Id }
            .Where(id => id.HasValue)
            .Select(id => id!.Value);
        var lista = await itens.ListarPorIdsAsync(ids, cancellationToken).ConfigureAwait(false);
        return lista.ToDictionary(i => i.Id);
    }
}
