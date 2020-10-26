using CartServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CartServices.Messaging
{
    public interface ICartSender
    {
        void SendCart(Cart cart);
    }
}
