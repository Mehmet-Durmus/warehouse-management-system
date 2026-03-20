using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WHMS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WasteRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkuId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Desctiption = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByName = table.Column<string>(type: "text", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedByName = table.Column<string>(type: "text", nullable: true),
                    UpdatedByUserName = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WasteRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WasteRecords_SKUs_SkuId",
                        column: x => x.SkuId,
                        principalTable: "SKUs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCountLines_InventoryCountId",
                table: "InventoryCountLines",
                column: "InventoryCountId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCountLines_SkuId",
                table: "InventoryCountLines",
                column: "SkuId");

            migrationBuilder.CreateIndex(
                name: "IX_WasteRecords_SkuId",
                table: "WasteRecords",
                column: "SkuId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryCountLines_InventoryCounts_InventoryCountId",
                table: "InventoryCountLines",
                column: "InventoryCountId",
                principalTable: "InventoryCounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryCountLines_SKUs_SkuId",
                table: "InventoryCountLines",
                column: "SkuId",
                principalTable: "SKUs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryCountLines_InventoryCounts_InventoryCountId",
                table: "InventoryCountLines");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryCountLines_SKUs_SkuId",
                table: "InventoryCountLines");

            migrationBuilder.DropTable(
                name: "WasteRecords");

            migrationBuilder.DropIndex(
                name: "IX_InventoryCountLines_InventoryCountId",
                table: "InventoryCountLines");

            migrationBuilder.DropIndex(
                name: "IX_InventoryCountLines_SkuId",
                table: "InventoryCountLines");
        }
    }
}
