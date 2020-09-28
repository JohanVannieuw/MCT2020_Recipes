using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recipes_DB.Models;

namespace Recipes_DB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipesController : ControllerBase
    {
        private readonly Recipes_DB1Context _context;
        private readonly IMapper mapper;
        private readonly IGenericRepo<Category> categoryRepo;
        private readonly IGenericRepo<Recipe> recipeRepo;

        //TODO: RecipeController ombouwen naar repo pattern volgens het IoC patroon. Het genersiche repo krijgt een new AddRecipe methode.
        public RecipesController(Recipes_DB1Context context, IMapper mapper, IGenericRepo<Category> categoryRepo, IGenericRepo<Recipe> recipeRepo)
        {
            _context = context;
            this.mapper = mapper;
            this.categoryRepo = categoryRepo;
            this.recipeRepo = recipeRepo;
        }

        // GET: api/Recipes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipe()
        {
            return await _context.Recipe.ToListAsync();
        }

        // GET: api/Recipes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> GetRecipe(Guid id)
        {
            var recipe = await _context.Recipe.FindAsync(id);

            if (recipe == null)
            {
                return NotFound();
            }

            return recipe;
        }

        // PUT: api/Recipes/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecipe(Guid id, Recipe recipe)
        {
            if (id != recipe.Id)
            {
                return BadRequest();
            }

            _context.Entry(recipe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeExists(id))
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

        [HttpPatch("{name}", Name = "PartUpdateApp")]
        public async Task<IActionResult> PartiallyUpdateRecipe(string name,
                        [FromBody] JsonPatchDocument<RecipeDTO> patchDoc)
        {
            //1. validaties en app via mapping ophalen 
            if ((patchDoc == null) || (name == null) || (name == ""))
                { return BadRequest(); }
            var recipes = (await recipeRepo.GetByExpressionAsync(r =>
                            r.RecipeName.Contains(name)));
            if (recipes == null || recipes.Count() == 0)
            {
                return NotFound();
            }
            if (!ModelState.IsValid) return BadRequest(ModelState);

            //2. RecipeDTO ophalen
            Recipe recipe = recipes.First();
            var recipeDTOToPatch = mapper.Map<RecipeDTO>(recipe); //map naar DTO
           //TODO: niet voorzien in Mapper (optioneel)
            recipe.Category = (await categoryRepo.GetByExpressionAsync(c => c.Id == recipe.CategoryId)).First();
            var tempId = recipe.CategoryId; //alleen indien verdwenen door mapping

            try
            {
                //2. Patch uitvoeren op het JsonDoc
                patchDoc.ApplyTo(recipeDTOToPatch); //PATCH TOEPASSEN OP DTO 
                recipe = mapper.Map<Recipe>(recipeDTOToPatch);
                recipe.CategoryId = tempId; //indien niet in mapper

                await recipeRepo.Update(recipe, recipe.Id); //generic repo
                                                        //await recipeRepo.SaveAsync(); //indien niet in repo
            }
            catch (Exception exc)
            {
                //TODO: exceptie 
                throw new Exception($"Patchupdate  of {recipe.RecipeName} failed.              {exc.InnerException.Message}");
               }

            return NoContent();
        }




        // POST: api/Recipes
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Recipe>> PostRecipe(Recipe recipe)
        {
            _context.Recipe.Add(recipe);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RecipeExists(recipe.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRecipe", new { id = recipe.Id }, recipe);
        }

        // DELETE: api/Recipes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Recipe>> DeleteRecipe(Guid id)
        {
            var recipe = await _context.Recipe.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }

            _context.Recipe.Remove(recipe);
            await _context.SaveChangesAsync();

            return recipe;
        }

        private bool RecipeExists(Guid id)
        {
            return _context.Recipe.Any(e => e.Id == id);
        }
    }
}
