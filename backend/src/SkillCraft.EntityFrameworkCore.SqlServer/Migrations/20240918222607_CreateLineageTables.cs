using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateLineageTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lineages",
                columns: table => new
                {
                    LineageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorldId = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Agility = table.Column<int>(type: "int", nullable: false),
                    Coordination = table.Column<int>(type: "int", nullable: false),
                    Intellect = table.Column<int>(type: "int", nullable: false),
                    Presence = table.Column<int>(type: "int", nullable: false),
                    Sensitivity = table.Column<int>(type: "int", nullable: false),
                    Spirit = table.Column<int>(type: "int", nullable: false),
                    Vigor = table.Column<int>(type: "int", nullable: false),
                    ExtraAttributes = table.Column<int>(type: "int", nullable: false),
                    Traits = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExtraLanguages = table.Column<int>(type: "int", nullable: false),
                    LanguagesText = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NamesText = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FamilyNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FemaleNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaleNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnisexNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WalkSpeed = table.Column<int>(type: "int", nullable: false),
                    ClimbSpeed = table.Column<int>(type: "int", nullable: false),
                    SwimSpeed = table.Column<int>(type: "int", nullable: false),
                    FlySpeed = table.Column<int>(type: "int", nullable: false),
                    HoverSpeed = table.Column<int>(type: "int", nullable: false),
                    BurrowSpeed = table.Column<int>(type: "int", nullable: false),
                    SizeCategory = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SizeRoll = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StarvedRoll = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SkinnyRoll = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NormalRoll = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OverweightRoll = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ObeseRoll = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AdolescentAge = table.Column<int>(type: "int", nullable: true),
                    AdultAge = table.Column<int>(type: "int", nullable: true),
                    MatureAge = table.Column<int>(type: "int", nullable: true),
                    VenerableAge = table.Column<int>(type: "int", nullable: true),
                    AggregateId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lineages", x => x.LineageId);
                    table.ForeignKey(
                        name: "FK_Lineages_Lineages_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Lineages",
                        principalColumn: "LineageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lineages_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalTable: "Worlds",
                        principalColumn: "WorldId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LineageLanguages",
                columns: table => new
                {
                    LineageId = table.Column<int>(type: "int", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineageLanguages", x => new { x.LineageId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_LineageLanguages_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LineageLanguages_Lineages_LineageId",
                        column: x => x.LineageId,
                        principalTable: "Lineages",
                        principalColumn: "LineageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LineageLanguages_LanguageId",
                table: "LineageLanguages",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_AggregateId",
                table: "Lineages",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_CreatedBy",
                table: "Lineages",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_CreatedOn",
                table: "Lineages",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_Id",
                table: "Lineages",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_Name",
                table: "Lineages",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_ParentId",
                table: "Lineages",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_UpdatedBy",
                table: "Lineages",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_UpdatedOn",
                table: "Lineages",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_Version",
                table: "Lineages",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_WorldId",
                table: "Lineages",
                column: "WorldId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineageLanguages");

            migrationBuilder.DropTable(
                name: "Lineages");
        }
    }
}
