using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Recipes_DB.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RecipesDB_UnitTests.Repositories
{
    
    [TestClass]
    public class CategoryRepoTests : DatabaseTestDB
    {

        private readonly Recipes_DB1Context context;
        private DbSet<Category> categoriesList;
        private DbSet<Recipe> recipesList;
        private readonly Category fakeNewCategory;

        public CategoryRepoTests()
        {
            context = Context;
            categoriesList = Context.Category;
            recipesList = Context.Recipe;

            fakeNewCategory = new Category
            {
                Id = 902,
                CategoryName = "FakeCategory2",
                Recipes = new[] {
             new Recipe{Id= Guid.NewGuid(), RecipeName="FakeCat2  Recipe1"  },
             new Recipe{Id= Guid.NewGuid(), RecipeName="FakeCat2  Recipe2"  }
            }
            };
        }


        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task CreateCategory_WhenExceptionOccurs_ThrowsException()
        {
            //1.Assign
            var catRepo = new GenericRepo<Category>(context);

            //2.Act: toevoegen van een bestaande categorie
            var nullCat = await catRepo.Create(new Category
            { Id = 1 });

            //3. Alternatief op dataannotatie met zelfde resultaat via Assert 
            await Assert.ThrowsExceptionAsync<Exception>(() => catRepo.Create(new
              Category
            { Id = 1 }));

        }



    }

}
