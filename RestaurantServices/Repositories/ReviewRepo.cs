using MongoDB.Driver;
using RestaurantServices.Data;
using RestaurantServices.Models;
using RestaurantServices.Services;
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
        public async Task<IEnumerable<Review>> GetAll(ReviewFilter filter = null)
        {
            try
            {

                if (filter == null || filter.RestaurantId == null && filter.Subject == null &&
                     filter.DateOfCreation == null)
                {
                    var result = await context.Reviews.Find(FilterDefinition<Review>.Empty).ToListAsync<Review>();

                    return result;
                }
                else
                {
                    return await context.Reviews.Find(filter.ToFilterDefinition())
                                  .SortBy(d => d.DateOfCreation).ThenBy(d => d.Subject)
                                  .ToListAsync<Review>();
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public IEnumerable<Object> GetReviewsGroupedBySubject()
        {
            IMongoCollection<Review> collection = context.Reviews;

            //Kan met de fluent Aggregate functie of LINQ
            var result = collection.AsQueryable() //Met LINQ bevraagbaar maken
                  .Select(r => new { r.Subject })
                  .GroupBy(r => r.Subject) //groepering (volgens de Key Subject) .
                                           //anoniem object bepaalt wat je wenst weer te geven
                                           //( hier: aantal per subject)
                .Select(group => new { SubjectRange = group.Key, Count = group.Count() })
                .OrderBy(b => b.SubjectRange)
                .ToList();
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


        //UPDATE ------- 
        public Guid Update(Guid restaurantId, int newQuotation)
        {
            throw new NotImplementedException();
        }

        //HARD DELETE ------



    }
}
