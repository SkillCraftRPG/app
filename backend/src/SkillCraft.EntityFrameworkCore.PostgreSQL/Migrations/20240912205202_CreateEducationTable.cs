using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkillCraft.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateEducationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Educations",
                columns: table => new
                {
                    EducationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorldId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Skill = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    WealthMultiplier = table.Column<double>(type: "double precision", nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Educations", x => x.EducationId);
                    table.ForeignKey(
                        name: "FK_Educations_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalTable: "Worlds",
                        principalColumn: "WorldId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Educations_AggregateId",
                table: "Educations",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Educations_CreatedBy",
                table: "Educations",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_CreatedOn",
                table: "Educations",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_Id",
                table: "Educations",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Educations_Name",
                table: "Educations",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_UpdatedBy",
                table: "Educations",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_UpdatedOn",
                table: "Educations",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_Version",
                table: "Educations",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_WorldId",
                table: "Educations",
                column: "WorldId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Educations");
        }
    }
}
