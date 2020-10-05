using Recipes_DB.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RecipesDB_UnitTests
{
    public class DatabaseInitializer
    {
        public static void Initialize(Recipes_DB1Context context)
        {
            Seed(context); //testen met (extra) FakeData
        }

        private static void Seed(Recipes_DB1Context context)
        {
            //eventueel met if voorwaarden opleggen voor aanmaken fakeCategories
            //bemerk gestructureerde namen -> mogelijks veel data via een for lus
            var fakeCategories = new[]
            {
        new Category
        {
            Id = 901,
            CategoryName = "FakeCategory1",
            Recipes = new[] {
              new Recipe{Id= Guid.NewGuid(), RecipeName="FakeRecipe1.1"},
                new Recipe{Id= Guid.NewGuid(), RecipeName="FakeRecipe1.2"  }
            }
         },
        new Category
        {
            Id = 902,
            CategoryName = "FakeCategory2",
            Recipes = new[] {
               new Recipe{Id= Guid.NewGuid(), RecipeName="FakeRecipe2.1"  },
               new Recipe{Id= Guid.NewGuid(), RecipeName="FakeRecipe2.2"  },
               new Recipe{Id= Guid.NewGuid(), RecipeName="FakeRecipe2.3"  },
               new Recipe{Id= Guid.NewGuid(), RecipeName="FakeRecipe2.4"  },
            }
         },
         new Category
         {
             Id = 903,
             CategoryName = "FakeCategory3",
             Recipes = new[] {
               new Recipe{Id= Guid.NewGuid(), RecipeName="FakeRecipe3.1" }
             }
         },
      };
            context.Category.AddRange(fakeCategories);
            context.SaveChanges();

            //TODO: fakeRecipes aanmaken.
        }

    }

}
