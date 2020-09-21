using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Recipes_DB.Migrations
{
    public partial class PropertiesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "TimeToPrepare",
                table: "Recipe",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<bool>(
                name: "Vegetarian",
                table: "Recipe",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeToPrepare",
                table: "Recipe");

            migrationBuilder.DropColumn(
                name: "Vegetarian",
                table: "Recipe");
        }
    }
}
