using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarkestDungeon.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialSerSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Seres");
        }
    }
}
