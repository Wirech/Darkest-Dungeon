using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarkestDungeon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MidiasDeEquipamentoEItens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Midia_ArquivoInventarioId",
                table: "NiveisArmadura",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Midia_ConjuntoSpineId",
                table: "NiveisArmadura",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Midia_HashArquivo",
                table: "NiveisArmadura",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Midia_Status",
                table: "NiveisArmadura",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pendente");

            migrationBuilder.AddColumn<string>(
                name: "Midia_ArquivoInventarioId",
                table: "NiveisArma",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Midia_ConjuntoSpineId",
                table: "NiveisArma",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Midia_HashArquivo",
                table: "NiveisArma",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Midia_Status",
                table: "NiveisArma",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Pendente");

            migrationBuilder.AlterColumn<string>(
                name: "Discriminador",
                table: "Itens",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(13)",
                oldMaxLength: 13);

            migrationBuilder.AddColumn<string>(
                name: "Midia_ArquivoInventarioId",
                table: "Itens",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Midia_ConjuntoSpineId",
                table: "Itens",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Midia_HashArquivo",
                table: "Itens",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Midia_Status",
                table: "Itens",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PublicacoesDeVinculosDeMidia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IniciadaEm = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ConcluidaEm = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ItensAtualizados = table.Column<int>(type: "int", nullable: false),
                    VinculosOk = table.Column<int>(type: "int", nullable: false),
                    VinculosPendentes = table.Column<int>(type: "int", nullable: false),
                    AcessoriosNovos = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicacoesDeVinculosDeMidia", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PublicacoesDeVinculosDeMidia_Categoria_Estado",
                table: "PublicacoesDeVinculosDeMidia",
                columns: new[] { "Categoria", "Estado" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublicacoesDeVinculosDeMidia");

            migrationBuilder.DropColumn(
                name: "Midia_ArquivoInventarioId",
                table: "NiveisArmadura");

            migrationBuilder.DropColumn(
                name: "Midia_ConjuntoSpineId",
                table: "NiveisArmadura");

            migrationBuilder.DropColumn(
                name: "Midia_HashArquivo",
                table: "NiveisArmadura");

            migrationBuilder.DropColumn(
                name: "Midia_Status",
                table: "NiveisArmadura");

            migrationBuilder.DropColumn(
                name: "Midia_ArquivoInventarioId",
                table: "NiveisArma");

            migrationBuilder.DropColumn(
                name: "Midia_ConjuntoSpineId",
                table: "NiveisArma");

            migrationBuilder.DropColumn(
                name: "Midia_HashArquivo",
                table: "NiveisArma");

            migrationBuilder.DropColumn(
                name: "Midia_Status",
                table: "NiveisArma");

            migrationBuilder.DropColumn(
                name: "Midia_ArquivoInventarioId",
                table: "Itens");

            migrationBuilder.DropColumn(
                name: "Midia_ConjuntoSpineId",
                table: "Itens");

            migrationBuilder.DropColumn(
                name: "Midia_HashArquivo",
                table: "Itens");

            migrationBuilder.DropColumn(
                name: "Midia_Status",
                table: "Itens");

            migrationBuilder.AlterColumn<string>(
                name: "Discriminador",
                table: "Itens",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(21)",
                oldMaxLength: 21);
        }
    }
}
