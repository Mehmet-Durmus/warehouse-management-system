using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WHMS.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Warehouses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "Warehouses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "Warehouses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdateByName",
                table: "Warehouses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdateByUserName",
                table: "Warehouses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedById",
                table: "Warehouses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Stores",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "Stores",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "Stores",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdateByName",
                table: "Stores",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdateByUserName",
                table: "Stores",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedById",
                table: "Stores",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "SKUs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "SKUs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "SKUs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdateByName",
                table: "SKUs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdateByUserName",
                table: "SKUs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedById",
                table: "SKUs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Categories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "Categories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "Categories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdateByName",
                table: "Categories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdateByUserName",
                table: "Categories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedById",
                table: "Categories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserName",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdateByName",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UpdateByUserName",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedById",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "UpdateByName",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "UpdateByUserName",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Warehouses");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "UpdateByName",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "UpdateByUserName",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "SKUs");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "SKUs");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "SKUs");

            migrationBuilder.DropColumn(
                name: "UpdateByName",
                table: "SKUs");

            migrationBuilder.DropColumn(
                name: "UpdateByUserName",
                table: "SKUs");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "SKUs");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UpdateByName",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UpdateByUserName",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedByUserName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UpdateByName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UpdateByUserName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "AspNetUsers");
        }
    }
}
