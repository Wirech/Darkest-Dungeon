using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarkestDungeon.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogoHeroisEEntidades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClasseDeHeroi = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NomeExibicao = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    NomeOriginal = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    ResistenciasBase_Atordoamento = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ResistenciasBase_Sangramento = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ResistenciasBase_Envenenamento = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ResistenciasBase_Debuff = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ResistenciasBase_Movimento = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ResistenciasBase_Doenca = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ResistenciasBase_GolpeMortal = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ResistenciasBase_Armadilha = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Habilidades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomeExibicao = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    NomeOriginal = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Discriminador = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    CustoDeDescanso = table.Column<int>(type: "int", nullable: true),
                    Alvo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    LimitePorUso_Escopo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LimitePorUso_MaximoUsos = table.Column<int>(type: "int", nullable: true),
                    AlvoEmArea = table.Column<bool>(type: "bit", nullable: true),
                    ModificadorDano = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ModificadorAcerto = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ModificadorCritico = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LimitePorUso_Escopo1 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LimitePorUso_MaximoUsos1 = table.Column<int>(type: "int", nullable: true),
                    CondicaoDeAparecer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ChanceDeExecucao = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habilidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Itens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomeExibicao = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    NomeOriginal = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Discriminador = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Raridade = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ClasseExclusiva = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ConjuntoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Arma_ClasseElegivel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ClasseElegivel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Itens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MapaDeCobertura",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Classe = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ChaveDoAtributo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notas = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapaDeCobertura", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Seres",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    HpMaximo = table.Column<int>(type: "int", nullable: false),
                    HpAtual = table.Column<int>(type: "int", nullable: false),
                    Velocidade = table.Column<int>(type: "int", nullable: false),
                    Critico = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    DanoBaseMinimo = table.Column<int>(type: "int", nullable: false),
                    DanoBaseMaximo = table.Column<int>(type: "int", nullable: false),
                    Movimento = table.Column<int>(type: "int", nullable: false),
                    BonusDeCritico = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Tamanho = table.Column<int>(type: "int", nullable: false),
                    AcoesPorTurno = table.Column<int>(type: "int", nullable: false),
                    Esquiva = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Precisao = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Protecao = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    Resistencias_Atordoamento = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Resistencias_Sangramento = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Resistencias_Envenenamento = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Resistencias_Debuff = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Resistencias_Movimento = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seres", x => x.Id);
                    table.CheckConstraint("CK_Seres_DanoBase", "[DanoBaseMinimo] >= 0 AND [DanoBaseMaximo] >= 0 AND [DanoBaseMinimo] <= [DanoBaseMaximo]");
                    table.CheckConstraint("CK_Seres_HP", "[HpMaximo] >= 0 AND [HpAtual] >= 0 AND [HpAtual] <= [HpMaximo]");
                    table.CheckConstraint("CK_Seres_Nivel", "[Nivel] >= 0 AND [Nivel] <= 6");
                    table.CheckConstraint("CK_Seres_Percentuais", "[Critico] >= 0 AND [Critico] <= 100 AND [BonusDeCritico] >= 0 AND [BonusDeCritico] <= 100 AND [Protecao] >= 0 AND [Protecao] <= 100");
                    table.CheckConstraint("CK_Seres_Resistencias", "[Resistencias_Atordoamento] >= 0 AND [Resistencias_Atordoamento] <= 100 AND [Resistencias_Sangramento] >= 0 AND [Resistencias_Sangramento] <= 100 AND [Resistencias_Envenenamento] >= 0 AND [Resistencias_Envenenamento] <= 100 AND [Resistencias_Debuff] >= 0 AND [Resistencias_Debuff] <= 100 AND [Resistencias_Movimento] >= 0 AND [Resistencias_Movimento] <= 100");
                    table.CheckConstraint("CK_Seres_Tamanho", "[Tamanho] >= 0 AND [Tamanho] <= 4");
                });

            migrationBuilder.CreateTable(
                name: "ClassesHabilidades",
                columns: table => new
                {
                    ClasseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HabilidadeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassesHabilidades", x => new { x.ClasseId, x.HabilidadeId });
                    table.ForeignKey(
                        name: "FK_ClassesHabilidades_Classes_ClasseId",
                        column: x => x.ClasseId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassesHabilidades_Habilidades_HabilidadeId",
                        column: x => x.HabilidadeId,
                        principalTable: "Habilidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EfeitosHabilidadeAcampamento",
                columns: table => new
                {
                    HabilidadeDeAcampamentoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeDoEfeito = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Alvo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                    Unidade = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ChanceBase = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    DuracaoEmRodadas = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EfeitosHabilidadeAcampamento", x => new { x.HabilidadeDeAcampamentoId, x.Id });
                    table.ForeignKey(
                        name: "FK_EfeitosHabilidadeAcampamento_Habilidades_HabilidadeDeAcampamentoId",
                        column: x => x.HabilidadeDeAcampamentoId,
                        principalTable: "Habilidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EfeitosHabilidadeCombate",
                columns: table => new
                {
                    HabilidadeDeCombateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeDoEfeito = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Alvo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                    Unidade = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ChanceBase = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    DuracaoEmRodadas = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EfeitosHabilidadeCombate", x => new { x.HabilidadeDeCombateId, x.Id });
                    table.ForeignKey(
                        name: "FK_EfeitosHabilidadeCombate_Habilidades_HabilidadeDeCombateId",
                        column: x => x.HabilidadeDeCombateId,
                        principalTable: "Habilidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EfeitosHabilidadeInimigo",
                columns: table => new
                {
                    HabilidadeDeInimigoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeDoEfeito = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Alvo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                    Unidade = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ChanceBase = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    DuracaoEmRodadas = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EfeitosHabilidadeInimigo", x => new { x.HabilidadeDeInimigoId, x.Id });
                    table.ForeignKey(
                        name: "FK_EfeitosHabilidadeInimigo_Habilidades_HabilidadeDeInimigoId",
                        column: x => x.HabilidadeDeInimigoId,
                        principalTable: "Habilidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EfeitosAcessorio",
                columns: table => new
                {
                    AcessorioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                    Unidade = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Sinal = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EfeitosAcessorio", x => new { x.AcessorioId, x.Id });
                    table.ForeignKey(
                        name: "FK_EfeitosAcessorio_Itens_AcessorioId",
                        column: x => x.AcessorioId,
                        principalTable: "Itens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NiveisArma",
                columns: table => new
                {
                    ArmaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    DanoMinimo = table.Column<int>(type: "int", nullable: false),
                    DanoMaximo = table.Column<int>(type: "int", nullable: false),
                    Critico = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Velocidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NiveisArma", x => new { x.ArmaId, x.Id });
                    table.ForeignKey(
                        name: "FK_NiveisArma_Itens_ArmaId",
                        column: x => x.ArmaId,
                        principalTable: "Itens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NiveisArmadura",
                columns: table => new
                {
                    ArmaduraId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    HpAdicional = table.Column<int>(type: "int", nullable: false),
                    Esquiva = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NiveisArmadura", x => new { x.ArmaduraId, x.Id });
                    table.ForeignKey(
                        name: "FK_NiveisArmadura_Itens_ArmaduraId",
                        column: x => x.ArmaduraId,
                        principalTable: "Itens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inimigos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoDeInimigo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inimigos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inimigos_Seres_Id",
                        column: x => x.Id,
                        principalTable: "Seres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Personagens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Classe = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ResistenciasExtras_Doenca = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ResistenciasExtras_GolpeMortal = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ResistenciasExtras_Armadilha = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Stress = table.Column<int>(type: "int", nullable: false),
                    ChanceDeVirtude = table.Column<int>(type: "int", nullable: false),
                    Aflicao = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Virtude = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    EstadoPortasDaMorte = table.Column<bool>(type: "bit", nullable: false),
                    RecuperouPortasDaMorte = table.Column<bool>(type: "bit", nullable: false),
                    RecuperouAtaqueCardiaco = table.Column<bool>(type: "bit", nullable: false),
                    ArmaEquipadaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ArmaduraEquipadaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AcessorioEquipado1Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AcessorioEquipado2Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personagens", x => x.Id);
                    table.CheckConstraint("CK_Personagens_ChanceVirtude", "[ChanceDeVirtude] >= 0 AND [ChanceDeVirtude] <= 100");
                    table.CheckConstraint("CK_Personagens_Stress", "[Stress] >= 0 AND [Stress] <= 200");
                    table.ForeignKey(
                        name: "FK_Personagens_Seres_Id",
                        column: x => x.Id,
                        principalTable: "Seres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HabilidadesDePersonagem",
                columns: table => new
                {
                    PersonagemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HabilidadeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Habilitada = table.Column<bool>(type: "bit", nullable: false),
                    Treinada = table.Column<bool>(type: "bit", nullable: false),
                    Equipada = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabilidadesDePersonagem", x => new { x.PersonagemId, x.Id });
                    table.ForeignKey(
                        name: "FK_HabilidadesDePersonagem_Personagens_PersonagemId",
                        column: x => x.PersonagemId,
                        principalTable: "Personagens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Classes_ClasseDeHeroi",
                table: "Classes",
                column: "ClasseDeHeroi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassesHabilidades_HabilidadeId",
                table: "ClassesHabilidades",
                column: "HabilidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_Habilidades_NomeExibicao",
                table: "Habilidades",
                column: "NomeExibicao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MapaDeCobertura_Classe_Categoria_ChaveDoAtributo",
                table: "MapaDeCobertura",
                columns: new[] { "Classe", "Categoria", "ChaveDoAtributo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassesHabilidades");

            migrationBuilder.DropTable(
                name: "EfeitosAcessorio");

            migrationBuilder.DropTable(
                name: "EfeitosHabilidadeAcampamento");

            migrationBuilder.DropTable(
                name: "EfeitosHabilidadeCombate");

            migrationBuilder.DropTable(
                name: "EfeitosHabilidadeInimigo");

            migrationBuilder.DropTable(
                name: "HabilidadesDePersonagem");

            migrationBuilder.DropTable(
                name: "Inimigos");

            migrationBuilder.DropTable(
                name: "MapaDeCobertura");

            migrationBuilder.DropTable(
                name: "NiveisArma");

            migrationBuilder.DropTable(
                name: "NiveisArmadura");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "Habilidades");

            migrationBuilder.DropTable(
                name: "Personagens");

            migrationBuilder.DropTable(
                name: "Itens");

            migrationBuilder.DropTable(
                name: "Seres");
        }
    }
}
