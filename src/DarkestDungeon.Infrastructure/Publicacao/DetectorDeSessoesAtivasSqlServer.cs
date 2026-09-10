using DarkestDungeon.Application.Publicacao;
using DarkestDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DarkestDungeon.Infrastructure.Publicacao;

/// Consulta `sys.dm_exec_sessions` filtrando sessões da API que não são a própria conexão do publicador (research R3).
public sealed class DetectorDeSessoesAtivasSqlServer : IDetectorDeSessoesAtivas
{
    private const string SqlContagem =
        "SELECT COUNT(*) FROM sys.dm_exec_sessions WHERE program_name LIKE 'DarkestDungeon.Api%' AND session_id <> @@SPID";

    private readonly DarkestDungeonDbContext contexto;

    public DetectorDeSessoesAtivasSqlServer(DarkestDungeonDbContext contexto)
    {
        this.contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
    }

    public async Task<int> ContarSessoesAtivasAsync(CancellationToken cancellationToken)
    {
        if (!contexto.Database.IsSqlServer())
        {
            // In-memory ou provider sem DMV — publicação livre em ambiente de teste.
            return 0;
        }

        // Abre uma conexão nova a partir da mesma connection string do DbContext — não usa a conexão compartilhada
        // (fechar a conexão compartilhada quebra `BeginTransactionAsync` subsequente do próprio DbContext).
        var connectionString = contexto.Database.GetConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return 0;
        }

        await using var connection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var comando = connection.CreateCommand();
        comando.CommandText = SqlContagem;
        comando.CommandTimeout = 15;
        var resultado = await comando.ExecuteScalarAsync(cancellationToken);
        return resultado is int i ? i : Convert.ToInt32(resultado);
    }
}
