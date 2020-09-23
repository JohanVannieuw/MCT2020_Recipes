using AutoMapper;
using Recipes_DB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes_DB
{
    public class Recipes_DBProfiles: Profile
    {
        public Recipes_DBProfiles()
        {
            InitCategoryMapper();
        }

        private void InitCategoryMapper()
        {
            //relaties mappen naar vlakke structuur van Category naar DTO 
            CreateMap<Category, CategoryDTO>()
            .ForMember(dest => dest.RecipeNames,
            src => src.MapFrom(src => src.Recipes.Select(rec => rec.RecipeName)));
        }



    }
}
