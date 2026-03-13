using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WHMS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DeliveryItems",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "DeliveryItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "DeliveryItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "DeliveryItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "DeliveryItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "DeliveryItems",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedById",
                table: "DeliveryItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByName",
                table: "DeliveryItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserName",
                table: "DeliveryItems",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DeliveryItems");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "DeliveryItems");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "DeliveryItems");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "DeliveryItems");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "DeliveryItems");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "DeliveryItems");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "DeliveryItems");

            migrationBuilder.DropColumn(
                name: "UpdatedByName",
                table: "DeliveryItems");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserName",
                table: "DeliveryItems");
        }
    }
}
