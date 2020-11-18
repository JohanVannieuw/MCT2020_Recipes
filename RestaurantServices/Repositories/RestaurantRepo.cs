using MongoDB.Driver;
using RestaurantServices.Data;
using RestaurantServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantServices.Repositories
{
    public class RestaurantRepo : IRestaurantRepo
    {
        private readonly RestaurantServicesContext context;

        public RestaurantRepo(RestaurantServicesContext context)
        {
            this.context = context;
        }


        //READ --------------------------
        public async Task<IEnumerable<Restaurant>> GetAll()
        {
            try
            {

                //1. docs ophalen  (case sensitive!!!) 
                IMongoCollection<Restaurant> collection =
                 context.Database.GetCollection<Restaurant>("restaurants");
                //context.Restaurants

                //2. docs bevragen (Mongo query) en returnen
                //noot: alle mongo methodes bestaan synchroon en asynchroon
                var result = await
                 collection.Find(FilterDefinition<Restaurant>.Empty).SortBy(r => r.Name).ToListAsync<Restaurant>();
                //3. Return query resultaat
                return result;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public async Task<IEnumerable<Review>> GetReviewsForRestaurant(string id)
        {
            var objId = new Guid(id);
            var reviews = await context.Reviews.Find(b => b.RestaurantID == objId).ToListAsync<Review>();
            return reviews;
        }

        //CREATE -----------------------------
        public async Task<Restaurant> CreateAsync(Restaurant restaurant)
        {
            //Gebruik van context acties op de IMongoCollecties
            await context.Restaurants.InsertOneAsync(restaurant);
            return restaurant;
        }



        //UPDATE-------------------------------



        //DELETE----------------------------



        //Helpers ------------------------------------------- 

        public async Task<bool> CollectionExistsAsync(string restaurantName)
        {
            var restaurant = await context.Restaurants.Find(r => r.Name == restaurantName).FirstOrDefaultAsync<Restaurant>();
            return restaurant != null;

        }



    }
}
