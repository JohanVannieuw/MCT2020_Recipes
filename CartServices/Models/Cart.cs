using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CartServices.Models
{
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();
        //Cart eenduidig gelinkt met de user (besteller)
        [Required]
        public Guid UserId { get; set; } = Guid.NewGuid();
        // virtuele collectie in een oneToMany relatie  

        public DateTime DateOfEntry { get; set; } = DateTime.Now; //voor track en trace


        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();



    }
}
