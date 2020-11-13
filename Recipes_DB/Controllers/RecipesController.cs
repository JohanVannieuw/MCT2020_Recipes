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
using Recipes_DB.Repositories;

namespace Recipes_DB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json", "application/json-path+json", "multipart/form-data", "application/form-data")]
    [Produces("application/json")]
    public class RecipesController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IGenericRepo<Category> categoryRepo;
        private readonly IRecipeRepo recipeRepo;

        public RecipesController(IMapper mapper, IGenericRepo<Category> categoryRepo, IRecipeRepo recipeRepo)
        {
            this.mapper = mapper;
            this.categoryRepo = categoryRepo;
            this.recipeRepo = recipeRepo;
        }

        // GET: api/Recipes
        /// <summary>
        /// Haalt gerechten op en cacht zijn response voor 30 seconden.
        /// </summary>
        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<ActionResult<IEnumerable<RecipeDTO>>> GetRecipes()
        {
            //return await _context.Recipe.ToListAsync();
            var recipes = await recipeRepo.GetAllAsync();
            foreach (Recipe r in recipes)
            {
                var categories = await categoryRepo.GetByExpressionAsync(c => c.Id == r.CategoryId);
                r.Category = categories.First();
            }

            var recipesDTO = mapper.Map<IEnumerable<RecipeDTO>>(recipes);
            return Ok(recipesDTO);
        }

        // GET: api/Recipes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDTO>> GetRecipe(Guid id)
        {
            //var recipe = await _context.Recipe.FindAsync(id);
            var recipes = await (recipeRepo.GetByExpressionAsync(r => r.Id == id));
            Recipe recipe = recipes.First(); //collecties 
            var recipeCategories = await categoryRepo.GetByExpressionAsync(cat => cat.Id == recipe.CategoryId);
            recipe.Category = recipeCategories.First(); //nodig in mapper (lazy loading)

            if (recipe == null)
            {
                return NotFound();
            }

            var recipeDTO = mapper.Map<RecipeDTO>(recipe);
            return Ok(recipeDTO);
        }

        // PUT: api/Recipes/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRecipe(Guid id, RecipeDTO recipeDTO)
        {
            //1. checks : null , ids, exists, valid
            if (recipeDTO == null || id == null) return BadRequest();

            if (id != recipeDTO.Id)
            {
                return BadRequest();
            }

            Recipe recipe = mapper.Map<Recipe>(recipeDTO);
            recipe.Id = recipeDTO.Id; //Indien Id Werd ignored in de mapper

            bool exists = await recipeRepo.Exists(recipe, recipe.Id); //asnotracking!
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //2. try update
            try
            {
                var result = await recipeRepo.Update(recipe); //new  in repo (niet meer generic)
                if (result == null)
                {
                    return BadRequest("Onbestaande categorie");
                }
                //await recipeRepo.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!recipeRepo.Exists(recipe, recipe.Id).Result)
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
            var recipes = (await recipeRepo.GetByExpressionAsync(r => r.RecipeName.Contains(name)));
            if (recipes == null || recipes.Count() == 0)
            {
                return NotFound();
            }
            if (!ModelState.IsValid) return BadRequest(ModelState);

            //2. RecipeDTO ophalen
            Recipe recipe = recipes.First();
            var recipeDTOToPatch = mapper.Map<RecipeDTO>(recipe); //map naar DTO
                                                                  //TODO: recipe.Category niet voorzien in Mapper (optioneel)
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
                //TODO:Recipe- exceptie moet logger worden + error controller redirect
                throw new Exception($"Patchupdate  of {recipe.RecipeName} failed. {exc.InnerException.Message}");
            }

            return NoContent();
        }

        // POST: api/Recipes
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<RecipeDTO>> PostRecipe([FromBody] [Bind("RecipeName,RecipeDescription,CategoryID,Category,CategoryName,TimeToPrepare,Vegetarian")]
        RecipeDTO  recipeDTO)
        {
            if (recipeDTO == null)
            {
                return BadRequest(new { Message = "Geen gerechten input" });
            };

            var recipe = mapper.Map<Recipe>(recipeDTO);
            if (recipe == null)
            {
                return BadRequest(new { Message = "Onvoldoende data." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                //if (!String.IsNullOrWhiteSpace(recipe.Category.CategoryName))
                //{
                //    recipe.Category = new Category();
                //    //recipe.Category.CategoryID = 0; //database maakt identity aan
                //    recipe.Category.CategoryName = formCollection["Category.CategoryName"];
                //}

                await recipeRepo.AddRecipeWithCategory(recipe);
                return CreatedAtAction("GetRecipe", new { id = recipe.Id }, mapper.Map<RecipeDTO>(recipe));
            }
            catch (DbUpdateException)
            {
                if (recipeRepo.Exists(recipe, recipe.Id).Result)
                {
                    return Conflict();
                }
                else
                {
                    return RedirectToAction("HandleErrorCode", "Error", new
                    {
                        statusCode = 400,
                        errorMessage = $"Het bewaren van gerecht '{recipe.RecipeName}' is mislukt."
                    });
                }
            }
        }

        // DELETE: api/Recipes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RecipeDTO>> DeleteRecipe(Guid id)
        {
            //1. Checks
            var recipes = await recipeRepo.GetByExpressionAsync(c => c.Id == id);
            if (recipes == null || recipes.Count() == 0)
            {
                return NotFound(new { message = "Gerecht niet gevonden." });
            }

            Recipe recipe = recipes.FirstOrDefault<Recipe>();
            //2. try/catch op de actie
            try
            {
                await recipeRepo.Delete(recipe);
            }
            catch
            {
                return RedirectToAction("HandleErrorCode", "Error", new
                {
                    statusCode = 400,
                    errorMessage = $"Het verwijderen van gerecht '{recipe.RecipeName}' is mislukt."
                });
            }
            return Ok(mapper.Map<RecipeDTO>(recipe));
        }
    }
}
