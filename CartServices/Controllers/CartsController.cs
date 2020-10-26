using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CartServices.Data;
using CartServices.Models;
using CartServices.Repositories;
using Microsoft.AspNetCore.Authorization;
using CartServices.Messaging;

namespace CartServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes("application/json", "application/json-path+json", "multipart/form-data", "application/form-data")]
    [Produces("application/json")]
    public class CartsController : ControllerBase
    {
             private readonly ICartRepo cartRepo;
        private readonly ICartSender cartSender;
        private readonly Guid TESTUSERID = ModelBuilderExtensions.TESTUSERID ; //enkel in development

        public CartsController(CartServicesContext context, ICartRepo cartItemRepo, ICartSender cartSender)
        {

            this.cartRepo = cartItemRepo;
            this.cartSender = cartSender;
        }

        //[Authorize]
        [HttpGet(Name="GetCart")]
        public async Task<IActionResult> GetCart([FromQuery(Name = "u")] Guid userId){
            //TODO: TESTUSERID overbrengen naar unittest
            userId = TESTUSERID;  //test value enkel in development 
            if (userId == null || userId == Guid.Empty)
            {                
                return BadRequest(new { Message = $"User {userId} niet ingevuld." });
            }

            var cartItems = await cartRepo.GetCartItems(userId); //async!
            return Ok(cartItems);
        }


        // PUT: api/Carts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        // PUT: api/Cart/5
        [HttpPut]
        public IActionResult Put([FromQuery(Name = "u")] Guid userId, [FromBody] CartItem cartItem)
        {
            cartRepo.UpdateCartItem(userId, cartItem);
            return new OkResult();
        }

        // POST: api/Carts
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Post(
            [Bind("userId,ItemName, ItemPrice,Quantity,CartId")]
            [FromQuery(Name = "u")] Guid userId, [FromBody] CartItem cartItem)
        {
            userId = TESTUSERID;
            if (userId == null)
            {             
                return BadRequest(new { Message = $"User {userId} niet ingevuld." });             }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }
            //TODO: POST error checks (vb. geen bind op de price, wat alleen voor admin is )

            CartItem cartItemResult = await cartRepo.InsertCartItem(userId, cartItem);
            return Created("GetCard",cartItemResult);
        }

        ////// POST: api/carts/publish
        [HttpPost("publish/")]
        public async Task<ActionResult<Cart>> PublishCart([FromBody] Cart cart)
        {
            try
            {
                if (cart.CartItems.Count() == 0)
                {
                    return BadRequest($"Je moet minstens één item bestellen in je shoppingcart {cart.Id}");
                }

                cartSender.SendCart(cart);
                return new OkResult();
            }
            catch (Exception ex)
            {
                var innerexc = "  " + ex.InnerException.InnerException.Message;
                return BadRequest(ex.Message + innerexc);
            }
        }
        // DELETE: api/Carts/5
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public IActionResult Delete([FromQuery(Name = "u")] Guid userId, [FromQuery(Name = "ci")] Guid cartItemId)
        {
            cartRepo.DeleteCartItem(userId, cartItemId);
            return new OkResult();
        }

    }
}