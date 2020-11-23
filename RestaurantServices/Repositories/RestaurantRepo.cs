using MongoDB.Bson;
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
                IEnumerable<Restaurant> result = await
                 collection.Find(FilterDefinition<Restaurant>.Empty).SortBy(r => r.Name).ToListAsync<Restaurant>();
                //var result = await context.Restaurants.Find(_ => true).ToListAsync<Restaurant>();

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

        public async Task<Restaurant> Get(string id)
        {
            //zoek zowel op het BsonId als het RestaurantId (case sensitive)
            ObjectId bsonId = (!ObjectId.TryParse(id, out bsonId)) ? ObjectId.Empty : ObjectId.Parse(id);
            //guid convertie returnt lower chars!!! Guids met hoofdletters worden hierdoor niet gevonden.      
            Guid restaurantId = (!Guid.TryParse(id, out restaurantId)) ? Guid.Empty : Guid.Parse(id);

            var query = context.Restaurants.Find(r => r.RestaurantId == restaurantId || r.Id == bsonId.ToString()); //cursor
            Restaurant restoEntity = await query.FirstOrDefaultAsync<Restaurant>();
            return restoEntity;
        }

        public async Task<IEnumerable<Restaurant>> GetRestaurantsByName(string name)
        {
            var query = context.Restaurants.Find(r => r.Name.ToLower().Contains(name.ToLower()));
            IEnumerable<Restaurant> restoEntities = await query.ToListAsync<Restaurant>();
            return restoEntities;
        }

        //CREATE -----------------------------
        public async Task<Restaurant> CreateAsync(Restaurant restaurant)
        {
            //Gebruik van context acties op de IMongoCollecties
            await context.Restaurants.InsertOneAsync(restaurant);
            return restaurant;
        }

        //UPDATE -------------------------------
        public async Task<Restaurant> UpsertAsync(Restaurant restaurant)
        {
            //upsert = aanmaken indien onbestaand.
            //bijna alle lambda methodes hebben als arg een "options" parameter.
            ReplaceOptions options = new ReplaceOptions { IsUpsert = true }; //upsert
            await context.Restaurants.ReplaceOneAsync<Restaurant>(r => r.RestaurantId == restaurant.RestaurantId, restaurant, options);
            //var restaurantConfirmed = Get(restaurant.RestaurantId.ToString()).Result;
            return restaurant;
        }

        //UPDATE -------------------------------------------------------------
        public async Task<Restaurant> ReplaceAsync(string id, Restaurant restaurant)
        {
            //gebruikt ReplaceOneAsync als variante op UpdateOneAsync()            
            await context.Restaurants.ReplaceOneAsync(r => r.RestaurantId == restaurant.RestaurantId, restaurant);
            //var restaurantConfirmed = Get(restaurant.RestaurantId).Result;
            return restaurant;
        }

        //HARD DELETE----------------------------
        public async Task<string> RemoveAsync(string id)
        {
            ObjectId bsonId = (!ObjectId.TryParse(id, out bsonId)) ? ObjectId.Empty : ObjectId.Parse(id);    
            Guid restaurantId = (!Guid.TryParse(id, out restaurantId)) ? Guid.Empty : Guid.Parse(id);

            await context.Restaurants.DeleteOneAsync(r => r.RestaurantId == restaurantId || r.Id == bsonId.ToString());
            return id;
        }

        //Helpers ------------------------------------------- 

        public async Task<bool> CollectionExistsAsync(string restaurantName)
        {
            var restaurant = await context.Restaurants.Find(r => r.Name == restaurantName).FirstOrDefaultAsync<Restaurant>();
            return restaurant != null;

        }

    }
}
