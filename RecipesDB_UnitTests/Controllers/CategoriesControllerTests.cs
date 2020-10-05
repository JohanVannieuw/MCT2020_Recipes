using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Recipes_DB;
using Recipes_DB.Controllers;
using Recipes_DB.Models;
using Recipes_DB.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipesDB_UnitTests.Controllers
{
    [TestClass()]
    public class CategoriesControllerTests : DatabaseTestDB
    {

        private readonly Recipes_DB1Context context;
        private DbSet<Category> categoriesList;
        private DbSet<Recipe> recipesList;
        private IMapper mapper; //concrete mapper
        private IMemoryCache memoryCache;

        //  public async Task GetFakeStudents() { . . . . }
        public CategoriesControllerTests()
        {
            context = Context; //property dependancy
            categoriesList = Context.Category;
            recipesList = Context.Recipe;

            //1. Concrete mapper
            var mapperConfig = new MapperConfiguration(opts =>
            {
                opts.AddProfile(new Recipes_DB.Models.Recipes_DBProfiles());
            });
            this.mapper = mapperConfig.CreateMapper(); //concrete mapper

            //2. Concrete MemoryCache

            //02. concrete memoryCache
            //ofwel de caching vd controller gebruiken,
            //ofwel een (mock)waarde invullen in de test - arrange  
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();

            this.memoryCache = serviceProvider.GetService<IMemoryCache>();












        }

        [TestMethod()]
        public async Task GetCategoriesIndex_ReturnsAllCategories()
        {
            //1.ASSIGN
            var mockRepo = new Mock<IRecipeRepo>();
            var mockCategoryRepo = new Mock<IGenericRepo<Category>>();

            //concrete mapper gebruiken
            //async functies verwacht ReturnsAsync
            mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(recipesList);
            mockCategoryRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(categoriesList);

            var APIcontroller = new CategoriesController(null,mockCategoryRepo.Object, mockRepo.Object, mapper, null, memoryCache);  //alle args en ‘.Object’ niet vergeten

            //2. ACT met await
            var actionResult = await APIcontroller.GetCategories(); //actienaam..
            var okResult = actionResult.Result as OkObjectResult; //Statuscodes
            IEnumerable<Category> Tasks = okResult.Value as IEnumerable<Category>;

            //3. ASSERT test altijd op : null - datatypes – inhoud - statuscode
            Assert.IsNotNull(okResult);

            Assert.IsInstanceOfType(okResult, typeof(OkObjectResult));
            Assert.IsInstanceOfType(okResult.Value, typeof(IEnumerable<CategoryDTO>));

            List<CategoryDTO> lst = okResult.Value as List<CategoryDTO>;
            Assert.IsTrue(lst.Count() == 6);  // Testdatabase!

            Assert.AreEqual(200, okResult.StatusCode);

        }

    }

}
