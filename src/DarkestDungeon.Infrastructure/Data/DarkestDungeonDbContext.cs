using DarkestDungeon.Application.Publicacao;
using DarkestDungeon.Domain.Classes;
using DarkestDungeon.Domain.Cobertura;
using DarkestDungeon.Domain.Habilidades;
using DarkestDungeon.Domain.Itens;
using DarkestDungeon.Domain.Personagens;
using DarkestDungeon.Domain.Seres;
using Microsoft.EntityFrameworkCore;

namespace DarkestDungeon.Infrastructure.Data;

public sealed class DarkestDungeonDbContext : DbContext
{
    public DarkestDungeonDbContext(DbContextOptions<DarkestDungeonDbContext> options)
        : base(options)
    {
    }

    public DbSet<Ser> Seres => Set<Ser>();
    public DbSet<Classe> Classes => Set<Classe>();
    public DbSet<Habilidade> Habilidades => Set<Habilidade>();
    public DbSet<ClasseHabilidade> ClassesHabilidades => Set<ClasseHabilidade>();
    public DbSet<Item> Itens => Set<Item>();
    public DbSet<PublicacaoDeVinculosDeMidia> PublicacoesDeVinculosDeMidia => Set<PublicacaoDeVinculosDeMidia>();
    public DbSet<EntradaDoMapaDeCobertura> MapaDeCobertura => Set<EntradaDoMapaDeCobertura>();
    public DbSet<LogDePublicacao> LogsDePublicacao => Set<LogDePublicacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        MapearSer(modelBuilder);
        MapearPersonagem(modelBuilder);
        MapearInimigo(modelBuilder);
        MapearClasse(modelBuilder);
        MapearHabilidade(modelBuilder);
        MapearClasseHabilidade(modelBuilder);
        MapearItem(modelBuilder);
        MapearPublicacaoDeVinculosDeMidia(modelBuilder);
        MapearMapaDeCobertura(modelBuilder);
        MapearLogDePublicacao(modelBuilder);
    }

    private static void MapearSer(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ser>(entity =>
        {
            entity.UseTptMappingStrategy();
            entity.ToTable("Seres", table =>
            {
                table.HasCheckConstraint("CK_Seres_HP", "[HpMaximo] >= 0 AND [HpAtual] >= 0 AND [HpAtual] <= [HpMaximo]");
                table.HasCheckConstraint("CK_Seres_DanoBase", "[DanoBaseMinimo] >= 0 AND [DanoBaseMaximo] >= 0 AND [DanoBaseMinimo] <= [DanoBaseMaximo]");
                table.HasCheckConstraint("CK_Seres_Percentuais", "[Critico] >= 0 AND [Critico] <= 100 AND [BonusDeCritico] >= 0 AND [BonusDeCritico] <= 100 AND [Protecao] >= 0 AND [Protecao] <= 100");
                table.HasCheckConstraint("CK_Seres_Tamanho", "[Tamanho] >= 0 AND [Tamanho] <= 4");
                table.HasCheckConstraint("CK_Seres_Nivel", "[Nivel] >= 0 AND [Nivel] <= 6");
                table.HasCheckConstraint("CK_Seres_Resistencias", "[Resistencias_Atordoamento] >= 0 AND [Resistencias_Atordoamento] <= 100 AND [Resistencias_Sangramento] >= 0 AND [Resistencias_Sangramento] <= 100 AND [Resistencias_Envenenamento] >= 0 AND [Resistencias_Envenenamento] <= 100 AND [Resistencias_Debuff] >= 0 AND [Resistencias_Debuff] <= 100 AND [Resistencias_Movimento] >= 0 AND [Resistencias_Movimento] <= 100");
            });
            entity.HasKey(ser => ser.Id);
            entity.Property(ser => ser.Id).ValueGeneratedNever();
            entity.Property(ser => ser.Nome).IsRequired().HasMaxLength(120);
            entity.Property(ser => ser.Tipo).IsRequired().HasMaxLength(80);
            entity.Property(ser => ser.Critico).HasPrecision(5, 2);
            entity.Property(ser => ser.BonusDeCritico).HasPrecision(5, 2);
            entity.Property(ser => ser.Esquiva).HasPrecision(5, 2);
            entity.Property(ser => ser.Precisao).HasPrecision(5, 2);
            entity.Property(ser => ser.Protecao).HasPrecision(5, 2);

            entity.OwnsOne(ser => ser.Resistencias, owned =>
            {
                owned.Property(resistencias => resistencias.Atordoamento).HasPrecision(5, 2);
                owned.Property(resistencias => resistencias.Sangramento).HasPrecision(5, 2);
                owned.Property(resistencias => resistencias.Envenenamento).HasPrecision(5, 2);
                owned.Property(resistencias => resistencias.Debuff).HasPrecision(5, 2);
                owned.Property(resistencias => resistencias.Movimento).HasPrecision(5, 2);
            });
        });
    }

    private static void MapearPersonagem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Personagem>(entity =>
        {
            entity.ToTable("Personagens", table =>
            {
                table.HasCheckConstraint("CK_Personagens_Stress", "[Stress] >= 0 AND [Stress] <= 200");
                table.HasCheckConstraint("CK_Personagens_ChanceVirtude", "[ChanceDeVirtude] >= 0 AND [ChanceDeVirtude] <= 100");
                table.HasCheckConstraint("CK_Personagens_Experiencia", "[Experiencia] >= 0");
            });
            entity.Property(p => p.Classe).HasConversion<string>().HasMaxLength(30);
            entity.Property(p => p.NivelDaArma);
            entity.Property(p => p.NivelDaArmadura);
            entity.Property(p => p.PassosAFrente);
            entity.Property(p => p.PassosAtras);
            entity.Property(p => p.Aflicao).HasMaxLength(60);
            entity.Property(p => p.Virtude).HasMaxLength(60);

            entity.OwnsOne(p => p.ResistenciasExtras, owned =>
            {
                owned.Property(r => r.Doenca).HasPrecision(5, 2);
                owned.Property(r => r.GolpeMortal).HasPrecision(5, 2);
                owned.Property(r => r.Armadilha).HasPrecision(5, 2);
            });

            entity.OwnsMany(p => p.Habilidades, owned =>
            {
                owned.ToTable("HabilidadesDePersonagem");
                owned.WithOwner();
            });

            var slotsConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<Inventario, string>(
                inv => System.Text.Json.JsonSerializer.Serialize(inv.Slots, (System.Text.Json.JsonSerializerOptions?)null),
                json => new Inventario(System.Text.Json.JsonSerializer.Deserialize<List<Guid?>>(json, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<Guid?> { null, null, null, null }));
            var slotsComparer = new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<Inventario>(
                (a, b) => a!.Slots.SequenceEqual(b!.Slots),
                v => v.Slots.Aggregate(0, (soma, guid) => HashCode.Combine(soma, guid ?? Guid.Empty)),
                v => new Inventario(v.Slots.ToList()));

            entity.Property(p => p.Inventario)
                .HasConversion(slotsConverter, slotsComparer)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("InventarioSlots");

            entity.Property(p => p.Aparencia).HasConversion<string>().HasMaxLength(2);
        });
    }

    private static void MapearInimigo(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Inimigo>(entity =>
        {
            entity.ToTable("Inimigos");
            entity.Property(i => i.TipoDeInimigo).HasConversion<string>().HasMaxLength(30);

            var idsConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<IReadOnlyList<Guid>, string>(
                lista => System.Text.Json.JsonSerializer.Serialize(lista, (System.Text.Json.JsonSerializerOptions?)null),
                json => System.Text.Json.JsonSerializer.Deserialize<List<Guid>>(json, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<Guid>());
            var comparer = new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<IReadOnlyList<Guid>>(
                (a, b) => a!.SequenceEqual(b!),
                v => v.Aggregate(0, (soma, guid) => HashCode.Combine(soma, guid)),
                v => v.ToList());

            entity.Property(i => i.HabilidadesIds)
                .HasConversion(idsConverter, comparer)
                .HasColumnType("nvarchar(max)")
                .HasColumnName("HabilidadesIds");
        });
    }

    private static void MapearClasse(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Classe>(entity =>
        {
            entity.ToTable("Classes");
            entity.HasKey(classe => classe.Id);
            entity.Property(classe => classe.Id).ValueGeneratedNever();
            entity.Property(classe => classe.ClasseDeHeroi).HasConversion<string>().HasMaxLength(30);
            entity.HasIndex(classe => classe.ClasseDeHeroi).IsUnique();
            entity.Property(classe => classe.NomeExibicao).IsRequired().HasMaxLength(60);
            entity.Property(classe => classe.NomeOriginal).IsRequired().HasMaxLength(60);
            entity.Property(classe => classe.PassosAFrente).HasDefaultValue(0);
            entity.Property(classe => classe.PassosAtras).HasDefaultValue(0);
            entity.Property(classe => classe.Religiosa).HasDefaultValue(false);
            entity.Property(classe => classe.ProvisaoInicial).IsRequired().HasMaxLength(200).HasDefaultValue(string.Empty);
            entity.Property(classe => classe.BonusAoCriticoDaClasse).IsRequired().HasMaxLength(200).HasDefaultValue(string.Empty);

            entity.OwnsOne(classe => classe.ResistenciasBase, owned =>
            {
                owned.Property(r => r.Atordoamento).HasPrecision(5, 2);
                owned.Property(r => r.Sangramento).HasPrecision(5, 2);
                owned.Property(r => r.Envenenamento).HasPrecision(5, 2);
                owned.Property(r => r.Debuff).HasPrecision(5, 2);
                owned.Property(r => r.Movimento).HasPrecision(5, 2);
                owned.Property(r => r.Doenca).HasPrecision(5, 2);
                owned.Property(r => r.GolpeMortal).HasPrecision(5, 2);
                owned.Property(r => r.Armadilha).HasPrecision(5, 2);
            });

            entity.OwnsMany(classe => classe.Assets, owned =>
            {
                owned.ToTable("AssetsDeClasse");
                owned.WithOwner().HasForeignKey("ClasseId");
                owned.HasKey("ClasseId", nameof(DarkestDungeon.Domain.Classes.AssetsDeClasse.Aparencia));
                owned.Property(a => a.Aparencia).HasConversion<string>().HasMaxLength(2);
                owned.Property(a => a.ConjuntoSpineId).HasMaxLength(200);
                owned.Property(a => a.HashArquivo).HasMaxLength(64);
                owned.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
            });
        });
    }

    private static void MapearHabilidade(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Habilidade>(entity =>
        {
            entity.ToTable("Habilidades");
            entity.HasKey(hab => hab.Id);
            entity.Property(hab => hab.Id).ValueGeneratedNever();
            entity.Property(hab => hab.NomeExibicao).IsRequired().HasMaxLength(80);
            entity.Property(hab => hab.NomeOriginal).IsRequired().HasMaxLength(80);
            entity.Property(hab => hab.Descricao).HasMaxLength(400);
            entity.HasIndex(hab => hab.NomeExibicao).IsUnique();

            entity.HasDiscriminator<string>("Discriminador")
                .HasValue<HabilidadeDeCombate>("Combate")
                .HasValue<HabilidadeDeAcampamento>("Acampamento")
                .HasValue<HabilidadeDeInimigo>("Inimigo");

            entity.OwnsMany(h => h.Niveis, owned =>
            {
                owned.ToTable("NiveisDeHabilidade");
                owned.WithOwner().HasForeignKey("HabilidadeId");
                owned.HasKey("HabilidadeId", nameof(DarkestDungeon.Domain.Habilidades.NivelDeHabilidade.NumeroDoNivel));
                owned.Property(n => n.ModificadorDano).HasPrecision(9, 2);
                owned.Property(n => n.ModificadorAcerto).HasPrecision(9, 2);
                owned.Property(n => n.ModificadorCritico).HasPrecision(9, 2);

                owned.OwnsMany(n => n.ValoresDeEfeito, ownedEfeito =>
                {
                    ownedEfeito.ToTable("ValoresDeEfeitoDeNivel");
                    ownedEfeito.WithOwner();
                    ownedEfeito.Property(v => v.TipoDoEfeito).IsRequired().HasMaxLength(60);
                    ownedEfeito.Property(v => v.Valor).HasPrecision(9, 2);
                    ownedEfeito.Property(v => v.Chance).HasPrecision(5, 2);
                });
            });
        });

        modelBuilder.Entity<HabilidadeDeCombate>(entity =>
        {
            entity.OwnsMany(h => h.Efeitos, MapearEfeitosHeroi);
            entity.OwnsOne(h => h.LimitePorUso, MapearLimiteHeroi);
        });

        modelBuilder.Entity<HabilidadeDeAcampamento>(entity =>
        {
            entity.OwnsMany(h => h.Efeitos, MapearEfeitosAcampamento);
            entity.OwnsOne(h => h.LimitePorUso, MapearLimiteAcampamento);
            entity.Property(h => h.Alvo).HasConversion<string>().HasMaxLength(30);
        });

        modelBuilder.Entity<HabilidadeDeInimigo>(entity =>
        {
            entity.OwnsMany(h => h.Efeitos, MapearEfeitosInimigo);
            entity.Property(h => h.CondicaoDeAparecer).HasMaxLength(200);
            entity.Property(h => h.ChanceDeExecucao).HasPrecision(5, 2);
        });
    }

    private static void MapearClasseHabilidade(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClasseHabilidade>(entity =>
        {
            entity.ToTable("ClassesHabilidades");
            entity.HasKey(assoc => new { assoc.ClasseId, assoc.HabilidadeId });
            entity.HasOne<Classe>().WithMany().HasForeignKey(assoc => assoc.ClasseId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Habilidade>().WithMany().HasForeignKey(assoc => assoc.HabilidadeId).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void MapearItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(entity =>
        {
            entity.ToTable("Itens");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.Id).ValueGeneratedNever();
            entity.Property(item => item.NomeExibicao).IsRequired().HasMaxLength(80);
            entity.Property(item => item.NomeOriginal).IsRequired().HasMaxLength(80);
            entity.Property(item => item.Descricao).HasMaxLength(400);

            entity.HasDiscriminator<string>("Discriminador")
                .HasValue<Arma>("Arma")
                .HasValue<Armadura>("Armadura")
                .HasValue<Acessorio>("Acessorio")
                .HasValue<ItemDeAcampamento>("ItemDeAcampamento")
                .HasValue<Consumivel>("Consumivel");
        });

        modelBuilder.Entity<Arma>(entity =>
        {
            entity.Property(a => a.ClasseElegivel).HasConversion<string>().HasMaxLength(30);
            entity.OwnsMany(a => a.Niveis, owned =>
            {
                owned.ToTable("NiveisArma");
                owned.WithOwner();
                owned.Property(n => n.Critico).HasPrecision(5, 2);
                owned.OwnsOne(n => n.Midia, MapearMidiaDeItem);
            });
        });

        modelBuilder.Entity<Armadura>(entity =>
        {
            entity.Property(a => a.ClasseElegivel).HasConversion<string>().HasMaxLength(30);
            entity.OwnsMany(a => a.Niveis, owned =>
            {
                owned.ToTable("NiveisArmadura");
                owned.WithOwner();
                owned.Property(n => n.Esquiva).HasPrecision(5, 2);
                owned.OwnsOne(n => n.Midia, MapearMidiaDeItem);
            });
        });

        modelBuilder.Entity<Acessorio>(entity =>
        {
            entity.Property(a => a.Raridade).HasConversion<string>().HasMaxLength(30);
            entity.Property(a => a.ClasseExclusiva).HasConversion<string>().HasMaxLength(30);
            entity.OwnsOne(a => a.Midia, MapearMidiaDeItem);
            entity.OwnsMany(a => a.Efeitos, owned =>
            {
                owned.ToTable("EfeitosAcessorio");
                owned.WithOwner();
                owned.Property(e => e.Nome).IsRequired().HasMaxLength(80);
                owned.Property(e => e.Unidade).HasConversion<string>().HasMaxLength(20);
                owned.Property(e => e.Sinal).HasConversion<string>().HasMaxLength(10);
                owned.Property(e => e.Valor).HasPrecision(9, 2);
            });
        });

        modelBuilder.Entity<ItemDeAcampamento>(entity =>
        {
            entity.OwnsOne(i => i.Midia, MapearMidiaDeItem);
        });

        modelBuilder.Entity<Consumivel>(entity =>
        {
            entity.OwnsOne(i => i.Midia, MapearMidiaDeItem);
        });
    }

    private static void MapearMidiaDeItem<TOwner>(Microsoft.EntityFrameworkCore.Metadata.Builders.OwnedNavigationBuilder<TOwner, MidiaDeItem> owned)
        where TOwner : class
    {
        owned.Property(m => m.ArquivoInventarioId).HasMaxLength(200).HasColumnName("Midia_ArquivoInventarioId");
        owned.Property(m => m.ConjuntoSpineId).HasMaxLength(200).HasColumnName("Midia_ConjuntoSpineId");
        owned.Property(m => m.HashArquivo).HasMaxLength(64).HasColumnName("Midia_HashArquivo");
        owned.Property(m => m.Status).HasConversion<string>().HasMaxLength(20).HasColumnName("Midia_Status");
    }

    private static void MapearPublicacaoDeVinculosDeMidia(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PublicacaoDeVinculosDeMidia>(entity =>
        {
            entity.ToTable("PublicacoesDeVinculosDeMidia");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Categoria).HasConversion<string>().HasMaxLength(30);
            entity.Property(e => e.Estado).HasConversion<string>().HasMaxLength(30);
            entity.HasIndex(e => new { e.Categoria, e.Estado });
        });
    }

    private static void MapearEfeitosHeroi(Microsoft.EntityFrameworkCore.Metadata.Builders.OwnedNavigationBuilder<HabilidadeDeCombate, EfeitoDeHabilidade> owned) =>
        ConfigurarEfeito(owned, "EfeitosHabilidadeCombate");

    private static void MapearEfeitosAcampamento(Microsoft.EntityFrameworkCore.Metadata.Builders.OwnedNavigationBuilder<HabilidadeDeAcampamento, EfeitoDeHabilidade> owned) =>
        ConfigurarEfeito(owned, "EfeitosHabilidadeAcampamento");

    private static void MapearEfeitosInimigo(Microsoft.EntityFrameworkCore.Metadata.Builders.OwnedNavigationBuilder<HabilidadeDeInimigo, EfeitoDeHabilidade> owned) =>
        ConfigurarEfeito(owned, "EfeitosHabilidadeInimigo");

    private static void ConfigurarEfeito<TOwner>(Microsoft.EntityFrameworkCore.Metadata.Builders.OwnedNavigationBuilder<TOwner, EfeitoDeHabilidade> owned, string tabela) where TOwner : class
    {
        owned.ToTable(tabela);
        owned.WithOwner();
        owned.Property(e => e.NomeDoEfeito).IsRequired().HasMaxLength(60);
        owned.Property(e => e.Alvo).HasConversion<string>().HasMaxLength(20);
        owned.Property(e => e.Unidade).HasConversion<string>().HasMaxLength(20);
        owned.Property(e => e.ChanceBase).HasPrecision(5, 2);
        owned.Property(e => e.Valor).HasPrecision(9, 2);
    }

    private static void MapearLimiteHeroi(Microsoft.EntityFrameworkCore.Metadata.Builders.OwnedNavigationBuilder<HabilidadeDeCombate, LimitePorUso> owned) =>
        ConfigurarLimite(owned);

    private static void MapearLimiteAcampamento(Microsoft.EntityFrameworkCore.Metadata.Builders.OwnedNavigationBuilder<HabilidadeDeAcampamento, LimitePorUso> owned) =>
        ConfigurarLimite(owned);

    private static void ConfigurarLimite<TOwner>(Microsoft.EntityFrameworkCore.Metadata.Builders.OwnedNavigationBuilder<TOwner, LimitePorUso> owned) where TOwner : class
    {
        owned.Property(l => l.Escopo).HasConversion<string>().HasMaxLength(20);
    }

    private static void MapearMapaDeCobertura(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EntradaDoMapaDeCobertura>(entity =>
        {
            entity.ToTable("MapaDeCobertura");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Classe).HasConversion<string>().HasMaxLength(30);
            entity.Property(e => e.Categoria).HasConversion<string>().HasMaxLength(30);
            entity.Property(e => e.Estado).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.ChaveDoAtributo).IsRequired().HasMaxLength(80);
            entity.Property(e => e.Notas).HasMaxLength(200);
            entity.HasIndex(e => new { e.Classe, e.Categoria, e.ChaveDoAtributo }).IsUnique();
        });
    }

    private static void MapearLogDePublicacao(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LogDePublicacao>(entity =>
        {
            entity.ToTable("LogsDePublicacao");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.PublicacaoId);
            entity.Property(e => e.Timestamp);
            entity.Property(e => e.Nivel).HasConversion<string>().HasMaxLength(16);
            entity.Property(e => e.HabilidadeId);
            entity.Property(e => e.NomeExibicao).HasMaxLength(200);
            entity.Property(e => e.Campo).HasMaxLength(64);
            entity.Property(e => e.Mensagem).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.StackTrace).HasColumnType("nvarchar(max)");
            entity.HasIndex(e => new { e.PublicacaoId, e.Timestamp });
        });
    }
}
