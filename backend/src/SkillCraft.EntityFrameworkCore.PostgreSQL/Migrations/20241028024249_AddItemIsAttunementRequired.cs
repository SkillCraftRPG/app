using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class AddItemIsAttunementRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAttunementRequired",
                table: "Items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Items_IsAttunementRequired",
                table: "Items",
                column: "IsAttunementRequired");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Items_IsAttunementRequired",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IsAttunementRequired",
                table: "Items");
        }
    }
}
