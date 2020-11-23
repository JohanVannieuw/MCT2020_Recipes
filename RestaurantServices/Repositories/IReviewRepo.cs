using RestaurantServices.Models;
using RestaurantServices.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantServices.Repositories
{
    public interface IReviewRepo
    {
        Task<Review> CreateAsync(Review review);
        Task<IEnumerable<Review>> GetAll(ReviewFilter filter);
        Task<IEnumerable<Review>> GetReviewsForRestaurant(string id);
        IEnumerable<object> GetReviewsGroupedBySubject();
        Guid Update(Guid restaurantId, int newQuotation);
    }
}