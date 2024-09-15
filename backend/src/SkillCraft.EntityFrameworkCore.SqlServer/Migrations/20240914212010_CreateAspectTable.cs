using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateAspectTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aspects",
                columns: table => new
                {
                    AspectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorldId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MandatoryAttribute1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MandatoryAttribute2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OptionalAttribute1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    OptionalAttribute2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DiscountedSkill1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DiscountedSkill2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AggregateId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aspects", x => x.AspectId);
                    table.ForeignKey(
                        name: "FK_Aspects_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalTable: "Worlds",
                        principalColumn: "WorldId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Aspects_AggregateId",
                table: "Aspects",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Aspects_CreatedBy",
                table: "Aspects",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Aspects_CreatedOn",
                table: "Aspects",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Aspects_Id",
                table: "Aspects",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Aspects_Name",
                table: "Aspects",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Aspects_UpdatedBy",
                table: "Aspects",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Aspects_UpdatedOn",
                table: "Aspects",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Aspects_Version",
                table: "Aspects",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Aspects_WorldId",
                table: "Aspects",
                column: "WorldId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Aspects");
        }
    }
}
