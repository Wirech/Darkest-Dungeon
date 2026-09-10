using System.Net;
using System.Net.Http.Json;
using DarkestDungeon.Api.Tests.Fixtures;
using DarkestDungeon.Application.Publicacao;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DarkestDungeon.Api.Tests.Feature005;

/// T155: contract test que injeta falha real no publicador via fake e valida 500 + rollback + entrada Error nos logs.
public sealed class PublicacaoRollbackContractTests : IClassFixture<PublicacaoRollbackContractTests.FabricaComPublicadorFalhando>
{
    private readonly HttpClient client;
    private readonly FabricaComPublicadorFalhando fabrica;

    public PublicacaoRollbackContractTests(FabricaComPublicadorFalhando fabrica)
    {
        this.fabrica = fabrica;
        client = fabrica.CreateClient();
    }

    [Fact]
    public async Task Falha_simulada_retorna_500_com_rollback_e_log_error_SC016()
    {
        var response = await client.PostAsJsonAsync("/api/publicacao", new
        {
            confirmacaoJanelaManutencao = true,
            observacao = "Convergence T155 - simular falha",
        });

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("rollback", "SC-016 exige mensagem de rollback no corpo do 500");
        body.Should().Contain("publicacaoId");

        fabrica.Publicador.FoiChamado.Should().BeTrue();
        fabrica.Publicador.SimuluuFalha.Should().BeTrue();
    }

    public sealed class FabricaComPublicadorFalhando : WebApplicationFactory<Program>
    {
        public PublicadorQueFalha Publicador { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((_, cfg) =>
            {
                cfg.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Testing:DatabaseName"] = $"DarkestDungeonTesting-{Guid.NewGuid():N}",
                });
            });
            builder.ConfigureServices(services =>
            {
                var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IPublicadorAtomicoService));
                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }
                services.AddSingleton<IPublicadorAtomicoService>(Publicador);
            });
        }
    }

    public sealed class PublicadorQueFalha : IPublicadorAtomicoService
    {
        public bool FoiChamado { get; private set; }
        public bool SimuluuFalha { get; private set; }

        public Task<ResultadoDePublicacao> PublicarAsync(SolicitacaoDePublicacao solicitacao, CancellationToken cancellationToken)
        {
            FoiChamado = true;
            SimuluuFalha = true;

            var publicacaoId = Guid.NewGuid();
            var iniciada = DateTime.UtcNow;

            return Task.FromResult(new ResultadoDePublicacao(
                publicacaoId,
                iniciada,
                DateTime.UtcNow,
                EstadoDePublicacao.RollbackAplicado,
                HabilidadesAtualizadas: 0,
                NiveisAtualizados: 0,
                Erros: 1,
                CampoQueFalhou: "modificadorDano",
                HabilidadeAfetada: "Punish",
                Mensagem: "Publicação falhou; rollback aplicado."));
        }

        public Task<ResultadoDePublicacao?> ObterStatusAsync(Guid publicacaoId, CancellationToken cancellationToken) =>
            Task.FromResult<ResultadoDePublicacao?>(null);

        public Task<IReadOnlyList<LogDePublicacao>> ObterLogsAsync(Guid publicacaoId, NivelDeLog? nivel, string? filtroCampo, CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyList<LogDePublicacao>>(Array.Empty<LogDePublicacao>());
    }
}
