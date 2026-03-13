using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WHMS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedArrivalDate",
                table: "Deliveries",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpectedArrivalDate",
                table: "Deliveries");
        }
    }
}
