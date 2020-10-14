using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServices.Models
{
    public class Role: IdentityRole<string>
    {
        public Role() : base() { }
        public Role(string name) : base(name) { }  //voor aanmaken op naam in Seeder
        public string Description { get; set; }
        public override string Id { get; set; } = Guid.NewGuid().ToString();

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    }
}
