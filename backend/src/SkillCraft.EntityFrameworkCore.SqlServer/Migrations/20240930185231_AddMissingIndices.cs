using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Talents_AllowMultiplePurchases",
                table: "Talents",
                column: "AllowMultiplePurchases");

            migrationBuilder.CreateIndex(
                name: "IX_Talents_Tier",
                table: "Talents",
                column: "Tier");

            migrationBuilder.CreateIndex(
                name: "IX_Personalities_Attribute",
                table: "Personalities",
                column: "Attribute");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_Skill",
                table: "Educations",
                column: "Skill");

            migrationBuilder.CreateIndex(
                name: "IX_Castes_Skill",
                table: "Castes",
                column: "Skill");

            migrationBuilder.CreateIndex(
                name: "IX_Aspects_DiscountedSkill1",
                table: "Aspects",
                column: "DiscountedSkill1");

            migrationBuilder.CreateIndex(
                name: "IX_Aspects_DiscountedSkill2",
                table: "Aspects",
                column: "DiscountedSkill2");

            migrationBuilder.CreateIndex(
                name: "IX_Aspects_MandatoryAttribute1",
                table: "Aspects",
                column: "MandatoryAttribute1");

            migrationBuilder.CreateIndex(
                name: "IX_Aspects_MandatoryAttribute2",
                table: "Aspects",
                column: "MandatoryAttribute2");

            migrationBuilder.CreateIndex(
                name: "IX_Aspects_OptionalAttribute1",
                table: "Aspects",
                column: "OptionalAttribute1");

            migrationBuilder.CreateIndex(
                name: "IX_Aspects_OptionalAttribute2",
                table: "Aspects",
                column: "OptionalAttribute2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Talents_AllowMultiplePurchases",
                table: "Talents");

            migrationBuilder.DropIndex(
                name: "IX_Talents_Tier",
                table: "Talents");

            migrationBuilder.DropIndex(
                name: "IX_Personalities_Attribute",
                table: "Personalities");

            migrationBuilder.DropIndex(
                name: "IX_Educations_Skill",
                table: "Educations");

            migrationBuilder.DropIndex(
                name: "IX_Castes_Skill",
                table: "Castes");

            migrationBuilder.DropIndex(
                name: "IX_Aspects_DiscountedSkill1",
                table: "Aspects");

            migrationBuilder.DropIndex(
                name: "IX_Aspects_DiscountedSkill2",
                table: "Aspects");

            migrationBuilder.DropIndex(
                name: "IX_Aspects_MandatoryAttribute1",
                table: "Aspects");

            migrationBuilder.DropIndex(
                name: "IX_Aspects_MandatoryAttribute2",
                table: "Aspects");

            migrationBuilder.DropIndex(
                name: "IX_Aspects_OptionalAttribute1",
                table: "Aspects");

            migrationBuilder.DropIndex(
                name: "IX_Aspects_OptionalAttribute2",
                table: "Aspects");
        }
    }
}
