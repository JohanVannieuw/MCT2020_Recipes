using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OrderServices.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid OrderId { get; set; }

        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderState OrderState { get; set; }
        // public decimal OrderTotal { get; set; }
        public IEnumerable<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public enum OrderState
    {
        Preparing = 1,
        Shipped = 2,
        Delivered = 3,
    }
}
