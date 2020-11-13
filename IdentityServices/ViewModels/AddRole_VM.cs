using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServices.ViewModels
{
    public class AddRole_VM
    {
        [Required]
        [Display(Name = "Role name")]
        public string RoleName { get; set; }
    }
}