using RestaurantServices.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantServices.Repositories
{
    public interface IReviewRepo
    {
        Task<Review> CreateAsync(Review review);
        Task<IEnumerable<Review>> GetAll();
        Task<IEnumerable<Review>> GetReviewsForRestaurant(string id);
        IEnumerable<object> GetReviewsGroupedBySubject();
        Guid Update(Guid restaurantId, int newQuotation);
    }
}