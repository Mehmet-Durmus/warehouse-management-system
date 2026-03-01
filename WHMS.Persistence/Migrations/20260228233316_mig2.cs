using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WHMS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "WarehouseManagerSequence");

            migrationBuilder.CreateSequence<int>(
                name: "WarehouseStaffSequence");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "WarehouseManagerSequence");

            migrationBuilder.DropSequence(
                name: "WarehouseStaffSequence");
        }
    }
}
