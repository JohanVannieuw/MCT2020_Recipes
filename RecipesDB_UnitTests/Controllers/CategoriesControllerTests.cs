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
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RecipesDB_UnitTests.Controllers
{
    [TestClass()]
    public class CategoriesControllerTests : DatabaseTestDB
    {

        //private readonly Recipes_DB1Context context;
        private DbSet<Category> categoriesList;
        private DbSet<Recipe> recipesList;
        private IMapper mapper; //concrete mapper
        private IMemoryCache memoryCache;
        private Mock<IRecipeRepo> mockRepo;
        private Mock<IGenericRepo<Category>> mockCategoryRepo;

        //  public async Task GetFakeStudents() { . . . . }
        public CategoriesControllerTests()
        {
            //context = Context; //property dependancy
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


        [TestInitialize]
        public void TestInitialize()
        {
            mockRepo = new Mock<IRecipeRepo>();
            mockCategoryRepo = new Mock<IGenericRepo<Category>>();
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

            var APIcontroller = new CategoriesController(mockCategoryRepo.Object, mockRepo.Object, mapper, null, memoryCache);  //alle args en ‘.Object’ niet vergeten

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


        [TestMethod]
        public async Task GetCategoryForId_ReturnsCategory_WhenIdExists()
        {
            //ARRANGE
            int testId = 6; //Id null kan dus niet
            var fakeCategory6 = categoriesList.FirstOrDefault(c => c.Id == testId);

            mockCategoryRepo.Setup(repo => repo.GetByExpressionAsync(It.IsAny<Expression<Func<Category, bool>>>())).ReturnsAsync(categoriesList.AsQueryable().Where(c => c.Id == testId));

            var APIcontroller = new CategoriesController( mockCategoryRepo.Object, mockRepo.Object, mapper, null, memoryCache);  //alle args en ‘.Object’ niet vergeten

            //ACT
            var actionResult = await APIcontroller.GetCategoryById(testId);
            var okResult = actionResult.Result as OkObjectResult;

            //ASSERT: NULL - TYPES - DATA -STATUSCODE
            Assert.IsNotNull(okResult); //null
            Assert.IsInstanceOfType(okResult, typeof(OkObjectResult)); //types
            Assert.IsInstanceOfType(okResult.Value, typeof(CategoryDTO));
            CategoryDTO categoryDTO = okResult.Value as CategoryDTO; //data
            Assert.AreEqual(200, okResult.StatusCode); //statuscode

        }

        [TestMethod]
        public async Task GetCategoryForId_ReturnsCategory_WhenIdDoesNotExists() {
            // ARRANGE 
            int testId = 0;
            var fakeCategory6 = categoriesList.FirstOrDefault(c => c.Id == testId);

            mockCategoryRepo.Setup(repo => repo.GetByExpressionAsync(It.IsAny<Expression<Func<Category, bool>>>())).ReturnsAsync(categoriesList.AsQueryable().Where(c => c.Id == testId));

            var APIcontroller = new CategoriesController(mockCategoryRepo.Object, mockRepo.Object, mapper, null, memoryCache);  //alle args en ‘.Object’ niet vergeten

            //ACT
            var actionResult = await APIcontroller.GetCategoryById(testId);
            var notFoundResult = actionResult.Result as NotFoundObjectResult;
            //ASSERT (4 DINGSKES !!!! )
            Assert.IsNotNull(notFoundResult);
            Assert.IsTrue((notFoundResult.Value.GetType()).Name.Contains("AnonymousType"));
            //data: customised message als json
            Assert.AreEqual(notFoundResult.Value.ToString(), "{ message = Categorie niet gevonden. }");
            Assert.AreEqual(404, notFoundResult.StatusCode); //statuscode
        }

           [TestMethod]
        public async Task GetCategoryForName_ReturnsBadRequest_WhenNameIsNull() {
            //ASSIGN 
            string testName = null;

            mockCategoryRepo.Setup(repo => repo.GetByExpressionAsync(It.IsAny<Expression<Func<Category, bool>>>())).ReturnsAsync(categoriesList.AsQueryable().Where(c => c.CategoryName == testName));

            var APIcontroller = new CategoriesController( mockCategoryRepo.Object, mockRepo.Object, mapper, null, memoryCache);  //alle args en ‘.Object’ niet vergeten

            //ACT 
            var actionResult = await APIcontroller.GetCategoryByName(testName);
            var badRequestResult = actionResult.Result as BadRequestObjectResult;

            //ASSERT 
            Assert.IsNotNull(badRequestResult);
            Assert.IsTrue((badRequestResult.Value.GetType()).Name.Contains("AnonymousType"));
            Assert.AreEqual(badRequestResult.Value.ToString(), "{ message = Ongeldige naam ingevoerd. }");
            Assert.AreEqual(400, badRequestResult.StatusCode);
        }


        [TestMethod]
        public async Task PostCategory_Returns_Category_WhenModelIsValid()
        {
            //ARRANGE
            //1. User input
            var newCategory = new CategoryEditCreateDTO
            {
                CategoryName = "een nieuwe categorie"
               // Id = 10000
            };
            //2. Mocking van controller en alle controller methodes ()
            mockCategoryRepo.Setup(repo => repo.Create(It.IsAny<Category>()))
            .Returns(Task.FromResult(mapper.Map<CategoryEditCreateDTO, Category>(newCategory)));

            var APIcontroller = new CategoriesController(mockCategoryRepo.Object, mockRepo.Object, mapper, null, memoryCache);  //alle args en ‘.Object’ niet vergeten

            //3. controleer of mockRepo opgeroepen werd;
            mockCategoryRepo.Verify();

            //ACT : controller oproepen 
            var actionResult = await APIcontroller.PostCategory(newCategory);
            var createdResult = (CreatedAtActionResult)actionResult.Result ;

            //ASSERT 
            Assert.IsNotNull(createdResult); //null
            Assert.IsInstanceOfType(createdResult, typeof(CreatedAtActionResult)); //type
            Assert.IsInstanceOfType(createdResult.Value, typeof(CategoryEditCreateDTO));
          Assert.AreEqual(((CategoryEditCreateDTO)createdResult.Value).CategoryName, "een nieuwe categorie"); //data
            Assert.AreEqual("GetCategoryById", createdResult.ActionName);
            Assert.AreEqual(201, createdResult.StatusCode);//statuscode
          }

        [TestMethod]
        public async Task PostCategory_ReturnsBadRequestResult400_WhenModelStateIsInvalid()
        {
            //ARRANGE:  Alle methodes opgeroepen in de controller, moeten gemocked worden .

            //1. Aanmaak nieuwe DTO onnodig -> model voert validatie uit 

            //2. Mocking van controller en alle controller methodes ()
            mockCategoryRepo.Setup(repo => repo.Create(It.IsAny<Category>()))
            //.Returns(Task.FromResult(mapper.ConvertTo_Entity(newTask, ref newTaskEntity))) //cutomised mapper
            .Returns(Task.FromResult(mapper.Map<CategoryEditCreateDTO, Category>(new CategoryEditCreateDTO())));
            var APIcontroller = new CategoriesController( mockCategoryRepo.Object, mockRepo.Object, mapper, null, memoryCache);

            //3. controleer of mockRepo opgeroepen werd;
            mockCategoryRepo.Verify();

            //ACT : controller oproepen met model error (must)
            APIcontroller.ModelState.AddModelError("CategoryName", "Required");

            var actionResult = await APIcontroller.PostCategory(new CategoryEditCreateDTO());
            var badReqObjResult = (BadRequestObjectResult)actionResult.Result;
            //BAdRequest(ModelState) kan geserializeerd worden.
           SerializableError badReqError = badReqObjResult.Value as SerializableError;
            //var JsonString = JsonConvert.SerializeObject(badReqError);

            //ASSERT 
            Assert.IsInstanceOfType(badReqObjResult, typeof(BadRequestObjectResult));
            Assert.IsInstanceOfType(((BadRequestObjectResult)badReqObjResult).Value, typeof(SerializableError));
            Assert.AreEqual(400, badReqObjResult.StatusCode);
            //omzetten naar string[] om index te kunnen gebruiken
            Assert.AreEqual(((string[])badReqError["CategoryName"])[0], "Required");
            //alternatief: for lus
            foreach (var error in badReqError)
            {
                if (error.Key == "CategoryName")
                {
                    Assert.AreEqual(((string[])error.Value)[0], "Required");
                }
            }
        }

        [TestMethod]
        public async Task PostCategory_ReturnsFromErrorController400_WhenException() {
            //ARRANGE
            //1. User input
            var newCategory2 = new CategoryEditCreateDTO
            {
                CategoryName = "een nieuwe categorie"  //moet een fout veroozaken
                // Id = 10000
            };
            //2. Mocking van controller en alle controller methodes ()
           // .Returns(Task.FromResult(mapper.Map<CategoryEditCreateDTO, Category>(newCategory2)));
            mockCategoryRepo.Setup(repo => repo.Create(It.IsAny<Category>())).Throws(new Exception());

            var APIcontroller = new CategoriesController(mockCategoryRepo.Object, mockRepo.Object, mapper, null, memoryCache);  //alle args en ‘.Object’ niet vergeten

            //3. controleer of mockRepo opgeroepen werd;
            mockCategoryRepo.Verify();

            //ACT : controller oproepen 
            var actionResult = await APIcontroller.PostCategory(newCategory2);
            var redirectToActionResult = (RedirectToActionResult)actionResult.Result;

            //ASSERT:
            Assert.IsNotNull(redirectToActionResult);
            Assert.AreEqual(redirectToActionResult.ActionName, "HandleErrorCode");
            Assert.AreEqual(redirectToActionResult.ControllerName, "Error");
            Assert.IsTrue(redirectToActionResult.RouteValues["ErrorMessage"].ToString().Contains("is mislukt"));


        }

    }

}
