using CartServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CartServices.Repositories
{
    public interface ICartRepo: IGenericRepo<Cart>
    {
        Task<IEnumerable<CartItem>> GetCartItems(Guid userId);
        Task<CartItem> InsertCartItem(Guid userId, CartItem cartItem);
        Task UpdateCartItem(Guid userId, CartItem cartItem);
        Task DeleteCartItem(Guid userId, Guid cartItemId);

    }
}
