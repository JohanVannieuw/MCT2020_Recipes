using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CartServices.Models
{
    public class CartItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ItemName { get; set; } //wordt Recipe (Name of Id) of iets anders
        public decimal ItemPrice { get; set; } //wordt RecipePrice of …
        public int Quantity { get; set; }

        public DateTime DateOfEntry { get; set; } = DateTime.Now; // voor track en trace


        //een virtuele collectie voor relatie aanmaak
        public Guid CartId { get; set; }
        public  virtual Cart Cart { get; set; } 

    }
}
