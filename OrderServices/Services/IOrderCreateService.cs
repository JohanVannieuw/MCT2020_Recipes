using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderServices.Services
{
    public interface IOrderCreateService
    {
        void CreateOrder(CartModel cartModel);
    }
}
