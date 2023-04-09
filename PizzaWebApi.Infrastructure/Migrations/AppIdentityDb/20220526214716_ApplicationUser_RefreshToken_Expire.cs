using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PizzaWebApi.Infrastructure.Migrations.AppIdentityDb
{
    public partial class ApplicationUser_RefreshToken_Expire : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Expire",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Expire",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");
        }
    }
}
