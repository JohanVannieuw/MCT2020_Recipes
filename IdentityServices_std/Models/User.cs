using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServices.Models
{
    public class User:IdentityUser
    {

        [Required]
        public string CardNumber { get; set; }

        // Niet <IdentityUserRole> !
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();




    }
}
