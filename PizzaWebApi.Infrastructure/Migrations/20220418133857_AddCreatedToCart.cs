using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PizzaWebApi.Infrastructure.Migrations
{
    public partial class AddCreatedToCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Carts",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "Carts");
        }
    }
}
