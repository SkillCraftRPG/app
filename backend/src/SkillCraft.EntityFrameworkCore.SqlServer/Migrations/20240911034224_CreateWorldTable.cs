﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillCraft.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateWorldTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Worlds",
                columns: table => new
                {
                    WorldId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SlugNormalized = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AggregateId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Worlds", x => x.WorldId);
                    table.ForeignKey(
                        name: "FK_Worlds_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "IX_Worlds_Id",
                table: "Worlds",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_Name",
                table: "Worlds",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_OwnerId",
                table: "Worlds",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_Slug",
                table: "Worlds",
                column: "Slug");

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_SlugNormalized",
                table: "Worlds",
                column: "SlugNormalized",
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
                name: "IX_Worlds_UserId",
                table: "Worlds",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Worlds_Version",
                table: "Worlds",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Worlds");
        }
    }
}
