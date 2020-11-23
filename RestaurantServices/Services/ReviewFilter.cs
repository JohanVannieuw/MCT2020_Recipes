using MongoDB.Driver;
using RestaurantServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantServices.Services
{
    public class ReviewFilter
    {
        //velden (querystring) waarop kan gefilterd worden. 
        public string RestaurantId { get; set; }
        public string Subject { get; set; }

        public DateTime? DateOfCreation { get; set; }  //nullable

        //opbouwen van een chain met MongoDB.Driver.FilterDefinition
        public FilterDefinition<Review> ToFilterDefinition()
        {
            var filterDefinition = Builders<Review>.Filter.Empty;
            if (RestaurantId != null && RestaurantId !="")
            {//casesensitive!
                filterDefinition &=
                Builders<Review>.Filter.Where(d => d.RestaurantID ==
                                             Guid.Parse(RestaurantId));
            }

            if (Subject != "" && Subject != null)
            {
                filterDefinition &= Builders<Review>.Filter.Where(d =>
                           d.Subject.ToLower().Contains(Subject.ToLower()));
            }

            if (DateOfCreation.HasValue)
            {
                filterDefinition &= Builders<Review>.Filter
                  .Where(d => d.DateOfCreation >=
                       DateOfCreation.Value.ToUniversalTime());//Value, Timezone 
            }

            return filterDefinition;
        }

    }
}
