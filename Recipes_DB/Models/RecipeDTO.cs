using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes_DB.Models
{
    public class RecipeDTO
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string RecipeName { get; set; }
        public string RecipeDescription { get; set; }
        public double TotalSecondsToPrepare { get; set; }
        public bool? Vegetarian { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string UserInfo { get; set; } = null;

        //vlakke structuur ( navigatie property),  geen weergave indien null
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CategoryName { get; set; }

   
    }
}
