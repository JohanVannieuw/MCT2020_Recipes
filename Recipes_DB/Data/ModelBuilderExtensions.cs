using Microsoft.EntityFrameworkCore;
using Recipes_DB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes_DB.Data
{
    public static class ModelBuilderExtensions
    {
        public static Recipes_DB1Context _context { get; set; }

        public static void Seed(this ModelBuilder modelBuilder)
        {
            Console.WriteLine("Seeding Table Category and Recipe");
            modelBuilder.Entity<Category>().HasData(_categoriesData);
            modelBuilder.Entity<Recipe>().HasData(_recipesData);
        }

        //Static Testdata
        private readonly static List<Recipe> _recipesData = new List<Recipe> {
         new Recipe
           {
               Id = Guid.NewGuid(),
               RecipeName = "Artichoke Stuffed Mushrooms",
               RecipeDescription = "These mushrooms are a combination of my favorite things. They are a hit among all crowds. There are never any left!",
               CategoryId = 1,
               TimeToPrepare = TimeSpan.Parse("0:25:0"),
               Vegetarian = true
           },   new Recipe
           {
               Id = Guid.NewGuid(),
               RecipeName = "Buffalo Chicken Dip",
               RecipeDescription = "This tangy, creamy dip tastes just like Buffalo chicken wings. It's best served hot with crackers and celery sticks. Everyone loves the results!",
               CategoryId = 1,
               TimeToPrepare = TimeSpan.Parse("0:45:0"),
               Vegetarian = true
           },
           new Recipe
           {
               Id = Guid.NewGuid(),
               RecipeName = "Gazpacho",
               RecipeDescription = "The perfect hot-weather soup",
               CategoryId = 2,
               TimeToPrepare = TimeSpan.Parse("0:15:0"),
               Vegetarian = true
           },
           new Recipe
           {
               Id = Guid.NewGuid(),
               RecipeName = "Savory Garlic Marinated Steaks",
               RecipeDescription = "This beautiful marinade adds an exquisite flavor to these already tender steaks. The final result will be so tender and juicy, it will melt in your mouth.",
               CategoryId = 3,
               TimeToPrepare = TimeSpan.Parse("23:30:0"),
               Vegetarian = false
           },
           new Recipe
           {
               Id = Guid.NewGuid(),
               RecipeName = "Chicken Fried Chicken",
               RecipeDescription = "A fun chicken recipe the kids can help prepare. They love crushing the crackers. It does not matter if the measurements aren't perfect, just wing it!",
               //CategoryID = context.Category.First(c => c.CategoryName == "Meat").CategoryID,
              CategoryId = 3,
               TimeToPrepare = TimeSpan.Parse("0:45:0"),
               Vegetarian = false
           },
           new Recipe
           {
               Id = Guid.NewGuid(),
               RecipeName = "Grilled Fish Steaks",
               RecipeDescription = "Make it with halibit. It's simple and delicious.",
               CategoryId = 4,
               TimeToPrepare = TimeSpan.Parse("1:30:0"),
               Vegetarian = true
           },
           new Recipe
           {
              Id = Guid.NewGuid(),
               RecipeName = "Desert Crepes",
               RecipeDescription = "Essential crepe recipe. Sprinkle warm crepes with sugar and lemon, or serve with cream or ice cream and fruit.",
               CategoryId = 6,
               TimeToPrepare = TimeSpan.Parse("0:20:0"),
               Vegetarian = true
           },
          new Recipe
          {
              Id = Guid.NewGuid(),
              RecipeName = "Desert Cake",
              RecipeDescription = "A real English cake.",
              CategoryId = 6,
              TimeToPrepare = TimeSpan.Parse("2:30:0"),
              Vegetarian = true
          }

        };
        private readonly static List<Category> _categoriesData = new List<Category> {
             new Category
                {
                    Id = 1,
                    CategoryName = "Appetizers"
                },
                new Category
                {
                     Id  = 2,
                    CategoryName = "Soup"
                },
                new Category
                {
                    Id  = 3,
                    CategoryName = "Meat"
                },
                new Category
                {
                     Id  = 4,
                    CategoryName = "Fisch"
                },
                new Category
                {
                    Id  = 5,
                    CategoryName = "Vegetables"
                },
                new Category
                {
                     Id  = 6,
                    CategoryName = "Dessert"
                }
        };
    }
}