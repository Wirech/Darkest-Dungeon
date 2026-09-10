using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarkestDungeon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NiveisEProgressao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Aparencia",
                table: "Personagens",
                type: "nvarchar(2)",
                maxLength: 2,
                nullable: false,
                defaultValue: "A");

            migrationBuilder.AddColumn<int>(
                name: "Experiencia",
                table: "Personagens",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumeroDoNivel",
                table: "HabilidadesDePersonagem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AssetsDeClasse",
                columns: table => new
                {
                    Aparencia = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    ClasseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConjuntoSpineId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HashArquivo = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetsDeClasse", x => new { x.ClasseId, x.Aparencia });
                    table.ForeignKey(
                        name: "FK_AssetsDeClasse_Classes_ClasseId",
                        column: x => x.ClasseId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NiveisDeHabilidade",
                columns: table => new
                {
                    NumeroDoNivel = table.Column<int>(type: "int", nullable: false),
                    HabilidadeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModificadorDano = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                    ModificadorAcerto = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                    ModificadorCritico = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                    CustoDeDescanso = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NiveisDeHabilidade", x => new { x.HabilidadeId, x.NumeroDoNivel });
                    table.ForeignKey(
                        name: "FK_NiveisDeHabilidade_Habilidades_HabilidadeId",
                        column: x => x.HabilidadeId,
                        principalTable: "Habilidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ValoresDeEfeitoDeNivel",
                columns: table => new
                {
                    NivelDeHabilidadeHabilidadeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NivelDeHabilidadeNumeroDoNivel = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoDoEfeito = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(9,2)", precision: 9, scale: 2, nullable: false),
                    Chance = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValoresDeEfeitoDeNivel", x => new { x.NivelDeHabilidadeHabilidadeId, x.NivelDeHabilidadeNumeroDoNivel, x.Id });
                    table.ForeignKey(
                        name: "FK_ValoresDeEfeitoDeNivel_NiveisDeHabilidade_NivelDeHabilidadeHabilidadeId_NivelDeHabilidadeNumeroDoNivel",
                        columns: x => new { x.NivelDeHabilidadeHabilidadeId, x.NivelDeHabilidadeNumeroDoNivel },
                        principalTable: "NiveisDeHabilidade",
                        principalColumns: new[] { "HabilidadeId", "NumeroDoNivel" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Personagens_Experiencia",
                table: "Personagens",
                sql: "[Experiencia] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_HabilidadesDePersonagem_NumeroDoNivel",
                table: "HabilidadesDePersonagem",
                sql: "[NumeroDoNivel] BETWEEN 0 AND 5");

            migrationBuilder.AddCheckConstraint(
                name: "CK_NiveisDeHabilidade_NumeroDoNivel",
                table: "NiveisDeHabilidade",
                sql: "[NumeroDoNivel] BETWEEN 1 AND 5");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetsDeClasse");

            migrationBuilder.DropTable(
                name: "ValoresDeEfeitoDeNivel");

            migrationBuilder.DropTable(
                name: "NiveisDeHabilidade");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Personagens_Experiencia",
                table: "Personagens");

            migrationBuilder.DropCheckConstraint(
                name: "CK_HabilidadesDePersonagem_NumeroDoNivel",
                table: "HabilidadesDePersonagem");

            migrationBuilder.DropColumn(
                name: "Aparencia",
                table: "Personagens");

            migrationBuilder.DropColumn(
                name: "Experiencia",
                table: "Personagens");

            migrationBuilder.DropColumn(
                name: "NumeroDoNivel",
                table: "HabilidadesDePersonagem");
        }
    }
}
