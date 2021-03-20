using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AppReservation.ViewModels
{
    public class ResStudentViewModel
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public int ResCount { get; set; }

        [DisplayName("Reservation Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public string Status { get; set; }

        public string Cause { get; set; }

        [DisplayName("Reservation Type")]
        public string Name { get; set; }

        [DisplayName("Reservation Type Id")]
        public int ReservationTypeId { get; set; }
        //public int AccessNumber { get; set; }
        [DisplayName("Student")]
        public string StudentId { get; set; }
        public DateTime CreateDate { get; set; }
        public IdentityUser Student { get; set; }
    }
}
