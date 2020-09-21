using Microsoft.EntityFrameworkCore.Migrations;

namespace Recipes_DB.Migrations
{
    public partial class AdaptedColumnNameInDatabase_recipe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RecipeName",
                table: "Recipe",
                newName: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Recipe",
                newName: "RecipeName");
        }
    }
}
