using MongoDB.Driver;
using RestaurantServices.Data;
using RestaurantServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantServices.Repositories
{
    public class ReviewRepo : IReviewRepo
    {
        private readonly RestaurantServicesContext context;

        public ReviewRepo(RestaurantServicesContext context)
        {
            this.context = context;
        }


        //** GET ---------------------------

        public async Task<IEnumerable<Review>> GetAll()
        {
            IMongoCollection<Review> collection = context.Database.GetCollection<Review>("reviews");
            var result = await collection.Find(FilterDefinition<Review>.Empty).ToListAsync<Review>();
            return result;
        }

        //CREATE ------

        public async Task<Review> CreateAsync(Review review)
        {
            await context.Reviews.InsertOneAsync(review);
            return review;
        }

        public Task<IEnumerable<Review>> GetReviewsForRestaurant(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetReviewsGroupedBySubject()
        {
            throw new NotImplementedException();
        }

        //UPDATE ------- 
        public Guid Update(Guid restaurantId, int newQuotation)
        {
            throw new NotImplementedException();
        }

        //HARD DELETE ------



    }
}
