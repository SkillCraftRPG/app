using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkillCraft.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Talents_Id",
                table: "Talents");

            migrationBuilder.DropIndex(
                name: "IX_Lineages_Id",
                table: "Lineages");

            migrationBuilder.DropIndex(
                name: "IX_Lineages_WorldId",
                table: "Lineages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_Id",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_WorldId",
                table: "Languages");

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorldId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<double>(type: "double precision", nullable: true),
                    Weight = table.Column<double>(type: "double precision", nullable: true),
                    Category = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_Items_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalTable: "Worlds",
                        principalColumn: "WorldId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Talents_WorldId_Id",
                table: "Talents",
                columns: new[] { "WorldId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_WorldId_Id",
                table: "Lineages",
                columns: new[] { "WorldId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_WorldId_Id",
                table: "Languages",
                columns: new[] { "WorldId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_AggregateId",
                table: "Items",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_Category",
                table: "Items",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CreatedBy",
                table: "Items",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CreatedOn",
                table: "Items",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Name",
                table: "Items",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Items_UpdatedBy",
                table: "Items",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Items_UpdatedOn",
                table: "Items",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Value",
                table: "Items",
                column: "Value");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Version",
                table: "Items",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Items_Weight",
                table: "Items",
                column: "Weight");

            migrationBuilder.CreateIndex(
                name: "IX_Items_WorldId_Id",
                table: "Items",
                columns: new[] { "WorldId", "Id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Talents_WorldId_Id",
                table: "Talents");

            migrationBuilder.DropIndex(
                name: "IX_Lineages_WorldId_Id",
                table: "Lineages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_WorldId_Id",
                table: "Languages");

            migrationBuilder.CreateIndex(
                name: "IX_Talents_Id",
                table: "Talents",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_Id",
                table: "Lineages",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lineages_WorldId",
                table: "Lineages",
                column: "WorldId");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Id",
                table: "Languages",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_WorldId",
                table: "Languages",
                column: "WorldId");
        }
    }
}
