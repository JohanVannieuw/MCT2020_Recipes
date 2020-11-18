using MongoDB.Driver;
using RestaurantServices.Models;
using RestaurantServices.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantServices.Data
{
    public class Seeder
    {
        private readonly IRestaurantRepo restaurantRepo;
        private readonly IReviewRepo reviewRepo;
        private readonly RestaurantServicesContext context;

        //Instantie oproepen vanuit Startup>> configure , met registratie in ConfigureServices.

        public List<Guid> Lst_RestaurantGuids { get; set; } = new List<Guid>();

        public Seeder(IRestaurantRepo restaurantRepo, IReviewRepo reviewRepo, RestaurantServicesContext context)
        {
            this.restaurantRepo = restaurantRepo;
            this.reviewRepo = reviewRepo;
            this.context = context;
        }

        public void initDatabase(int nmbrRestaurants = 2)
        {
            //geen data blijven toevoegen (MongoDB.Driver)
            try
            {
                //2. testRestaurants aanmaken
                for (var i = 0; i < nmbrRestaurants; i++)
                {

                    if (!restaurantRepo.CollectionExistsAsync("TestResto" + i).Result)
                    {

                        Guid currentId = Guid.NewGuid();
                        Lst_RestaurantGuids.Add(currentId);

                        restaurantRepo.CreateAsync(new Restaurant
                        {
                            RestaurantId = currentId,
                            Name = "TestResto" + i,
                            Description = "Description TestResto" + i,
                            Long = 51 + new Random().Next(10),
                            Lat = 51 + new Random().Next(10),
                            Street = "City TestResto" + i,
                            HouseNumber = i.ToString(),
                            Main_city_name = "City TestResto" + i,
                        });
                    }
                }


                //3.Reviews toevoegen
                reviewRepo.CreateAsync(new Review
                {
                    Id = new MongoDB.Bson.ObjectId(),
                    RestaurantID = Lst_RestaurantGuids[new Random().Next(Lst_RestaurantGuids.Count)],
                    Subject = "Pricing",
                    Comment = "Too expensive",
                    Quotation = 4.5M

                });
                reviewRepo.CreateAsync(new Review
                {
                    Id = new MongoDB.Bson.ObjectId(),
                    RestaurantID = Lst_RestaurantGuids[new Random().Next(Lst_RestaurantGuids.Count)],
                    Subject = "Location",
                    Comment = "Nice location in beautiful city.",
                    Quotation = 7.2M

                });

                reviewRepo.CreateAsync(new Review
                {
                    Id = new MongoDB.Bson.ObjectId(),
                    RestaurantID = Lst_RestaurantGuids[new Random().Next(Lst_RestaurantGuids.Count)],
                    Subject = "Service",
                    Comment = "Excellent",
                    Quotation = 8.0M
                });


                reviewRepo.CreateAsync(new Review
                {
                    Id = new MongoDB.Bson.ObjectId(),
                    RestaurantID = Lst_RestaurantGuids[new Random().Next(Lst_RestaurantGuids.Count)],
                    Subject = "Location",
                    Comment = "Difficult to find.",
                    Quotation = 5

                });

                reviewRepo.CreateAsync(new Review
                {
                    Id = new MongoDB.Bson.ObjectId(),
                    RestaurantID = Lst_RestaurantGuids[new Random().Next(Lst_RestaurantGuids.Count)],
                    Subject = "Location",
                    Comment = "Beautiful garden and sunny terrace.",
                    Quotation = 6

                });

                reviewRepo.CreateAsync(new Review
                {
                    Id = new MongoDB.Bson.ObjectId(),
                    RestaurantID = Lst_RestaurantGuids[new Random().Next(Lst_RestaurantGuids.Count)],
                    Subject = "Food",
                    Comment = "Excellent BBQ.",
                    Quotation = 8

                });

                //zoekindexen aanmaken op Mongo
                IndexKeysDefinition<Review> keys = "{ RestaurantID: 1 }";
                var indexModel = new CreateIndexModel<Review>(keys);
                context.Reviews.Indexes.CreateOneAsync(indexModel);

                IndexKeysDefinition<Restaurant> Restaurantkeys = "{ RestaurantID: 1 }";
                var indexModelComment = new CreateIndexModel<Restaurant>(Restaurantkeys);
                context.Restaurants.Indexes.CreateOneAsync(indexModelComment);

            }
            catch (Exception exc)
            {
                Console.WriteLine("fout bij het seeden:", exc);
            }
        }
    }
}
