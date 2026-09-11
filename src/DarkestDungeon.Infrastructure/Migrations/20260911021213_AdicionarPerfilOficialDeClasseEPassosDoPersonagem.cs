using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarkestDungeon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarPerfilOficialDeClasseEPassosDoPersonagem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PassosAFrente",
                table: "Personagens",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PassosAtras",
                table: "Personagens",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BonusAoCriticoDaClasse",
                table: "Classes",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PassosAFrente",
                table: "Classes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PassosAtras",
                table: "Classes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProvisaoInicial",
                table: "Classes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Religiosa",
                table: "Classes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassosAFrente",
                table: "Personagens");

            migrationBuilder.DropColumn(
                name: "PassosAtras",
                table: "Personagens");

            migrationBuilder.DropColumn(
                name: "BonusAoCriticoDaClasse",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "PassosAFrente",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "PassosAtras",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "ProvisaoInicial",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Religiosa",
                table: "Classes");
        }
    }
}
