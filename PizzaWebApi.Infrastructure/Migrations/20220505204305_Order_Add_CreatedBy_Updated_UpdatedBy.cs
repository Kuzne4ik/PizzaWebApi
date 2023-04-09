using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PizzaWebApi.Infrastructure.Migrations
{
    public partial class Order_Add_CreatedBy_Updated_UpdatedBy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "Orders",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "Orders",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Orders");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");
        }
    }
}
