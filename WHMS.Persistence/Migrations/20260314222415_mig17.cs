using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WHMS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig17 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "SendingDate",
                table: "Shipments",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_StoreId",
                table: "Shipments",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_WarehouseId",
                table: "Shipments",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_Stores_StoreId",
                table: "Shipments",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_Warehouses_WarehouseId",
                table: "Shipments",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_Stores_StoreId",
                table: "Shipments");

            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_Warehouses_WarehouseId",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_StoreId",
                table: "Shipments");

            migrationBuilder.DropIndex(
                name: "IX_Shipments_WarehouseId",
                table: "Shipments");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SendingDate",
                table: "Shipments",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);
        }
    }
}
