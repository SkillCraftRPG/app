using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class RenameLineageTraitToFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Traits",
                table: "Lineages",
                newName: "Features");

            migrationBuilder.AlterColumn<string>(
                name: "NamesText",
                table: "Lineages",
                type: "character varying(65535)",
                maxLength: 65535,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LanguagesText",
                table: "Lineages",
                type: "character varying(65535)",
                maxLength: 65535,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Features",
                table: "Lineages",
                newName: "Traits");

            migrationBuilder.AlterColumn<string>(
                name: "NamesText",
                table: "Lineages",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(65535)",
                oldMaxLength: 65535,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LanguagesText",
                table: "Lineages",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(65535)",
                oldMaxLength: 65535,
                oldNullable: true);
        }
    }
}
