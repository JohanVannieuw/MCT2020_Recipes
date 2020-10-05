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


    }
}
