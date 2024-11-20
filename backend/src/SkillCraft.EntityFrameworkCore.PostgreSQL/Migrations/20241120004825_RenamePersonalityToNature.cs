using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkillCraft.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class RenamePersonalityToNature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Personalities_PersonalityId",
                table: "Characters");

            migrationBuilder.DropTable(
                name: "Personalities");

            migrationBuilder.RenameColumn(
                name: "PersonalityId",
                table: "Characters",
                newName: "NatureId");

            migrationBuilder.RenameIndex(
                name: "IX_Characters_PersonalityId",
                table: "Characters",
                newName: "IX_Characters_NatureId");

            migrationBuilder.CreateTable(
                name: "Natures",
                columns: table => new
                {
                    NatureId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorldId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Attribute = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    GiftId = table.Column<int>(type: "integer", nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Natures", x => x.NatureId);
                    table.ForeignKey(
                        name: "FK_Natures_Customizations_GiftId",
                        column: x => x.GiftId,
                        principalTable: "Customizations",
                        principalColumn: "CustomizationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Natures_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalTable: "Worlds",
                        principalColumn: "WorldId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Natures_AggregateId",
                table: "Natures",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Natures_Attribute",
                table: "Natures",
                column: "Attribute");

            migrationBuilder.CreateIndex(
                name: "IX_Natures_CreatedBy",
                table: "Natures",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Natures_CreatedOn",
                table: "Natures",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Natures_GiftId",
                table: "Natures",
                column: "GiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Natures_Name",
                table: "Natures",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Natures_UpdatedBy",
                table: "Natures",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Natures_UpdatedOn",
                table: "Natures",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Natures_Version",
                table: "Natures",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Natures_WorldId_Id",
                table: "Natures",
                columns: new[] { "WorldId", "Id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Natures_NatureId",
                table: "Characters",
                column: "NatureId",
                principalTable: "Natures",
                principalColumn: "NatureId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Natures_NatureId",
                table: "Characters");

            migrationBuilder.DropTable(
                name: "Natures");

            migrationBuilder.RenameColumn(
                name: "NatureId",
                table: "Characters",
                newName: "PersonalityId");

            migrationBuilder.RenameIndex(
                name: "IX_Characters_NatureId",
                table: "Characters",
                newName: "IX_Characters_PersonalityId");

            migrationBuilder.CreateTable(
                name: "Personalities",
                columns: table => new
                {
                    PersonalityId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GiftId = table.Column<int>(type: "integer", nullable: true),
                    WorldId = table.Column<int>(type: "integer", nullable: false),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Attribute = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personalities", x => x.PersonalityId);
                    table.ForeignKey(
                        name: "FK_Personalities_Customizations_GiftId",
                        column: x => x.GiftId,
                        principalTable: "Customizations",
                        principalColumn: "CustomizationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Personalities_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalTable: "Worlds",
                        principalColumn: "WorldId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Personalities_AggregateId",
                table: "Personalities",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personalities_Attribute",
                table: "Personalities",
                column: "Attribute");

            migrationBuilder.CreateIndex(
                name: "IX_Personalities_CreatedBy",
                table: "Personalities",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Personalities_CreatedOn",
                table: "Personalities",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Personalities_GiftId",
                table: "Personalities",
                column: "GiftId");

            migrationBuilder.CreateIndex(
                name: "IX_Personalities_Name",
                table: "Personalities",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Personalities_UpdatedBy",
                table: "Personalities",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Personalities_UpdatedOn",
                table: "Personalities",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Personalities_Version",
                table: "Personalities",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Personalities_WorldId_Id",
                table: "Personalities",
                columns: new[] { "WorldId", "Id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Personalities_PersonalityId",
                table: "Characters",
                column: "PersonalityId",
                principalTable: "Personalities",
                principalColumn: "PersonalityId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
