using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Application.Itens;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Itens;

namespace DarkestDungeon.Api.Tests.Feature013;

internal static class Feature013Helpers
{
    public static CriarPersonagemRequest Personagem(
        ClasseDeHeroi classe = ClasseDeHeroi.Cruzado,
        int hp = 30,
        decimal protecao = 0m) =>
        new(
            Nome: $"Herói 013 {Guid.NewGuid():N}",
            Classe: classe,
            HpMaximo: hp,
            HpAtual: hp,
            Velocidade: 4,
            Critico: 5m,
            DanoBaseMinimo: 6,
            DanoBaseMaximo: 10,
            Movimento: 2,
            BonusDeCritico: 3m,
            Tamanho: 1,
            AcoesPorTurno: 1,
            Esquiva: 10m,
            Precisao: 5m,
            Protecao: protecao,
            Nivel: 0,
            Stress: 0,
            ChanceDeVirtude: 25,
            HabilidadesEquipadas: null);

    public static CriarAcessorioRequest Acessorio(
        string nome,
        ClasseDeHeroi? exclusiva = null,
        params EfeitoDeAcessorioRequest[] efeitos) =>
        new(
            NomeExibicao: nome,
            NomeOriginal: $"{nome}-{Guid.NewGuid():N}",
            Descricao: "Feature 013",
            Raridade: RaridadeDeAcessorio.Comum,
            ClasseExclusiva: exclusiva,
            ConjuntoId: null,
            Efeitos: efeitos);

    public static EfeitoDeAcessorioRequest Efeito(
        string nome,
        decimal valor,
        UnidadeDeEfeitoDeAcessorio unidade = UnidadeDeEfeitoDeAcessorio.Percentual,
        SinalDeEfeito sinal = SinalDeEfeito.Positivo) =>
        new(nome, valor, unidade, sinal);

    public static async Task<PersonagemDetalheDto> CriarPersonagemAsync(
        HttpClient client,
        CriarPersonagemRequest? request = null)
    {
        var response = await client.PostAsJsonAsync("/personagens", request ?? Personagem());
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<PersonagemDetalheDto>())!;
    }

    public static async Task<ItemDetalheDto> CriarAcessorioAsync(
        HttpClient client,
        CriarAcessorioRequest request)
    {
        var response = await client.PostAsJsonAsync("/acessorios", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ItemDetalheDto>())!;
    }

    public static async Task<PersonagemDetalheDto> EquiparEspacoAsync(
        HttpClient client,
        Guid personagemId,
        int espaco,
        Guid? acessorioId)
    {
        var response = await client.PutAsJsonAsync(
            $"/personagens/{personagemId}/acessorios/{espaco}",
            new EquiparAcessorioNoEspacoRequest(acessorioId));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<PersonagemDetalheDto>())!;
    }
}
