using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarkestDungeon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarNiveisEquipamentosPersonagem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NivelDaArma",
                table: "Personagens",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NivelDaArmadura",
                table: "Personagens",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NivelDaArma",
                table: "Personagens");

            migrationBuilder.DropColumn(
                name: "NivelDaArmadura",
                table: "Personagens");
        }
    }
}
