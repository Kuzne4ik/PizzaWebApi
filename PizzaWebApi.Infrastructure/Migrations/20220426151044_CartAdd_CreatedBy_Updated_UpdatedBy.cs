using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PizzaWebApi.Infrastructure.Migrations
{
    public partial class CartAdd_CreatedBy_Updated_UpdatedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "Carts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "Carts",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Carts");
        }
    }
}
