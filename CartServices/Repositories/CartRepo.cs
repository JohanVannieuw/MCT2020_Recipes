using CartServices.Data;
using CartServices.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CartServices.Repositories
{
    public class CartRepo : GenericRepo<Cart>, ICartRepo
    {

        public CartRepo(CartServicesContext _context) : base(_context)
        {

        }

        public Task DeleteCartItem(Guid userId, Guid cartItemId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CartItem>> GetCartItems(Guid userId)
        {
            Cart cart = await _context.Carts
                .Include(c => c.CartItems)
                .AsNoTracking()
                .Where(c => c.UserId == userId).FirstOrDefaultAsync();


            if (cart != null)
            {
                return cart.CartItems;
            }
            else
            {
                return new List<CartItem>();
                //return await Task.Run(()=> new List<CartItem>());            
            }
        }

        public async Task<CartItem> InsertCartItem(Guid userId, CartItem cartItem)
        {
            {
               //Doel: historiek van elke transactie bijhouden in tussentabel CartItems
                try
                {
                    if (cartItem.Cart != null)
                    {
                        //1. Cart (FirstOrDefault returnt een null indien onbestaand) 
                        Cart existsCart = _context.Carts.Where(c => c.Id == cartItem.Cart.Id).FirstOrDefault();
                        if (existsCart == null)
                        {
                            _context.Entry<Cart>(cartItem.Cart).State = EntityState.Added;
                        }
                        else
                        {
                            _context.Entry<Cart>(cartItem.Cart).State = EntityState.Detached;
                        }

                        //2. CartItem (tussentabel -> die op (unieke) naam werkt)                        
                        var existsItem = _context.CartItems.FirstOrDefault(ci => ci.ItemName == cartItem.ItemName&& ci.CartId == cartItem.Cart.Id);
                        if (existsItem == null)
                        {
                            _context.Entry<CartItem>(cartItem).State = EntityState.Added;
                        }
                        else
                        {
                            //TODO:afspreken met front -> add of update bij wijziging? 
                                cartItem.Quantity += existsItem.Quantity;
                            _context.Entry<CartItem>(cartItem).State = EntityState.Added;
                        }
                    }
                    else
                    {
                        throw new Exception($"Shoppingkar {nameof(cartItem)} kan niet worden opgenomen.");
                    }

                    //Noot: await _context.AddAsync(cartItem); doet een SaveAsync
                    await _context.SaveChangesAsync();
                    return cartItem;
                }
                catch (Exception exc)
                {
                    ////TODO: foutboodschappen afwerken voor InsertCartItem
                    throw new Exception($"Shoppingkar {nameof(cartItem)} kan niet worden opgenomen. {exc.Message }");
                }
            }
        }

        public Task UpdateCartItem(Guid userId, CartItem cartItem)
        {
            throw new NotImplementedException();
        }
    }
}
