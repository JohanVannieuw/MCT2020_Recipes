using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Recipes_DB.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Category",
            //    columns: table => new
            //    {
            //        CategoryID = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Category = table.Column<string>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Category", x => x.CategoryID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Recipe",
            //    columns: table => new
            //    {
            //        RecipeID = table.Column<Guid>(nullable: false),
            //        RecipeName = table.Column<string>(nullable: false),
            //        RecipeDescription = table.Column<string>(nullable: true),
            //        CategoryID = table.Column<int>(nullable: false),
            //        CountryOfOrigin = table.Column<string>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Recipe", x => x.RecipeID);
            //        table.ForeignKey(
            //            name: "FK_Recipe_Category_CategoryID",
            //            column: x => x.CategoryID,
            //            principalTable: "Category",
            //            principalColumn: "CategoryID",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Category_Category",
            //    table: "Category",
            //    column: "Category",
            //    unique: true,
            //    filter: "([Category] IS NOT NULL)");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Recipe_CategoryID",
            //    table: "Recipe",
            //    column: "CategoryID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Recipe");

            migrationBuilder.DropTable(
                name: "Category");
        }
    }
}
