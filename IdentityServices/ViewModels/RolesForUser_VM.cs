using IdentityServices.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServices.ViewModels
{
    public class RolesForUser_VM
    {
        public User User { get; set; }
        public string UserId { get; set; }

        public string RoleId { get; set; }
        public IList<string> AssignedRoles { get; set; }
        public IList<string> UnAssignedRoles { get; set; }
    }
}