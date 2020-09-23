using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Recipes_DB.Models
{
    public class Recipes_DBProfiles : Profile
    {
        public Recipes_DBProfiles()
        {
            InitCategoryMapper();
        }

        private void InitCategoryMapper()
        {
            //CATEGORY Mapping:------------------------------------------------
            //--- relaties mappen naar vlakke structuur
            //--- Identity Column niet meenemen
            CreateMap<Category, CategoryDTO>()
            .ForMember(dest => dest.RecipeNames, src => src.MapFrom(src => src.Recipes.Select(rec => rec.RecipeName)))
            .ReverseMap();
             

            CreateMap<Category, CategoryEditCreateDTO>()
                .ReverseMap()
                .ForMember(dest => dest.Id, src => src.Ignore())
            .ForMember(dest => dest.Recipes, src => src.Ignore());

            //RECIPE Mapping:---------------------------------------------------
            //TODO: Automapper aanvullen voor Recipe 


        }
    }
}