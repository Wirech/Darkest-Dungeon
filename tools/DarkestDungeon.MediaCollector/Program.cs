using DarkestDungeon.MediaCollector.Catalogo;
using DarkestDungeon.MediaCollector.Configuracao;
using DarkestDungeon.MediaCollector.Importacao;
using DarkestDungeon.MediaCollector.Inventario;

if (!OpcoesDoColetor.TentarCriar(args, out var opcoes, out var erro))
{
	Console.Error.WriteLine(erro);
	return 1;
}

if (opcoes!.ModoCamping)
{
	var resultadoCamping = await new ImportadorDeCamping(opcoes).ExecutarAsync(CancellationToken.None);
	Console.WriteLine($"{resultadoCamping.Arquivos.Count} arquivos de acampamento inventariados; {resultadoCamping.Lacunas.Count} lacunas.");
	return 0;
}

if (opcoes.ModoEquipamentos)
{
	var resultadoEquipamentos = await new ImportadorDeEquipamentos(opcoes).ExecutarAsync(CancellationToken.None);
	Console.WriteLine($"{resultadoEquipamentos.Arquivos.Count} arquivos inventariados; {resultadoEquipamentos.Lacunas.Count} lacunas.");
	return 0;
}

var catalogo = CatalogoDeHerois.ObterTodos();
var herois = opcoes!.Classes.Count == 0
	? catalogo
	: catalogo.Where(heroi => opcoes.Classes.Contains(heroi.NomeOriginal, StringComparer.OrdinalIgnoreCase)).ToArray();

if (opcoes.Classes.Count > 0 && herois.Count != opcoes.Classes.Count)
{
	Console.Error.WriteLine("Uma ou mais classes informadas não pertencem ao catálogo oficial.");
	return 1;
}

if (opcoes.GerarManifesto)
{
	return await GeradorDoManifesto.GerarAsync(opcoes, herois, CancellationToken.None);
}

var resultado = await new ImportadorLocal(opcoes).ExecutarAsync(herois, CancellationToken.None);

foreach (var classe in herois)
{
	var arquivos = resultado.Arquivos.Count(item => item.Classe == classe.NomeExibicao);
	var lacunas = resultado.Lacunas.Count(item => item.Classe == classe.NomeExibicao);
	Console.WriteLine($"{classe.NomeExibicao}: {arquivos} arquivos importados; {lacunas} lacunas.");
}

return 0;