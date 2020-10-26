using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OrderServices.Services
{
    public class OrderCreateService : IOrderCreateService
    {
        public OrderCreateService()
        {
        }

        public void CreateOrder(CartModel cartModel)
        {
            try
            {

            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
            }
        }
    }
}
