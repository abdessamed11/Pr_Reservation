using AppReservation.Data;
using AppReservation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppReservation.ViewModels;

namespace AppReservation.Controllers
{
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ReservationController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: ReservationController
        public IActionResult Index()
        {
            var list = (_context.Reservations.Include(s => s.Student).Include(rt => rt.Reserv)).ToList();
            //ViewBag.role = new IdentityRole();
            return View(list);
        }

        // GET: ReservationController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ReservationController/Create
        public ActionResult Create()
        {
            var list = _context.TypeReservations;
            ViewBag.types = list.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservation reservation)
        {

            if (ModelState.IsValid)
            {
                var type = _context.TypeReservations.Where(r => r.Id == reservation.ReservId).FirstOrDefault();
                var student = await _userManager.GetUserAsync(HttpContext.User);

                var reser = new Reservation();
                reser.Status = reservation.Status;
                reser.Date = reservation.Date;
                reser.Cause = reservation.Cause;
                reser.StudentId = student.Id;
                reser.ReservId = type.Id;


                _context.Add(reser);

                await _context.SaveChangesAsync();
                return RedirectToAction("index");
            }

            return View(reservation);
        }

        
        public ActionResult Edit(int? id)
        {
            var getid = _context.Reservations.Find(id);
            ViewBag.gettype = _context.TypeReservations.ToList();
            return View(getid);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Reservation reservation)
        {

            if (ModelState.IsValid)
            {
                var type = _context.TypeReservations.Where(r => r.Id == reservation.ReservId).FirstOrDefault();
                //reservation.ReservationType.Id = type.ToString();
                var student = await _userManager.GetUserAsync(HttpContext.User);
                //var studentId = student.Id;


                reservation.StudentId = student.Id;
                reservation.ReservId = type.Id;

                _context.Update(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction("index");
            }

            return View(reservation);
        }

        public IActionResult Delete(int? id)
        {
            var list = _context.Reservations.Include(s => s.Student).Include(rt => rt.Reserv);
            ViewBag.data = list.AsEnumerable();
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var del = _context.Reservations.Find(id);
            return View(del);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {

            var del = _context.Reservations.Find(id);
            _context.Reservations.Remove(del);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
