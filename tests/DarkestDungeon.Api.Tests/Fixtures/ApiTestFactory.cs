using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.MsSql;

namespace DarkestDungeon.Api.Tests.Fixtures;

public class ApiTestFactory : WebApplicationFactory<Program>
{
    private readonly string databaseName = $"DarkestDungeonTesting-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var sqlServerConnectionString = GetSqlServerConnectionString();
        if (sqlServerConnectionString is null)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Testing:DatabaseName"] = databaseName
                });
            });
            return;
        }

        builder.UseEnvironment("TestingSqlServer");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DarkestDungeonDb"] = sqlServerConnectionString
            });
        });
    }

    protected virtual string? GetSqlServerConnectionString() => null;
}

public sealed class SqlServerApiTestFactory : ApiTestFactory, IAsyncLifetime
{
    private readonly MsSqlContainer container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public Task InitializeAsync()
    {
        return container.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await container.DisposeAsync();
        await base.DisposeAsync();
    }

    protected override string? GetSqlServerConnectionString() => container.GetConnectionString();
}