using Recipes_DB.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Recipes_DB.Repositories
{
    public interface IRecipeRepo : IGenericRepo<Recipe>
    {
        Task<int> CountRecipes();
        new Task<IEnumerable<Recipe>> GetAllAsync();
    }
}