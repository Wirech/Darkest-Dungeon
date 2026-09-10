using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DarkestDungeon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LogsDePublicacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogsDePublicacao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublicacaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nivel = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    HabilidadeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NomeExibicao = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Campo = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Mensagem = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    StackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogsDePublicacao", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogsDePublicacao_PublicacaoId_Timestamp",
                table: "LogsDePublicacao",
                columns: new[] { "PublicacaoId", "Timestamp" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogsDePublicacao");
        }
    }
}
