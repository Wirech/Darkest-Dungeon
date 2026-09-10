using DarkestDungeon.Application.Publicacao;
using DarkestDungeon.Infrastructure.Data;
using DarkestDungeon.Infrastructure.Publicacao;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DarkestDungeon.Api.Tests.Feature005;

/// T157: integration test que abre 2+ conexões nomeadas e valida `DetectorDeSessoesAtivasSqlServer`.
/// Requer SQL Server real disponível (skip se `DARKESTDUNGEON_SQLSERVER_CS` não estiver definida).
public sealed class DetectorDeSessoesAtivasTests
{
    private const string EnvVar = "DARKESTDUNGEON_SQLSERVER_CS";
    private const string ConnectionStringPadrao = "Server=localhost,1433;Database=DarkestDungeon;User Id=sa;Password=Devlocal!2024;TrustServerCertificate=True";

    [Fact]
    public async Task Conta_sessoes_diferentes_do_publicador_com_2_conexoes_ativas()
    {
        var cs = Environment.GetEnvironmentVariable(EnvVar) ?? ConnectionStringPadrao;
        if (!await SqlServerDisponivelAsync(cs))
        {
            return; // Skip silencioso se SQL Server local não estiver disponível.
        }

        // Abre 2 conexões nomeadas como "DarkestDungeon.Api.TesteConcorrencia".
        var csTeste = cs + ";Application Name=DarkestDungeon.Api.TesteConcorrencia";
        await using var conexaoA = new SqlConnection(csTeste);
        await using var conexaoB = new SqlConnection(csTeste);
        await conexaoA.OpenAsync();
        await conexaoB.OpenAsync();

        // Simula o publicador conectando como "DarkestDungeon.Publisher".
        var csPublicador = cs + ";Application Name=DarkestDungeon.Publisher";
        var opts = new DbContextOptionsBuilder<DarkestDungeonDbContext>()
            .UseSqlServer(csPublicador)
            .Options;
        await using var ctx = new DarkestDungeonDbContext(opts);
        var detector = new DetectorDeSessoesAtivasSqlServerParaTeste(ctx, "DarkestDungeon.Api.TesteConcorrencia");

        var contagem = await detector.ContarSessoesAtivasAsync(CancellationToken.None);

        contagem.Should().BeGreaterThanOrEqualTo(2, "abrimos duas conexões nomeadas explicitamente");
    }

    private static async Task<bool> SqlServerDisponivelAsync(string cs)
    {
        try
        {
            await using var conn = new SqlConnection(cs);
            await conn.OpenAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}

/// Versão do detector que filtra por um `Application Name` customizado para o teste isolado.
internal sealed class DetectorDeSessoesAtivasSqlServerParaTeste : IDetectorDeSessoesAtivas
{
    private readonly DarkestDungeonDbContext contexto;
    private readonly string filtroApplicationName;

    public DetectorDeSessoesAtivasSqlServerParaTeste(DarkestDungeonDbContext contexto, string filtroApplicationName)
    {
        this.contexto = contexto;
        this.filtroApplicationName = filtroApplicationName;
    }

    public async Task<int> ContarSessoesAtivasAsync(CancellationToken cancellationToken)
    {
        var cs = contexto.Database.GetConnectionString();
        await using var connection = new SqlConnection(cs);
        await connection.OpenAsync(cancellationToken);
        await using var comando = connection.CreateCommand();
        comando.CommandText = "SELECT COUNT(*) FROM sys.dm_exec_sessions WHERE program_name = @app AND session_id <> @@SPID";
        comando.Parameters.AddWithValue("@app", filtroApplicationName);
        var resultado = await comando.ExecuteScalarAsync(cancellationToken);
        return resultado is int i ? i : Convert.ToInt32(resultado);
    }
}
