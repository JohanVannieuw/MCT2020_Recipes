using System;
using System.Collections.Generic;

namespace Recipes_DB.Models
{
    public partial class Category
    {
        public Category()
        {
            Recipes = new HashSet<Recipe>();
        }

        public int Id { get; set; }
        public string CategoryName { get; set; }

        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}
