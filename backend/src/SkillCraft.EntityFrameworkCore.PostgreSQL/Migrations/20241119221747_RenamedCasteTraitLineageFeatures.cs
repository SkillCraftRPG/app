using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkillCraft.EntityFrameworkCore.PostgreSQL.Migrations
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

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorldId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PlayerName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    LineageId = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<double>(type: "double precision", nullable: false),
                    Weight = table.Column<double>(type: "double precision", nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: false),
                    PersonalityId = table.Column<int>(type: "integer", nullable: false),
                    CasteId = table.Column<int>(type: "integer", nullable: false),
                    EducationId = table.Column<int>(type: "integer", nullable: false),
                    Agility = table.Column<int>(type: "integer", nullable: false),
                    Coordination = table.Column<int>(type: "integer", nullable: false),
                    Intellect = table.Column<int>(type: "integer", nullable: false),
                    Presence = table.Column<int>(type: "integer", nullable: false),
                    Sensitivity = table.Column<int>(type: "integer", nullable: false),
                    Spirit = table.Column<int>(type: "integer", nullable: false),
                    Vigor = table.Column<int>(type: "integer", nullable: false),
                    BestAttribute = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    WorstAttribute = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    MandatoryAttributes = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OptionalAttributes = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ExtraAttributes = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.CharacterId);
                    table.ForeignKey(
                        name: "FK_Characters_Castes_CasteId",
                        column: x => x.CasteId,
                        principalTable: "Castes",
                        principalColumn: "CasteId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_Educations_EducationId",
                        column: x => x.EducationId,
                        principalTable: "Educations",
                        principalColumn: "EducationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_Lineages_LineageId",
                        column: x => x.LineageId,
                        principalTable: "Lineages",
                        principalColumn: "LineageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_Personalities_PersonalityId",
                        column: x => x.PersonalityId,
                        principalTable: "Personalities",
                        principalColumn: "PersonalityId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalTable: "Worlds",
                        principalColumn: "WorldId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharacterAspects",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    AspectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterAspects", x => new { x.CharacterId, x.AspectId });
                    table.ForeignKey(
                        name: "FK_CharacterAspects_Aspects_AspectId",
                        column: x => x.AspectId,
                        principalTable: "Aspects",
                        principalColumn: "AspectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterAspects_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterCustomizations",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    CustomizationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterCustomizations", x => new { x.CharacterId, x.CustomizationId });
                    table.ForeignKey(
                        name: "FK_CharacterCustomizations_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterCustomizations_Customizations_CustomizationId",
                        column: x => x.CustomizationId,
                        principalTable: "Customizations",
                        principalColumn: "CustomizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterLanguages",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    LanguageId = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterLanguages", x => new { x.CharacterId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_CharacterLanguages_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterLanguages_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharacterTalents",
                columns: table => new
                {
                    CharacterTalentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    TalentId = table.Column<int>(type: "integer", nullable: false),
                    Cost = table.Column<int>(type: "integer", nullable: false),
                    Precision = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterTalents", x => x.CharacterTalentId);
                    table.ForeignKey(
                        name: "FK_CharacterTalents_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterTalents_Talents_TalentId",
                        column: x => x.TalentId,
                        principalTable: "Talents",
                        principalColumn: "TalentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    InventoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    ContainingItemId = table.Column<Guid>(type: "uuid", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    IsAttuned = table.Column<bool>(type: "boolean", nullable: true),
                    IsEquipped = table.Column<bool>(type: "boolean", nullable: false),
                    IsIdentified = table.Column<bool>(type: "boolean", nullable: false),
                    IsProficient = table.Column<bool>(type: "boolean", nullable: true),
                    Skill = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    RemainingCharges = table.Column<int>(type: "integer", nullable: true),
                    RemainingResistance = table.Column<int>(type: "integer", nullable: true),
                    NameOverride = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DescriptionOverride = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ValueOverride = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.InventoryId);
                    table.ForeignKey(
                        name: "FK_Inventory_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inventory_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterAspects_AspectId",
                table: "CharacterAspects",
                column: "AspectId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterCustomizations_CustomizationId",
                table: "CharacterCustomizations",
                column: "CustomizationId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterLanguages_LanguageId",
                table: "CharacterLanguages",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_AggregateId",
                table: "Characters",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CasteId",
                table: "Characters",
                column: "CasteId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CreatedBy",
                table: "Characters",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CreatedOn",
                table: "Characters",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_EducationId",
                table: "Characters",
                column: "EducationId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_LineageId",
                table: "Characters",
                column: "LineageId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_Name",
                table: "Characters",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_PersonalityId",
                table: "Characters",
                column: "PersonalityId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_PlayerName",
                table: "Characters",
                column: "PlayerName");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_UpdatedBy",
                table: "Characters",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_UpdatedOn",
                table: "Characters",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_Version",
                table: "Characters",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_WorldId_Id",
                table: "Characters",
                columns: new[] { "WorldId", "Id" },
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

            migrationBuilder.CreateIndex(
                name: "IX_CharacterTalents_TalentId",
                table: "CharacterTalents",
                column: "TalentId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_CharacterId",
                table: "Inventory",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_ContainingItemId",
                table: "Inventory",
                column: "ContainingItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_Id",
                table: "Inventory",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_ItemId",
                table: "Inventory",
                column: "ItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterAspects");

            migrationBuilder.DropTable(
                name: "CharacterCustomizations");

            migrationBuilder.DropTable(
                name: "CharacterLanguages");

            migrationBuilder.DropTable(
                name: "CharacterTalents");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "Characters");

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
