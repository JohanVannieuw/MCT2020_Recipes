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
            CreateMap<Recipe, RecipeDTO>() //Map van Recipe naar
.ForMember(dest => dest.CategoryName, opt => opt.MapFrom(r => r.Category.CategoryName)) //simplified one to many
.ForMember(dest => dest.TotalSecondsToPrepare, opt => opt.MapFrom(r => r.TimeToPrepare.TotalSeconds))
 //.ForMember(dest => dest.Restaurants, opt => opt.MapFrom(rr => rr.RestaurantRecipes.Select(r => r.Restaurant).ToList()))
 .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)) //|| ((ICollection<Recipe>) srcMember).Count > 0
;                                                                                                                             
            //mapping in dest kan enkel vanuit het toplevel (niet op props van de related ojects)
            CreateMap<RecipeDTO, Recipe>() //Map van RecipeDTO naar Recipe
                                           //.ForMember(dest=>dest.Id, src=> src.Ignore()) //dan wel zelf mappen waar nodig (edit)
              .ForMember(dest => dest.Category, opt => opt.MapFrom(model => new Category() { CategoryName = model.CategoryName }))
              .ForMember(dest => dest.TimeToPrepare, opt => opt.MapFrom(r => TimeSpan.FromSeconds(r.TotalSecondsToPrepare)));

        }
    }
}