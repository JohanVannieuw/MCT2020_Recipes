using RestaurantServices.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantServices.Repositories
{
    public interface IRestaurantRepo
    {
        Task<bool> CollectionExistsAsync(string restaurantName);
        Task<Restaurant> CreateAsync(Restaurant restaurant);
        Task<Restaurant> Get(string id);
        Task<IEnumerable<Restaurant>> GetAll();
        Task<IEnumerable<Restaurant>> GetRestaurantsByName(string name);
        Task<IEnumerable<Review>> GetReviewsForRestaurant(string id);
        Task<string> RemoveAsync(string id);
        Task<Restaurant> ReplaceAsync(string id, Restaurant restaurant);
        Task<Restaurant> UpsertAsync(Restaurant restaurant);
    }
}