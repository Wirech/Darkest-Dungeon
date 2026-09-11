using DarkestDungeon.WikiCatalogCollector;
using FluentAssertions;

namespace DarkestDungeon.Api.Tests.Feature013;

public sealed class ParserTrinketsWikiTests
{
    [Fact]
    public void Parser_deve_ler_raridade_classe_e_efeitos_positivos_e_negativos()
    {
        const string wikitext = """
            | name = Sniper's Ring
            | rarity = Rare
            | class = Crusader
            | effect = +8 ACC<br>+3% CRIT<br>-10% MAX HP
            """;

        var resultado = ParserDeTrinkets.Parsear(wikitext, "https://darkestdungeon.wiki.gg/wiki/Sniper%27s_Ring");

        resultado.Snapshot.Should().NotBeNull();
        resultado.Snapshot!.Raridade.Should().Be("Rara");
        resultado.Snapshot.ClasseExclusiva.Should().Be("Cruzado");
        resultado.Snapshot.Efeitos.Should().Contain(e => e.Nome == "ACC" && e.Sinal == "Positivo" && e.Unidade == "Pontos");
        resultado.Snapshot.Efeitos.Should().Contain(e => e.Nome == "CRIT" && e.Sinal == "Positivo");
        resultado.Snapshot.Efeitos.Should().Contain(e => e.Nome == "MAX HP" && e.Sinal == "Negativo");
    }

    [Fact]
    public void Parser_deve_aceitar_pagina_dlc_crimson_court()
    {
        const string wikitext = """
            | name = Bloodied Fetish
            | rarity = Crimson Court
            | class = Occultist
            | effect = +15% DMG
            """;

        var resultado = ParserDeTrinkets.Parsear(wikitext, "https://darkestdungeon.wiki.gg/wiki/Bloodied_Fetish");

        resultado.Snapshot.Should().NotBeNull();
        resultado.Snapshot!.Raridade.Should().Be("CrimsonCourt");
        resultado.Snapshot.ClasseExclusiva.Should().Be("Ocultista");
    }

    [Fact]
    public void Parser_pagina_incompleta_gera_lacuna_sem_inventar_raridade()
    {
        const string wikitext = """
            | name = Mystery Charm
            | effect = +5 ACC
            """;

        var resultado = ParserDeTrinkets.Parsear(wikitext, "https://darkestdungeon.wiki.gg/wiki/Mystery_Charm");

        resultado.Snapshot.Should().NotBeNull();
        resultado.Snapshot!.Raridade.Should().BeNull();
        resultado.Lacunas.Should().Contain(l => l.Campo == "raridade");
        resultado.Lacunas.Should().NotContain(l => l.Motivo.Contains("Comum") && l.Campo != "raridade");
    }

    [Fact]
    public void Parser_nao_inventa_valor_quando_efeito_e_ilegivel()
    {
        const string wikitext = """
            | name = Broken Charm
            | rarity = Common
            | effect = lots of power maybe
            """;

        var resultado = ParserDeTrinkets.Parsear(wikitext, "https://example.test/broken");

        resultado.Snapshot.Should().NotBeNull();
        resultado.Snapshot!.Efeitos.Should().BeEmpty();
        resultado.Lacunas.Should().Contain(l => l.Campo == "efeitos");
    }
}
