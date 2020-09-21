using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recipes_DB.Models
{
    public partial class Recipe
    {
        public Guid Id { get; set; }


        [Column("Name")]
        public string RecipeName { get; set; }
        public string RecipeDescription { get; set; }
        public int CategoryId { get; set; }


        [DisplayFormat(NullDisplayText = "Nog geenland of oorsrong gespecifieerd.")]
        public string CountryOfOrigin { get; set; }

        [DataType(System.ComponentModel.DataAnnotations.DataType.Time),
DisplayFormat(DataFormatString = @"{0:hh\:mm}",
ApplyFormatInEditMode = true)]
        [DisplayName("TimeToPrepare(hh/mm)")]
        public TimeSpan TimeToPrepare { get; set; }

        public bool? Vegetarian { get; set; }




        public virtual Category Category { get; set; }
    }
}
