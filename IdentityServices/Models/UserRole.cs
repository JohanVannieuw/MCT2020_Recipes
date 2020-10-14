using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServices.Models
{
    public class UserRole : IdentityUserRole<string>
    {
        public DateTime DateOfentry { get; set; } = DateTime.Now;

        //navigatie properties - many to many --------------------
        public virtual Role Role { get; set; }
        public virtual User User { get; set; }

    }
}
