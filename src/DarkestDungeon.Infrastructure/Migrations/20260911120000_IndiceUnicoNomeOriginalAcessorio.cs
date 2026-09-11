using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using DarkestDungeon.Infrastructure.Data;

#nullable disable

namespace DarkestDungeon.Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(DarkestDungeonDbContext))]
    [Migration("20260911120000_IndiceUnicoNomeOriginalAcessorio")]
    public partial class IndiceUnicoNomeOriginalAcessorio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE UNIQUE INDEX [IX_Itens_Acessorio_NomeOriginal]
                ON [Itens] ([NomeOriginal])
                WHERE [Discriminador] = N'Acessorio';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP INDEX [IX_Itens_Acessorio_NomeOriginal] ON [Itens];
                """);
        }
    }
}
