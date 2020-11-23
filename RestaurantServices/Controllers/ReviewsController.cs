using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantServices.Repositories;
using RestaurantServices.Services;

namespace RestaurantServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewRepo reviewRepo;

        public ReviewsController(IReviewRepo reviewRepo)
        {
            this.reviewRepo = reviewRepo;
        }


        public async Task<IActionResult> Index([FromQuery(Name = "ReviewFilter")] ReviewFilter reviewFilter = null)
        {
            var filter = new ReviewFilter()
            {
                Subject = reviewFilter.Subject,
                RestaurantId = reviewFilter.RestaurantId,
                DateOfCreation = reviewFilter.DateOfCreation
            };

            return Ok(await reviewRepo.GetAll(filter));
        } 

        [Route("ReviewsGroupedBySubject")]
        [HttpGet]
        public IActionResult ReviewsBySubject() {
            var detailsGrouped = reviewRepo.GetReviewsGroupedBySubject();
            return Ok(detailsGrouped);
        
        }

    }
}