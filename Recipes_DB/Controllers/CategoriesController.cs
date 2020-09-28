using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Recipes_DB.Models;
using Serilog;

namespace Recipes_DB.Controllers
{
    //TODO: CategoryController: PUT en DELETE context niet meer gebruiken , overal DTO's gebruiken 



    //https://localhost:44390/api/categories?api-version=2.0
    [Route("api/[controller]")]
    // https://localhost:44390/api/2.0/categories  via Route:
    //[Route("api/{version:apiVersion}/[controller]")] 
    [ApiController]
    [ApiVersion("1.0")]
    public class CategoriesController : ControllerBase
    {
        private readonly Recipes_DB1Context _context;
        private readonly IGenericRepo<Category> genericRepo;
        private readonly IGenericRepo<Recipe> genericRecipeRepo;
        private readonly IMapper mapper;
        private readonly ILogger<CategoriesController> logger;

        public CategoriesController(Recipes_DB1Context context, IGenericRepo<Category> genericRepo, IGenericRepo<Recipe> genericRecipeRepo, IMapper mapper, ILogger<CategoriesController> logger)
        {
            _context = context;
            this.genericRepo = genericRepo;
            this.genericRecipeRepo = genericRecipeRepo;
            this.mapper = mapper;
            this.logger = logger;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategory()
        {
            var categories = await genericRepo.GetAllAsync();
            //relaties -> recipes
            foreach (Category c in categories)
            {
                var recipes = await genericRecipeRepo.GetByExpressionAsync(r => r.CategoryId == c.Id);
                c.Recipes = (ICollection<Recipe>)recipes;

            }

            var categoriesDTO = mapper.Map<IEnumerable<CategoryDTO>>(categories);
            
             return Ok(categoriesDTO);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            var categories = await genericRepo.GetByExpressionAsync(m => m.Id == id);
           
            // Vergeet de count niet! categories is een collectie en nooit null
            if (categories == null || categories.Count() == 0)
            {
                return NotFound(new { message = $"Product {id} not found" });
                //return NotFound();
            }
            Category category = categories.FirstOrDefault<Category>();
            //deferred loading vd navigatie properties
            category.Recipes = await genericRecipeRepo.GetByExpressionAsync(r => r.CategoryId == category.Id) as ICollection<Recipe>;


            return Ok(mapper.Map<CategoryDTO>(category));
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Categories
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<CategoryEditCreateDTO>> PostCategory(
            [FromBody] [Bind("Id","CategoryName")]        
            CategoryEditCreateDTO categoryDTO)
        {
            if (categoryDTO == null) { 
                return BadRequest(new { Message = "Geen categorie input" }); 
            };
         
            var category = mapper.Map<Category>(categoryDTO);
            if (category == null)
            {
                return BadRequest(new { Message = "Onvoldoende data bij de categorie" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = $"Geen geldige input. {ModelState}" });
            }

            try
            {
                //_context.Category.Add(category); 
                //await _context.SaveChangesAsync();
                await genericRepo.Create(category);

                return CreatedAtAction("GetCategory", new { id = category.Id }, mapper.Map<CategoryEditCreateDTO>(category));
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
        [MapToApiVersion("2.0")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Category>> DeleteCategory(int id)
        {
            var category = await _context.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Category.Remove(category);
            await _context.SaveChangesAsync();

            return category;
        }

        private bool CategoryExists(int id)
        {
            return _context.Category.Any(e => e.Id == id);
        }
    }
}
