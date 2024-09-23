using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class FixTalentSkillIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Talents_Skill",
                table: "Talents");

            migrationBuilder.DropIndex(
                name: "IX_Talents_WorldId",
                table: "Talents");

            migrationBuilder.CreateIndex(
                name: "IX_Talents_Skill",
                table: "Talents",
                column: "Skill");

            migrationBuilder.CreateIndex(
                name: "IX_Talents_WorldId_Skill",
                table: "Talents",
                columns: new[] { "WorldId", "Skill" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Talents_Skill",
                table: "Talents");

            migrationBuilder.DropIndex(
                name: "IX_Talents_WorldId_Skill",
                table: "Talents");

            migrationBuilder.CreateIndex(
                name: "IX_Talents_Skill",
                table: "Talents",
                column: "Skill",
                unique: true,
                filter: "[Skill] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Talents_WorldId",
                table: "Talents",
                column: "WorldId");
        }
    }
}
