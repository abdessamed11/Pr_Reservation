using AppReservation.Data;
using AppReservation.Models;
using AppReservation.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AppReservation.Controllers
{
    public class StudentReservationController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly Reservation reservation;

        public StudentReservationController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        // GET: StudentReservationController
        public async Task <ActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var UserReservation = await _context.Reservations
                .Select(res => new ResStudentViewModel
                {
                    Id = res.Id,
                    StudentId = res.StudentId,
                    Date = res.Date,
                    Status = res.Status,
                    Cause = res.Cause,
                    ReservationTypeId = res.Reserv.Id,
                    Name = res.Reserv.name
                })
            .Where(res => res.StudentId == user.Id)
            .ToListAsync();
            return View(UserReservation);
        }

        // GET: StudentReservationController/Details/5
        public ActionResult Details(int id)
        {
            if (id < 1 )
            {
                return NotFound();
            }

            var reservation = _context.Reservations
                .First(m => m.Id == id);
                
            //ResStudentViewModel resStudentView = new ResStudentViewModel
            //{
            //    Id = reservation.Id,
            //    StudentId = reservation.StudentId,
            //    Date = reservation.Date,
            //    Status = reservation.Status,
            //    Cause = reservation.Cause,
            //    ReservationTypeId = reservation.Reserv.Id,
            //    Name = reservation.Reserv.name,
            //    Student = reservation.Student,
            //}

            //.Select(m => new ResStudentViewModel
            // {
            //     Id = m.Id,
            //     StudentId = m.StudentId,
            //     Date = m.Date,
            //     Status = m.Status,
            //     Cause = m.Cause,
            //     ReservationTypeId = m.Reserv.Id,
            //     Name = m.Reserv.name,
            //     Student = m.Student,
            //     //CreateDate = m.CreateDate,
            // });
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
            
            
        }

        // GET: StudentReservationController/Create
        public ActionResult Create()
        {
            var reservationType = _context.TypeReservations.Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.name
            });
            ViewBag.ResType = reservationType;
            return View();
        }

        // POST: StudentReservationController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <ActionResult> Create(ResStudentViewModel studentReservation)
        {
            if (ModelState.IsValid)
            {

                var IdUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var resType = _context.TypeReservations.Single(t => t.Id == studentReservation.ReservationTypeId);
                var reservation = new Reservation
                {

                    Date = studentReservation.Date,
                    Status = studentReservation.Status,
                    Cause = studentReservation.Cause,
                    StudentId = IdUser,
                    Reserv = resType

                };

                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                
           
            }
            return View(studentReservation);
        }



        // GET: StudentReservationController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (id < 1)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            return View(reservation);
        }

        // POST: StudentReservationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
                    {
                        return NotFound();
                    }
                    else 
                    {
                        throw;
                    }
                    
                }
                return RedirectToAction(nameof(Index));
            }
            return View(reservation);
        }

        // GET: StudentReservationController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            if (id < 1)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: StudentReservationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
