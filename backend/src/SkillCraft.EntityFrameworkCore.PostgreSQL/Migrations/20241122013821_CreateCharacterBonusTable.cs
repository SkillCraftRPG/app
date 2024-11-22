using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkillCraft.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateCharacterBonusTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Inventory_CharacterId",
                table: "Inventory");

            migrationBuilder.DropIndex(
                name: "IX_Inventory_Id",
                table: "Inventory");

            migrationBuilder.DropIndex(
                name: "IX_CharacterTalents_CharacterId",
                table: "CharacterTalents");

            migrationBuilder.DropIndex(
                name: "IX_CharacterTalents_Id",
                table: "CharacterTalents");

            migrationBuilder.CreateTable(
                name: "CharacterBonuses",
                columns: table => new
                {
                    CharacterBonusId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    Category = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Target = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    IsTemporary = table.Column<bool>(type: "boolean", nullable: false),
                    Precision = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterBonuses", x => x.CharacterBonusId);
                    table.ForeignKey(
                        name: "FK_CharacterBonuses_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_CharacterId_Id",
                table: "Inventory",
                columns: new[] { "CharacterId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterTalents_CharacterId_Id",
                table: "CharacterTalents",
                columns: new[] { "CharacterId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterBonuses_Category",
                table: "CharacterBonuses",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterBonuses_CharacterId_Id",
                table: "CharacterBonuses",
                columns: new[] { "CharacterId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterBonuses_IsTemporary",
                table: "CharacterBonuses",
                column: "IsTemporary");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterBonuses_Precision",
                table: "CharacterBonuses",
                column: "Precision");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterBonuses_Target",
                table: "CharacterBonuses",
                column: "Target");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterBonuses_Value",
                table: "CharacterBonuses",
                column: "Value");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterBonuses");

            migrationBuilder.DropIndex(
                name: "IX_Inventory_CharacterId_Id",
                table: "Inventory");

            migrationBuilder.DropIndex(
                name: "IX_CharacterTalents_CharacterId_Id",
                table: "CharacterTalents");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_CharacterId",
                table: "Inventory",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_Id",
                table: "Inventory",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterTalents_CharacterId",
                table: "CharacterTalents",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterTalents_Id",
                table: "CharacterTalents",
                column: "Id",
                unique: true);
        }
    }
}
