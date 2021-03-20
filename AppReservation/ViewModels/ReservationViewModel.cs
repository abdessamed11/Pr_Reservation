using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppReservation.ViewModels
{
    public class ReservationViewModel
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; }
        public string Cause { get; set; }

    }
}
