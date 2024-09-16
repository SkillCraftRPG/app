using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreatePersonalityTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Personalities",
                columns: table => new
                {
                    PersonalityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorldId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Attribute = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    GiftId = table.Column<int>(type: "int", nullable: true),
                    AggregateId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "IX_Personalities_Id",
                table: "Personalities",
                column: "Id",
                unique: true);

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
                name: "IX_Personalities_WorldId",
                table: "Personalities",
                column: "WorldId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Personalities");
        }
    }
}
