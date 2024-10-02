using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class RefactorCasteEducationIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Educations_Id",
                table: "Educations");

            migrationBuilder.DropIndex(
                name: "IX_Educations_WorldId",
                table: "Educations");

            migrationBuilder.DropIndex(
                name: "IX_Castes_Id",
                table: "Castes");

            migrationBuilder.DropIndex(
                name: "IX_Castes_WorldId",
                table: "Castes");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_WorldId_Id",
                table: "Educations",
                columns: new[] { "WorldId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Castes_WorldId_Id",
                table: "Castes",
                columns: new[] { "WorldId", "Id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Educations_WorldId_Id",
                table: "Educations");

            migrationBuilder.DropIndex(
                name: "IX_Castes_WorldId_Id",
                table: "Castes");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_Id",
                table: "Educations",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Educations_WorldId",
                table: "Educations",
                column: "WorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Castes_Id",
                table: "Castes",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Castes_WorldId",
                table: "Castes",
                column: "WorldId");
        }
    }
}
