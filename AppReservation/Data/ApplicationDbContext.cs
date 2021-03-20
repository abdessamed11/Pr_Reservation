using AppReservation.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AppReservation.ViewModels;

namespace AppReservation.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Reservation> Reservations { get; set; }
        public virtual DbSet<TypeReservation> TypeReservations { get; set; }



    }
}
