using DarkestDungeon.Application.Auditoria;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DarkestDungeon.Api.Auditoria;

/// CLI runner da Feature 005 — gera `relatorio.md` fora do fluxo HTTP.
/// Uso: `dotnet run --project src/DarkestDungeon.Api -- auditoria [--classe=<slug>|todas]`.
public static class AuditoriaJobRunner
{
    public static async Task<int> ExecutarAsync(string[] args, IServiceProvider services)
    {
        var argumentos = ParseArgumentos(args);
        var filtroClasse = ResolverClasse(argumentos.Classe);

        using var scope = services.CreateScope();
        var servico = scope.ServiceProvider.GetRequiredService<IAuditoriaWikiService>();

        Console.WriteLine($"[Auditoria] Gerando relatório (classe={argumentos.Classe ?? "todas"})...");
        var relatorio = await servico.GerarRelatorioAsync(filtroClasse, CancellationToken.None);

        var caminho = argumentos.Saida ?? ResolverCaminhoPadrao();

        await servico.SalvarComoMarkdownAsync(relatorio, caminho, CancellationToken.None);

        Console.WriteLine($"[Auditoria] Relatório salvo em: {caminho}");
        Console.WriteLine($"[Auditoria] Total: {relatorio.Resumo.TotalHabilidades}  OK: {relatorio.Resumo.Ok}  Parcial: {relatorio.Resumo.Parcial}  Faltando: {relatorio.Resumo.Faltando}");
        return relatorio.Resumo.Faltando == 0 && relatorio.Resumo.Parcial == 0 ? 0 : 1;
    }

    private static string ResolverCaminhoPadrao()
    {
        var atual = AppContext.BaseDirectory;
        for (int nivel = 0; nivel < 8; nivel++)
        {
            var candidato = Path.Combine(atual, "specs", "005-auditoria-habilidades-mineradas");
            if (Directory.Exists(candidato))
            {
                return Path.Combine(candidato, "relatorio.md");
            }
            var pai = Directory.GetParent(atual)?.FullName;
            if (pai is null)
            {
                break;
            }
            atual = pai;
        }
        return Path.Combine(Directory.GetCurrentDirectory(), "relatorio.md");
    }

    public static bool DeveExecutar(string[] args) => args.Length > 0 && string.Equals(args[0], "auditoria", StringComparison.OrdinalIgnoreCase);

    private static ArgumentosCli ParseArgumentos(string[] args)
    {
        string? classe = null;
        string? saida = null;
        foreach (var a in args.Skip(1))
        {
            if (a.StartsWith("--classe=", StringComparison.OrdinalIgnoreCase))
            {
                classe = a.Substring("--classe=".Length);
            }
            else if (a.StartsWith("--saida=", StringComparison.OrdinalIgnoreCase))
            {
                saida = a.Substring("--saida=".Length);
            }
        }
        return new ArgumentosCli(classe, saida);
    }

    private static ClasseDeHeroi? ResolverClasse(string? nome)
    {
        if (string.IsNullOrWhiteSpace(nome) || string.Equals(nome, "todas", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }
        foreach (var valor in Enum.GetValues<ClasseDeHeroi>())
        {
            if (string.Equals(valor.ToString(), nome, StringComparison.OrdinalIgnoreCase))
            {
                return valor;
            }
        }
        throw new ArgumentException($"Classe '{nome}' não reconhecida. Use um dos valores de ClasseDeHeroi ou 'todas'.");
    }

    private sealed record ArgumentosCli(string? Classe, string? Saida);
}
