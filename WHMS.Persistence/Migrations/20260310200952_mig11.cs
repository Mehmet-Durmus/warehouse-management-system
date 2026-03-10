using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WHMS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_WarehouseId",
                table: "Deliveries",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deliveries_Warehouses_WarehouseId",
                table: "Deliveries",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deliveries_Warehouses_WarehouseId",
                table: "Deliveries");

            migrationBuilder.DropIndex(
                name: "IX_Deliveries_WarehouseId",
                table: "Deliveries");
        }
    }
}
