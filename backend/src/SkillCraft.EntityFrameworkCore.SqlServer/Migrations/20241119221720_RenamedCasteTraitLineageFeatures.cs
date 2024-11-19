using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class RenamedCasteTraitLineageFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Features",
                table: "Lineages",
                newName: "Traits");

            migrationBuilder.RenameColumn(
                name: "Traits",
                table: "Castes",
                newName: "Features");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Traits",
                table: "Lineages",
                newName: "Features");

            migrationBuilder.RenameColumn(
                name: "Features",
                table: "Castes",
                newName: "Traits");
        }
    }
}
