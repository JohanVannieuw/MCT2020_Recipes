using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using RestaurantServices.Models;
using RestaurantServices.Repositories;

namespace RestaurantServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : ControllerBase
    {
        private readonly IRestaurantRepo restaurantRepo;

        public RestaurantsController(IRestaurantRepo restaurantRepo)
        {
            this.restaurantRepo = restaurantRepo;
        }

        [HttpGet]
        [Route("/api/Restaurants")]
        public async Task<ActionResult<IEnumerable<Restaurant>>> GetRestaurants()
        {
            var restos = await restaurantRepo.GetAll();
            return Ok(restos);
        }

    }
}