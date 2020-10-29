using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Recipes_DB.Helpers;
using Recipes_DB.Hubs;
using Recipes_DB.Models;
using Recipes_DB.Repositories;
using Serilog;

namespace Recipes_DB.Controllers
{

    [Route("api/[controller]")]
    //https://localhost:44390/api/categories?api-version=2.0

    // https://localhost:44390/api/2.0/categories  via Route:
    //[Route("api/{version:apiVersion}/[controller]")] 
    [ApiController]
    [ApiVersion("1.0")]
    [Consumes("application/json", "application/json-path+json", "multipart/form-data", "application/form-data")]
    [Produces("application/json")]


    public class CategoriesController : ControllerBase
    {
        private readonly IGenericRepo<Category> genericRepo;
        private readonly IRecipeRepo genericRecipeRepo;
        private readonly IMapper mapper;
        private readonly ILogger<CategoriesController> logger;
        private readonly IMemoryCache memoryCache;
        private readonly IHubContext<RepoHub> hubContext;

        public CategoriesController(IGenericRepo<Category> genericRepo, IRecipeRepo genericRecipeRepo, IMapper mapper, ILogger<CategoriesController> logger, IMemoryCache memoryCache, IHubContext<RepoHub> hubContext)
        {
            // _context = context;  //hier geen context meer bij een repo pattern.
            this.genericRepo = genericRepo;
            this.genericRecipeRepo = genericRecipeRepo;
            this.mapper = mapper;
            this.logger = logger;
            this.memoryCache = memoryCache;
            this.hubContext = hubContext;
        }

        // GET: api/Categories
        /// <summary>
        /// Haalt gerechten op en cacht in het geheugen voor 1 minuut.
        /// Demo: Stuurt een notificatie uit naar Hub bij GET Categories.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {

            IEnumerable<Category> categoriesCached;

            // if cache ? aanmaken of gebruiken
            if (!memoryCache.TryGetValue(CacheKeys.CategoriesCacheKey, out categoriesCached))
            {

                categoriesCached = await genericRepo.GetAllAsync();
                //relaties -> recipes
                foreach (Category c in categoriesCached)
                {
                    var recipes = await genericRecipeRepo.GetByExpressionAsync(r => r.CategoryId == c.Id);
                    c.Recipes = (ICollection<Recipe>)recipes;

                }

                //2. Set options 
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                 .SetSize(10)
         .SetSlidingExpiration(TimeSpan.FromMinutes(1));

                //3. Save data in cache 
                memoryCache.Set(CacheKeys.CategoriesCacheKey, categoriesCached, cacheEntryOptions);
            }
            else
            {
                categoriesCached = (ICollection<Category>)memoryCache.Get(CacheKeys.CategoriesCacheKey);
            }

            //3. mappen naar DTO
            var categoriesDTO = mapper.Map<IEnumerable<CategoryDTO>>(categoriesCached);

            //4. Notificatie met geserialiseerde categoriesDTO naar een Hub sturen (registreren) 
            await hubContext.Clients.All.SendAsync("ServerMessage", new { message = $"{JsonSerializer.Serialize(categoriesDTO)}" });
            //alternatief: methode in RepoHub oproepen

            return Ok(categoriesDTO);
        }

        // GET: api/Categories/5
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(CategoryDTO), (int)HttpStatusCode.OK)]
          [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryDTO>> GetCategoryById(int id)
        {
            var categories = await genericRepo.GetByExpressionAsync(m => m.Id == id);

            // Vergeet de count niet! categories is een collectie en nooit null
            if (categories == null || categories.Count() == 0)
            {
                return NotFound(new { message = "Categorie niet gevonden." });
                //return NotFound();
            }
            Category category = categories.FirstOrDefault<Category>();
            //deferred loading vd navigatie properties
            category.Recipes = await genericRecipeRepo.GetByExpressionAsync(r => r.CategoryId == category.Id) as ICollection<Recipe>;


            return Ok(mapper.Map<CategoryDTO>(category));
        }


        // GET: api/Categories/Name/Dessert
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CategoryDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("name/{name}", Name = "GetCategoryByName")]
        public async Task<ActionResult<CategoryDTO>> GetCategoryByName(string name)
        {
            //var category = await _context.Category.FindAsync(id);
            //Expressie returnt een collectie

            if (name == null)
            {
                return BadRequest(new { message = "Ongeldige naam ingevoerd." });
            }

            var categories = await genericRepo.GetByExpressionAsync(m => m.CategoryName.Contains(name));
            if (categories == null || categories.Count() == 0)
            {
                return NotFound(new { message = "Categorie niet gevonden." });
                //return NotFound();
            }
            Category category = categories.FirstOrDefault<Category>();

            return Ok(mapper.Map<CategoryDTO>(category));
        }


        // PUT: api/Categories/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, CategoryDTO categoryDTO)
        {
            //1. altijd null check naast supplementaire Id check
            if (categoryDTO == null || id != categoryDTO.Id) return BadRequest();

            //2. mapping 
            var category = mapper.Map<Category>(categoryDTO);
            if (category == null)
            {
                return BadRequest(new { Message = "Onvoldoende data bij de categorie" });
            }

            //3. validatie
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = $"Geen geldige input. {ModelState}" });
            }

            // _context.Entry(category).State = EntityState.Modified;

            try
            {
                  Category categorySearch = (await genericRepo.GetByExpressionAsync(c => c.Id == categoryDTO.Id)).FirstOrDefault();

                await genericRepo.Update(mapper.Map<Category>(categoryDTO),id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!genericRepo.Exists(category,id).Result)
                
                    {
                    return NotFound();
                }
                else
                {
                    return RedirectToAction("HandleErrorCode", "Error", new
                    {
                        statusCode = 400,
                        errorMessage = $"De categorie '{category.CategoryName}' werd niet aangepast."
                    });
                }
            }

            return NoContent();
        }

        // POST: api/Categories
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //alle mogelijke Statuscodes worden zo zichtbaar in Swagger:
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        public async Task<ActionResult<CategoryEditCreateDTO>> PostCategory([FromBody] [Bind("CategoryName")] CategoryEditCreateDTO categoryDTO)
        {
            if (categoryDTO == null)
            {
                return BadRequest(new { Message = "Geen categorie input" });
            };

            var category = mapper.Map<Category>(categoryDTO);
            if (category == null)
            {
                return BadRequest(new { Message = "Onvoldoende data bij de categorie" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                //_context.Category.Add(category); 
                //await _context.SaveChangesAsync();
                await genericRepo.Create(category);
                return CreatedAtAction("GetCategoryById", new { id = category.Id }, mapper.Map<CategoryEditCreateDTO>(category));
            }
            catch (Exception exc)
            {
                //Serilog
                Log.Logger.Warning(LogEventIds.InsertData.ToString() + $" - CategorieController: Exceptie bij toevoegen van {category.CategoryName}: {exc.Message}");

                //Customised gebruikers error
                return RedirectToAction("HandleErrorCode", "Error", new
                {
                    statusCode = 400,
                    errorMessage = $"Het bewaren van categorie '{category.CategoryName}' is mislukt."
                });
            }
        }

        // DELETE: api/Categories/5
        /// <summary>
        /// Verwijderen van een categorie kan enkel vanaf versie 2.0 :
        /// /api/categories/5?api-version=2.0
        /// </summary>
        [MapToApiVersion("2.0")]  
        [HttpDelete("{id}")]
        public async Task<ActionResult<CategoryDTO>> DeleteCategory(int id)
        {
            var categories = await genericRepo.GetByExpressionAsync(c => c.Id == id) ;
            if (categories == null || categories.Count() ==0)
            {
                return NotFound(new { message = "Categorie niet gevonden." });
            }

            Category category = categories.FirstOrDefault<Category>();
            try { 
                
                await genericRepo.Delete(category);
            
            } catch {
                //Customised gebruikers error
                return RedirectToAction("HandleErrorCode", "Error", new
                {
                    statusCode = 400,
                    errorMessage = $"Het verwijderen van categorie '{category.CategoryName}' is mislukt."
                });

            }

            return Ok(mapper.Map<CategoryDTO>(category));
        }

        //private bool CategoryExists(int id)
        //{
        //    return _context.Category.Any(e => e.Id == id);
        //}
    }
}
