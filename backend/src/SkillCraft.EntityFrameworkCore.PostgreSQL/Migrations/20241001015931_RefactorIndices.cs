using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class RefactorIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Parties_Id",
                table: "Parties");

            migrationBuilder.DropIndex(
                name: "IX_Parties_WorldId",
                table: "Parties");

            migrationBuilder.DropIndex(
                name: "IX_Aspects_Id",
                table: "Aspects");

            migrationBuilder.DropIndex(
                name: "IX_Aspects_WorldId",
                table: "Aspects");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_WorldId_Id",
                table: "Parties",
                columns: new[] { "WorldId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Aspects_WorldId_Id",
                table: "Aspects",
                columns: new[] { "WorldId", "Id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Parties_WorldId_Id",
                table: "Parties");

            migrationBuilder.DropIndex(
                name: "IX_Aspects_WorldId_Id",
                table: "Aspects");

            migrationBuilder.CreateIndex(
                name: "IX_Parties_Id",
                table: "Parties",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parties_WorldId",
                table: "Parties",
                column: "WorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Aspects_Id",
                table: "Aspects",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Aspects_WorldId",
                table: "Aspects",
                column: "WorldId");
        }
    }
}
