using Microsoft.EntityFrameworkCore;
using Recipes_DB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes_DB.Repositories
{
    public class RecipeRepo : GenericRepo<Recipe>, IRecipeRepo
    {
        private readonly Recipes_DB1Context context;

        public RecipeRepo(Recipes_DB1Context context) : base(context)
        {
            this.context = context;

        }

        public async Task<int> CountRecipes() => await context.Recipe.CountAsync();

        public new async Task<IEnumerable<Recipe>> GetAllAsync()
        {
            return await context.Recipe.Include(r => r.Category).ToListAsync();
        }

        public async Task<Recipe> AddRecipeWithCategory(Recipe recipe)
        //ook een nieuwe category wordt aanvaardt. Je informeert de user via een 
        // extra property: UserInfo
        {
            //Identity column in acht nemen
            if (recipe.Category.CategoryName != null)
            {
                //geen dubbels,  
                var exists = _context.Category.FirstOrDefault(cat => cat.CategoryName == recipe.Category.CategoryName);
                if (exists == null)
                {
                    _context.Entry<Category>(recipe.Category).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                    recipe.UserInfo = $"De categorie '{recipe.Category.CategoryName}' werd als nieuwe categorie toegevoegd({DateTime.Now})";
                }
                else
                    recipe.Category = _context.Category.First(c => c.CategoryName == recipe.Category.CategoryName);
            }
            else
            {
                //default state is EntityState.Unchanged
                //_context.Entry<Category>(recipe.Category).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                //om FK error te verhinderen:
                recipe.Category = _context.Category.First(c => c.Id == recipe.CategoryId);
            }

            await _context.AddAsync(recipe);
            await _context.SaveChangesAsync();
            return recipe;
        }

        public async Task<Recipe> Update(Recipe recipe)
        // //CategoryID dient te bestaan maar de Category kan niet worden gewijzigd in deze update
        {
            if (recipe.Category.CategoryName != null && recipe.CategoryId == 0)
            {
                var category = _context.Category.Where(c => c.CategoryName == recipe.Category.CategoryName).FirstOrDefault();

                if (category != null) { recipe.CategoryId = category.Id; recipe.Category = category; } else { return null; }
                _context.Entry<Category>(category).State = EntityState.Detached;

            }
            _context.Entry<Category>(recipe.Category).State = EntityState.Detached;//verhinder FK probleem
            _context.Entry<Recipe>(recipe).State = EntityState.Modified;

            _context.Update<Recipe>(recipe);
            await _context.SaveChangesAsync();
            return recipe;
        }
    }
}
