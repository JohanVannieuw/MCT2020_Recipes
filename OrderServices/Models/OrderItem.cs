using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderServices.Models
{
    public class OrderItem
    {
        public Guid OrderItemId { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; private set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; }


    }
}
