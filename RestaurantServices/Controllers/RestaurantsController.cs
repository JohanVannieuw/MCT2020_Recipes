using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using RestaurantServices.Models;
using RestaurantServices.Repositories;

namespace RestaurantServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
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

        [HttpGet]
        [Route("/api/Restaurant/{id}")]
        public async Task<ActionResult<Restaurant>> GetRestaurant(string id)
        {
            if (id == null || id == "")
            {
                return BadRequest();
            }

            var resto = await restaurantRepo.Get(id);

            if (resto == null)
            {
                return NotFound();
            }

            // var restoDTO = mapper.Map<RestaurantDTO>(resto);
            return Ok(resto);
        }

        [HttpGet]
        [Route("/api/Restaurants/{name}")]
        public async Task<ActionResult<IEnumerable<Restaurant>>> GetRestaurantsByName(string name)
        {
            if (name == null || name == "")
            {
                return BadRequest();
            }

            var resto = await restaurantRepo.GetRestaurantsByName(name);

            if (resto == null)
            {
                return NotFound();
            }

            // var restoDTO = mapper.Map<RestaurantDTO>(resto);
            return Ok(resto);
        }


        [HttpGet]
        [Route("/api/RestaurantsJoinedWithReviews")]
        public IActionResult RestaurantJoinedWithReviews() {
            return Ok(restaurantRepo.RestaurantJoinedWithReviews());       
        }


        // PUT ------------------------------------------------------------------------------------------
        [HttpPut("{restoId}")]
        public async Task<IActionResult> PutRestaurant(string restoId, Restaurant restaurant)
        {
            //1. checks : null , ids, exists, valid
            // correcte JsonPropertyNames en datatype 
            if (restaurant == null || restoId == null) return BadRequest();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (Guid.Parse(restoId) != restaurant.RestaurantId)
            {
                return BadRequest();
            }

            //restaurant ophalen (op ObjecId of RestaurantId)
            if (restaurantRepo.Get(restoId) == null)
            {
                return NotFound("Restaurant bestaat niet.");
            }

            //ObjectId verplicht in Mongo(invullen als zekerheid)
            Restaurant restoToUpdate = await restaurantRepo.Get(restaurant.RestaurantId.ToString());
            if (restoToUpdate != null)
            {
                restaurant.Id = restoToUpdate.Id;  //bestaat niet -> upsert maakt het aan.
            }
            else
            {
                restaurant.Id = ObjectId.GenerateNewId().ToString(); //vervangt 00000 empty objectId
            }

            ////Indien een mapper 
            // Restaurant resto = mapper.Map<Restaurant>(restaurantDTO);

            //2. try update
            try
            {
                var result = await restaurantRepo.UpsertAsync(restaurant); //new  in repo (niet meer generic)
                if (result == null)
                {
                    return BadRequest("Onbestaand gerecht");
                }
            }
            catch (Exception exc)
            {
                throw new Exception(exc.Message);
            }
            return NoContent();
        }

        // POST -----------------------------------------------------------------------------------------
        [HttpPost()]
        public async Task<ActionResult<Restaurant>> PostRestaurant([FromBody] Restaurant restaurant)
        {
            if (restaurant == null)
            {
                return BadRequest(new { Message = "Geen restaurant input" });
            }

            //eventueel RestaurantDTO
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await restaurantRepo.CreateAsync(restaurant);
                return CreatedAtAction("GetRestaurant", new { id = restaurant.RestaurantId }, restaurant); ;
            }
            catch (Exception exc)
            {
                throw exc;
            }

        }
    }
}