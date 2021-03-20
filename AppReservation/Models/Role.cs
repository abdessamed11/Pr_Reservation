using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppReservation.Models
{
    public class Role
    {
        [Required(ErrorMessage = "The role name field is required !")]
        [Display(Name = "Enter The Name")]
        
        public string RoleName { get; set; }
    }
}
