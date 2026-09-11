using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Contracts.Catalogo;
using DarkestDungeon.Application.Personagens;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Personagens;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature012;

internal static class PersonagemCardTestHelper
{
    internal static async Task<PersonagemDetalheDto> CriarCruzado(HttpClient client, AparenciaDePersonagem aparencia)
    {
        var response = await client.PostAsJsonAsync("/personagens", new CriarPersonagemRequest(
            Nome: $"Corpo {aparencia} {Guid.NewGuid():N}",
            Classe: ClasseDeHeroi.Cruzado,
            HpMaximo: 33, HpAtual: 30, Velocidade: 1, Critico: 3,
            DanoBaseMinimo: 6, DanoBaseMaximo: 12, Movimento: 2, BonusDeCritico: 0,
            Tamanho: 1, AcoesPorTurno: 1, Esquiva: 5, Precisao: 0, Protecao: 0, Nivel: 0,
            Stress: 12, ChanceDeVirtude: 25,
            Aparencia: aparencia));
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var detalhe = await response.Content.ReadFromJsonAsync<PersonagemDetalheDto>();
        detalhe.Should().NotBeNull();
        return detalhe!;
    }
}
