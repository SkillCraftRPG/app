using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkillCraft.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateWorldAndStorageTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StorageSummaries",
                columns: table => new
                {
                    StorageSummaryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AllocatedBytes = table.Column<long>(type: "bigint", nullable: false),
                    UsedBytes = table.Column<long>(type: "bigint", nullable: false),
                    AvailableBytes = table.Column<long>(type: "bigint", nullable: false),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageSummaries", x => x.StorageSummaryId);
                });

            migrationBuilder.CreateTable(
                name: "Worlds",
                columns: table => new
                {
                    WorldId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    UniqueSlug = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueSlugNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worlds", x => x.WorldId);
                });

            migrationBuilder.CreateTable(
                name: "StorageDetails",
                columns: table => new
                {
                    StorageDetailId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorldId = table.Column<int>(type: "integer", nullable: false),
                    EntityType = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageDetails", x => x.StorageDetailId);
                    table.ForeignKey(
                        name: "FK_StorageDetails_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalTable: "Worlds",
                        principalColumn: "WorldId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StorageDetails_EntityType_EntityId",
                table: "StorageDetails",
                columns: new[] { "EntityType", "EntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StorageDetails_Size",
                table: "StorageDetails",
                column: "Size");

            migrationBuilder.CreateIndex(
                name: "IX_StorageDetails_UserId",
                table: "StorageDetails",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageDetails_WorldId",
                table: "StorageDetails",
                column: "WorldId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageSummaries_AggregateId",
                table: "StorageSummaries",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StorageSummaries_AllocatedBytes",
                table: "StorageSummaries",
                column: "AllocatedBytes");

            migrationBuilder.CreateIndex(
                name: "IX_StorageSummaries_AvailableBytes",
                table: "StorageSummaries",
                column: "AvailableBytes");

            migrationBuilder.CreateIndex(
                name: "IX_StorageSummaries_CreatedBy",
                table: "StorageSummaries",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StorageSummaries_CreatedOn",
                table: "StorageSummaries",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_StorageSummaries_UpdatedBy",
                table: "StorageSummaries",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StorageSummaries_UpdatedOn",
                table: "StorageSummaries",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_StorageSummaries_UsedBytes",
                table: "StorageSummaries",
                column: "UsedBytes");

            migrationBuilder.CreateIndex(
                name: "IX_StorageSummaries_UserId",
                table: "StorageSummaries",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StorageSummaries_Version",
                table: "StorageSummaries",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_AggregateId",
                table: "Worlds",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_CreatedBy",
                table: "Worlds",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_CreatedOn",
                table: "Worlds",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_DisplayName",
                table: "Worlds",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_Id",
                table: "Worlds",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_OwnerId",
                table: "Worlds",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_UniqueSlug",
                table: "Worlds",
                column: "UniqueSlug");

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_UniqueSlugNormalized",
                table: "Worlds",
                column: "UniqueSlugNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_UpdatedBy",
                table: "Worlds",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_UpdatedOn",
                table: "Worlds",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_Version",
                table: "Worlds",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StorageDetails");

            migrationBuilder.DropTable(
                name: "StorageSummaries");

            migrationBuilder.DropTable(
                name: "Worlds");
        }
    }
}
