using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class RefactorCustomizationPersonalityIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Personalities_Id",
                table: "Personalities");

            migrationBuilder.DropIndex(
                name: "IX_Personalities_WorldId",
                table: "Personalities");

            migrationBuilder.DropIndex(
                name: "IX_Customizations_Id",
                table: "Customizations");

            migrationBuilder.DropIndex(
                name: "IX_Customizations_WorldId",
                table: "Customizations");

            migrationBuilder.CreateIndex(
                name: "IX_Personalities_WorldId_Id",
                table: "Personalities",
                columns: new[] { "WorldId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_WorldId_Id",
                table: "Customizations",
                columns: new[] { "WorldId", "Id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Personalities_WorldId_Id",
                table: "Personalities");

            migrationBuilder.DropIndex(
                name: "IX_Customizations_WorldId_Id",
                table: "Customizations");

            migrationBuilder.CreateIndex(
                name: "IX_Personalities_Id",
                table: "Personalities",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personalities_WorldId",
                table: "Personalities",
                column: "WorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_Id",
                table: "Customizations",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customizations_WorldId",
                table: "Customizations",
                column: "WorldId");
        }
    }
}
