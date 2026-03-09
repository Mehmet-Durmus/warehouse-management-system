using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WHMS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig_fix_typo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdateByUserName",
                table: "Warehouses",
                newName: "UpdatedByUserName");

            migrationBuilder.RenameColumn(
                name: "UpdateByName",
                table: "Warehouses",
                newName: "UpdatedByName");

            migrationBuilder.RenameColumn(
                name: "UpdateByUserName",
                table: "Stores",
                newName: "UpdatedByUserName");

            migrationBuilder.RenameColumn(
                name: "UpdateByName",
                table: "Stores",
                newName: "UpdatedByName");

            migrationBuilder.RenameColumn(
                name: "UpdateByUserName",
                table: "SKUs",
                newName: "UpdatedByUserName");

            migrationBuilder.RenameColumn(
                name: "UpdateByName",
                table: "SKUs",
                newName: "UpdatedByName");

            migrationBuilder.RenameColumn(
                name: "UpdateByUserName",
                table: "Categories",
                newName: "UpdatedByUserName");

            migrationBuilder.RenameColumn(
                name: "UpdateByName",
                table: "Categories",
                newName: "UpdatedByName");

            migrationBuilder.RenameColumn(
                name: "UpdateByUserName",
                table: "AspNetUsers",
                newName: "UpdatedByUserName");

            migrationBuilder.RenameColumn(
                name: "UpdateByName",
                table: "AspNetUsers",
                newName: "UpdatedByName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedByUserName",
                table: "Warehouses",
                newName: "UpdateByUserName");

            migrationBuilder.RenameColumn(
                name: "UpdatedByName",
                table: "Warehouses",
                newName: "UpdateByName");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserName",
                table: "Stores",
                newName: "UpdateByUserName");

            migrationBuilder.RenameColumn(
                name: "UpdatedByName",
                table: "Stores",
                newName: "UpdateByName");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserName",
                table: "SKUs",
                newName: "UpdateByUserName");

            migrationBuilder.RenameColumn(
                name: "UpdatedByName",
                table: "SKUs",
                newName: "UpdateByName");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserName",
                table: "Categories",
                newName: "UpdateByUserName");

            migrationBuilder.RenameColumn(
                name: "UpdatedByName",
                table: "Categories",
                newName: "UpdateByName");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserName",
                table: "AspNetUsers",
                newName: "UpdateByUserName");

            migrationBuilder.RenameColumn(
                name: "UpdatedByName",
                table: "AspNetUsers",
                newName: "UpdateByName");
        }
    }
}
