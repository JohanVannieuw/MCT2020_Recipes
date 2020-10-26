using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderServices.Services
{
    public class CartModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        // virtuele collectie in een oneToMany relatie  
        public ICollection<CartItem> CartItems { get; set; }
    }

    public class CartItem
    {

        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal ItemPrice { get; set; }
        public int Quantity { get; set; }

       //public string PictureUrl { get; set; }

        //een virtuele collectie voor relatie aanmaak
        //public virtual CartModel Cart { get; set; }
    }
}
