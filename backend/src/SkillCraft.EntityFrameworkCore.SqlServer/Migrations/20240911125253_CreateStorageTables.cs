using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateStorageTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StorageDetails",
                columns: table => new
                {
                    StorageDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorldId = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageDetails", x => x.StorageDetailId);
                    table.ForeignKey(
                        name: "FK_StorageDetails_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StorageDetails_Worlds_WorldId",
                        column: x => x.WorldId,
                        principalTable: "Worlds",
                        principalColumn: "WorldId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StorageSummaries",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllocatedBytes = table.Column<long>(type: "bigint", nullable: false),
                    UsedBytes = table.Column<long>(type: "bigint", nullable: false),
                    AvailableBytes = table.Column<long>(type: "bigint", nullable: false),
                    AggregateId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageSummaries", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_StorageSummaries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StorageDetails_EntityType_EntityId",
                table: "StorageDetails",
                columns: new[] { "EntityType", "EntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StorageDetails_OwnerId",
                table: "StorageDetails",
                column: "OwnerId");

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
                name: "IX_StorageSummaries_OwnerId",
                table: "StorageSummaries",
                column: "OwnerId",
                unique: true);

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
                name: "IX_StorageSummaries_Version",
                table: "StorageSummaries",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StorageDetails");

            migrationBuilder.DropTable(
                name: "StorageSummaries");
        }
    }
}
