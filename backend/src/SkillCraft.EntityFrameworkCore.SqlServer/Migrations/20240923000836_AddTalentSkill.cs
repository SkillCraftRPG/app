using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddTalentSkill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Skill",
                table: "Talents",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Talents_Skill",
                table: "Talents",
                column: "Skill",
                unique: true,
                filter: "[Skill] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Talents_Skill",
                table: "Talents");

            migrationBuilder.DropColumn(
                name: "Skill",
                table: "Talents");
        }
    }
}
