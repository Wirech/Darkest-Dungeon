using DarkestDungeon.Application.Midias;
using DarkestDungeon.Domain.Itens;
using Microsoft.AspNetCore.Mvc;

namespace DarkestDungeon.Api.Controllers.Midias;

[ApiController]
[Route("api/midias/cobertura")]
public sealed class MidiasCoberturaController : ControllerBase
{
    private readonly ICoberturaDeMidiasService cobertura;

    public MidiasCoberturaController(ICoberturaDeMidiasService cobertura)
    {
        this.cobertura = cobertura;
    }

    [HttpGet]
    public async Task<IActionResult> ObterAsync([FromQuery] string? categoria, CancellationToken cancellationToken)
    {
        if (categoria is not null && !CategoriaDeMidiaParser.TentarAnalisar(categoria, out _))
        {
            return BadRequest(new { titulo = "Categoria inválida", mensagem = "Informe arma, armadura, acessorio, acampamento ou consumivel." });
        }

        CategoriaDeMidiaDeItem? filtro = null;
        if (categoria is not null)
        {
            CategoriaDeMidiaParser.TentarAnalisar(categoria, out var parsed);
            filtro = parsed;
        }

        var relatorio = await cobertura.GerarAsync(filtro, cancellationToken);
        return Ok(Mapear(relatorio));
    }

    [HttpGet("categorias/{categoria}")]
    public async Task<IActionResult> ObterCategoriaAsync(string categoria, CancellationToken cancellationToken)
    {
        if (!CategoriaDeMidiaParser.TentarAnalisar(categoria, out var parsed))
        {
            return NotFound(new { titulo = "Categoria inválida", mensagem = "Informe arma, armadura, acessorio, acampamento ou consumivel." });
        }

        var relatorio = await cobertura.GerarAsync(parsed, cancellationToken);
        return Ok(Mapear(relatorio));
    }

    private static object Mapear(RelatorioDeCoberturaDeMidias relatorio) => new
    {
        geradoEm = relatorio.GeradoEm,
        categorias = relatorio.Categorias.Select(c => new
        {
            categoria = CategoriaDeMidiaParser.NomeDeApi(c.Categoria),
            esperados = c.Esperados,
            ok = c.Ok,
            parcial = c.Parcial,
            pendente = c.Pendente,
            linhas = c.Linhas.Select(l => new
            {
                itemId = l.ItemId,
                nomeExibicao = l.NomeExibicao,
                nomeOriginal = l.NomeOriginal,
                status = l.Status.ToString(),
                niveisOk = l.NiveisOk,
                niveisPendentes = l.NiveisPendentes,
            }),
        }),
        orfaos = relatorio.Orfaos.Select(o => new
        {
            caminhoOrigem = o.CaminhoOrigem,
            sha256 = o.Sha256,
            caminhoDestino = o.CaminhoDestino,
            motivo = "Arquivo fora das origens mapeadas; permanece no inventário sem categoria de catálogo.",
        }),
        lacunas = relatorio.Lacunas.Select(l => new
        {
            categoria = l.Categoria,
            caminhoConsultado = l.CaminhoConsultado,
            motivo = l.Motivo,
            tentadoEm = l.TentadoEmUtc,
        }),
    };
}
