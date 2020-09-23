using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes_DB.Models
{
    public class CategoryDTO
    {
       [Key]
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public string[] RecipeNames { get; set; } = new string[0];
    }

}
