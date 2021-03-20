using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppReservation.Models
{
    public class Student : IdentityUser
    {
        public string FullName { get; set; }
        public string classe { get; set; }
        public int ResCount { get; set; }
    }
}
